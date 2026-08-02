import xml.etree.ElementTree as ET
import sys
from pathlib import Path

sys.stdout.reconfigure(encoding='utf-8', errors='replace')

DIR = Path('drawio_diagrams')
files = sorted([f for f in DIR.iterdir() if f.suffix == '.drawio'])

print(f'KIEM TRA {len(files)} FILE DRAWIO')
print('='*80)

ok = 0
for f in files:
    try:
        tree = ET.parse(f)
        root = tree.getroot()
        # Dem shapes va edges
        ns = '{http://www.w3.org/2001/XMLSchema-instance}'
        all_cells = list(root.iter())
        shape_count = sum(1 for c in all_cells if c.tag == 'mxCell' and c.get('vertex') == '1')
        edge_count = sum(1 for c in all_cells if c.tag == 'mxCell' and c.get('edge') == '1')
        # Ten so do
        diagram_node = root.find('.//diagram')
        diagram_name = diagram_node.get('name', 'N/A') if diagram_node is not None else 'N/A'

        print(f'[OK]  {f.name:45} | {shape_count:3d} shapes | {edge_count:3d} edges | {diagram_name}')
        ok += 1
    except Exception as e:
        print(f'[FAIL] {f.name:45} | ERROR: {e}')

print()
print(f'TONG: {ok}/{len(files)} file hop le')