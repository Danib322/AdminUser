using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AdminUser.Models
{
    public partial class Automovil
    {
        public Automovil()
        {
            Reserva = new HashSet<Reserva>();
        }

        public int AutoId { get; set; }
        public string Marca { get; set; }
        public string Placa { get; set; }
        public int CategoriaId { get; set; }
        public string Observaciones { get; set; }
        public bool EstaDisponible { get; set; }

        public virtual Categoria Categoria { get; set; }
        public virtual ICollection<Reserva> Reserva { get; set; }
    }
}
