﻿using System;
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
    public class ConcelhosController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Concelhos
        public ActionResult Index(string distrito)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                List<Concelho> concelhos = db.Concelho.ToList();

                //search
                if (!String.IsNullOrEmpty(distrito))
                {
                    concelhos = concelhos.Where(s => s.Distrito == distrito).ToList();
                }

                IEnumerable<SelectListItem> distritos = db.Distrito.OrderBy(dp => dp.Nome).Select(c => new SelectListItem
                {
                    Value = c.Nome,
                    Text = c.Nome
                });

                ViewBag.Distrito = distritos.ToList();

                return View(concelhos);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Concelhos/Details/5
        public ActionResult Details(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Concelho concelho = db.Concelho.Find(id);
                if (concelho == null)
                {
                    return HttpNotFound();
                }

                return View(concelho);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Concelhos/Create
        public ActionResult Create()
        {
            if (ADAuthorization.ADAuthenticate())
            {
                IEnumerable<SelectListItem> distritos = db.Distrito.OrderBy(dp => dp.Nome).Select(c => new SelectListItem
                {
                    Value = c.Nome,
                    Text = c.Nome
                });

                ViewBag.Distrito = distritos.ToList();

                return View();
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Concelhos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nome,Distrito")] Concelho concelho)
        {
            if (ModelState.IsValid)
            {
                db.Concelho.Add(concelho);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(concelho);
        }

        // GET: Concelhos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Concelho concelho = db.Concelho.Find(id);
                if (concelho == null)
                {
                    return HttpNotFound();
                }

                IEnumerable<SelectListItem> distritos = db.Distrito.OrderBy(dp => dp.Nome).Select(c => new SelectListItem
                {
                    Value = c.Nome,
                    Text = c.Nome,
                    Selected = c.Nome == concelho.Distrito
                });

                ViewBag.Distrito = distritos.ToList();

                return View(concelho);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Concelhos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nome,Distrito")] Concelho concelho)
        {
            if (ModelState.IsValid)
            {
                db.Entry(concelho).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(concelho);
        }

        // GET: Concelhos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Concelho concelho = db.Concelho.Find(id);
                if (concelho == null)
                {
                    return HttpNotFound();
                }

                return View(concelho);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Concelhos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Concelho concelho = db.Concelho.Find(id);
            db.Concelho.Remove(concelho);
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
