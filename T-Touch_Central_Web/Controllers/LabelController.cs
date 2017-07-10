using DATA;
using System.IO;
using System.Linq;
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
                        if (System.IO.File.Exists(path+"\\"+item))
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
            var result = string.Empty;
            return result;
        }
    }
}