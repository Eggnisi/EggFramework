#region

//文件创建者：Egg
//创建时间：11-10 10:02

#endregion

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Random = System.Random;

namespace EggFramework.Util
{
    public static class TranslateUtil
    {
        public static List<SystemLanguage> SupportedSystemLanguages = new()
        {
            SystemLanguage.English,
            SystemLanguage.ChineseSimplified,
            SystemLanguage.ChineseTraditional,
            SystemLanguage.Japanese,
            SystemLanguage.Korean,
            SystemLanguage.French,
            SystemLanguage.German,
            SystemLanguage.Spanish,
            SystemLanguage.Russian
        };

        public static string GetLanguageCodeByLanguageEnum(SystemLanguage language)
        {
            return language switch
            {
                SystemLanguage.English            => "en",
                SystemLanguage.ChineseSimplified  => "zh",
                SystemLanguage.ChineseTraditional => "cht",
                SystemLanguage.Japanese           => "jp",
                SystemLanguage.Korean             => "kor",
                SystemLanguage.French             => "fra",
                SystemLanguage.German             => "de",
                SystemLanguage.Spanish            => "spa",
                SystemLanguage.Russian            => "ru",
                _                                 => string.Empty
            };
        }

        private static string _appId;
        private static string _secretKey;

        public static void Init(string appid, string secretKey)
        {
            _appId     = appid;
            _secretKey = secretKey;
        }

        public static async UniTask<string> HttpGet(string url)
        {
            // 创建 HttpClient 实例
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.Timeout = TimeSpan.FromSeconds(60);
                    HttpResponseMessage response =
                        await client.GetAsync(url);
                    // 检查响应是否成功
                    response.EnsureSuccessStatusCode();

                    // 读取响应内容（异步操作）
                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException e)
                {
                    // 捕获并处理 HTTP 请求异常
                    Console.WriteLine($"请求错误: {e.Message}");
                }
            }

            return null;
        }


        public static async UniTask<string> Translate(string content,
            SystemLanguage originLanguage = SystemLanguage.ChineseSimplified,
            SystemLanguage targetLanguage = SystemLanguage.English)
        {
            // 原文
            string q = content;
            if (string.IsNullOrEmpty(content)) return null;
            // 源语言
            string from = GetLanguageCodeByLanguageEnum(originLanguage);
            // 目标语言
            string to = GetLanguageCodeByLanguageEnum(targetLanguage);
            // 改成您的APP ID
            string appId = "20241110002198669";
            Random rd    = new Random();
            string salt  = rd.Next(100000).ToString();
            // 改成您的密钥
            string secretKey = "XHTXHE0d9HQAFjZ2ff4g";
            string sign      = EncryptString(appId + q + salt + secretKey);
            string url       = "http://api.fanyi.baidu.com/api/trans/vip/translate?";
            url += "q=" + HttpUtility.UrlEncode(q);
            url += "&from=" + from;
            url += "&to=" + to;
            url += "&appid=" + appId;
            url += "&salt=" + salt;
            url += "&sign=" + sign;
            var retryKey = from + to + content;
            _retryCountDic[retryKey] = 0;
            var translate = await TryGetResult(retryKey, url); //

            return translate;
        }

        private static readonly Dictionary<string, int> _retryCountDic = new();

        private static async UniTask<string> TryGetResult(string retryKey, string url)
        {
            while (true)
            {
                var retString = await HttpGet(url);
                if (retString == null)
                {
                    return null;
                }

                var data = JsonConvert.DeserializeObject<SuccessTranslateResult>(retString);
                if (data is { trans_result: { Length: > 0 } })
                {
                    return data.trans_result.First().dst;
                }

                _retryCountDic[retryKey]++;
                await UniTask.Delay(50);
                if (_retryCountDic[retryKey] > 5) return null;
            }
        }

        public class SuccessTranslateResult
        {
            public class Result
            {
                public string src;
                public string dst;
            }

            public string   from;
            public string   to;
            public Result[] trans_result;
        }

        // 计算MD5值
        public static string EncryptString(string str)
        {
            MD5 md5 = MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }

            // 返回加密的字符串
            return sb.ToString();
        }
    }
}