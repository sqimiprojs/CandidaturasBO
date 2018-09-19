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
    public class NacionalidadesController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Nacionalidades
        public ActionResult Index(string searchString)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                List<Nacionalidade> nacionalidades = db.Nacionalidade.ToList();

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    nacionalidades = nacionalidades.Where(s => s.Nome.Contains(searchString)).ToList();
                }

                return View(nacionalidades);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Nacionalidades/Details/5
        public ActionResult Details(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Nacionalidade nacionalidade = db.Nacionalidade.Find(id);

                if (nacionalidade == null)
                {
                    return HttpNotFound();
                }

                return View(nacionalidade);
            }
            else
            {
                return View("Error");
            }
            
        }

        // GET: Nacionalidades/Create
        public ActionResult Create()
        {
            if (ADAuthorization.ADAuthenticate())
            {
                return View();
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Nacionalidades/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nome")] Nacionalidade nacionalidade)
        {
            if (ModelState.IsValid)
            {
                db.Nacionalidade.Add(nacionalidade);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(nacionalidade);
        }

        // GET: Nacionalidades/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Nacionalidade nacionalidade = db.Nacionalidade.Find(id);

                if (nacionalidade == null)
                {
                    return HttpNotFound();
                }

                return View(nacionalidade);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Nacionalidades/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nome")] Nacionalidade nacionalidade)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nacionalidade).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nacionalidade);
        }

        // GET: Nacionalidades/Delete/5
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Nacionalidade nacionalidade = db.Nacionalidade.Find(id);

                if (nacionalidade == null)
                {
                    return HttpNotFound();
                }

                return View(nacionalidade);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Nacionalidades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Nacionalidade nacionalidade = db.Nacionalidade.Find(id);
            db.Nacionalidade.Remove(nacionalidade);
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
