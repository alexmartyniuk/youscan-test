using SaleTerminal.Errors;
using SaleTerminal.Price;
using Xunit;

namespace SaleTerminal.Tests
{
    public class PrintOfSaleTerminalTests
    {
        private readonly PointOfSaleTerminal _sut = new PointOfSaleTerminal();

        [Theory]
        [InlineData(new[] { "A", "B", "C", "D", "A", "B", "A" }, 13.25)]
        [InlineData(new[] { "C", "C", "C", "C", "C", "C", "C" }, 6.0)]
        [InlineData(new[] { "A", "B", "C", "D" }, 7.25)]
        public void TotalPriceShouldBeCalculatedSuccessfully(string[] products, double totalPriceExpected)
        {
            _sut.SetPricing("A", new UnitPrice(1.25), new VolumePrice(3.0, 3));
            _sut.SetPricing("B", new UnitPrice(4.25));
            _sut.SetPricing("C", new UnitPrice(1), new VolumePrice(5.0, 6));
            _sut.SetPricing("D", new UnitPrice(0.75));

            foreach (var product in products)
            {
                _sut.Scan(product);
            }

            var price = _sut.CalculateTotal();

            Assert.Equal(totalPriceExpected, price);
        }

        
        [Theory]
        [InlineData(new string[] { }, 0)]
        [InlineData(new[] { "A" }, double.MaxValue)]
        [InlineData(new[] { "A", "B" }, double.MaxValue + 0.1)]
        [InlineData(new[] { "A", "A" }, double.PositiveInfinity)]
        public void TotalPriceShouldBeCalculatedSuccessfullyForBigNumbers(string[] products, double totalPriceExpected)
        {
            _sut.SetPricing("A", new UnitPrice(double.MaxValue));
            _sut.SetPricing("B", new UnitPrice(0.1));

            foreach (var product in products)
            {
                _sut.Scan(product);
            }

            var price = _sut.CalculateTotal();

            Assert.Equal(totalPriceExpected, price);
        }

        [Fact]
        public void SetPricingShouldFailForDuplicatedProduct()
        {
            _sut.SetPricing("A", new UnitPrice(1.25F));

            Assert.Throws<ProductPriceAlreadyRegisteredException>(
                () =>
                {
                    _sut.SetPricing("A", new UnitPrice(1.25F));
                });
        }

        [Fact]
        public void ScanProductShouldFailForNonExistentProduct()
        {
            Assert.Throws<ProductNotFoundException>(
                () =>
                {
                    _sut.Scan("A");
                });
        }
    }
}
