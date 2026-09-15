"""Generate neutral, source-aligned diagrams for the master's report.

The figures deliberately distinguish the proposed architecture from the local
prototype.  They contain no measured performance values and do not imply that
roadmap components have already been implemented.
"""

from __future__ import annotations

import math
import textwrap
from pathlib import Path

from PIL import Image, ImageDraw, ImageFont


WIDTH, HEIGHT = 2000, 1200
BG = "#F7F9FC"
INK = "#18324A"
MUTED = "#5A6B7B"
BLUE = "#1F5A94"
BLUE_2 = "#DCEAF7"
TEAL = "#167D86"
TEAL_2 = "#DDF1F2"
GOLD = "#A46B12"
GOLD_2 = "#F7EBCF"
GREEN = "#39764B"
GREEN_2 = "#E1F0E5"
GRAY = "#708090"
GRAY_2 = "#E9EEF3"
RED = "#984B45"
RED_2 = "#F5E5E3"
WHITE = "#FFFFFF"

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / "references" / "figures"
FONT_DIR = Path(r"C:\Windows\Fonts")


def font(size: int, bold: bool = False, italic: bool = False) -> ImageFont.FreeTypeFont:
    if bold and italic:
        name = "arialbi.ttf"
    elif bold:
        name = "arialbd.ttf"
    elif italic:
        name = "ariali.ttf"
    else:
        name = "arial.ttf"
    return ImageFont.truetype(str(FONT_DIR / name), size)


F_TITLE = font(44, bold=True)
F_SUBTITLE = font(23)
F_SECTION = font(25, bold=True)
F_BOX = font(24, bold=True)
F_BODY = font(21)
F_SMALL = font(18)
F_BADGE = font(19, bold=True)


def canvas(title: str, subtitle: str, badge: str) -> tuple[Image.Image, ImageDraw.ImageDraw]:
    im = Image.new("RGB", (WIDTH, HEIGHT), BG)
    d = ImageDraw.Draw(im)
    d.rounded_rectangle((42, 34, WIDTH - 42, HEIGHT - 34), radius=26, fill=WHITE, outline="#C5D1DD", width=3)
    d.text((90, 72), title, font=F_TITLE, fill=INK)
    d.text((92, 132), subtitle, font=F_SUBTITLE, fill=MUTED)
    badge_w = max(290, int(d.textlength(badge, font=F_BADGE)) + 54)
    d.rounded_rectangle((WIDTH - badge_w - 88, 71, WIDTH - 88, 121), radius=22, fill=GOLD_2, outline=GOLD, width=2)
    d.text((WIDTH - badge_w - 61, 83), badge, font=F_BADGE, fill=GOLD)
    return im, d


def wrapped_lines(d: ImageDraw.ImageDraw, text: str, fnt: ImageFont.FreeTypeFont, max_width: int) -> list[str]:
    lines: list[str] = []
    for raw in text.split("\n"):
        if not raw:
            lines.append("")
            continue
        words = raw.split()
        current = words[0]
        for word in words[1:]:
            candidate = f"{current} {word}"
            if d.textlength(candidate, font=fnt) <= max_width:
                current = candidate
            else:
                lines.append(current)
                current = word
        lines.append(current)
    return lines


def centered_text(
    d: ImageDraw.ImageDraw,
    box: tuple[int, int, int, int],
    text: str,
    fnt: ImageFont.FreeTypeFont,
    fill: str = INK,
    pad: int = 24,
    spacing: int = 7,
) -> None:
    x1, y1, x2, y2 = box
    lines = wrapped_lines(d, text, fnt, x2 - x1 - 2 * pad)
    heights = []
    for line in lines:
        b = d.textbbox((0, 0), line or " ", font=fnt)
        heights.append(b[3] - b[1])
    total = sum(heights) + spacing * max(0, len(lines) - 1)
    y = y1 + (y2 - y1 - total) / 2
    for line, h in zip(lines, heights):
        w = d.textlength(line, font=fnt)
        d.text((x1 + (x2 - x1 - w) / 2, y), line, font=fnt, fill=fill)
        y += h + spacing


def box(
    d: ImageDraw.ImageDraw,
    xy: tuple[int, int, int, int],
    text: str,
    *,
    fill: str = BLUE_2,
    outline: str = BLUE,
    fnt: ImageFont.FreeTypeFont = F_BOX,
    radius: int = 20,
    width: int = 3,
) -> None:
    d.rounded_rectangle(xy, radius=radius, fill=fill, outline=outline, width=width)
    centered_text(d, xy, text, fnt)


