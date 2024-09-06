using Xunit;
using FakeItEasy; 
using AutoFixture;
using Tienda;
using FakeItEasy.Core;
using AutoFixture.AutoFakeItEasy;
using AutoFixture.Kernel;
using AutoFixture.Xunit2;
using AutoFixture.Dsl;
using Microsoft.Win32;
using System.Buffers.Text;
using System.Linq.Expressions;
using System;
// Fixture: instancia real, valores random
// Mock: instancia falsa, puede usarse con fixture para no poner los valores a mano
//https://autofixture.github.io/docs/quick-start/

namespace UnitTesting
{
   
    public class Test_tienda_fixture
    {
        [Fact]
        public void AgregarProductoTest()
        {
            // Arrange
            //  sirve para configurar AutoFixture de tal manera que cuando se necesite crear mocks de interfaces o
            //  clases abstractas, lo haga utilizando FakeItEasy en lugar de generarlos manualmente o con otra biblioteca de mockin

            var fixture = new Fixture()
                .Customize(new AutoFakeItEasyCustomization() { ConfigureMembers = true });// Personalización para FakeItEasy
                                                                                          // ConfigureMembers = true es para asignar valores a los mocks, si no vienen vacios

            //Now AutoFixture will be able to create mocks as well as instances of Abstract types and interfaces.
            //Since our mocks will now be auto - generated we can go straight on and generate our Producto instance.

            var tienda = fixture.Create<Tienda.Tienda>(); // Crea una instancia real ya que no hay interfaz de tienda
            var productoFake = fixture.Create<IProducto>(); // Crea un mock (ya que es una interfaz) utilizando fakeItEasy

            // var productoReal = fixture.Create<Producto>();         

            //El Fixture puede crear objetos asignandoles valores aleatorios a los atributos del objeto

            // Act
            tienda.AgregarProducto(productoFake);

            // Assert
            Assert.Contains(productoFake, tienda.Inventario);
        }


        [Fact]
        public void BuscarProductoTest()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization() { ConfigureMembers = true });
            var tienda = fixture.Create<Tienda.Tienda>();

            var producto = fixture.Create<IProducto>();
            tienda.AgregarProducto(producto);

            //Act
            var productoEncontrado = tienda.BuscarProducto(producto.Nombre);

            //Assert
            Assert.Equal(producto, productoEncontrado);
        }

        [Fact]
        public void BuscarProductoNoExistenteTest()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization() { ConfigureMembers = true });
            var tienda = fixture.Create<Tienda.Tienda>();   //objeto real
            var producto = fixture.Create<IProducto>();     // mock
            tienda.AgregarProducto(producto);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => tienda.EliminarProducto("Pera"));
        }

        [Fact]
        public void EliminarProductoTest()
        {
            //Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization() { ConfigureMembers = true });
            var tienda = new Tienda.Tienda();
            /*var producto1 = fixture.Create<IProducto>();
            var producto2 = fixture.Create<IProducto>();
            var producto3 = fixture.Create<IProducto>();
            tienda.AgregarProducto(producto1);
            tienda.AgregarProducto(producto2);
            tienda.AgregarProducto(producto3);*/
            var productos = fixture.CreateMany<IProducto>();
            var productoABorrar = fixture.Create<IProducto>();
            foreach (var p in productos)
            {
                tienda.AgregarProducto(p);
            }
            tienda.AgregarProducto(productoABorrar);


            //Act
            tienda.EliminarProducto(productoABorrar.Nombre);

            //Assert
            Assert.DoesNotContain(productoABorrar, tienda.Inventario); // Este no funciona, por que??
            ;
        }

        [Fact]
        public void EliminarProductoNoExistenteTest()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization() { ConfigureMembers = true });
            var tienda = fixture.Create<Tienda.Tienda>();
            // Creo el mock de IProducto con un nombre personalizado
            var producto = fixture.Create<IProducto>();
            A.CallTo(() => producto.Nombre).Returns("Manzana");
            tienda.AgregarProducto(producto);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => tienda.EliminarProducto("Pera"));
        }


               [Fact]
        public void AplicarDescuentoTest()
    {
        // Arrange
            var fixture = new Fixture().Customize(new AutoFakeItEasyCustomization() { ConfigureMembers = true });
        var tienda = fixture.Create<Tienda.Tienda>();
        var producto = fixture.Create<IProducto>();
        float porcentajeDescuento = 10;
        float precioActual = 100;  // Variable local para mantener el estado del precio
        float precioEsperado = precioActual * (1 - porcentajeDescuento / 100);


        // Configurar la propiedad Precio para que sea mutable usando precioActual
            A.CallTo(() => producto.Precio).ReturnsLazily(() => precioActual);

        // Configuro el metodo asi se le asigne correctamente el valor al mock
            A.CallTo(() => producto.ActualizarPrecio(A<float>.Ignored))
        .Invokes((float nuevoPrecio) => precioActual = nuevoPrecio);
        tienda.AgregarProducto(producto);

        // Act
        tienda.AplicarDescuento(producto.Nombre, porcentajeDescuento);
        var prodActualizado = tienda.BuscarProducto(producto.Nombre);
        // Assert
        Assert.Equal(precioEsperado, prodActualizado.Precio);

        }
    }
}