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

//  DETALLE A CONSIDERAR EN EL USO DE MOCKS: las librerias pueden no tener implementados de forma correcta los 
//  metodos como Equals y GetHashCode. Por lo que conviene hacer override de los metodos o sino evitar hacer comparaciones
//  entre Mocks por REFERENCIA


namespace UnitTesting
{
    public class Test_tienda_fixture:IDisposable //AutoFixture no implementa teardown, por lo que lo implementamos manualmente
    {
        private Fixture? _fixture;
        private Tienda.Tienda _tienda;

        public Test_tienda_fixture()
        {
            //  sirve para configurar AutoFixture de tal manera que cuando se necesite crear mocks de interfaces o
            //  clases abstractas, lo haga utilizando FakeItEasy en lugar de generarlos manualmente o con otra biblioteca de mockin

            _fixture = (Fixture?)new Fixture().Customize(new AutoFakeItEasyCustomization() { ConfigureMembers = true });// Personalización para FakeItEasy
                                                                                                                        // ConfigureMembers = true es para asignar valores a los mocks, si no vienen vacios
            _tienda = _fixture.Create<Tienda.Tienda>();
            var productos = _fixture.CreateMany<IProducto>().ToList(); // 5 productos de ejemplo
            foreach (var producto in productos)
            {
                _tienda.AgregarProducto(producto);
            }
            // Para poder reutilizar los datos de las pruebas
        }

        [Fact]
        public void AgregarProductoTest()
        {
            // Arrange
            var productoFake = _fixture.Create<IProducto>(); // Crea un mock (ya que es una interfaz) utilizando fakeItEasy
                                                             // var productoReal = fixture.Create<Producto>();     
            // Act
            _tienda.AgregarProducto(productoFake);

            // Assert
            Assert.Contains(productoFake, _tienda.Inventario);
        }

        [Theory, AutoData]
        public void BuscarProductoTest(string nombreBuscado)
        {
            //Arrange
            var producto = _fixture.Create<IProducto>();
            A.CallTo(() => producto.Nombre).Returns(nombreBuscado);
            _tienda.AgregarProducto(producto);

            //Act
            var productoEncontrado = _tienda.BuscarProducto(producto.Nombre);

            //Assert
            Assert.Equal(producto.Nombre, productoEncontrado.Nombre); // Entre mocks conviene comparar con prop y no por referencia
        }

        [Theory, AutoData]
        public void BuscarProductoNoExistenteTest(string NombreNoExistente)
        {
            //Arrange
            var producto = _fixture.Create<IProducto>();     // mock
            _tienda.AgregarProducto(producto);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _tienda.EliminarProducto(NombreNoExistente));
        }

        [Fact]
        public void EliminarProductoTest()
        {
            //Arrange
            var productoABorrar = _fixture.Create<IProducto>();
            A.CallTo(() => productoABorrar.Nombre).Returns("ProductoAEliminar");
            _tienda.AgregarProducto(productoABorrar);
            _tienda.AgregarProducto(_fixture.Create<IProducto>());
            _tienda.AgregarProducto(_fixture.Create<IProducto>());

            //Act
            _tienda.EliminarProducto(productoABorrar.Nombre);

            //Assert
            Assert.DoesNotContain(_tienda.Inventario, p => p.Nombre == productoABorrar.Nombre);
        }

        [Fact]
        public void EliminarProductoNoExistenteTest()
        {
            // Arrange
            // Creo el mock de IProducto con un nombre personalizado
            var producto = _fixture.Create<IProducto>();
            A.CallTo(() => producto.Nombre).Returns("Manzana");
            _tienda.AgregarProducto(producto);

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => _tienda.EliminarProducto("Pera"));
        }


        [Fact]
        public void AplicarDescuentoTest()
        {
            // Arrange
            var producto = _fixture.Create<IProducto>();
            float porcentajeDescuento = 10;
            float precioActual = 100;  // Variable local para mantener el estado del precio
            float precioEsperado = precioActual * (1 - porcentajeDescuento / 100);

            // Configurar la propiedad Precio para que sea mutable usando precioActual
            A.CallTo(() => producto.Precio).ReturnsLazily(() => precioActual);

            // Configuro el metodo asi se le asigne correctamente el valor al mock
            A.CallTo(() => producto.ActualizarPrecio(A<float>.Ignored))
            .Invokes((float nuevoPrecio) => precioActual = nuevoPrecio);
            _tienda.AgregarProducto(producto);

            // Act
            _tienda.AplicarDescuento(producto.Nombre, porcentajeDescuento);
            var prodActualizado = _tienda.BuscarProducto(producto.Nombre);
            // Assert
            Assert.Equal(precioEsperado, prodActualizado.Precio);
        }

        public void Dispose()
        {
            _tienda.LimpiarInventario();  // Limpieza del estado de la tienda
        }
    }
}