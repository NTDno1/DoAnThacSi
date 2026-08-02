# -*- coding: utf-8 -*-
"""
Chuyen doi Markdown -> PDF cho Bao cao Thac Si
Su dung reportlab voi font Arial ho tro Tieng Viet
"""

import os
import re
import sys

sys.stdout.reconfigure(encoding='utf-8')

BASE_DIR = r"c:\Users\datt\Documents\GitProject\DoAnThacSI\BaoCaoDeAn"
INPUT_MD = os.path.join(BASE_DIR, "BaoCaoDeAn_ThacSi_Full.md")
OUTPUT_HTML = os.path.join(BASE_DIR, "BaoCaoDeAn_ThacSi_Full.html")
OUTPUT_PDF = os.path.join(BASE_DIR, "BaoCaoDeAn_ThacSi_Full.pdf")

# ============================================================
# PREPROCESSING
# ============================================================

def preprocess_markdown(content):
    """Tien xu ly markdown"""
    # Loai bo comment marker
    content = re.sub(r'<!-- FILE:.*?-->', '', content, flags=re.DOTALL)
    return content


def _strip_tags(text):
    """Loai bo HTML tags"""
    text = re.sub(r'<strong[^>]*>(.*?)</strong>', r'\1', text, flags=re.DOTALL)
    text = re.sub(r'<b[^>]*>(.*?)</b>', r'\1', text, flags=re.DOTALL)
    text = re.sub(r'<em[^>]*>(.*?)</em>', r'\1', text, flags=re.DOTALL)
    text = re.sub(r'<i[^>]*>(.*?)</i>', r'\1', text, flags=re.DOTALL)
    text = re.sub(r'<code[^>]*>(.*?)</code>', r'\1', text, flags=re.DOTALL)
    text = re.sub(r'<br\s*/?>', ' ', text)
    text = re.sub(r'<[^>]+>', '', text)
    text = text.replace('&lt;', '<').replace('&gt;', '>').replace('&amp;', '&').replace('&nbsp;', ' ')
    return text.strip()


# ============================================================
# HTML GENERATION
# ============================================================

def convert_md_to_html(md_content):
    """Chuyen doi Markdown sang HTML"""
    import markdown
    from markdown.extensions import tables, fenced_code

    md = markdown.Markdown(
        extensions=['tables', 'fenced_code', 'sane_lists', 'attr_list'],
        extension_configs={
            'codehilite': {'css_class': 'highlight', 'guess_lang': False},
        }
    )
    html_body = md.convert(md_content)
    return html_body


