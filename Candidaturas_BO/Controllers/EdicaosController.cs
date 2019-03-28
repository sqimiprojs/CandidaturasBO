using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Candidaturas_BO.Models;
using OfficeOpenXml;

namespace Candidaturas_BO.Controllers
{
    public class EdicaosController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Edicaos
        public ActionResult Index(string searchString, string sortOrder)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.UserAdmin = ADAuthorization.ADAuthenticateAdmin();
                ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

                List<Edicao> edicao = db.Edicao.ToList();

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    edicao = edicao.Where(s => s.Sigla.ToLower().Contains(searchString) || s.Sigla.Contains(searchString)).ToList();
                }

                //sort
                switch (sortOrder)
                {
                    case "name_desc":
                        edicao = edicao.OrderByDescending(s => s.Sigla).ToList();
                        break;
                    default:
                        edicao = edicao.OrderBy(s => s.Sigla).ToList();
                        break;
                }

                ViewBag.TotalEdicao = edicao.Count();

                return View(edicao);
            }
            else
            {
                return View("Error");
            }

        }

        // GET: Edicaos/Create
        public ActionResult Create()
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                return View();
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Edicaos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Sigla,AnoLectivo,DataInicio,DataFim")] Edicao edicao)
        {
            if (ModelState.IsValid)
            {
                Edicao ultimaEdicao = db.Edicao.OrderByDescending(e => e.DataInicio).First();

                db.Edicao.Add(edicao);
                db.SaveChanges();
                Edicao novaEdicao = db.Edicao.OrderByDescending(e => e.DataInicio).First();

               /* List<User> users = db.User.Where(u => u.Edicao == ultimaEdicao.Sigla).ToList();
                foreach (User user in users)
                {
                    User novoUser = new User();
                    novoUser.Password = user.Password;
                    novoUser.Email = user.Email;
                    novoUser.DataCriacao = user.DataCriacao;
                    novoUser.NomeColoquial = user.NomeColoquial;
                    novoUser.DataNascimento = user.DataNascimento;
                    novoUser.TipoDocID = user.TipoDocID;
                    novoUser.NDI = user.NDI;
                    novoUser.DocumentoValidade = user.DocumentoValidade;
                    novoUser.Militar = user.Militar;
                    novoUser.Ramo = user.Ramo;
                    novoUser.Categoria = user.Categoria;
                    novoUser.Posto = user.Posto;
                    novoUser.Classe = user.Classe;
                    novoUser.NIM = user.NIM;
                    novoUser.Edicao = novaEdicao.Sigla;
                    db.User.Add(novoUser);
                }*/

                List<Situacao> situacoes = db.Situacao.Where(u => u.Edicao == ultimaEdicao.Sigla).ToList();
                foreach (Situacao situacao in situacoes)
                {
                    Situacao novaSituacao = new Situacao();
                    novaSituacao.Nome = situacao.Nome;
                    novaSituacao.Edicao = novaEdicao.Sigla;
                    db.Situacao.Add(novaSituacao);
                }

                List<ConhecimentoEscola> conhecimentos = db.ConhecimentoEscola.Where(u => u.Edicao == ultimaEdicao.Sigla).ToList();
                foreach (ConhecimentoEscola conhecimento in conhecimentos)
                {
                    ConhecimentoEscola novoConhecimento = new ConhecimentoEscola();
                    novoConhecimento.Nome = conhecimento.Nome;
                    novoConhecimento.Edicao = novaEdicao.Sigla;
                    db.ConhecimentoEscola.Add(novoConhecimento);
                }

                List<Curso> cursos = db.Curso.Where(u => u.Edicao == ultimaEdicao.Sigla).ToList();
                foreach (Curso curso in cursos)
                {
                    Curso novoCurso = new Curso();
                    novoCurso.Nome = curso.Nome;
                    novoCurso.Edicao = novaEdicao.Sigla;
                    db.Curso.Add(novoCurso);
                }

                List<Exame> exames = db.Exame.Where(u => u.Edicao == ultimaEdicao.Sigla).ToList();
                foreach (Exame exame in exames)
                {
                    Exame novoExame = new Exame();
                    novoExame.Nome = exame.Nome;
                    novoExame.Código = exame.Código;
                    novoExame.Edicao = novaEdicao.Sigla;
                    db.Exame.Add(novoExame);
                }

                List<CursoExame> cursosExames = db.CursoExame.Where(u => u.Edicao == ultimaEdicao.Sigla).ToList();
                foreach (CursoExame cursoexame in cursosExames)
                {
                    CursoExame novoCursoExame = new CursoExame();
                    novoCursoExame.CursoID = cursoexame.CursoID;
                    novoCursoExame.ExameID = cursoexame.ExameID;
                    novoCursoExame.Edicao = novaEdicao.Sigla;
                    db.CursoExame.Add(novoCursoExame);
                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(edicao);
        }


        // GET: Edicaos/Edit/5
        public ActionResult Edit(string id)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Edicao edicao = db.Edicao.Find(id);
            if (edicao == null)
            {
                return HttpNotFound();
            }
            return View(edicao);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Edicaos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Sigla,AnoLectivo,DataInicio,DataFim")] Edicao edicao)
        {
            if (ModelState.IsValid)
            {
                db.Entry(edicao).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(edicao);
        }

        // GET: Edicaos/Delete/5
        public ActionResult Delete(string id)
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Edicao edicao = db.Edicao.Find(id);
                if (edicao == null)
                {
                    return HttpNotFound();
                }
                return View(edicao);
            }
            else
            {
                return View("Error");
            }
        }

        // POST: Edicaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Edicao edicao = db.Edicao.Find(id);
            db.Edicao.Remove(edicao);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //GET: ConhecimentoEscolas/MassInsert
        public ActionResult MassInsert()
        {
            if (ADAuthorization.ADAuthenticateAdmin())
            {
                return View();
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult MassInsert(HttpPostedFileBase uploadFile)
        {
            if (uploadFile != null && uploadFile.ContentLength > 0)
            {
                if (ExcelValidator.HasExcelExtension(uploadFile) && ExcelValidator.HasExcelMimeType(uploadFile))
                {
                    string fileName = uploadFile.FileName;
                    string fileContentType = uploadFile.ContentType;
                    byte[] fileBytes = new byte[uploadFile.ContentLength];
                    var data = uploadFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(uploadFile.ContentLength));

                    try
                    {
                        var package = new ExcelPackage(uploadFile.InputStream);
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;

                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var sigla = workSheet.Cells[rowIterator, 1].Value.ToString();
                            var anoletivo = workSheet.Cells[rowIterator, 2].Value.ToString();
                            var datainicio = Convert.ToDateTime(workSheet.Cells[rowIterator, 3].Value.ToString());
                            var datafim = Convert.ToDateTime(workSheet.Cells[rowIterator, 4].Value.ToString());


                            if (!db.Edicao.Any(ce => ce.Sigla == sigla))
                            {
                                Edicao edicao = new Edicao();
                                edicao.Sigla = sigla;
                                edicao.AnoLectivo = anoletivo;
                                edicao.DataInicio = datainicio;
                                edicao.DataFim = datafim;
                                db.Edicao.Add(edicao);
                                db.SaveChanges();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        ViewBag.ErrorMessage = "Tipo de ficheiro inválido. Por favor seleccione um ficheiro Excel válido.";
                        return View();
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Tipo de ficheiro inválido. Por favor seleccione um ficheiro Excel válido.";
                    return View();
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Não foi seleccionado nenhum ficheiro. Por favor seleccione um ficheiro Excel válido.";
                return View();
            }
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
