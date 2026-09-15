param(
    [Parameter(Mandatory = $true)]
    [string]$InputDocx,

    [Parameter(Mandatory = $true)]
    [string]$OutputDir,

    [switch]$UpdateFields
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Drawing
Add-Type -AssemblyName System.Windows.Forms
Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class ReportClipboardEmf {
    [DllImport("user32.dll", SetLastError=true)] public static extern bool OpenClipboard(IntPtr owner);
    [DllImport("user32.dll", SetLastError=true)] public static extern bool CloseClipboard();
    [DllImport("user32.dll", SetLastError=true)] public static extern IntPtr GetClipboardData(uint format);
    [DllImport("gdi32.dll", CharSet=CharSet.Unicode, SetLastError=true)] public static extern IntPtr CopyEnhMetaFile(IntPtr source, string fileName);
    [DllImport("gdi32.dll")] public static extern bool DeleteEnhMetaFile(IntPtr handle);
}
'@

$inputPath = (Resolve-Path -LiteralPath $InputDocx).Path
$outputPath = [System.IO.Path]::GetFullPath($OutputDir)

if (Test-Path -LiteralPath $outputPath) {
    $existing = Get-ChildItem -LiteralPath $outputPath -Force
    if ($existing.Count -gt 0) {
        throw "OutputDir must be new or empty: $outputPath"
    }
} else {
    New-Item -ItemType Directory -Path $outputPath | Out-Null
}

$emfDir = Join-Path $outputPath 'emf'
New-Item -ItemType Directory -Path $emfDir | Out-Null

$word = $null
$document = $null
try {
    $word = New-Object -ComObject Word.Application
    $word.Visible = $false
    $word.DisplayAlerts = 0
    $word.AutomationSecurity = 3

    Write-Output 'Opening document in Microsoft Word...'
    $document = $word.Documents.Open($inputPath, $false, (-not $UpdateFields.IsPresent))
    Write-Output 'Document opened.'

    if ($UpdateFields.IsPresent) {
        # Do not rely only on w:updateFields=true: Word can display a refreshed
        # TOC while leaving the generated placeholder in the saved field cache.
        # Updating each dynamic collection explicitly makes the DOCX portable
        # to previewers that never recalculate Word fields.
        Write-Output 'Updating and persisting dynamic fields...'
        $document.Repaginate()
        for ($i = 1; $i -le $document.TablesOfContents.Count; $i++) {
            $document.TablesOfContents.Item($i).Update()
        }
        for ($i = 1; $i -le $document.TablesOfFigures.Count; $i++) {
            $document.TablesOfFigures.Item($i).Update()
        }
        $null = $document.Content.Fields.Update()

        # Only the primary header of the main section contains a PAGE field.
        # Do not iterate through every header/footer variant: touching an
        # inactive story makes Word persist empty header/footer parts on the
        # cover and front-matter sections.
        $mainSection = $document.Sections.Item($document.Sections.Count)
        $primaryHeader = $mainSection.Headers.Item(1) # wdHeaderFooterPrimary
        if ($primaryHeader.Exists -and $primaryHeader.Range.Fields.Count -gt 0) {
            $null = $primaryHeader.Range.Fields.Update()
        }
        # Explicit field refresh can reapply Word's blue Hyperlink character
        # style even when the generated style definition is black.  Lock the
        # cached TOC/list results to the sample's black, non-underlined format.
        for ($i = 1; $i -le $document.TablesOfContents.Count; $i++) {
            $tocRange = $document.TablesOfContents.Item($i).Range
            $tocRange.Font.Color = 0       # wdColorBlack
            $tocRange.Font.Underline = 0   # wdUnderlineNone
            $tocRange.Font.Name = 'Times New Roman'
            $tocRange.Font.Size = 13
            $tocRange.ParagraphFormat.LineSpacingRule = 1 # wdLineSpace1pt5
        }
        for ($i = 1; $i -le $document.TablesOfFigures.Count; $i++) {
            $listRange = $document.TablesOfFigures.Item($i).Range
            $listRange.Font.Color = 0
            $listRange.Font.Underline = 0
            $listRange.Font.Name = 'Times New Roman'
            $listRange.Font.Size = 13
            $listRange.ParagraphFormat.LineSpacingRule = 1
        }
        $document.Repaginate()
        # Reflow can change every PAGEREF value.  Refresh displayed page
        # numbers once more after final pagination, without rebuilding styles.
        for ($i = 1; $i -le $document.TablesOfContents.Count; $i++) {
            $document.TablesOfContents.Item($i).UpdatePageNumbers()
        }
        for ($i = 1; $i -le $document.TablesOfFigures.Count; $i++) {
            $document.TablesOfFigures.Item($i).UpdatePageNumbers()
        }
        $document.Save()
        Write-Output 'Fields updated and document saved.'
    }

    # The Pages.EnhMetaFileBits API can deadlock in some Word installations.
    # Copy each page range as an enhanced metafile through the clipboard instead.
    Write-Output 'Paginating document...'
    $document.Repaginate()
    $pageCount = $document.ComputeStatistics(2) # wdStatisticPages
    if ($pageCount -lt 1) {
        throw 'Microsoft Word did not expose any rendered pages.'
    }
    Write-Output ("Microsoft Word exposed {0} pages; rendering..." -f $pageCount)

    for ($i = 1; $i -le $pageCount; $i++) {
        $emfPath = Join-Path $emfDir ('page-{0:D3}.emf' -f $i)
        $pngPath = Join-Path $outputPath ('page-{0:D3}.png' -f $i)
        $startRange = $document.GoTo(1, 1, $i) # wdGoToPage, wdGoToAbsolute
        if ($i -lt $pageCount) {
            $nextRange = $document.GoTo(1, 1, ($i + 1))
            $endPosition = $nextRange.Start - 1
        } else {
            $nextRange = $null
            $endPosition = $document.Content.End
        }
        $pageRange = $document.Range($startRange.Start, $endPosition)
        $pageRange.CopyAsPicture()
        Start-Sleep -Milliseconds 180

        $opened = $false
        for ($attempt = 0; $attempt -lt 60 -and -not $opened; $attempt++) {
            $opened = [ReportClipboardEmf]::OpenClipboard([IntPtr]::Zero)
            if (-not $opened) { Start-Sleep -Milliseconds 100 }
        }
        if (-not $opened) { throw "Cannot open clipboard for page $i" }
        try {
            $sourceHandle = [ReportClipboardEmf]::GetClipboardData(14) # CF_ENHMETAFILE
            if ($sourceHandle -eq [IntPtr]::Zero) { throw "No enhanced metafile for page $i" }
            $copyHandle = [ReportClipboardEmf]::CopyEnhMetaFile($sourceHandle, $emfPath)
            if ($copyHandle -eq [IntPtr]::Zero) { throw "Cannot save enhanced metafile for page $i" }
            [void][ReportClipboardEmf]::DeleteEnhMetaFile($copyHandle)
        } finally {
            [void][ReportClipboardEmf]::CloseClipboard()
        }

        $metafile = [System.Drawing.Imaging.Metafile]::FromFile($emfPath)
        try {
            $width = [Math]::Max(1, $metafile.Width)
            $height = [Math]::Max(1, $metafile.Height)
            $bitmap = New-Object System.Drawing.Bitmap($width, $height)
            try {
                $bitmap.SetResolution(144, 144)
                $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
                try {
                    $graphics.Clear([System.Drawing.Color]::White)
                    $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
                    $graphics.DrawImage($metafile, 0, 0, $width, $height)
                } finally {
                    $graphics.Dispose()
                }
                $bitmap.Save($pngPath, [System.Drawing.Imaging.ImageFormat]::Png)
            } finally {
                $bitmap.Dispose()
            }
        } finally {
            $metafile.Dispose()
        }
        [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($pageRange)
        [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($startRange)
        if ($nextRange -ne $null) {
            [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($nextRange)
        }
        if (($i % 10) -eq 0 -or $i -eq $pageCount) {
            Write-Output ("Rendered {0}/{1} pages" -f $i, $pageCount)
        }
    }

    Write-Output "Rendered $pageCount page(s) to $outputPath"
} finally {
    if ($document -ne $null) {
        $document.Close($false)
        [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($document)
    }
    if ($word -ne $null) {
        $word.Quit()
        [void][System.Runtime.InteropServices.Marshal]::ReleaseComObject($word)
    }
    [GC]::Collect()
    [GC]::WaitForPendingFinalizers()
}

if ($UpdateFields.IsPresent) {
    # Word may materialize empty first/even header/footer stories during a
    # field update.  Remove only those empty parts after Word has fully closed;
    # preserve the sole primary PAGE header on the main-content section.
    $cleanupScript = Join-Path $PSScriptRoot 'cleanup_empty_header_footer.py'
    & python $cleanupScript $inputPath
    if ($LASTEXITCODE -ne 0) {
        throw 'Failed to remove empty header/footer stories from the updated DOCX.'
    }
}