def build_full_html(html_body):
    """Tao HTML hoan chinh voi CSS"""

    cover = """
<div class="cover-page">
    <div class="school-name">HỌC VIỆN CÔNG NGHỆ BƯU CHÍNH VIỄN THÔNG</div>
    <div class="doc-type">BÁO CÁO ĐỒ ÁN THẠC SĨ</div>
    <div class="doc-subtitle">(Theo định hướng ứng dụng)</div>
    <div class="main-title">NGHIÊN CỨU VÀ XÂY DỰNG NỀN TẢNG AI DOANH NGHIỆP HƯỚNG TÍCH HỢP ĐA HỆ THỐNG</div>
    <div class="main-title-en">(Enterprise AI Platform for Cross-System Integration)</div>
    <div class="info-block">
        <table class="info-table">
            <tr><td class="info-label">Học viên:</td><td class="info-value">Trần Quang Ninh</td></tr>
            <tr><td class="info-label">Chuyên ngành:</td><td class="info-value">Hệ thống Thông tin</td></tr>
            <tr><td class="info-label">Mã số:</td><td class="info-value">8.48.01.04</td></tr>
            <tr><td class="info-label">Người hướng dẫn:</td><td class="info-value">PGS.TS. Trần Đình Quế</td></tr>
        </table>
    </div>
    <div class="location">HÀ NỘI – 2026</div>
</div>
"""

    return f"""<!DOCTYPE html>
<html lang="vi">
<head>
<meta charset="UTF-8">
<title>Báo Cáo Đồ Án Thạc Sĩ - Trần Quang Ninh</title>
<style>
@page {{
    size: A4;
    margin: 2.5cm 2.5cm 2.5cm 3cm;
}}
@page :first {{
    size: A4;
    margin: 0;
}}
@page toc-page {{
    size: A4;
    margin: 2.5cm 2.5cm 2.5cm 3cm;
    @bottom-center {{
        content: counter(page);
    }}
}}

body {{
    font-family: 'Arial', 'Times New Roman', sans-serif;
    font-size: 14pt;
    line-height: 1.8;
    text-align: justify;
    color: #000;
    background: #fff;
    margin: 0;
    padding: 0;
}}

h1.chapter-heading {{
    font-size: 16pt;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    margin-top: 1.5cm;
    margin-bottom: 0.5cm;
    page-break-after: avoid;
}}

h1 {{
    font-size: 18pt;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    margin: 0.8cm 0 0.4cm 0;
}}

h2 {{
    font-size: 14pt;
    font-weight: bold;
    margin-top: 0.6cm;
    margin-bottom: 0.25cm;
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
    margin: 0.2cm 0;
    text-indent: 1cm;
    line-height: 1.8;
}}

ul, ol {{
    margin: 0.2cm 0;
    padding-left: 1.2cm;
}}

li {{
    margin: 0.1cm 0;
    line-height: 1.7;
}}

table {{
    width: 100%;
    border-collapse: collapse;
    margin: 0.4cm 0;
    font-size: 11pt;
    page-break-inside: avoid;
}}

th {{
    background-color: #e8e8e8;
    font-weight: bold;
    text-align: center;
    padding: 5px 8px;
    border: 1px solid #666;
}}

td {{
    padding: 4px 8px;
    border: 1px solid #666;
    vertical-align: top;
    text-align: left;
}}

tr:nth-child(even) {{
    background-color: #f5f5f5;
}}

pre {{
    font-family: 'Courier New', Courier, monospace;
    font-size: 9pt;
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
    margin: 0.3cm 0;
}}

strong {{ font-weight: bold; }}
em {{ font-style: italic; }}

/* === COVER PAGE === */
.cover-page {{
    text-align: center;
    padding-top: 2cm;
    page-break-after: always;
}}

.cover-page .school-name {{
    font-size: 13pt;
    font-weight: bold;
    text-transform: uppercase;
    margin-bottom: 1.5cm;
}}

.cover-page .doc-type {{
    font-size: 14pt;
    font-weight: bold;
    text-transform: uppercase;
    margin-bottom: 0.3cm;
}}

.cover-page .doc-subtitle {{
    font-size: 12pt;
    font-style: italic;
    margin-bottom: 1.5cm;
}}

.cover-page .main-title {{
    font-size: 15pt;
    font-weight: bold;
    text-transform: uppercase;
    line-height: 1.5;
    margin-bottom: 0.3cm;
}}

.cover-page .main-title-en {{
    font-size: 12pt;
    font-style: italic;
    margin-bottom: 2cm;
}}

.cover-page .info-block {{
    text-align: left;
    display: inline-block;
}}

.cover-page .info-table {{
    width: auto;
    border: none;
    font-size: 13pt;
}}

.cover-page .info-table td {{
    border: none;
    padding: 4px 10px;
}}

.cover-page .info-label {{
    font-weight: bold;
    white-space: nowrap;
}}

.cover-page .location {{
    margin-top: 3cm;
    font-size: 13pt;
}}

/* Danh sach bang/hinh */
.danh-muc-heading {{
    font-size: 16pt;
    font-weight: bold;
    text-align: center;
    text-transform: uppercase;
    margin: 0.5cm 0;
}}

.danh-muc-entry {{
    font-size: 12pt;
    margin: 0.1cm 0;
    line-height: 1.7;
}}

.ref-entry {{
    text-align: left;
    font-size: 12pt;
    margin: 0.15cm 0;
    text-indent: -1cm;
    padding-left: 1cm;
    line-height: 1.7;
}}
</style>
</head>
<body>
{cover}
{html_body}
</body>
</html>"""


