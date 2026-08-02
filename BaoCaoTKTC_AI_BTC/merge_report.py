# -*- coding: utf-8 -*-
"""
Script gộp các file markdown thành phần của Báo cáo
"Thiết kế chức năng Hệ thống AI phục vụ Văn phòng số Bộ Tài Chính"

Cách chạy:
    python merge_report.py
"""

import os
import sys
from pathlib import Path

if sys.platform == "win32":
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

BASE_DIR = Path(__file__).parent
OUTPUT_FILE = BASE_DIR / "BaoCaoTKTC_AI_BTC_Full.md"

# Thứ tự file theo đúng cấu trúc tài liệu
FILES_ORDER = [
    "00_BIA_VA_LOI_NOI_DAU.md",
    "01_PHAN_I_TONG_QUAN.md",
    "02_PHAN_II_KIEN_TRUC.md",
    "03_PHAN_II_5_THIET_KE_CHI_TIET.md",
]


def merge_files():
    parts = []
    for fname in FILES_ORDER:
        fpath = BASE_DIR / fname
        if not fpath.exists():
            print(f"[WARN] Khong tim thay file: {fname}")
            continue
        with open(fpath, "r", encoding="utf-8") as f:
            content = f.read().strip()
        parts.append(content)
        print(f"[OK] Doc xong: {fname} ({len(content)} chars)")

    full = "\n\n---\n\n".join(parts)
    full = (
        "<!-- Tai lieu thiet ke chuc nang - He thong AI phuc vu Van phong so Bo Tai Chinh -->\n\n"
        + full
        + "\n"
    )

    with open(OUTPUT_FILE, "w", encoding="utf-8") as f:
        f.write(full)
    print(f"\n[OK] Gop xong -> {OUTPUT_FILE}")
    print(f"     Tong ky tu: {len(full):,}")


if __name__ == "__main__":
    merge_files()
