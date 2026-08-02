"""Tao 7 file .drawio chuan cho draw.io web (mxGraphModel format)."""
import os

OUT_DIR = '.'
os.makedirs(OUT_DIR, exist_ok=True)


def make_drawio_xml(diagram_id, diagram_name, page_w, page_h, cells):
    """Tao file drawio XML chuan mxGraphModel.

    cells: list of dicts voi keys:
        - id, value, style, x, y, w, h (cho vertex)
        - id, source, target, style (cho edge)
    """
    xml = ['<?xml version="1.0" encoding="UTF-8"?>']
    xml.append('<mxfile host="app.diagrams.net" modified="2026-08-02T10:30:00.000Z" agent="Cursor" version="24.0.0" type="device">')
    xml.append(f'  <diagram id="{diagram_id}" name="{diagram_name}">')
    xml.append(f'    <mxGraphModel dx="1422" dy="794" grid="1" gridSize="10" guides="1" tooltips="1" connect="1" arrows="1" fold="1" page="1" pageScale="1" pageWidth="{page_w}" pageHeight="{page_h}" math="0" shadow="0">')
    xml.append('      <root>')
    # Required: root layer cell id="1"
    xml.append('        <mxCell id="1" parent="0"/>')

    for cell in cells:
        cid = cell['id']
        kind = cell.get('kind', 'vertex')
        value = cell.get('value', '')
        style = cell.get('style', '')
        # Escape XML - escape & FIRST, sau do moi escape < >
        value = value.replace('&', '&amp;').replace('<', '&lt;').replace('>', '&gt;').replace('"', '&quot;')

        if kind == 'vertex':
            x, y, w, h = cell['x'], cell['y'], cell['w'], cell['h']
            xml.append(f'        <mxCell id="{cid}" value="{value}" style="{style}" vertex="1" parent="1">')
            xml.append(f'          <mxGeometry x="{x}" y="{y}" width="{w}" height="{h}" as="geometry"/>')
            xml.append('        </mxCell>')
        else:  # edge
            source = cell['source']
            target = cell['target']
            edge_label = cell.get('label', '')
            xml.append(f'        <mxCell id="{cid}" style="{style}" edge="1" parent="1" source="{source}" target="{target}">')
            xml.append(f'          <mxGeometry relative="1" as="geometry"/>')
            if edge_label:
                edge_label = edge_label.replace('&', '&amp;').replace('<', '&lt;').replace('>', '&gt;').replace('"', '&quot;')
                xml.append(f'          <mxCell id="{cid}_label" value="{edge_label}" style="edgeLabel;html=1;align=center;verticalAlign=middle;resizable=0;points=[];fontSize=11;" vertex="1" connectable="0" parent="{cid}">')
                xml.append(f'            <mxGeometry x="-0.1" relative="1" as="geometry"><mxPoint as="offset"/></mxGeometry>')
                xml.append('          </mxCell>')
            xml.append('        </mxCell>')

    xml.append('      </root>')
    xml.append('    </mxGraphModel>')
    xml.append('  </diagram>')
    xml.append('</mxfile>')
    return '\n'.join(xml)


# ============================================================
# STYLE HELPERS
# ============================================================
def style_box(fill='#ffffff', stroke='#666666', font_size=12, bold=False, rounded=1):
    bold_int = 1 if bold else 0
    rounded_int = 1 if rounded else 0
    return f'rounded={rounded_int};whiteSpace=wrap;html=1;fillColor={fill};strokeColor={stroke};fontSize={font_size};fontStyle={bold_int}'


def style_layer(fill='#ffe6cc', stroke='#d79b00', font_size=14):
    return f'rounded=0;whiteSpace=wrap;html=1;fillColor={fill};strokeColor={stroke};verticalAlign=top;fontSize={font_size};fontStyle=1'


def style_text(font_size=20, bold=True, color='#0F2A50'):
    bold_int = 1 if bold else 0
    return f'text;html=1;strokeColor=none;fillColor=none;align=center;verticalAlign=middle;whiteSpace=wrap;rounded=0;fontSize={font_size};fontStyle={bold_int};fontColor={color}'


def style_edge(color='#666666', width=1.5, dashed=False, label=''):
    dash_str = ';dashed=1' if dashed else ''
    return f'edgeStyle=none;html=1;exitX=1;exitY=0.5;exitDx=0;exitDy=0;entryX=0;entryY=0.5;entryDx=0;entryDy=0;strokeColor={color};strokeWidth={width}{dash_str}'


