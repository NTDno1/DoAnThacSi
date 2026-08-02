"""Ve 7 so do kien truc Module AI thanh PNG chat luong cao (final)."""
import matplotlib.pyplot as plt
import matplotlib.patches as patches
from matplotlib.patches import FancyBboxPatch
from matplotlib import rcParams
import os

rcParams['font.sans-serif'] = ['Segoe UI', 'Arial Unicode MS', 'DejaVu Sans']
rcParams['axes.unicode_minus'] = False

OUT_DIR = 'png_diagrams'
os.makedirs(OUT_DIR, exist_ok=True)

COLOR_LAYER = {
    1: '#dae8fc', 2: '#d5e8d4', 3: '#d5e8d4', 4: '#e1d5e7',
    5: '#fad7ac', 6: '#f5f5f5', 7: '#f5f5f5', 'physical': '#fad7ac',
}
EDGE_COLOR = '#b85450'
TEXT_COLOR = '#0F2A50'


def draw_box(ax, x, y, w, h, text, fill='#ffffff', edge='#666666', fontsize=10, bold=False):
    box = FancyBboxPatch((x, y), w, h,
                         boxstyle='round,pad=0.02,rounding_size=0.08',
                         linewidth=1.5, edgecolor=edge, facecolor=fill)
    ax.add_patch(box)
    weight = 'bold' if bold else 'normal'
    ax.text(x + w/2, y + h/2, text, ha='center', va='center',
            fontsize=fontsize, fontweight=weight, color=TEXT_COLOR, wrap=True)


def draw_layer_box(ax, x, y, w, h, title, fill='#ffe6cc', fontsize=13, edge='#d79b00'):
    box = FancyBboxPatch((x, y), w, h,
                         boxstyle='round,pad=0.02,rounding_size=0.1',
                         linewidth=2, edgecolor=edge, facecolor=fill)
    ax.add_patch(box)
    ax.text(x + w/2, y + h - 0.3, title, ha='center', va='top',
            fontsize=fontsize, fontweight='bold', color=TEXT_COLOR)


def arrow(ax, x1, y1, x2, y2, color='#666666', width=1.5, label=None, linestyle='solid'):
    ax.annotate('', xy=(x2, y2), xytext=(x1, y1),
                arrowprops=dict(arrowstyle='->', color=color, lw=width, linestyle=linestyle))
    if label:
        ax.text((x1+x2)/2, (y1+y2)/2 + 0.2, label, ha='center', va='bottom',
                fontsize=9, color='#666666', style='italic')


def save(fig, name):
    path = os.path.join(OUT_DIR, f'{name}.png')
    fig.savefig(path, dpi=150, bbox_inches='tight', facecolor='white')
    plt.close(fig)
    print(f'[OK] {name}.png ({os.path.getsize(path)//1024} KB)')


