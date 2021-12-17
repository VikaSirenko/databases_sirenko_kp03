using System;
using MySqlConnector;
using System.Collections.Generic;

namespace db3
{
    public class CustomerRepository
    {
        private MySqlConnection connection;
        public CustomerRepository(MySqlConnection connection)
        {
            this.connection = connection;
        }


        //checks if the customer exists by his ID 
        public bool CustomerExists(long id)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM customers WHERE id=@id";
            command.Parameters.AddWithValue("@id", id);
            MySqlDataReader reader = command.ExecuteReader();
            bool result = reader.Read();
            connection.Close();
            return result;

        }


        //adds a new customer to the database
        public ulong Insert(Customer customer)
        {
            connection.Open();

            MySqlCommand command = connection.CreateCommand();
            command.CommandText =
                @"
             INSERT INTO customers (username, address, phone_number)
             VALUES (@username, @address, @phone_number);
             SELECT LAST_INSERT_ID();";
            command.Parameters.AddWithValue("@username", customer.username);
            command.Parameters.AddWithValue("@address", customer.address);
            command.Parameters.AddWithValue("@phone_number", customer.phone_number);
            ulong newId = (ulong)command.ExecuteScalar();
            connection.Close();
            return newId;
        }


        //deletes the customer by his ID
        public bool Delete(long id)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM customers WHERE id=@id";
            command.Parameters.AddWithValue("@id", id);
            int nChanges = command.ExecuteNonQuery();
            connection.Close();
            return nChanges == 1;
        }


        //updates the customer
        public bool Update(Customer customer, long customerId)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE customers SET username=@username , address=@address , phone_number=@phone_number WHERE id=@id";
            command.Parameters.AddWithValue("@id", customerId);
            command.Parameters.AddWithValue("@username", customer.username);
            command.Parameters.AddWithValue("@address", customer.address);
            command.Parameters.AddWithValue("@phone_number", customer.phone_number);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;

        }


        //parses customer data that came from the database
        private Customer ParseCustomer(MySqlDataReader reader)
        {
            Customer customer = new Customer();
            try
            {
                customer.id = reader.GetInt64(0);
                customer.username = reader.GetString(1);
                customer.address = reader.GetString(2);
                customer.phone_number = reader.GetString(3);
                return customer;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Values cannot be parsed");
                Console.WriteLine("ERROR:  " + ex.Message);
                return null;
            }
        }



        public void Generate(int numberOfCustomers)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"INSERT into customers (username, address, phone_number)
               SELECT SUBSTR(MD5(RAND()), 1, 8) AS randomString,
               SUBSTR(MD5(RAND()), 1, 8) AS randomString,
               LPAD(FLOOR(RAND() * 9999999999.99), 10, '0') AS randomString ";
            int res = command.ExecuteNonQuery();
            connection.Close();
        }


        public List<Customer> FindCustomer(string user_name)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM customers WHERE username LIKE CONCAT('%', @username, '%'); ";
            command.Parameters.AddWithValue("@username", user_name);
            MySqlDataReader reader = command.ExecuteReader();
            List<Customer> customersList = new List<Customer>();
            while (reader.Read())
            {
                Customer customer = ParseCustomer(reader);
                customersList.Add(customer);
            }
            reader.Close();
            connection.Close();
            return customersList;

        }
    }
}









