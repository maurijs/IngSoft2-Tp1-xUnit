using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tienda;

namespace UnitTesting
{
    public class TiendaYProductosFixture : IDisposable
    {
        public Tienda.Tienda tienda;
        public Fixture fixture;
        public TiendaYProductosFixture() 
        {
            tienda = new Tienda.Tienda();
            fixture = new Fixture();
            tienda.Inventario =
            [
                new Producto("Manzana", 20, "Fruta"),
                new Producto("Pera", 30, "Fruta"),
                new Producto("Naranja", 20, "Fruta"),
                new Producto("Coca Cola", 50, "Bebida"),
                new Producto("Fanta", 50, "Bebida"),
            ];
        }
        public void Dispose()
        {
            tienda.LimpiarInventario();
            tienda.LimpiarCarrito();
            GC.SuppressFinalize(this);
        }
    }
}
