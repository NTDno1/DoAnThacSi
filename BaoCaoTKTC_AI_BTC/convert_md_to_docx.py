# -*- coding: utf-8 -*-
"""
Chuyen file BaoCaoTKTC_AI_BTC_Full.md -> BaoCaoTKTC_AI_BTC_Full.docx
format chuan: Times New Roman 13pt body, heading 1=15pt, heading 2=14pt,
heading 3=13pt bold, line-spacing 1.5, A4, le 2.5cm.
"""

import re
import sys
from pathlib import Path

from docx import Document
from docx.shared import Pt, Cm, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH, WD_LINE_SPACING
from docx.enum.table import WD_ALIGN_VERTICAL, WD_TABLE_ALIGNMENT
from docx.oxml.ns import qn
from docx.oxml import OxmlElement

if sys.platform == "win32":
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

BASE_DIR = Path(__file__).parent
INPUT_FILE = BASE_DIR / "BaoCaoTKTC_AI_BTC_Full.md"
OUTPUT_FILE = BASE_DIR / "BaoCaoTKTC_AI_BTC_Full.docx"

FONT_NAME = "Times New Roman"
BODY_SIZE = Pt(13)
H1_SIZE = Pt(15)
H2_SIZE = Pt(14)
H3_SIZE = Pt(13)
H4_SIZE = Pt(13)
CODE_SIZE = Pt(11)


def set_run_font(run, size=BODY_SIZE, bold=False, italic=False, color=None):
    run.font.name = FONT_NAME
    run.font.size = size
    run.font.bold = bold
    run.font.italic = italic
    if color is not None:
        run.font.color.rgb = color
    # Set font for east-asian too
    rPr = run._element.get_or_add_rPr()
    rFonts = rPr.find(qn("w:rFonts"))
    if rFonts is None:
        rFonts = OxmlElement("w:rFonts")
        rPr.append(rFonts)
    rFonts.set(qn("w:ascii"), FONT_NAME)
    rFonts.set(qn("w:hAnsi"), FONT_NAME)
    rFonts.set(qn("w:eastAsia"), FONT_NAME)
    rFonts.set(qn("w:cs"), FONT_NAME)


def set_cell_borders(cell):
    tcPr = cell._tc.get_or_add_tcPr()
    tcBorders = OxmlElement("w:tcBorders")
    for edge in ("top", "left", "bottom", "right"):
        border = OxmlElement(f"w:{edge}")
        border.set(qn("w:val"), "single")
        border.set(qn("w:sz"), "4")
        border.set(qn("w:color"), "000000")
        tcBorders.append(border)
    tcPr.append(tcBorders)


def set_cell_shading(cell, color_hex):
    tcPr = cell._tc.get_or_add_tcPr()
    shd = OxmlElement("w:shd")
    shd.set(qn("w:val"), "clear")
    shd.set(qn("w:color"), "auto")
    shd.set(qn("w:fill"), color_hex)
    tcPr.append(shd)


def add_paragraph(doc, text, style=None, size=BODY_SIZE, bold=False, italic=False,
                  alignment=None, space_before=0, space_after=6, line_spacing=1.5,
                  left_indent=None):
    p = doc.add_paragraph(style=style) if style else doc.add_paragraph()
    if alignment is not None:
        p.alignment = alignment
    p.paragraph_format.space_before = Pt(space_before)
    p.paragraph_format.space_after = Pt(space_after)
    p.paragraph_format.line_spacing = line_spacing
    if left_indent is not None:
        p.paragraph_format.left_indent = left_indent
    if text:
        run = p.add_run(text)
        set_run_font(run, size=size, bold=bold, italic=italic)
    return p


def add_heading(doc, text, level):
    """level: 1..4"""
    if level == 1:
        size, bold, before, after = H1_SIZE, True, 18, 12
    elif level == 2:
        size, bold, before, after = H2_SIZE, True, 14, 8
    elif level == 3:
        size, bold, before, after = H3_SIZE, True, 10, 6
    else:
        size, bold, before, after = H4_SIZE, True, 8, 4
    p = doc.add_paragraph()
    p.paragraph_format.space_before = Pt(before)
    p.paragraph_format.space_after = Pt(after)
    p.paragraph_format.line_spacing = 1.3
    p.paragraph_format.keep_with_next = True
    run = p.add_run(text)
    set_run_font(run, size=size, bold=bold)
    return p


