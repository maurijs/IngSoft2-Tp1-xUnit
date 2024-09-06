using Tienda;
using Xunit;
using FakeItEasy; //Libreria mock

/*Un objeto Mock es un objeto imitacion que sirve para hacer pruebas unitarias sin tener que utilizar el
 objeto real que normalmente tiene muchas dependencias (servidores, bd, etc) que pueden dificultar las pruebas*/


namespace UnitTesting
{
    /*ARRANGE inputs and targets. Arrange steps should set up the test case. Does the test require any objects 
    or special settings? Does it need to prep a database? Does it need to log into a web app? Handle all of these
    operations at the start of the test.
    
    ACT on the target behavior. Act steps should cover the main thing to be tested. This could be calling a function
    or method, calling a REST API, or interacting with a web page. Keep actions focused on the target behavior.

    ASSERT expected outcomes. Act steps should elicit some sort of response. Assert steps verify the goodness or 
    badness of that response. Sometimes, assertions are as simple as checking numeric or string values. Other times,
    they may require checking multiple facets of a system. Assertions will ultimately determine if the test passes
    or fails.*/

    public class Test_tienda
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
            var productoFake = A.Fake<IProducto>();
            A.CallTo(() => productoFake.Nombre).Returns("Manzana");
            A.CallTo(() => productoFake.Categoria).Returns("Fruta");
            tienda.AgregarProducto(productoFake);

            //Act
            tienda.EliminarProducto(productoFake.Nombre);

            //Assert
            Assert.DoesNotContain(productoFake, tienda.Inventario);
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