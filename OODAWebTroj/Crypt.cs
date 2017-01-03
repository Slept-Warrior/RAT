using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace OODAWebTroj
{
    class Crypt
    {
        public static string Key = "XuYiMiao";

        public static string Iv = "19890604";

        public static string Encrypt(string sourceString)
        {
            try
            {
                var btKey = Encoding.UTF8.GetBytes(Key);
                var btIv = Encoding.UTF8.GetBytes(Iv);
                var des = new DESCryptoServiceProvider();
                using (var ms = new MemoryStream())
                {
                    var inData = Encoding.UTF8.GetBytes(sourceString);
                    try
                    {
                        using (var cs = new CryptoStream(ms, des.CreateEncryptor(btKey, btIv), CryptoStreamMode.Write))
                        {
                            cs.Write(inData, 0, inData.Length);
                            cs.FlushFinalBlock();
                        }
                        return Convert.ToBase64String(ms.ToArray());
                    }
                    catch (Exception)
                    {
                        return sourceString;
                    }
                }
            }
            catch (Exception)
            {
                //MessageBox.Show(ex.ToString());
            }
            return "DES加密出错";
        }

        public static string Decrypt(string encryptedString)
        {
            var btKey = Encoding.UTF8.GetBytes(Key);
            var btIv = Encoding.UTF8.GetBytes(Iv);
            var des = new DESCryptoServiceProvider();
            using (var ms = new MemoryStream())
            {
                var inData = Convert.FromBase64String(encryptedString);
                try
                {
                    using (var cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIv), CryptoStreamMode.Write))
                    {
                        cs.Write(inData, 0, inData.Length);
                        cs.FlushFinalBlock();
                    }
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
                catch (Exception)
                {
                    //MessageBox.Show(ex.ToString());

                    return encryptedString;
                }
            }
        }

        public static string Md5(string sourceString)
        {
            var convertString = sourceString;
            var md5 = new MD5CryptoServiceProvider();
            var t2 = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(convertString)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2.Substring(0, 10);
        }

        public static string WeakEnc(string str)
        {
            var array = Encoding.UTF8.GetBytes(str); //string转换的字母
            var bytecode = new List<byte>();
            foreach (var ascii in array)
            {
                bytecode.Add(Convert.ToByte(Convert.ToInt32(ascii) + 3));
            }
            return Encoding.UTF8.GetString(bytecode.ToArray());
        }

        public static string WeakDec(string str)
        {
            var array = Encoding.UTF8.GetBytes(str); //string转换的字母
            var bytecode = new List<byte>();
            foreach (var ascii in array)
            {
                bytecode.Add(Convert.ToByte(Convert.ToInt32(ascii) - 3));
            }
            return Encoding.UTF8.GetString(bytecode.ToArray());
        }

        internal class CgZipUtil
        {
            /// <summary>
            /// 解压
            /// </summary>
            /// <param name="value"></param>
            /// <returns></returns>
            public static DataSet GetDatasetByString(string value)
            {
                var ds = new DataSet();
                var cc = GZipDecompressString(value);
                var sr = new StringReader(cc);
                ds.ReadXml(sr);
                return ds;
            }

            /// <summary>
            /// 根据DATASET压缩字符串
            /// </summary>
            /// <param name="ds"></param>
            /// <returns></returns>
            public static string GetStringByDataset(string ds)
            {
                return GZipCompressString(ds);
            }

            /// <summary>
            /// 将传入字符串以GZip算法压缩后，返回Base64编码字符
            /// </summary>
            /// <param name="rawString">需要压缩的字符串</param>
            /// <returns>压缩后的Base64编码的字符串</returns>
            public static string GZipCompressString(string rawString)
            {
                if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
                {
                    return "";
                }
                var rawData = Encoding.UTF8.GetBytes(rawString);
                var zippedData = Compress(rawData);
                return Convert.ToBase64String(zippedData);
            }

            /// <summary>
            /// GZip压缩
            /// </summary>
            /// <param name="rawData"></param>
            /// <returns></returns>
            static byte[] Compress(byte[] rawData)
            {
                var ms = new MemoryStream();
                var compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
                compressedzipStream.Write(rawData, 0, rawData.Length);
                compressedzipStream.Close();
                return ms.ToArray();
            }


            /// <summary>
            /// 将传入的二进制字符串资料以GZip算法解压缩
            /// </summary>
            /// <param name="zippedString">经GZip压缩后的二进制字符串</param>
            /// <returns>原始未压缩字符串</returns>
            public static string GZipDecompressString(string zippedString)
            {
                if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
                {
                    return "";
                }
                var zippedData = Convert.FromBase64String(zippedString);
                return Encoding.UTF8.GetString(Decompress(zippedData));
            }


            /// <summary>
            /// ZIP解压
            /// </summary>
            /// <param name="zippedData"></param>
            /// <returns></returns>
            public static byte[] Decompress(byte[] zippedData)
            {
                var ms = new MemoryStream(zippedData);
                var compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
                var outBuffer = new MemoryStream();
                var block = new byte[1024];
                while (true)
                {
                    var bytesRead = compressedzipStream.Read(block, 0, block.Length);
                    if (bytesRead <= 0)
                        break;
                    outBuffer.Write(block, 0, bytesRead);
                }
                compressedzipStream.Close();
                return outBuffer.ToArray();
            }
        }
    }
}
