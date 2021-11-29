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
                WriteLine();
                WriteLine("What entity do you want to interact with? ('customer' / 'product' / 'order') or 'exit'");
                string command = ReadLine().Trim();
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
                        doProgram = false;
                        break;
                    default:
                        WriteLine("Such a command does not exist, please try again");
                        break;
                }
            }


        }


        private void DoActionsWithCustomer()
        {
            WriteLine("What do you want to do with the customer? ('add' / 'delete' / 'edit'/ 'generate') or 'exit' ");
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
                case "exit":
                    break;
                default:
                    WriteLine("Such a command does not exist");
                    break;

            }

        }

        private void DoActionsWithProduct()
        {
            WriteLine("What do you want to do with the product? ('add' / 'delete' / 'edit'/ 'generate') or 'exit' ");
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
                case "exit":
                    break;
                default:
                    WriteLine("Such a command does not exist");
                    break;

            }

        }
        private void DoActionsWithOrder()
        {
            WriteLine("What do you want to do with the order? ('add' / 'deleteOrder' / 'deleteProductFromOrder'/  'generate') or 'exit' ");
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
                case "generate":
                    controller.GenerateOrder();
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
