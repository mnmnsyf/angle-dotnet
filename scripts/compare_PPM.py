#!/usr/bin/env python3
"""
Generic image pixel-similarity comparator.

Supported formats without external dependencies:
- PPM (P6, maxval=255)
- PGM (P5, maxval=255) -> expanded to RGB for comparison

If Pillow (PIL) is installed, PNG/JPG/etc. are supported as well.
"""

import argparse
import math
import sys
from pathlib import Path
from typing import Optional, Tuple

RgbImage = Tuple[int, int, bytes]  # width, height, rgb bytes


def try_load_with_pillow(path: Path) -> Optional[RgbImage]:
    try:
        from PIL import Image  # type: ignore
    except Exception:
        return None

    with Image.open(str(path)) as img:
        img = img.convert("RGB")
        w, h = img.size
        return (w, h, img.tobytes())


def _read_token(data: bytes, i: int) -> Tuple[bytes, int]:
    n = len(data)
    while i < n:
        c = data[i]
        if c in b" \t\r\n":
            i += 1
            continue
        if c == ord("#"):
            while i < n and data[i] not in b"\r\n":
                i += 1
            continue
        break

    start = i
    while i < n and data[i] not in b" \t\r\n":
        i += 1
    return data[start:i], i


def load_ppm_pgm(path: Path) -> RgbImage:
    data = path.read_bytes()
    i = 0
    magic, i = _read_token(data, i)
    if magic not in (b"P6", b"P5"):
        raise ValueError("Unsupported PPM/PGM magic: %r" % (magic,))

    w_b, i = _read_token(data, i)
    h_b, i = _read_token(data, i)
    max_b, i = _read_token(data, i)
    if not w_b or not h_b or not max_b:
        raise ValueError("Invalid PPM/PGM header.")

    w = int(w_b)
    h = int(h_b)
    maxval = int(max_b)
    if w <= 0 or h <= 0:
        raise ValueError("Invalid dimensions: %dx%d" % (w, h))
    if maxval != 255:
        raise ValueError("Only maxval=255 is supported (got %d)" % (maxval,))

    while i < len(data) and data[i] in b" \t\r\n":
        i += 1

    if magic == b"P6":
        needed = w * h * 3
        payload = data[i : i + needed]
        if len(payload) != needed:
            raise ValueError("Truncated PPM data (need %d bytes)" % (needed,))
        return (w, h, payload)

    needed = w * h
    gray = data[i : i + needed]
    if len(gray) != needed:
        raise ValueError("Truncated PGM data (need %d bytes)" % (needed,))
    rgb = bytearray(w * h * 3)
    for p in range(w * h):
        v = gray[p]
        o = p * 3
        rgb[o + 0] = v
        rgb[o + 1] = v
        rgb[o + 2] = v
    return (w, h, bytes(rgb))


def load_image_rgb(path: Path) -> RgbImage:
    if not path.exists():
        raise FileNotFoundError(str(path))

    pil = try_load_with_pillow(path)
    if pil is not None:
        return pil

    suffix = path.suffix.lower()
    if suffix in (".ppm", ".pgm"):
        return load_ppm_pgm(path)

    raise ValueError(
        "Unsupported image format: %s (install Pillow for PNG/JPG/etc.)" % path.name
    )