# ============================================================
# SO DO 01: KIẾN TRÚC TỔNG THỂ 7 TẦNG
# ============================================================
def diagram_01():
    cells = []
    # Title
    cells.append(dict(id='title', value='PHAN MEM VAN PHONG SO BO TAI CHINH - KIEN TRUC TONG THE (7 TANG)',
                      style=style_text(font_size=20, bold=True), x=40, y=20, w=2200, h=50))

    # Tang 1 - Trinh bay
    cells.append(dict(id='tang1_box', value='TANG 1 - TRINH BAY (Nguoi dung nhin thay)',
                      style=style_layer('#dae8fc', '#6c8ebf', 14), x=40, y=100, w=2200, h=200))
    cells.append(dict(id='t1_web', value='Web Portal (Lanh dao, van thu)', style=style_box('#ffffff', '#6c8ebf', 12), x=80, y=140, w=300, h=140))
    cells.append(dict(id='t1_mobile', value='Mobile App (Chuyen vien)', style=style_box('#ffffff', '#6c8ebf', 12), x=420, y=140, w=300, h=140))
    cells.append(dict(id='t1_chatops', value='ChatOps UI (Hoi-dap AI bang tieng Viet)', style=style_box('#fff2cc', '#d6b656', 12, True), x=760, y=140, w=400, h=140))
    cells.append(dict(id='t1_admin', value='AI Admin Console (Quan tri AI)', style=style_box('#fff2cc', '#d6b656', 12, True), x=1200, y=140, w=400, h=140))

    # Tang 2 - API Gateway
    cells.append(dict(id='tang2_box', value='TANG 2 - API GATEWAY (Kong) - JWT - Rate-limit - DDoS - Dinh tuyen API',
                      style=style_layer('#d5e8d4', '#82b366', 14), x=40, y=330, w=2200, h=100))

    # Tang 3 - BFF
    cells.append(dict(id='tang3_box', value='TANG 3 - BFF (.NET 10) - Dieu phoi yeu cau - Gom du lieu tu nhieu microservice',
                      style=style_layer('#d5e8d4', '#82b366', 14), x=40, y=460, w=2200, h=100))

    # Tang 4 - Microservice nghiep vu
    cells.append(dict(id='tang4_box', value='TANG 4 - MICROSERVICE NGHIEP VU (Hien huu)',
                      style=style_layer('#e1d5e7', '#9673a6', 14), x=40, y=590, w=2200, h=180))
    for i, name in enumerate(['DMS\nVan ban', 'Workflow\nCong viec', 'Files\nDinh kem', 'Identity\nUser', 'HRM\nNhan su', 'Notification', 'ERP']):
        cells.append(dict(id=f't4_{i}', value=name, style=style_box('#ffffff', '#9673a6', 11), x=80 + i*305, y=640, w=280, h=110))

    # Tang 5 - MODULE AI (TAM DIEM)
    cells.append(dict(id='tang5_box', value='★ TANG 5 - MICROSERVICE AI (MOI - MODULE AI CUA DE AN) ★',
                      style=style_layer('#fad7ac', '#b46504', 14), x=40, y=800, w=2200, h=860))
    # AI Gateway
    cells.append(dict(id='ai1', value='[1] AI GATEWAY (Cong tiep nhan duy nhat)\nXac thuc - Gioi han - Chong injection - Che PII - Chon provider',
                      style=style_box('#fad7ac', '#b46504', 11, True), x=750, y=850, w=800, h=100))
    # Engine + Agent
    cells.append(dict(id='ai2', value='[2] AI ENGINE CORE\n(Tom tat - Hoi-dap - Phan loai)', style=style_box('#d5e8d4', '#82b366', 11, True), x=450, y=1000, w=550, h=100))
    cells.append(dict(id='ai3', value='[3] AGENT ORCHESTRATION\n(Tro ly da buoc - Human-in-the-Loop)', style=style_box('#d5e8d4', '#82b366', 11, True), x=1300, y=1000, w=550, h=100))
    # Provider + 3 LLMs
    cells.append(dict(id='ai6', value='[6] PROVIDER ABSTRACTION (LLM)', style=style_box('#dae8fc', '#6c8ebf', 12, True), x=750, y=1150, w=800, h=70))
    cells.append(dict(id='ai6_openai', value='OpenAI Cloud (gpt-4o-mini)', style=style_box('#ffffff', '#6c8ebf', 11), x=420, y=1260, w=350, h=80))
    cells.append(dict(id='ai6_claude', value='Anthropic (Claude)', style=style_box('#ffffff', '#6c8ebf', 11), x=970, y=1260, w=350, h=80))
    cells.append(dict(id='ai6_ollama', value='Ollama Local ⭐ (qwen2.5:14b)\nVAN BAN MAT', style=style_box('#f8cecc', '#b85450', 11, True), x=1520, y=1260, w=400, h=80))
    # Knowledge + OCR
    cells.append(dict(id='ai4', value='[4] KNOWLEDGE BASE &amp; RAG\n(Tim nghia - Ingestion)', style=style_box('#d5e8d4', '#82b366', 11, True), x=120, y=1380, w=550, h=100))
    cells.append(dict(id='ai5', value='[5] OCR &amp; DOC EXTRACTION\n(OCR tieng Viet)', style=style_box('#d5e8d4', '#82b366', 11, True), x=1650, y=1380, w=550, h=100))
    # Job Queue
    cells.append(dict(id='ai7', value='[7] AI JOB &amp; TASK QUEUE (Kafka) - Hang doi xu ly tac vu nang', style=style_box('#e1d5e7', '#9673a6', 12, True), x=750, y=1420, w=800, h=80))
    # MCP + Audit
    cells.append(dict(id='ai8', value='[8] MCP Server\n(Kho cong cu cho Agent)', style=style_box('#ffe6cc', '#d79b00', 11), x=120, y=1530, w=300, h=100))
    cells.append(dict(id='ai9', value='[9] Audit &amp; Observability\n(Log - Metric - Trace)', style=style_box('#ffe6cc', '#d79b00', 11), x=1900, y=1530, w=300, h=100))
    # Admin + Auth
    cells.append(dict(id='ai10', value='[10] AI Admin Console\n(Bang dieu khien cho admin)', style=style_box('#fad7ac', '#b46504', 11), x=450, y=1530, w=600, h=100))
    cells.append(dict(id='ai11', value='[11] Auth &amp; Tenant\n(Phan quyen - RLS - JWT)', style=style_box('#fad7ac', '#b46504', 11), x=1300, y=1530, w=550, h=100))

    # Tang 6
    cells.append(dict(id='tang6_box', value='TANG 6 - SU KIEN &amp; BO DEM',
                      style=style_layer('#f5f5f5', '#666666', 14), x=40, y=1690, w=2200, h=130))
    cells.append(dict(id='t6_kafka', value='Kafka Cluster (Su kien AI, Job Queue)', style=style_box('#ffffff', '#666666', 11), x=150, y=1740, w=600, h=70))
    cells.append(dict(id='t6_redis', value='Redis Cluster (Cache, rate-limit)', style=style_box('#ffffff', '#666666', 11), x=1500, y=1740, w=600, h=70))

    # Tang 7
    cells.append(dict(id='tang7_box', value='TANG 7 - DU LIEU (bao ve boi DB Firewall)',
                      style=style_layer('#f5f5f5', '#666666', 14), x=40, y=1850, w=2200, h=160))
    cells.append(dict(id='t7_pg', value='PostgreSQL (Nghiep vu + ai_platform)', style=style_box('#ffffff', '#666666', 11), x=80, y=1900, w=450, h=90))
    cells.append(dict(id='t7_pgvector', value='pgvector / Qdrant (Vector tri thuc)', style=style_box('#ffffff', '#666666', 11), x=600, y=1900, w=500, h=90))
    cells.append(dict(id='t7_minio', value='File Server MinIO/S3 (PDF/DOCX scan)', style=style_box('#ffffff', '#666666', 11), x=1200, y=1900, w=500, h=90))
    cells.append(dict(id='t7_chunk', value='Knowledge Chunk (Redis)', style=style_box('#ffffff', '#666666', 11), x=1780, y=1900, w=420, h=90))

    # Ha tang vat ly
    cells.append(dict(id='physical_box', value='HA TANG VAT LY (dat duoi tang 7)',
                      style=style_layer('#e1d5e7', '#9673a6', 14), x=40, y=2040, w=2200, h=180))
    cells.append(dict(id='p_k8s', value='K8s Cluster (CPU)\n11 microservice AI', style=style_box('#ffffff', '#9673a6', 11), x=300, y=2100, w=700, h=90))
    cells.append(dict(id='p_gpu', value='GPU Cluster (3 node A10 24GB)\nOllama LLM + OCR Worker nang', style=style_box('#fad7ac', '#b46504', 11, True), x=1200, y=2100, w=800, h=90))

    # Legend
    cells.append(dict(id='legend', value='CHU THICH MAU:\nXanh duong (Tang 1) = Trinh bay\nXanh la (Tang 2,3,4) = Nghiep vu\nCam (Tang 5) = Module AI - TAM DIEM\nXam (Tang 6,7) = Ha tang du lieu\nTim (Ha tang vat ly) = Server that',
                      style=style_box('#ffffff', '#cccccc', 11), x=40, y=2260, w=2200, h=160))

    # Edges
    cells.append(dict(id='e_top1', kind='edge', style=style_edge('#6c8ebf', 2), source='t1_chatops', target='ai1'))
    cells.append(dict(id='e_top2', kind='edge', style=style_edge('#6c8ebf', 2), source='t1_admin', target='ai1'))
    cells.append(dict(id='e1', kind='edge', style=style_edge('#b85450', 2), source='ai1', target='ai2', label='don gian'))
    cells.append(dict(id='e2', kind='edge', style=style_edge('#b85450', 2), source='ai1', target='ai3', label='phuc tap'))
    cells.append(dict(id='e3', kind='edge', style=style_edge('#666666', 1.5), source='ai2', target='ai6'))
    cells.append(dict(id='e4', kind='edge', style=style_edge('#666666', 1.5), source='ai3', target='ai6'))
    cells.append(dict(id='e5', kind='edge', style=style_edge('#666666', 1), source='ai6', target='ai6_openai'))
    cells.append(dict(id='e6', kind='edge', style=style_edge('#666666', 1), source='ai6', target='ai6_claude'))
    cells.append(dict(id='e7', kind='edge', style=style_edge('#666666', 1), source='ai6', target='ai6_ollama'))
    cells.append(dict(id='e8', kind='edge', style=style_edge('#666666', 1), source='ai2', target='ai4'))
    cells.append(dict(id='e9', kind='edge', style=style_edge('#666666', 1), source='ai3', target='ai5'))

    return make_drawio_xml('ai-platform-tong-the', '01 - Kien truc tong the 7 tang', 2280, 2480, cells)


