using Candidaturas_BO.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web.Mvc;

namespace Candidaturas_BO.Controllers
{
    public class CandidatosController : Controller
    {
        private CandidaturasBOEntities db = new CandidaturasBOEntities();

        // GET: Candidatos
        public ActionResult Index(string startDate, string endDate, string searchString, string numCand, string sortOrder, string currentFilter, int? page)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                ViewBag.CurrentSort = sortOrder;
                ViewBag.EmailSortParm = String.IsNullOrEmpty(sortOrder) ? "email_desc" : "";
                ViewBag.NumSortParm = sortOrder == "Número" ? "num_desc" : "Número";
                ViewBag.NomeSortParm = sortOrder == "Nome" ? "nome_desc" : "Nome";

                List<CandidatoDisplay> candidatos = new List<CandidatoDisplay>();

                List<Candidato> candDB = db.Candidato.ToList();

                //search
                if (!String.IsNullOrEmpty(searchString))
                {
                    candDB = candDB.Where(s => s.User.DadosPessoais.NomeColoquial.Contains(searchString)).ToList();
                    page = 1;
                }

                //search
                if (!String.IsNullOrEmpty(numCand))
                {
                    candDB = candDB.Where(s => s.Numero.Equals(int.Parse(numCand))).ToList();
                }

                foreach ( Candidato c in candDB)
                {
                    CandidatoDisplay candDisplay = new CandidatoDisplay();

                    candDisplay.userId = c.UserID;

                    candDisplay.numCand = db.Candidato.Where(guy => guy.UserID == c.UserID).Select(guy => guy.Numero).FirstOrDefault();

                    candDisplay.nome = db.DadosPessoais.Where(guy => guy.UserId == c.UserID).Select(guy => guy.NomeColoquial).FirstOrDefault();

                    candDisplay.email = db.User.Where(guy => guy.ID == c.UserID).Select(guy => guy.Email).FirstOrDefault();

                    candDisplay.dataCandidatura = db.Form.Where(guy => guy.UserID == c.UserID).Select(guy => guy.DataCriação).FirstOrDefault();

                    candidatos.Add(candDisplay);
                }

                if (startDate != null || endDate != null)
                {
                    if (startDate != "" && endDate != "")
                    {
                        DateTime? start = Convert.ToDateTime(startDate);
                        DateTime? end = Convert.ToDateTime(endDate);

                        candidatos = candidatos.Where(u => u.dataCandidatura >= start && u.dataCandidatura <= end).ToList();
                    }
                    else if (startDate != "")
                    {
                        DateTime? start = Convert.ToDateTime(startDate);

                        candidatos = candidatos.Where(u => u.dataCandidatura >= start).ToList();
                    }
                    else if (endDate != "")
                    {
                        DateTime? end = Convert.ToDateTime(endDate);

                        candidatos = candidatos.Where(u => u.dataCandidatura <= end).ToList();
                    }
                }

                //sort
                switch (sortOrder)
                {
                    case "email_desc":
                        candidatos = candidatos.OrderByDescending(s => s.email).ToList();
                        break;
                    case "Número":
                        candidatos = candidatos.OrderBy(s => s.numCand).ToList();
                        break;
                    case "num_desc":
                        candidatos = candidatos.OrderByDescending(s => s.numCand).ToList();
                        break;
                    case "Nome":
                        candidatos = candidatos.OrderBy(s => s.nome).ToList();
                        break;
                    case "nome_desc":
                        candidatos = candidatos.OrderByDescending(s => s.nome).ToList();
                        break;
                    default:
                        candidatos = candidatos.OrderBy(s => s.email).ToList();
                        break;
                }

                ViewBag.TotalCandidatos = candidatos.Count();

                //change pageSize to be global variable
                int pageSize = 50;
                int pageNumber = (page ?? 1);