def compare_rgb(
    a_rgb: bytes,
    b_rgb: bytes,
    width: int,
    height: int,
    tolerance: int,
    crop_to_min: bool,
) -> Tuple[int, int, int, float, float, float]:
    pixels_a = (len(a_rgb) // 3)
    pixels_b = (len(b_rgb) // 3)
    if pixels_a == 0 or pixels_b == 0:
        return (0, 0, 0, 0.0, 0.0, 1.0)

    pixels = width * height
    total_channels = pixels * 3

    sum_abs = 0
    sum_sq = 0
    max_abs = 0
    within_tol_pixels = 0

    for p in range(pixels):
        base = p * 3
        dr = abs(a_rgb[base + 0] - b_rgb[base + 0])
        dg = abs(a_rgb[base + 1] - b_rgb[base + 1])
        db = abs(a_rgb[base + 2] - b_rgb[base + 2])

        sum_abs += dr + dg + db
        sum_sq += dr * dr + dg * dg + db * db
        if dr > max_abs:
            max_abs = dr
        if dg > max_abs:
            max_abs = dg
        if db > max_abs:
            max_abs = db

        if dr <= tolerance and dg <= tolerance and db <= tolerance:
            within_tol_pixels += 1

    mean_abs = float(sum_abs) / float(total_channels)
    mse = float(sum_sq) / float(total_channels)
    rmse = math.sqrt(mse)
    similarity = float(within_tol_pixels) / float(pixels) if pixels else 1.0
    return (pixels, within_tol_pixels, max_abs, mean_abs, rmse, similarity)


def main() -> int:
    parser = argparse.ArgumentParser(description="Compare two images by pixel similarity.")
    parser.add_argument("image_a", help="First image (PPM/PGM; or PNG/JPG if Pillow installed).")
    parser.add_argument("image_b", help="Second image.")
    parser.add_argument(
        "--tolerance",
        type=int,
        default=0,
        help="Per-channel absolute tolerance [0..255] (default: 0).",
    )
    parser.add_argument(
        "--min-similarity",
        type=float,
        default=1.0,
        help="Minimum fraction of pixels within tolerance (default: 1.0).",
    )
    parser.add_argument(
        "--crop-to-min",
        action="store_true",
        help="If sizes differ, compare the top-left overlapping area.",
    )
    args = parser.parse_args()

    tol = int(args.tolerance)
    if tol < 0 or tol > 255:
        print("FAIL: tolerance must be in [0..255].", file=sys.stderr)
        return 1
    if args.min_similarity < 0.0 or args.min_similarity > 1.0:
        print("FAIL: min-similarity must be in [0..1].", file=sys.stderr)
        return 1

    try:
        aw, ah, a_rgb = load_image_rgb(Path(args.image_a))
        bw, bh, b_rgb = load_image_rgb(Path(args.image_b))
    except Exception as e:
        print("FAIL: %s" % (e,), file=sys.stderr)
        return 1

    if (aw, ah) != (bw, bh):
        if not args.crop_to_min:
            print("FAIL: size mismatch (%dx%d vs %dx%d)" % (aw, ah, bw, bh))
            return 1

        w = min(aw, bw)
        h = min(ah, bh)
        if w <= 0 or h <= 0:
            print("FAIL: invalid crop dimensions.", file=sys.stderr)
            return 1

        def crop(rgb: bytes, src_w: int, src_h: int) -> bytes:
            out = bytearray(w * h * 3)
            for y in range(h):
                src_row = y * src_w * 3
                dst_row = y * w * 3
                out[dst_row : dst_row + (w * 3)] = rgb[src_row : src_row + (w * 3)]
            return bytes(out)

        a_rgb = crop(a_rgb, aw, ah)
        b_rgb = crop(b_rgb, bw, bh)
        aw, ah, bw, bh = w, h, w, h

    pixels, within_tol_pixels, max_abs, mean_abs, rmse, similarity = compare_rgb(
        a_rgb, b_rgb, aw, ah, tol, args.crop_to_min
    )

    if rmse == 0.0:
        psnr = float("inf")
    else:
        psnr = 20.0 * math.log10(255.0 / rmse)

    print("Images: %dx%d (%d pixels)" % (aw, ah, pixels))
    print("Tolerance: %d" % (tol,))
    print("Similarity: %.6f (within tolerance pixels: %d/%d)" % (similarity, within_tol_pixels, pixels))
    print(
        "MaxAbsDiff: %d, MeanAbsDiff: %.6f, RMSE: %.6f, PSNR: %s"
        % (
            max_abs,
            mean_abs,
            rmse,
            "inf" if psnr == float("inf") else "%.3f dB" % (psnr,),
        )
    )

    if similarity >= float(args.min_similarity):
        print(
            "PASS: 全部%d个像素中，有%d个像素在容差内（相似度=%.6f）"
            % (pixels, within_tol_pixels, similarity)
        )
        return 0

    print("FAIL: similarity %.6f < %.6f" % (similarity, float(args.min_similarity)))
    return 1


if __name__ == "__main__":
    raise SystemExit(main())
