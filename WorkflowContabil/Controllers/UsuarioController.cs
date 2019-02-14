using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NvtecUploadFiles.Models;
using System.Text;
using System.Net.Mail;

namespace NvtecUploadFiles.Controllers
{
    public class UsuarioController : Controller
    {
        databaseEntities ctx = new databaseEntities();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(ClassUsuario user)
        {
            string passEncoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(user.Senha));
            var consultaUsuario = ctx.usuarios.Where(u => u.Email == user.Email && u.Senha == passEncoded).FirstOrDefault();
            if(consultaUsuario != null)
            {
                if(consultaUsuario.IdTipoUsu == 1)
                {
                    Session.Add("usuario", consultaUsuario.NomeUsuario);
                    Session.Add("grupo","cliente");
                    Session.Add("id",consultaUsuario.IdUsuario);
                    return Redirect("/Workflow/Index");
                }
                else if(consultaUsuario.IdTipoUsu == 2)
                {
                    Session.Add("usuario", consultaUsuario.NomeUsuario);
                    Session.Add("grupo", "usuario");
                    Session.Add("id",consultaUsuario.IdUsuario);
                    return Redirect("/Workflow/Index");
                }
                else
                {
                    ModelState.Clear();
                    return Content("<script language='javascript' type='text/javascript'>alert('O usuário não pertence a um grupo válido.');window.location = '/Home/Index';</script>");
                }    
            }
            else
            {
                ModelState.Clear();
                return Content("<script language='javascript' type='text/javascript'>alert('Nome de usuário ou senha incorretos/inválidos. Tente novamente.');window.location = '/Home/Index';</script>");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastro(ClassUsuario user)
        {
            if(user.Senha == user.ConfirmarSenha)
            {
                try
                {
                    var consultaUsuario = ctx.usuarios.Where(u => u.NomeUsuario == user.NomeUsuario || u.Email == user.Email).FirstOrDefault();
                    if(consultaUsuario == null)
                    {
                        int tipoUsu;
                        if (user.TipoUsuario == "cliente")
                        {
                            tipoUsu = 1;
                        }
                        else
                        {
                            tipoUsu = 2;
                        }
                        var passwordBytes = Encoding.UTF8.GetBytes(user.Senha);
                        var usu = new usuarios()
                        {
                            NomeUsuario = user.NomeUsuario,
                            Email = user.Email,
                            Senha = Convert.ToBase64String(passwordBytes),
                            IdTipoUsu = tipoUsu,
                        };
                        ctx.usuarios.Add(usu);
                        ctx.SaveChanges();

                        ModelState.Clear();
                        ViewBag.Message = "Cadastro efetuado com sucesso!!";
                        return View("~/Views/Workflow/PaginaCadastro.cshtml");
                    }
                    else
                    {
                        ModelState.Clear();
                        return Content("<script type='text/javascript'>alert('Este usuário/E-mail já está cadastrado no sistema. Por favor,tente novamente.'); window.location = '/Workflow/PaginaCadastro';</script>");
                    }
                }
                catch
                {
                    ViewBag.Message = "Ocorreu um erro inesperado no cadastro. Tente novamente mais tarde.";
                    ModelState.Clear();
                    return View("~/Views/Workflow/PaginaCadastro.cshtml");
                }
            }
            else
            {
                ModelState.Clear();
                return Content("<script type='text/javascript'>alert('senha e confirmar senha não são idênticos. Por favor,tente novamente.'); window.location = '/Workflow/PaginaCadastro';</script>");
            }
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Redirect("/Home/Index");
        }

    }
}