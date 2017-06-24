using DATA;
using DATA.model;
using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace T_Touch_Central_Web.Controllers
{
    public class ScaleController : Controller
    {
        // GET: Scale
        public ActionResult Index(string ScaleNo)
        {
            var db = new DB();
            var ip = from t in db.Scales select new { t.Id, t.IpAddress };
            string result = string.Empty;

            foreach (var item in ip)
            {
                Ping pingSender = new Ping();
                PingOptions options = new PingOptions();
                string data = "";
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                int timeout = 120;
                PingReply reply = pingSender.Send(item.IpAddress, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                {
                    db.UpdateState(item.Id.ToString(), "True", ref result);
                }
                else
                {
                    db.UpdateState(item.Id.ToString(), "False", ref result);
                }
            }

            var sql = from t in db.Scales select t;
            if (!string.IsNullOrEmpty(ScaleNo))
            {
                sql = sql.Where(s => s.Pos_No.Contains(ScaleNo));
            }
            return View(sql);
        }

        // GET: Scale/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Scale/Create
        public ActionResult Create()
        {
            var db = new DB();
            var maxposno = (from t in db.Scales select t.Pos_No).Max();
            if (maxposno != null)
            {
                ViewBag.Message = int.Parse(maxposno) + 1;
            }
            else
            {
                ViewBag.Message = 1;
            }
            return View();
        }

        // ToBase64
        public static string ToBase64(HttpPostedFileBase file)
        {
            Stream stream = file.InputStream;
            byte[] bytes = new byte[stream.Length];
            long data = file.InputStream.Read(bytes, 0, (int)stream.Length);
            stream.Close();
            return Convert.ToBase64String(bytes, 0, bytes.Length);
        }

        // POST: Scale/Create
        [HttpPost]
        public ActionResult Create(Scales Sql)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here

                    string images = string.Empty;
                    images = ToBase64(Request.Files["Images"]);

                    string images1 = string.Empty;
                    images1 = ToBase64(Request.Files["Images1"]);

                    var db = new DB();
                    var Scale = new Scales
                    {
                        Shop_id = Sql.Shop_id,
                        Branch_id = Sql.Branch_id,
                        Pos_No = Sql.Pos_No,
                        ScaleName = Sql.ScaleName,
                        IpAddress = Sql.IpAddress,
                        Enable = Sql.Enable,
                        Images = images,
                        Images1 = images1,
                        Color = Sql.Color,
                        Remark = Sql.Remark
                    };

                    db.Scales.InsertOnSubmit(Scale);
                    db.SubmitChanges();
                    return RedirectToAction("Index");
                }
                catch
                {
                    return View(Sql);
                }
            }
            else
            {
                return View(Sql);
            }
        }

        // GET: Scale/Edit/5
        public ActionResult Edit(int id)
        {
            var db = new DB();
            var Sql = db.Scales.SingleOrDefault(x => x.Id == id);
            return View(Sql);

        }

        // POST: Scale/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here


                string images = string.Empty;
                images = ToBase64(Request.Files["Images"]);

                string images1 = string.Empty;
                images1 = ToBase64(Request.Files["Images1"]);

                var db = new DB();
                var Sql = db.Scales.SingleOrDefault(x => x.Id == id);
                if (images != string.Empty)
                {
                    Sql.Images = images;
                }
                if (images1 != string.Empty)
                {
                    Sql.Images1 = images1;
                }

                UpdateModel(Sql, collection.ToValueProvider());
                db.SubmitChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }

        // GET: Scale/Delete/5
        public ActionResult Delete(int id)
        {
            var db = new DB();
            var Sql = db.Scales.SingleOrDefault(x => x.Id == id);
            return View(Sql);
        }

        // POST: Scale/Delete/5
        [HttpPost]
        public ActionResult Delete(string Id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var db = new DB();

                if (Id.Contains(","))
                {
                    foreach (var item in Id.Split(',').ToArray())
                    {
                        var Sql = db.Scales.SingleOrDefault(x => x.Id == int.Parse(item));
                        db.Scales.DeleteOnSubmit(Sql);
                        db.SubmitChanges();
                    }
                }
                else
                {
                    var Sql = db.Scales.SingleOrDefault(x => x.Id == int.Parse(Id));
                    db.Scales.DeleteOnSubmit(Sql);
                    db.SubmitChanges();
                }
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public string DownLoad(string Id)
        {
            var result = string.Empty;
            var db = new DB();
            foreach (var item in Id.Split(',').ToArray())
            {
                var Sql = db.Scales.SingleOrDefault(x => x.Id == int.Parse(item));

                try
                {
                    //发送请求
                    string[] textArray1 = new string[] { "http://", Sql.IpAddress, ":", "1235", "/info" };
                    string uri = string.Concat(textArray1);
                    if (Sql.Shop_id == null)
                    {
                        result += Sql.IpAddress + ":" + "店号为空！" + Environment.NewLine;
                        continue;
                    }
                    if (Sql.Branch_id == null)
                    {
                        result += Sql.IpAddress + ":" + "部门为空！" + Environment.NewLine;
                        continue;
                    }
                    if (Sql.Pos_No == null)
                    {
                        result += Sql.IpAddress + ":" + "称号为空！" + Environment.NewLine;
                        continue;
                    }
                    //测试IP
                    Ping pingSender = new Ping();
                    PingOptions options = new PingOptions();
                    string data = "";
                    byte[] buffer = Encoding.ASCII.GetBytes(data);
                    int timeout = 120;
                    PingReply reply = pingSender.Send(Sql.IpAddress, timeout, buffer, options);
                    if (reply.Status == IPStatus.Success)
                    {
                        //方法1
                        string result1 = json.JsonTree(HttpHelper.HttpPost(uri,
                      "{\"shop_id\":" + Sql.Shop_id +
                      ", \"department_id\":" + Sql.Branch_id +
                      ", \"scale_id\":" + Sql.Pos_No +
                      "}")) +
                      Environment.NewLine;
                        if (result1.Contains("OK"))
                        {
                            result += Sql.IpAddress + ":下载成功！" + Environment.NewLine;
                        }
                        else
                        {
                            result += Sql.IpAddress + ":下载失败！" + Environment.NewLine;
                        }
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
            return result;
        }
    }
}
