# -*- coding: utf-8 -*-
"""
Chuyen doi Markdown -> HTML -> PDF cho Bao cao Thac Si
Su dung fpdf2
"""

import os
import re
import sys

# Force UTF-8
sys.stdout.reconfigure(encoding='utf-8')

BASE_DIR = r"c:\Users\datt\Documents\GitProject\DoAnThacSI\BaoCaoDeAn"
INPUT_MD = os.path.join(BASE_DIR, "BaoCaoDeAn_ThacSi_Full.md")
OUTPUT_HTML = os.path.join(BASE_DIR, "BaoCaoDeAn_ThacSi_Full.html")
OUTPUT_PDF = os.path.join(BASE_DIR, "BaoCaoDeAn_ThacSi_Full.pdf")

def read_file(path):
    with open(path, 'r', encoding='utf-8') as f:
        return f.read()

def preprocess_content(content):
    """Tien xu ly noi dung markdown truoc khi tao HTML"""

    # Loai bo comment marker
    content = re.sub(r'<!-- FILE:.*?-->', '', content, flags=re.DOTALL)

    # ===== CHAPTER HEADINGS =====
    # Xu ly "# Chương X" thanh page-break heading
    content = re.sub(
        r'\n# (Chương \d+:? .*?)\n',
        r'\n<div class="page-break"></div>\n<h1 class="chapter-heading">\1</h1>\n',
        content
    )

    # Xu ly cac muc luc level 2 (##)
    def replace_h2(m):
        text = m.group(1)
        return f'\n<h2>{text}</h2>\n'

    content = re.sub(r'\n## (.*?)\n', replace_h2, content)

    # Xu ly muc luc level 3 (###)
    def replace_h3(m):
        text = m.group(1)
        return f'\n<h3>{text}</h3>\n'

    content = re.sub(r'\n### (.*?)\n', replace_h3, content)

    # Xu ly muc luc level 4 (####)
    def replace_h4(m):
        text = m.group(1)
        return f'\n<h4>{text}</h4>\n'

    content = re.sub(r'\n#### (.*?)\n', replace_h4, content)

    # ===== PARAGRAPHS =====
    # Tach cac dong thuong thanh paragraph
    lines = content.split('\n')
    result = []
    skip_next = False

    for i, line in enumerate(lines):
        stripped = line.strip()

        # Bo qua dong trong
        if not stripped:
            result.append('')
            continue

        # Neu la tag HTML da xu ly thi giu nguyen
        if re.match(r'^<(h[1-6]|p|ul|ol|li|table|pre|blockquote|div|hr|/|#)', stripped):
            result.append(line)
            continue

        # Neu la phan cua code block
        if stripped.startswith('```') or stripped.startswith('|'):
            result.append(line)
            continue

        # Neu la bullet list
        if re.match(r'^[-*+] ', stripped) or re.match(r'^\d+\. ', stripped):
            result.append(line)
            continue

        # Dong binh thuong -> wrap trong <p>
        result.append(f'<p>{stripped}</p>')

    content = '\n'.join(result)

    # ===== BOLD =====
    content = re.sub(r'\*\*(.+?)\*\*', r'<strong>\1</strong>', content)
    content = re.sub(r'__(.+?)__', r'<strong>\1</strong>', content)

    # ===== ITALIC =====
    content = re.sub(r'\*(.+?)\*', r'<em>\1</em>', content)
    content = re.sub(r'_(.+?)_', r'<em>\1</em>', content)

    # ===== INLINE CODE =====
    content = re.sub(r'`([^`]+)`', r'<code>\1</code>', content)

    # ===== CODE BLOCKS =====

    def process_code_block(m):
        lang = m.group(1) or ''
        code = m.group(2)
        code = code.replace('<', '&lt;').replace('>', '&gt;')
        return f'<pre><code class="language-{lang}">{code}</code></pre>'

    content = re.sub(r'```([a-z]*)\n(.*?)```', process_code_block, content, flags=re.DOTALL)

    # ===== TABLES =====
    content = _process_tables(content)

    # ===== LISTS =====
    content = _process_lists(content)

    # ===== HORIZONTAL RULES =====
    content = re.sub(r'\n---\n', '\n<hr/>\n', content)
    content = re.sub(r'\n\*\*\*\n', '\n<hr/>\n', content)

    return content


