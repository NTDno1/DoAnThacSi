# -*- coding: utf-8 -*-
"""
Chuyen doi Markdown -> HTML -> PDF cho Bao cao Thac Si
Su dung WeasyPrint
"""

import re
import os
import markdown
from markdown.extensions import tables, fenced_code, codehilite, toc, sane_lists, attr_list

BASE_DIR = r"c:\Users\datt\Documents\GitProject\DoAnThacSI\BaoCaoDeAn"
INPUT_MD = os.path.join(BASE_DIR, "BaoCaoDeAn_ThacSi_Full.md")
OUTPUT_HTML = os.path.join(BASE_DIR, "BaoCaoDeAn_ThacSi_Full.html")
OUTPUT_PDF = os.path.join(BASE_DIR, "BaoCaoDeAn_ThacSi_Full.pdf")

# Noi dung HTML template
HTML_TEMPLATE = """<!DOCTYPE html>
<html lang="vi">
<head>
<meta charset="UTF-8">
<meta name="viewport" content="width=device-width, initial-scale=1.0">
<title>Bao Cao De An Thac Si - Trần Quang Ninh</title>
<style>
@import url('https://fonts.googleapis.com/css2?family=Times+New+Roman&display=swap');

* {{
    box-sizing: border-box;
}}

body {{
    font-family: 'Times New Roman', Times, serif;
    font-size: 14pt;
    line-height: 1.6;
    text-align: justify;
    margin: 0;
    padding: 0;
    color: #000;
    background: #fff;
}}

/* Trang */
@page {{
    size: A4;
    margin: 3cm 2.5cm 2.5cm 3cm;
    @bottom-center {{
        content: counter(page);
        font-family: 'Times New Roman', serif;
        font-size: 12pt;
    }}
}}

@page :first {{
    margin: 0;
    @bottom-center {{ content: none; }}
}}

/* Heading */
h1 {{
    font-size: 22pt;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    margin-top: 2cm;
    margin-bottom: 0.5cm;
    page-break-after: avoid;
    font-family: 'Times New Roman', serif;
}}

h2 {{
    font-size: 16pt;
    font-weight: bold;
    text-align: left;
    margin-top: 0.8cm;
    margin-bottom: 0.3cm;
    page-break-after: avoid;
    font-family: 'Times New Roman', serif;
}}

h3 {{
    font-size: 14pt;
    font-weight: bold;
    text-align: left;
    margin-top: 0.5cm;
    margin-bottom: 0.2cm;
    page-break-after: avoid;
    font-family: 'Times New Roman', serif;
}}

h4 {{
    font-size: 13pt;
    font-weight: bold;
    font-style: italic;
    text-align: left;
    margin-top: 0.4cm;
    margin-bottom: 0.2cm;
    page-break-after: avoid;
    font-family: 'Times New Roman', serif;
}}

/* Paragraphs */
p {{
    margin: 0.3cm 0;
    text-indent: 1cm;
    line-height: 1.8;
    font-size: 14pt;
}}

/* Lists */
ul, ol {{
    margin: 0.3cm 0;
    padding-left: 1.5cm;
}}

li {{
    margin: 0.15cm 0;
    line-height: 1.7;
    font-size: 14pt;
}}

/* Tables */
table {{
    width: 100%;
    border-collapse: collapse;
    margin: 0.5cm 0;
    font-size: 12pt;
    page-break-inside: avoid;
}}

th {{
    background-color: #f0f0f0;
    font-weight: bold;
    text-align: center;
    padding: 6px 8px;
    border: 1px solid #000;
}}

td {{
    padding: 5px 8px;
    border: 1px solid #000;
    vertical-align: top;
    text-align: left;
}}

tr:nth-child(even) {{
    background-color: #fafafa;
}}

/* Code */
code {{
    font-family: 'Courier New', monospace;
    font-size: 11pt;
    background-color: #f5f5f5;
    padding: 1px 4px;
    border-radius: 3px;
}}

pre {{
    background-color: #f8f8f8;
    border: 1px solid #ddd;
    border-radius: 4px;
    padding: 8px 12px;
    margin: 0.4cm 0;
    overflow-x: auto;
    font-size: 10pt;
    line-height: 1.4;
    page-break-inside: avoid;
}}

pre code {{
    background: none;
    padding: 0;
    font-size: 10pt;
}}

/* Blockquote */
blockquote {{
    margin: 0.4cm 1cm;
    padding: 0.2cm 0.5cm;
    border-left: 3px solid #ccc;
    font-style: italic;
    background: #f9f9f9;
}}

/* Horizontal rule */
hr {{
    border: none;
    border-top: 1px solid #ccc;
    margin: 0.5cm 0;
}}

/* Strong / Bold */
strong, b {{
    font-weight: bold;
}}

em, i {{
    font-style: italic;
}}

/* Links */
a {{
    color: #000;
    text-decoration: none;
}}

/* Section breaks */
.chapter-heading {{
    text-align: center;
    text-transform: uppercase;
    font-size: 16pt;
    font-weight: bold;
    margin: 1.5cm 0 0.5cm 0;
    page-break-before: always;
}}

/* Title page special */
.title-page {{
    text-align: center;
    padding-top: 5cm;
}}

.title-page h1 {{
    font-size: 18pt;
    margin-bottom: 1cm;
}}

/* Special styles for file markers */
<!-- FILE: {filename} -->
.file-marker {{
    color: #999;
    font-size: 9pt;
    font-style: italic;
    text-align: center;
    margin: 2cm 0 0.5cm 0;
}}

/* Page break control */
.page-break {{
    page-break-before: always;
}}

/* Danh muc bang / hinh */
.danh-muc-title {{
    font-size: 16pt;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    margin: 1cm 0;
}}

.danh-muc-entry {{
    text-align: left;
    margin: 0.15cm 0;
    font-size: 13pt;
}}

/* Cover page */
.cover-page {{
    text-align: center;
    padding-top: 3cm;
}}

.cover-page .school-name {{
    font-size: 14pt;
    font-weight: bold;
    text-transform: uppercase;
    margin-bottom: 2cm;
}}

.cover-page .title {{
    font-size: 16pt;
    font-weight: bold;
    text-transform: uppercase;
    margin-bottom: 1cm;
}}

.cover-page .subtitle {{
    font-size: 14pt;
    margin-bottom: 2cm;
}}

.cover-page .info {{
    font-size: 13pt;
    text-align: left;
    margin-left: 4cm;
    line-height: 2;
}}

/* Lời cam đoan */
.camdoan {{
    text-align: center;
    font-weight: bold;
    font-size: 15pt;
    margin: 1.5cm 0 0.5cm 0;
}}

/* Mục lục table */
.toc-table {{
    width: 100%;
}}

.toc-entry {{
    font-size: 13pt;
    line-height: 2;
}}

/* ASCII diagram */
.ascii-diagram {{
    font-family: 'Courier New', monospace;
    font-size: 9pt;
    line-height: 1.2;
    background-color: #f5f5f5;
    border: 1px solid #ddd;
    padding: 8px;
    margin: 0.4cm 0;
    white-space: pre;
    overflow-x: auto;
    page-break-inside: avoid;
}}

/* Reference list */
.ref-entry {{
    text-align: left;
    margin: 0.2cm 0;
    font-size: 13pt;
    text-indent: -1.5cm;
    padding-left: 1.5cm;
}}

/* Footnote */
sup {{
    color: #555;
    font-size: 10pt;
}}

/* Figure caption */
.caption {{
    text-align: center;
    font-style: italic;
    font-size: 12pt;
    margin-top: 0.2cm;
    margin-bottom: 0.3cm;
}}

</style>
</head>
<body>
{content}
</body>
</html>
"""

