#!/usr/bin/env python
# -*- coding: utf-8 -*-
# Script gộp các file markdown thành phần thành một file Full.md
# Mục đích: tạo đề cương từ các file chapters

import os
import sys
from pathlib import Path

# Fix encoding cho stdout trên Windows
if sys.platform == "win32":
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

# Cấu hình theo bố cục reports/<report>/{chapters,outputs,tools}.
REPORT_DIR = Path(__file__).resolve().parent.parent
CHAPTERS_DIR = REPORT_DIR / "chapters"
OUTPUTS_DIR = REPORT_DIR / "outputs"
OUTPUT_FILE = OUTPUTS_DIR / "BaoCaoDeAn_ThacSi_NguyenTienDat_Full.md"

# Thứ tự file theo đúng cấu trúc đề cương
FILES_ORDER = [
    "00_BIA_VA_LOI_NOI_DAU.md",
    "00_LOI_CAM_ON.md",
    "00_LOI_NOI_DAU.md",
    "01_MUC_LUC.md",
    "10_DANH_MUC_VIET_TAT.md",
    "02_MO_DAU.md",
    "03_CHUONG_1_CO_SO_LY_LUAN.md",
    "04_CHUONG_2_THIET_KE.md",
    "05_CHUONG_3_THUC_NGHIEM.md",
    "06_KET_LUAN.md",
    "07_TAI_LIEU_THAM_KHAO.md",
    "11_KE_HOACH_THUC_HIEN.md",
]


def merge_files():
    """Merge tất cả file markdown theo thứ tự đã định."""
    parts = []
    missing = [fname for fname in FILES_ORDER if not (CHAPTERS_DIR / fname).exists()]
    if missing:
        raise FileNotFoundError("Thiếu file chương bắt buộc: " + ", ".join(missing))

    for fname in FILES_ORDER:
        fpath = CHAPTERS_DIR / fname
        with open(fpath, "r", encoding="utf-8") as f:
            content = f.read().strip()
        if not content or content.startswith("<!-- PLACEHOLDER"):
            print(f"[SKIP] Bo qua: {fname} (placeholder/empty)")
            continue
        parts.append(content)
        print(f"[OK] Doc xong: {fname} ({len(content)} chars)")

    full = "\n\n---\n\n".join(parts)
    full = f"<!-- Bao cao do an thac si - Nguyen Tien Dat -->\n\n{full}\n"

    OUTPUTS_DIR.mkdir(parents=True, exist_ok=True)
    with open(OUTPUT_FILE, "w", encoding="utf-8") as f:
        f.write(full)
    print(f"\n[OK] Gop xong -> {OUTPUT_FILE} ({len(full)} chars)")


if __name__ == "__main__":
    merge_files()