def add_rich_paragraph(doc, segments, alignment=None, left_indent=None,
                       space_before=0, space_after=4):
    """segments: list of (text, {bold, italic, code})"""
    p = doc.add_paragraph()
    if alignment is not None:
        p.alignment = alignment
    p.paragraph_format.space_before = Pt(space_before)
    p.paragraph_format.space_after = Pt(space_after)
    p.paragraph_format.line_spacing = 1.5
    if left_indent is not None:
        p.paragraph_format.left_indent = left_indent
    for text, opts in segments:
        run = p.add_run(text)
        set_run_font(
            run,
            size=CODE_SIZE if opts.get("code") else BODY_SIZE,
            bold=opts.get("bold", False),
            italic=opts.get("italic", False),
        )
    return p


def add_table(doc, rows, header=True, col_widths=None):
    if not rows:
        return None
    n_cols = len(rows[0])
    table = doc.add_table(rows=len(rows), cols=n_cols)
    table.alignment = WD_TABLE_ALIGNMENT.CENTER
    table.autofit = False
    if col_widths:
        for i, w in enumerate(col_widths):
            for cell in table.columns[i].cells:
                cell.width = w
    for r_idx, row_data in enumerate(rows):
        row = table.rows[r_idx]
        for c_idx, cell_text in enumerate(row_data):
            cell = row.cells[c_idx]
            cell.vertical_alignment = WD_ALIGN_VERTICAL.CENTER
            # Clear default paragraph
            cell.paragraphs[0].text = ""
            p = cell.paragraphs[0]
            p.paragraph_format.line_spacing = 1.3
            p.paragraph_format.space_before = Pt(2)
            p.paragraph_format.space_after = Pt(2)
            run = p.add_run(str(cell_text))
            is_header = (r_idx == 0 and header)
            set_run_font(run, size=BODY_SIZE, bold=is_header)
            if is_header:
                set_cell_shading(cell, "D9E2F3")
            set_cell_borders(cell)
    return table


def add_code_block(doc, code_text):
    """Render code block as monospace Times New Roman with gray background."""
    lines = code_text.split("\n")
    for line in lines:
        p = doc.add_paragraph()
        p.paragraph_format.space_before = Pt(0)
        p.paragraph_format.space_after = Pt(0)
        p.paragraph_format.line_spacing = 1.15
        p.paragraph_format.left_indent = Cm(0.8)
        run = p.add_run(line if line else " ")
        run.font.name = "Consolas"
        run.font.size = Pt(10)
        rPr = run._element.get_or_add_rPr()
        rFonts = rPr.find(qn("w:rFonts"))
        if rFonts is None:
            rFonts = OxmlElement("w:rFonts")
            rPr.append(rFonts)
        rFonts.set(qn("w:ascii"), "Consolas")
        rFonts.set(qn("w:hAnsi"), "Consolas")
        rFonts.set(qn("w:cs"), "Consolas")
    # spacing after block
    add_paragraph(doc, "", space_after=4)


def add_horizontal_rule(doc):
    p = doc.add_paragraph()
    p.paragraph_format.space_before = Pt(6)
    p.paragraph_format.space_after = Pt(6)
    pPr = p._p.get_or_add_pPr()
    pBdr = OxmlElement("w:pBdr")
    bottom = OxmlElement("w:bottom")
    bottom.set(qn("w:val"), "single")
    bottom.set(qn("w:sz"), "6")
    bottom.set(qn("w:space"), "1")
    bottom.set(qn("w:color"), "auto")
    pBdr.append(bottom)
    pPr.append(pBdr)


def add_page_break(doc):
    p = doc.add_paragraph()
    p_xml = p._p
    br = OxmlElement("w:br")
    br.set(qn("w:type"), "page")
    p_xml.append(br)


def setup_document(doc):
    """Set page margins, default font, styles."""
    section = doc.sections[0]
    section.page_height = Cm(29.7)
    section.page_width = Cm(21.0)
    section.top_margin = Cm(2.5)
    section.bottom_margin = Cm(2.5)
    section.left_margin = Cm(2.5)
    section.right_margin = Cm(2.5)

    # Default style
    style = doc.styles["Normal"]
    style.font.name = FONT_NAME
    style.font.size = BODY_SIZE
    rPr = style.element.get_or_add_rPr()
    rFonts = rPr.find(qn("w:rFonts"))
    if rFonts is None:
        rFonts = OxmlElement("w:rFonts")
        rPr.append(rFonts)
    rFonts.set(qn("w:ascii"), FONT_NAME)
    rFonts.set(qn("w:hAnsi"), FONT_NAME)
    rFonts.set(qn("w:eastAsia"), FONT_NAME)
    rFonts.set(qn("w:cs"), FONT_NAME)


