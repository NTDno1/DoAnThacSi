import pdfplumber
import sys

pdf_path = r"c:\Users\datt\Documents\GitProject\DoAnThacSI\B25CHHT046_Trần Quang Ninh_BaoCaoDeAnTTTN.pdf"

out_path = r"c:\Users\datt\Documents\GitProject\DoAnThacSI\sample_content.txt"

with open(out_path, "w", encoding="utf-8") as f:
    with pdfplumber.open(pdf_path) as pdf:
        f.write(f"Total pages: {len(pdf.pages)}\n\n")
        for i, page in enumerate(pdf.pages):
            text = page.extract_text() or ""
            f.write(f"\n===== PAGE {i+1} =====\n")
            f.write(text)
            f.write("\n")
print("Done")
