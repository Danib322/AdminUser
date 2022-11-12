using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AdminUser.Models
{
    public partial class Reserva
    {
        public int ReservaId { get; set; }
        public int UsuarioId { get; set; }
        public int AutoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public virtual Automovil Auto { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}
