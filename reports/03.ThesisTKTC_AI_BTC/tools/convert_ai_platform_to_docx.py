# -*- coding: utf-8 -*-
"""Convert AI_Platform_TKTC.md -> AI_Platform_TKTC.docx"""

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent))
from convert_md_to_docx import convert_md_to_docx

if __name__ == "__main__":
    report_dir = Path(__file__).resolve().parent.parent
    convert_md_to_docx(
        report_dir / "chapters" / "AI_Platform_TKTC.md",
        report_dir / "outputs" / "AI_Platform_TKTC.docx",
    )
