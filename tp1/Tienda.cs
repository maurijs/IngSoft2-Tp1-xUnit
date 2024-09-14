using System.Collections.Generic;

namespace Tienda 
{
    public class Tienda:ITienda
    {
        private List<IProducto> inventario;
        private List<IProducto> carrito;

        public List<IProducto> Inventario { get => inventario; set => inventario = value; }
        public List<IProducto> Carrito { get => inventario; set => inventario = value; }

        public Tienda(){
            inventario = new List<IProducto>();
            carrito = new List<IProducto>();
        }

        public void AgregarProducto(IProducto producto){
            inventario.Add(producto);
        }

        public IProducto BuscarProducto(string nombre){
            foreach(IProducto producto in inventario){
                if(producto.Nombre == nombre){
                    return producto;
                }
            }
        
            throw new KeyNotFoundException($"No se encontró el producto con el nombre {nombre}");
        }

        public bool EliminarProducto(string nombre){
            var producto = inventario.FirstOrDefault(p => p.Nombre == nombre);
            if (producto != null){
                var index = inventario.FindIndex(p => p.Nombre == nombre); 
                // Encontramos de esta forma ya que FakeItEasy no tiene una implementación correcta de Equals, entonces IndexOf no puede encontrar la instancia correcta,
                // devolviendo 0
                inventario.RemoveAt(index);
                return true;
            }
            throw new KeyNotFoundException($"El producto {nombre} no se encontro en el inventario");
        }

        public void AplicarDescuento(string nombreProducto, float porcentajeDescuento)
        {
            var producto = BuscarProducto(nombreProducto);
            float nuevoPrecio = (float)(producto.Precio * (1 - porcentajeDescuento / 100));
            producto.ActualizarPrecio(nuevoPrecio);
        }

        public void AgregarAlCarrito(IProducto producto) 
        {
            if (producto != null || !carrito.Contains(producto) || inventario.Contains(producto))
            {
                carrito.Add(producto); 
            }
        }
        public float CalcularTotalCarrito() 
        {
            float total = 0;
            foreach (var p in Carrito)
            {
                total += p.Precio; 
            }
            return total;
        }
        public void LimpiarInventario()
        {
            inventario.Clear();  // Limpia todos los productos del inventario
        }
        public void LimpiarCarrito()
        {
            carrito.Clear();
        }

    }
}