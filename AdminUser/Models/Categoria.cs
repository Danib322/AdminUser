using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AdminUser.Models
{
    public partial class Categoria
    {
        public Categoria()
        {
            Automovil = new HashSet<Automovil>();
        }

        public int CategoriaId { get; set; }
        public string Descripcion { get; set; }
        public double Valor { get; set; }

        public virtual ICollection<Automovil> Automovil { get; set; }
    }
}
