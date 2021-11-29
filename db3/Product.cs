namespace db3
{
    public class Product
    {
        public long id;
        public string product_name;
        public double price;

        public Product()
        {
            this.id=default;
            this.product_name = default;
            this.price = default;
        }

        public Product(string product_name, double price)
        {
            this.price = price;
            this.product_name = product_name;
        }

        public override string ToString()
        {
            return $"[{id}] | Product name:'{product_name}' | Price: '{price}'";
        }

    }
}