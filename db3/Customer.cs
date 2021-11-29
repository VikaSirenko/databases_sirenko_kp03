namespace db3
{
    public class Customer
    {
        public long id;
        public string username;
        public string address;
        public string phone_number;
        public Customer()
        {
            this.id = default;
            this.username = default;
            this.address = default;
            this.phone_number = default;
        }

        public Customer(string username, string address, string phone_number)
        {
            this.username = username;
            this.address = address;
            this.phone_number = phone_number;
        }

        public override string ToString()
        {
            return $"[{id}] | User name:'{username}' | Address: '{address}'";
        }

    }
}