using DATA;
using DATA.model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Web.Mvc;

namespace T_Touch_Central_Web.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index(string ProductNumber)
        {
            var db = new DB();
            var sql = from t in db.Product select t;
            if (!string.IsNullOrEmpty(ProductNumber))
            {
                sql = sql.Where(s => s.product_num.Contains(ProductNumber));
            }
            return View(sql);
        }

        // GET: Product/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        public ActionResult Create(Product Sql)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    var db = new DB();
                    db.Product.InsertOnSubmit(Sql);
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

        // GET: Product/Edit/5
        public ActionResult Edit(int id)
        {
            var db = new DB();
            var Sql = db.Product.SingleOrDefault(x => x.Id == id);
            return View(Sql);
        }

        // POST: Product/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                var db = new DB();
                var Sql = db.Product.SingleOrDefault(x => x.Id == id);
                UpdateModel(Sql, collection.ToValueProvider());
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int id)
        {
            var db = new DB();
            var Sql = db.Product.SingleOrDefault(x => x.Id == id);
            return View(Sql);
        }

        // POST: Product/Delete/5
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
                        var Sql = db.Product.SingleOrDefault(x => x.Id == int.Parse(item));
                        db.Product.DeleteOnSubmit(Sql);
                        db.SubmitChanges();
                    }
                }
                else
                {
                    var Sql = db.Product.SingleOrDefault(x => x.Id == int.Parse(Id));
                    db.Product.DeleteOnSubmit(Sql);
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
        public string DownLoad(string Id,string Ip)
        {
            var db = new DB();
            DataTable dt = new DataTable();
            string products;
            var result = string.Empty;
            var result1 = string.Empty;
            List<string> id = Id.Split(',').ToList();
             var product = from t in db.Product
                          where id.Contains(t.Id.ToString())  
                          select new
                          {
                              product_number = t.product_num,
                              product_code = t.barcode,
                              t.product_name,
                              product_abbr = t.abbr,
                              original_price = t.price,
                              sales_price = t.price_lowest
                          };

            dt = Linq.ToDataTable(product);
            products = json.DataTableToJson(dt);
            if (products != "")
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
                                string[] textArray1 = new string[] { "http://", Sql.IpAddress, ":", "1235", "/products" };
                                string uri = string.Concat(textArray1);
                                result1 += HttpHelper.HttpPost(uri, products);
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
