using DATA;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Web.Mvc;

namespace T_Touch_Central_Web.Controllers
{
    public class DownloadController : Controller
    {
        // GET: Download
        public ActionResult Index()
        {
            return View();
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
            List<string> id = Id.Split(',').ToList();
            var result = string.Empty;
            var result1 = string.Empty;

            //用户
            string users;
            var user = from t in db.Tab_User
                       select new
                       {
                           t.user_num,
                           t.user_name,
                           t.user_password,
                           t.user_permission,
                           t.phone,
                           t.address
                       };
            dt = Linq.ToDataTable(user);
            users = json.DataTableToJson(dt);

            //部门
            string departments;
            var department = from t in db.Tab_Department
                             select new
                             {
                                 t.department_num,
                                 t.department_name,
                                 t.remark
                             };
            dt = Linq.ToDataTable(department);
            departments = json.DataTableToJson(dt);

            //分类
            string categorys;
            var category = from t in db.Category
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

            //商品
            string products;
            var product = from t in db.Product
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

            //税率
            string taxs;
            var tax = from t in db.Tab_Tax
                      select new
                      {
                          t.tax_num,
                          t.tax_name,
                          t.tax_value,
                          t.tax_type
                      };
            dt = Linq.ToDataTable(tax);
            taxs = json.DataTableToJson(dt);

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
                        for (int i = 0; i < id.Count; i++)
                        {
                            switch (id[i])
                            {
                                case "1": //发送用户
                                    if (users != "")
                                    {
                                        string[] textArray1 = new string[] { "http://", Sql.IpAddress, ":", "1235", "/user" };
                                        string uri = string.Concat(textArray1);
                                        result1 += HttpHelper.HttpPost(uri, users);
                                        if (result1.Contains("OK"))
                                        {
                                            result += Sql.IpAddress + " tab_user " + ":下载成功！" + Environment.NewLine;
                                        }
                                        else
                                        {
                                            result += Sql.IpAddress + " tab_user " + ":下载失败！" + Environment.NewLine;
                                        }
                                    }
                                    break;
                                case "2": //发送部门
                                    if (departments != "")
                                    {
                                        string[] textArray1 = new string[] { "http://", Sql.IpAddress, ":", "1235", "/department" };
                                        string uri = string.Concat(textArray1);
                                        result1 += HttpHelper.HttpPost(uri, departments);
                                        if (result1.Contains("OK"))
                                        {
                                            result += Sql.IpAddress + " tab_department " + ":下载成功！" + Environment.NewLine;
                                        }
                                        else
                                        {
                                            result += Sql.IpAddress + " tab_department " + ":下载失败！" + Environment.NewLine;
                                        }
                                    }
                                    break;
                                case "3"://发送分类
                                    if (categorys != "")
                                    {
                                        string[] textArray1 = new string[] { "http://", Sql.IpAddress, ":", "1235", "/category" };
                                        string uri = string.Concat(textArray1);
                                        result1 += HttpHelper.HttpPost(uri, categorys);
                                        if (result1.Contains("OK"))
                                        {
                                            result += Sql.IpAddress + " tab_category_a " + ":下载成功！" + Environment.NewLine;
                                        }
                                        else
                                        {
                                            result += Sql.IpAddress + " tab_category_a " + ":下载失败！" + Environment.NewLine;
                                        }
                                    }
                                    break;
                                case "4"://发送商品
                                    if (products != "")
                                    {
                                        string[] textArray1 = new string[] { "http://", Sql.IpAddress, ":", "1235", "/products" };
                                        string uri = string.Concat(textArray1);
                                        result1 += HttpHelper.HttpPost(uri, products);
                                        if (result1.Contains("OK"))
                                        {
                                            result += Sql.IpAddress + " tab_product " + ":下载成功！" + Environment.NewLine;
                                        }
                                        else
                                        {
                                            result += Sql.IpAddress + " tab_product " + ":下载失败！" + Environment.NewLine;
                                        }
                                    }
                                    break;
                                case "5"://发送税率
                                    if (taxs != "")
                                    {
                                        string[] textArray1 = new string[] { "http://", Sql.IpAddress, ":", "1235", "/tax" };
                                        string uri = string.Concat(textArray1);
                                        result1 += HttpHelper.HttpPost(uri, taxs);
                                        if (result1.Contains("OK"))
                                        {
                                            result += Sql.IpAddress + " tab_tax " + ":下载成功！" + Environment.NewLine;
                                        }
                                        else
                                        {
                                            result += Sql.IpAddress + " tab_tax " + ":下载失败！" + Environment.NewLine;
                                        }
                                    }
                                    break;
                            }
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
                    continue;
                }
            }
            return result;
        }
    }
}