using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NvtecUploadFiles.Models
{
    public class ClassArquivoUploadForm
    {
        public int IdArquivo { get; set; }
        public string NomeArquivo { get; set; }
        public string ContentType { get; set; }
        public int IdDepartamento { get; set; }
        public int IdUsuarioRemetente { get; set; }
        public int IdUsuarioDestinatario { get; set; }
        public int IdTipoDoc { get; set; }
        public string Descricao { get; set; }
        public byte Visualizado { get; set; }
        public int ContDownloads { get; set; }
        public DateTime? DataUltDownload { get; set; }
        public DateTime DataUpload { get; set; }
        public HttpPostedFileBase Arquivo { get; set; }
    }

    public class ArquivoViewModel : ClassArquivoUploadForm
    {
        public string TipoDocumento { get; set; }
        public string NomeDepartamento { get; set; }
        public byte[] arquivo { get; set; }
    }
}