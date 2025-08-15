import os
import sys
import argparse
import numpy as np
from PIL import Image

sys.stdout.reconfigure(encoding='utf-8')
sys.stderr.reconfigure(encoding='utf-8')

def process_image(file_path, threshold):
    try:
        with Image.open(file_path) as img:
            # 转换为RGBA并转为numpy数组
            img_rgba = img.convert('RGBA')
            arr = np.array(img_rgba)
            
            # 提取alpha通道并找到非透明区域
            alpha = arr[:, :, 3]
            rows, cols = np.where(alpha > 0)
            
            # 处理全透明情况
            if len(rows) == 0 or len(cols) == 0:
                print(f"{file_path} 全透明，跳过处理")
                return

            # 计算包围盒
            min_y, max_y = np.min(rows), np.max(rows)
            min_x, max_x = np.min(cols), np.max(cols)
            
            # 计算扩展后的坐标（考虑图像边界）
            new_min_x = max(0, min_x - threshold)
            new_max_x = min(arr.shape[1]-1, max_x + threshold)
            new_min_y = max(0, min_y - threshold)
            new_max_y = min(arr.shape[0]-1, max_y + threshold)

            # 裁剪目标区域
            cropped = arr[new_min_y:new_max_y+1, new_min_x:new_max_x+1]
            
            # 创建新图像
            new_img = Image.fromarray(cropped, 'RGBA')
            
            # 覆盖保存
            new_img.save(file_path, 'PNG')
            print(f"{file_path} 处理完成，新尺寸：{cropped.shape[1]}x{cropped.shape[0]}")

    except Exception as e:
        print(f"处理 {file_path} 时发生错误: {str(e)}")

def main():
    parser = argparse.ArgumentParser(description='PNG透明区域处理工具（numpy加速版）')
    parser.add_argument('threshold', type=int, help='扩展像素数（非负整数）')
    parser.add_argument('folder', type=str, help='需要处理的图片文件夹路径')
    args = parser.parse_args()

    if args.threshold < 0:
        print("错误：threshold 必须为非负整数")
        sys.exit(1)

    if not os.path.isdir(args.folder):
        print(f"错误：路径 {args.folder} 无效")
        sys.exit(1)

    for root, _, files in os.walk(args.folder):
        for file in files:
            if file.lower().endswith('.png'):
                file_path = os.path.join(root, file)
                process_image(file_path, args.threshold)

if __name__ == "__main__":
    main()