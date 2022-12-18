using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminUser.Models
{

    public class reporteCategorias
    {

        public List<CategoriaInforme> ListaCategoria { get; set; }
       
        public reporteCategorias()
        {
            ListaCategoria = new List<CategoriaInforme>();
        }
    }

    public class CategoriaInforme
    {
        public string categoria { get; set; }

        public int NumeroDeReservas { get; set; }

    }

}