# ============================================================
# PDF GENERATION WITH REPORTLAB
# ============================================================

def convert_html_to_pdf(html_content, output_pdf):
    """Chuyen doi HTML -> PDF su dung reportlab"""
    from reportlab.lib.pagesizes import A4
    from reportlab.lib.units import cm, mm
    from reportlab.platypus import (
        SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle,
        PageBreak, HRFlowable, KeepTogether
    )
    from reportlab.lib.styles import ParagraphStyle, getSampleStyleSheet
    from reportlab.lib.enums import TA_LEFT, TA_CENTER, TA_JUSTIFY
    from reportlab.pdfbase import pdfmetrics
    from reportlab.pdfbase.ttfonts import TTFont
    from reportlab.platypus.flowables import HRFlowable
    from html.parser import HTMLParser
    import io

    print("Dang chuyen doi HTML -> PDF voi reportlab...")

    # Dang ky font Unicode
    arial_path = 'C:/Windows/Fonts/arial.ttf'
    arial_bold_path = 'C:/Windows/Fonts/arialbd.ttf'
    arial_italic_path = 'C:/Windows/Fonts/ariali.ttf'
    arial_bolditalic_path = 'C:/Windows/Fonts/arialbi.ttf'
    courier_path = 'C:/Windows/Fonts/cour.ttf'

    try:
        pdfmetrics.registerFont(TTFont('Arial', arial_path))
        pdfmetrics.registerFont(TTFont('Arial-Bold', arial_bold_path))
        pdfmetrics.registerFont(TTFont('Arial-Italic', arial_italic_path))
        pdfmetrics.registerFont(TTFont('Arial-BoldItalic', arial_bolditalic_path))
        pdfmetrics.registerFont(TTFont('Courier', courier_path))
        print("Font da dang ky thanh cong")
    except Exception as e:
        print(f"Loi dang ky font: {e}")
        return False

    # Tao PDF
    doc = SimpleDocTemplate(
        output_pdf,
        pagesize=A4,
        leftMargin=3*cm,
        rightMargin=2.5*cm,
        topMargin=2.5*cm,
        bottomMargin=2.5*cm,
    )

    # Styles
    styles = getSampleStyleSheet()

    normal = ParagraphStyle(
        'Normal',
        fontName='Arial',
        fontSize=12,
        leading=16,
        alignment=TA_JUSTIFY,
        firstLineIndent=20,
        spaceAfter=2,
    )

    normal_noindent = ParagraphStyle(
        'NormalNoIndent',
        fontName='Arial',
        fontSize=12,
        leading=16,
        alignment=TA_JUSTIFY,
        firstLineIndent=0,
        spaceAfter=2,
    )

    heading1 = ParagraphStyle(
        'Heading1Custom',
        fontName='Arial-Bold',
        fontSize=15,
        leading=20,
        alignment=TA_CENTER,
        spaceAfter=10,
        spaceBefore=20,
        textColor='#000',
    )

    heading2 = ParagraphStyle(
        'Heading2Custom',
        fontName='Arial-Bold',
        fontSize=13,
        leading=18,
        alignment=TA_LEFT,
        spaceAfter=6,
        spaceBefore=14,
    )

    heading3 = ParagraphStyle(
        'Heading3Custom',
        fontName='Arial-Bold',
        fontSize=12,
        leading=16,
        alignment=TA_LEFT,
        spaceAfter=4,
        spaceBefore=10,
    )

    heading4 = ParagraphStyle(
        'Heading4Custom',
        fontName='Arial-BoldItalic',
        fontSize=11,
        leading=15,
        alignment=TA_LEFT,
        spaceAfter=3,
        spaceBefore=8,
    )

    code_style = ParagraphStyle(
        'Code',
        fontName='Courier',
        fontSize=8,
        leading=10,
        alignment=TA_LEFT,
        backColor='#f5f5f5',
        borderPadding=4,
        leftIndent=0,
        firstLineIndent=0,
    )

    ref_style = ParagraphStyle(
        'Reference',
        fontName='Arial',
        fontSize=10,
        leading=14,
        alignment=TA_JUSTIFY,
        leftIndent=0,
        firstLineIndent=0,
        spaceAfter=6,
    )

    danhmuc_style = ParagraphStyle(
        'DanhMuc',
        fontName='Arial',
        fontSize=11,
        leading=15,
        alignment=TA_LEFT,
        firstLineIndent=0,
        spaceAfter=3,
    )

    danhmuc_heading = ParagraphStyle(
        'DanhMucHeading',
        fontName='Arial-Bold',
        fontSize=14,
        leading=18,
        alignment=TA_CENTER,
        spaceAfter=8,
        spaceBefore=6,
    )

    # ========== HTML PARSER ==========

    class MiniHTMLParser(HTMLParser):
        """Parser HTML don gian chi lay text va cau truc"""

        def __init__(self):
            super().__init__()
            self.elements = []  # list of (type, data)
            self._stack = []
            self._in_code = False
            self._code_content = []
            self._current_tag = ''
            self._in_cover = False
            self._skip_until_cover_end = False

        def handle_starttag(self, tag, attrs):
            attrs_dict = dict(attrs)
            if tag == 'div' and 'cover-page' in attrs_dict.get('class', ''):
                self._in_cover = True
            elif tag == 'div' and self._in_cover:
                pass
            elif tag == 'div' and 'page-break' in attrs_dict.get('style', ''):
                self.elements.append(('pagebreak', None))
            elif tag == 'br':
                self.elements.append(('br', None))
            elif tag == 'hr':
                self.elements.append(('hr', None))
            elif tag == 'p':
                self.elements.append(('para_start', None))
            elif tag in ('h1', 'h2', 'h3', 'h4'):
                self.elements.append(('heading', (tag, '')))
            elif tag == 'strong' or tag == 'b':
                self.elements.append(('bold_start', None))
            elif tag == 'em' or tag == 'i':
                self.elements.append(('italic_start', None))
            elif tag == 'code':
                self.elements.append(('code_start', None))
            elif tag == 'pre':
                self._in_code = True
                self._code_content = []
            elif tag == 'ul':
                self.elements.append(('ul_start', None))
            elif tag == 'ol':
                self.elements.append(('ol_start', None))
            elif tag == 'li':
                self.elements.append(('li_start', None))
            elif tag == 'table':
                self.elements.append(('table_start', []))
            elif tag == 'tr':
                self.elements.append(('tr_start', []))
            elif tag == 'th':
                self.elements.append(('th_start', ''))
            elif tag == 'td':
                self.elements.append(('td_start', ''))
            elif tag == 'thead':
                pass
            elif tag == 'tbody':
                pass
            elif tag == '/table':
                self.elements.append(('table_end', None))
            elif tag == '/div' and self._in_cover:
                self._in_cover = False
                self._skip_until_cover_end = True
            elif tag == '/div' and self._skip_until_cover_end:
                self._skip_until_cover_end = False

        def handle_endtag(self, tag):
            if tag == 'pre':
                self._in_code = False
                code_text = '\n'.join(self._code_content)
                self.elements.append(('code_block', code_text))
                self._code_content = []
            elif tag in ('h1', 'h2', 'h3', 'h4'):
                self.elements.append(('heading_end', tag))
            elif tag == 'strong' or tag == 'b':
                self.elements.append(('bold_end', None))
            elif tag == 'em' or tag == 'i':
                self.elements.append(('italic_end', None))
            elif tag == 'code':
                self.elements.append(('code_end', None))
            elif tag == 'p':
                self.elements.append(('para_end', None))
            elif tag == 'ul':
                self.elements.append(('ul_end', None))
            elif tag == 'ol':
                self.elements.append(('ol_end', None))
            elif tag == 'li':
                self.elements.append(('li_end', None))
            elif tag == 'th':
                self.elements.append(('th_end', None))
            elif tag == 'td':
                self.elements.append(('td_end', None))

        def handle_data(self, data):
            if self._in_code:
                self._code_content.append(data)
                return

            if self._skip_until_cover_end:
                return

            if not data.strip():
                # Neu la space trong tag body
                return

            # Cap nhat tag heading dang mo
            for i in range(len(self.elements) - 1, -1, -1):
                el = self.elements[i]
                if el[0] == 'heading' and not el[1][1]:
                    tag, _ = el[1]
                    self.elements[i] = ('heading', (tag, data))
                    return
                if el[0] == 'th_start':
                    self.elements[i] = ('th_start', data)
                    return
                if el[0] == 'td_start':
                    self.elements[i] = ('td_start', data)
                    return

            # Text binh thuong
            self.elements.append(('text', data))

        def handle_entityref(self, name):
            entities = {'amp': '&', 'lt': '<', 'gt': '>', 'nbsp': ' '}
            if name in entities:
                self.elements.append(('text', entities[name]))

        def handle_charref(self, name):
            try:
                c = chr(int(name.lstrip('&#').rstrip(';')))
                self.elements.append(('text', c))
            except:
                pass

    # Parse HTML
    parser = MiniHTMLParser()
    parser.feed(html_content)
    elements = parser.elements

    # ========== BUILD FLOWABLES ==========

    story = []
    table_data = None
    table_rows = []
    in_table = False
    in_head = False
    head_tag = None
    current_head_text = ''
    in_bold = False
    in_italic = False
    in_code = False
    current_para = ''
    current_style = normal_noindent
    list_stack = []
    list_buffer = []
    in_cover_page = False

    for i, (etype, edata) in enumerate(elements):

        if etype == 'pagebreak':
            if table_data is not None:
                # Close pending table
                story.append(_build_table(table_rows))
                table_rows = []
                table_data = None
                in_table = False
            story.append(PageBreak())

        elif etype == 'heading':
            if table_data is not None:
                story.append(_build_table(table_rows))
                table_rows = []
                table_data = None
                in_table = False

            tag, text = edata
            current_head_text = text
            in_head = True
            head_tag = tag

        elif etype == 'heading_end':
            if not in_head:
                continue
            in_head = False
            text = current_head_text.strip()

            if not text:
                continue

            # Kiem tra neu la chapter heading
            is_chapter = (
                text.startswith('Chương') or
                text.upper() in ('KẾT LUẬN', 'TÀI LIỆU THAM KHẢO', 'MỤC LỤC', 'DANH MỤC BẢNG', 'DANH MỤC HÌNH', 'DANH MỤC CHỮ CÁI VIẾT TẮT', 'MỞ ĐẦU') or
                text.startswith('##') or
                text.startswith('1.') or
                text.startswith('2.') or
                text.startswith('3.')
            )

            # Clean up chapter numbers
            clean_text = re.sub(r'^(Chương \d+:) ', '', text).strip()
            if clean_text.upper() in ('KẾT LUẬN', 'TÀI LIỆU THAM KHẢO') or \
               text.startswith('Chương') or \
               text.upper() in ('MỤC LỤC', 'DANH MỤC BẢNG', 'DANH MỤC HÌNH', 'MỞ ĐẦU', 'DANH MỤC CHỮ CÁI VIẾT TẮT'):
                clean_text = text

            style = {
                'h1': heading1,
                'h2': heading2,
                'h3': heading3,
                'h4': heading4,
            }.get(head_tag, heading2)

            story.append(Paragraph(clean_text, style))
            current_head_text = ''

        elif etype == 'para_start':
            current_para = ''

        elif etype == 'text':
            current_para += edata

        elif etype == 'para_end':
            text = current_para.strip()
            if text:
                story.append(Paragraph(text, normal))
            current_para = ''

        elif etype == 'br':
            current_para += ' '

        elif etype == 'hr':
            story.append(HRFlowable(width='100%', thickness=0.5, color='#ccc', spaceAfter=6, spaceBefore=6))

        elif etype == 'code_block':
            text = edata.strip()
            # Format code
            lines = text.split('\n')
            formatted_lines = []
            for line in lines:
                line = line.replace('&', '&amp;').replace('<', '&lt;').replace('>', '&gt;')
                formatted_lines.append(line)
            code_text = '<br/>'.join(formatted_lines)
            story.append(Paragraph(code_text, code_style))
            story.append(Spacer(1, 6))

        elif etype == 'code_start':
            in_code = True

        elif etype == 'code_end':
            in_code = False

        elif etype == 'bold_start':
            in_bold = True

        elif etype == 'bold_end':
            in_bold = False

        elif etype == 'italic_start':
            in_italic = True

        elif etype == 'italic_end':
            in_italic = False

        elif etype == 'table_start':
            in_table = True
            table_rows = []

        elif etype == 'th_start':
            table_rows.append(('th', edata))

        elif etype == 'td_start':
            table_rows.append(('td', edata))

        elif etype == 'tr_start':
            pass  # row handled by th/td

        elif etype == 'table_end':
            if in_table and table_rows:
                story.append(_build_table(table_rows))
                story.append(Spacer(1, 8))
                table_rows = []
            in_table = False

        elif etype == 'ul_start':
            list_stack.append(('ul', []))
        elif etype == 'ol_start':
            list_stack.append(('ol', []))
        elif etype == 'li_start':
            pass  # content handled by text
        elif etype == 'li_end':
            text = current_para.strip()
            if text and list_stack:
                list_type, items = list_stack[-1]
                items.append(text)
            current_para = ''
        elif etype == 'ul_end':
            if list_stack and list_stack[-1][0] == 'ul':
                list_type, items = list_stack.pop()
                for item in items:
                    story.append(Paragraph(f"• {item}", normal_noindent))
            story.append(Spacer(1, 4))
        elif etype == 'ol_end':
            if list_stack and list_stack[-1][0] == 'ol':
                list_type, items = list_stack.pop()
                for j, item in enumerate(items, 1):
                    story.append(Paragraph(f"{j}. {item}", normal_noindent))
            story.append(Spacer(1, 4))

        elif etype == 'text':
            # standalone text outside of paragraphs (shouldn't happen often)
            pass

    # Flush pending table
    if table_data is not None:
        story.append(_build_table(table_rows))
        table_rows = []

    # Build PDF
    doc.build(story)
    print(f"PDF da luu: {output_pdf}")
    import os
    size = os.path.getsize(output_pdf)
    print(f"Kich thuoc: {size / 1024:.0f} KB")
    return True