# ============================================================
# SO DO 01: KIẾN TRÚC TỔNG THỂ 7 TẦNG
# ============================================================
def diagram_01():
    fig, ax = plt.subplots(figsize=(20, 14))
    ax.set_xlim(0, 20)
    ax.set_ylim(-0.15, 14)
    ax.axis('off')
    ax.text(10, 13.5, 'KIEN TRUC TONG THE 7 TANG - PHAN MEM VAN PHONG SO BO TAI CHINH',
            ha='center', fontsize=18, fontweight='bold', color=TEXT_COLOR)

    # Tang 1
    draw_layer_box(ax, 0.3, 11.5, 19.4, 1.6, 'TANG 1 - TRINH BAY', '#dae8fc', 13, '#6c8ebf')
    draw_box(ax, 0.6, 11.7, 4.2, 1.2, 'Web Portal\n(Lanh dao, van thu)', '#ffffff', '#6c8ebf', 10)
    draw_box(ax, 5.0, 11.7, 4.2, 1.2, 'Mobile App\n(Chuyen vien)', '#ffffff', '#6c8ebf', 10)
    draw_box(ax, 9.4, 11.7, 4.5, 1.2, 'ChatOps UI\n(Hoi-dap AI bang tieng Viet)', '#fff2cc', '#d6b656', 10, bold=True)
    draw_box(ax, 14.1, 11.7, 5.3, 1.2, 'AI Admin Console\n(Quan tri AI)', '#fff2cc', '#d6b656', 10, bold=True)

    # Tang 2
    draw_layer_box(ax, 0.3, 10.3, 19.4, 1.0, 'TANG 2 - API GATEWAY (Kong) - JWT - Rate-limit - DDoS', '#d5e8d4', 11, '#82b366')

    # Tang 3
    draw_layer_box(ax, 0.3, 9.1, 19.4, 1.0, 'TANG 3 - BFF (.NET 10) - Dieu phoi - Gom du lieu', '#d5e8d4', 11, '#82b366')

    # Tang 4
    draw_layer_box(ax, 0.3, 7.4, 19.4, 1.5, 'TANG 4 - MICROSERVICE NGHIEP VU (Hien huu)', '#e1d5e7', 11, '#9673a6')
    for i, name in enumerate(['DMS\nVan ban', 'Workflow\nCong viec', 'Files\nDinh kem', 'Identity\nUser', 'HRM\nNhan su', 'Notification', 'ERP']):
        draw_box(ax, 0.4 + i*2.75, 7.5, 2.5, 1.2, name, '#ffffff', '#9673a6', 10)

    # Tang 5 - MODULE AI
    draw_layer_box(ax, 0.3, 2.0, 19.4, 5.2, '[MODULE AI] TANG 5 - MICROSERVICE AI MOI', '#fad7ac', 13, '#b46504')
    # AI Gateway
    draw_box(ax, 7.5, 6.4, 5.0, 0.7, '[1] AI GATEWAY (Cong tiep nhan duy nhat) - Xac thuc - Che PII - Chon provider', '#fad7ac', '#b46504', 10, bold=True)
    # Engine + Agent
    draw_box(ax, 4.0, 5.3, 4.0, 0.8, '[2] AI ENGINE CORE\n(Tom tat - Hoi-dap - Phan loai)', '#d5e8d4', '#82b366', 10, bold=True)
    draw_box(ax, 12.0, 5.3, 4.0, 0.8, '[3] AGENT ORCH.\n(Tro ly da buoc)', '#d5e8d4', '#82b366', 10, bold=True)
    # Provider + 3 LLMs
    draw_box(ax, 7.5, 4.4, 5.0, 0.6, '[6] PROVIDER ABSTRACTION (LLM)', '#dae8fc', '#6c8ebf', 10, bold=True)
    draw_box(ax, 3.5, 3.6, 2.8, 0.6, 'OpenAI Cloud\n(gpt-4o-mini)', '#ffffff', '#6c8ebf', 9)
    draw_box(ax, 7.5, 3.6, 2.8, 0.6, 'Anthropic\n(Claude)', '#ffffff', '#6c8ebf', 9)
    draw_box(ax, 11.7, 3.6, 2.8, 0.6, '[MAT] Ollama Local\n(qwen2.5:14b) VAN BAN MAT', '#f8cecc', '#b85450', 9, bold=True)
    # Knowledge + OCR
    draw_box(ax, 0.6, 2.7, 4.0, 0.8, '[4] KNOWLEDGE BASE & RAG\n(Tim nghia - Ingestion)', '#d5e8d4', '#82b366', 10, bold=True)
    draw_box(ax, 15.4, 2.7, 4.0, 0.8, '[5] OCR & DOC EXTRACTION\n(OCR tieng Viet)', '#d5e8d4', '#82b366', 10, bold=True)
    # Job Queue
    draw_box(ax, 5.5, 2.2, 9.0, 0.5, '[7] AI JOB & TASK QUEUE (Kafka)', '#e1d5e7', '#9673a6', 10, bold=True)
    # MCP + Audit
    draw_box(ax, 0.6, 2.05, 2.8, 0.55, '[8] MCP Server', '#ffe6cc', '#d79b00', 9)
    draw_box(ax, 16.6, 2.05, 2.8, 0.55, '[9] Audit', '#ffe6cc', '#d79b00', 9)
    # Auth + Admin
    draw_box(ax, 4.0, 1.4, 4.0, 0.6, '[10] AI Admin Console\n(Bang dieu khien admin)', '#fad7ac', '#b46504', 10)
    draw_box(ax, 12.0, 1.4, 4.0, 0.6, '[11] Auth & Tenant\n(JWT - RLS - Phan quyen)', '#fad7ac', '#b46504', 10)

    # Tang 6
    draw_layer_box(ax, 0.3, 0.7, 19.4, 0.6, 'TANG 6 - SU KIEN & BO DEM (Kafka + Redis)', '#f5f5f5', 11, '#666666')

    # Tang 7
    draw_layer_box(ax, 0.3, -0.05, 19.4, 0.7, 'TANG 7 - DU LIEU (PostgreSQL - pgvector - MinIO)', '#f5f5f5', 11, '#666666')

    # Arrows
    arrow(ax, 10, 7.4, 10, 6.4, '#b85450', 2)
    arrow(ax, 8, 6.4, 6, 5.3, '#82b366', 1.5)
    arrow(ax, 12, 6.4, 14, 5.3, '#82b366', 1.5)
    arrow(ax, 6, 5.3, 9, 4.4, '#666666', 1)
    arrow(ax, 14, 5.3, 11, 4.4, '#666666', 1)
    arrow(ax, 8, 4.4, 5, 3.6, '#666666', 1)
    arrow(ax, 10, 4.4, 9, 3.6, '#666666', 1)
    arrow(ax, 12, 4.4, 13, 3.6, '#666666', 1)

    save(fig, '01_architecture_tong_the_7_tang')