def arrow(
    d: ImageDraw.ImageDraw,
    start: tuple[int, int],
    end: tuple[int, int],
    *,
    color: str = GRAY,
    width: int = 5,
    dashed: bool = False,
) -> None:
    x1, y1 = start
    x2, y2 = end
    if dashed:
        length = math.hypot(x2 - x1, y2 - y1)
        if length == 0:
            return
        ux, uy = (x2 - x1) / length, (y2 - y1) / length
        pos = 0.0
        while pos < length - 18:
            seg_end = min(pos + 18, length - 18)
            d.line((x1 + ux * pos, y1 + uy * pos, x1 + ux * seg_end, y1 + uy * seg_end), fill=color, width=width)
            pos += 31
    else:
        d.line((x1, y1, x2, y2), fill=color, width=width)
    angle = math.atan2(y2 - y1, x2 - x1)
    size = 18
    p1 = (x2, y2)
    p2 = (x2 - size * math.cos(angle - 0.55), y2 - size * math.sin(angle - 0.55))
    p3 = (x2 - size * math.cos(angle + 0.55), y2 - size * math.sin(angle + 0.55))
    d.polygon((p1, p2, p3), fill=color)


def note(d: ImageDraw.ImageDraw, text: str, y: int = 1092) -> None:
    d.rounded_rectangle((90, y, WIDTH - 90, y + 54), radius=18, fill=GRAY_2, outline="#C5D1DD", width=2)
    centered_text(d, (112, y + 2, WIDTH - 112, y + 52), text, F_SMALL, fill=MUTED)


def save(im: Image.Image, name: str) -> None:
    OUT.mkdir(parents=True, exist_ok=True)
    im.save(OUT / name, format="PNG", dpi=(180, 180), optimize=True)


def figure_01() -> None:
    im, d = canvas(
        "Kiến trúc tổng thể bảy tầng",
        "Phân rã trách nhiệm của Enterprise AI Platform theo kiến trúc mục tiêu",
        "KIẾN TRÚC ĐỀ XUẤT",
    )
    rows = [
        ("7", "Trình diễn & API", "Web Next.js · API cho người dùng và hệ thống ngoài", BLUE_2, BLUE),
        ("6", "AI Gateway", "Định tuyến · chính sách · giới hạn truy cập · quan sát", TEAL_2, TEAL),
        ("5", "Agent & Workflow", "Lập kế hoạch · công cụ · bộ nhớ · human-in-the-loop", GOLD_2, GOLD),
        ("4", "AI Engine Core", "RAG · embedding · prompt · citation · tìm kiếm", GREEN_2, GREEN),
        ("3", "Provider Abstraction", "Hợp đồng chung; adapter Ollama và OpenAI trong prototype", BLUE_2, BLUE),
        ("2", "Integration", "REST/gRPC/MCP/connectors theo lộ trình tích hợp", RED_2, RED),
        ("1", "Platform Core", "Định danh · tenant · cấu hình · audit · dữ liệu dùng chung", GRAY_2, GRAY),
    ]
    y = 198
    for number, name, body, fill, outline in rows:
        d.rounded_rectangle((95, y, 1905, y + 108), radius=22, fill=fill, outline=outline, width=3)
        d.ellipse((120, y + 22, 184, y + 86), fill=outline)
        centered_text(d, (120, y + 22, 184, y + 86), number, F_BOX, fill=WHITE, pad=0)
        d.text((220, y + 20), name, font=F_SECTION, fill=outline)
        d.text((620, y + 24), body, font=F_BODY, fill=INK)
        y += 120
    note(d, "Các tầng là ranh giới thiết kế; mức độ hiện thực hóa của prototype được đánh giá riêng ở Chương 3.")
    save(im, "01_architecture_tong_the_7_tang.png")


