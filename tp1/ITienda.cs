using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tienda;

namespace Tienda
{
    public interface ITienda
    {
        public void AgregarProducto(IProducto producto);
        public IProducto BuscarProducto(string nombre);
        public bool EliminarProducto(string nombre);
        public void AplicarDescuento(string nombreProducto, float porcentajeDescuento);
        public void AgregarAlCarrito(IProducto producto);
        public float CalcularTotalCarrito();
        public void LimpiarInventario();
        public void LimpiarCarrito();
    }
}
