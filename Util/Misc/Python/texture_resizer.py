import os
import sys
import argparse
from PIL import Image

sys.stdout.reconfigure(encoding='utf-8')  # Python 3.7+ 特性
sys.stderr.reconfigure(encoding='utf-8')

def process_image(file_path):
    try:
        with Image.open(file_path) as img:
            width, height = img.size
            # 检查是否已经符合要求
            if width % 4 == 0 and height % 4 == 0:
                print(f"{file_path} 符合要求，无需处理")
                return
            
            # 计算新的宽度和高度（向下取到最近的4的倍数）
            new_width = (width // 4) * 4
            new_height = (height // 4) * 4
            
            # 检查新尺寸是否有效
            if new_width <= 0 or new_height <= 0:
                print(f"{file_path} 尺寸过小，无法裁剪")
                return
            
            # 计算裁剪区域（居中裁剪）
            left = (width - new_width) // 2
            upper = (height - new_height) // 2
            right = left + new_width
            lower = upper + new_height
            
            # 执行裁剪
            cropped_img = img.crop((left, upper, right, lower))
            
            # 根据扩展名确定保存格式
            file_ext = os.path.splitext(file_path)[1].lower()
            if file_ext == '.png':
                save_format = 'PNG'
            else:
                save_format = 'JPEG'
                if cropped_img.mode != 'RGB':
                    cropped_img = cropped_img.convert('RGB')
            
            # 保存处理后的图片
            cropped_img.save(file_path, save_format)
            print(f"{file_path} 已裁剪为 {new_width}x{new_height}")
    
    except Exception as e:
        print(f"处理 {file_path} 时发生错误: {str(e)}")

def main():
    parser = argparse.ArgumentParser(description='图片批量裁剪工具')
    parser.add_argument('folder', 
                      type=str, 
                      help='需要处理的图片文件夹路径')
    args = parser.parse_args()
    
    if not os.path.isdir(args.folder):
        print(f"错误：路径 {args.folder} 不是一个有效的文件夹")
        sys.exit(1)

    for root, dirs, files in os.walk(args.folder):
        for file in files:
            if file.lower().endswith(('.jpg', '.png')):
                file_path = os.path.join(root, file)
                process_image(file_path)

if __name__ == "__main__":
    main()