# ============================================================
# SO DO 02: QUAN HE 11 PHAN HE AI
# ============================================================
def diagram_02():
    cells = []
    cells.append(dict(id='title', value='MO HINH QUAN HE 11 PHAN HE MODULE AI',
                      style=style_text(font_size=20, bold=True), x=40, y=20, w=2200, h=50))

    # AI Gateway
    cells.append(dict(id='ai1', value='[1] AI GATEWAY (Cong tiep nhan duy nhat)\nXac thuc - Gioi han - Chong injection - Che PII - Chon provider - Cache',
                      style=style_box('#fad7ac', '#b46504', 12, True), x=800, y=120, w=700, h=130))

    # Engine + Agent
    cells.append(dict(id='ai2', value='[2] AI ENGINE CORE\n(Tom tat - Hoi-dap - Phan loai - Trich metadata)',
                      style=style_box('#d5e8d4', '#82b366', 12, True), x=500, y=320, w=550, h=130))
    cells.append(dict(id='ai3', value='[3] AGENT ORCHESTRATION\n(Tro ly da buoc - Human-in-the-Loop)',
                      style=style_box('#d5e8d4', '#82b366', 12, True), x=1300, y=320, w=600, h=130))

    # Provider
    cells.append(dict(id='ai6', value='[6] PROVIDER ABSTRACTION (LLM) - OpenAI - Anthropic - Ollama',
                      style=style_box('#dae8fc', '#6c8ebf', 12, True), x=800, y=510, w=700, h=80))

    # MCP + Knowledge + OCR
    cells.append(dict(id='ai8', value='[8] MCP SERVER\n(Kho cong cu cho Agent)\n12+ tools: tim van ban, soan email, ky so...',
                      style=style_box('#ffe6cc', '#d79b00', 12), x=100, y=680, w=480, h=140))
    cells.append(dict(id='ai4', value='[4] KNOWLEDGE BASE &amp; RAG\n(Vector DB - Rerank - Hybrid search - Ingestion)',
                      style=style_box('#d5e8d4', '#82b366', 12, True), x=900, y=680, w=520, h=140))
    cells.append(dict(id='ai5', value='[5] OCR &amp; DOC EXTRACTION\n(Nhan dang van ban scan)',
                      style=style_box('#d5e8d4', '#82b366', 12), x=1750, y=680, w=520, h=140))

    # Job Queue
    cells.append(dict(id='ai7', value='[7] AI JOB &amp; TASK QUEUE (Kafka)\nHang doi xu ly tac vu nang song song',
                      style=style_box('#e1d5e7', '#9673a6', 12, True), x=800, y=880, w=700, h=100))

    # Lop nen tang
    cells.append(dict(id='nen_tang', value='LOP NEN TANG (chay ngam, phuc vu cho 11 phan he)',
                      style=style_layer('#f5f5f5', '#666666', 14), x=40, y=1040, w=2200, h=240))
    cells.append(dict(id='ai11', value='[11] AUTH &amp; TENANT\nPhan quyen - RLS - JWT', style=style_box('#fad7ac', '#b46504', 12), x=120, y=1110, w=480, h=150))
    cells.append(dict(id='ai9', value='[9] AUDIT &amp; OBSERVABILITY\nLog - Metric - Trace', style=style_box('#fad7ac', '#b46504', 12), x=860, y=1110, w=480, h=150))
    cells.append(dict(id='ai10', value='[10] AI ADMIN CONSOLE\n(Bang dieu khien UI cho admin)\nQuan ly tat ca phan he phia tren',
                      style=style_box('#fad7ac', '#b46504', 12), x=1620, y=1110, w=600, h=150))

    # Edges
    cells.append(dict(id='e1', kind='edge', style=style_edge('#82b366', 2), source='ai1', target='ai2', label='don gian'))
    cells.append(dict(id='e2', kind='edge', style=style_edge('#82b366', 2), source='ai1', target='ai3', label='phuc tap'))
    cells.append(dict(id='e3', kind='edge', style=style_edge('#666666', 1.5), source='ai2', target='ai6'))
    cells.append(dict(id='e4', kind='edge', style=style_edge('#666666', 1.5), source='ai3', target='ai6'))
    cells.append(dict(id='e5', kind='edge', style=style_edge('#666666', 1.5), source='ai3', target='ai8'))
    cells.append(dict(id='e6', kind='edge', style=style_edge('#666666', 1.5), source='ai3', target='ai4'))
    cells.append(dict(id='e7', kind='edge', style=style_edge('#666666', 1.5), source='ai2', target='ai4'))
    cells.append(dict(id='e8', kind='edge', style=style_edge('#9673a6', 2), source='ai4', target='ai5'))
    cells.append(dict(id='e9', kind='edge', style=style_edge('#666666', 1.5), source='ai4', target='ai7'))
    cells.append(dict(id='e10', kind='edge', style=style_edge('#666666', 1.5), source='ai5', target='ai7'))

    return make_drawio_xml('ai-platform-11-phan-he', '02 - Quan he 11 phan he AI', 2280, 1340, cells)