def preprocess_markdown(content):
    """Tien xu ly markdown truoc khi convert"""

    # Loai bo comment marker cua file
    content = re.sub(r'<!-- FILE:.*?-->', '', content, flags=re.DOTALL)

    # Xu ly ASCII art blocks (giữ nguyên formatting)
    content = re.sub(r'(\n```[a-z]*\n)(.*?)(\n```)', _process_code_blocks, content, flags=re.DOTALL)

    # Xu ly table - dam bao format dung
    content = _fix_tables(content)

    # Them page break truoc cac chuong
    chapter_patterns = [
        r'\n# Chương \d+',
        r'\n## [0-9]+\.',
        r'\n## KẾT LUẬN',
        r'\n## DANH MỤC CÁC TÀI LIỆU',
        r'\n# MỤC LỤC',
        r'\n# DANH MỤC BẢNG',
        r'\n# DANH MỤC HÌNH',
        r'\n# DANH MỤC CHỮ CÁI VIẾT TẮT',
        r'\n# MỞ ĐẦU',
        r'\n# BÁO CÁO ĐỒ ÁN THẠC SĨ',
    ]

    return content


def _process_code_blocks(match):
    """Giu nguyen code blocks cho HTML pre/code"""
    return match.group(0)


def _fix_tables(content):
    """Fix table formatting"""
    lines = content.split('\n')
    result = []
    i = 0
    while i < len(lines):
        line = lines[i]
        # Neu la dong header (co | dau tien)
        if '|' in line and line.strip().startswith('|'):
            # Lay dong header va body
            table_lines = [line]
            i += 1
            # Bo qua dong separator (|---|---|)
            if i < len(lines) and re.match(r'^[\|\-\s:]+$', lines[i].strip()):
                i += 1
            # Lay cac dong body
            while i < len(lines) and '|' in lines[i] and lines[i].strip().startswith('|'):
                table_lines.append(lines[i])
                i += 1
            result.extend(table_lines)
        else:
            result.append(line)
            i += 1

    return '\n'.join(result)


