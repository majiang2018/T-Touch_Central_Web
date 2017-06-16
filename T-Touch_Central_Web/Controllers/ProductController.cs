﻿using DATA;
using DATA.model;
using System.Linq;
using System.Web.Mvc;

namespace T_Touch_Central_Web.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        public ActionResult Index()
        {
            var db = new DB();
            var sql = from t in db.Product orderby t.product_num select t ;
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
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
                var db = new DB();
                var Sql = db.Product.SingleOrDefault(x => x.Id == id);
                db.Product.DeleteOnSubmit(Sql);
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