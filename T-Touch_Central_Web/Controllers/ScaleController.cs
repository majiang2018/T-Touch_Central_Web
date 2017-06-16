using DATA;
using DATA.model;
using System.Linq;
using System.Web.Mvc;

namespace T_Touch_Central_Web.Controllers
{
    public class ScaleController : Controller
    {
        // GET: Scale
        public ActionResult Index(string ScaleNo)
        {
            var db = new DB();
            var sql = from t in db.Scales select t; ;
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
            return View();
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
                    var db = new DB();
                    db.Scales.InsertOnSubmit(Sql);
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

                var db = new DB();
                var Sql = db.Scales.SingleOrDefault(x => x.Id == id);
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
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                var db = new DB();
                var Sql = db.Scales.SingleOrDefault(x => x.Id == id);
                db.Scales.DeleteOnSubmit(Sql);
                db.SubmitChanges();
                return RedirectToAction("Index");

            }
            catch
            {
                return View();
            }
        }
    }
}
