import sys, re
sys.stdout.reconfigure(encoding='utf-8', errors='replace')
from docx import Document

doc = Document('AI_Platform_TKTC_v2.docx')
print('=== TONG KET FILE AI_Platform_TKTC_v2.docx ===')
print(f'Tong paragraphs: {len(doc.paragraphs)}')
print(f'Tong tables:     {len(doc.tables)}')
print(f'File size:       106,433 bytes')
print()

# Kiem tra so do co trong cac bang (tim bang co nhieu dong)
print('=== VI DU CAC BANG TRONG FILE ===')
for i, tbl in enumerate(doc.tables[:5]):
    if len(tbl.rows) > 0:
        first_row = ' | '.join(cell.text.strip()[:30] for cell in tbl.rows[0].cells)
        print(f'  Bang {i+1}: {len(tbl.rows)} dong | {first_row[:120]}')

print()
print('=== SO DO BANG KY TU (ASCII art) ===')
# Trong cac cell cua bang, cac ky tu dac biet nhu ╔ ═ ║ ╚ ║ ╔ ═ ╚ ║ │ ─ ▼ ► ◄
count = 0
for tbl in doc.tables:
    for row in tbl.rows:
        for cell in row.cells:
            special_chars = ['╔', '═', '╚', '║', '╠', '╬', '╣', '╦', '╩', '┌', '┐', '└', '┘', '├', '┤', '─', '│', '▼', '▲', '►', '◄', '◊', '★', '◆', '■']
            for c in special_chars:
                if c in cell.text:
                    count += 1
                    break
print(f'Tong so o bang chua ASCII art: {count}')

print()
print('=== TIEU DE PHAN HE 11 PE AS LX.X ===')
for p in doc.paragraphs:
    text = p.text.strip()
    if re.match(r'^II\.5\.\d+\.\s*Phân hệ', text):
        if p.runs:
            size = p.runs[0].font.size
            bold = p.runs[0].font.bold
            if size and bold and size.pt >= 14:
                print(f'  {text}')