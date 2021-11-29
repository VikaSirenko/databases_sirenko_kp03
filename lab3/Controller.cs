using static System.Console;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;

public class DatabaseContext : DbContext
{
    public DbSet<User> users { get; set; }
    public DbSet<Message> messages { get; set; }
    public DbSet<Folder> folders { get; set; }
    public DatabaseContext()
    {
        Database.EnsureCreated();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Username=postgres;Password=16842778;Database=lab2");
    }
}

public class Controller
{

    public void AddUser()
    {
        User user = InputUserData();
        if (user != null)
        {
            try
            {
                UserRepository.Insert(user);
                WriteLine("User added");
            }
            catch (Exception ex)
            {
                WriteLine("User failed to add");
                Console.WriteLine("ERROR:   " + ex.Message);
            }
        }
        else
        {
            WriteLine("User data entered incorrectly");
        }

    }
    public void DeleteUser()
    {

        WriteLine("Enter the ID of the user you want to delete:");
        int id;
        bool isUserId = int.TryParse(ReadLine(), out id);
        if (isUserId)
        {
            User user = UserRepository.GetByUserId(id);
            if (user != null)
            {
                try
                {
                    UserRepository.Delete(id);
                    WriteLine("User has been removed from the database");

                }
                catch (Exception ex)
                {
                    WriteLine("User could not be deleted");
                    Console.WriteLine("ERROR:   " + ex.Message);

                }
            }

            else
            {
                WriteLine("There is no user with this ID");
            }


        }
        else
        {
            WriteLine("Id entered incorrectly");
        }

    }