def convert_md_to_html(md_content):
    """Chuyen doi Markdown sang HTML"""

    # Preprocess
    md_content = preprocess_markdown(md_content)

    # Configure markdown
    md = markdown.Markdown(
        extensions=[
            'tables',
            'fenced_code',
            'codehilite',
            'toc',
            'sane_lists',
            'attr_list',
        ],
        extension_configs={
            'codehilite': {
                'css_class': 'highlight',
                'guess_lang': False
            },
            'toc': {
                'title': 'Mục lục',
                'toc_depth': 3,
            }
        },
        output_format='html'
    )

    html_body = md.convert(md_content)

    # Wrap in template
    html = HTML_TEMPLATE.format(
        content=html_body,
        filename=INPUT_MD
    )

    return html


def convert_html_to_pdf(html_content, output_pdf):
    """Chuyen doi HTML sang PDF bang WeasyPrint"""
    from weasyprint import HTML, CSS

    print("Dang chuyen doi HTML -> PDF...")

    # Write HTML file first for debugging
    with open(OUTPUT_HTML, 'w', encoding='utf-8') as f:
        f.write(html_content)
    print(f"HTML da luu: {OUTPUT_HTML}")

    # Convert to PDF
    HTML(string=html_content, base_url=os.path.dirname(OUTPUT_HTML)).write_pdf(output_pdf)
    print(f"PDF da luu: {output_pdf}")

    import os
    pdf_size = os.path.getsize(output_pdf)
    print(f"Kich thuoc PDF: {pdf_size / 1024:.1f} KB")


def main():
    # Doc file markdown
    print(f"Doc file: {INPUT_MD}")
    with open(INPUT_MD, 'r', encoding='utf-8') as f:
        md_content = f.read()

    print(f"Tong so ky tu: {len(md_content):,}")
    print(f"Tong so dong: {md_content.count(chr(10)):,}")

    # Chuyen doi
    html_content = convert_md_to_html(md_content)

    # Lua chon xuat
    print("\nChon dinh dang xuat:")
    print("1. Chi xuat HTML")
    print("2. Xuat ca HTML va PDF")
    print("3. Chi xuat PDF")

    choice = "2"

    if choice in ['1', '2']:
        with open(OUTPUT_HTML, 'w', encoding='utf-8') as f:
            f.write(html_content)
        print(f"\nHTML da luu: {OUTPUT_HTML}")

    if choice in ['2', '3']:
        convert_html_to_pdf(html_content, OUTPUT_PDF)

    print("\nHoan tat!")


if __name__ == "__main__":
    main()
