using DATA;
using DATA.model;
using System.Linq;
using System.Web.Mvc;

namespace T_Touch_Central_Web.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index(string CategoryName)
        {
            var db = new DB();
            var sql = from t in db.Category   select t;
            if (!string.IsNullOrEmpty(CategoryName))
            {
                sql = sql.Where(s => s.category_name.Contains(CategoryName));
            }
            return View(sql);
        }

        // GET: Category/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Category/Create
        public ActionResult Create()
        {
            return View();
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
                    var db = new DB();
                    db.Category.InsertOnSubmit(Sql);
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
                // TODO: Add update logic here

                var db = new DB();
                var Sql = db.Category.SingleOrDefault(x => x.Id == id);
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
    }
}
