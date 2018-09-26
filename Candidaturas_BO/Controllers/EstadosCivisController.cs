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
    public class EstadosCivisController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: EstadosCivis
        public ActionResult Index(string searchString)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                List<EstadoCivil> estadosCivis = db.EstadoCivil.ToList();

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    estadosCivis = estadosCivis.Where(s => s.Nome.Contains(searchString) || s.Nome.ToLower().Contains(searchString)).ToList();
                }

                return View(estadosCivis);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: EstadosCivis/Details/5
        public ActionResult Details(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                EstadoCivil estadoCivil = db.EstadoCivil.Find(id);
                if (estadoCivil == null)
                {
                    return HttpNotFound();
                }
                return View(estadoCivil);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: EstadosCivis/Create
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

        // POST: EstadosCivis/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nome")] EstadoCivil estadoCivil)
        {
            if (ModelState.IsValid)
            {
                db.EstadoCivil.Add(estadoCivil);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(estadoCivil);
        }

        // GET: EstadosCivis/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                EstadoCivil estadoCivil = db.EstadoCivil.Find(id);
                if (estadoCivil == null)
                {
                    return HttpNotFound();
                }
                return View(estadoCivil);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: EstadosCivis/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nome")] EstadoCivil estadoCivil)
        {
            if (ModelState.IsValid)
            {
                db.Entry(estadoCivil).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(estadoCivil);
        }

        // GET: EstadosCivis/Delete/5
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                EstadoCivil estadoCivil = db.EstadoCivil.Find(id);
                if (estadoCivil == null)
                {
                    return HttpNotFound();
                }
                return View(estadoCivil);
            }
            else
            {
                return View("Error");
            }        
        }

        // POST: EstadosCivis/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EstadoCivil estadoCivil = db.EstadoCivil.Find(id);
            db.EstadoCivil.Remove(estadoCivil);
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
