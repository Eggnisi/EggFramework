# -*- coding: UTF-8 -*-


import re
import sys

sys.stdout.reconfigure(encoding='utf-8')  # Python 3.7+ 特性
sys.stderr.reconfigure(encoding='utf-8')

import os
import json
import logging
import requests
import argparse


def column_id(col):
    ans = ""
    i = col
    while i > 0:
        m = int((i - 1) % 26)
        i = int((i - 1) / 26)
        ans = chr(m + 65) + ans
    return ans


def sanitize_filename(title):
    """清理文件名中的非法字符"""
    # 替换Windows/Linux文件系统非法字符为下划线
    return re.sub(r'[\\/*?:"<>|]', '_', title.strip())


def get_pull_params():
    parser = argparse.ArgumentParser(description='下载飞书云端表格')
    parser.add_argument('app_id', type=str, help='AppId')
    parser.add_argument('app_secret', type=str, help='密钥')
    parser.add_argument('folder', type=str, help='文件夹路径')
    parser.add_argument('sheet_token', type=str, help='表token')
    args = parser.parse_args()

    if not os.path.isdir(args.folder):
        print("错误：指定的路径不是有效目录", file=sys.stderr, flush=True)
        exit(1)
    return args.app_id, args.app_secret, args.folder, args.sheet_token


def get_update_remote_params():
    parser = argparse.ArgumentParser(description='下载飞书云端表格')
    parser.add_argument('app_id', type=str, help='AppId')
    parser.add_argument('app_secret', type=str, help='密钥')
    parser.add_argument('path', type=str, help='配置文件路径')
    parser.add_argument('branch', type=str, help='分支名称')
    args = parser.parse_args()

    if not os.path.isfile(args.path):
        print("错误：指定的路径不是有效路径", file=sys.stderr, flush=True)
        exit(1)
    return args.app_id, args.app_secret, args.path, args.branch


def get_update_authority_params():
    parser = argparse.ArgumentParser(description='下载飞书云端表格')
    parser.add_argument('app_id', type=str, help='AppId')
    parser.add_argument('app_secret', type=str, help='密钥')
    parser.add_argument('path', type=str, help='配置文件路径')
    parser.add_argument('sheet_token', type=str, help='表token')
    args = parser.parse_args()

    if not os.path.isfile(args.path):
        print("错误：指定的路径不是有效路径", file=sys.stderr, flush=True)
        exit(1)
    return args.app_id, args.app_secret, args.path, args.sheet_token


class LarkException(Exception):
    def __init__(self, code=0, msg=None):
        self.code = code
        self.msg = msg

    def __str__(self) -> str:
        return "{}:{}".format(self.code, self.msg)

    __repr__ = __str__


def request(method, url, headers, payload=None):
    if payload is None:
        payload = {}
    response = requests.request(method, url, headers=headers, json=payload)
    logging.info("URL: " + url)
    logging.info("X-Tt-Logid: " + response.headers['X-Tt-Logid'])
    logging.info("headers:\n" + json.dumps(headers, indent=2, ensure_ascii=False))
    logging.info("payload:\n" + json.dumps(payload, indent=2, ensure_ascii=False))
    resp = {}
    if response.text[0] == '{':
        resp = response.json()
        logging.info("response:\n" + json.dumps(resp, indent=2, ensure_ascii=False))
    else:
        logging.info("response:\n" + response.text)
    code = resp.get("code", -1)
    if code == -1:
        code = resp.get("StatusCode", -1)
    if code == -1 and response.status_code != 200:
        response.raise_for_status()
    if code != 0:
        raise LarkException(code=code, msg=resp.get("msg", ""))
    return resp
