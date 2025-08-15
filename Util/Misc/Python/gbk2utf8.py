import os
import sys
import shutil
sys.stdout.reconfigure(encoding='utf-8')  # Python 3.7+ 特性
sys.stderr.reconfigure(encoding='utf-8')
try:
    import chardet
except ImportError:
    chardet = None


def normalize_encoding(encoding):
    """将检测到的编码名称转换为Python可识别的格式"""
    if not encoding:
        return None
    encoding = encoding.lower()
    # 处理常见编码别名
    encoding_map = {
        'gb2312': 'gbk',
        'gb18030': 'gbk',
        'shift-jis': 'shift_jis',
        'euc-kr': 'euc_kr',
        'iso-8859-1': 'latin_1',
        'windows-1252': 'cp1252',
    }
    return encoding_map.get(encoding, encoding)


def detect_encoding(file_path):
    """检测文件编码并返回Python兼容的编码名称"""
    # 优先使用chardet检测
    if chardet:
        with open(file_path, 'rb') as f:
            raw_data = f.read()
            result = chardet.detect(raw_data)
            if result['confidence'] > 0.5:
                detected_enc = normalize_encoding(result['encoding'])
                # 验证编码是否正确
                try:
                    with open(file_path, 'r', encoding=detected_enc) as f_test:
                        f_test.read()
                    return detected_enc
                except (UnicodeDecodeError, LookupError):
                    pass  # 检测失败，继续尝试其他方法

    # 备选编码列表（按优先级排序）
    encodings_to_try = [
        'utf-8',  # 最常见
        'gbk',  # 中文
        'big5',  # 繁体中文
        'shift_jis',  # 日文
        'euc_kr',  # 韩文
        'latin_1',  # ISO-8859-1
        'cp1252',  # Windows-1252
        'utf-16',  # UTF-16（可能需要处理BOM）
    ]

    for enc in encodings_to_try:
        try:
            with open(file_path, 'r', encoding=enc) as f:
                f.read()
            # 二次验证UTF-8 BOM情况
            if enc == 'utf-8':
                with open(file_path, 'rb') as f_bom:
                    bom = f_bom.read(3)
                    if bom == b'\xef\xbb\xbf':
                        return 'utf-8-sig'
            return enc
        except UnicodeDecodeError:
            continue
        except LookupError:  # 不支持的编码
            continue

    return None  # 无法检测编码


def convert_encoding(file_path, source_encoding):
    """将文件从源编码转换为UTF-8"""
    try:
        # 创建备份文件
        backup_path = file_path + '.bak'
        shutil.copy2(file_path, backup_path)

        # 读取源编码内容
        with open(file_path, 'r', encoding=source_encoding, errors='strict') as f:
            content = f.read()

        # 写入UTF-8编码（无BOM）
        with open(file_path, 'w', encoding='utf-8', errors='strict') as f:
            f.write(content)

        print(f"成功转换: {file_path} ({source_encoding} → UTF-8)")
        os.remove(backup_path)  # 转换成功删除备份
    except Exception as e:
        print(f"转换失败: {file_path} - {str(e)}")
        # 恢复备份
        if os.path.exists(backup_path):
            shutil.move(backup_path, file_path)


def main():
    if len(sys.argv) < 2:
        print("请拖拽文件夹到脚本图标上或通过命令行指定路径")
        return

    directory = os.path.abspath(sys.argv[1])
    extensions = ['.cs', '.csv', '.json', '.txt', '.xml', '.html', '.js', '.py']  # 可扩展的文件类型

    print("——自动转换本地表格文件字符集为utf-8——")
    print(f"开始处理目录: {directory}")

    for root, dirs, files in os.walk(directory):
        for file in files:
            if any(file.lower().endswith(ext) for ext in extensions):
                file_path = os.path.join(root, file)
                print(f"检测文件: {file_path}", end='...')

                detected_enc = detect_encoding(file_path)
                if not detected_enc:
                    print("无法检测编码，跳过")
                    continue

                if detected_enc.lower() in ['utf-8', 'utf-8-sig']:
                    print("已经是UTF-8编码，跳过")
                    continue

                print(f"检测到编码: {detected_enc}")
                convert_encoding(file_path, detected_enc)


if __name__ == '__main__':
    main()