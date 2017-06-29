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
    public class DepartmentController : Controller
    {
        // GET: Department
        public ActionResult Index(int? page, string DepartmentName)
        {
            var db = new DB();
            var sql = from t in db.Tab_Department select t;
            if (!string.IsNullOrEmpty(DepartmentName))
            {
                sql = sql.Where(s => s.department_name.Contains(DepartmentName));
            }
            return View(sql.OrderBy(s => s.department_num).ToPagedList(page ?? 1, 8));
        }

        // GET: Department/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Department/Create
        [Authorize]
        public ActionResult Create()
        {
            var db = new DB();
            var maxdepartmentnum = (from t in db.Tab_Department select t.department_num).Max();
            if (maxdepartmentnum != null)
            {
                ViewBag.Message = int.Parse(maxdepartmentnum) + 1;
            }
            else
            {
                ViewBag.Message = 1;
            }
            return View();
        }

        // POST: Department/Create
        [HttpPost]
        public ActionResult Create(Tab_Department Sql)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // TODO: Add insert logic here
                    var db = new DB();
                    db.Tab_Department.InsertOnSubmit(Sql);
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

        // GET: Department/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            var db = new DB();
            var Sql = db.Tab_Department.SingleOrDefault(x => x._id == id);
            return View(Sql);
        }

        // POST: Department/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
                var db = new DB();
                var Sql = db.Tab_Department.SingleOrDefault(x => x._id == id);
                UpdateModel(Sql, collection.ToValueProvider());
                db.SubmitChanges();
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Department/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            var db = new DB();
            var Sql = db.Tab_Department.SingleOrDefault(x => x._id == id);
            return View(Sql);
        }

        // POST: Department/Delete/5
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
                        var Sql = db.Tab_Department.SingleOrDefault(x => x._id == int.Parse(item));
                        db.Tab_Department.DeleteOnSubmit(Sql);
                        db.SubmitChanges();
                    }
                }
                else
                {
                    var Sql = db.Tab_Department.SingleOrDefault(x => x._id == int.Parse(Id));
                    db.Tab_Department.DeleteOnSubmit(Sql);
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
            string departments;
            var result = string.Empty;
            var result1 = string.Empty;
            List<string> id = Id.Split(',').ToList();
            var department = from t in db.Tab_Department
                           where id.Contains(t._id.ToString())
                           select new
                           {
                               t.department_num,
                               t.department_name,
                               t.remark
                           };
            dt = Linq.ToDataTable(department);
            departments = json.DataTableToJson(dt);
            if (departments != "")
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
                            string[] textArray1 = new string[] { "http://", Sql.IpAddress, ":", "1235", "/department" };
                            string uri = string.Concat(textArray1);
                            result1 += HttpHelper.HttpPost(uri, departments);
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
