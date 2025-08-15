# -*- coding: UTF-8 -*-
import json
import time
from functools import wraps

import config
from utils import request


def retry(max_retries, retry_interval):
    def decorator(func):
        @wraps(func)
        def wrapped(*args, **kwargs):
            attempts = 0
            while attempts <= max_retries:
                try:
                    return func(*args, **kwargs)
                except Exception as e:
                    if attempts == max_retries:
                        raise e  # 达到上限后抛出异常
                    attempts += 1
                    time.sleep(retry_interval)
            return None

        return wrapped

    return decorator


class Client(object):
    def __init__(self, lark_host):
        self._host = lark_host

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def get_tenant_access_token(self, app_id, app_secret):
        url = self._host + "/open-apis/auth/v3/app_access_token/internal/"
        headers = {
            'Content-Type': 'application/json; charset=utf-8'
        }
        payload = {
            'app_id': app_id,
            'app_secret': app_secret
        }
        resp = request("POST", url, headers, payload)
        return resp['tenant_access_token']

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def get_user_access_token(self, tenant_access_token, code):
        url = self._host + "/open-apis/authen/v1/access_token"
        headers = {
            'Content-Type': 'application/json; charset=utf-8'
        }
        payload = {
            "grant_type": "authorization_code",
            "code": code,
            "app_access_token": tenant_access_token
        }
        resp = request("POST", url, headers, payload)
        return resp['data']['access_token']

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def get_root_folder_token(self, access_token):
        url = self._host + "/open-apis/drive/explorer/v2/root_folder/meta"
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }
        resp = request("GET", url, headers)
        return resp['data']['token']

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def get_folder_children(self, access_token, parent_folder_token):
        url = self._host + "/open-apis/drive/v1/files?folder_token=" + parent_folder_token
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }
        resp = request("GET", url, headers)
        return resp['data']['files']

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def create_folder(self, access_token, parent_folder_token, folder_name):
        url = self._host + "/open-apis/drive/v1/files/create_folder"
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }
        payload = {
            'folder_token': parent_folder_token,
            'name': folder_name
        }
        resp = request("POST", url, headers, payload)
        return resp['data']['token']

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def create_spreadsheet(self, access_token, foldertoken, title):
        url = self._host + "/open-apis/sheets/v3/spreadsheets"
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }
        payload = {
            "title": title,
            "folder_token": foldertoken
        }
        resp = request("POST", url, headers, payload)
        return resp['data']['spreadsheet']['spreadsheet_token'], resp['data']['spreadsheet']['url']

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def get_sheets(self, access_token, doctoken):
        url = self._host + "/open-apis/sheets/v3/spreadsheets/" + doctoken + "/sheets/query"
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }
        resp = request("GET", url, headers)
        return resp['data']['sheets']

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def delete_sheet(self, access_token, doctoken, sheed_id):
        url = self._host + "/open-apis/sheets/v2/spreadsheets/" + doctoken + "/sheets_batch_update"
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }
        payload = {
            'requests': [
                {
                    'deleteSheet': {
                        'sheetId': sheed_id
                    }
                }
            ]
        }
        resp = request("POST", url, headers, payload)
        return resp['data']['replies']

    @retry(max_retries=1, retry_interval=config.RETRY_INTERVAL)
    def add_sheet(self, access_token, doctoken, sheet_title):
        url = self._host + "/open-apis/sheets/v2/spreadsheets/" + doctoken + "/sheets_batch_update"
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }
        payload = {
            'requests': [
                {
                    'addSheet': {
                        'properties': {
                            'title': sheet_title,
                            'index': 0
                        }
                    }
                }
            ]
        }
        resp = request("POST", url, headers, payload)
        return resp['data']['replies']['addSheet']['properties']['sheetId']

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def add_sheets(self, access_token, doctoken, sheet_titles):
        url = self._host + "/open-apis/sheets/v2/spreadsheets/" + doctoken + "/sheets_batch_update"
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }

        payload = {
            'requests': [
                {
                    'addSheet': {
                        'properties': {
                            'title': title,
                            'index': 0
                        }
                    }
                }
                for title in sheet_titles
            ]
        }

        # 发送请求
        resp = request("POST", url, headers, payload)

        # 提取返回的 sheetIds（注意顺序与传入 titles 对应）
        sheet_ids = [
            reply['addSheet']['properties']['sheetId']
            for reply in resp.get('data', {}).get('replies', [])
        ]

        return sheet_ids

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def get_sheet_content(self, access_token, doctoken, sheetId):
        url = self._host + "/open-apis/sheets/v2/spreadsheets/" + doctoken + "/values/" + sheetId
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }
        resp = request("GET", url, headers)
        return resp['data']['valueRange']['values']

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def get_sheets_content(self, access_token, doctoken, sheetIds):
        url = self._host + "/open-apis/sheets/v2/spreadsheets/" + doctoken + "/values_batch_get?ranges=" + ",".join(
            sheetIds)
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }
        resp = request("GET", url, headers)
        return resp['data']['valueRanges']

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def get_first_sheetid(self, access_token, doctoken):
        url = self._host + "/open-apis/sheets/v2/spreadsheets/" + doctoken + "/metainfo"
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }
        resp = request("GET", url, headers)
        return resp['data']['sheets'][0]["sheetId"]

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def get_sheets(self, access_token, doctoken):
        url = self._host + "/open-apis/sheets/v2/spreadsheets/" + doctoken + "/metainfo"
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }
        resp = request("GET", url, headers)
        return resp['data']['sheets']

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def batch_update_values(self, access_token, doctoken, data):
        url = self._host + "/open-apis/sheets/v2/spreadsheets/" + doctoken + "/values_batch_update"
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }
        payload = data
        resp = request("POST", url, headers, payload)
        return resp['data']['spreadsheetToken']

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def batch_update_styles(self, access_token, doctoken, data):
        url = self._host + "/open-apis/sheets/v2/spreadsheets/" + doctoken + "/styles_batch_update"
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }
        payload = data
        resp = request("PUT", url, headers, payload)
        return resp['data']['spreadsheetToken']

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def add_permissions_member(self, access_token, doctoken, doctype, member_type, member_id, perm):
        url = self._host + "/open-apis/drive/v1/permissions/" + doctoken + "/members?type=" + doctype + "&need_notification=false"
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }
        payload = {
            "member_type": member_type,
            "member_id": member_id,
            "perm": perm
        }
        request("POST", url, headers, payload)

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def add_permissions_members(self, access_token, doctoken, doctype, member_type, member_ids, perm):
        url = self._host + "/open-apis/drive/v1/permissions/" + doctoken + "/members/batch_create?type=" + doctype + "&need_notification=false"
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }
        members_payload = []
        for member_id in member_ids:
            members_payload.append({
                "member_type": member_type,
                "member_id": member_id,
                "perm": perm
            })

        payload = {
            "members": members_payload
        }
        request("POST", url, headers, payload)

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def set_permissions(self, access_token, doctoken, can_edit):
        url = self._host + "/open-apis/drive/v2/permissions/" + doctoken + "/public?type=sheet"
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }
        payload = {
            "external_access_entity": "open",
            "security_entity": "anyone_can_view",
            "comment_entity": "anyone_can_view",
            "share_entity": "anyone",
            "manage_collaborator_entity": "collaborator_can_view",
            "link_share_entity": "anyone_editable" if can_edit else "anyone_readable",
            "copy_entity": "anyone_can_view"
        }
        resp = request("PATCH", url, headers, payload)
        return resp['data']

    @retry(max_retries=config.MAX_RETRIES, retry_interval=config.RETRY_INTERVAL)
    def batch_get_id(self, access_token, mobiles):
        url = self._host + "/open-apis/contact/v3/users/batch_get_id"
        headers = {
            'Content-Type': 'application/json; charset=utf-8',
            'Authorization': 'Bearer ' + access_token
        }
        payload = {
            'mobiles': mobiles
        }
        resp = request("POST", url, headers, payload)
        return resp['data']['user_list']