# ============================================================
# SO DO 03: LUONG 1 YEU CAU AI (sequence)
# ============================================================
def diagram_03():
    cells = []
    cells.append(dict(id='title', value='LUONG 1 YEU CAU AI - 11 BUOC (User -> Gateway -> Engine -> Knowledge -> LLM)',
                      style=style_text(font_size=18, bold=True), x=40, y=20, w=2600, h=50))

    # 6 columns - lifelines
    cols = [
        ('user', 'Nguoi dung\n(Web/Mobile)', '#dae8fc', '#6c8ebf'),
        ('kong', 'Tang 1-2\n(Kong Gateway)', '#d5e8d4', '#82b366'),
        ('ai_gw', 'AI Gateway\n[1]', '#fad7ac', '#b46504'),
        ('ai_eng', 'AI Engine\n[2]', '#d5e8d4', '#82b366'),
        ('kw', 'Knowledge\n[4]', '#d5e8d4', '#82b366'),
        ('provider', 'Provider LLM\n[6] -> OpenAI/Ollama', '#dae8fc', '#6c8ebf'),
    ]
    col_w = 380
    col_x_start = 100
    col_y = 120
    col_y_lifeline_end = 1180

    for i, (cid, name, fill, stroke) in enumerate(cols):
        x = col_x_start + i*col_w
        cells.append(dict(id=cid, value=name, style=style_box(fill, stroke, 11, True), x=x, y=col_y, w=col_w, h=100))

    # Edges (sequence)
    steps = [
        ('user', 'kong', '1. Dat cau hoi'),
        ('kong', 'ai_gw', '2. Xac thuc JWT - Kiem tra quyen'),
        ('ai_gw', 'ai_eng', '3-4. Chong injection - Che PII - Cache -> Goi AI Engine'),
        ('ai_eng', 'kw', '5. Tim kiem lai: TU KHOA + NGU NGHIA'),
        ('kw', 'ai_eng', '6. Tra ve 8 van ban lien quan nhat'),
        ('ai_eng', 'ai_eng', '7. Xep hang lai (BGE) - Tong hop ngu canh'),
        ('ai_eng', 'provider', '8. Goi LLM - Streaming response'),
        ('provider', 'ai_eng', '9. Tra ve cau tra loi tung phan'),
        ('ai_eng', 'ai_eng', '10. Kiem tra chat luong - Trich dan nguon'),
        ('ai_gw', 'user', '11. Tra cau tra loi + Ghi log + Cap nhat chi phi'),
    ]
    y_offset = 280
    y_step = 90
    for i, (src, dst, label) in enumerate(steps):
        cells.append(dict(id=f'step_{i}', kind='edge',
                          style=style_edge('#b85450' if (src=='ai_gw' and dst=='user') else '#666666', 2,
                                            dashed=(src==dst)),
                          source=src, target=dst, label=label))

    # Legend
    cells.append(dict(id='legend', value='GIAI THICH: MUI TEN DO = BUOC CUOI (tra ket qua user); MUI TEN DEN = CAC BUOC TRUNG GIAN',
                      style=style_box('#fff2cc', '#d6b656', 11), x=100, y=1230, w=2400, h=80))

    return make_drawio_xml('luong-yeu-cau-ai', '03 - Luong 1 yeu cau AI (Sequence)', 2680, 1360, cells)


