# -*- coding: utf-8 -*-
"""
Chuyen doi Markdown -> PDF cho Bao cao Thac Si
Parser HTML chinh xac + reportlab
"""

import os
import re
import sys
sys.stdout.reconfigure(encoding='utf-8')

from bs4 import BeautifulSoup
from reportlab.lib.pagesizes import A4
from reportlab.lib.units import cm
from reportlab.platypus import (
    SimpleDocTemplate, Paragraph, Spacer, Table, TableStyle,
    PageBreak, HRFlowable, KeepTogether
)
from reportlab.lib.styles import ParagraphStyle
from reportlab.lib.enums import TA_LEFT, TA_CENTER, TA_JUSTIFY, TA_RIGHT
from reportlab.lib import colors
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus.flowables import Flowable

BASE_DIR = r"c:\Users\datt\Documents\GitProject\DoAnThacSI\BaoCaoDeAn"
INPUT_MD = os.path.join(BASE_DIR, "BaoCaoDeAn_ThacSi_Full.md")
OUTPUT_HTML = os.path.join(BASE_DIR, "BaoCaoDeAn_ThacSi_Full.html")
OUTPUT_PDF = os.path.join(BASE_DIR, "BaoCaoDeAn_ThacSi_Full.pdf")


# ============================================================
# MARKDOWN -> HTML
# ============================================================

def md_to_html(md_content):
    import markdown
    from markdown.extensions import tables, fenced_code, codehilite, sane_lists, attr_list

    md = markdown.Markdown(
        extensions=['tables', 'fenced_code', 'codehilite', 'sane_lists', 'attr_list'],
        extension_configs={
            'codehilite': {'css_class': 'highlight', 'guess_lang': False},
        }
    )

    # Loai bo file markers
    md_content = re.sub(r'<!-- FILE:.*?-->', '', md_content, flags=re.DOTALL)

    # Chuyen doi
    html_body = md.convert(md_content)
    return html_body


