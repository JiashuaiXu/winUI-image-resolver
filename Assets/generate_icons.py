#!/usr/bin/env python3
"""
generate_icons.py
【完全硬编码】为 Image Resolver (WinUI 3) 自动绘制并生成 MSIX 图标
无需任何输入图片，纯代码绘制
"""

import os
from PIL import Image, ImageDraw, ImageFont

# ============================= 配置区 =============================
OUTPUT_DIR = "Assets"  # 输出目录（已存在）

ICON_SIZES = {
    "StoreLogo.png":         (50, 50),
    "Square44x44Logo.png":   (44, 44),
    "Square150x150Logo.png": (150, 150),
    "Wide310x150Logo.png":   (310, 150),
}

# 颜色方案
BG_START = (30, 60, 120)      # 深蓝
BG_END   = (70, 110, 180)     # 浅蓝渐变
IMG_COLOR = (255, 255, 255)   # 白色图片图标
MAG_COLOR = (255, 220, 100)   # 金色放大镜
# =================================================================

def create_base_icon(size):
    """绘制基础图标：图片 + 放大镜"""
    w, h = size
    img = Image.new('RGBA', (w, h), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    # 1. 渐变背景
    for y in range(h):
        ratio = y / h
        r = int(BG_START[0] * (1 - ratio) + BG_END[0] * ratio)
        g = int(BG_START[1] * (1 - ratio) + BG_END[1] * ratio)
        b = int(BG_START[2] * (1 - ratio) + BG_END[2] * ratio)
        draw.line([(0, y), (w, y)], fill=(r, g, b))

    # 2. 图片边框（居中）
    margin = int(min(w, h) * 0.15)
    pic_w = w - 2 * margin
    pic_h = h - 2 * margin
    pic_x = margin
    pic_y = margin

    # 外框
    draw.rectangle([pic_x, pic_y, pic_x + pic_w, pic_y + pic_h], outline=IMG_COLOR, width=max(1, w//30))
    # 顶部小翻页
    draw.polygon([
        (pic_x, pic_y),
        (pic_x + pic_w * 0.3, pic_y),
        (pic_x + pic_w * 0.3, pic_y - pic_h * 0.1),
        (pic_x, pic_y - pic_h * 0.1)
    ], fill=IMG_COLOR)

    # 3. 放大镜（右下角）
    mag_size = int(min(w, h) * 0.28)
    mag_x = w - margin - mag_size
    mag_y = h - margin - mag_size

    # 镜框
    draw.ellipse([mag_x, mag_y, mag_x + mag_size, mag_y + mag_size], outline=MAG_COLOR, width=max(1, w//40))
    # 镜柄
    handle_len = mag_size * 0.6
    handle_angle = -45
    import math
    dx = handle_len * math.cos(math.radians(handle_angle))
    dy = handle_len * math.sin(math.radians(handle_angle))
    draw.line([
        (mag_x + mag_size, mag_y + mag_size),
        (mag_x + mag_size + dx, mag_y + mag_size + dy)
    ], fill=MAG_COLOR, width=max(1, w//40))

    return img

def main():
    script_dir = os.path.dirname(os.path.abspath(__file__))
    out_dir = os.path.join(script_dir, OUTPUT_DIR)

    os.makedirs(out_dir, exist_ok=True)
    print(f"正在生成图标到: {out_dir}")

    for filename, size in ICON_SIZES.items():
        icon = create_base_icon(size)
        out_path = os.path.join(out_dir, filename)
        icon.save(out_path, "PNG")
        print(f"生成: {filename} ({size[0]}×{size[1]})")

    print("\n所有图标生成完成！")
    print("可直接用于 MSIX 打包或自包含发布")
    print("图标风格：深蓝渐变 + 图片 + 放大镜（信息查看主题）")

if __name__ == "__main__":
    main()