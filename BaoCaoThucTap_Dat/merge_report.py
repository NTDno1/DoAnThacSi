#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""
Script gộp các file markdown thành phần thành một file Full.md
Báo cáo thực tập - Nguyễn Tiến Đạt
"""

import os
import sys
from pathlib import Path

# Fix encoding cho stdout trên Windows
if sys.platform == "win32":
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

# Cấu hình
BASE_DIR = Path(__file__).parent
OUTPUT_FILE = BASE_DIR / "BaoCaoThucTap_NguyenTienDat_Full.md"

# Thứ tự file theo đúng cấu trúc báo cáo
FILES_ORDER = [
    "00_BIA_VA_LOI_NOI_DAU.md",
    "01_MUC_LUC.md",
    "08_DANH_MUC.md",
    "02_MO_DAU.md",
    "03_CHUONG_1_CO_SO_LY_LUAN.md",
    "04_CHUONG_2_THIET_KE.md",
    "05_CHUONG_3_THUC_NGHIEM.md",
    "06_KET_LUAN.md",
    "07_TAI_LIEU_THAM_KHAO.md",
]


def merge_files():
    """Merge tất cả file markdown theo thứ tự đã định."""
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
    full = f"<!-- Bao cao thuc tap - Nguyen Tien Dat -->\n\n{full}\n"

    with open(OUTPUT_FILE, "w", encoding="utf-8") as f:
        f.write(full)
    print(f"\n[OK] Gop xong -> {OUTPUT_FILE} ({len(full)} chars)")


if __name__ == "__main__":
    merge_files()
