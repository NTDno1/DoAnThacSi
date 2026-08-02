# -*- coding: utf-8 -*-
"""Convert AI_Platform_TKTC.md -> AI_Platform_TKTC.docx"""

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent))
from convert_md_to_docx import convert_md_to_docx

if __name__ == "__main__":
    base = Path(__file__).parent
    convert_md_to_docx(
        base / "AI_Platform_TKTC.md",
        base / "AI_Platform_TKTC.docx",
    )