                return View(candidatos.ToPagedList(pageNumber, pageSize));
            }
            else
            {
                return View("Error");
            }
        }

        public void getDataForDropdownLists()
        {
            IEnumerable<SelectListItem> edicoes = db.Edicao.OrderBy(dp => dp.Sigla).Select(c => new SelectListItem
            {
                Value = c.Sigla,
                Text = c.Sigla
            });

            ViewBag.Edicoes = edicoes.ToList();
        }

        // GET: Candidatos/Details/5
        public ActionResult Details(int? id)
        {
            if (ADAuthorization.ADAuthenticate())
            {
                CandidatoDTO candidato = new CandidatoDTO();
                candidato.user = new UserFull();

                candidato.user.email = db.User.Where(guy => guy.ID == id).Select(guy => guy.Email).FirstOrDefault();

                candidato.user.dadosDTO = db.DadosPessoais
                    .Where(guy => guy.UserId == id)
                    .Select(data => new DadosPessoaisDTO
                    {
                        NomeColoquial = data.NomeColoquial,
                        Nomes = data.Nomes,
                        Apelidos = data.Apelidos,
                        NomePai = data.NomePai,
                        NomeMae = data.NomeMae,
                        NDI = data.NDI,
                        TipoDocID = db.TipoDocumentoID.Where(td => td.ID == data.TipoDocID).Select(td => td.Nome).FirstOrDefault(),
                        DocumentoValidade = data.DocumentoValidade,
                        Genero = db.Genero.Where(g => g.ID == data.Genero).Select(g => g.Nome).FirstOrDefault(),
                        EstadoCivil = db.EstadoCivil.Where(ec => ec.ID == data.EstadoCivil).Select(ec => ec.Nome).FirstOrDefault(),
                        Nacionalidade = db.Pais.Where(p => p.Sigla == data.Nacionalidade).Select(p => p.Nome).FirstOrDefault(),
                        DistritoNatural = db.Distrito.Where(d => d.Codigo == data.DistritoNatural).Select(d => d.Nome).FirstOrDefault(),
                        ConcelhoNatural = db.Concelho.Where(c => c.Codigo == data.ConcelhoNatural).Select(c => c.Nome).FirstOrDefault(),
                        FreguesiaNatural = db.Freguesia.Where(f => f.Codigo == data.FreguesiaNatural).Select(f => f.Nome).FirstOrDefault(),
                        Morada = data.Morada,
                        Localidade = db.Localidade.Where(l => l.Codigo == data.Localidade).Select(l => l.Nome).FirstOrDefault(),
                        RepFinNIF = db.Reparticoes.Where(r => r.Codigo == data.RepFinNIF).Select(r => r.Nome).FirstOrDefault(),
                        CCDigitosControlo = data.CCDigitosControlo,
                        NSegSoc = data.NSegSoc,
                        NIF = data.NIF,
                        DistritoMorada = db.Distrito.Where(d => d.Codigo == data.DistritoMorada).Select(d => d.Nome).FirstOrDefault(),
                        ConcelhoMorada = db.Concelho.Where(c => c.Codigo == data.ConcelhoMorada).Select(c => c.Nome).FirstOrDefault(),
                        FreguesiaMorada = db.Freguesia.Where(f => f.Codigo == data.FreguesiaMorada).Select(f => f.Nome).FirstOrDefault(),
                        Telefone = data.Telefone,
                        CodigoPostal4Dig = data.CodigoPostal4Dig,
                        CodigoPostal3Dig = data.CodigoPostal3Dig,
                        DataCriacao = data.DataCriacao,
                        DataUltimaAtualizacao = data.DataUltimaAtualizacao,
                        DataNascimento = data.DataNascimento,
                        Militar = data.Militar,
                        Ramo = db.Ramo.Where(r => r.Sigla == data.Ramo).Select(r => r.Nome).FirstOrDefault(),
                        Categoria = db.Categoria.Where(c => c.Sigla == data.Categoria).Select(c => c.Nome).FirstOrDefault(),
                        Posto = db.Posto.Where(p => p.Código == data.Posto).Select(p => p.Nome).FirstOrDefault(),
                        Classe = data.Classe,
                        NIM = data.NIM
                    })
                    .FirstOrDefault();

                candidato.user.inqueritoDTO = db.Inquerito
                    .Where(guy => guy.UserId == id)
                    .Select(data => new InqueritoDTO
                    {
                        SituacaoPai = db.Situacao.Where(s => s.ID == data.SituacaoPai).Select(s => s.Nome).FirstOrDefault(),
                        OutraPai = data.OutraPai,
                        SituacaoMae = db.Situacao.Where(s => s.ID == data.SituacaoMae).Select(s => s.Nome).FirstOrDefault(),
                        OutraMae = data.OutraMae,
                        ConhecimentoEscola = db.ConhecimentoEscola.Where(ce => ce.ID == data.ConhecimentoEscola).Select(ce => ce.Nome).FirstOrDefault(),
                        CandidatarOutros = data.CandidatarOutros,
                        Outro = data.Outro
                    })
                    .FirstOrDefault();

                candidato.user.cursosDTO = db.UserCurso
                    .Where(guy => guy.UserId == id)
                    .Select(data => new UserCursoDTO
                    {
                        Nome = db.Curso.Where(c => c.ID == data.CursoId).Select(c => c.Nome).FirstOrDefault(),
                        Prioridade = data.Prioridade
                    })
                    .ToList();

                candidato.user.examesDTO = db.UserExame
                    .Where(guy => guy.UserId == id)
                    .Select(data => new UserExameDTO
                    {
                        Codigo = db.Exame.Where(e => e.ID == data.ExameId).Select(e => e.Código).FirstOrDefault(),
                        Exame = db.Exame.Where(e => e.ID == data.ExameId).Select(e => e.Nome).FirstOrDefault()
                    })
                    .ToList();

                candidato.user.docsDTO = db.Documento
                    .Where(guy => guy.UserID == id)
                    .Select(data => new DocsDTO
                    {
                        Nome = data.Nome,
                        Descricao = data.Descricao,
                        Tipo = data.Tipo,
                        DocumentoBinario = db.DocumentoBinario.Where(doc => doc.DocID == data.ID).FirstOrDefault()
                    })
                    .ToList();

                candidato.candidato = db.Candidato
                    .Where(guy => guy.UserID == id)
                    .FirstOrDefault();

                candidato.form = db.Form
                    .Where(guy => guy.UserID == id)
                    .FirstOrDefault();

                ViewData["mil"] = candidato.user.dadosDTO.Militar;

                return View(candidato);
            }
            else
            {
                return View("Error");
            }
        }

        public ActionResult DownloadFormulario(int id)
        {
            CandidaturasBOEntities db = new CandidaturasBOEntities();
            byte[] dForm = db.Form.Where(dp => dp.UserID == id).Select(dp => dp.FormBin).FirstOrDefault();

            Response.Clear();
            Response.Buffer = true;
            Response.Charset = "";
            Response.Cache.SetCacheability(System.Web.HttpCacheability.Private);
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + "ComprovativoCandidatura.pdf");
            Response.BinaryWrite(dForm);
            Response.Flush();
            Response.End();

            return View("~/Views/Home/Welcome.cshtml");
        }
    }
}