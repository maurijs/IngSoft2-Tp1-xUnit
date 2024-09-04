﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Tienda;


namespace UnitTesting
{
    public class test_producto
    {
        [Fact]
        public void ActualizarPrecioTest()
        {
            // Arrange
            var producto = new Producto("Manzana", 2, "Fruta");

            //Act
            producto.ActualizarPrecio(1);

            //Assert
            Assert.NotEqual(2, producto.Precio);
        }

        [Fact]
        public void ActualizarPrecioNegativoTest()
        {
            // Arrange
            var tienda = new Tienda.Tienda();
            var producto = new Producto("Manzana", 0.50f, "Fruta");

            //Act && Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => producto.ActualizarPrecio(-1));
        }


    }
}
