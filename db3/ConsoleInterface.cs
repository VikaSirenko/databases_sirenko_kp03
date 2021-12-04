using static System.Console;
using System;

namespace db3
{
    public class ConsoleInterface
    {
        private Controller controller;
        public void Start()
        {
            controller = new Controller();

            bool doProgram = true;
            while (doProgram)
            {
                try
                {

                    WriteLine();
                    WriteLine("What you want to do? :'interactEntity' or 'showStatistics' or 'exit'");
                    string command = ReadLine();
                    if (command.Trim() == "interactEntity")
                    {
                        bool interact = true;
                        while (interact)
                        {
                            WriteLine("What entity do you want to interact with? ('customer' / 'product' / 'order')  or 'exit'");
                            command = ReadLine().Trim();
                            switch (command)
                            {
                                case "customer":
                                    DoActionsWithCustomer();
                                    break;
                                case "product":
                                    DoActionsWithProduct();
                                    break;
                                case "order":
                                    DoActionsWithOrder();
                                    break;
                                case "exit":
                                    interact = false;
                                    break;
                                default:
                                    WriteLine("Such a command does not exist, please try again");
                                    break;
                            }
                        }
                    }
                    else if (command.Trim() == "showStatistics")
                    {
                        WriteLine("What statistics you want to get: 'orderStat' / 'productStat' ");
                        command = ReadLine().Trim();
                        switch (command)
                        {
                            case "orderStat":
                                controller.GetStatisticsOnOrders();
                                break;
                            case "productStat":
                                controller.GetProductStatistics();
                                break;
                            default:
                                WriteLine("Such a command does not exist, please try again");
                                break;
                        }

                    }
                    else if (command.Trim() == "exit")
                    {
                        doProgram = false;
                    }
                    else
                    {
                        WriteLine("Such a command does not exist, please try again");
                    }
                }
                catch (Exception ex)
                {
                    WriteLine("ERROR:  " + ex.Message);
                }
            }

        }

        private void DoActionsWithCustomer()
        {
            WriteLine("What do you want to do with the customer? ('add' / 'delete' / 'edit'/ 'generate' / 'find') or 'exit' ");
            string command = ReadLine().Trim();

            switch (command)
            {
                case "add":
                    controller.AddCustomer();
                    break;
                case "delete":
                    controller.DeleteCustomer();
                    break;
                case "edit":
                    controller.EditCustomer();
                    break;
                case "generate":
                    controller.GenerateCustomer();
                    break;
                case "find":
                    controller.FindCustomer();
                    break;
                case "exit":
                    break;
                default:
                    WriteLine("Such a command does not exist");
                    break;

            }

        }

        private void DoActionsWithProduct()
        {
            WriteLine("What do you want to do with the product? ('add' / 'delete' / 'edit'/ 'generate'/ 'find' / 'filter') or 'exit' ");
            string command = ReadLine().Trim();
            switch (command)
            {
                case "add":
                    controller.AddProduct();
                    break;
                case "delete":
                    controller.DeleteProduct();
                    break;
                case "edit":
                    controller.EditProduct();
                    break;
                case "generate":
                    controller.GenerateProduct();
                    break;
                case "find":
                    controller.FindProductByName();
                    break;
                case "filter":
                    controller.FilterProductByPrice();
                    break;
                case "exit":
                    break;
                default:
                    WriteLine("Such a command does not exist");
                    break;

            }

        }
        private void DoActionsWithOrder()
        {
            WriteLine("What do you want to do with the order? ('add' /'deleteOrder' /'deleteProductFromOrder' /'addProductToOrder' /'generate' /'find' /'filter') or 'exit' ");
            WriteLine();
            string command = ReadLine().Trim();
            switch (command)
            {
                case "add":
                    controller.AddOrder();
                    break;
                case "deleteOrder":
                    controller.DeleteOrder();
                    break;
                case "deleteProductFromOrder":
                    controller.DeleteProductFromOrder();
                    break;
                case "addProductToOrder":
                    controller.AddProductToOrder();
                    break;
                case "generate":
                    controller.GenerateOrder();
                    break;
                case "find":
                    controller.FindCustomerOrders();
                    break;
                case "filter":
                    controller.FilterOrdersByPeriod();
                    break;
                case "exit":
                    break;
                default:
                    WriteLine("Such a command does not exist");
                    break;

            }

        }
    }
}