# ============================================================
# SO DO 04: HA TANG 7 VUNG MANG
# ============================================================
def diagram_04():
    cells = []
    cells.append(dict(id='title', value='SO DO HA TANG TRIEN KHAI - 7 VUNG MANG',
                      style=style_text(font_size=20, bold=True), x=40, y=20, w=2200, h=50))

    # Internet
    cells.append(dict(id='internet', value='INTERNET / INTRANET Bo Tai Chinh',
                      style=style_box('#f5f5f5', '#666666', 13, True), x=800, y=110, w=600, h=80))

    # Vung 1 - Bien
    cells.append(dict(id='v1_box', value='VUNG 1 - BIEN', style=style_layer('#ffe6cc', '#d79b00', 14), x=40, y=230, w=2200, h=180))
    cells.append(dict(id='v1_fw', value='Firewall #1 (HA)\nChong DDoS, loc tan cong', style=style_box('#fad7ac', '#b46504', 12), x=120, y=290, w=500, h=100))
    cells.append(dict(id='v1_waf', value='WAF (HA)\nChong SQL injection, XSS', style=style_box('#fad7ac', '#b46504', 12), x=1620, y=290, w=500, h=100))

    # Vung 2 - LB
    cells.append(dict(id='v2_box', value='VUNG 2 - LOAD BALANCER', style=style_layer('#dae8fc', '#6c8ebf', 14), x=40, y=440, w=2200, h=170))
    cells.append(dict(id='v2_lb1', value='Load Balancer #1\nSSL/TLS Termination', style=style_box('#ffffff', '#6c8ebf', 12), x=300, y=510, w=550, h=90))
    cells.append(dict(id='v2_lb2', value='Load Balancer #2\nSSL/TLS Termination', style=style_box('#ffffff', '#6c8ebf', 12), x=1400, y=510, w=550, h=90))

    # Vung 3 - Proxy
    cells.append(dict(id='v3_box', value='VUNG 3 - PROXY / API GATEWAY', style=style_layer('#d5e8d4', '#82b366', 14), x=40, y=640, w=2200, h=170))
    cells.append(dict(id='v3_nginx', value='NGINX\nReverse proxy, SSL termination', style=style_box('#ffffff', '#82b366', 12), x=300, y=710, w=550, h=90))
    cells.append(dict(id='v3_kong', value='Kong Gateway\nRouting, rate-limit, JWT verify', style=style_box('#ffffff', '#82b366', 12), x=1400, y=710, w=550, h=90))

    # Vung 4 - Firewall Core
    cells.append(dict(id='v4_box', value='VUNG 4 - FIREWALL CORE (IDS/IPS)', style=style_layer('#ffe6cc', '#d79b00', 14), x=40, y=840, w=2200, h=90))

    # Vung 5 - Application & Processing
    cells.append(dict(id='v5_box', value='VUNG 5 - UNG DUNG &amp; XU LY (Compute Zone)', style=style_layer('#fad7ac', '#b46504', 14), x=40, y=960, w=2200, h=620))
    # K8s CPU
    cells.append(dict(id='k8s_box', value='K8s Cluster (CPU) - 26 node', style=style_layer('#dae8fc', '#6c8ebf', 13), x=80, y=1030, w=1100, h=520))
    k8s_components = ['AI GW x 3', 'AI Engine x 4', 'Agent x 3', 'Admin API x 2', 'MCP x 2',
                      'Knowledge x 3', 'Auth x 2', 'Audit x 2', 'OCR Worker\nx 2 (GPU T4)', 'Buffer x 3']
    for i, c in enumerate(k8s_components):
        col = i % 5
        row = i // 5
        cells.append(dict(id=f'k8s_{i}', value=c, style=style_box('#ffffff', '#6c8ebf', 10), x=100 + col*215, y=1090 + row*120, w=200, h=110))
    cells.append(dict(id='k8s_master', value='K8s Master (3 node control plane)\nTong: ~340 vCPU, ~656 GB RAM, ~10 TB SSD',
                      style=style_box('#e1d5e7', '#9673a6', 11), x=100, y=1410, w=1060, h=120))

    # GPU Cluster
    cells.append(dict(id='gpu_box', value='GPU Cluster (7 node, 3 A10 24GB + 4 T4 16GB)', style=style_layer('#fad7ac', '#b46504', 13), x=1220, y=1030, w=980, h=520))
    cells.append(dict(id='gpu_ollama', value='Ollama LLM x 3 node (A10 24GB) - qwen2.5:14b\nCong suat: 30-45 yeu cau/giay (gap 10 lan peak)',
                      style=style_box('#ffffff', '#b46504', 12, True), x=1260, y=1090, w=920, h=130))
    cells.append(dict(id='gpu_ocr', value='OCR Engine x 2 (T4)\nPaddleOCR-VL + VietOCR', style=style_box('#ffffff', '#b46504', 12), x=1260, y=1250, w=440, h=130))
    cells.append(dict(id='gpu_embed', value='Embedding Server x 2 (T4)\nBGE-M3 multilingual', style=style_box('#ffffff', '#b46504', 12), x=1740, y=1250, w=440, h=130))
    cells.append(dict(id='gpu_note', value='GPU A10 24GB chay qwen2.5:14b (<8s response)', style=style_box('#fff2cc', '#d6b656', 11), x=1260, y=1410, w=920, h=120))

    # Vung 6 - Su kien & bo dem
    cells.append(dict(id='v6_box', value='VUNG 6 - SU KIEN &amp; BO DEM', style=style_layer('#e1d5e7', '#9673a6', 14), x=40, y=1610, w=2200, h=200))
    cells.append(dict(id='kafka', value='Kafka Cluster x 3 broker (8 broker de xuat)\nEvents + Job Queue', style=style_box('#ffffff', '#9673a6', 12), x=120, y=1670, w=600, h=120))
    cells.append(dict(id='redis_c', value='Redis Cluster x 3 node (32 GB RAM)\nCache + rate-limit + session', style=style_box('#ffffff', '#9673a6', 12), x=1620, y=1670, w=600, h=120))

    # Vung 7 - Du lieu
    cells.append(dict(id='v7_box', value='VUNG 7 - DU LIEU (bao ve boi DB Firewall)', style=style_layer('#f5f5f5', '#666666', 14), x=40, y=1840, w=2200, h=280))
    cells.append(dict(id='pg', value='PostgreSQL HA x 3 (Patroni 1+2)\nchua ai_platform + nghiep vu', style=style_box('#ffffff', '#666666', 12), x=120, y=1900, w=550, h=180))
    cells.append(dict(id='vector', value='Vector DB x 3 (pgvector)\n10M vector - 60 GB\nIndex HNSW', style=style_box('#ffffff', '#666666', 12), x=820, y=1900, w=550, h=180))
    cells.append(dict(id='minio', value='MinIO Object Storage x 4\n16 TB tong - PDF/DOCX scan goc', style=style_box('#ffffff', '#666666', 12), x=1520, y=1900, w=600, h=180))

    # Tong ket
    cells.append(dict(id='summary_box', value='TONG KET TOAN BO HA TANG', style=style_layer('#0F2A50', '#0F2A50', 14), x=40, y=2150, w=2200, h=220))
    cells.append(dict(id='summary_txt', value='~60 server (CPU + GPU) + 8 thiet bi mang/bao mat\n~620 vCPU - ~1,7 TB RAM - ~47 TB luu tru\nTong dau tu CAPEX: ~18,1 ty VNĐ\nTong OPEX / nam: ~4,0 ty VNĐ (van hanh + AI cloud)\nTong 5 nam: ~38-40 ty VNĐ',
                      style='text;html=1;align=left;verticalAlign=top;fontSize=13;fontColor=white;',
                      x=120, y=2200, w=2080, h=160))

    # Edges
    cells.append(dict(id='e_i', kind='edge', style=style_edge('#666666', 2), source='internet', target='v1_fw'))
    cells.append(dict(id='e1', kind='edge', style=style_edge('#666666', 2), source='v1_fw', target='v2_lb1'))
    cells.append(dict(id='e2', kind='edge', style=style_edge('#666666', 2), source='v2_lb1', target='v3_nginx'))
    cells.append(dict(id='e3', kind='edge', style=style_edge('#666666', 2), source='v3_kong', target='v4_box'))
    cells.append(dict(id='e4', kind='edge', style=style_edge('#b85450', 3), source='v4_box', target='ai_gw', label='protected'))

    return make_drawio_xml('ha-tang-7-vung', '04 - Ha tang 7 vung mang', 2280, 2410, cells)


