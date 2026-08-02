# Script ghép các file markdown thành báo cáo hoàn chỉnh

import os

BASE_DIR = r"c:\Users\datt\Documents\GitProject\DoAnThacSI\BaoCaoDeAn"

# Thứ tự ghép file
FILE_ORDER = [
    ("00_BIA_VA_LOI_NOI_DAU.md", "BIA_TRANG_BIA"),
    ("01_MUC_LUC.md", "MUC_LUC"),
    ("08_DANH_MUC_BANG.md", "DANH_MUC_BANG"),
    ("09_DANH_MUC_HINH.md", "DANH_MUC_HINH"),
    ("10_DANH_MUC_VIET_TAT.md", "DANH_MUC_VIET_TAT"),
    ("02_MO_DAU.md", "MO_DAU"),
    ("03_CHUONG_1_CO_SO_LY_LUAN.md", "CHUONG_1"),
    ("04_CHUONG_2_THIET_KE.md", "CHUONG_2"),
    ("05_CHUONG_3_THUC_NGHIEM.md", "CHUONG_3"),
    ("06_KET_LUAN.md", "KET_LUAN"),
    ("07_TAI_LIEU_THAM_KHAO.md", "TAI_LIEU_THAM_KHAO"),
]

OUTPUT_FILE = os.path.join(BASE_DIR, "BaoCaoDeAn_ThacSi_Full.md")

def merge_markdown_files():
    merged_content = []

    for filename, section_id in FILE_ORDER:
        filepath = os.path.join(BASE_DIR, filename)
        if not os.path.exists(filepath):
            print(f"WARNING: File not found: {filepath}")
            continue

        with open(filepath, "r", encoding="utf-8") as f:
            content = f.read()

        merged_content.append(f"\n\n<!-- FILE: {filename} -->\n\n")
        merged_content.append(content)
        print(f"Added: {filename} ({len(content)} chars)")

    with open(OUTPUT_FILE, "w", encoding="utf-8") as f:
        f.writelines(merged_content)

    print(f"\nMerged file saved to: {OUTPUT_FILE}")
    total = sum(len(c) for c in merged_content)
    print(f"Total characters: {total:,}")

if __name__ == "__main__":
    merge_markdown_files()
