import sys
sys.stdout.reconfigure(encoding='utf-8', errors='replace')
from docx import Document

doc = Document('AI_Platform_TKTC_v2.docx')

# Find paragraphs containing ASCII art (╔ ═ ╗ ... or ┌ ─ ┐)
print('=== TIM PARAGRAPHS CHUA ASCII ART ===')
count = 0
for i, p in enumerate(doc.paragraphs):
    if any(c in p.text for c in ['╔', '═', '║', '╚', '╠', '╬', '╣', '★', '┌', '┐', '└', '┘', '├', '▼', '►', '◄']):
        font_name = p.runs[0].font.name if p.runs else 'N/A'
        font_size = p.runs[0].font.size if p.runs and p.runs[0].font.size else 'N/A'
        print(f'  Pt {i}: size={font_size} font={font_name}')
        print(f'    Text begin: {p.text[:80]}')
        count += 1
        if count >= 4:
            break

print(f'\nTim thay {count} paragraph co ASCII art')
print()

# Kiem tra trong tables
print('=== TIM TABLES CHUA ASCII ART ===')
count_tbl = 0
for i, tbl in enumerate(doc.tables):
    for row in tbl.rows:
        for cell in row.cells:
            if any(c in cell.text for c in ['╔', '═', '║', '╚', '★', '┌', '┐', '└', '┘', '├', '▼', '►', '◄']):
                print(f'  Table {i+1}: cell text begin: {cell.text[:80]}')
                count_tbl += 1
                if count_tbl >= 3:
                    break
        if count_tbl >= 3:
            break
    if count_tbl >= 3:
        break
print(f'Tong tables co ASCII art: {count_tbl}')