# ============================================================
# SO DO 02: QUAN HỆ 11 PHÂN HỆ AI
# ============================================================
def diagram_02():
    fig, ax = plt.subplots(figsize=(20, 11))
    ax.set_xlim(0, 20)
    ax.set_ylim(0, 11)
    ax.axis('off')
    ax.text(10, 10.5, 'MO HINH QUAN HE 11 PHAN HE MODULE AI',
            ha='center', fontsize=18, fontweight='bold', color=TEXT_COLOR)

    draw_box(ax, 7, 9, 6, 1, '[1] AI GATEWAY\n(Cong tiep nhan duy nhat)', '#fad7ac', '#b46504', 12, bold=True)

    draw_box(ax, 3, 7.4, 5, 1.1, '[2] AI ENGINE CORE\n(Tom tat - Hoi-dap - Phan loai)', '#d5e8d4', '#82b366', 12, bold=True)
    draw_box(ax, 12, 7.4, 5, 1.1, '[3] AGENT ORCHESTRATION\n(Lap ke hoach - Goi cong cu - HITL)', '#d5e8d4', '#82b366', 12, bold=True)

    draw_box(ax, 7, 6, 6, 0.8, '[6] PROVIDER ABSTRACTION (LLM)', '#dae8fc', '#6c8ebf', 12, bold=True)

    draw_box(ax, 1, 4.3, 4, 1.0, '[8] MCP SERVER\n(Kho cong cu cho Agent)', '#ffe6cc', '#d79b00', 11)
    draw_box(ax, 7.5, 4.3, 5, 1.0, '[4] KNOWLEDGE BASE & RAG\n(Vector DB - Rerank - Hybrid)', '#d5e8d4', '#82b366', 11, bold=True)
    draw_box(ax, 15, 4.3, 4, 1.0, '[5] OCR & DOC EXTRACTION\n(Nhan dang van ban scan)', '#d5e8d4', '#82b366', 11)

    draw_box(ax, 7, 3.2, 6, 0.7, '[7] AI JOB & TASK QUEUE (Kafka)', '#e1d5e7', '#9673a6', 12, bold=True)

    rect = patches.FancyBboxPatch((0.3, 0.3), 19.4, 2.4,
                                   boxstyle='round,pad=0.02,rounding_size=0.1',
                                   linewidth=2, edgecolor='#666666', facecolor='#f5f5f5')
    ax.add_patch(rect)
    ax.text(10, 2.55, 'LOP NEN TANG (chay ngam)', ha='center', fontsize=12, fontweight='bold', color='#666666')
    draw_box(ax, 0.6, 0.6, 4.5, 1.5, '[11] AUTH & TENANT\nPhan quyen - RLS - JWT', '#fad7ac', '#b46504', 11)
    draw_box(ax, 5.7, 0.6, 4.5, 1.5, '[9] AUDIT & OBSERVABILITY\nLog - Metric - Trace', '#fad7ac', '#b46504', 11)
    draw_box(ax, 10.8, 0.6, 4.5, 1.5, '[10] AI ADMIN CONSOLE\nQuan ly tat ca phan he', '#fad7ac', '#b46504', 11)
    draw_box(ax, 15.9, 0.6, 3.5, 1.5, 'Web UI\nReact Admin', '#ffffff', '#666666', 11)

    arrow(ax, 9, 9, 5.5, 8.5, '#82b366', 2)
    arrow(ax, 11, 9, 14.5, 8.5, '#82b366', 2)
    arrow(ax, 5.5, 7.4, 9, 6.8, '#666666', 1)
    arrow(ax, 14.5, 7.4, 11, 6.8, '#666666', 1)
    arrow(ax, 5.5, 7.4, 10, 5.3, '#666666', 1)
    arrow(ax, 14.5, 7.4, 3, 5.3, '#666666', 1)
    arrow(ax, 12, 4.8, 13, 4.8, '#9673a6', 2)
    arrow(ax, 10, 4.3, 10, 3.9, '#666666', 1)
    arrow(ax, 17, 4.3, 10, 3.9, '#666666', 1)

    save(fig, '02_quan_he_11_phan_he')


