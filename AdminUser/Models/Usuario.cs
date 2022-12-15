using System;
using System.Collections.Generic;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace AdminUser.Models
{
    public partial class Usuario
    {
        public Usuario()
        {
            Reserva = new HashSet<Reserva>();
        }

        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public byte[] Pass { get; set; }
        public string Salt { get; set; }
        public string Tipo { get; set; }
        public string pwd { get; set; }

        public virtual ICollection<Reserva> Reserva { get; set; }
    }
}