# ============================================================
# SO DO 05: PIPELINE RAG
# ============================================================
def diagram_05():
    cells = []
    cells.append(dict(id='title', value='PIPELINE RAG (Truy xuat tang cuong) - CHI TIET 5 BUOC',
                      style=style_text(font_size=18, bold=True), x=40, y=20, w=2800, h=50))

    steps_data = [
        ('step1', 'BUOC 1\nEMBED\n(Vector hoa)', '#dae8fc', '#6c8ebf',
         ['INPUT: Cau hoi user (tieng Viet, 1-3 cau)',
          'Xu ly: BGE-M3 / OpenAI Embed -> Vector 1536 chieu',
          'OUTPUT: Day so 1536 chieu']),
        ('step2', 'BUOC 2\nHYBRID SEARCH\n(Tim kiem lai)', '#d5e8d4', '#82b366',
         ['BM25: Tim theo TU KHOA -> Top 50',
          'Cosine: Tim theo NGU NGHIA -> Top 50',
          'RRF: Hop nhat 2 list -> 50 ung vien']),
        ('step3', 'BUOC 3\nRERANK\n(Xep hang lai)', '#fff2cc', '#d6b656',
         ['Mo hinh: BGE-reranker-v2-m3',
          'Danh gia lai do lien quan -> Chon Top 8',
          'OUTPUT: Top 8 van ban (da cham diem)']),
        ('step4', 'BUOC 4\nLLM\n(Goi mo hinh AI)', '#fad7ac', '#b46504',
         ['Prompt: [System] + [Top 8] + [Cau hoi]',
          'Mat -> Ollama, Thuong -> GPT-4o-mini',
          'OUTPUT: Streaming response']),
        ('step5', 'BUOC 5\nCITE + QUALITY', '#e1d5e7', '#9673a6',
         ['Trich dan: File, trang, khoang',
          'Faithfulness: Cau tra li co trung thuc voi van ban goc?',
          'OUTPUT: Cau tra loi + trich dan + diem tin cay 0-1']),
    ]
    step_w = 540
    step_x_start = 100
    step_y = 120

    for i, (sid, title, fill, stroke, subs) in enumerate(steps_data):
        x = step_x_start + i*step_w
        cells.append(dict(id=sid, value=title, style=style_box(fill, stroke, 14, True), x=x, y=step_y, w=step_w, h=200))
        for j, sub in enumerate(subs):
            cells.append(dict(id=f'{sid}_{j}', value=sub, style=style_box('#ffffff', stroke, 11), x=x, y=step_y + 230 + j*130, w=step_w, h=120))

    # Arrows giua cac buoc
    for i in range(4):
        x1 = step_x_start + i*step_w + step_w
        x2 = step_x_start + (i+1)*step_w
        cells.append(dict(id=f'arrow_{i}', kind='edge', style=style_edge('#b85450', 3), source=f'step{i+1}', target=f'step{i+2}',
                          label=f'buoc {i+1} -> buoc {i+2}'))

    # Legend
    cells.append(dict(id='legend', value='GIAI THICH:  • BGE-M3 = Embedding da ngon ngu  • BM25 = Tim theo tu khoa truyen thong  • RRF = Hop nhat 2 danh sach  • BGE-reranker = Xep hang lai chinh xac hon vector similarity  • Faithfulness = Chong hallucination',
                      style=style_box('#fff2cc', '#d6b656', 11), x=100, y=750, w=2600, h=200))

    return make_drawio_xml('pipeline-rag', '05 - Pipeline RAG chi tiet', 2880, 1000, cells)


