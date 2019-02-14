using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using NvtecUploadFiles.Models;

namespace NvtecUploadFiles.Controllers
{
    public class ArquivoController : Controller
    {
        databaseEntities ctx = new databaseEntities();
        // GET: Arquivo
        [HttpGet]
        public ActionResult EnviarArquivo()
        {
            var usuarioLogado = HttpContext.Session["usuario"];
            if(usuarioLogado != null)
            {
                ViewBag.DepartamentosLista = GetDepartamentos();
                ViewBag.ClientesLista = GetClientes();
                ViewBag.TipoDocsLista = GetTipoDocs();
                if(TempData["Message"] != null)
                {
                    ViewBag.Message = TempData["Message"];
                }
                return View();
            }
            else
            {
                return Content("<script type='text/javascript'>alert('Você precisa estar logado para visualizar essa página.'); window.location = '/Home/Index';</script>");
            }
            
        }

        public SelectList GetDepartamentos()
        {
            List<SelectListItem> departamentos = new List<SelectListItem>();
            
            var consultaDepartamentos = ctx.Departamento;

            foreach (var departamento in consultaDepartamentos)
            {
                departamentos.Add(new SelectListItem { Text = departamento.NomeDepartamento, Value = departamento.IdDepartamento.ToString() });
            }

            SelectList slDep = new SelectList(departamentos,"Value","Text","Selecione um departamento");
            return slDep;
        }

        public SelectList GetTipoDocs()
        {
            List<SelectListItem> tipoDocs = new List<SelectListItem>();

            var consultaTipoDocs = ctx.TipoDocumento;

            foreach (var tipoDoc in consultaTipoDocs)
            {
                tipoDocs.Add(new SelectListItem { Text = tipoDoc.TipoDoc, Value = tipoDoc.IdTipoDoc.ToString() });
            }

            SelectList slTipoDoc = new SelectList(tipoDocs, "Value", "Text", "Selecione um tipo de documento");
            return slTipoDoc;
        }

