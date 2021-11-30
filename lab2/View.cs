using System;
using static System.Console;
using Npgsql;

namespace lab2
{
    public class View
    {

        static void Main(string[] args)
        {
            bool doProgram = true;
            while (doProgram)
            {

                try
                {
                    WriteLine("\n");
                    Console.WriteLine("What you want to do : generate data or enter data in person?\nTo generate data, input 'g' .\nTo enter data by hand, input'h'\nTo complete the execution, enter 'exit'");
                    string command = Console.ReadLine();

                    if (command == "g")
                    {
                        GenerateEntities();

                    }
                    else if (command == "h")
                    {
                        DoActions();
                    }
                    else if (command == "exit")
                    {
                        doProgram = false;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Such a command does not exist");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR:   " + ex.Message);

                }
            }
        }



        private static void DoActions()
        {
            string connString = String.Format("Host=localhost;Username=postgres;Password=16842778;Database=lab2");
            NpgsqlConnection connection = new NpgsqlConnection(connString);
            Console.WriteLine();

            UserRepository userRepository = new UserRepository(connection);
            MessageRepository messageRepository = new MessageRepository(connection);
            FolderRepository folderRepository = new FolderRepository(connection);
            WriteLine("On what entity do you want to perform actions?('user' / 'message' / 'folder')");
            string str = ReadLine();
            if (str == "user")
            {
                DoActionsWithUsersTable(userRepository, messageRepository, folderRepository);

            }
            else if (str == "message")
            {
                DoActionsWithMessagesTable(messageRepository, userRepository, folderRepository);
            }
            else if (str == "folder")
            {
                DoActionsWithFoldersTable(userRepository, folderRepository, messageRepository);
            }
            else
            {
                WriteLine("Such an entity does not exist");
            }

        }
        private static void DoActionsWithUsersTable(UserRepository userRepository, MessageRepository messageRepository, FolderRepository folderRepository)
        {

            WriteLine("What do you want to do with the user table?('add'/ 'delete' / 'edit' / 'find')");
            string command = ReadLine();
            if (command == "add")
            {
                Controller.AddUser(userRepository);

            }
            else if (command == "delete")
            {
                Controller.DeleteUser(userRepository, messageRepository, folderRepository);
            }
            else if (command == "edit")
            {
                Controller.EditUser(userRepository);

            }
            else if (command == "find")
            {
                Controller.FindUser(userRepository);
            }
            else
            {
                WriteLine("Such a command does not exist");
            }

        }

        private static void DoActionsWithMessagesTable(MessageRepository messageRepository, UserRepository userRepository, FolderRepository folderRepository)
        {
            WriteLine("What do you want to do with the messages table?('add'/ 'delete' / 'find')");
            string command = ReadLine();
            if (command == "add")
            {
                Controller.AddMessage(messageRepository, userRepository, folderRepository);
            }
            else if (command == "delete")
            {
                Controller.DeleteMessage(messageRepository);
            }
            else if (command == "find")
            {
                Controller.FindMessage(messageRepository, userRepository, folderRepository);
            }
            else
            {
                WriteLine("Such a command does not exist");
            }

        }


        private static void DoActionsWithFoldersTable(UserRepository userRepository, FolderRepository folderRepository, MessageRepository messageRepository)
        {
            WriteLine("What do you want to do with the folder table?('add'/ 'delete' / 'edit' / 'find')");
            string command = ReadLine();
            if (command == "add")
            {
                Controller.AddFolder(folderRepository, userRepository);

            }
            else if (command == "delete")
            {
                Controller.DeleteFolder(folderRepository, messageRepository);
            }
            else if (command == "edit")
            {
                Controller.EditFolder(folderRepository);

            }
            else if (command == "find")
            {
                Controller.FindFolder(folderRepository, userRepository);
            }
            else
            {
                WriteLine("Such a command does not exist");
            }

        }


        private static void GenerateEntities()
        {
            string connString = String.Format("Host=localhost;Username=postgres;Password=16842778;Database=lab2");
            NpgsqlConnection connection = new NpgsqlConnection(connString);

            try
            {
                bool run = true;
                while (run)
                {
                    WriteLine("Enter the entity you want to generate ('user'/ 'message' / 'folder') or 'exit' :");
                    string entity = ReadLine();

                    switch (entity)
                    {
                        case "user":
                            Controller.GenereteUsers(connection);
                            break;
                        case "message":
                            Controller.GenereteMessages(connection);
                            break;
                        case "folder":
                            Controller.GenereteFolders(connection);
                            break;
                        case "exit":
                            run = false;
                            break;
                        default:
                            Console.Error.WriteLine($"[{entity}] cannot be generated,because such an entity is not listed. Try again.");
                            break;

                    }

                    WriteLine();
                }
            }
            catch (Exception ex)
            {
                WriteLine($"Error connecting to data base: {ex.Message.ToString()} ");
            }
        }



    }


}



