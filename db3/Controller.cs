using System;
using static System.Console;
using MySqlConnector;
using System.Collections.Generic;

namespace db3
{
    public class Controller
    {
        private CustomerRepository customerRepository;
        private ProductRepository productRepository;
        private PurchaseRepository purchaseRepository;
        private OrderRepository orderRepository;
        public Controller()
        {
            MySqlConnection connection = new MySqlConnection("Server=localhost;User ID=root;Password=password;Database=db3");
            customerRepository = new CustomerRepository(connection);
            purchaseRepository = new PurchaseRepository(connection);
            productRepository = new ProductRepository(connection);
            orderRepository = new OrderRepository(connection);
        }

        public void AddCustomer()
        {
            Customer customer = new Customer();
            WriteLine("Enter a username");
            customer.username = ReadLine();
            WriteLine("Enter the customers's address");
            customer.address = ReadLine();
            WriteLine("Enter the customers's phone number");
            customer.phone_number = ReadLine();
            if (!String.IsNullOrEmpty(customer.username) && !String.IsNullOrEmpty(customer.address) && !String.IsNullOrEmpty(customer.phone_number))
            {
                try
                {
                    customerRepository.Insert(customer);
                    WriteLine($"Customer added to the database");
                }
                catch (Exception ex)
                {
                    WriteLine("the customer could not be added to the database");
                    WriteLine("ERROR:    " + ex.Message);
                }
            }
            else
            {
                WriteLine("Incorrectly filled in information about the customer");
            }
        }

        public void DeleteCustomer()
        {
            WriteLine("Enter the ID of the customer you want to delete");
            long id;
            bool isId = long.TryParse(ReadLine(), out id);
            if (isId && customerRepository.CustomerExists(id))
            {
                try
                {
                    customerRepository.Delete(id);
                    WriteLine("Customer removed from database");
                }
                catch (Exception ex)
                {
                    WriteLine("Customer could not be deleted");
                    WriteLine("ERROR :   " + ex.Message);
                }
            }
            else
            {
                WriteLine("An incorrectly specified ID or a customer with such an ID does not exist");
            }

        }

        public void EditCustomer()
        {
            Customer customer = new Customer();
            WriteLine("Enter the ID of the customer you want to edit");
            bool isId = long.TryParse(ReadLine(), out customer.id);
            if (isId && customerRepository.CustomerExists(customer.id))
            {
                WriteLine("Enter a username");
                customer.username = ReadLine();
                WriteLine("Enter the customers's address");
                customer.address = ReadLine();
                WriteLine("Enter the customers's phone number");
                customer.phone_number = ReadLine();
                if (!String.IsNullOrEmpty(customer.username) && !String.IsNullOrEmpty(customer.address) && !String.IsNullOrEmpty(customer.phone_number))
                {
                    try
                    {
                        customerRepository.Update(customer, customer.id);
                        WriteLine($"Customer updated to the database");
                    }
                    catch (Exception ex)
                    {
                        WriteLine("The customer could not be updated");
                        WriteLine("ERROR:    " + ex.Message);
                    }
                }
                else
                {
                    WriteLine("Incorrectly filled in information about the customer");
                }
            }
            else
            {
                WriteLine("An incorrectly specified ID or a customer with such an ID does not exist");
            }

        }

        public void GenerateCustomer()
        {
            WriteLine("Enter the number of customers you want to generate");
            int count;
            bool isCount = int.TryParse(ReadLine(), out count);
            if (isCount && count > 0)
            {
                try
                {
                    while (count != 0)
                    {
                        customerRepository.Generate(count);
                        count--;
                    }
                    WriteLine("Customers generated");
                }
                catch (Exception ex)
                {
                    WriteLine("Failed to generate customers");
                    WriteLine("ERROR:  " + ex.Message);
                }
            }
            else
            {
                WriteLine("The number of customers to be generated is incorrect");
            }

        }

        public void AddProduct()
        {
            Product product = new Product();
            WriteLine("Enter the product name:");
            product.product_name = ReadLine();
            WriteLine("Enter the price of the product:");
            bool isPrice = double.TryParse(ReadLine(), out product.price);
            if (!String.IsNullOrEmpty(product.product_name) && isPrice)
            {
                try
                {
                    productRepository.Insert(product);
                    WriteLine("The product is added to the database");
                }
                catch (Exception ex)
                {
                    WriteLine("The product could not be added to the database");
                    WriteLine("ERROR:  " + ex.Message);
                }

            }
            else
            {
                WriteLine("Product fields are filled incorrectly");
            }


        }

        public void DeleteProduct()
        {
            WriteLine("Enter the ID of the product you want to delete");
            long id;
            bool isId = long.TryParse(ReadLine(), out id);
            if (isId && productRepository.ProductExists(id))
            {
                try
                {
                    productRepository.Delete(id);
                    WriteLine("Product removed from database");
                }
                catch (Exception ex)
                {
                    WriteLine("Product could not be deleted");
                    WriteLine("ERROR :   " + ex.Message);
                }
            }
            else
            {
                WriteLine("An incorrectly specified ID or a product with such an ID does not exist");
            }

        }