        public SelectList GetClientes()
        {
            List<SelectListItem> clientes = new List<SelectListItem>();

            var consultaClientes = ctx.usuarios.Where(c => c.IdTipoUsu == 1);

            foreach (var cliente in consultaClientes)
            {
                clientes.Add(new SelectListItem { Text = String.Format("{0} ({1})", cliente.NomeUsuario, cliente.Email), Value = cliente.IdUsuario.ToString() });
            }

            SelectList slCli = new SelectList(clientes, "Value", "Text", "Selecione um cliente");
            return slCli;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Upload(ClassArquivoUploadForm arq)
        {
            string[] allowedFiletypes = { ".doc", ".docx", ".pdf", ".xls", ".xlsx", ".xlsm", ".ppt", ".pptx" };
            string ext = Path.GetExtension(arq.Arquivo.FileName);
            if (arq.Arquivo.ContentLength > 0 && allowedFiletypes.Contains(ext))
            {
                byte[] arquivoBytes = new byte[arq.Arquivo.ContentLength];
                using (BinaryReader binReader = new BinaryReader(arq.Arquivo.InputStream))
                {
                    arquivoBytes = binReader.ReadBytes(arq.Arquivo.ContentLength);
                }
                arquivos clsArq = new arquivos()
                {
                    NomeArquivo = arq.Arquivo.FileName,
                    ContentType = arq.Arquivo.ContentType,
                    Visualizado = 0,
                    IdDepartamento = arq.IdDepartamento,
                    IdTipoDoc = arq.IdTipoDoc,
                    IdUsuarioRemetente = arq.IdUsuarioRemetente,
                    IdUsuarioDestinatario = arq.IdUsuarioDestinatario,
                    Descricao = arq.Descricao,
                    DataUpload = DateTime.Now,
                    ContDownloads = 0,
                    arquivo = arquivoBytes
                };
                try
                {
                    ctx.arquivos.Add(clsArq);
                    if (ctx.SaveChanges() != 0)
                    {
                        //Envio de email
                    }
                    ModelState.Clear();
                    TempData["Message"] = "Arquivo enviado com sucesso!";
                    return RedirectToAction("EnviarArquivo","Arquivo");
                }
                catch
                {
                    ModelState.Clear();
                    return Content("<script type='text/javascript'>alert('Ocorreu um erro inesperado no banco de dados.'); window.location = '/Arquivo/EnviarArquivo'</script>");
                }
            }
            else
            {
                ModelState.Clear();
                return Content("<script type='text/javascript'>alert('Por favor, selecione um tipo de arquivo válido.'); window.location = '/Arquivo/EnviarArquivo';</script>");
            }
        }

        //Visualização individual de documentos
        [HttpGet]
        public ActionResult Show(int IdArquivo)
        {
            var usuarioLogado = HttpContext.Session;
            if(usuarioLogado != null)
            {
                //Consulta de dados de arquivo
                var consultaArquivo = ctx.arquivos.Where(a => a.IdArquivo == IdArquivo).FirstOrDefault();
                var consultaDep = ctx.Departamento.Where(d => d.IdDepartamento == consultaArquivo.IdDepartamento).FirstOrDefault();
                var consultaTipo = ctx.TipoDocumento.Where(t => t.IdTipoDoc == consultaArquivo.IdTipoDoc).FirstOrDefault();
                ViewBag.Arquivo = consultaArquivo;
                ViewBag.Departamento = consultaDep;
                ViewBag.TipoDocumento = consultaTipo;

                //Alterações no banco
                if ((string)HttpContext.Session["grupo"] != "usuario")
                {
                    consultaArquivo.ContDownloads += 1;
                    consultaArquivo.Visualizado = 1;
                    ctx.SaveChanges();
                }
                return View();
            }
            else
            {
                return Content("<script type='text/javascript'>alert('Você precisa estar logado para visualizar essa página.'); window.top.location.href = '/Home/Index';</script>");
            }
        }

        [HttpGet]
        public FileResult DownloadFile(int IdArquivo)
        {
            var consultaArquivo = ctx.arquivos.Where(a => a.IdArquivo == IdArquivo).FirstOrDefault();

            if ((string)HttpContext.Session["grupo"] != "usuario")
            {
                consultaArquivo.DataUltDownload = DateTime.Now;
                ctx.SaveChanges();
            }

            return File(consultaArquivo.arquivo, consultaArquivo.ContentType,consultaArquivo.NomeArquivo);

        }

        // Envio de e-mail. Não implementado.
        public async Task<ActionResult> EnviarEmail()
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(new MailAddress(""));
            mail.Body = "";
            mail.From = new MailAddress("sender@outlook.com");
            mail.IsBodyHtml = true;
            mail.Subject = "Novo arquivo disponível para Download no Workflow Contábil";
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = "user@outlook.com",  
                    Password = "password"  
                };
                smtp.Credentials = credential;
                smtp.Host = "smtp-mail.outlook.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mail);
                return RedirectToAction("Sent");
            }
        }

        public ActionResult CadastrarDep(Departamento departamento)
        {
            try
            {
                ctx.Departamento.Add(departamento);
                ctx.SaveChanges();
                TempData["MessageDep"] = "Departamento incluído com sucesso!";
                ModelState.Clear();
                return RedirectToAction("CadastroProps", "Workflow");
            }
            catch(Exception ex)
            {
                ModelState.Clear();
                return Content("<script type='text/javascript'>alert('Ocorreu um erro inesperado no banco de dados.'); window.location = '/Workflow/CadastroProps'</script>");
            }
        }

        public ActionResult CadastrarTipoDoc(string tipoDoc)
        {
            try
            {
                TipoDocumento tDoc = new TipoDocumento();
                tDoc.TipoDoc = tipoDoc;
                ctx.TipoDocumento.Add(tDoc);
                ctx.SaveChanges();
                TempData["MessageTipoDoc"] = "Tipo de arquivo incluído com sucesso!";
                ModelState.Clear();
                return RedirectToAction("CadastroProps", "Workflow");
            }
            catch (Exception ex)
            {
                ModelState.Clear();
                return Content("<script type='text/javascript'>alert('Ocorreu um erro inesperado no banco de dados.'); window.location = '/Workflow/CadastroProps'</script>");
            }
        }

    }
}