# ============================================================
# SO DO 06: CLEAN ARCHITECTURE
# ============================================================
def diagram_06():
    cells = []
    cells.append(dict(id='title', value='CAU TRUC 4 LOP CLEAN ARCHITECTURE BEN TRONG MOI PHAN HE AI\n(Vi du: AI Gateway)',
                      style=style_text(font_size=18, bold=True), x=40, y=20, w=2200, h=80))

    layers = [
        ('api', 'LOP 1 - API (Giao tiep)', '#dae8fc', '#6c8ebf',
         'AiGateway.Api\nREST API endpoint, gRPC service\nJWT middleware xac thuc\nControllers, Request/Response DTO\n\n-> Tiep nhan yeu cau HTTP/gRPC tu client'),
        ('infra', 'LOP 2 - INFRASTRUCTURE (Ha tang)', '#fff2cc', '#d6b656',
         'AiGateway.Infrastructure\nRedis client (cache, rate-limit)\nPostgreSQL client (audit log)\nOpenAI/Anthropic API client\nHTTP client cho KMS service\n\n-> Ket noi voi the gioi ben ngoai'),
        ('app', 'LOP 3 - APPLICATION (Xu ly)', '#d5e8d4', '#82b366',
         'AiGateway.Application\nProcessRequestHandler (MediatR Command)\nPipeline behaviors: validate -> logging -> inject-check\nUse cases: AskQuestion, SummarizeDoc, ClassifyText\n\n-> Quy trinh xu ly yeu cau'),
        ('domain', 'LOP 4 - DOMAIN (Loi nghiep vu)', '#f8cecc', '#b85450',
         'AiGateway.Domain\nEntities: AiRequest, RateLimitCounter, SemanticCacheEntry, AuditLog\nValue Objects: TokenCount, ProviderId, TenantId\nDomain Services: PromptSanitizer, PiiRedactor, ProviderSelector\n\n-> QUY TAC NGHIEP VU COT LOI\n(KHONG phu thuoc framework)'),
    ]
    for i, (lid, title, fill, stroke, content) in enumerate(layers):
        y = 140 + i*210
        h = 180
        if lid == 'domain':
            h = 240
        cells.append(dict(id=lid, value=title, style=style_box(fill, stroke, 14, True), x=350, y=y, w=1800, h=80))
        cells.append(dict(id=f'{lid}_detail', value=content, style=style_box('#ffffff', stroke, 11), x=350, y=y+80, w=1800, h=h-80))

    # Arrows (dependency inward)
    for i in range(3):
        cells.append(dict(id=f'arr_{i}', kind='edge', style=style_edge('#b85450', 3), source=list(layers)[i][0], target=list(layers)[i+1][0], label='goi'))

    # Labels
    cells.append(dict(id='lbl_outer', value='LOP NGOAI\n(phu thuoc framework, co the thay doi)',
                      style='text;html=1;align=center;verticalAlign=middle;fontSize=11;fontColor=#666666;',
                      x=50, y=200, w=280, h=200))
    cells.append(dict(id='lbl_inner', value='LOP TRONG\n(KHONG phu thuoc framework)',
                      style='text;html=1;align=center;verticalAlign=middle;fontSize=11;fontColor=#b85450;fontStyle=1;',
                      x=50, y=800, w=280, h=200))

    # Nguyen tac
    cells.append(dict(id='principle', value='NGUYEN TAC DEPENDENCY INVERSION: Cac lop ngoai phu thuoc vao lop trong (qua interface), KHONG phu thuoc nguoc lai.\n-> Lop DOMAIN khong biet gi ve Redis, PostgreSQL, hay OpenAI.',
                      style='text;html=1;align=center;verticalAlign=middle;fontSize=13;fontStyle=1;fontColor=#0F2A50;strokeColor=#0F2A50;fillColor=#dae8fc;',
                      x=100, y=1050, w=2100, h=120))

    # Loi ich
    cells.append(dict(id='benefits', value='LOI ICH CUA CLEAN ARCHITECTURE:\n- De thay doi ha tang (doi Redis -> Memcached chi can sua lop Infrastructure)\n- De test nghiep vu (lop Domain khong can database de chay test)\n- Tang toc phat trien (cac lop phat trien song song)\n- Giam rui ro khi nang cap (chi thay doi lop can thiet)',
                      style=style_box('#ffffff', '#666666', 12), x=100, y=1200, w=2100, h=200))

    return make_drawio_xml('clean-architecture', '06 - Clean Architecture trong phan he AI', 2280, 1450, cells)


