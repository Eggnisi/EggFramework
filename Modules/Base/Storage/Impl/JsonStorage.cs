using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

#if ENABLE_STORAGE_ENCRYPTION
using System;
using ICSharpCode.SharpZipLib.GZip;
#endif

namespace EggFramework.Storage
{
    public sealed class JsonStorage : IStorage
    {
#if ENABLE_STORAGE_ENCRYPTION
        private string _savePath = Path.Combine(Application.persistentDataPath, "fileInfo.dat");
#else
        private string _savePath = Path.Combine(Application.persistentDataPath, "fileInfo.json");
#endif
        private Dictionary<string, object> _saveData = new();

        // 加密常量（实际项目中应从安全配置获取）
        private const string ENCRYPTION_KEY = "EggFramework_Encrypt!n_K3y_32bytes";
        private const int BUFFER_SIZE = 4096;

        public void ReadFromDisk()
        {
            if (File.Exists(_savePath))
            {
                byte[] fileData = File.ReadAllBytes(_savePath);
                
#if ENABLE_STORAGE_ENCRYPTION
                try 
                {
                    byte[] decrypted = Decrypt(fileData);
                    using (var ms = new MemoryStream(decrypted))
                    using (var reader = new StreamReader(ms))
                    {
                        _saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(reader.ReadToEnd());
                    }
                }
                catch (CryptographicException)
                {
                    Debug.LogWarning("解密失败，尝试读取未加密数据");
                    ReadPlainJson(fileData);
                }
#else
                ReadPlainJson(fileData);
#endif
            }
            else
            {
                _saveData.Clear();
            }
        }

        private void ReadPlainJson(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            _saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }

        public void ReadFromText(string json)
        {
            _saveData = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }

        public void WriteToDisk()
        {
            string json = JsonConvert.SerializeObject(_saveData);
            byte[] data = Encoding.UTF8.GetBytes(json);

#if ENABLE_STORAGE_ENCRYPTION
            byte[] encrypted = Encrypt(data);
            File.WriteAllBytes(_savePath, encrypted);
#else
            File.WriteAllBytes(_savePath, data);
#endif
        }

#if ENABLE_STORAGE_ENCRYPTION
        private byte[] Encrypt(byte[] input)
        {
            using (Aes aes = Aes.Create())
            {
                // 配置加密器
                aes.Key     = GetValidKeyBytes(ENCRYPTION_KEY);
                aes.Mode    = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // 生成随机IV
                aes.GenerateIV();
                
                using (var encryptor = aes.CreateEncryptor())
                using (var output = new MemoryStream())
                {
                    // 写入IV
                    output.Write(aes.IV, 0, aes.IV.Length);
                    
                    // 添加压缩层
                    using (var gzip = new GZipOutputStream(output) { IsStreamOwner = false })
                    {
                        gzip.Write(input, 0, input.Length);
                        gzip.Flush();
                    }
                    
                    // 执行加密
                    return encryptor.TransformFinalBlock(output.ToArray(), 0, (int)output.Length);
                }
            }
        }
        
        private static byte[] GetValidKeyBytes(string key)
        {
            byte[] keyBytes      = Encoding.UTF8.GetBytes(key);
            byte[] validKeyBytes = new byte[32];
            
            Array.Copy(keyBytes, 0, validKeyBytes, 0, Math.Min(keyBytes.Length, validKeyBytes.Length));
            return validKeyBytes;
        }

        private byte[] Decrypt(byte[] input)
        {
            using (Aes aes = Aes.Create())
            {
                // 提取IV（前16字节）
                byte[] iv = new byte[16];
                Buffer.BlockCopy(input, 0, iv, 0, iv.Length);
                
                // 配置解密器
                aes.Key = GetValidKeyBytes(ENCRYPTION_KEY);
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                
                // 解密数据
                using (var decryptor = aes.CreateDecryptor())
                {
                    byte[] decrypted = decryptor.TransformFinalBlock(input, iv.Length, input.Length - iv.Length);
                    
                    // 解压数据
                    using (var inputStream = new MemoryStream(decrypted))
                    using (var gzip = new GZipInputStream(inputStream))
                    using (var output = new MemoryStream())
                    {
                        byte[] buffer = new byte[BUFFER_SIZE];
                        int read;
                        while ((read = gzip.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            output.Write(buffer, 0, read);
                        }
                        return output.ToArray();
                    }
                }
            }
        }
#endif

        public void Save<T>(string key, T saveData)
        {
            if (saveData == null)
            {
                Debug.LogWarning("Saving null data");
                return;
            }

            if (!_saveData.TryAdd(key, saveData))
            {
                _saveData[key] = saveData;
            }
        }

        public T Load<T>(string key, T defaultValue, out bool newValue)
        {
            newValue = false;

            if (!_saveData.TryGetValue(key, out var saveValue))
            {
                newValue = true;
                return defaultValue;
            }

            if (typeof(T) == typeof(bool) || typeof(T) == typeof(string))
            {
                return (T)saveValue;
            }

            if (typeof(IList).IsAssignableFrom(typeof(T)))
            {
                if (saveValue.ToString() == "[]")
                {
                    newValue = true;
                    return defaultValue;
                }

                if (saveValue is not JArray && saveValue is IList)
                {
                    return (T)saveValue;
                }
            }

            return JsonConvert.DeserializeObject<T>(saveValue.ToString());
        }
#if UNITY_EDITOR

        public void AssignEditorSavePath(string path)
        {
#if ENABLE_STORAGE_ENCRYPTION
            _savePath = path + ".dat";
#else
            _savePath = path + ".json";
#endif
        }
#endif

        public void AssignSavePath(string path = "fileInfo")
        {
#if ENABLE_STORAGE_ENCRYPTION
            _savePath = Path.Combine(Application.persistentDataPath, path + ".dat");
#else
            _savePath = Path.Combine(Application.persistentDataPath, path + ".json");
#endif
        }

        public void Clear()
        {
            _saveData.Clear();
        }
    }
}