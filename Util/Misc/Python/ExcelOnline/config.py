# -*- coding: UTF-8 -*-
import os
from dotenv import load_dotenv, find_dotenv

load_dotenv(find_dotenv())

# load from env
APP_ID = os.getenv("APP_ID")
APP_SECRET = os.getenv("APP_SECRET")
LARK_HOST = os.getenv("LARK_HOST")
APP_HOST = os.getenv("APP_HOST")
SHEET_TOKEN = os.getenv("SHEET_TOKEN")

MAX_RETRIES = 20
RETRY_INTERVAL = 0.1