def _build_table(rows):
    """Xay dung Table tu danh sach rows"""
    from reportlab.platypus import Table, TableStyle
    from reportlab.lib.styles import ParagraphStyle
    from reportlab.lib.enums import TA_LEFT, TA_CENTER
    from reportlab.lib import colors

    if not rows:
        return None

    # Tach header va body
    data = []
    header = []
    body = []
    is_header = True
    current_row = []

    for rtype, rdata in rows:
        if rtype == 'th':
            current_row.append(rdata)
        elif rtype == 'td':
            current_row.append(rdata)
            is_header = False

        if rtype in ('th', 'td'):
            pass  # da them
        elif rtype == 'tr' or (rtype in ('th', 'td') and len(current_row) > 0):
            # finished a row
            pass

    # Re-process: group by tr
    data = []
    header = []
    body = []
    row_idx = 0
    col_count = 0
    i = 0
    while i < len(rows):
        rtype, rdata = rows[i]
        if rtype == 'th':
            header.append(rdata)
            col_count = max(col_count, len(header))
            i += 1
        elif rtype == 'td':
            body.append(rdata)
            col_count = max(col_count, len(body))
            i += 1
        else:
            i += 1

    # Build proper rows
    all_rows = [header] if header else []
    current_body_row = []
    for rtype, rdata in rows:
        if rtype == 'th':
            if current_body_row:
                all_rows.append(current_body_row)
                current_body_row = []
            header.append(rdata)
        elif rtype == 'td':
            current_body_row.append(rdata)
    if current_body_row:
        all_rows.append(current_body_row)

    if not all_rows:
        return None

    # Normalize row lengths
    normalized = []
    for row in all_rows:
        normalized_row = list(row)
        while len(normalized_row) < col_count:
            normalized_row.append('')
        normalized.append(normalized_row[:col_count])

    # Create table
    col_widths = None  # auto

    # Style
    style = [
        ('FONTNAME', (0, 0), (-1, -1), 'Arial'),
        ('FONTSIZE', (0, 0), (-1, -1), 10),
        ('FONTNAME', (0, 0), (-1, 0), 'Arial-Bold'),
        ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#e8e8e8')),
        ('ALIGN', (0, 0), (-1, 0), 'CENTER'),
        ('ALIGN', (0, 1), (-1, -1), 'LEFT'),
        ('VALIGN', (0, 0), (-1, -1), 'TOP'),
        ('GRID', (0, 0), (-1, -1), 0.5, colors.HexColor('#666')),
        ('TOPPADDING', (0, 0), (-1, -1), 4),
        ('BOTTOMPADDING', (0, 0), (-1, -1), 4),
        ('LEFTPADDING', (0, 0), (-1, -1), 6),
        ('RIGHTPADDING', (0, 0), (-1, -1), 6),
    ]

    # Alternating row colors for body
    if len(normalized) > 1:
        for r in range(1, len(normalized)):
            if r % 2 == 1:
                style.append(('BACKGROUND', (0, r), (-1, r), colors.HexColor('#f5f5f5')))

    # Wrap in Paragraph
    def make_cell(text):
        from reportlab.platypus import Paragraph
        from reportlab.lib.styles import ParagraphStyle
        s = ParagraphStyle('cell', fontName='Arial', fontSize=10, leading=13, alignment=TA_LEFT)
        return Paragraph(text or '', s)

    table_data = [[make_cell(cell) for cell in row] for row in normalized]

    t = Table(table_data, colWidths=col_widths)
    t.setStyle(TableStyle(style))
    return t


# ============================================================
# MAIN
# ============================================================

def main():
    print(f"Doc file: {INPUT_MD}")
    with open(INPUT_MD, 'r', encoding='utf-8') as f:
        md_content = f.read()
    print(f"Tong so ky tu: {len(md_content):,}")
    print(f"Tong so dong: {md_content.count(chr(10)):,}")

    # Preprocess
    md_content = preprocess_markdown(md_content)

    # Convert MD -> HTML
    html_body = convert_md_to_html(md_content)

    # Build full HTML
    html_full = build_full_html(html_body)

    # Save HTML
    with open(OUTPUT_HTML, 'w', encoding='utf-8') as f:
        f.write(html_full)
    print(f"HTML da luu: {OUTPUT_HTML}")

    # Convert HTML -> PDF
    success = convert_html_to_pdf(html_full, OUTPUT_PDF)
    if success:
        print(f"\n=== HOAN TAT ===")
        print(f"PDF: {OUTPUT_PDF}")
        import os
        size = os.path.getsize(OUTPUT_PDF)
        print(f"Kich thuoc: {size / 1024:.0f} KB")


if __name__ == "__main__":
    main()