def parse_markdown_table(lines, start_idx):
    """Parse a markdown table starting at start_idx. Returns (rows, next_idx)."""
    rows = []
    i = start_idx
    while i < len(lines):
        line = lines[i].strip()
        if not line.startswith("|") and not line.startswith("|-"):
            break
        if line.startswith("|---") or line.startswith("|---") or re.match(r"^\|[\s\-:|]+\|$", line):
            i += 1
            continue
        # Split by |, drop first and last empty
        parts = line.split("|")
        if len(parts) >= 2:
            parts = parts[1:-1] if len(parts) > 2 else parts[1:]
        parts = [p.strip() for p in parts]
        rows.append(parts)
        i += 1
    return rows, i


def inline_bold_to_segments(text):
    """Convert markdown bold/italic/code to a list of (text, opts) segments."""
    # We'll do a simple token-based parse
    # Pattern: **bold**, *italic*, `code`
    segments = []
    i = 0
    n = len(text)
    while i < n:
        if text.startswith("**", i):
            # find closing **
            end = text.find("**", i + 2)
            if end == -1:
                segments.append((text[i:], {}))
                break
            segments.append((text[i + 2:end], {"bold": True}))
            i = end + 2
        elif text[i] == "`":
            end = text.find("`", i + 1)
            if end == -1:
                segments.append((text[i:], {}))
                break
            segments.append((text[i + 1:end], {"code": True}))
            i = end + 1
        else:
            # plain char
            buf_start = i
            while i < n and not text.startswith("**", i) and text[i] != "`":
                i += 1
            segments.append((text[buf_start:i], {}))
    return segments


def add_rich_line(doc, text, left_indent=None):
    """Add a paragraph with inline markdown formatting."""
    segments = inline_bold_to_segments(text)
    if not segments:
        add_paragraph(doc, "", left_indent=left_indent)
        return
    add_rich_paragraph(doc, segments, left_indent=left_indent)


