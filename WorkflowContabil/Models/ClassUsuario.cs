using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NvtecUploadFiles.Models
{
    public class ClassUsuario
    {
        [Required]
        public string NomeUsuario { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Senha { get; set; }
        [Required]
        public string ConfirmarSenha { get; set; }
        [Required]
        public string TipoUsuario { get; set; }
    }
}