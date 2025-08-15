# -*- coding: UTF-8 -*-
import csv
import logging
import os

import api
import config
from utils import get_pull_params, sanitize_filename

LOG_FORMAT = "%(asctime)s - %(levelname)s - %(message)s"
logging.basicConfig(format=LOG_FORMAT, level=logging.INFO)


def pull_sheet():
    app_id, app_secret, folder, sheet_token = get_pull_params()
    client = api.Client(config.LARK_HOST)
    access_token = client.get_tenant_access_token(app_id, app_secret)
    print("成功获取到access_token：" + access_token)
    sheets = client.get_sheets(access_token, sheet_token)

    sheets_content = client.get_sheets_content(access_token, sheet_token, list(sh['sheetId'] for sh in sheets))
    print(sheets_content)

    for value_range, sh in zip(sheets_content, sheets):
        sheet_title = sh["title"]
        sanitized_name = sanitize_filename(sheet_title)  # 避免重复计算
        folder_path = os.path.join(folder, sanitized_name)  # 使用os.path.join确保路径兼容性
        filename = f"{sanitized_name}.csv"
        filepath = os.path.join(folder_path, filename)
        try:
            os.makedirs(folder_path, exist_ok=True)
            with open(filepath, 'w', newline='', encoding='utf-8') as f:
                writer = csv.writer(f)
                writer.writerows(value_range['values'])  # 批量写入所有行
                print(f"✅ Successfully saved: {filename}")

        except PermissionError:
            print(f"⛔ Permission denied when saving {filename}")
        except Exception as e:
            print(f"❌ Failed to save {filename}: {str(e)}")


if __name__ == "__main__":
    pull_sheet()
