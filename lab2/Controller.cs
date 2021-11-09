using Npgsql;
using static System.Console;
using System.Collections.Generic;


namespace lab2
{
    public static class Controller
    {


        public static void AddUser(UserRepository userRepository)
        {
            User user = InputUserData();
            if (user != null)
            {
                userRepository.Insert(user);

            }
            else
            {
                WriteLine("User data entered incorrectly");
            }

        }
        public static void DeleteUser(UserRepository userRepository, MessageRepository messageRepository, FolderRepository folderRepository)
        {

            WriteLine("Enter the ID of the user you want to delete:");
            int id;
            bool isUserId = int.TryParse(ReadLine(), out id);
            if (isUserId)
            {
                bool userExist = userRepository.UserExists(id);
                if (userExist)
                {

                    bool isDelete = userRepository.Delete(id);
                    if (!isDelete)
                    {
                        WriteLine("User could not be deleted");
                    }
                    else
                    {
                        messageRepository.DeleteAllByUserId(id);
                        folderRepository.DeleteAllByUserId(id);
                        WriteLine("User has been removed from the database");
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


        public static void EditUser(UserRepository userRepository)
        {
            WriteLine("Enter the ID of the user you want to edit");
            int id;
            bool isUserId = int.TryParse(ReadLine(), out id);
            if (isUserId)
            {
                bool userExist = userRepository.UserExists(id);
                if (userExist)
                {
                    User user = InputUserData();
                    if (user != null)
                    {
                        user.id = id;
                        bool isUpdate = userRepository.Update(user, id);
                        if (isUpdate)
                        {
                            WriteLine("User data updated");
                        }
                        else
                        {
                            WriteLine("User data failed to update");
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

        public static void FindUser(UserRepository userRepository)
        {
            WriteLine("Enter the username you want to find");
            string user_name = ReadLine();
            WriteLine("Enter the full name of the user you want to find");
            string full_name = ReadLine();
            User user = userRepository.GetUser(user_name, full_name);
            if (user != null)
            {

                WriteLine(user.ToString());
            }
            else
            {
                WriteLine("User not found");
            }


        }




        public static void AddMessage(MessageRepository messageRepository, UserRepository userRepository, FolderRepository folderRepository)
        {

            Message message = new Message();
            WriteLine("Enter the ID of the user to whom the message belongs:");
            bool isUserId = int.TryParse(ReadLine(), out message.user_id);
            WriteLine("Enter the ID of the folder that will contain the message:");
            bool isFolderId = int.TryParse(ReadLine(), out message.folder_id);
            if (isFolderId && isUserId)
            {
                if (userRepository.UserExists(message.user_id) && folderRepository.FolderExists(message.folder_id))
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

                    messageRepository.Insert(message);

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
        public static void DeleteMessage(MessageRepository messageRepository)
        {

            WriteLine("Enter the ID of the message you want to delete:");
            int id;
            bool isMessageId = int.TryParse(ReadLine(), out id);
            if (isMessageId)
            {
                bool messageExist = messageRepository.MessageExists(id);
                if (messageExist)
                {

                    bool isDelete = messageRepository.Delete(id);
                    if (!isDelete)
                    {
                        WriteLine("Message could not be deleted");
                    }
                    else
                    {
                        WriteLine("Message has been removed from the database");
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
        public static void FindMessage(MessageRepository messageRepository, UserRepository userRepository, FolderRepository folderRepository)
        {

            WriteLine("Enter the user_name of the user whose message you want to receive");
            string user_name = ReadLine();
            WriteLine("Enter the ID of the folder from which you want to receive the message");
            int folder_id;
            bool isFolderId = int.TryParse(ReadLine(), out folder_id);
            if (isFolderId)
            {
                if (folderRepository.FolderExists(folder_id))
                {
                    WriteLine("enter the date from which messages should be filtered (in format yyyy-mm-dd):");
                    string sentAt = ReadLine();
                    string[] date = sentAt.Split('-');
                    System.DateTime sent_at;
                    try
                    {
                        sent_at = new System.DateTime(int.Parse(date[0]), int.Parse(date[1]), int.Parse(date[2]));
                    }
                    catch (System.Exception ex)
                    {
                        throw new System.Exception(ex.Message);
                    }
                    List<Message> messages = messageRepository.GetListFilteredMessages(user_name, folder_id, sent_at);
                    if (messages.Count != 0)
                    {
                        foreach (Message m in messages)
                        {
                            User user = userRepository.GetByUserId(m.user_id);
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




        public static void AddFolder(FolderRepository folderRepository, UserRepository userRepository)
        {
            Folder folder = new Folder();
            WriteLine("Enter the ID of the user to whom the message belongs:");
            bool isUserId = int.TryParse(ReadLine(), out folder.user_id);
            if (isUserId)
            {
                if (userRepository.UserExists(folder.user_id))
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

                    folderRepository.Insert(folder);

                }
                else
                {
                    WriteLine("there is no user with this ID");
                }

            }
            else
            {
                WriteLine("id set incorrectly");
            }


        }

        public static void DeleteFolder(FolderRepository folderRepository, MessageRepository messageRepository)
        {

            WriteLine("Enter the ID of the folder you want to delete:");
            int id;
            bool isFolderId = int.TryParse(ReadLine(), out id);
            if (isFolderId)
            {
                bool folderExist = folderRepository.FolderExists(id);
                if (folderExist)
                {

                    bool isDelete = folderRepository.Delete(id);
                    if (!isDelete)
                    {
                        WriteLine("Folder could not be deleted");
                    }
                    else
                    {
                        messageRepository.DeleteAllByFolderId(id);
                        WriteLine("Folder has been removed from the database");
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

        public static void EditFolder(FolderRepository folderRepository)
        {
            WriteLine("Enter the ID of the folder you want to edit");
            int id;
            bool isFolderId = int.TryParse(ReadLine(), out id);
            if (isFolderId)
            {
                bool folderExist = folderRepository.FolderExists(id);
                if (folderExist)
                {
                    Folder folder = folderRepository.GetByFolderId(id);
                    WriteLine("Enter a new folder name:");
                    folder.folder_name = ReadLine();
                    bool isUpdate = folderRepository.Update(folder, id);
                    if (isUpdate)
                    {
                        WriteLine("Folder data updated");
                    }
                    else
                    {
                        WriteLine("Folder data failed to update");
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

        public static void FindFolder(FolderRepository folderRepository, UserRepository userRepository)
        {
            WriteLine("Enter the user_name of the user to whom the folder belongs");
            string user_name = ReadLine();
            WriteLine("Enter the name of the folder you want to find");
            string folder_name = ReadLine();
            List<Folder> folders = folderRepository.GetListOfFilteredFolders(folder_name, user_name);
            if (folders.Count != 0)
            {
                foreach (Folder f in folders)
                {
                    User user = userRepository.GetByUserId(f.user_id);
                    WriteLine($"FOLDER: {f.ToString()} BELONGS TO THE USER: {user.ToString()}");
                }
            }
            else
            {
                WriteLine("There are no folders with such filtered data");
            }

        }


        public static void GenereteUsers(NpgsqlConnection connection)
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

            UserRepository userRepository = new UserRepository(connection);
            userRepository.Generate(numberOfUsers);
            watch.Stop();

            WriteLine($"Time: {watch.ElapsedMilliseconds} ms");

        }


        public static void GenereteMessages(NpgsqlConnection connection)
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

            MessageRepository messageRepository = new MessageRepository(connection);
            messageRepository.Generate(numberOfMessages);
            watch.Stop();

            WriteLine($"Time: {watch.ElapsedMilliseconds} ms");

        }


        public static void GenereteFolders(NpgsqlConnection connection)
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

            FolderRepository folderRepository = new FolderRepository(connection);
            folderRepository.Generate(numberOfFolders);
            watch.Stop();

            WriteLine($"Time: {watch.ElapsedMilliseconds} ms");

        }



    }

}