# ============================================================
# SO DO 03: LUỒNG 1 YÊU CẦU AI
# ============================================================
def diagram_03():
    fig, ax = plt.subplots(figsize=(22, 14))
    ax.set_xlim(0, 22)
    ax.set_ylim(0, 14)
    ax.axis('off')
    ax.text(11, 13.5, 'LUONG 1 YEU CAU AI - 11 BUOC (User -> Gateway -> Engine -> Knowledge -> LLM)',
            ha='center', fontsize=18, fontweight='bold', color=TEXT_COLOR)

    cols = ['Nguoi dung\n(Web/Mobile)', 'Tang 1-2\n(Kong GW)', 'AI Gateway\n[1]',
            'AI Engine\n[2]', 'Knowledge\n[4]', 'LLM\n[6]']
    x_positions = [2, 5.5, 9, 12.5, 16, 19.5]
    box_w = 2.7

    for i, name in enumerate(cols):
        x = x_positions[i] - box_w/2
        draw_box(ax, x, 11.5, box_w, 0.8, name, '#dae8fc', '#6c8ebf', 10, bold=True)
        ax.plot([x_positions[i], x_positions[i]], [11.5, 1], 'k--', linewidth=1, alpha=0.5)

    steps = [
        (1, 0, '1. Dat cau hoi'),
        (1, 1, '2. Xac thuc JWT - Kiem tra quyen'),
        (2, 2, '3. Chong injection - Che PII - Kiem tra cache'),
        (2, 3, '4. Goi AI Engine (don gian) / Agent (phuc tap)'),
        (3, 4, '5. Tim kiem lai: TU KHOA + NGU NGHIA'),
        (4, 3, '6. Tra ve 8 van ban lien quan nhat'),
        (3, 3, '7. Xep hang lai (BGE) - Tong hop ngu canh'),
        (3, 5, '8. Goi LLM - Streaming response'),
        (5, 3, '9. Tra ve cau tra loi tung phan'),
        (3, 3, '10. Kiem tra chat luong - Trich dan nguon'),
        (2, 0, '11. Tra cau tra loi + Ghi log + Cap nhat chi phi'),
    ]
    y_pos = 11
    for src, dst, label in steps:
        y_pos -= 0.85
        x1 = x_positions[src]
        x2 = x_positions[dst]
        is_final = (src == 2 and dst == 0 and 'Tra cau tra loi' in label)
        color = '#b85450' if is_final else '#666666'
        ls = 'dashed' if src == dst else 'solid'
        ax.annotate('', xy=(x2, y_pos), xytext=(x1, y_pos),
                    arrowprops=dict(arrowstyle='->', color=color, lw=1.5, linestyle=ls))
        mid_x = (x1 + x2) / 2
        ax.text(mid_x, y_pos + 0.15, label, ha='center', va='bottom',
                fontsize=9, color='#333333',
                bbox=dict(boxstyle='round,pad=0.2', facecolor='#fff2cc', edgecolor='#d6b656'))

    save(fig, '03_luong_1_yeu_cau_ai')