    public static User InputUserData()
    {
        WriteLine("Enter username:");
        string username = ReadLine();
        WriteLine("Enter fullname:");
        string fullname = ReadLine();
        WriteLine("Enter date of birth(n format: yyyy-mm-dd) :");
        string dateBirth = ReadLine();
        User user = new User();


        if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(fullname))
        {
            user.user_name = username;
            user.full_name = fullname;
            string[] date = dateBirth.Split('-');
            try
            {
                user.date_of_birth = new System.DateTime(int.Parse(date[0]), int.Parse(date[1]), int.Parse(date[2]));
            }
            catch (System.Exception ex)
            {
                WriteLine("ERROR:   " + ex.Message);
                return null;
            }

            return user;

        }
        else
        {
            WriteLine("ERROR: username or fullname entered incorrectly");
            return null;
        }



    }


    public void EditUser()
    {
        WriteLine("Enter the ID of the user you want to edit");
        int id;
        bool isUserId = int.TryParse(ReadLine(), out id);
        if (isUserId)
        {
            User user = UserRepository.GetByUserId(id);
            if (user != null)
            {
                user = InputUserData();
                if (user != null)
                {
                    user.id = id;
                    try
                    {
                        UserRepository.Update(user, id);
                        WriteLine("User data updated");
                    }
                    catch (Exception ex)
                    {
                        WriteLine("User data failed to update");
                        WriteLine("ERROR:   " + ex.Message);

                    }

                }
                else
                {
                    WriteLine("User data entered incorrectly");
                }

            }
            else
            {
                WriteLine("There is no user with this ID");
            }
        }
        else
        {
            WriteLine("Id entered incorrectly");
        }


    }





    public void AddMessage()
    {

        Message message = new Message();
        WriteLine("Enter the ID of the user to whom the message belongs:");
        bool isUserId = int.TryParse(ReadLine(), out message.user_id);
        WriteLine("Enter the ID of the folder that will contain the message:");
        bool isFolderId = int.TryParse(ReadLine(), out message.folder_id);
        if (isFolderId && isUserId)
        {
            if (UserRepository.GetByUserId(message.user_id) != null && FolderRepository.GetByFolderId(message.folder_id) != null)
            {
                WriteLine("Enter the subject of the message");
                message.message_subject = ReadLine();
                WriteLine("Enter the date the message was sent(in format: yyyy-mm-dd)");
                string sentAt = ReadLine();
                string[] date = sentAt.Split('-');
                try
                {
                    message.sent_at = new System.DateTime(int.Parse(date[0]), int.Parse(date[1]), int.Parse(date[2]));
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception(ex.Message);
                }

                try
                {
                    MessageRepository.Insert(message);
                    WriteLine("Message added");
                }
                catch (Exception ex)
                {
                    WriteLine("Message could not be added");
                    WriteLine("ERROR:   " + ex.Message);
                }

            }
            else
            {
                WriteLine("the user or the folder with the specified IDs does not exist");
            }

        }
        else
        {
            WriteLine("identifiers are set incorrectly");
        }

    }
    public void DeleteMessage()
    {

        WriteLine("Enter the ID of the message you want to delete:");
        int id;
        bool isMessageId = int.TryParse(ReadLine(), out id);
        if (isMessageId)
        {
            Message message = MessageRepository.GetByMessageId(id);
            if (message != null)
            {
                try
                {
                    MessageRepository.Delete(id);
                    WriteLine("Message has been removed from the database");

                }
                catch (Exception ex)
                {
                    WriteLine("Message could not be deleted");
                    WriteLine("ERROR:   " + ex.Message);

                }
            }
            else
            {
                WriteLine("There is no message with this ID");
            }

        }
        else
        {
            WriteLine("Id entered incorrectly");
        }
    }
    public void EditMessage()
    {

        WriteLine("Enter the ID of the messsage you want to edit");
        int id;
        bool isMessageId = int.TryParse(ReadLine(), out id);
        if (isMessageId)
        {
            Message message = MessageRepository.GetByMessageId(id);
            if (message != null)
            {
                WriteLine("Enter the ID of the folder that will contain the message:");
                bool isFolderId = int.TryParse(ReadLine(), out message.folder_id);
                if (isFolderId && FolderRepository.GetByFolderId(message.folder_id) != null)
                {
                    WriteLine("Enter the subject of the message");
                    message.message_subject = ReadLine();
                    message.id = id;
                    try
                    {
                        MessageRepository.Update(message, id);
                        WriteLine("Message data updated");
                    }
                    catch (Exception ex)
                    {
                        WriteLine("Message data failed to update");
                        WriteLine("ERROR:   " + ex.Message);

                    }

                }
                else
                {
                    WriteLine("The folder with the specified IDs does not exist");
                }
            }
            else
            {
                WriteLine("There is no message with this ID");
            }
        }

        else
        {
            WriteLine("Id entered incorrectly");
        }


    }




    public void AddFolder()
    {
        Folder folder = new Folder();
        WriteLine("Enter the ID of the user to whom the message belongs:");
        bool isUserId = int.TryParse(ReadLine(), out folder.user_id);
        if (isUserId)
        {
            if (UserRepository.GetByUserId(folder.user_id) != null)
            {
                WriteLine("Enter a name for the folder");
                folder.folder_name = ReadLine();
                WriteLine("Enter a name for the folder(in format: yyyy-mm-dd)");
                string createdAt = ReadLine();
                string[] date = createdAt.Split('-');
                try
                {
                    folder.date_of_creation = new System.DateTime(int.Parse(date[0]), int.Parse(date[1]), int.Parse(date[2]));
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception(ex.Message);
                }

                try
                {
                    FolderRepository.Insert(folder);
                    WriteLine("Folder added");
                }
                catch (Exception ex)
                {
                    WriteLine("Folder could not be added");
                    WriteLine("ERROR:   " + ex.Message);
                }

            }
            else
            {
                WriteLine("There is no user with this ID");
            }

        }
        else
        {
            WriteLine("Id set incorrectly");
        }


    }

    public void DeleteFolder()
    {

        WriteLine("Enter the ID of the folder you want to delete:");
        int id;
        bool isFolderId = int.TryParse(ReadLine(), out id);
        if (isFolderId)
        {
            Folder folder = FolderRepository.GetByFolderId(id);
            if (folder != null)
            {
                try
                {
                    FolderRepository.Delete(id);
                    WriteLine("Folder has been removed from the database");

                }
                catch (Exception ex)
                {
                    WriteLine("Folder could not be deleted");
                    WriteLine("ERROR:   " + ex.Message);

                }
            }

            else
            {
                WriteLine("There is no folder with this ID");
            }
        }
        else
        {
            WriteLine("Id entered incorrectly");
        }

    }

    public void EditFolder()
    {
        WriteLine("Enter the ID of the folder you want to edit");
        int id;
        bool isFolderId = int.TryParse(ReadLine(), out id);
        if (isFolderId)
        {
            Folder folder = FolderRepository.GetByFolderId(id);
            if (folder != null)
            {
                folder.id = id;
                WriteLine("Enter a new folder name:");
                folder.folder_name = ReadLine();
                try
                {
                    FolderRepository.Update(folder, id);
                    WriteLine("Folder data updated");

                }
                catch (Exception ex)
                {
                    WriteLine("Folder data failed to update");
                    WriteLine("ERROR:   " + ex.Message);
                }
            }
            else
            {
                WriteLine("There is no folder with this ID");
            }
        }
        else
        {
            WriteLine("Id entered incorrectly");
        }

    }


}