def _process_tables(content):
    """Xu ly bang markdown thanh HTML table"""
    lines = content.split('\n')
    result = []
    i = 0
    in_table = False
    table_rows = []

    while i < len(lines):
        line = lines[i]
        stripped = line.strip()

        if '|' in stripped and stripped.startswith('|'):
            # La bang
            if not in_table:
                in_table = True
                table_rows = []

            # Bo qua dong separator
            if re.match(r'^[\|\-\s:\.]+$', stripped):
                i += 1
                continue

            # Tach cot
            cells = [c.strip() for c in stripped.split('|')[1:-1]]
            table_rows.append(cells)
        else:
            if in_table and table_rows:
                # Ket thuc bang
                result.extend(_build_table_html(table_rows))
                table_rows = []
                in_table = False
            result.append(line)

        i += 1

    # Bang cuoi cung
    if in_table and table_rows:
        result.extend(_build_table_html(table_rows))

    return '\n'.join(result)


def _build_table_html(rows):
    """Tao HTML tu danh sach cac dong bang"""
    if not rows:
        return []

    html = ['<table>']

    # Header
    html.append('<thead><tr>')
    for cell in rows[0]:
        html.append(f'<th>{cell}</th>')
    html.append('</tr></thead>')

    # Body
    if len(rows) > 1:
        html.append('<tbody>')
        for row in rows[1:]:
            html.append('<tr>')
            for cell in row:
                html.append(f'<td>{cell}</td>')
            html.append('</tr>')
        html.append('</tbody>')

    html.append('</table>')
    return html


def _process_lists(content):
    """Xu ly danh sach markdown thanh HTML"""

    lines = content.split('\n')
    result = []
    i = 0

    while i < len(lines):
        line = lines[i]
        stripped = line.strip()

        # Bullet list
        if re.match(r'^[-*+] ', stripped):
            items = []
            while i < len(lines) and re.match(r'^[-*+] ', lines[i].strip()):
                items.append(re.sub(r'^[-*+] ', '', lines[i].strip()))
                i += 1
            if items:
                result.append('<ul>')
                for item in items:
                    result.append(f'<li>{item}</li>')
                result.append('</ul>')
            continue

        # Numbered list
        if re.match(r'^\d+\. ', stripped):
            items = []
            while i < len(lines) and re.match(r'^\d+\. ', lines[i].strip()):
                items.append(re.sub(r'^\d+\. ', '', lines[i].strip()))
                i += 1
            if items:
                result.append('<ol>')
                for item in items:
                    result.append(f'<li>{item}</li>')
                result.append('</ol>')
            continue

        result.append(line)
        i += 1

    return '\n'.join(result)


