﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NvtecUploadFiles.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class databaseEntities : DbContext
    {
        public databaseEntities()
            : base("name=databaseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<arquivos> arquivos { get; set; }
        public virtual DbSet<Departamento> Departamento { get; set; }
        public virtual DbSet<TipoDocumento> TipoDocumento { get; set; }
        public virtual DbSet<TipoUsuario> TipoUsuario { get; set; }
        public virtual DbSet<usuarios> usuarios { get; set; }
    }
}
