using System;

namespace SaleTerminal.Price
{
    public class VolumePrice
    {
        public double Price { get; }
        public long Quantity { get; }

        public VolumePrice(double price, long quantity) 
        {
            if (quantity <= 1)
            {
                throw new ArgumentException("Quantity for volume price cant be less or equal 1.");
            }

            Price = price;
            Quantity = quantity;
        }

        public static VolumePrice Empty = new VolumePrice(0, long.MaxValue);
    }
}