# -*- coding: utf-8 -*-
"""Remove empty header/footer stories materialized by Microsoft Word.

Word can create blank first/even header and footer parts while recalculating
fields.  They are visually empty but violate the report rule that only the
top-centred PAGE field from MỞ ĐẦU onward may use a header story.
"""

from __future__ import annotations

import os
import sys
import tempfile
import zipfile
from pathlib import Path

from lxml import etree


W = "http://schemas.openxmlformats.org/wordprocessingml/2006/main"
R = "http://schemas.openxmlformats.org/officeDocument/2006/relationships"
PR = "http://schemas.openxmlformats.org/package/2006/relationships"
CT = "http://schemas.openxmlformats.org/package/2006/content-types"
NS = {"w": W, "r": R, "pr": PR, "ct": CT}


def is_empty_story(data: bytes) -> bool:
    root = etree.fromstring(data)
    if any((value or "").strip() for value in root.xpath(".//w:t/text() | .//w:instrText/text()", namespaces=NS)):
        return False
    # Empty paragraphs and their formatting are harmless; all other content
    # (drawing, table, field, object, etc.) makes the story meaningful.
    meaningful = root.xpath(
        ".//*[not(self::w:hdr or self::w:ftr or self::w:p or self::w:pPr "
        "or self::w:pStyle or self::w:r or self::w:rPr or self::w:rFonts or self::w:sz "
        "or self::w:szCs or self::w:lang or self::w:jc or self::w:spacing)]",
        namespaces=NS,
    )
    return not meaningful


def cleanup(path: Path) -> tuple[int, int]:
    with zipfile.ZipFile(path, "r") as source:
        parts = {name: source.read(name) for name in source.namelist()}

    document_name = "word/document.xml"
    rels_name = "word/_rels/document.xml.rels"
    types_name = "[Content_Types].xml"
    document = etree.fromstring(parts[document_name])
    rels = etree.fromstring(parts[rels_name])
    content_types = etree.fromstring(parts[types_name])

    relationships = {
        rel.get("Id"): rel
        for rel in rels.findall(f"{{{PR}}}Relationship")
    }
    removed_refs = 0
    candidate_parts: set[str] = set()

    for ref in list(document.xpath(".//w:headerReference | .//w:footerReference", namespaces=NS)):
        rid = ref.get(f"{{{R}}}id")
        rel = relationships.get(rid)
        if rel is None:
            continue
        target = rel.get("Target", "").replace("\\", "/")
        part_name = target.lstrip("/") if target.startswith("/") else f"word/{target}"
        if part_name in parts and is_empty_story(parts[part_name]):
            ref.getparent().remove(ref)
            candidate_parts.add(part_name)
            removed_refs += 1

    remaining_rids = set(document.xpath(".//@r:id", namespaces=NS))
    removed_parts = 0
    for rid, rel in list(relationships.items()):
        target = rel.get("Target", "").replace("\\", "/")
        part_name = target.lstrip("/") if target.startswith("/") else f"word/{target}"
        if rid in remaining_rids or part_name not in candidate_parts:
            continue
        rels.remove(rel)
        parts.pop(part_name, None)
        parts.pop(f"word/_rels/{Path(part_name).name}.rels", None)
        override_name = f"/{part_name}"
        for override in list(content_types.findall(f"{{{CT}}}Override")):
            if override.get("PartName") == override_name:
                content_types.remove(override)
        removed_parts += 1

    page_fields = sum(
        1
        for name, data in parts.items()
        if name.startswith("word/header")
        for text in etree.fromstring(data).xpath(".//w:instrText/text()", namespaces=NS)
        if "PAGE" in (text or "").upper()
    )
    sections = len(document.xpath(".//w:sectPr", namespaces=NS))
    footer_refs = len(document.xpath(".//w:footerReference", namespaces=NS))
    header_refs = len(document.xpath(".//w:headerReference", namespaces=NS))
    if sections != 3 or page_fields != 1 or footer_refs != 0 or header_refs != 1:
        raise RuntimeError(
            "Refusing to replace DOCX after cleanup: "
            f"sections={sections}, PAGE={page_fields}, headers={header_refs}, footers={footer_refs}"
        )

    parts[document_name] = etree.tostring(document, xml_declaration=True, encoding="UTF-8", standalone="yes")
    parts[rels_name] = etree.tostring(rels, xml_declaration=True, encoding="UTF-8", standalone="yes")
    parts[types_name] = etree.tostring(content_types, xml_declaration=True, encoding="UTF-8", standalone="yes")

    fd, temp_name = tempfile.mkstemp(prefix=f"{path.stem}.", suffix=".docx", dir=path.parent)
    os.close(fd)
    try:
        with zipfile.ZipFile(temp_name, "w", compression=zipfile.ZIP_DEFLATED) as target:
            for name, data in parts.items():
                target.writestr(name, data)
        os.replace(temp_name, path)
    finally:
        if os.path.exists(temp_name):
            os.unlink(temp_name)
    return removed_refs, removed_parts


if __name__ == "__main__":
    if len(sys.argv) != 2:
        raise SystemExit("Usage: cleanup_empty_header_footer.py <report.docx>")
    report = Path(sys.argv[1]).resolve()
    refs, story_parts = cleanup(report)
    print(f"[OK] Removed {refs} empty header/footer reference(s) and {story_parts} unused part(s).")
