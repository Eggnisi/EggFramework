import os
import sys

def count_lines_in_file(file_path):
    """统计单个文件的行数（严格UTF-8模式）"""
    try:
        with open(file_path, 'r', encoding='utf-8', errors='strict') as f:  # 添加严格错误处理
            return sum(1 for _ in f)
    except Exception as e:
        print(f"无法读取文件 {file_path}: {str(e)}", file=sys.stderr, flush=True)  # 错误输出到stderr
        return 0

def calculate_code_statistics(folder_path):
    """统计代码时忽略Generator目录下的所有cs文件"""
    total_lines = 0
    file_count = 0
    
    for root, dirs, files in os.walk(folder_path):
        # 过滤Generator目录（修改点1）
        if 'Generator' in os.path.normpath(root).split(os.sep):
            dirs[:] = []  # 清空待遍历子目录
            continue
        
        # 处理当前目录文件（修改点2）
        cs_files = [f for f in files if f.endswith('.cs')]
        for file in cs_files:
            file_path = os.path.join(root, file)
            lines = count_lines_in_file(file_path)
            total_lines += lines
            file_count += 1
    
    final_result = total_lines - (file_count * 7)
    return {
        "total_files": file_count,
        "raw_lines": total_lines,
        "final_result": final_result
    }

if __name__ == "__main__":
    # 强制设置标准流编码
    sys.stdout.reconfigure(encoding='utf-8')  # Python 3.7+ 特性
    sys.stderr.reconfigure(encoding='utf-8')
    
    import argparse
    
    parser = argparse.ArgumentParser(description='统计C#代码行数')
    parser.add_argument('folder', type=str, help='要扫描的文件夹路径')
    args = parser.parse_args()
    
    if not os.path.isdir(args.folder):
        print("错误：指定的路径不是有效目录", file=sys.stderr, flush=True)
        exit(1)
    
    stats = calculate_code_statistics(args.folder)
    
    # 添加flush保证及时输出
    print(f"统计结果：", flush=True)
    print(f"文件总数: {stats['total_files']}", flush=True)
    print(f"原始代码行数: {stats['raw_lines']}", flush=True)
    print(f"最终计算结果（扣除{stats['total_files']}×7）: {stats['final_result']}", flush=True)