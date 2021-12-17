using System;
using static System.Console;
using MySqlConnector;
using System.Collections.Generic;
using System.Linq;
using System.IO;
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
            MySqlConnection connection = new MySqlConnection("Server=localhost;User ID=root;Password=16842778;Database=db3");
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
            WriteLine("Enter the customers's phone number (the number consists of 10 digits)");
            customer.phone_number = CheckPhoneNumber(ReadLine()).ToString();
            if (!String.IsNullOrEmpty(customer.username) && !String.IsNullOrEmpty(customer.address) && customer.phone_number != "0")
            {
                try
                {
                    customer.id = (long)customerRepository.Insert(customer);
                    WriteLine($"Customer added to the database with id [{customer.id}]");
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

        public int CheckPhoneNumber(string phone_number)
        {
            int number;
            bool isNumber = int.TryParse(phone_number, out number);
            if (isNumber)
            {
                int count = 0;
                int checkN = number;
                while (checkN != 0)
                {
                    checkN = checkN / 10;
                    count++;
                }
                if (count == 10)
                {
                    return number;
                }
                else
                {
                    return default;
                }
            }
            else
            {
                throw new Exception("Еhe phone number must contain only numbers");
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
                customer.phone_number = CheckPhoneNumber(ReadLine()).ToString();
                if (!String.IsNullOrEmpty(customer.username) && !String.IsNullOrEmpty(customer.address) && customer.phone_number != "0")
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


        public void FindCustomer()
        {
            WriteLine("Enter the substring of customer username you want to find");
            string user_name = ReadLine();
            List<Customer> customers = customerRepository.FindCustomer(user_name);
            if (customers.Count != 0)
            {
                foreach (Customer c in customers)
                {
                    WriteLine($"Customer: {c.ToString()} ");
                }

            }
            else
            {
                WriteLine("Customers not found");
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

        public void FindProductByName()
        {
            WriteLine("Enter the substring of product you want to find");
            string product_name = ReadLine();
            List<Product> products = productRepository.FindProduct(product_name);
            if (products.Count != 0)
            {
                foreach (Product p in products)
                {
                    WriteLine($"Product: {p.ToString()} ");
                }

            }
            else
            {
                WriteLine("Products not found");
            }

        }

        public void FilterProductByPrice()
        {
            WriteLine("Enter the minimum price");
            double min_price;
            bool isMin = double.TryParse(ReadLine(), out min_price);
            WriteLine("Enter the maximum price");
            double max_price;
            bool isMax = double.TryParse(ReadLine(), out max_price);
            if (isMin && isMax && min_price > 0 && max_price > 0 && min_price < max_price)
            {
                List<Product> products = productRepository.FilterByPrice(min_price, max_price);
                if (products.Count != 0)
                {
                    foreach (Product p in products)
                    {
                        WriteLine($"Product: {p.ToString()} ");
                    }

                }
                else
                {
                    WriteLine("Products not found");
                }
            }
            else
            {
                WriteLine("Data entered incorrectly");
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
                    purchaseRepository.DeleteAllByOrderId(id);
                    orderRepository.Delete(id);
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
            WriteLine("Enter the number of orders you want to generate");
            int count;
            bool isCount = int.TryParse(ReadLine(), out count);
            if (isCount && count > 0)
            {
                try
                {
                    while (count != 0)
                    {
                        Purchase purchase = new Purchase();
                        Product product = productRepository.GetRandomProduct();
                        purchase.product_id = product.id;
                        orderRepository.Generate(product);
                        purchase.order_id = orderRepository.GetLastOrder().id;
                        purchaseRepository.Insert(purchase);
                        count--;
                    }
                    WriteLine("Orders generated");
                }
                catch (Exception ex)
                {
                    WriteLine("Failed to generate orders");
                    WriteLine("ERROR:  " + ex.Message);
                }
            }
            else
            {
                WriteLine("The number of orders to be generated is incorrect");
            }


        }

        public void AddProductToOrder()
        {
            WriteLine("Enter the order ID to which you want to add the product");
            long order_id;
            bool isOrderId = long.TryParse(ReadLine(), out order_id);
            if (isOrderId && orderRepository.OrderExists(order_id))
            {
                WriteLine("Enter the ID of the product you want to add to the order");
                long product_id;
                bool isProductId = long.TryParse(ReadLine(), out product_id);
                if (isProductId && productRepository.ProductExists(product_id))
                {
                    try
                    {
                        Purchase purchase = new Purchase();
                        purchase.order_id = order_id;
                        purchase.product_id = product_id;
                        purchaseRepository.Insert(purchase);
                        Order order = orderRepository.GetOrderById(order_id);
                        Product product = productRepository.GetProductById(product_id);
                        order.order_price = order.order_price + product.price;
                        orderRepository.Update(order, order_id);
                        WriteLine("The product has been added to the order");
                    }
                    catch (Exception ex)
                    {
                        WriteLine("The product cannot be added to the order");
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

        public void FindCustomerOrders()
        {
            WriteLine("Enter the ID of the customer of the order you want to find");
            int user_id;
            bool isUserId = int.TryParse(ReadLine(), out user_id);
            if (isUserId && customerRepository.CustomerExists(user_id))
            {
                List<Order> orders = orderRepository.FindCustomerOrders(user_id);
                if (orders.Count != 0)
                {
                    foreach (Order o in orders)
                    {

                        WriteLine($"Order: {o.ToString()} ");
                        List<Product> products = productRepository.GetListByOrderId(o.id);
                        foreach (Product p in products)
                        {
                            WriteLine($"------ Product: {p.ToString()}");
                        }
                    }
                }
                else
                {
                    WriteLine("Products not found");
                }

            }
            else
            {
                WriteLine("The customer ID was entered incorrectly or does not exist");
            }

        }

        public void FilterOrdersByPeriod()
        {
            WriteLine("Enter the start date (in format:yyyy-mm-dd)");
            DateTime startDate;
            bool isStartDate = DateTime.TryParse(ReadLine(), out startDate);
            WriteLine("Enter the end date (in format:yyyy-mm-dd)");
            DateTime endDate;
            bool isEndDate = DateTime.TryParse(ReadLine(), out endDate);
            if (isStartDate && isEndDate && startDate < endDate)
            {
                List<Order> orders = orderRepository.FilterByDate(startDate, endDate);
                if (orders.Count != 0)
                {
                    foreach (Order o in orders)
                    {
                        WriteLine($"Order: {o.ToString()} ");
                        List<Product> products = productRepository.GetListByOrderId(o.id);
                        foreach (Product p in products)
                        {
                            WriteLine($"------ Product: {p.ToString()}");
                        }
                    }
                }
                else
                {
                    WriteLine("Products not found");
                }
            }
            else
            {
                WriteLine("The data is set incorrectly");
            }
        }

        public void GetStatisticsOnOrders()
        {
            WriteLine("Enter the start date (in format:yyyy-mm-dd)");
            DateTime startDate;
            bool isStartDate = DateTime.TryParse(ReadLine(), out startDate);
            WriteLine("Enter the end date (in format:yyyy-mm-dd)");
            DateTime endDate;
            bool isEndDate = DateTime.TryParse(ReadLine(), out endDate);
            if (isStartDate && isEndDate && startDate < endDate)
            {
                WriteLine("Enter a name to save the image");
                string name = ReadLine().Trim();
                if (!Directory.Exists("./orders"))
                {
                    Directory.CreateDirectory("./orders");
                }
                string saveFile = "./orders/" + name + ".png";
                Statistics.GenereteImage(orderRepository, startDate, endDate, saveFile);
                WriteLine($"Image with order statistics by time period saved as: [{saveFile}]");
            }
            else
            {
                WriteLine("The data is set incorrectly");
            }

        }

        public void GetProductStatistics()
        {
            WriteLine("Enter a name to save the image");
            string name = ReadLine().Trim();
            if (!Directory.Exists("./products"))
            {
                Directory.CreateDirectory("./products");
            }
            string saveFile = "./products/" + name + ".png";
            Dictionary<int, int> dict = Statistics.GetTopTenProducts(productRepository, purchaseRepository);
            Statistics.GenerateGraphicForTopProducts(dict, saveFile, productRepository);
            int[] counts = new int[dict.Count];
            Product[] products = new Product[dict.Count];
            int i = 0;
            foreach (KeyValuePair<int, int> keyValue in dict)
            {
                products[i] = productRepository.GetProductById(keyValue.Key);
                counts[i] = keyValue.Value;
                i++;
            }
            WriteLine();
            WriteLine("Favorite products of customers:");
            if (dict.Count > 10)
            {
                for (int j = products.Count() - 1; j >= products.Count() - 10; j--)
                {
                    Console.WriteLine($"Number of product orders: [{counts[j]}] | Product : [{products[j].product_name}] | Price: {products[j].price}");
                }
                WriteLine();
                WriteLine($"Images with statistics of products by number of orders saved as: [{saveFile}]");

            }
            else if (dict.Count > 0 && dict.Count <= 10)
            {
                for (int j = products.Count() - 1; j >= 0; j--)
                {
                    Console.WriteLine($"Number of product orders: [{counts[j]}] | Product : [{products[j].product_name}] | Price: {products[j].price}");
                }
                WriteLine();
                WriteLine($"Images with statistics of products by number of orders saved as: [{saveFile}]");

            }
            else
            {
                Console.WriteLine("No products found");
            }

        }

    }
}