def create_html(content):
    """Tao HTML hoan chinh"""

    # Tach page-break div thanh page-break CSS
    content_for_html = content.replace('<div class="page-break"></div>', '<div style="page-break-after: always;"></div>')

    # Bao ten file dinh dang bang title page
    title_html = """
    <div class="cover-page">
        <div class="school-name">HỌC VIỆN CÔNG NGHỆ BƯU CHÍNH VIỄN THÔNG</div>
        <div class="title">NGHIÊN CỨU VÀ XÂY DỰNG NỀN TẢNG AI DOANH NGHIỆP<br>HƯỚNG TÍCH HỢP ĐA HỆ THỐNG</div>
        <div class="subtitle">(Enterprise AI Platform for Cross-System Integration)</div>
        <div class="info">
            <p><strong>Học viên:</strong> Trần Quang Ninh</p>
            <p><strong>Chuyên ngành:</strong> Hệ thống Thông tin</p>
            <p><strong>Mã số:</strong> 8.48.01.04</p>
            <p><strong>Người hướng dẫn:</strong> PGS.TS. Trần Đình Quế</p>
        </div>
        <div style="margin-top: 3cm;">HÀ NỘI – 2026</div>
    </div>
    <div style="page-break-after: always;"></div>
    """

    return f"""<!DOCTYPE html>
<html lang="vi">
<head>
<meta charset="UTF-8">
<title>Báo Cáo Đồ Án Thạc Sĩ - Trần Quang Ninh</title>
<style>
@page {{
    size: A4;
    margin: 2.5cm 3cm 2.5cm 3cm;
}}
@page :first {{
    size: A4;
    margin: 0;
}}

body {{
    font-family: 'Times New Roman', Times, serif;
    font-size: 14pt;
    line-height: 1.8;
    text-align: justify;
    color: #000;
    background: #fff;
}}

h1.chapter-heading {{
    font-size: 16pt;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    margin-top: 1cm;
    margin-bottom: 0.5cm;
    page-break-after: avoid;
}}

h1 {{
    font-size: 18pt;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    margin: 1cm 0 0.5cm 0;
}}

h2 {{
    font-size: 14pt;
    font-weight: bold;
    margin-top: 0.6cm;
    margin-bottom: 0.3cm;
    page-break-after: avoid;
}}

h3 {{
    font-size: 13pt;
    font-weight: bold;
    margin-top: 0.5cm;
    margin-bottom: 0.2cm;
    page-break-after: avoid;
}}

h4 {{
    font-size: 12pt;
    font-weight: bold;
    font-style: italic;
    margin-top: 0.4cm;
    margin-bottom: 0.2cm;
    page-break-after: avoid;
}}

p {{
    margin: 0.25cm 0;
    text-indent: 1cm;
    line-height: 1.8;
}}

ul, ol {{
    margin: 0.2cm 0;
    padding-left: 1.2cm;
}}

li {{
    margin: 0.15cm 0;
    line-height: 1.7;
    font-size: 14pt;
}}

table {{
    width: 100%;
    border-collapse: collapse;
    margin: 0.4cm 0;
    font-size: 11pt;
    page-break-inside: avoid;
}}

th {{
    background-color: #f0f0f0;
    font-weight: bold;
    text-align: center;
    padding: 5px 8px;
    border: 1px solid #000;
}}

td {{
    padding: 4px 8px;
    border: 1px solid #000;
    vertical-align: top;
    text-align: left;
}}

tr:nth-child(even) {{
    background-color: #fafafa;
}}

pre {{
    font-family: 'Courier New', Courier, monospace;
    font-size: 9.5pt;
    line-height: 1.4;
    background-color: #f5f5f5;
    border: 1px solid #ddd;
    padding: 8px 10px;
    margin: 0.3cm 0;
    overflow-x: auto;
    page-break-inside: avoid;
}}

code {{
    font-family: 'Courier New', Courier, monospace;
    font-size: 10.5pt;
    background-color: #f5f5f5;
    padding: 1px 3px;
}}

blockquote {{
    margin: 0.3cm 1cm;
    padding: 0.2cm 0.5cm;
    border-left: 3px solid #ccc;
    font-style: italic;
}}

hr {{
    border: none;
    border-top: 1px solid #ccc;
    margin: 0.4cm 0;
}}

strong {{ font-weight: bold; }}
em {{ font-style: italic; }}

.cover-page {{
    text-align: center;
    padding-top: 3cm;
    page-break-after: always;
}}

.cover-page .school-name {{
    font-size: 14pt;
    font-weight: bold;
    text-transform: uppercase;
    margin-bottom: 2cm;
}}

.cover-page .title {{
    font-size: 15pt;
    font-weight: bold;
    text-transform: uppercase;
    margin-bottom: 1cm;
    line-height: 1.5;
}}

.cover-page .subtitle {{
    font-size: 13pt;
    margin-bottom: 2cm;
    font-style: italic;
}}

.cover-page .info {{
    font-size: 13pt;
    text-align: left;
    margin-left: 3cm;
    line-height: 2;
}}

.ref-entry {{
    text-align: left;
    margin: 0.15cm 0;
    font-size: 12pt;
    text-indent: -1.2cm;
    padding-left: 1.2cm;
    line-height: 1.7;
}}
</style>
</head>
<body>
{title_html}
{content_for_html}
</body>
</html>"""


