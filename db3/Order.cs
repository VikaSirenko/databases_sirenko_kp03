using System;

namespace db3
{
    public class Order
    {
        public long id;
        public DateTime order_date;
        public double order_price;
        public long customer_id;

        public Order()
        {
            this.id = default;
            this.order_date = default;
            this.order_price = default;
            this.customer_id = default;
        }

        public Order(DateTime order_date, double order_price, long customer_id)
        {
            this.order_date = order_date;
            this.order_price = order_price;
            this.customer_id = customer_id;
        }

        public override string ToString()
        {
            return $"[{id}] | Order date:'{order_date.ToString("o")}' | Order price: '{order_price}'";
        }

    }
}