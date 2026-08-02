# -*- coding: utf-8 -*-
"""
Chuyen file 04_TAI_LIEU_RIENG_AI.md -> 04_TAI_LIEU_RIENG_AI.docx
Chi danh cho file tai lieu rieng ve AI.
"""

import sys
from pathlib import Path

# Import ham convert tu file chinh
sys.path.insert(0, str(Path(__file__).parent))
from convert_md_to_docx import convert_md_to_docx

if __name__ == "__main__":
    base = Path(__file__).parent
    convert_md_to_docx(
        base / "04_TAI_LIEU_RIENG_AI.md",
        base / "04_TAI_LIEU_RIENG_AI.docx",
    )