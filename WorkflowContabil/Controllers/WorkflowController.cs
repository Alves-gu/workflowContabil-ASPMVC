using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NvtecUploadFiles.Models;

namespace NvtecUploadFiles.Controllers
{
    public class WorkflowController : Controller
    {
        databaseEntities ctx = new databaseEntities();
        // GET: Workflow
        public ActionResult Index()
        {
            var usuarioLogado = HttpContext.Session["usuario"];

            if(usuarioLogado != null)
            {
                var departamentos = ctx.Departamento.ToList();
                return View(departamentos);
            }
            else
            {
                return Content("<script type='text/javascript'>alert('Você precisa estar logado para visualizar essa página.'); window.location = '/Home/Index';</script>");
            }
        }

        [HttpGet]
        public ActionResult ViewDocs(int IdUsuario, int IdDepartamento)
        {
            var usuarioLogado = HttpContext.Session;

            if (usuarioLogado != null)
            {
                IEnumerable<ArquivoViewModel> files;
                if ((string)usuarioLogado["grupo"] == "usuario")
                {
                   files = (from a in ctx.arquivos
                        join t in ctx.TipoDocumento
                        on a.IdTipoDoc equals t.IdTipoDoc
                        join d in ctx.Departamento
                        on a.IdDepartamento equals d.IdDepartamento
                        where a.IdDepartamento == IdDepartamento && a.IdUsuarioRemetente == IdUsuario 
                        select new ArquivoViewModel
                        {
                            NomeArquivo = a.NomeArquivo,
                            IdArquivo = a.IdArquivo,
                            TipoDocumento = t.TipoDoc,
                            NomeDepartamento = d.NomeDepartamento,
                            DataUltDownload = a.DataUltDownload,
                            DataUpload = a.DataUpload,
                            ContDownloads = a.ContDownloads,
                            arquivo = a.arquivo,
                            Visualizado = a.Visualizado
                        }
                    ).ToList();
                }
                else
                {
                    files = (from a in ctx.arquivos
                             join t in ctx.TipoDocumento
                             on a.IdTipoDoc equals t.IdTipoDoc
                             where a.IdDepartamento == IdDepartamento && a.IdUsuarioDestinatario == IdUsuario
                             select new ArquivoViewModel
                             {
                                 NomeArquivo = a.NomeArquivo,
                                 IdArquivo = a.IdArquivo,
                                 TipoDocumento = t.TipoDoc,
                                 DataUltDownload = a.DataUltDownload,
                                 DataUpload = a.DataUpload,
                                 ContDownloads = a.ContDownloads,
                                 arquivo = a.arquivo,
                                 Visualizado = a.Visualizado
                             }
                    ).ToList();
                }
                return View(files);
            }
            else
            {
                return Content("<script type='text/javascript'>alert('Você precisa estar logado para visualizar essa página.'); window.top.location.href = '/Home/Index';</script>");
            }
        }

        [HttpGet]
        //ATENÇÃO : Action usada p/ Sobrecarga da Action ViewDocs
        [ActionName("AllDocs")]
        public ActionResult ViewDocs(int IdUsuario)
        {
            var usuarioLogado = HttpContext.Session;

            if (usuarioLogado != null)
            {
                IEnumerable<ArquivoViewModel> files;

                //Todos os arquivos enviados pelos administradores
                if ((string)usuarioLogado["grupo"] == "usuario")
                {
                    files = (from a in ctx.arquivos
                             join t in ctx.TipoDocumento
                             on a.IdTipoDoc equals t.IdTipoDoc
                             join d in ctx.Departamento
                             on a.IdDepartamento equals d.IdDepartamento
                             where a.IdUsuarioRemetente == IdUsuario
                             select new ArquivoViewModel
                             {
                                 NomeArquivo = a.NomeArquivo,
                                 IdArquivo = a.IdArquivo,
                                 TipoDocumento = t.TipoDoc,
                                 NomeDepartamento = d.NomeDepartamento,
                                 DataUltDownload = a.DataUltDownload,
                                 DataUpload = a.DataUpload,
                                 ContDownloads = a.ContDownloads,
                                 arquivo = a.arquivo,
                                 Visualizado = a.Visualizado
                             }
                    ).ToList();
                }
                //Todos os arquivos não lidos pelo cliente
                else
                {
                    files = (from a in ctx.arquivos
                             join t in ctx.TipoDocumento
                             on a.IdTipoDoc equals t.IdTipoDoc
                             join d in ctx.Departamento
                             on a.IdDepartamento equals d.IdDepartamento
                             where a.IdUsuarioDestinatario == IdUsuario && a.Visualizado == 0
                             select new ArquivoViewModel
                             {
                                 NomeArquivo = a.NomeArquivo,
                                 IdArquivo = a.IdArquivo,
                                 TipoDocumento = t.TipoDoc,
                                 NomeDepartamento = d.NomeDepartamento,
                                 DataUltDownload = a.DataUltDownload,
                                 DataUpload = a.DataUpload,
                                 ContDownloads = a.ContDownloads,
                                 arquivo = a.arquivo,
                                 Visualizado = a.Visualizado
                             }
                    ).ToList();
                }
                return View("~/Views/Workflow/ViewDocs.cshtml",files);
            }
            else
            {
                return Content("<script type='text/javascript'>alert('Você precisa estar logado para visualizar essa página.'); window.top.location.href = '/Home/Index';</script>");
            }
        }

        public ActionResult PaginaCadastro()
        {
            var usuarioLogado = HttpContext.Session["usuario"];
            var tipoUsuario = HttpContext.Session["grupo"];

            if (usuarioLogado != null && (string)tipoUsuario == "usuario")
            {
                return View();
            }
            else
            {
                return Content("<script type='text/javascript'>alert('Você precisa ser um administrador para visualizar essa página.'); window.location = '/Home/Index';</script>");
            }
        }

        public ActionResult CadastroProps()
        {
            var usuarioLogado = HttpContext.Session["usuario"];
            var tipoUsuario = HttpContext.Session["grupo"];

            if (usuarioLogado != null && (string)tipoUsuario == "usuario")
            {
                var arquivoControllerCall = new ArquivoController();
                ViewBag.DepartamentosLista = arquivoControllerCall.GetDepartamentos();
                ViewBag.TipoDocsLista = arquivoControllerCall.GetTipoDocs();
                if (TempData["MessageDep"] != null)
                {
                    ViewBag.MessageDep = TempData["MessageDep"];
                }
                if (TempData["MessageTipoDoc"] != null)
                {
                    ViewBag.MessageTipoDoc = TempData["MessageTipoDoc"];
                }
                return View();
            }
            else
            {
                return Content("<script type='text/javascript'>alert('Você precisa ser um administrador para visualizar essa página.'); window.location = '/Home/Index';</script>");
            }
        }
    }
}