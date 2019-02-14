using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NvtecUploadFiles.Models;

namespace NvtecUploadFiles.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var usuarioLogado = HttpContext.Session["usuario"];

            if (usuarioLogado != null)
            {
                return Redirect("/Workflow/Index");
            }
            else
            {
                return View();
            }
        }
    }
}