def build_html(html_body):
    """Tao HTML hoan chinh voi CSS"""

    cover = """
<div class="cover-page">
    <div class="school-name">HỌC VIỆN CÔNG NGHỆ BƯU CHÍNH VIỄN THÔNG</div>
    <div class="doc-type">BÁO CÁO ĐỒ ÁN THẠC SĨ</div>
    <div class="doc-subtitle">(Theo định hướng ứng dụng)</div>
    <div class="main-title">NGHIÊN CỨU VÀ XÂY DỰNG<br>NỀN TẢNG AI DOANH NGHIỆP<br>HƯỚNG TÍCH HỢP ĐA HỆ THỐNG</div>
    <div class="main-title-en">(Enterprise AI Platform for Cross-System Integration)</div>
    <div class="info-section">
        <div class="info-row"><span class="info-label">Học viên:</span><span class="info-value">Trần Quang Ninh</span></div>
        <div class="info-row"><span class="info-label">Chuyên ngành:</span><span class="info-value">Hệ thống Thông tin</span></div>
        <div class="info-row"><span class="info-label">Mã số:</span><span class="info-value">8.48.01.04</span></div>
        <div class="info-row"><span class="info-label">Người hướng dẫn:</span><span class="info-value">PGS.TS. Trần Đình Quế</span></div>
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

body {{
    font-family: Arial, 'Times New Roman', sans-serif;
    font-size: 14pt;
    line-height: 1.8;
    text-align: justify;
    color: #000;
    background: #fff;
    margin: 0;
    padding: 0;
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
    line-height: 1.35;
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
    padding-top: 1.5cm;
    page-break-after: always;
}}

.cover-page .school-name {{
    font-size: 13pt;
    font-weight: bold;
    text-transform: uppercase;
    margin-bottom: 1.5cm;
}}

.cover-page .doc-type {{
    font-size: 15pt;
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
    line-height: 1.6;
    margin-bottom: 0.3cm;
}}

.cover-page .main-title-en {{
    font-size: 12pt;
    font-style: italic;
    margin-bottom: 1.5cm;
}}

.cover-page .info-section {{
    display: inline-block;
    text-align: left;
    font-size: 13pt;
    line-height: 2;
}}

.cover-page .info-label {{
    font-weight: bold;
    display: inline-block;
    min-width: 3cm;
}}

.cover-page .location {{
    margin-top: 3cm;
    font-size: 13pt;
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
# HTML -> PDF with reportlab
# ============================================================

def html_to_pdf(html_content, output_pdf):
    """Chuyen doi HTML -> PDF su dung BeautifulSoup + reportlab"""
    print("Dang chuyen doi HTML -> PDF...")

    # Dang ky font
    fonts = {
        'Arial': 'C:/Windows/Fonts/arial.ttf',
        'Arial-Bold': 'C:/Windows/Fonts/arialbd.ttf',
        'Arial-Italic': 'C:/Windows/Fonts/ariali.ttf',
        'Arial-BoldItalic': 'C:/Windows/Fonts/arialbi.ttf',
        'Courier': 'C:/Windows/Fonts/cour.ttf',
    }
    for name, path in fonts.items():
        try:
            pdfmetrics.registerFont(TTFont(name, path))
        except Exception as e:
            print(f"Khong dang ky duoc font {name}: {e}")

    PAGE_W, PAGE_H = A4
    LEFT_M = 3.0 * cm
    RIGHT_M = 2.5 * cm
    TOP_M = 2.5 * cm
    BOT_M = 2.5 * cm

    doc = SimpleDocTemplate(
        output_pdf,
        pagesize=A4,
        leftMargin=LEFT_M,
        rightMargin=RIGHT_M,
        topMargin=TOP_M,
        bottomMargin=BOT_M,
    )

    # Styles
    s_normal = ParagraphStyle('Normal', fontName='Arial', fontSize=12, leading=16,
                              alignment=TA_JUSTIFY, firstLineIndent=20, spaceAfter=4)
    s_noindent = ParagraphStyle('NoIndent', fontName='Arial', fontSize=12, leading=16,
                                alignment=TA_JUSTIFY, firstLineIndent=0, spaceAfter=4)
    s_center = ParagraphStyle('Center', fontName='Arial', fontSize=12, leading=16,
                              alignment=TA_CENTER, firstLineIndent=0, spaceAfter=4)
    s_h1 = ParagraphStyle('H1', fontName='Arial-Bold', fontSize=16, leading=22,
                          alignment=TA_CENTER, firstLineIndent=0, spaceAfter=8, spaceBefore=14)
    s_h2 = ParagraphStyle('H2', fontName='Arial-Bold', fontSize=13, leading=18,
                          alignment=TA_LEFT, firstLineIndent=0, spaceAfter=6, spaceBefore=12)
    s_h3 = ParagraphStyle('H3', fontName='Arial-Bold', fontSize=12, leading=16,
                          alignment=TA_LEFT, firstLineIndent=0, spaceAfter=4, spaceBefore=10)
    s_h4 = ParagraphStyle('H4', fontName='Arial-BoldItalic', fontSize=11, leading=15,
                          alignment=TA_LEFT, firstLineIndent=0, spaceAfter=3, spaceBefore=8)
    s_code = ParagraphStyle('Code', fontName='Courier', fontSize=8, leading=11,
                             alignment=TA_LEFT, firstLineIndent=0, spaceAfter=4)
    s_ref = ParagraphStyle('Ref', fontName='Arial', fontSize=10, leading=14,
                            alignment=TA_JUSTIFY, firstLineIndent=0, spaceAfter=4)
    s_cover_school = ParagraphStyle('CoverSchool', fontName='Arial-Bold', fontSize=12,
                                     leading=18, alignment=TA_CENTER, firstLineIndent=0)
    s_cover_title = ParagraphStyle('CoverTitle', fontName='Arial-Bold', fontSize=14,
                                    leading=20, alignment=TA_CENTER, firstLineIndent=0)
    s_cover_sub = ParagraphStyle('CoverSub', fontName='Arial', fontSize=12,
                                  leading=16, alignment=TA_CENTER, firstLineIndent=0)
    s_cover_info_label = ParagraphStyle('CoverInfoLabel', fontName='Arial-Bold', fontSize=12,
                                         leading=18, alignment=TA_LEFT, firstLineIndent=0)
    s_cover_info_val = ParagraphStyle('CoverInfoVal', fontName='Arial', fontSize=12,
                                       leading=18, alignment=TA_LEFT, firstLineIndent=0)

    TEXT_W = PAGE_W - LEFT_M - RIGHT_M

    # Parse HTML
    soup = BeautifulSoup(html_content, 'lxml')
    body = soup.find('body')

    story = []
    _add_elements(body, story, s_normal, s_noindent, s_h1, s_h2, s_h3, s_h4,
                  s_code, s_center, TEXT_W, s_ref, s_cover_school, s_cover_title,
                  s_cover_sub, s_cover_info_label, s_cover_info_val)

    doc.build(story)
    import os
    size = os.path.getsize(output_pdf)
    print(f"PDF da luu: {output_pdf} ({size/1024:.0f} KB)")
    return True


def _get_text(element, include_children=True):
    """Lay text tu element va children, loai bo script/style"""
    if element.name in ('script', 'style', 'noscript'):
        return ''
    if include_children:
        return element.get_text(separator=' ', strip=True)
    return element.string or ''


def _child_iter(element):
    """Yield child elements (not NavigableString directly)"""
    for child in element.children:
        if hasattr(child, 'name') and child.name:
            yield child


def _inline_text(element):
    """Lay text cua element, giu nguyen bold/italic cua children"""
    parts = []
    for child in element.children:
        if hasattr(child, 'name') and child.name:
            if child.name in ('strong', 'b'):
                parts.append(f"<b>{_inline_text(child)}</b>")
            elif child.name in ('em', 'i'):
                parts.append(f"<i>{_inline_text(child)}</i>")
            elif child.name == 'br':
                parts.append(' ')
            elif child.name in ('code', 'span'):
                parts.append(_inline_text(child))
            else:
                parts.append(_inline_text(child))
        else:
            s = str(child)
            parts.append(s)
    return ''.join(parts)


def _para(text, style, indent=0, **kwargs):
    """Tao Paragraph, xu ly bold/italic"""
    # Clean text
    text = text.replace('\u00A0', ' ').strip()
    if not text:
        return None
    return Paragraph(text, style)


def _add_elements(element, story, s_normal, s_noindent, s_h1, s_h2, s_h3, s_h4,
                 s_code, s_center, TEXT_W, s_ref, s_cover_school, s_cover_title,
                 s_cover_sub, s_cover_info_label, s_cover_info_val):
    """Duyet HTML element va tao PDF flowables"""

    for child in _child_iter(element):

        tag = child.name

        # ---- PAGE BREAK ----
        if tag == 'div':
            style = child.get('style', '')
            if 'page-break-after' in style or 'page-break-before' in style:
                story.append(PageBreak())
                continue
            # Cover page elements
            cls = child.get('class', [])
            cls_str = ' '.join(cls) if isinstance(cls, list) else str(cls)
            if 'cover-page' in cls_str:
                _add_cover_page(child, story, s_cover_school, s_cover_title,
                                s_cover_sub, s_cover_info_label, s_cover_info_val, s_center)
                continue
            # Recurse into div
            _add_elements(child, story, s_normal, s_noindent, s_h1, s_h2, s_h3, s_h4,
                          s_code, s_center, TEXT_W, s_ref, s_cover_school, s_cover_title,
                          s_cover_sub, s_cover_info_label, s_cover_info_val)
            continue

        # ---- HEADINGS ----
        if tag == 'h1':
            text = _get_text(child)
            text = text.strip()
            if text:
                story.append(Paragraph(text, s_h1))
            continue

        if tag == 'h2':
            text = _get_text(child)
            text = text.strip()
            if text:
                story.append(Paragraph(text, s_h2))
            continue

        if tag == 'h3':
            text = _get_text(child)
            text = text.strip()
            if text:
                story.append(Paragraph(text, s_h3))
            continue

        if tag == 'h4':
            text = _get_text(child)
            text = text.strip()
            if text:
                story.append(Paragraph(text, s_h4))
            continue

        # ---- PARAGRAPH ----
        if tag == 'p':
            text = _inline_text(child)
            text = text.strip()
            if text:
                story.append(Paragraph(text, s_normal))
            story.append(Spacer(1, 4))
            continue

        # ---- HORIZONTAL RULE ----
        if tag == 'hr':
            story.append(HRFlowable(width='100%', thickness=0.5, color='#ccc',
                                     spaceAfter=6, spaceBefore=6))
            continue

        # ---- CODE BLOCK ----
        if tag == 'pre':
            # Get all text in pre
            code_text = child.get_text(separator='\n')
            code_text = code_text.rstrip('\n')
            if code_text:
                # Wrap long lines manually at ~80 chars
                wrapped = _wrap_code(code_text, 90)
                p = Paragraph(wrapped, s_code)
                story.append(p)
                story.append(Spacer(1, 6))
            continue

        # ---- UNORDERED LIST ----
        if tag == 'ul':
            for li in child.find_all('li', recursive=False):
                text = _inline_text(li).strip()
                if text:
                    story.append(Paragraph(f"\u2022  {text}", s_noindent))
            story.append(Spacer(1, 6))
            continue

        # ---- ORDERED LIST ----
        if tag == 'ol':
            items = child.find_all('li', recursive=False)
            for j, li in enumerate(items, 1):
                text = _inline_text(li).strip()
                if text:
                    story.append(Paragraph(f"{j}.  {text}", s_noindent))
            story.append(Spacer(1, 6))
            continue

        # ---- TABLE ----
        if tag == 'table':
            tbl = _build_table(child, TEXT_W)
            if tbl:
                story.append(tbl)
                story.append(Spacer(1, 8))
            continue

        # ---- BLOCKQUOTE ----
        if tag == 'blockquote':
            text = _get_text(child).strip()
            if text:
                story.append(Paragraph(text, s_noindent))
            story.append(Spacer(1, 6))
            continue

        # ---- GENERIC (recurse) ----
        if tag:
            _add_elements(child, story, s_normal, s_noindent, s_h1, s_h2, s_h3, s_h4,
                          s_code, s_center, TEXT_W, s_ref, s_cover_school, s_cover_title,
                          s_cover_sub, s_cover_info_label, s_cover_info_val)


def _add_cover_page(element, story, s_cover_school, s_cover_title, s_cover_sub,
                    s_cover_info_label, s_cover_info_val, s_center):
    """Them cover page"""
    for child in _child_iter(element):
        tag = child.name

        if tag == 'div':
            cls = child.get('class', [])
            cls_str = ' '.join(cls) if isinstance(cls, list) else str(cls)

            if 'school-name' in cls_str:
                text = _get_text(child).strip()
                story.append(Spacer(1, 20))
                story.append(Paragraph(text, s_cover_school))
                story.append(Spacer(1, 10))

            elif 'doc-type' in cls_str:
                text = _get_text(child).strip()
                story.append(Paragraph(text, s_cover_title))

            elif 'doc-subtitle' in cls_str:
                text = _get_text(child).strip()
                story.append(Spacer(1, 8))
                story.append(Paragraph(text, s_cover_sub))

            elif 'main-title' in cls_str:
                # May have <br> tags
                lines = []
                for sub in child.children:
                    if hasattr(sub, 'name'):
                        if sub.name == 'br':
                            lines.append('\n')
                        else:
                            lines.append(_get_text(sub))
                    elif sub.string:
                        lines.append(sub.string)
                text = ''.join(lines).strip()
                story.append(Spacer(1, 8))
                story.append(Paragraph(text, s_cover_title))

            elif 'main-title-en' in cls_str:
                text = _get_text(child).strip()
                story.append(Spacer(1, 6))
                story.append(Paragraph(text, s_cover_sub))

            elif 'info-section' in cls_str:
                story.append(Spacer(1, 16))
                for row in _child_iter(child):
                    if row.name == 'div':
                        label = ''
                        value = ''
                        for sub in row.children:
                            if hasattr(sub, 'name') and sub.name == 'span':
                                cls2 = sub.get('class', [])
                                cls2_str = ' '.join(cls2) if isinstance(cls2, list) else str(cls2)
                                txt = _get_text(sub).strip()
                                if 'info-label' in cls2_str:
                                    label = txt
                                elif 'info-value' in cls2_str:
                                    value = txt
                        if label:
                            story.append(Paragraph(f"<b>{label}</b> {value}", s_cover_info_label))
            elif 'location' in cls_str:
                text = _get_text(child).strip()
                story.append(Spacer(1, 40))
                story.append(Paragraph(text, s_cover_title))

            continue

        if tag == 'br':
            story.append(Spacer(1, 6))


def _build_table(element, TEXT_W):
    """Xay dung Table tu HTML table element"""
    from reportlab.lib import colors

    # Lay cac dong
    header_row = []
    body_rows = []
    thead = element.find('thead')
    tbody = element.find('tbody')

    if thead:
        tr = thead.find('tr')
        if tr:
            header_row = [_get_text(th).strip() for th in tr.find_all(['th', 'td'])]
    else:
        first_tr = element.find('tr')
        if first_tr:
            ths = first_tr.find_all('th')
            if ths:
                header_row = [_get_text(th).strip() for th in ths]
            else:
                tds = first_tr.find_all('td')
                if tds:
                    header_row = [_get_text(td).strip() for td in tds]
                    body_trs = tbody.find_all('tr') if tbody else element.find_all('tr')[1:]
                    for tr in body_trs:
                        body_rows.append([_get_text(td).strip() for td in tr.find_all('td')])
                else:
                    return None
        else:
            return None

    if tbody:
        for tr in tbody.find_all('tr'):
            body_rows.append([_get_text(td).strip() for td in tr.find_all('td')])
    else:
        # Lay body rows = all tr sau header
        all_trs = element.find_all('tr')
        if len(all_trs) > 1:
            for tr in all_trs[1:]:
                body_rows.append([_get_text(td).strip() for td in tr.find_all('td')])

    if not header_row and not body_rows:
        return None

    # Determine col count
    col_count = len(header_row) if header_row else (len(body_rows[0]) if body_rows else 1)
    col_count = max(col_count, 1)

    # Normalize
    def norm_row(row):
        r = list(row)
        while len(r) < col_count:
            r.append('')
        return r[:col_count]

    header_row = norm_row(header_row) if header_row else [''] * col_count

    # Build data
    def cell_p(text):
        return Paragraph(text or '', ParagraphStyle('TblCell', fontName='Arial',
                                                    fontSize=9, leading=12,
                                                    alignment=TA_LEFT))

    data = [[cell_p(h) for h in header_row]]
    for row in body_rows:
        data.append([cell_p(c) for c in norm_row(row)])

    # Col widths
    num_cols = col_count
    min_col_w = TEXT_W / num_cols
    col_widths = [min_col_w] * num_cols

    # Style
    tbl_style = [
        ('FONTNAME', (0, 0), (-1, -1), 'Arial'),
        ('FONTSIZE', (0, 0), (-1, -1), 9),
        ('FONTNAME', (0, 0), (-1, 0), 'Arial-Bold'),
        ('BACKGROUND', (0, 0), (-1, 0), colors.HexColor('#e8e8e8')),
        ('ALIGN', (0, 0), (-1, 0), 'CENTER'),
        ('ALIGN', (0, 1), (-1, -1), 'LEFT'),
        ('VALIGN', (0, 0), (-1, -1), 'TOP'),
        ('GRID', (0, 0), (-1, -1), 0.5, colors.HexColor('#666')),
        ('TOPPADDING', (0, 0), (-1, -1), 4),
        ('BOTTOMPADDING', (0, 0), (-1, -1), 4),
        ('LEFTPADDING', (0, 0), (-1, -1), 5),
        ('RIGHTPADDING', (0, 0), (-1, -1), 5),
    ]

    # Alternating row colors
    for r in range(1, len(data)):
        if r % 2 == 1:
            tbl_style.append(('BACKGROUND', (0, r), (-1, r), colors.HexColor('#f5f5f5')))

    tbl = Table(data, colWidths=col_widths)
    tbl.setStyle(TableStyle(tbl_style))
    return tbl


def _wrap_code(text, max_chars):
    """Wrap code lines at max_chars, nhung giu nguyen formatting"""
    lines = text.split('\n')
    result = []
    for line in lines:
        if len(line) <= max_chars:
            result.append(line)
        else:
            # Try to break at space
            parts = line.split(' ')
            current = ''
            for part in parts:
                if len(current) + len(part) + 1 <= max_chars:
                    current = (current + ' ' + part).strip()
                else:
                    if current:
                        result.append(current)
                    current = part
            if current:
                result.append(current)
    return '\n'.join(result)


# ============================================================
# MAIN
# ============================================================

def main():
    print(f"Doc file: {INPUT_MD}")
    with open(INPUT_MD, 'r', encoding='utf-8') as f:
        md_content = f.read()
    print(f"Tong so ky tu: {len(md_content):,}")
    print(f"Tong so dong: {md_content.count(chr(10)):,}")

    # Step 1: MD -> HTML body
    html_body = md_to_html(md_content)

    # Step 2: Build full HTML with CSS
    html_full = build_html(html_body)

    # Step 3: Save HTML
    with open(OUTPUT_HTML, 'w', encoding='utf-8') as f:
        f.write(html_full)
    print(f"HTML da luu: {OUTPUT_HTML}")

    # Step 4: HTML -> PDF
    html_to_pdf(html_full, OUTPUT_PDF)

    # Step 5: Check
    from PyPDF2 import PdfReader
    try:
        r = PdfReader(OUTPUT_PDF)
        print(f"Tong so trang PDF: {len(r.pages)}")
    except Exception as e:
        print(f"Loi doc PDF: {e}")

    print("\nHOAN TAT!")


if __name__ == "__main__":
    main()
