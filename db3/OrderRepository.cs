using System;
using MySqlConnector;
using System.Collections.Generic;


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
            command.Parameters.AddWithValue("@id", id);
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
                order.customer_id = reader.GetInt32(3);
                return order;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Values cannot be parsed");
                Console.WriteLine("ERROR:  " + ex.Message);
                return null;
            }

        }

        public void Generate(Product product)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = $@"INSERT into orders (order_date, order_price, customer_id )   
            SELECT CURRENT_DATE - INTERVAL FLOOR(RAND() * 120) DAY  , 
            @order_price,
            id as customer_id FROM customers ORDER BY RAND() LIMIT 1 ;";
            command.Parameters.AddWithValue("@order_price", product.price);
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

        public List<Order> FindCustomerOrders(int user_id)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM orders WHERE customer_id=@customer_id ";
            command.Parameters.AddWithValue("@customer_id", user_id);
            MySqlDataReader reader = command.ExecuteReader();
            List<Order> ordersList = new List<Order>();
            while (reader.Read())
            {
                Order order = ParseOrder(reader);
                ordersList.Add(order);
            }
            reader.Close();
            connection.Close();
            return ordersList;

        }

        public List<Order> FilterByDate(DateTime startDate, DateTime endDate)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM orders WHERE order_date>=@startDate AND order_date<=@endDate";
            command.Parameters.AddWithValue("@startDate", startDate);
            command.Parameters.AddWithValue("@endDate", endDate);
            MySqlDataReader reader = command.ExecuteReader();
            List<Order> ordersList = new List<Order>();
            while (reader.Read())
            {
                Order order = ParseOrder(reader);
                ordersList.Add(order);
            }
            reader.Close();
            connection.Close();
            return ordersList;
        }



        public Order GetLastOrder()
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM orders ORDER BY id DESC LIMIT 1; ";
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