# ============================================================
# SO DO 04: HẠ TẦNG 7 VÙNG MẠNG
# ============================================================
def diagram_04():
    fig, ax = plt.subplots(figsize=(20, 17))
    ax.set_xlim(0, 20)
    ax.set_ylim(0, 17)
    ax.axis('off')
    ax.text(10, 16.5, 'SO DO HA TANG TRIEN KHAI - 7 VUNG MANG',
            ha='center', fontsize=18, fontweight='bold', color=TEXT_COLOR)

    draw_box(ax, 7, 15.5, 6, 0.6, 'INTERNET / INTRANET Bo Tai Chinh', '#f5f5f5', '#666666', 12, bold=True)

    draw_layer_box(ax, 0.3, 14.2, 19.4, 1.1, 'VUNG 1 - BIEN', '#ffe6cc', 12)
    draw_box(ax, 1, 14.4, 5, 0.7, 'Firewall #1 (HA)', '#fad7ac', '#b46504', 11)
    draw_box(ax, 14, 14.4, 5, 0.7, 'WAF (HA)', '#fad7ac', '#b46504', 11)

    draw_layer_box(ax, 0.3, 13.0, 19.4, 1.0, 'VUNG 2 - LOAD BALANCER', '#dae8fc', 12, '#6c8ebf')
    draw_box(ax, 3, 13.2, 4, 0.6, 'Load Balancer #1', '#ffffff', '#6c8ebf', 11)
    draw_box(ax, 13, 13.2, 4, 0.6, 'Load Balancer #2', '#ffffff', '#6c8ebf', 11)

    draw_layer_box(ax, 0.3, 11.8, 19.4, 1.0, 'VUNG 3 - PROXY / API GATEWAY', '#d5e8d4', 12, '#82b366')
    draw_box(ax, 3, 12.0, 4, 0.6, 'NGINX (SSL)', '#ffffff', '#82b366', 11)
    draw_box(ax, 13, 12.0, 5, 0.6, 'Kong Gateway (rate-limit)', '#ffffff', '#82b366', 11)

    draw_layer_box(ax, 0.3, 10.7, 19.4, 0.8, 'VUNG 4 - FIREWALL CORE (IDS/IPS)', '#ffe6cc', 12)

    draw_layer_box(ax, 0.3, 6.5, 19.4, 4.0, 'VUNG 5 - UNG DUNG & XU LY (Compute Zone)', '#fad7ac', 13, '#b46504')
    rect = patches.FancyBboxPatch((0.5, 6.7), 10.5, 3.6,
                                   boxstyle='round,pad=0.02,rounding_size=0.05',
                                   linewidth=1.5, edgecolor='#6c8ebf', facecolor='#dae8fc')
    ax.add_patch(rect)
    ax.text(5.75, 10.1, 'K8s Cluster (CPU) - 26 node', ha='center', fontsize=11, fontweight='bold')
    k8s_components = ['AI GW\nx 3', 'AI Engine\nx 4', 'Agent\nx 3', 'Admin API\nx 2', 'MCP\nx 2',
                      'Knowledge\nx 3', 'Auth\nx 2', 'Audit\nx 2', 'OCR Worker\nx 2 (T4)', 'Buffer\nx 3']
    for i, c in enumerate(k8s_components):
        col = i % 5
        row = i // 5
        draw_box(ax, 0.7 + col*2.0, 9.2 - row*1.1, 1.9, 1.0, c, '#ffffff', '#6c8ebf', 8)

    rect = patches.FancyBboxPatch((11.2, 6.7), 8.3, 3.6,
                                   boxstyle='round,pad=0.02,rounding_size=0.05',
                                   linewidth=1.5, edgecolor='#b46504', facecolor='#fad7ac')
    ax.add_patch(rect)
    ax.text(15.35, 10.1, 'GPU Cluster (7 node)', ha='center', fontsize=11, fontweight='bold')
    draw_box(ax, 11.4, 8.7, 8.0, 1.3, 'Ollama LLM x 3 node\nA10 24GB - qwen2.5:14b\n~30-45 yeu cau/giay [MAT]',
             '#ffffff', '#b46504', 11, bold=True)
    draw_box(ax, 11.4, 7.0, 3.8, 1.5, 'OCR Engine x 2\nPaddleOCR-VL\n+ VietOCR (T4)', '#ffffff', '#b46504', 10)
    draw_box(ax, 15.5, 7.0, 3.8, 1.5, 'Embedding x 2\nBGE-M3\nmultilingual (T4)', '#ffffff', '#b46504', 10)

    draw_layer_box(ax, 0.3, 5.4, 19.4, 1.0, 'VUNG 6 - SU KIEN & BO DEM', '#e1d5e7', 12, '#9673a6')
    draw_box(ax, 1, 5.6, 6, 0.6, 'Kafka x 3 broker (events + Job Queue)', '#ffffff', '#9673a6', 11)
    draw_box(ax, 13, 5.6, 6, 0.6, 'Redis x 3 node (cache + rate-limit)', '#ffffff', '#9673a6', 11)

    draw_layer_box(ax, 0.3, 3.2, 19.4, 2.0, 'VUNG 7 - DU LIEU (DB Firewall)', '#f5f5f5', 12, '#666666')
    draw_box(ax, 1, 3.4, 5, 1.5, 'PostgreSQL HA x 3\n(Patroni 1+2)\nai_platform + nghiep vu', '#ffffff', '#666666', 10)
    draw_box(ax, 7.5, 3.4, 5, 1.5, 'Vector DB x 3\n(pgvector)\n10M vector - 60 GB', '#ffffff', '#666666', 10)
    draw_box(ax, 14, 3.4, 5, 1.5, 'MinIO x 4 node\n16 TB tong\nPDF/DOCX scan', '#ffffff', '#666666', 10)

    rect = patches.FancyBboxPatch((0.3, 0.3), 19.4, 2.5,
                                   boxstyle='round,pad=0.02,rounding_size=0.05',
                                   linewidth=2, edgecolor='#0F2A50', facecolor='#0F2A50')
    ax.add_patch(rect)
    ax.text(10, 2.4, 'TONG KET TOAN BO HA TANG', ha='center', fontsize=14, fontweight='bold', color='white')
    ax.text(10, 1.7, '~60 server (CPU + GPU) + 8 thiet bi mang/bao mat\n~620 vCPU - ~1,7 TB RAM - ~47 TB luu tru',
            ha='center', fontsize=12, color='white')
    ax.text(10, 0.7, 'CAPEX: ~18,1 ty VNĐ | OPEX/nam: ~4,0 ty VNĐ | Tong 5 nam: ~38-40 ty VNĐ',
            ha='center', fontsize=12, fontweight='bold', color='#fff2cc')

    arrow(ax, 10, 15.5, 3.5, 15.1, '#666666', 1.5)
    arrow(ax, 3.5, 14.4, 5, 13.8, '#666666', 1.5)
    arrow(ax, 5, 13.2, 5, 12.6, '#666666', 1.5)
    arrow(ax, 5, 12.0, 10, 11.5, '#666666', 1.5)
    arrow(ax, 10, 10.7, 5.75, 10.3, '#b85450', 2)

    save(fig, '04_ha_tang_7_vung_mang')