def convert_md_to_docx(md_path, docx_path):
    print(f"[INFO] Doc file: {md_path}")
    text = md_path.read_text(encoding="utf-8")
    # Loai bo comment HTML va doan header Markdown (front matter)
    text = re.sub(r"<!--.*?-->\n*", "", text, flags=re.DOTALL)
    lines = text.split("\n")

    doc = Document()
    setup_document(doc)

    i = 0
    n = len(lines)
    in_code_block = False
    code_buffer = []
    in_table = False
    table_buffer = []

    def flush_table():
        nonlocal table_buffer
        if table_buffer:
            add_table(doc, table_buffer)
            table_buffer = []

    while i < n:
        line = lines[i]
        stripped = line.strip()

        # Code block
        if stripped.startswith("```"):
            if not in_code_block:
                flush_table()
                in_code_block = True
                code_buffer = []
            else:
                # End code block
                add_code_block(doc, "\n".join(code_buffer))
                in_code_block = False
                code_buffer = []
            i += 1
            continue

        if in_code_block:
            code_buffer.append(line)
            i += 1
            continue

        # Empty line -> flush table
        if not stripped:
            flush_table()
            i += 1
            continue

        # Headings
        # Page break truoc cac phan lon
        if stripped.startswith("# "):
            flush_table()
            txt = stripped[2:].strip()
            # Page break cho cac heading lon
            if any(t in txt for t in [
                "BỘ TÀI CHÍNH",
                "TÀI LIỆU THIẾT KẾ CHỨC NĂNG",
                "TỔNG QUAN",
                "MÔ HÌNH KIẾN TRÚC",
            ]):
                add_page_break(doc)
            add_heading(doc, txt, 1)
            i += 1
            continue
        if stripped.startswith("## "):
            flush_table()
            txt = stripped[3:].strip()
            # Page break cho cac heading lon
            if any(t in txt for t in [
                "TỔNG QUAN",
                "MÔ HÌNH KIẾN TRÚC",
                "THIẾT KẾ CHI TIẾT CHỨC NĂNG",
                "Phân hệ AI",
                "II.5.",
            ]):
                add_page_break(doc)
            add_heading(doc, txt, 2)
            i += 1
            continue
        if stripped.startswith("### "):
            flush_table()
            add_heading(doc, stripped[4:].strip(), 3)
            i += 1
            continue
        if stripped.startswith("#### "):
            flush_table()
            add_heading(doc, stripped[5:].strip(), 4)
            i += 1
            continue

        # Horizontal rule
        if re.match(r"^\s*-{3,}\s*$", stripped):
            flush_table()
            add_horizontal_rule(doc)
            i += 1
            continue

        # Table
        if stripped.startswith("|") and not in_table:
            flush_table()
            in_table = True
            table_buffer = []
            # Parse this line
            parts = stripped.split("|")
            if len(parts) >= 2:
                parts = parts[1:-1] if len(parts) > 2 else parts[1:]
            table_buffer.append([p.strip() for p in parts])
            i += 1
            continue

        if in_table:
            if stripped.startswith("|"):
                # Skip separator line |---|---|
                if re.match(r"^\|[\s\-:|]+\|$", stripped):
                    i += 1
                    continue
                parts = stripped.split("|")
                if len(parts) >= 2:
                    parts = parts[1:-1] if len(parts) > 2 else parts[1:]
                table_buffer.append([p.strip() for p in parts])
                i += 1
                continue
            else:
                in_table = False
                flush_table()
                # don't increment i, loop will process this line

        # Bullet list
        if re.match(r"^\s*[-*]\s+", line):
            flush_table()
            content = re.sub(r"^\s*[-*]\s+", "", line)
            indent_level = 0
            m = re.match(r"^(\s*)", line)
            if m:
                indent_level = len(m.group(1)) // 2
            # Sub bullet
            if indent_level >= 1:
                add_rich_line(doc, "• " + content, left_indent=Cm(1.0 + 0.5 * indent_level))
            else:
                add_rich_line(doc, "• " + content, left_indent=Cm(0.75))
            i += 1
            continue

        # Numbered list
        if re.match(r"^\s*\d+\.\s+", line):
            flush_table()
            content = re.sub(r"^\s*\d+\.\s+", "", line)
            num_match = re.match(r"^\s*(\d+)\.\s+", line)
            num = num_match.group(1) if num_match else ""
            add_rich_line(doc, f"{num}. {content}", left_indent=Cm(0.75))
            i += 1
            continue

        # Checkbox
        if re.match(r"^\s*-\s*\[[ xX]\]\s+", line):
            flush_table()
            content = re.sub(r"^\s*-\s*\[[ xX]\]\s+", "", line)
            add_rich_line(doc, "☐ " + content, left_indent=Cm(0.75))
            i += 1
            continue

        # Blockquote
        if stripped.startswith(">"):
            flush_table()
            content = stripped[1:].strip()
            add_paragraph(doc, content, italic=True, left_indent=Cm(1.0), space_after=4)
            i += 1
            continue

        # Image ![alt](url)
        if stripped.startswith("![") and "](" in stripped:
            flush_table()
            # Just write a placeholder
            m = re.match(r"!\[([^\]]*)\]\(([^)]+)\)", stripped)
            if m:
                alt = m.group(1) or "Hinh"
                url = m.group(2)
                add_paragraph(doc, f"[Hinh: {alt} - {url}]", italic=True, alignment=WD_ALIGN_PARAGRAPH.CENTER, space_after=8)
            i += 1
            continue

        # Plain paragraph (potentially continue multi-line)
        flush_table()
        # Collect lines until empty line
        para_lines = [line]
        j = i + 1
        while j < n and lines[j].strip() and not lines[j].lstrip().startswith(("#", ">", "```", "|", "-", "*", "1.", "2.")):
            para_lines.append(lines[j])
            j += 1
        full_text = " ".join(l.strip() for l in para_lines)
        add_rich_line(doc, full_text)
        i = j

    flush_table()
    doc.save(docx_path)
    print(f"[OK] Da luu file: {docx_path}")
    print(f"     Kich thuoc: {docx_path.stat().st_size:,} bytes")


if __name__ == "__main__":
    convert_md_to_docx(INPUT_FILE, OUTPUT_FILE)
