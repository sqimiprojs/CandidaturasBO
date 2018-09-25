using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Candidaturas_BO.Models;

namespace Candidaturas_BO.Controllers
{
    public class FreguesiasController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Freguesias
        public ActionResult Index(string searchString)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                List<Freguesia> freguesias = db.Freguesia.ToList();

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    freguesias = freguesias.Where(s => s.Nome.Contains(searchString) || s.Concelho.Contains(searchString)).ToList();
                }

                return View(freguesias);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Freguesias/Details/5
        public ActionResult Details(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Freguesia freguesia = db.Freguesia.Find(id);

                if (freguesia == null)
                {
                    return HttpNotFound();
                }

                return View(freguesia);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Freguesias/Create
        public ActionResult Create()
        {
            if (ADAuthorization.ADAuthenticate())
            {
                IEnumerable<SelectListItem> concelhos = db.Concelho.OrderBy(dp => dp.Nome).Select(c => new SelectListItem
                {
                    Value = c.Nome,
                    Text = c.Nome
                });

                ViewBag.Concelho = concelhos.ToList();

                return View();
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Freguesias/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nome,Concelho")] Freguesia freguesia)
        {
            if (ModelState.IsValid)
            {
                db.Freguesia.Add(freguesia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(freguesia);
        }

        // GET: Freguesias/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Freguesia freguesia = db.Freguesia.Find(id);
                if (freguesia == null)
                {
                    return HttpNotFound();
                }

                IEnumerable<SelectListItem> concelhos = db.Concelho.OrderBy(dp => dp.Nome).Select(c => new SelectListItem
                {
                    Value = c.Nome,
                    Text = c.Nome,
                    Selected = c.Nome == freguesia.Concelho
                });

                ViewBag.Concelho = concelhos.ToList();

                return View(freguesia);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Freguesias/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nome,Concelho")] Freguesia freguesia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(freguesia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(freguesia);
        }

        // GET: Freguesias/Delete/5
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Freguesia freguesia = db.Freguesia.Find(id);

                if (freguesia == null)
                {
                    return HttpNotFound();
                }

                return View(freguesia);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Freguesias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Freguesia freguesia = db.Freguesia.Find(id);
            db.Freguesia.Remove(freguesia);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