# ============================================================
# SO DO 05: PIPELINE RAG
# ============================================================
def diagram_05():
    fig, ax = plt.subplots(figsize=(24, 11))
    ax.set_xlim(0, 24)
    ax.set_ylim(0, 11)
    ax.axis('off')
    ax.text(12, 10.5, 'PIPELINE RAG - 5 BUOC (Embed -> Hybrid Search -> Rerank -> LLM -> Cite + Quality)',
            ha='center', fontsize=18, fontweight='bold', color=TEXT_COLOR)

    steps = [
        (0.3, 7.5, '#dae8fc', '#6c8ebf', 'BUOC 1\nEMBED\n(Vector hoa)', [
            ('INPUT:', 'Cau hoi user\n(tieng Viet, 1-3 cau)'),
            ('Xu ly:', 'BGE-M3 / OpenAI Embed\n-> Vector 1536 chieu'),
            ('OUTPUT:', 'Day so 1536 chieu', '#d5e8d4'),
        ]),
        (5.0, 7.5, '#d5e8d4', '#82b366', 'BUOC 2\nHYBRID SEARCH\n(Tim kiem lai)', [
            ('BM25:', 'Tim theo TU KHOA\n-> Top 50'),
            ('Cosine:', 'Tim theo NGU NGHIA\n-> Top 50'),
            ('RRF:', 'Hop nhat 2 list\n-> 50 ung vien', '#e1d5e7'),
        ]),
        (9.7, 7.5, '#fff2cc', '#d6b656', 'BUOC 3\nRERANK\n(Xep hang lai)', [
            ('Mo hinh:', 'BGE-reranker-v2-m3'),
            ('Xu ly:', 'Danh gia lai do lien quan\n-> Chon Top 8 van ban'),
            ('OUTPUT:', 'Top 8 van ban\n(da cham diem)', '#d5e8d4'),
        ]),
        (14.4, 7.5, '#fad7ac', '#b46504', 'BUOC 4\nLLM\n(Goi mo hinh AI)', [
            ('Prompt:', '[System] + [Top 8] + [Cau hoi]'),
            ('Chon model:', 'Mat -> Ollama\nThuong -> GPT-4o-mini'),
            ('Output:', 'Streaming response', '#d5e8d4'),
        ]),
        (19.1, 7.5, '#e1d5e7', '#9673a6', 'BUOC 5\nCITE + QUALITY', [
            ('Trich dan:', 'File, trang, khoang'),
            ('Faithfulness:', 'Cau tra loi co trung thuc\nvoi van ban goc?'),
            ('OUTPUT:', 'Cau tra loi + trich dan\n+ diem tin cay 0-1', '#d5e8d4'),
        ]),
    ]

    for x, y, fill, edge, title, sub_items in steps:
        draw_box(ax, x, y, 4.4, 2.0, title, fill, edge, 13, bold=True)
        for i, item in enumerate(sub_items):
            label = item[0]
            content = item[1]
            sub_fill = item[2] if len(item) > 2 else '#ffffff'
            sub_edge = '#82b366' if sub_fill == '#d5e8d4' else edge
            # Khoang cach 1.35 (thay vi 1.2) de tranh overlap
            draw_box(ax, x, y - 1.35 - i*1.35, 4.4, 1.25,
                     f'{label}\n{content}', sub_fill, sub_edge, 9)
        if x < 19:
            arrow(ax, x + 4.5, y + 1, x + 5.5, y + 1, '#b85450', 3)

    draw_box(ax, 0.3, 0.5, 23.4, 1.2,
             'GIAI THICH:  - BGE-M3 = Embedding da ngon ngu  - BM25 = Tim theo tu khoa truyen thong  - RRF = Hop nhat 2 danh sach  '
             '- BGE-reranker = Mo hinh xep hang lai chinh xac hon vector similarity  - Faithfulness = Danh gia chong hallucination',
             '#fff2cc', '#d6b656', 11)

    save(fig, '05_pipeline_rag_chi_tiet')


