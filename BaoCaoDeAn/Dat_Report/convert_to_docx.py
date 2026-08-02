# -*- coding: utf-8 -*-
"""
Script chuyen doi bao cao Markdown sang Word (.docx)
Bao cao do an thac si - Nguyen Tien Dat

Yeu cau: pip install python-docx
"""

import sys
import os
import re
import docx
from pathlib import Path
from docx.shared import Pt, Inches, RGBColor, Cm
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.style import WD_STYLE_TYPE
from docx.oxml.ns import qn
from docx.oxml import OxmlElement

# Fix encoding cho stdout tren Windows
if sys.platform == "win32":
    try:
        sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    except Exception:
        pass

# ── Cau hinh ────────────────────────────────────────────────────────────────
BASE_DIR = Path(__file__).parent
INPUT_FILE = BASE_DIR / "BaoCaoDeAn_ThacSi_NguyenTienDat_Full.md"
OUTPUT_FILE = BASE_DIR / "BaoCaoDeAn_ThacSi_NguyenTienDat_Full.docx"

# Font mac dinh Times New Roman, size 13pt (kho A4)
DEFAULT_FONT = "Times New Roman"
DEFAULT_SIZE = 13  # pt
TITLE_SIZE = 16    # pt
H1_SIZE = 14       # pt
H2_SIZE = 13       # pt
MARGIN_CM = 2.5    # le 2.5 cm moi ben


# ── Tien ich ────────────────────────────────────────────────────────────────

def set_page_margins(doc):
    for section in doc.sections:
        section.top_margin = Cm(MARGIN_CM)
        section.bottom_margin = Cm(MARGIN_CM)
        section.left_margin = Cm(MARGIN_CM)
        section.right_margin = Cm(MARGIN_CM)


def set_run_font(run, size_pt=None, bold=False):
    run.font.name = DEFAULT_FONT
    if size_pt:
        run.font.size = Pt(size_pt)
    run.font.bold = bold
    r = run._element
    rPr = r.get_or_add_rPr()
    rFonts = OxmlElement("w:rFonts")
    rFonts.set(qn("w:eastAsia"), DEFAULT_FONT)
    rFonts.set(qn("w:ascii"), DEFAULT_FONT)
    rFonts.set(qn("w:hAnsi"), DEFAULT_FONT)
    rPr.insert(0, rFonts)


def add_heading(doc, text, level):
    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.LEFT
    if level == 1:
        p.alignment = WD_ALIGN_PARAGRAPH.CENTER
        p.paragraph_format.space_before = Pt(18)
        p.paragraph_format.space_after = Pt(12)
    elif level == 2:
        p.paragraph_format.space_before = Pt(12)
        p.paragraph_format.space_after = Pt(6)
    elif level == 3:
        p.paragraph_format.space_before = Pt(8)
        p.paragraph_format.space_after = Pt(4)
    else:
        p.paragraph_format.space_before = Pt(6)
        p.paragraph_format.space_after = Pt(3)

    run = p.add_run(text)
    run.font.bold = True
    if level == 1:
        run.font.size = Pt(TITLE_SIZE)
    elif level == 2:
        run.font.size = Pt(H1_SIZE)
    elif level == 3:
        run.font.size = Pt(H2_SIZE)
    else:
        run.font.size = Pt(H2_SIZE)
    set_run_font(run)
    return p


def add_paragraph(doc, text):
    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.JUSTIFY
    p.paragraph_format.first_line_indent = Cm(0.74)  # 2 spaces ~ 0.74cm
    p.paragraph_format.space_after = Pt(6)
    p.paragraph_format.line_spacing = 1.5
    run = p.add_run(text)
    set_run_font(run, DEFAULT_SIZE)
    return p


def add_code_block(doc, code_text):
    p = doc.add_paragraph()
    p.alignment = WD_ALIGN_PARAGRAPH.LEFT
    p.paragraph_format.left_indent = Cm(1.0)
    p.paragraph_format.space_before = Pt(3)
    p.paragraph_format.space_after = Pt(3)
    # Dat border de tao hieu ung code block
    pPr = p._element.get_or_add_pPr()
    pBdr = OxmlElement("w:pBdr")
    for side in ["top", "left", "bottom", "right"]:
        border = OxmlElement(f"w:{side}")
        border.set(qn("w:val"), "single")
        border.set(qn("w:sz"), "4")
        border.set(qn("w:space"), "4")
        border.set(qn("w:color"), "AAAAAA")
        pBdr.append(border)
    pPr.append(pBdr)
    run = p.add_run(code_text)
    run.font.name = "Courier New"
    run.font.size = Pt(9)
    run.font.color.rgb = RGBColor(0x55, 0x55, 0x55)
    return p


