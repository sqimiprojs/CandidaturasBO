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
    public class CursosController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Cursos
        public ActionResult Index(string searchString)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                List<Curso> cursos = db.Curso.ToList();

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    cursos = cursos.Where(s => s.Nome.Contains(searchString) || s.CodigoCurso.Contains(searchString) || s.CodigoRamo.Contains(searchString)).ToList();
                }

                return View(cursos);
            }
            else
            {
                return View("Error");
            }

            
        }

        // GET: Cursos/Details/5
        public ActionResult Details(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Curso curso = db.Curso.Find(id);
                if (curso == null)
                {
                    return HttpNotFound();
                }
                return View(curso);
            }
            else
            {
                return View("Error");
            }
        }

        // GET: Cursos/Create
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

        // POST: Cursos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Nome,CodigoCurso,CodigoRamo")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                db.Curso.Add(curso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(curso);
        }

        // GET: Cursos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Curso curso = db.Curso.Find(id);
                if (curso == null)
                {
                    return HttpNotFound();
                }
                return View(curso);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Cursos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Nome,CodigoCurso,CodigoRamo")] Curso curso)
        {
            if (ModelState.IsValid)
            {
                db.Entry(curso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(curso);
        }

        // GET: Cursos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Curso curso = db.Curso.Find(id);
                if (curso == null)
                {
                    return HttpNotFound();
                }
                return View(curso);
            }
            else
            {
                return View("Error");
            }            
        }

        // POST: Cursos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Curso curso = db.Curso.Find(id);
            db.Curso.Remove(curso);
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