# ============================================================
# SO DO 06: CLEAN ARCHITECTURE
# ============================================================
def diagram_06():
    fig, ax = plt.subplots(figsize=(20, 11))
    ax.set_xlim(0, 20)
    ax.set_ylim(0, 11)
    ax.axis('off')
    ax.text(10, 10.5, 'CAU TRUC 4 LOP CLEAN ARCHITECTURE TRONG MOI PHAN HE AI',
            ha='center', fontsize=18, fontweight='bold', color=TEXT_COLOR)

    layers = [
        (7, '#dae8fc', '#6c8ebf', 'LOP 1 - API (Giao tiep)',
         'AiGateway.Api - REST API endpoint - JWT middleware - Controllers - DTO',
         '(phu thuoc framework - co the thay doi)', '#666666'),
        (5.5, '#fff2cc', '#d6b656', 'LOP 2 - INFRASTRUCTURE (Ha tang)',
         'Redis client - PostgreSQL client - OpenAI/Anthropic API client - HTTP client cho KMS',
         '(ket noi the gioi ben ngoai)', '#666666'),
        (4, '#d5e8d4', '#82b366', 'LOP 3 - APPLICATION (Xu ly)',
         'ProcessRequestHandler (MediatR) - Pipeline behaviors - Use cases: AskQuestion, SummarizeDoc',
         '(quy trinh xu ly yeu cau)', '#666666'),
        (2.0, '#f8cecc', '#b85450', 'LOP 4 - DOMAIN (Loi nghiep vu) [QUAN TRONG]',
         'Entities: AiRequest, RateLimitCounter, SemanticCacheEntry - Domain Services: PromptSanitizer, PiiRedactor\n'
         '(QUY TAC NGHIEP VU COT LOI - KHONG phu thuoc framework)',
         '(THAY DOI KHONG ANH HUONG)', '#b85450'),
    ]
    for y, fill, edge, title, content, sub_text, sub_color in layers:
        h = 1.5 if y != 2 else 2.0
        draw_box(ax, 2.5, y, 13.5, h, title, fill, edge, 14, bold=True)
        ax.text(9.25, y + h*0.45, content, ha='center', va='top', fontsize=10, color=TEXT_COLOR)
        ax.text(0.5, y + h/2, sub_text, ha='center', va='center', fontsize=10,
                color=sub_color, fontweight='bold', rotation=0)

    for y_top, y_bot in [(7, 6.5), (5.5, 5.0), (4, 3.0)]:
        arrow(ax, 9.25, y_bot, 9.25, y_bot - 0.5, '#b85450', 2)

    save(fig, '06_clean_architecture')