def add_table(doc, rows_data, col_widths=None):
    first_row = rows_data[0]
    table = doc.add_table(rows=len(rows_data), cols=len(first_row))
    table.style = "Table Grid"
    for i, row_data in enumerate(rows_data):
        row = table.rows[i]
        for j, cell_text in enumerate(row_data):
            cell = row.cells[j]
            cell.text = ""
            p = cell.paragraphs[0]
            p.alignment = WD_ALIGN_PARAGRAPH.CENTER if i == 0 else WD_ALIGN_PARAGRAPH.LEFT
            run = p.add_run(str(cell_text))
            run.font.name = DEFAULT_FONT
            run.font.size = Pt(11 if i == 0 else 10)
            run.font.bold = (i == 0)
            r = run._element
            rPr = r.get_or_add_rPr()
            rFonts = OxmlElement("w:rFonts")
            rFonts.set(qn("w:eastAsia"), DEFAULT_FONT)
            rFonts.set(qn("w:ascii"), DEFAULT_FONT)
            rFonts.set(qn("w:hAnsi"), DEFAULT_FONT)
            rPr.insert(0, rFonts)
    return table


def is_table_row(line):
    return line.startswith("|") and line.endswith("|")


def parse_table(lines, start_idx):
    rows = []
    while start_idx < len(lines) and is_table_row(lines[start_idx]):
        line = lines[start_idx]
        cells = [c.strip() for c in line.strip("|").split("|")]
        if not all(c in ("---", "") for c in cells):
            rows.append(cells)
        start_idx += 1
    return rows, start_idx


# ── Parse markdown don gian ─────────────────────────────────────────────────

def parse_inline(text):
    text = re.sub(r"\*\*(.+?)\*\*", r"\1", text)
    text = re.sub(r"\*(.+?)\*", r"\1", text)
    text = re.sub(r"`(.+?)`", r"\1", text)
    text = re.sub(r"\[(.+?)\]\(.+?\)", r"\1", text)
    return text


def md_to_docx(md_path, docx_path):
    with open(md_path, "r", encoding="utf-8") as f:
        content = f.read()

    lines = content.split("\n")

    doc = docx.Document()
    set_page_margins(doc)

    i = 0

    while i < len(lines):
        line = lines[i]

        # ── Comment/HTML ──────────────────────────────────────
        if re.match(r"^<!--.*-->$", line.strip()) or line.strip().startswith("<!--"):
            i += 1
            continue

        # ── Tieu de # ─────────────────────────────────────────
        m = re.match(r"^(#{1,6})\s+(.*)", line)
        if m:
            level = len(m.group(1))
            text = parse_inline(m.group(2).strip())
            add_heading(doc, text, level)
            i += 1
            continue

        # ── Bang ──────────────────────────────────────────────
        if is_table_row(line):
            rows, i = parse_table(lines, i)
            if len(rows) >= 2:
                add_table(doc, rows)
            continue

        # ── Code block ────────────────────────────────────────
        if line.strip().startswith("```"):
            lang = line.strip()[3:]
            code_lines = []
            i += 1
            while i < len(lines) and not lines[i].strip().startswith("```"):
                code_lines.append(lines[i])
                i += 1
            code_text = "\n".join(code_lines)
            add_code_block(doc, code_text)
            i += 1
            continue

        # ── Horizontal rule ───────────────────────────────────
        if re.match(r"^---+$", line.strip()):
            i += 1
            continue

        # ── Empty line ─────────────────────────────────────────
        if line.strip() == "":
            i += 1
            continue

        # ── Normal paragraph ──────────────────────────────────
        text = parse_inline(line.strip())
        if text:
            add_paragraph(doc, text)

        i += 1

    doc.save(docx_path)
    print(f"Da tao file: {docx_path}")


if __name__ == "__main__":
    md_to_docx(INPUT_FILE, OUTPUT_FILE)
