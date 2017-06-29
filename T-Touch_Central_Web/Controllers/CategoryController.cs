using DATA;
using DATA.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Web;
using System.Web.Mvc;
using PagedList;
namespace T_Touch_Central_Web.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index(int? page,string CategoryName)
        {
            var db = new DB();
            var sql = from t in db.Category   select t;
            if (!string.IsNullOrEmpty(CategoryName))
            {
                sql = sql.Where(s => s.category_name.Contains(CategoryName));
            }
            return View(sql.OrderBy(s=>s.category_num).ToPagedList(page??1,8));
        }

        // GET: Category/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Category/Create
        [Authorize]
        public ActionResult Create()
        {
            var db = new DB();
            var maxcategorynum = (from t in db.Category select t.category_num).Max();
            if (maxcategorynum != null)
            {
                ViewBag.Message = int.Parse(maxcategorynum) + 1;
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
        // POST: Category/Create
        [HttpPost]
        public ActionResult Create(Category Sql)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here

                    string Images = string.Empty;
                    Images = ToBase64(Request.Files["images"]);
                    var db = new DB();
                    var category = new Category
                    {
                        category_num = Sql.category_num,
                        category_name = Sql.category_name,
                        describ = Sql.describ,
                        order_index = Sql.order_index,
                        image = Sql.category_num+".png",
                        images = Images,
                    };

                    db.Category.InsertOnSubmit(category);
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

        // GET: Category/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            var db = new DB();
            var Sql = db.Category.SingleOrDefault(x => x.Id == id);
            return View(Sql);
        }

        // POST: Category/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                string Images = string.Empty;
                Images = ToBase64(Request.Files["images"]);

                var db = new DB();
                var Sql = db.Category.SingleOrDefault(x => x.Id == id);
                if (Images != string.Empty)
                {
                    Sql.image = Sql.category_num + ".png";
                    Sql.images = Images;
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

        // GET: Category/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            var db = new DB();
            var Sql = db.Category.SingleOrDefault(x => x.Id == id);
            return View(Sql);
        }

        // POST: Category/Delete/5
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
                        var Sql = db.Category.SingleOrDefault(x => x.Id == int.Parse(item));
                        db.Category.DeleteOnSubmit(Sql);
                        db.SubmitChanges();
                    }
                }
                else
                {
                    var Sql = db.Category.SingleOrDefault(x => x.Id == int.Parse(Id));
                    db.Category.DeleteOnSubmit(Sql);
                    db.SubmitChanges();
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
            DataTable dt = new DataTable();
            string categorys;
            var result = string.Empty;
            var result1 = string.Empty;
            List<string> id = Id.Split(',').ToList();
            var category = from t in db.Category
                          where id.Contains(t.Id.ToString())
                          select new
                          {
                              t.category_num,
                              t.category_name,
                              t.describ,
                              t.order_index,
                              t.image
                          };
            dt = Linq.ToDataTable(category);
            categorys = json.DataTableToJson(dt);
            if (categorys != "")
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
                            string[] textArray1 = new string[] { "http://", Sql.IpAddress, ":", "1235", "/category" };
                            string uri = string.Concat(textArray1);
                            result1 += HttpHelper.HttpPost(uri, categorys);
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