# ============================================================
# SO DO 07: BẢO MẬT 7 LỚP CHO AI
# ============================================================
def diagram_07():
    fig, ax = plt.subplots(figsize=(20, 11))
    ax.set_xlim(0, 20)
    ax.set_ylim(0, 11)
    ax.axis('off')
    ax.text(10, 10.5, 'MO HINH BAO MAT 7 LOP CHO MODULE AI',
            ha='center', fontsize=18, fontweight='bold', color=TEXT_COLOR)

    layers = [
        (9, '#f5f5f5', '#666666', 'LOP 7 - NHAT KY KIEM TRA (Audit Log)',
         '"So ghi chep" moi hoat dong AI - Phan he [9] Audit & Observability'),
        (7.7, '#fad7ac', '#b46504', 'LOP 6 - PHAT HIEN TAN CONG (IDS/IPS)',
         '"Camera an ninh" - Chong prompt injection - Phat hien jailbreak'),
        (6.4, '#f8cecc', '#b85450', 'LOP 5 - PHAN LOAI DO MAT (Data Classification)',
         'Mat -> Ollama - Thuong -> OpenAI - Phan he [1] AI Gateway chon provider theo do nhay'),
        (5.1, '#e1d5e7', '#9673a6', 'LOP 4 - MA HOA DU LIEU (Encryption)',
         '"Hop kin" - TLS 1.3 (truyen) - AES-256 (luu tru) - Che PII truoc khi gui AI'),
        (3.8, '#fff2cc', '#d6b656', 'LOP 3 - JWT + PHAN QUYEN',
         '"The nhan vien" - Admin/Operator/User/Auditor - Phan he [11] + [1]'),
        (2.5, '#d5e8d4', '#82b366', 'LOP 2 - API GATEWAY',
         '"Le tan" - JWT - Gioi han 100 cau/phut/user - Phan he [1]'),
        (1.2, '#dae8fc', '#6c8ebf', 'LOP 1 - TUONG LUA BIEN (Firewall + WAF)',
         '"Hang rao" - Chan DDoS - Loc SQL injection - Firewall + WAF'),
    ]
    for y, fill, edge, title, content in layers:
        draw_box(ax, 3, y, 13.5, 1.1, title, fill, edge, 12, bold=True)
        ax.text(9.75, y + 0.4, content, ha='center', va='center', fontsize=9, color=TEXT_COLOR)

    ax.text(1.5, 9.5, 'NGOAI', ha='center', fontsize=12, fontweight='bold', color='#b85450', rotation=90)
    ax.text(1.5, 1.7, 'TRONG', ha='center', fontsize=12, fontweight='bold', color='#0F2A50', rotation=90)

    draw_box(ax, 17.5, 7, 2.2, 2.5,
             'Dac biet\ncho AI\n\n- Prompt injection\n- Che giau PII\n- Rate limit\n- HITL',
             '#fff2cc', '#d6b656', 10, bold=True)

    save(fig, '07_bao_mat_7_lop_ai')


# ============================================================
# RUN ALL
# ============================================================
if __name__ == '__main__':
    print('VE 7 SO DO THANH PNG CHAT LUONG CAO')
    print('='*60)
    diagram_01()
    diagram_02()
    diagram_03()
    diagram_04()
    diagram_05()
    diagram_06()
    diagram_07()
    print('='*60)
    print('HOAN THANH!')