def figure_02() -> None:
    im, d = canvas(
        "Quan hệ giữa các phân hệ chính",
        "Nhóm 11 phân hệ theo luồng phụ thuộc từ trải nghiệm đến nền tảng",
        "BẢN ĐỒ MỤC TIÊU",
    )
    row_specs = [
        ("Kênh sử dụng", ["Web / Admin", "Public API"], 2, BLUE_2, BLUE),
        ("Năng lực AI", ["AI Gateway", "RAG & Search", "Agent & Workflow", "Provider"], 4, TEAL_2, TEAL),
        ("Dữ liệu & tích hợp", ["Document", "Vector Store", "Tool Registry", "MCP / Connectors"], 4, GOLD_2, GOLD),
        ("Nền tảng", ["Identity · Tenant · Config · Audit"], 1, GRAY_2, GRAY),
    ]
    y_positions = [230, 445, 680, 910]
    row_heights = [130, 145, 145, 120]
    centers: list[list[tuple[int, int]]] = []
    for (label, items, count, fill, outline), y, h in zip(row_specs, y_positions, row_heights):
        d.text((100, y - 42), label.upper(), font=F_BADGE, fill=outline)
        gap = 28
        area_x1, area_x2 = 100, 1900
        item_w = (area_x2 - area_x1 - gap * (count - 1)) // count
        row_centers = []
        for i, item in enumerate(items):
            x1 = area_x1 + i * (item_w + gap)
            x2 = x1 + item_w
            box(d, (x1, y, x2, y + h), item, fill=fill, outline=outline)
            row_centers.append(((x1 + x2) // 2, y + h // 2))
        centers.append(row_centers)
    for upper, lower in zip(centers, centers[1:]):
        for ux, uy in upper:
            nearest = min(lower, key=lambda p: abs(p[0] - ux))
            arrow(d, (ux, uy + 70), (nearest[0], nearest[1] - 70), color="#94A3B1", width=4)
    note(d, "Các mũi tên biểu diễn phụ thuộc logic, không đồng nghĩa mỗi phân hệ đã là một dịch vụ triển khai độc lập.")
    save(im, "02_quan_he_11_phan_he.png")


def figure_03() -> None:
    im, d = canvas(
        "Luồng xử lý một yêu cầu AI",
        "Luồng tham chiếu xuyên qua giao diện, API, năng lực AI và nhà cung cấp mô hình",
        "LUỒNG ĐỀ XUẤT",
    )
    top = [
        ("1", "Người dùng"),
        ("2", "Giao diện\nNext.js"),
        ("3", "API .NET"),
        ("4", "Xác thực &\nchính sách"),
        ("5", "Điều phối\nyêu cầu"),
    ]
    bottom = [
        ("9", "Phản hồi · trích dẫn\n· dấu vết xử lý"),
        ("8", "Ollama / OpenAI"),
        ("7", "Hợp đồng\nprovider"),
        ("6", "RAG / công cụ\n(nếu cần)"),
    ]
    x0, gap, w, h = 100, 30, 336, 155
    top_boxes = []
    for i, (n, label) in enumerate(top):
        x = x0 + i * (w + gap)
        xy = (x, 280, x + w, 435)
        box(d, xy, f"{n}. {label}", fill=BLUE_2 if i < 3 else TEAL_2, outline=BLUE if i < 3 else TEAL)
        top_boxes.append(xy)
        if i:
            arrow(d, (top_boxes[i - 1][2] + 4, 357), (x - 5, 357), color=GRAY)
    bottom_boxes = []
    bx_positions = [100, 558, 1016, 1474]
    for i, ((n, label), x) in enumerate(zip(bottom, bx_positions)):
        xy = (x, 710, x + 360, 875)
        fill, outline = (GREEN_2, GREEN) if n == "9" else (GOLD_2, GOLD)
        box(d, xy, f"{n}. {label}", fill=fill, outline=outline)
        bottom_boxes.append(xy)
    arrow(d, (top_boxes[-1][2] - 168, top_boxes[-1][3] + 4), (bottom_boxes[-1][0] + 180, bottom_boxes[-1][1] - 5), color=GRAY)
    for i in range(len(bottom_boxes) - 1, 0, -1):
        arrow(d, (bottom_boxes[i][0] - 5, 792), (bottom_boxes[i - 1][2] + 5, 792), color=GRAY)
    d.text((102, 525), "Ngữ cảnh, chính sách và nhật ký phải đi cùng yêu cầu trong toàn bộ vòng đời xử lý.", font=F_SECTION, fill=INK)
    note(d, "RAG và công cụ là nhánh tùy chọn; Agent/MCP chỉ được dùng khi đã có chính sách và hợp đồng tích hợp tương ứng.")
    save(im, "03_luong_1_yeu_cau_ai.png")


def figure_04() -> None:
    im, d = canvas(
        "Mô hình triển khai local của prototype",
        "Bảy service được khai báo trong docker/docker-compose.yml",
        "HIỆN TRẠNG CẤU HÌNH",
    )
    d.rounded_rectangle((95, 205, 1905, 1010), radius=28, fill="#FBFCFE", outline=GRAY, width=3)
    d.text((125, 225), "Docker Compose · mạng nội bộ", font=F_SECTION, fill=GRAY)
    frontend = (150, 350, 530, 525)
    backend = (810, 350, 1190, 525)
    ollama = (1470, 350, 1850, 525)
    box(d, frontend, "frontend\nNext.js", fill=BLUE_2, outline=BLUE)
    box(d, backend, "backend\nASP.NET Core", fill=TEAL_2, outline=TEAL)
    box(d, ollama, "ollama\nLLM local", fill=GOLD_2, outline=GOLD)
    arrow(d, (530, 437), (805, 437), color=GRAY)
    arrow(d, (1190, 437), (1465, 437), color=GRAY)
    services = [
        ("postgresql", "Dữ liệu quan hệ\n+ pgvector"),
        ("redis", "Bộ nhớ đệm"),
        ("seq", "Nhật ký tập trung"),
        ("minio", "Lưu tệp đối tượng"),
    ]
    xs = [130, 585, 1040, 1495]
    for x, (name, desc) in zip(xs, services):
        xy = (x, 690, x + 375, 875)
        box(d, xy, f"{name}\n{desc}", fill=GRAY_2, outline=GRAY, fnt=F_BODY)
        arrow(d, (1000, 530), (x + 187, 685), color="#8A9AAA", width=4)
    d.text((725, 590), "backend kết nối các dịch vụ hỗ trợ", font=F_BADGE, fill=MUTED)
    note(d, "Sơ đồ phản ánh cấu hình tĩnh; không được hiểu là bằng chứng cả bảy service đã khởi động và kiểm thử thành công.")
    save(im, "04_ha_tang_7_vung_mang.png")


def lane(
    d: ImageDraw.ImageDraw,
    y: int,
    label: str,
    steps: list[str],
    fill: str,
    outline: str,
) -> None:
    d.rounded_rectangle((95, y, 1905, y + 275), radius=25, fill="#FBFCFE", outline=outline, width=3)
    d.rounded_rectangle((115, y + 25, 340, y + 250), radius=20, fill=fill, outline=outline, width=3)
    centered_text(d, (130, y + 40, 325, y + 235), label, F_SECTION, fill=outline)
    x0, gap = 390, 24
    w = (1475 - gap * (len(steps) - 1)) // len(steps)
    boxes = []
    for i, step in enumerate(steps):
        x = x0 + i * (w + gap)
        xy = (x, y + 74, x + w, y + 200)
        box(d, xy, step, fill=fill, outline=outline, fnt=F_BODY, radius=16)
        boxes.append(xy)
        if i:
            arrow(d, (boxes[i - 1][2] + 3, y + 137), (x - 4, y + 137), color=outline, width=4)


def figure_05() -> None:
    im, d = canvas(
        "Pipeline RAG tham chiếu",
        "Hai luồng tách biệt: nạp tri thức và trả lời truy vấn",
        "THIẾT KẾ MỤC TIÊU",
    )
    lane(d, 235, "NẠP\nTRI THỨC", ["Tải lên", "Kiểm tra", "Trích xuất", "Chia đoạn", "Embedding", "Lưu & lập chỉ mục"], BLUE_2, BLUE)
    lane(d, 580, "TRẢ LỜI\nTRUY VẤN", ["Câu hỏi", "Embedding", "Truy hồi", "Ghép ngữ cảnh", "Gọi LLM", "Trả lời & nguồn"], TEAL_2, TEAL)
    d.text((100, 930), "Điểm kiểm soát: quyền truy cập tài liệu · chất lượng ngữ cảnh · provenance · lỗi provider", font=F_SECTION, fill=INK)
    note(d, "Đây là pipeline thiết kế. Không có bộ dữ liệu hay benchmark trong kho mã để khẳng định chất lượng định lượng.")
    save(im, "05_pipeline_rag_chi_tiet.png")


def figure_06() -> None:
    im, d = canvas(
        "Clean Architecture đề xuất cho backend",
        "Dependency Rule hướng mọi phụ thuộc mã nguồn vào domain và use case",
        "CẤU TRÚC TÁI TỔ CHỨC",
    )
    layers = [
        ((120, 215, 1880, 1015), GRAY_2, GRAY, "FRAMEWORKS & DRIVERS", "ASP.NET Core · PostgreSQL/pgvector · Redis · MinIO · Ollama/OpenAI"),
        ((290, 345, 1710, 930), BLUE_2, BLUE, "INTERFACE ADAPTERS", "Controllers · DTO/mappers · repository/provider adapters"),
        ((500, 480, 1500, 840), TEAL_2, TEAL, "APPLICATION", "Use cases · ports · orchestration · transaction boundary"),
        ((745, 605, 1255, 765), GOLD_2, GOLD, "DOMAIN", "Entities · value objects · domain policies"),
    ]
    for xy, fill, outline, label, detail in layers:
        d.rounded_rectangle(xy, radius=28, fill=fill, outline=outline, width=4)
        d.text((xy[0] + 30, xy[1] + 20), label, font=F_SECTION, fill=outline)
        if label != "DOMAIN":
            d.text((xy[0] + 30, xy[3] - 56), detail, font=F_BODY, fill=INK)
        else:
            centered_text(d, (xy[0] + 30, xy[1] + 48, xy[2] - 30, xy[3] - 16), detail, F_BODY)
    arrow(d, (205, 615), (490, 615), color=GRAY, width=5)
    arrow(d, (1795, 615), (1510, 615), color=GRAY, width=5)
    arrow(d, (1000, 275), (1000, 470), color=GRAY, width=5)
    arrow(d, (1000, 955), (1000, 850), color=GRAY, width=5)
    d.rounded_rectangle((122, 1040, 680, 1084), radius=16, fill=WHITE, outline=GRAY, width=2)
    centered_text(d, (136, 1042, 666, 1082), "Mũi tên: phụ thuộc hướng vào trong", F_SMALL, fill=MUTED)
    note(d, "Prototype hiện tập trung trong một project; hình mô tả cấu trúc tái tổ chức, không khẳng định Dependency Rule đã đạt.")
    save(im, "06_clean_architecture.png")


def figure_07() -> None:
    im, d = canvas(
        "Mô hình bảo mật và quản trị theo chiều sâu",
        "Bảy lớp kiểm soát cần được hiện thực hóa và kiểm chứng theo lộ trình",
        "KIẾN TRÚC ĐỀ XUẤT",
    )
    layers = [
        ("1", "Định danh & xác thực", "Tài khoản, token, phiên đăng nhập"),
        ("2", "Phân quyền & tenant", "RBAC/ABAC, ranh giới dữ liệu"),
        ("3", "Kiểm soát đầu vào", "Xác thực dữ liệu, prompt injection, tệp tải lên"),
        ("4", "Chính sách Agent/Tool", "Danh sách cho phép, phê duyệt, giới hạn hành động"),
        ("5", "Bảo vệ dữ liệu", "Mã hóa, bí mật, tối thiểu hóa dữ liệu nhạy cảm"),
        ("6", "Audit & quan sát", "Nhật ký bất biến, truy vết, cảnh báo"),
        ("7", "Vận hành & phục hồi", "Sao lưu, rollback, ứng phó sự cố"),
    ]
    colors = [(BLUE_2, BLUE), (TEAL_2, TEAL), (GOLD_2, GOLD), (RED_2, RED), (GREEN_2, GREEN), (GRAY_2, GRAY), (BLUE_2, BLUE)]
    y = 205
    for (num, title, body), (fill, outline) in zip(layers, colors):
        d.rounded_rectangle((125, y, 1875, y + 104), radius=20, fill=fill, outline=outline, width=3)
        d.ellipse((150, y + 20, 214, y + 84), fill=outline)
        centered_text(d, (150, y + 20, 214, y + 84), num, F_BOX, fill=WHITE, pad=0)
        d.text((250, y + 17), title, font=F_SECTION, fill=outline)
        d.text((760, y + 21), body, font=F_BODY, fill=INK)
        y += 118
    note(d, "Sơ đồ là khung kiểm soát mục tiêu; Chương 3 nêu rõ những lớp chưa có bằng chứng kiểm thử trong prototype.")
    save(im, "07_bao_mat_7_lop_ai.png")


def main() -> None:
    figure_01()
    figure_02()
    figure_03()
    figure_04()
    figure_05()
    figure_06()
    figure_07()
    print(f"Generated 7 figures in {OUT}")


if __name__ == "__main__":
    main()
