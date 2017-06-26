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
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index(string Username)
        {
            var db = new DB();
            var sql = from t in db.Tab_User select t;
            if (!string.IsNullOrEmpty(Username))
            {
                sql = sql.Where(s => s.user_name.Contains(Username));
            }
            return View(sql);
        }


        // GET: User/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: User/Create
        [Authorize]
        public ActionResult Create()
        {
            var db = new DB();
            var maxusernum = (from t in db.Tab_User select t.user_num).Max();
            if (maxusernum != null)
            {
                ViewBag.Message = int.Parse(maxusernum) + 1;
            }
            else
            {
                ViewBag.Message = 1;
            }
            return View();
        }

        // POST: User/Create
        [HttpPost]
        public ActionResult Create(Tab_User Sql)
        {
            if (ModelState.IsValid)
            {
                try
                {    
                    // TODO: Add insert logic here
                    var db = new DB();
                    var tab_user = new Tab_User
                    {
                        user_num=Sql.user_num,
                        user_name=Sql.user_name,
                        user_password=md5.MD5Encrypt(Sql.user_password),
                        user_permission=Sql.user_permission,
                        phone=Sql.phone,
                        address=Sql.address
                    };

                    db.Tab_User.InsertOnSubmit(tab_user);
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

        // GET: User/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            var db = new DB();
            var Sql = db.Tab_User.SingleOrDefault(x => x._id == id);
            return View(Sql);
        }

        // POST: User/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                var db = new DB();
                var Sql = db.Tab_User.SingleOrDefault(x => x._id == id);
                Sql.user_password = md5.MD5Encrypt(Sql.user_password);
                UpdateModel(Sql, collection.ToValueProvider());
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: User/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            var db = new DB();
            var Sql = db.Tab_User.SingleOrDefault(x => x._id == id);
            return View(Sql);
        }

        // POST: User/Delete/5
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
                        var Sql = db.Tab_User.SingleOrDefault(x => x._id == int.Parse(item));
                        db.Tab_User.DeleteOnSubmit(Sql);
                        db.SubmitChanges();
                    }
                }
                else
                {
                    var Sql = db.Tab_User.SingleOrDefault(x => x._id == int.Parse(Id));
                    db.Tab_User.DeleteOnSubmit(Sql);
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
            string users;
            var result = string.Empty;
            var result1 = string.Empty;
            List<string> id = Id.Split(',').ToList();
            var user = from t in db.Tab_User
                           where id.Contains(t._id.ToString())
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
            if (users != "")
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
                            string[] textArray1 = new string[] { "http://", Sql.IpAddress, ":", "1235", "/user" };
                            string uri = string.Concat(textArray1);
                            result1 += HttpHelper.HttpPost(uri, users);
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
