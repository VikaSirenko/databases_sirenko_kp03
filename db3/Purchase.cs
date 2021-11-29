namespace db3
{
    public class Purchase
    {
        public long id;
        public long product_id;
        public long order_id;
        public Purchase()
        {
            this.id = default;
            this.product_id = default;
            this.order_id = default;

        }

        public Purchase(long product_id, long order_id)
        {
            this.product_id = product_id;
            this.order_id = order_id;
        }
    }
}