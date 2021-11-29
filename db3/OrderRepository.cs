using System;
using MySqlConnector;


namespace db3
{
    public class OrderRepository
    {

        private MySqlConnection connection;
        public OrderRepository(MySqlConnection connection)
        {
            this.connection = connection;
        }



        //checks if the order exists by his ID 
        public bool OrderExists(long id)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM orders WHERE id=@id";
            command.Parameters.AddWithValue("$id", id);
            MySqlDataReader reader = command.ExecuteReader();
            bool result = reader.Read();
            connection.Close();
            return result;

        }


        //adds a new order to the database
        public ulong Insert(Order order)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText =
                @"
            INSERT INTO orders (order_date, order_price, customer_id)
            VALUES (@order_date, @order_price, @customer_id);
            SELECT LAST_INSERT_ID();";
            command.Parameters.AddWithValue("@order_date", order.order_date);
            command.Parameters.AddWithValue("@order_price", order.order_price);
            command.Parameters.AddWithValue("@customer_id", order.customer_id);
            ulong newId = (ulong)command.ExecuteScalar();
            connection.Close();
            return newId;
        }



        //deletes the order by his ID
        public bool Delete(long id)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM orders WHERE id=@id";
            command.Parameters.AddWithValue("@id", id);
            int nChanges = command.ExecuteNonQuery();
            connection.Close();
            return nChanges == 1;
        }


        //updates the order
        public bool Update(Order order, long orderId)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE orders SET order_price=@order_price  WHERE id=@id";
            command.Parameters.AddWithValue("@id", orderId);
            command.Parameters.AddWithValue("@order_price", order.order_price);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;

        }


        //parses order data that came from the database
        private Order ParseOrder(MySqlDataReader reader)
        {
            Order order = new Order();
            try
            {
                order.id = reader.GetInt32(0);
                order.order_date = reader.GetDateTime(1);
                order.order_price = reader.GetDouble(2);
                order.customer_id = reader.GetInt32(4);
                return order;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Values cannot be parsed");
                Console.WriteLine("ERROR:  " + ex.Message);
                return null;
            }

        }

        public void Generate(int numberOfOrders)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = $@"INSERT into orders (order_date, order_price, customer_id ) 
            SELECT DISTINCT ON (date) date , trunc, customer_id  FROM   
            (SELECT date(timestamp '2000-01-01' + random() * (timestamp '2021-11-09' - timestamp'2000-01-01')) FROM generate_series(1, {numberOfOrders}) ) as result1, 
            (SELECT trunc(random()*(1000)) FROM generete_series(1, {numberOfOrders}) ) as result2,
            (SELECT id as customer_id FROM customers ORDER BY random() LIMIT {numberOfOrders} ) as result3";
            command.Parameters.AddWithValue("numberOfFolders", numberOfOrders);
            int res = command.ExecuteNonQuery();
            connection.Close();

        }

        public Order GetOrderById(long id)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM orders WHERE id = @id ";
            command.Parameters.AddWithValue("@id", id);
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Order order = ParseOrder(reader);
                connection.Close();
                return order;
            }
            reader.Close();
            connection.Close();
            return null;

        }

    }


}
