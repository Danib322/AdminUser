using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminUser.Models
{
    public class InformeModel
    {
        public InformeModel()
        {
            Autos = new List<AutoInforme>();
        }
        public List<AutoInforme> Autos { get; set; }

    }

    public class AutoInforme
    {
        public string Modelo { get; set; }

        public int NumeroDeReservas { get; set; }

        public string categoriaTop { get; set; }
    }
}
