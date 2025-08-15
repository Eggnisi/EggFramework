# -*- coding: UTF-8 -*-
import json
import logging

import api
import config
from utils import get_update_authority_params

LOG_FORMAT = "%(asctime)s - %(levelname)s - %(message)s"
logging.basicConfig(format=LOG_FORMAT, level=logging.INFO)


def update_authority():
    app_id, app_secret, path, sheet_token = get_update_authority_params()
    print("配置文件路径：" + path)
    with open(path, 'r', encoding='utf-8') as file:
        data = json.load(file)

    client = api.Client(config.LARK_HOST)
    access_token = client.get_tenant_access_token(app_id, app_secret)
    print("成功获取到access_token：" + access_token)

    if data['ExcelOnlineSetting']['IsPublic']:
        client.set_permissions(access_token, sheet_token, True)
    else:
        client.set_permissions(access_token, sheet_token, False)

        if len(data['ExcelOnlineSetting']['Mobiles']) > 0:
            user_list = client.batch_get_id(access_token, data['ExcelOnlineSetting']['Mobiles'])
            client.add_permissions_members(access_token, sheet_token, "sheet", "openid",
                                           list(user['user_id'] for user in user_list if user['user_id']), "edit")


if __name__ == "__main__":
    update_authority()
