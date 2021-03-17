using System;

namespace SaleTerminal.Errors
{
    public class ProductPriceAlreadyRegisteredException : Exception
    {
        public ProductPriceAlreadyRegisteredException(string message) : base(message)
        {
        }
    }
}