def convert_html_to_pdf_fpdf2(html_content, output_pdf):
    """Chuyen doi HTML sang PDF bang fpdf2"""
    from fpdf import FPDF

    print("Dang chuyen doi HTML -> PDF voi fpdf2...")

    # Luu HTML de kiem tra
    with open(OUTPUT_HTML, 'w', encoding='utf-8') as f:
        f.write(html_content)
    print(f"HTML da luu: {OUTPUT_HTML}")

    # Su dung fpdf2 HTML support
    pdf = FPDF(unit='pt', format='A4')
    pdf.set_auto_page_break(auto=True, margin=80)
    pdf.add_font('Times', '', 'C:/Windows/Fonts/times.ttf', uni=True)
    pdf.add_font('Times', 'B', 'C:/Windows/Fonts/timesbd.ttf', uni=True)
    pdf.add_font('Courier', '', 'C:/Windows/Fonts/cour.ttf', uni=True)

    pdf.set_font('Times', '', 11)
    pdf.add_page()

    def process_html_to_fpdf(html_text):
        """Don gian: tach HTML thanh parts va render"""

        lines = html_text.split('\n')
        for line in lines:
            stripped = line.strip()

            if not stripped:
                pdf.ln(4)
                continue

            # Cover page
            if 'class="cover-page"' in stripped:
                continue

            # Page break
            if 'page-break-after' in stripped or stripped == '<div style="page-break-after: always;"></div>':
                pdf.add_page()
                continue

            # Headings
            if stripped.startswith('<h1'):
                m = re.search(r'<h1[^>]*>(.*?)</h1>', stripped, re.DOTALL)
                if m:
                    pdf.set_font('Times', 'B', 13)
                    pdf.ln(10)
                    pdf.multi_cell(0, 18, m.group(1).strip(), align='C')
                    pdf.ln(6)
                    pdf.set_font('Times', '', 11)
                continue

            if stripped.startswith('<h2'):
                m = re.search(r'<h2[^>]*>(.*?)</h2>', stripped, re.DOTALL)
                if m:
                    pdf.set_font('Times', 'B', 12)
                    pdf.ln(8)
                    pdf.multi_cell(0, 16, m.group(1).strip())
                    pdf.ln(4)
                    pdf.set_font('Times', '', 11)
                continue

            if stripped.startswith('<h3'):
                m = re.search(r'<h3[^>]*>(.*?)</h3>', stripped, re.DOTALL)
                if m:
                    pdf.set_font('Times', 'B', 11)
                    pdf.ln(6)
                    pdf.multi_cell(0, 14, m.group(1).strip())
                    pdf.ln(3)
                    pdf.set_font('Times', '', 11)
                continue

            if stripped.startswith('<h4'):
                m = re.search(r'<h4[^>]*>(.*?)</h4>', stripped, re.DOTALL)
                if m:
                    pdf.set_font('Times', 'BI', 11)
                    pdf.ln(4)
                    pdf.multi_cell(0, 14, m.group(1).strip())
                    pdf.ln(2)
                    pdf.set_font('Times', '', 11)
                continue

            # Paragraph
            if stripped.startswith('<p>'):
                m = re.search(r'<p[^>]*>(.*?)</p>', stripped, re.DOTALL)
                if m:
                    text = m.group(1)
                    text = _strip_html_tags(text)
                    if text.strip():
                        pdf.set_x(72)  # indent
                        pdf.multi_cell(0, 14, text)
                        pdf.ln(2)
                continue

            # List
            if stripped.startswith('<ul>') or stripped.startswith('<ol>'):
                continue
            if stripped.startswith('<li>'):
                m = re.search(r'<li[^>]*>(.*?)</li>', stripped, re.DOTALL)
                if m:
                    text = m.group(1)
                    text = _strip_html_tags(text)
                    if text.strip():
                        pdf.set_x(72)
                        pdf.multi_cell(0, 14, f"• {text}")
                continue
            if stripped.startswith('</ul>') or stripped.startswith('</ol>'):
                pdf.ln(2)
                continue

            # Table
            if stripped.startswith('<table'):
                continue
            if stripped.startswith('<thead>') or stripped.startswith('<tbody>') or stripped.startswith('<tr>') or stripped.startswith('</thead>') or stripped.startswith('</tbody>') or stripped.startswith('</tr>'):
                continue
            if stripped.startswith('<th'):
                continue
            if stripped.startswith('<td'):
                continue
            if stripped.startswith('</table'):
                pdf.ln(6)
                continue

            # Code block
            if stripped.startswith('<pre>'):
                continue
            if stripped.startswith('<code'):
                m = re.search(r'<code[^>]*>(.*?)</code>', stripped, re.DOTALL)
                if m:
                    text = m.group(1)
                    text = _strip_html_tags(text)
                    pdf.set_font('Courier', '', 8)
                    pdf.set_fill_color(245, 245, 245)
                    pdf.multi_cell(0, 10, text, fill=True)
                    pdf.ln(3)
                    pdf.set_font('Times', '', 11)
                continue
            if stripped.startswith('</pre>'):
                pdf.ln(4)
                continue

            # Horizontal rule
            if stripped.startswith('<hr'):
                pdf.ln(6)
                continue

            # Cover page elements
            if 'school-name' in stripped:
                m = re.search(r'<div class="school-name">(.*?)</div>', stripped, re.DOTALL)
                if m:
                    pdf.set_font('Times', 'B', 12)
                    pdf.ln(30)
                    pdf.multi_cell(0, 16, m.group(1).strip(), align='C')
                continue

            if 'class="title"' in stripped:
                m = re.search(r'<div class="title">(.*?)</div>', stripped, re.DOTALL)
                if m:
                    pdf.set_font('Times', 'B', 13)
                    pdf.ln(10)
                    pdf.multi_cell(0, 18, m.group(1).strip().replace('<br>', ' '), align='C')
                continue

            if 'subtitle' in stripped:
                m = re.search(r'<div class="subtitle">(.*?)</div>', stripped, re.DOTALL)
                if m:
                    pdf.set_font('Times', 'I', 11)
                    pdf.multi_cell(0, 15, m.group(1).strip(), align='C')
                continue

            if 'class="info"' in stripped or stripped.startswith('<p><strong>'):
                # Lay text tu strong tag
                m = re.search(r'<strong>(.*?)</strong>(.*?)(?:</p>|<br)', stripped, re.DOTALL)
                if m:
                    label = m.group(1)
                    value = m.group(2).strip()
                    value = _strip_html_tags(value)
                    pdf.set_font('Times', '', 11)
                    pdf.multi_cell(0, 14, f"{label}: {value}")
                continue

            if 'margin-top: 3cm' in stripped:
                continue

            # Strip remaining HTML and add as text
            text = _strip_html_tags(stripped)
            if text.strip():
                pdf.set_font('Times', '', 11)
                pdf.multi_cell(0, 14, text)
                pdf.ln(2)

    process_html_to_fpdf(html_content)

    pdf.output(output_pdf)
    print(f"PDF da luu: {output_pdf}")
    import os
    pdf_size = os.path.getsize(output_pdf)
    print(f"Kich thuoc PDF: {pdf_size / 1024:.1f} KB")


