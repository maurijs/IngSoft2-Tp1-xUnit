using AutoFixture.AutoFakeItEasy;
using AutoFixture;
using FakeItEasy;
using Xunit;
using Tienda;
using AutoFixture.Xunit2;

namespace UnitTesting
{
    // El uso de Mocks me sirve para aislar prubeas, pero en este
    // caso trato de integrar dos clases distintas no debo usar mocks
    public class TestIntegracion : IClassFixture<TiendaYProductosFixture>
    {
        private readonly TiendaYProductosFixture _fixture;

        public TestIntegracion(TiendaYProductosFixture Fixture)
        {
            _fixture = Fixture;
        }

        [Fact]
        public void AgregarProductoTest()
        {
            // Arrange
            var tienda = _fixture.tienda;
            var productoReal = new Producto("Manzana", 2, "Fruta");
            // Act
            tienda.AgregarProducto(productoReal);

            // Assert
            Assert.Contains(productoReal, tienda.Inventario);
        }

        [Fact]
        public void BuscarProductoTest()
        {
            //Arrange
            var tienda = _fixture.tienda;
            var productoEsperado = _fixture.tienda.Inventario[0];

            //Act
            var productoEncontrado = tienda.BuscarProducto(productoEsperado.Nombre);

            //Assert
            Assert.Equal(productoEsperado, productoEncontrado);
        }

        [Fact]
        public void BuscarProductoNoExistenteTest()
        {
            //Arrange
            var tienda = _fixture.tienda;
            var producto = _fixture.fixture.Create<Producto>();

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => tienda.BuscarProducto(producto.Nombre));
        }

        [Fact]
        public void EliminarProductoTest()
        {
            //Arrange
            var producto = _fixture.tienda.Inventario[0];

            //Act
            _fixture.tienda.EliminarProducto(producto.Nombre);

            //Assert
            Assert.DoesNotContain(producto, _fixture.tienda.Inventario);
        }

        [Fact]
        public void EliminarProductoNoExistenteTest()
        {
            // Arrange
            var tienda = new Tienda.Tienda();
            var producto = _fixture.fixture.Create<Producto>();

            // Act & Assert
            Assert.Throws<KeyNotFoundException>(() => tienda.EliminarProducto(producto.Nombre));
        }

        [Theory]
        [InlineData(10,200,180)]
        [InlineData(15,200,170)]
        [InlineData(50,200,100)]
        public void AplicarDescuentoTest(float porcentajeDescuento, float precio, float precioEsperado)
        {
            // Arrange
            var tienda = _fixture.tienda;
            var producto = _fixture.tienda.Inventario[0];
            producto.Precio = precio;


            // Act
            tienda.AplicarDescuento(producto.Nombre, porcentajeDescuento);

            // Assert
            Assert.Equal(precioEsperado, tienda.BuscarProducto(producto.Nombre).Precio);
        }

        [Fact]
        public void AgregarAlCarritoTest()
        {
            //Arrange
            var producto = _fixture.tienda.Inventario[0];

            //Act
            _fixture.tienda.AgregarAlCarrito(producto);

            //Assert
            Assert.Contains(producto, _fixture.tienda.Carrito);
        }

        [Theory]
        [InlineData(15, 20)]
        [InlineData(12, 7)]
        [InlineData(2, 6)]
        public void CalcularTotalCarritoTest(float porcentajeDesc1,float porcentajeDesc2)
        {
            //Arrange
            var prod1 = _fixture.tienda.Inventario[0];
            var prod2 = _fixture.tienda.Inventario[1];
            float totalEsperado = 0;

            _fixture.tienda.AgregarAlCarrito(prod1);
            _fixture.tienda.AgregarAlCarrito(prod2);

            _fixture.tienda.AplicarDescuento(prod1.Nombre, porcentajeDesc1);
            _fixture.tienda.AplicarDescuento(prod2.Nombre, porcentajeDesc2);

            foreach (var p in _fixture.tienda.Carrito)
            {
                totalEsperado += p.Precio;
            }

            //Act
            float totalCalculado = _fixture.tienda.CalcularTotalCarrito();


            Assert.Equal(totalEsperado, totalCalculado);
        }

    }
}