# ============================================================
# SO DO 07: BAO MAT 7 LOP CHO AI
# ============================================================
def diagram_07():
    cells = []
    cells.append(dict(id='title', value='MO HINH BAO MAT 7 LOP CHO MODULE AI',
                      style=style_text(font_size=20, bold=True), x=40, y=20, w=2200, h=50))
    cells.append(dict(id='subtitle', value='(Giong nhu 1 toa nha co 7 lop cua bao ve)',
                      style='text;html=1;align=center;verticalAlign=middle;fontSize=13;fontStyle=2;fontColor=#666666;',
                      x=40, y=70, w=2200, h=40))

    layers = [
        ('l7', 'LOP 7 - NHAT KY KIEM TRA (Audit Log)', '#f5f5f5', '#666666',
         '"So ghi chep" moi hoat dong AI de truy vet khi can\nPhan he phu trach: [9] Audit &amp; Observability'),
        ('l6', 'LOP 6 - PHAT HIEN TAN CONG (IDS/IPS)', '#fad7ac', '#b46504',
         '"Camera an ninh" tu dong phat hien hanh vi bat thuong\nChong prompt injection - Phat hien jailbreak attempt'),
        ('l5', 'LOP 5 - PHAN LOAI DO MAT (Data Classification)', '#f8cecc', '#b85450',
         'Van ban mat -> chi dung AI noi bo (Ollama)\nVan ban thuong -> co the dung AI cloud\nPhan he phu trach: [1] AI Gateway (chon provider theo do nhay)'),
        ('l4', 'LOP 4 - MA HOA DU LIEU (Encryption)', '#e1d5e7', '#9673a6',
         '"Hop kin" bao ve du lieu khi truyen (TLS 1.3) va luu tru (AES-256)\nChe giau PII (CCCD, SDT, email) truoc khi gui AI'),
        ('l3', 'LOP 3 - JWT + PHAN QUYEN (Authorization)', '#fff2cc', '#d6b656',
         '"The nhan vien" gan voi tung vai tro (Admin, Operator, User, Auditor)\nPhan he phu trach: [11] Auth &amp; Tenant + [1] AI Gateway'),
        ('l2', 'LOP 2 - API GATEWAY', '#d5e8d4', '#82b366',
         '"Le tan" kiem tra JWT, gioi han toc do (100 cau/phut/user)\nPhan he phu trach: [1] AI Gateway'),
        ('l1', 'LOP 1 - TUONG LUA BIEN (Firewall + WAF)', '#dae8fc', '#6c8ebf',
         '"Hang rao" ngoai cung, chan tin tac, chong DDoS, loc SQL injection\nTuong lua bien + WAF'),
    ]
    for i, (lid, title, fill, stroke, content) in enumerate(layers):
        y = 130 + i*130
        cells.append(dict(id=lid, value=title, style=style_box(fill, stroke, 14, True), x=350, y=y, w=1700, h=70))
        cells.append(dict(id=f'{lid}_detail', value=content, style=style_box('#ffffff', stroke, 11), x=350, y=y+70, w=1700, h=60))

    # Labels NGOAI / TRONG
    cells.append(dict(id='lbl_outer', value='NGOAI', style='text;html=1;align=center;fontSize=14;fontStyle=1;fontColor=#b85450;',
                      x=80, y=160, w=240, h=130))
    cells.append(dict(id='lbl_inner', value='TRONG', style='text;html=1;align=center;fontSize=14;fontStyle=1;fontColor=#0F2A50;',
                      x=80, y=980, w=240, h=130))

    # Dac biet cho AI
    cells.append(dict(id='special_ai', value='DAC BIET CHO AI:\n• Chong prompt injection\n• Che giau PII\n• Rate limit\n• HITL cho Agent\n• Audit chi tiet',
                      style=style_box('#fff2cc', '#d6b656', 11, True), x=2080, y=600, w=400, h=400))

    # Ghi chu
    cells.append(dict(id='note', value='GIẢI THÍCH BẢO MẬT:\n- Che PII: regex phát hiện CCCD (12 số), SĐT (10 số), email; tự động thay bằng [REDACTED]\n- Giới hạn: 100 câu/phút/user, 5.000 câu/giờ/user, 50.000 câu/tháng/đơn vị\n- Vòng xoay khóa API: tự động thay đổi khóa OpenAI/Anthropic mỗi tháng\n- HITL: thao tác nguy hiểm phải có người duyệt trước khi thực hiện',
                      style=style_box('#ffffff', '#cccccc', 11), x=80, y=1130, w=2400, h=200))

    return make_drawio_xml('security-7-layers', '07 - Bao mat 7 lop cho AI', 2280, 1400, cells)


# ============================================================
# MAIN
# ============================================================
if __name__ == '__main__':
    import os
    os.chdir(os.path.dirname(os.path.abspath(__file__)) if '__file__' in globals() else '.')

    print('='*70)
    print('TAO 7 FILE DRAWIO CHUAN CHO DRAW.IO WEB (mxGraphModel)')
    print('='*70)

    files = [
        ('01_architecture_tong_the_7_tang.drawio', diagram_01),
        ('02_quan_he_11_phan_he.drawio', diagram_02),
        ('03_luong_1_yeu_cau_ai.drawio', diagram_03),
        ('04_ha_tang_7_vung_mang.drawio', diagram_04),
        ('05_pipeline_rag_chi_tiet.drawio', diagram_05),
        ('06_clean_architecture.drawio', diagram_06),
        ('07_bao_mat_7_lop_ai.drawio', diagram_07),
    ]

    for filename, func in files:
        xml = func()
        with open(filename, 'w', encoding='utf-8') as f:
            f.write(xml)
        size = os.path.getsize(filename)
        print(f'[OK] {filename:48} | {size//1024:3d} KB')

    print('='*70)
    print(f'HOAN THANH! 7 file .drawio da duoc tao/refresh trong {os.getcwd()}')