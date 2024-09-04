using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tienda
{
    public interface IProducto
    {
        string Nombre { get; set; }
        string Categoria { get; set; }
        float Precio { get; set; }
        public void ActualizarPrecio(float nuevoPrecio);
     
    }
}
