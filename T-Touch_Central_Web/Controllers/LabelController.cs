using DATA;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Web.Mvc;

namespace T_Touch_Central_Web.Controllers
{
    public class LabelController : Controller
    {
        // GET: Label
        public ActionResult Index()
        {
            DirectoryInfo directory = null;
            FileInfo[] files = null;
            string path = Server.MapPath("~/Uploads");
            directory = new DirectoryInfo(path);
            files = directory.GetFiles();
            FileModel fs = new FileModel();
            fs.MyInfo = files;
            return View(fs);
        }
        // POST: Label/Delete/5
        [HttpPost]
        public ActionResult Delete(string Id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                string path = Server.MapPath("~/Uploads");
                if (Id.Contains(","))
                {
                    foreach (var item in Id.Split(',').ToArray())
                    {
                        if (System.IO.File.Exists(path + "\\" + item))
                        {
                            System.IO.File.Delete(path + "\\" + item);
                        }
                    }
                }
                else
                {
                    if (System.IO.File.Exists(path + "\\" + Id))
                    {
                        System.IO.File.Delete(path + "\\" + Id);
                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        public ActionResult Scale()
        {
            var db = new DB();
            var sql = from t in db.Scales select t;
            return PartialView(sql);
        }
        [HttpPost]
        public string DownLoad(string Id, string Ip)
        {
            var db = new DB();
            var result = string.Empty;
            var result1 = string.Empty;
            string zipPath = string.Empty;
            if (Id != "")
            {
                if (Id.Split(',').ToArray().Length == 1)
                {
                    if (Id.Contains(".zip") || Ip.Contains(".ZIP"))
                    {
                        zipPath = Server.MapPath("~/Uploads") + @"\" + Id;
                    }
                    else if (Id.Contains(".fmt")|| Id.Contains(".FMT"))
                    {
                        zipPath = Server.MapPath("~/Uploads") + @"\data-" + DateTime.Now.ToFileTimeUtc() + ".zip";
                        using (ZipFile zip = new ZipFile())
                        {
                            foreach (var item in Id.Split(',').ToArray())
                            {
                                zip.AddFile(Server.MapPath("~/Uploads") + @"\" + Id, "format_label");
                            }
                            zip.Save(zipPath);
                        }
                    }
                }
                else if (Id.Split(',').ToArray().Length>1 )
                {
                    zipPath = Server.MapPath("~/Uploads") + @"\data-" + DateTime.Now.ToFileTimeUtc() + ".zip";
                    using (ZipFile zip = new ZipFile())
                    {
                        foreach (var item in Id.Split(',').ToArray())
                        {
                            zip.AddFile(Server.MapPath("~/Uploads") + @"\" + item, "format_label");
                        }
                        zip.Save(zipPath);
                    }
                }
                else
                {
                    result = "文件错误！";
                    return result;
                }
                if (Ip != "")
                {
                    foreach (var item in Ip.Split(',').ToArray())
                    {
                        var Sql = db.Scales.SingleOrDefault(x => x.Id == int.Parse(item));
                        try
                        {
                            //测试IP
                            Ping pingSender = new Ping();
                            PingOptions options = new PingOptions();
                            string data = "";
                            byte[] buffer = Encoding.ASCII.GetBytes(data);
                            int timeout = 120;
                            PingReply reply = pingSender.Send(Sql.IpAddress, timeout, buffer, options);
                            if (reply.Status == IPStatus.Success)
                            {
                                TcpClient tcpClient = new TcpClient(Sql.IpAddress, 1000);
                                NetworkStream networkStream = tcpClient.GetStream();
                                //打开文件流
                                using (FileStream fs = new FileStream(zipPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    string filename = Path.GetFileName(zipPath);
                                    byte[] sendBytes = Encoding.Default.GetBytes("set:data:" + filename + ":" + fs.Length.ToString() + "\r\n");
                                    networkStream.Write(sendBytes, 0, sendBytes.Length);
                                    byte[] buffer1 = new byte[1024];
                                    int read, sent = 0;
                                    while ((read = fs.Read(buffer1, 0, 1024)) != 0)
                                    {
                                        networkStream.Write(buffer1, 0, buffer1.Length);
                                        sent += read;
                                    }
                                    fs.Close();
                                }
                                result += Sql.IpAddress + ":下载成功！" + Environment.NewLine;

                            }
                            else
                            {
                                result += Sql.IpAddress + ":网络断线！" + Environment.NewLine;
                            }
                        }
                        catch (Exception ex)
                        {
                            result += Sql.IpAddress + ":" + ex.Message + Environment.NewLine;
                        }
                    }
                }
            }
            if (System.IO.File.Exists(zipPath))
            {
                System.IO.File.Delete(zipPath);
            }
            return result;
        }
    }
}