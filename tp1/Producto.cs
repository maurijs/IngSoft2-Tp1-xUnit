namespace Tienda;

public class Producto: IProducto{
    private string nombre; 
    private float precio;
    private string categoria;

    public virtual string Categoria { get => categoria; set => categoria = value; }
    public virtual float Precio { get => precio; set => precio = value; }
    public virtual string Nombre { get => nombre; set => nombre = value; }

    public Producto(string nombre, float precio, string categoria){
        this.nombre = nombre;
        this.precio = precio;
        this.categoria = categoria;
    }

    public virtual void ActualizarPrecio(float nuevoPrecio){
        if(nuevoPrecio <= 0){
            throw new ArgumentOutOfRangeException(nameof(nuevoPrecio), "El precio no puede ser negativo");
        }
        Precio = nuevoPrecio;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Producto other)
        {
            return this.Nombre == other.Nombre;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Nombre.GetHashCode();
    }
}
