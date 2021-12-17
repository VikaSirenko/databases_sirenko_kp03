using System;
using MySqlConnector;
using System.Collections.Generic;

namespace db3
{
    public class PurchaseRepository
    {

        private MySqlConnection connection;
        public PurchaseRepository(MySqlConnection connection)
        {
            this.connection = connection;
        }


        //adds a new purchase to the database
        public void Insert(Purchase purchase)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText =
                @"
            INSERT INTO purchases (product_id, order_id)
            VALUES (@product_id, @order_id);
            SELECT LAST_INSERT_ID();
            ";
            command.Parameters.AddWithValue("@product_id", purchase.product_id);
            command.Parameters.AddWithValue("@order_id", purchase.order_id);
            ulong newId = (ulong)command.ExecuteScalar();
            connection.Close();
        }



        //parses purchases data that came from the database
        private Purchase ParsePurchase(MySqlDataReader reader)
        {
            Purchase purchase = new Purchase();
            try
            {
                purchase.id=reader.GetInt32(0);
                purchase.order_id=reader.GetInt32(2);
                purchase.product_id=reader.GetInt32(1);
                return purchase;
            }
               catch (Exception ex)
            {
                System.Console.WriteLine("ERROR:   " + ex.Message);
                return null;
            }

        }


        public int DeleteAllByOrderId(long orderId)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM purchases WHERE order_id=@order_id";
            command.Parameters.AddWithValue("@order_id", orderId);
            int nChanges = command.ExecuteNonQuery();
            connection.Close();
            return nChanges;
        }

        public int DeleteProductFromPurchase(long orderId, long productId)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM purchases WHERE order_id=@order_id AND product_id=@product_id";
            command.Parameters.AddWithValue("@order_id", orderId);
            command.Parameters.AddWithValue("@product_id", productId);
            int nChanges = command.ExecuteNonQuery();
            connection.Close();
            return nChanges;

        }


        public long GetCountOfOrders(long order_id)
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM purchases WHERE order_id=@order_id";
            command.Parameters.AddWithValue("@order_id", order_id);
            long count = (long)command.ExecuteScalar();
            connection.Close();
            return count;
        }

         public List<Purchase> GetAllPurchase()
        {
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM purchases ";
            MySqlDataReader reader = command.ExecuteReader();
            List<Purchase> purchasesList = new List<Purchase>();
            while (reader.Read())
            {
                Purchase purchase = ParsePurchase(reader);
                purchasesList.Add(purchase);
            }
            reader.Close();
            connection.Close();
            return purchasesList;

        }


    }


}
