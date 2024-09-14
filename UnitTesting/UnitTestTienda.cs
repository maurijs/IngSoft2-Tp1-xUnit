using Tienda;
using Xunit;
using FakeItEasy; //Libreria mock

/*Un objeto Mock es un objeto imitacion que sirve para hacer pruebas unitarias sin tener que utilizar el
 objeto real que normalmente tiene muchas dependencias (servidores, bd, etc) que pueden dificultar las pruebas*/


namespace UnitTesting
{
  
    public class UnitTestTienda
    {
        [Fact]
        public void AgregarProductoTest()
        {
            // Arrange
            var tienda = new Tienda.Tienda();
            var productoFake = A.Fake<IProducto>();
            A.CallTo(() => productoFake.Nombre).Returns("Manzana");
            A.CallTo(() => productoFake.Categoria).Returns("Fruta");

            // Act
            tienda.AgregarProducto(productoFake);

            // Assert
            Assert.Contains(productoFake, tienda.Inventario);
        }

        [Fact]
        public void BuscarProductoTest()
        {
            //Arrange
            var tienda = new Tienda.Tienda();
            var productoFake = A.Fake<IProducto>();
            A.CallTo(() => productoFake.Nombre).Returns("Manzana");
            A.CallTo(() => productoFake.Categoria).Returns("Fruta");
            tienda.AgregarProducto(productoFake);

            //Act
            var productoEncontrado = tienda.BuscarProducto(productoFake.Nombre);

            //Assert
            Assert.Equal(productoFake, productoEncontrado);
        }

        [Fact]
        public void BuscarProductoNoExistenteTest()
        {
            //Arrange
            var tienda = new Tienda.Tienda();
            var productoFake = A.Fake<IProducto>();
            A.CallTo(() => productoFake.Nombre).Returns("Manzana");
            A.CallTo(() => productoFake.Categoria).Returns("Fruta");
            tienda.AgregarProducto(productoFake);


            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => tienda.EliminarProducto("Pera"));
        }

        [Fact]
        public void EliminarProductoTest()
        {
            //Arrange
            var tienda = new Tienda.Tienda();
            var productoFake1 = A.Fake<IProducto>();
            var productoFake2 = A.Fake<IProducto>();
            var productoFake3 = A.Fake<IProducto>();

            A.CallTo(() => productoFake1.Nombre).Returns("Manzana");
            A.CallTo(() => productoFake1.Categoria).Returns("Fruta");

            A.CallTo(() => productoFake2.Nombre).Returns("Naranja");
            A.CallTo(() => productoFake2.Categoria).Returns("Fruta");

            A.CallTo(() => productoFake3.Nombre).Returns("Pera");
            A.CallTo(() => productoFake3.Categoria).Returns("Fruta");

            tienda.AgregarProducto(productoFake1);
            tienda.AgregarProducto(productoFake2);
            tienda.AgregarProducto(productoFake3);

            //Act
            tienda.EliminarProducto(productoFake2.Nombre);

            //Assert
            Assert.DoesNotContain(productoFake2, tienda.Inventario);
        }

        [Fact]
        public void EliminarProductoNoExistenteTest()
        {
            // Arrange
            var tienda = new Tienda.Tienda();
            var productoFake = A.Fake<IProducto>();
            A.CallTo(() => productoFake.Nombre).Returns("Manzana");
            A.CallTo(() => productoFake.Categoria).Returns("Fruta");
            tienda.AgregarProducto(productoFake);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => tienda.EliminarProducto("Pera"));
        }

        // Los mock solo se pueden utilizar con metodos y propiedades virtuales, u otra opcion es usar interfaces 
        // (la mejor practica), lo que permite que ellos puedan interceptarla o reemplazar su implementación con una simulación.
        // Cambio
        [Fact]
        public void AplicarDescuentoTest()
        {
            // Arrange
            var productoFake = A.Fake<IProducto>();
            float porcentajeDescuento = 10;
            float precioActual = 200; // Variable local para mantener el estado del precio
            float precioEsperado = precioActual * (1 - porcentajeDescuento / 100);

            // Configurar valores iniciales
            A.CallTo(() => productoFake.Nombre).Returns("Manzana");
            A.CallTo(() => productoFake.Categoria).Returns("Fruta");

            // Configurar la propiedad Precio para que sea mutable usando precioActual
            A.CallTo(() => productoFake.Precio).ReturnsLazily(() => precioActual);
            
            // Configuro el metodo asi se le asigne correctamente el valor al mock
            A.CallTo(() => productoFake.ActualizarPrecio(A<float>.Ignored))
            .Invokes((float nuevoPrecio) => precioActual = nuevoPrecio);


            /* Otra forma de hacerlo
             A.CallTo(() => productoFake.ActualizarPrecio(A<float>.That.Matches(f => f == 85)))
                .Invokes(() => precioActual = 85);*/

            var tienda = new Tienda.Tienda();
            tienda.AgregarProducto(productoFake);

            // Act
            tienda.AplicarDescuento(productoFake.Nombre, porcentajeDescuento);

            // Assert
            Assert.Equal(precioEsperado, tienda.BuscarProducto(productoFake.Nombre).Precio);
        }
    }
}