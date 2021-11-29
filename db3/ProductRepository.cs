using System;
using MySqlConnector;

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
            VALUES (@product_name, @price)
            ";
            command.Parameters.AddWithValue("@product_name", product.product_name);
            command.Parameters.AddWithValue("@price", product.price);
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
    }

}
