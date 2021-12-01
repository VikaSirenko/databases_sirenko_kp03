using System;
using static System.Console;


public class View
{

    static void Main(string[] args)
    {

        bool doProgram = true;
        while (doProgram)
        {

            try
            {

                Console.WriteLine();
                WriteLine("On what entity do you want to perform actions?('user' / 'message' / 'folder')  or 'exit'");
                string str = ReadLine();
                Controller controller = new Controller();
                if (str == "user")
                {
                    DoActionsWithUsersTable(controller);

                }
                else if (str == "message")
                {
                    DoActionsWithMessagesTable(controller);
                }
                else if (str == "folder")
                {
                    DoActionsWithFoldersTable(controller);
                }
                else if (str == "exit")
                {
                    doProgram = false;
                }
                else
                {
                    WriteLine("Such an entity does not exist");
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR:   " + ex.Message);

            }
        }
    }

    private static void DoActionsWithUsersTable(Controller controller)
    {

        WriteLine("What do you want to do with the user table?('add'/ 'delete' / 'edit' / 'find'/ 'findById'/ 'generate')");
        string command = ReadLine();

        if (command == "add")
        {
            controller.AddUser();

        }
        else if (command == "delete")
        {
            controller.DeleteUser();
        }
        else if (command == "edit")
        {
            controller.EditUser();

        }
        else if (command == "find")
        {
            controller.FindUser();

        }
         else if (command == "generate")
        {
            controller.GenereteUsers();

        }
         else if (command == "findById")
        {
            controller.FindUserById();

        }
        else
        {
            WriteLine("Such a command does not exist");
        }

    }

    private static void DoActionsWithMessagesTable(Controller controller)
    {
        WriteLine("What do you want to do with the messages table?('add'/ 'delete' / 'edit' /'find'/'generate')");
        string command = ReadLine();
        if (command == "add")
        {
            controller.AddMessage();
        }
        else if (command == "delete")
        {
            controller.DeleteMessage();
        }
        else if (command == "edit")
        {
            controller.EditMessage();
        }
         else if (command == "find")
        {
            controller.FindMessage();
        }
         else if (command == "generate")
        {
            controller.GenereteMessages();
        }
        else
        {
            WriteLine("Such a command does not exist");
        }

    }


    private static void DoActionsWithFoldersTable(Controller controller)
    {
        WriteLine("What do you want to do with the folder table?('add'/ 'delete' / 'edit'/ 'find'/ 'generate')");
        string command = ReadLine();
        if (command == "add")
        {
            controller.AddFolder();

        }
        else if (command == "delete")
        {
            controller.DeleteFolder();
        }
        else if (command == "edit")
        {
            controller.EditFolder();

        }
         else if (command == "find")
        {
            controller.FindFolder();

        }
         else if (command == "generate")
        {
            controller.GenereteFolders();
            

        }
        else
        {
            WriteLine("Such a command does not exist");
        }

    }


}