def _strip_html_tags(text):
    """Loai bo tat ca HTML tags"""
    # Bold
    text = re.sub(r'<strong[^>]*>(.*?)</strong>', r'\1', text, flags=re.DOTALL)
    text = re.sub(r'<b[^>]*>(.*?)</b>', r'\1', text, flags=re.DOTALL)
    # Italic
    text = re.sub(r'<em[^>]*>(.*?)</em>', r'\1', text, flags=re.DOTALL)
    text = re.sub(r'<i[^>]*>(.*?)</i>', r'\1', text, flags=re.DOTALL)
    # Code
    text = re.sub(r'<code[^>]*>(.*?)</code>', r'\1', text, flags=re.DOTALL)
    # BR
    text = re.sub(r'<br\s*/?>', ' ', text)
    text = re.sub(r'<br>', ' ', text)
    # Generic
    text = re.sub(r'<[^>]+>', '', text)
    # Decode entities
    text = text.replace('&lt;', '<').replace('&gt;', '>').replace('&amp;', '&').replace('&nbsp;', ' ')
    return text.strip()


def main():
    print(f"Doc file: {INPUT_MD}")
    md_content = read_file(INPUT_MD)
    print(f"Tong so ky tu: {len(md_content):,}")
    print(f"Tong so dong: {md_content.count(chr(10)):,}")

    # Preprocess
    processed = preprocess_content(md_content)

    # Tao HTML
    html = create_html(processed)

    # Chuyen doi PDF
    convert_html_to_pdf_fpdf2(html, OUTPUT_PDF)

    print("\nHoan tat!")


if __name__ == "__main__":
    main()
