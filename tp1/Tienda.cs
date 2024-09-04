using System.Collections.Generic;
namespace Tienda;

public class Tienda{
    private List<Producto> inventario;

    public List<Producto> Inventario { get => inventario; set => inventario = value; }

    public Tienda(){
        inventario = new List<Producto>();
    }

    public void AgregarProducto(Producto producto){
        inventario.Add(producto);
    }

    public Producto BuscarProducto(string nombre){
        foreach(Producto producto in inventario){
            if(producto.Nombre == nombre){
                return producto;
            }
        }
        
        throw new KeyNotFoundException($"No se encontró el producto con el nombre {nombre}");
    }

    public bool EliminarProducto(string nombre){
        Producto producto = BuscarProducto(nombre);
        if(producto != null){
            inventario.Remove(producto);
            return true;
        }
        throw new KeyNotFoundException($"El producto {nombre} no se pudo eliminar de manera correcta");
    }

    public void AplicarDescuento(string nombreProducto, float porcentajeDescuento)
    {
        var producto = BuscarProducto(nombreProducto);
        float nuevoPrecio = (float)(producto.Precio * (1 - porcentajeDescuento / 100));
        producto.ActualizarPrecio(nuevoPrecio);
    }
}