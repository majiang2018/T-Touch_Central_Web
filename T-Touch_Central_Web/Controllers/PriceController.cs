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
    public class PriceController : Controller
    {
        // GET: Price
        public ActionResult Index(int? page, string ProductNumber)
        {
            var db = new DB();
            var sql = from t in db.Product select t;
            if (!string.IsNullOrEmpty(ProductNumber))
            {
                sql = sql.Where(s => s.product_num.Contains(ProductNumber));
            }
            return View(sql.OrderBy(s => s.product_num).ToPagedList(page ?? 1, 8));
        }

        // GET: Price/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Price/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Price/Edit/5
        [HttpPost]
        public ActionResult Edit(string Id,string Price, string PriceLowest)
        {
            try
            {
                // TODO: Add update logic here
                var db = new DB();

                if (Id.Contains(","))
                {
                    string[]  id= Id.Split(',').ToArray();
                    string[]  price = Price.Split(',').ToArray();
                    string[]  pricelowest = PriceLowest.Split(',').ToArray();

                    for (int i = 0; i < id.Count(); i++)
                    {
                        Product product = db.Product.First(p => p.Id == int.Parse(id[i]));
                        product.price = price[i];
                        product.price_lowest = pricelowest[i];
                        db.SubmitChanges();
                    }
                }
                else
                {
                    Product product = db.Product.First(p => p.Id == int.Parse(Id));
                    product.price = Price;
                    product.price_lowest = PriceLowest;
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