        public void EditProduct()
        {

            Product product = new Product();
            WriteLine("Enter the ID of the product you want to edit");
            long id;
            bool isId = long.TryParse(ReadLine(), out id);
            if (isId && productRepository.ProductExists(id))
            {
                WriteLine("Enter the product name:");
                product.product_name = ReadLine();
                WriteLine("Enter the price of the product:");
                bool isPrice = double.TryParse(ReadLine(), out product.price);
                if (!String.IsNullOrEmpty(product.product_name) && isPrice)
                {
                    try
                    {
                        productRepository.Update(product, id);
                        WriteLine("The product is updated ");
                    }
                    catch (Exception ex)
                    {
                        WriteLine("The product could not be updated");
                        WriteLine("ERROR:  " + ex.Message);
                    }

                }
                else
                {
                    WriteLine("Product fields are filled incorrectly");
                }
            }
            else
            {
                WriteLine("An incorrectly specified ID or a product with such an ID does not exist");
            }


        }

        public void GenerateProduct()
        {
            WriteLine("Enter the number of products you want to generate");
            int count;
            bool isCount = int.TryParse(ReadLine(), out count);
            if (isCount && count > 0)
            {
                try
                {
                    while (count != 0)
                    {
                        productRepository.Generate(count);
                        count--;
                    }
                    WriteLine("Products generated");
                }
                catch (Exception ex)
                {
                    WriteLine("Failed to generate products");
                    WriteLine("ERROR:  " + ex.Message);
                }
            }
            else
            {
                WriteLine("The number of products to be generated is incorrect");
            }

        }

        public void AddOrder()
        {
            WriteLine("Enter your customer ID:");
            long customer_id;
            bool isId = long.TryParse(ReadLine(), out customer_id);
            if (isId && customerRepository.CustomerExists(customer_id))
            {
                bool add_products = true;
                List<long> productsId = new List<long>();
                while (add_products)
                {
                    WriteLine("Enter the ID of the product you want to add to the order or 'done'");
                    long product_id;
                    string command = ReadLine();
                    bool isProductId = long.TryParse(command, out product_id);
                    if (isProductId && productRepository.ProductExists(product_id))
                    {
                        productsId.Add(product_id);
                    }
                    else
                    {
                        if (command.Trim() == "done")
                        {
                            Order order = new Order();
                            order.customer_id = customer_id;
                            order.order_date = DateTime.Now;
                            long[] id_prod = productsId.ToArray();
                            for (int i = 0; i < productsId.Count; i++)
                            {
                                Product product = productRepository.GetProductById(id_prod[i]);
                                order.order_price = order.order_price + product.price;
                            }
                            try
                            {
                                long new_orderId = (long)orderRepository.Insert(order);
                                for (int i = 0; i < productsId.Count; i++)
                                {
                                    Purchase purchase = new Purchase();
                                    purchase.order_id = new_orderId;
                                    purchase.product_id = id_prod[i];
                                    purchaseRepository.Insert(purchase);
                                }
                                WriteLine("Order added");
                            }
                            catch (Exception ex)
                            {
                                WriteLine("Failed to add order");
                                WriteLine("ERROR:   ", ex.Message);
                            }
                            add_products = false;
                        }
                        else
                        {
                            WriteLine("The product ID is incorrect or the product does not exist");
                        }


                    }

                }


            }
            else
            {
                WriteLine("Id is incorrect or no such customer exists");
            }




        }

        public void DeleteOrder()
        {
            WriteLine("Enter the ID of the order you want to delete");
            long id;
            bool isId = long.TryParse(ReadLine(), out id);
            if (isId && orderRepository.OrderExists(id))
            {
                try
                {
                    orderRepository.Delete(id);
                    purchaseRepository.DeleteAllByOrderId(id);
                    WriteLine("Order removed from database");
                }
                catch (Exception ex)
                {
                    WriteLine("Order could not be deleted");
                    WriteLine("ERROR :   " + ex.Message);
                }
            }
            else
            {
                WriteLine("ID is incorrect or the order does not exist");
            }
        }

        public void DeleteProductFromOrder()
        {
            WriteLine("Enter the order ID from which you want to delete the product");
            long order_id;
            bool isOrderId = long.TryParse(ReadLine(), out order_id);
            if (isOrderId && orderRepository.OrderExists(order_id))
            {
                WriteLine("Enter the ID of the product you want to delete from the order");
                long product_id;
                bool isProductId = long.TryParse(ReadLine(), out product_id);
                if (isProductId && productRepository.ProductExists(product_id))
                {
                    try
                    {
                        if (purchaseRepository.DeleteProductFromPurchase(order_id, product_id) != 0)
                        {
                            WriteLine("The product has been removed from the order");
                            if (purchaseRepository.GetCountOfOrders(order_id) != 0)
                            {
                                Product product = productRepository.GetProductById(product_id);
                                Order order = orderRepository.GetOrderById(order_id);
                                order.order_price = order.order_price - product.price;
                                orderRepository.Update(order, order_id);
                            }
                            else
                            {
                                orderRepository.Delete(order_id);
                            }
                        }
                        else
                        {
                            WriteLine("The product has not been removed from the order");
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLine("The product could not be removed from the order");
                        WriteLine("ERROR:  " + ex.Message);
                    }
                }
                else
                {
                    WriteLine("The product id is set incorrectly or the product does not exist");

                }

            }
            else
            {
                WriteLine("ID was entered incorrectly or the order does not exist");
            }

        }

        public void GenerateOrder()
        {

        }
    }
}