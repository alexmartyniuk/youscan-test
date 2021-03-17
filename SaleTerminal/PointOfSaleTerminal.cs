using System;
using System.Collections.Generic;
using System.Linq;
using SaleTerminal.Errors;
using SaleTerminal.Price;

namespace SaleTerminal
{
    public class PointOfSaleTerminal
    {
        private readonly Dictionary<string, Tuple<UnitPrice, VolumePrice>> _productPrices =
            new Dictionary<string, Tuple<UnitPrice, VolumePrice>>();
        private readonly Dictionary<string, long> _scannedProducts = new Dictionary<string, long>();

        public void SetPricing(string productCode, UnitPrice unitPrice)
        {
            SetPricing(productCode, unitPrice, VolumePrice.Empty);
        }

        public void SetPricing(string productCode, UnitPrice unitPrice, VolumePrice volumePrice)
        {
            if (_productPrices.ContainsKey(productCode))
            {
                throw new ProductPriceAlreadyRegisteredException($"Sale terminal already contains a product with the code {productCode}.");
            }

            _productPrices[productCode] = new Tuple<UnitPrice, VolumePrice>(unitPrice, volumePrice);
        }

        public void Scan(string productCode)
        {
            if (!_productPrices.ContainsKey(productCode))
            {
                throw new ProductNotFoundException($"Sale terminal doesn't contain a product with the code {productCode}.");
            }

            if (_scannedProducts.TryGetValue(productCode, out var currentQuantity))
            {
                _scannedProducts[productCode] = currentQuantity + 1;
            }
            else
            {
                _scannedProducts[productCode] = 1;
            }
        }

        public double CalculateTotal()
        {
            return _scannedProducts
                .Join(_productPrices,
                    scannedProduct => scannedProduct.Key,
                    productPrice => productPrice.Key,
                    (scannedProduct, productPrice) =>
                    {
                        var quantity = scannedProduct.Value;
                        var (unitPrice, volumePrice) = productPrice.Value;
                        var quotient = Math.DivRem(quantity, volumePrice.Quantity, out var remainder);

                        return quotient * volumePrice.Price + remainder * unitPrice.Price;
                    })
                .Aggregate(0.0, (total, price) => total + price);
        }
    }
}
