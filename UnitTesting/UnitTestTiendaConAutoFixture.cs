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

//https://autofixture.github.io/docs/quick-start/

//  DETALLE A CONSIDERAR EN EL USO DE MOCKS: las librerias pueden no tener implementados de forma correcta los 
//  metodos como Equals y GetHashCode. Por lo que conviene hacer override de los metodos o sino evitar hacer comparaciones entre Mocks por REFERENCIA

namespace UnitTesting
{
    public class UnitTestTiendaConAutoFixture:IDisposable //AutoFixture no implementa teardown, por lo que lo implementamos manualmente
    {
        private readonly Fixture? _fixture;
        private readonly Tienda.Tienda _tienda;

        // Setup: Se ejecuta antes de cada prueba
        public UnitTestTiendaConAutoFixture()
        {
            _fixture = (Fixture?)new Fixture()
                .Customize(new AutoFakeItEasyCustomization() { ConfigureMembers = true });
                
            //fixture.Customize<IProducto>(c => c.With(p => p.Precio, 10)); 
            // ConfigureMembers = true es para asignar valores a los mocks, si no vienen vacios

            _tienda = _fixture.Create<Tienda.Tienda>();
      
            _tienda.Inventario = _fixture.CreateMany<IProducto>().Where(x => x.Precio > 0).ToList();
            _tienda.Carrito = _tienda.Inventario;
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


        [Theory,AutoData]
        public void AplicarDescuentoTest(float porcentajeDescuento, float precioActual)
        {
            // Arrange
            var producto = _fixture.Create<IProducto>();
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

        [Fact]
        public void AgregarAlCarritoTest() 
        {
            var producto = _fixture.Create<IProducto>();
            _tienda.AgregarProducto(producto);

            _tienda.AgregarAlCarrito(producto);

            //Act
            var productoEncontrado = _tienda.Carrito.FirstOrDefault(p => p.Nombre == producto.Nombre);

            //Assert
            Assert.NotNull(productoEncontrado); 
            // Entre mocks conviene comparar con propiedades y no por referencia
        }

        [Fact]
        public void CalcularTotalCarrito() 
        {
            float totalEsperado = 0;
            foreach (var p in _tienda.Carrito)
            {
                _tienda.AplicarDescuento(p.Nombre, 10);
                totalEsperado += p.Precio;
            }

            Assert.Equal(totalEsperado, _tienda.CalcularTotalCarrito());
        }
        //Después de que la prueba termine(ya sea exitosa o fallida), xUnit llamará automáticamente a Dispose() para
        //limpiar los recursos o realizar cualquier tarea necesaria de "teardown" (como limpiar el inventario).
        public void Dispose() 
        {
            _tienda.LimpiarInventario(); 
            _tienda.LimpiarCarrito();
            GC.SuppressFinalize(this);
        }
    }
}