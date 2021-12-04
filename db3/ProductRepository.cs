using System;
using MySqlConnector;
using System.Collections.Generic;
using System.Collections;

namespace db3
{
    public class ProductRepository
    {

        private MySqlConnection connection;
        public ProductRepository(MySqlConnection connection)
        {
            this.connection = connection;
        }



        //checks if the product exists by his ID 
        public bool ProductExists(long id)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM products WHERE id=@id";
            command.Parameters.AddWithValue("@id", id);
            MySqlDataReader reader = command.ExecuteReader();
            bool result = reader.Read();
            connection.Close();
            return result;

        }


        //adds a new product to the database
        public void Insert(Product product)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText =
                @"
            INSERT INTO products (product_name, price)
            VALUES (@product_name, @price);
            SELECT LAST_INSERT_ID();";
            command.Parameters.AddWithValue("@product_name", product.product_name);
            command.Parameters.AddWithValue("@price", product.price);
            ulong newId = (ulong)command.ExecuteScalar();
            connection.Close();
        }


        //finds the product in the database by his product_name and returns it
        public Product GetProduct(string product_name)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM products WHERE product_name = @product_name ";
            command.Parameters.AddWithValue("@product_name", product_name);
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Product product = ParseProduct(reader);
                connection.Close();
                return product;
            }
            reader.Close();
            connection.Close();
            return null;

        }

        public Product GetProductById(long id)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM products WHERE id = @id ";
            command.Parameters.AddWithValue("@id", id);
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Product product = ParseProduct(reader);
                connection.Close();
                return product;
            }
            reader.Close();
            connection.Close();
            return null;

        }



        //deletes the product by his ID
        public bool Delete(long id)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM products WHERE id=@id";
            command.Parameters.AddWithValue("@id", id);
            int nChanges = command.ExecuteNonQuery();
            connection.Close();
            return nChanges == 1;
        }


        //updates the product
        public bool Update(Product product, long productId)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE products SET product_name=@product_name, price=@price  WHERE id=@id";
            command.Parameters.AddWithValue("@id", productId);
            command.Parameters.AddWithValue("@product_name", product.product_name);
            command.Parameters.AddWithValue("@price", product.price);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;

        }


        //parses product data that came from the database
        private Product ParseProduct(MySqlDataReader reader)
        {
            try
            {
                Product product = new Product();
                product.id = reader.GetInt32(0);
                product.product_name = reader.GetString(1);
                product.price = reader.GetDouble(2);
                return product;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("ERROR:   " + ex.Message);
                return null;
            }

        }


        public void Generate(int numberOfProducts)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = $@"INSERT into products ( product_name, price) 
            SELECT SUBSTR(MD5(RAND()), 1, 8) AS randomString,
            FLOOR(RAND()*(10000-1000+1))+10000 ";
            int res = command.ExecuteNonQuery();
            connection.Close();
        }



        public List<Product> FindProduct(string product_name)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM products WHERE product_name LIKE CONCAT('%', @product_name, '%'); ";
            command.Parameters.AddWithValue("@product_name", product_name);
            MySqlDataReader reader = command.ExecuteReader();
            List<Product> productsList = new List<Product>();
            while (reader.Read())
            {
                Product product = ParseProduct(reader);
                productsList.Add(product);
            }
            reader.Close();
            connection.Close();
            return productsList;

        }

        public List<Product> FilterByPrice(double min_price, double max_price)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM products WHERE price>=@min_price AND price<=@max_price ";
            command.Parameters.AddWithValue("@min_price", min_price);
            command.Parameters.AddWithValue("@max_price", max_price);
            MySqlDataReader reader = command.ExecuteReader();
            List<Product> productsList = new List<Product>();
            while (reader.Read())
            {
                Product product = ParseProduct(reader);
                productsList.Add(product);
            }
            reader.Close();
            connection.Close();
            return productsList;
        }

        public List<Product> GetListByOrderId(long order_id)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM products CROSS JOIN purchases WHERE purchases.order_id=@order_id AND purchases.product_id=products.id ";
            command.Parameters.AddWithValue("@order_id", order_id);
            MySqlDataReader reader = command.ExecuteReader();
            List<Product> productsList = new List<Product>();
            while (reader.Read())
            {
                Product product = ParseProduct(reader);
                productsList.Add(product);
            }
            reader.Close();
            connection.Close();
            return productsList;


        }

        public Product GetRandomProduct()
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM products ORDER BY rand() LIMIT 1 ";
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                Product product = ParseProduct(reader);
                connection.Close();
                return product;
            }
            reader.Close();
            connection.Close();
            return null;

        }

        public List<Product> GetAllProducts()
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM products ";
            MySqlDataReader reader = command.ExecuteReader();
            List<Product> productsList = new List<Product>();
            while (reader.Read())
            {
                Product product = ParseProduct(reader);
                productsList.Add(product);
            }
            reader.Close();
            connection.Close();
            return productsList;

        }
    }

}
