using static System.Console;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using Npgsql;


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
    private UserRepository userRepository;
    private MessageRepository messageRepository;
    private FolderRepository folderRepository;
    public Controller()
    {
        DatabaseContext databaseContext = new DatabaseContext();
        string connString = String.Format("Host=localhost;Username=postgres;Password=16842778;Database=lab2");
        NpgsqlConnection connection = new NpgsqlConnection(connString);
        userRepository = new UserRepository(databaseContext, connection);
        messageRepository = new MessageRepository(databaseContext, connection);
        folderRepository = new FolderRepository(databaseContext, connection);
       
    }

    public void AddUser()
    {
        User user = InputUserData();
        if (user != null)
        {
            try
            {
                userRepository.Insert(user);
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
            User user = userRepository.GetByUserId(id);
            if (user != null)
            {
                try
                {
                    messageRepository.DeleteAllByUserId(user.id);
                    folderRepository.DeleteAllByUserId(user.id);
                    userRepository.Delete(id);
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
        User user = new User();


        if (!string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(fullname))
        {
            user.user_name = username;
            user.full_name = fullname;

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
            User user =userRepository.GetByUserId(id);
            if (user != null)
            {
                user = InputUserData();
                if (user != null)
                {
                    user.id = id;
                    try
                    {
                        userRepository.Update(user, id);
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
    public  void FindUser()
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        WriteLine("Enter the username you want to find");
        string user_name = ReadLine();
        WriteLine("Enter the full name of the user you want to find");
        string full_name = ReadLine();
        List<User> users = userRepository.FindUser(user_name, full_name);
        if (users.Count != 0)
        {
            foreach (User u in users)
            {
                WriteLine($"User: {u.ToString()} ");
            }
          
        }
        else
        {
            WriteLine("Users not found");
        }

         watch.Stop();

         WriteLine($"Time: {watch.ElapsedMilliseconds} ms");

    }

     public  void FindUserById()
    {
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        WriteLine("Enter the ID of the user you want to find");
        int id;
        bool isUserId = int.TryParse(ReadLine(), out id);
        if (isUserId)
        {
            User user =userRepository.GetByUserId(id);
            if (user != null)
            {
                 WriteLine($"User: {user.ToString()} ");
            }
            else
            {
                 WriteLine("Users not found");
            }
        }
         else
        {
            WriteLine("There is no user with this ID");
        }
         watch.Stop();

         WriteLine($"Time: {watch.ElapsedMilliseconds} ms");

    }





    public void AddMessage()
    {

        Message message = new Message();
        WriteLine("Enter the ID of the user to whom the message belongs:");
        int userId;
        bool isUserId = int.TryParse(ReadLine(), out userId);
        WriteLine("Enter the ID of the folder that will contain the message:");
        int folderId;
        bool isFolderId = int.TryParse(ReadLine(), out folderId );
        if (isFolderId && isUserId)
        {
            if (userRepository.GetByUserId(userId) != null && folderRepository.GetByFolderId(folderId) != null)
            {
                 message.Userid=userId;
                 message.Folderid=folderId;
                WriteLine("Enter the subject of the message");
                message.message_subject = ReadLine();

                try
                {
                    messageRepository.Insert(message);
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
            Message message = messageRepository.GetByMessageId(id);
            if (message != null)
            {
                try
                {
                    messageRepository.Delete(id);
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
            Message message = messageRepository.GetByMessageId(id);
            if (message != null)
            {
                WriteLine("Enter the ID of the folder that will contain the message:");
                int folderId;
                bool isFolderId = int.TryParse(ReadLine(), out folderId);
                if (isFolderId && folderRepository.GetByFolderId(folderId) != null)
                {
                     message.Folderid=folderId;
                    WriteLine("Enter the subject of the message");
                    message.message_subject = ReadLine();
                    message.id = id;
                    try
                    {
                        messageRepository.Update(message, id);
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
        int userId;
        bool isUserId = int.TryParse(ReadLine(), out userId);
        if (isUserId)
        {
            if (userRepository.GetByUserId(userId) != null)
            {
                folder.Userid=userId;
                WriteLine("Enter a name for the folder");
                folder.folder_name = ReadLine();

                try
                {
                    folderRepository.Insert(folder);
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
            Folder folder = folderRepository.GetByFolderId(id);
            if (folder != null)
            {
                try
                {
                    messageRepository.DeleteAllByFolderId(folder.id);
                    folderRepository.Delete(id);
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
            Folder folder = folderRepository.GetByFolderId(id);
            if (folder != null)
            {
                folder.id = id;
                WriteLine("Enter a new folder name:");
                folder.folder_name = ReadLine();
                try
                {
                    folderRepository.Update(folder, id);
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
    public  void GenereteUsers()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            WriteLine("How many users do you want to generate?");
            int numberOfUsers;
            bool isNumberOfUsers = int.TryParse(ReadLine(), out numberOfUsers);

            if (isNumberOfUsers == false || numberOfUsers <= 0)
            {
                Error.WriteLine("The number of users is incorrect. Try again");
                return;
            }

            userRepository.Generate(numberOfUsers);
            watch.Stop();

            WriteLine($"Time: {watch.ElapsedMilliseconds} ms");

        }


        public  void GenereteMessages()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            WriteLine("How many messages do you want to generate?");
            int numberOfMessages;
            bool isNumberOfMessages = int.TryParse(ReadLine(), out numberOfMessages);

            if (isNumberOfMessages == false || numberOfMessages <= 0)
            {
                Error.WriteLine("The number of messages is incorrect. Try again");
                return;
            }

            messageRepository.Generate(numberOfMessages);
            watch.Stop();

            WriteLine($"Time: {watch.ElapsedMilliseconds} ms");

        }


        public  void GenereteFolders()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            WriteLine("How many folders do you want to generate?");
            int numberOfFolders;
            bool isNumberOfMessages = int.TryParse(ReadLine(), out numberOfFolders);

            if (isNumberOfMessages == false || numberOfFolders <= 0)
            {
                Error.WriteLine("The number of folders is incorrect. Try again");
                return;
            }

            folderRepository.Generate(numberOfFolders);
            watch.Stop();

            WriteLine($"Time: {watch.ElapsedMilliseconds} ms");

        }

        public  void FindMessage()
        {

            WriteLine("Enter the user_name of the user whose message you want to receive");
            string user_name = ReadLine();
            WriteLine("Enter the ID of the folder from which you want to receive the message");
            int folder_id;
            bool isFolderId = int.TryParse(ReadLine(), out folder_id);
            if (isFolderId)
            {
                if (folderRepository.GetByFolderId(folder_id)!=null)
                {
                    List<Message> messages = messageRepository.GetListFilteredMessages(user_name, folder_id);
                    if (messages.Count != 0)
                    {
                        foreach (Message m in messages)
                        {
                            User user = userRepository.GetByUserId(m.Userid);
                            WriteLine($"Message: {m.ToString()}  FROM user {user.ToString()}");
                        }
                    }
                    else
                    {
                        WriteLine("There are no such messages");
                    }

                }
                else
                {
                    WriteLine("The folder with the specified IDs does not exist");
                }

            }
            else
            {
                WriteLine("Identifiers are set incorrectly");
            }



        }


        public void FindFolder()
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            WriteLine("Enter the user_name of the user to whom the folder belongs");
            string user_name = ReadLine();
            WriteLine("Enter the name of the folder you want to find");
            string folder_name = ReadLine();
            List<Folder> folders = folderRepository.GetListOfFilteredFolders(folder_name, user_name);
            if (folders.Count != 0)
            {
                foreach (Folder f in folders)
                {
                    User user = userRepository.GetByUserId(f.Userid);
                    WriteLine($"FOLDER: {f.ToString()} BELONGS TO THE USER: {user.ToString()}");
                }
            }
            else
            {
                WriteLine("There are no folders with such filtered data");
            }

             watch.Stop();

            WriteLine($"Time: {watch.ElapsedMilliseconds} ms");

        }



}

