using DATA;
using DATA.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Web.Mvc;
using PagedList;
namespace T_Touch_Central_Web.Controllers
{
    public class TaxController : Controller
    {
        // GET: Tax
        public ActionResult Index(int? page, string TaxName)
        {
            var db = new DB();
            var sql = from t in db.Tab_Tax select t;
            if (!string.IsNullOrEmpty(TaxName))
            {
                sql = sql.Where(s => s.tax_name.Contains(TaxName));
            }
            return View(sql.OrderBy(s => s.tax_num).ToPagedList(page ?? 1, 8));
        }

        // GET: Tax/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Tax/Create
        [Authorize]
        public ActionResult Create()
        {
            var db = new DB();
            var maxtaxnum = (from t in db.Tab_Tax select t.tax_num).Max();
            if (maxtaxnum != null)
            {
                ViewBag.Message = int.Parse(maxtaxnum) + 1;
            }
            else
            {
                ViewBag.Message = 1;
            }
            return View();
        }

        // POST: Tax/Create
        [HttpPost]
        public ActionResult Create(Tab_Tax Sql)
        {
            try
            {
                // TODO: Add insert logic here
                var db = new DB();
                db.Tab_Tax.InsertOnSubmit(Sql);
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Tax/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            var db = new DB();
            var Sql = db.Tab_Tax.SingleOrDefault(x => x._id == id);
            return View(Sql);
        }

        // POST: Tax/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                var db = new DB();
                var Sql = db.Tab_Tax.SingleOrDefault(x => x._id == id);
                UpdateModel(Sql, collection.ToValueProvider());
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Tax/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            var db = new DB();
            var Sql = db.Tab_Tax.SingleOrDefault(x => x._id == id);
            return View(Sql);
        }

        // POST: Tax/Delete/5
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
                        var Sql = db.Tab_Tax.SingleOrDefault(x => x._id == int.Parse(item));
                        db.Tab_Tax.DeleteOnSubmit(Sql);
                        db.SubmitChanges();
                    }
                }
                else
                {
                    var Sql = db.Tab_Tax.SingleOrDefault(x => x._id == int.Parse(Id));
                    db.Tab_Tax.DeleteOnSubmit(Sql);
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
        public string DownLoad(string Id, string Ip)
        {
            var db = new DB();
            DataTable dt = new DataTable();
            string taxs;
            var result = string.Empty;
            var result1 = string.Empty;
            List<string> id = Id.Split(',').ToList();
            var tax = from t in db.Tab_Tax
                             where id.Contains(t._id.ToString())
                             select new
                             {
                                 t.tax_num,
                                 t.tax_name,
                                 t.tax_value
                             };
            dt = Linq.ToDataTable(tax);
            taxs = json.DataTableToJson(dt);
            if (taxs != "")
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
                            //发送产品
                            string[] textArray1 = new string[] { "http://", Sql.IpAddress, ":", "1235", "/tax" };
                            string uri = string.Concat(textArray1);
                            result1 += HttpHelper.HttpPost(uri, taxs);
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
            }

            return result;
        }
    }
}
