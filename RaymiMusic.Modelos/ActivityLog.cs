using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaymiMusic.Modelos
{
    public class ActivityLog
    {

        public Guid Id { get; set; }  // Identificador único

        public string Usuario { get; set; }  // Usuario que realizó la acción

        public string Accion { get; set; }  // Descripción de la acción

        public DateTime Fecha { get; set; }  // Fecha de la acción

    }
}
