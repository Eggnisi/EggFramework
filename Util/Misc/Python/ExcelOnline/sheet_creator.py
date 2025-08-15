# -*- coding: UTF-8 -*-
import json
import logging

import api
import config
from utils import get_update_remote_params, column_id

LOG_FORMAT = "%(asctime)s - %(levelname)s - %(message)s"
logging.basicConfig(format=LOG_FORMAT, level=logging.INFO)


def build_data_body(sheet_id_dic):
    body = {
        "valueRanges": [

        ]
    }

    for sheet_id, table_config in sheet_id_dic.items():
        range_element = {
            "values": []
        }

        cols = len(table_config['Columns'])
        range_element['range'] = sheet_id + "!A1:" + column_id(cols) + "2"

        first_row = []
        second_row = []

        for col in table_config['Columns']:
            name = col['Name']
            chinese_name = col['ChineseName']
            col_type = col['ExcelColType']
            is_list = col['IsList']

            # 构建第一个字符串
            if not is_list:
                first_str = f"*{name}({col_type})"
            else:
                first_str = f"*{name}(List<{col_type}>)"

            first_row.append(first_str)

            # 构建第二个字符串
            second_str = f"*{chinese_name}"
            second_row.append(second_str)

        range_element['values'].append(first_row)
        range_element['values'].append(second_row)
        body["valueRanges"].append(range_element)

    return body


def create_sheet():
    app_id, app_secret, path, branch = get_update_remote_params()
    print("配置文件路径：" + path)
    with open(path, 'r', encoding='utf-8') as file:
        data = json.load(file)

    client = api.Client(config.LARK_HOST)
    access_token = client.get_tenant_access_token(app_id, app_secret)
    print("成功获取到access_token：" + access_token)
    root_folder_token = client.get_root_folder_token(access_token)
    print("成功获取到root_folder_token：" + root_folder_token)
    files = client.get_folder_children(access_token, root_folder_token)
    folder_token = next((file['token'] for file in files if file['name'] == "EggFramework"), None)
    if folder_token is not None:
        print(f"找到名为'EggFramework'的文件夹，其token是: {folder_token}")
    else:
        folder_token = client.create_folder(access_token, root_folder_token, "EggFramework")
        print(f"成功创建'EggFramework'文件夹，其token是: {folder_token}")

    spread_sheets = client.get_folder_children(access_token, folder_token)
    sheet_token = next(
        (spreadsheet['token'] for spreadsheet in spread_sheets if
         spreadsheet['name'] == f"EggFramework工作表_{branch}"),
        None)
    if sheet_token is not None:
        print(f"找到名为'EggFramework工作表_{branch}'的文件，其token是: {sheet_token}")
    else:
        sheet_token, url = client.create_spreadsheet(access_token, folder_token, f"EggFramework工作表_{branch}")
        print(f"成功创建'EggFramework工作表_{branch}'，其token是: {sheet_token}")
        for br in data['ExcelOnlineSetting']['Branches']:
            if br['Name'] == branch:
                br['Token'] = sheet_token
                br['Url'] = url
        with open(path, 'w', encoding='utf-8') as file:
            json.dump(data, file, indent=4, ensure_ascii=False)

    sheets = client.get_sheets(access_token, sheet_token)
    config_names = {cfg['ConfigName'] for cfg in data['ExcelTableConfigs']}
    # 收集所有已存在的 sheet 标题及其 sheetId
    existing_sheets_dict = {sheet['title']: sheet['sheetId'] for sheet in sheets}
    # 找出缺失的 sheet 名称
    missing_sheets = list(config_names - set(existing_sheets_dict.keys()))
    if len(missing_sheets) > 0:
        # 创建缺失的 sheets 并获得新的 sheetIds
        created_sheet_ids = client.add_sheets(access_token, sheet_token, missing_sheets)
        # 将新创建的 sheets 添加到 existing_sheets_dict 中
        for title, sheet_id in zip(missing_sheets, created_sheet_ids):
            existing_sheets_dict[title] = sheet_id
    else:
        print("没有缺失的工作表需要创建。")

    for sheet in sheets:
        if sheet['title'] == 'Sheet1':
            client.delete_sheet(access_token, sheet_token, sheet['sheetId'])

    sheet_id_dic = {}
    for title, sheetId in existing_sheets_dict.items():
        found_config = next((cfg for cfg in data['ExcelTableConfigs'] if cfg['ConfigName'] == title), None)
        if found_config:
            sheet_id_dic[sheetId] = found_config
            print(f"找到配置信息：{title}")
        else:
            print(f"未找到 ConfigName 为 {title} 的配置")

    client.batch_update_values(access_token, sheet_token, build_data_body(sheet_id_dic))


if __name__ == "__main__":
    create_sheet()
