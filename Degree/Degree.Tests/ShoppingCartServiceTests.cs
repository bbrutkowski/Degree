using Degree.Services.Interfaces;
using Degree.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Degree.Models.DTO;
using Degree.Models;

namespace Degree.Tests
{
    [TestFixture]
    public class ShoppingCartServiceTests
    {
        private Mock<ISessionService> _sessionServiceMock;
        private Mock<ILogger<ShoppingCartService>> _loggerMock;
        private ShoppingCartService _shoppingCartService;

        [SetUp]
        public void SetUp()
        {
            _sessionServiceMock = new Mock<ISessionService>();
            _loggerMock = new Mock<ILogger<ShoppingCartService>>();

            _shoppingCartService = new ShoppingCartService(_sessionServiceMock.Object, _loggerMock.Object);
        }

        [Test]
        public void GetCart_ReturnsCart_WhenSessionHasCartData()
        {
            // Arrange
            var cartItems = new List<CartItem>
            {
               new() { Item = new CartItemDto { ProductId = 1, Title = "Product 1" }, Amount = 1 },
               new() { Item = new CartItemDto { ProductId = 2, Title = "Product 2" }, Amount = 2 }
            };

            var expectedItemsCount = 2;

            _sessionServiceMock
                .Setup(x => x.GetObjectFromJson<List<CartItem>>("Cart"))
                .Returns(cartItems);

            // Act
            var result = _shoppingCartService.GetCart();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.EqualTo(true));
                Assert.That(result.Value.Count, Is.EqualTo(expectedItemsCount));
            });
        }

        [Test]
        public void GetCart_ReturnsEmptyCart_WhenSessionHasNoCartData()
        {
            // Arrange
            _sessionServiceMock
                .Setup(ss => ss.GetObjectFromJson<List<CartItem>>("Cart"))
                .Returns((List<CartItem>)null);

            var expectedItemsCount = 0;

            // Act
            var result = _shoppingCartService.GetCart();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.EqualTo(true));
                Assert.That(result.Value.Count, Is.EqualTo(expectedItemsCount));
            });
        }

        [Test]
        public void AddItemToCart_AddsProduct_WhenProductIsNotInCart()
        {
            // Arrange
            var cartItemDto = new CartItemDto { ProductId = 1, Title = "Product 1" };
            var cartItems = new List<CartItem>();
            var expectedItemsCount = 1;

            _sessionServiceMock
                .Setup(ss => ss.GetObjectFromJson<List<CartItem>>("Cart"))
                .Returns(cartItems);

            // Act
            var result = _shoppingCartService.AddItemToCart(cartItemDto);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.EqualTo(true));
                Assert.That(cartItems.Count, Is.EqualTo(expectedItemsCount));
            });

            _sessionServiceMock.Verify(x => x.SetObjectAsJson("Cart", It.Is<List<CartItem>>(c => c.Count == 1)), Times.Once);
        }
    }
}
