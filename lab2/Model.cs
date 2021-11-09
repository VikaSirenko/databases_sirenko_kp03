using System;
using System.Collections.Generic;
using Npgsql;

namespace lab2
{

    public class Folder
    {

        public int id;
        public string folder_name;
        public DateTime date_of_creation;
        public int user_id;

        public Folder(int id, int user_id, string folder_name, DateTime date_of_creation)
        {
            this.id = id;
            this.user_id = user_id;
            this.folder_name = folder_name;
            this.date_of_creation = date_of_creation;
        }

        public Folder()
        {
            this.id = default;
            this.user_id = default;
            this.folder_name = default;
            this.date_of_creation = default;

        }

        public override string ToString()
        {
            return $"Id : {id} | Folder name: {folder_name} | User id: {user_id} | Date of creation : {date_of_creation.ToShortDateString()}";
        }

    }


    public class FolderRepository
    {

        private NpgsqlConnection connection;
        public FolderRepository(NpgsqlConnection connection)
        {
            this.connection = connection;
        }

        public void Generate(int numberOfFolders)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = $@"INSERT into folders (folder_name, date_of_creation, user_id) 
            SELECT DISTINCT ON (date)  substr , date , user_id  FROM   
            (SELECT  substr(md5(random()::text), 1, 8)  FROM generate_series(1, {numberOfFolders}) ) as result1,  
            (SELECT date(timestamp '2000-01-01' + random() * (timestamp '2021-11-09' - timestamp'2000-01-01')) FROM generate_series(1, {numberOfFolders}) ) as result2, 
            (SELECT id as user_id FROM users ORDER BY random() LIMIT {numberOfFolders} ) as result3";
            command.Parameters.AddWithValue("numberOfFolders", numberOfFolders);
            int res = command.ExecuteNonQuery();
            connection.Close();

        }


        // adds a folder to the database
        public long Insert(Folder folder)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                INSERT INTO folders (folder_name, date_of_creation, user_id)
                VALUES (@folder_nem, @date_if_creation, @user_id) RETURNING id;
               
                ";
            command.Parameters.AddWithValue("folder_name", folder.folder_name);
            command.Parameters.AddWithValue("date_of_creation", folder.date_of_creation);
            command.Parameters.AddWithValue("user_id", folder.user_id);
            long newId = (long)command.ExecuteScalar();
            connection.Close();
            return newId;
        }







        // parses folder data from a database
        private Folder ParseFolderData(NpgsqlDataReader reader, Folder folder)
        {
            folder.id = reader.GetInt32(0);
            folder.user_id = reader.GetInt32(3);
            folder.folder_name = reader.GetString(1);
            folder.date_of_creation = reader.GetDateTime(2);
            return folder;
        }



        //returns a folder on its id
        public Folder GetByFolderId(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM folders WHERE id = @id";
            command.Parameters.AddWithValue("id", id);
            NpgsqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {

                Folder folder = new Folder();
                folder = ParseFolderData(reader, folder);
                connection.Close();
                return folder;

            }

            reader.Close();
            connection.Close();
            return null;

        }



        // updates the folder
        public bool Update(Folder folder, int folderId)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE folders SET  folder_name=@folder_name   WHERE id=@id";
            command.Parameters.AddWithValue("id", folderId);
            command.Parameters.AddWithValue("folder_name", folder.folder_name);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;

        }


        //removes the folder by id
        public bool Delete(int folderId)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM folders WHERE id=@id";
            command.Parameters.AddWithValue("id", folderId);
            int nChanges = command.ExecuteNonQuery();
            connection.Close();
            return nChanges == 1;
        }



        //deletes all folders belonging to the user
        public void DeleteAllByUserId(int userId)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM folders WHERE user_id=@user_id";
            command.Parameters.AddWithValue("user_id", userId);
            int nChanges = command.ExecuteNonQuery();
            connection.Close();
        }




        //checks if this folder is in the database
        public bool FolderExists(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM folders WHERE id=@id";
            command.Parameters.AddWithValue("id", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            bool result = reader.Read();
            connection.Close();
            return result;
        }


        public List<Folder> GetListOfFilteredFolders(string folder_name, string user_name)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM folders, users WHERE folders.folder_name LIKE '%' || @folder_name || '%' AND users.user_name LIKE '%' || @user_name || '%'  AND folders.user_id=users.id";
            command.Parameters.AddWithValue("folder_name", folder_name);
            command.Parameters.AddWithValue("user_name", user_name);
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Folder> foldersList = new List<Folder>();

            while (reader.Read())
            {
                Folder folder = new Folder();
                folder = ParseFolderData(reader, folder);
                foldersList.Add(folder);
            }

            reader.Close();
            connection.Close();
            return foldersList;
        }



    }

    public class Message
    {
        public int id;
        public int user_id;
        public string message_subject;
        public DateTime sent_at;
        public int folder_id;

        public Message(int id, int user_id, int folder_id, string message_subject, DateTime sent_at)
        {
            this.id = id;
            this.user_id = user_id;
            this.folder_id = folder_id;
            this.message_subject = message_subject;
            this.sent_at = sent_at;
        }

        public Message()
        {
            this.id = default;
            this.user_id = default;
            this.folder_id = default;
            this.message_subject = default;
            this.sent_at = default;
        }

        public override string ToString()
        {
            return $"Id : {id}  |  Message subject : {message_subject} | Sent at : {sent_at.ToShortDateString()}";
        }

    }

    public class MessageRepository
    {
        private NpgsqlConnection connection;
        public MessageRepository(NpgsqlConnection connection)
        {
            this.connection = connection;
        }

        public void Generate(int numberOfMessages)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = $@"INSERT into messages ( message_subject, sent_at, user_id, folder_id) 
            SELECT DISTINCT ON(folder_id)  substr , date , user_id, folder_id FROM   
            (SELECT  substr(md5(random()::text), 1, 8)  FROM generate_series(1, {numberOfMessages}) ) as result1,  
            (SELECT date(timestamp '2000-01-01' + random() * (timestamp '2021-11-09' - timestamp'2000-01-01')) FROM generate_series(1, {numberOfMessages}) ) as result2, 
            (SELECT id as user_id FROM users ORDER BY random() LIMIT {numberOfMessages} ) as result3, 
            (SELECT   id as folder_id FROM folders ORDER BY random() LIMIT {numberOfMessages} ) as result4";
            command.Parameters.AddWithValue("numberOfMessages", numberOfMessages);
            int res = command.ExecuteNonQuery();
            connection.Close();
        }




        // adds a message to the database
        public long Insert(Message message)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText =
            @"
                INSERT INTO messages (user_id, message_subject, sent_at, folder_id)
                VALUES (@id, @user_id, @message_subject, @sent_at, @folder_id) RETURNING id;
                ";
            command.Parameters.AddWithValue("user_id", message.user_id);
            command.Parameters.AddWithValue("message_subject", message.message_subject);
            command.Parameters.AddWithValue("sent_at", message.sent_at);
            command.Parameters.AddWithValue("folder_id", message.folder_id);
            long newId = (long)command.ExecuteScalar();
            connection.Close();
            return newId;
        }







        // parses message data from a database
        private Message ParseMessageData(NpgsqlDataReader reader, Message message)
        {

            message.id = reader.GetInt32(0);
            message.user_id = reader.GetInt32(1);
            message.message_subject = reader.GetString(2);
            message.sent_at = reader.GetDateTime(3);
            message.folder_id = reader.GetInt32(4);
            return message;
        }




        //returns a message on its id
        public Message GetByMessageId(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM messages WHERE id = @id";
            command.Parameters.AddWithValue("id", id);
            NpgsqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {

                Message message = new Message();
                message = ParseMessageData(reader, message);
                connection.Close();
                return message;

            }

            reader.Close();
            connection.Close();
            return null;

        }



        // updates the message
        public bool Update(Message message, int messageId)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE messages SET  message_subject=@message_subject , folder_id =@folder_id  WHERE id=@id";
            command.Parameters.AddWithValue("id", messageId);
            command.Parameters.AddWithValue("message_subject", message.message_subject);
            command.Parameters.AddWithValue("folder_id", message.folder_id);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;

        }


        //removes the message by id
        public bool Delete(int messageId)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM messages WHERE id=@id";
            command.Parameters.AddWithValue("id", messageId);
            int nChanges = command.ExecuteNonQuery();
            connection.Close();
            return nChanges == 1;
        }







        //deletes all messages belonging to the user
        public void DeleteAllByUserId(int userId)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM messages WHERE user_id=@user_id";
            command.Parameters.AddWithValue("user_id", userId);
            int nChanges = command.ExecuteNonQuery();
            connection.Close();
        }


        //deletes all comments belonging to the folder
        public void DeleteAllByFolderId(int folderId)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM messages WHERE folder_id=@folder_id";
            command.Parameters.AddWithValue("folder_id", folderId);
            int nChanges = command.ExecuteNonQuery();
            connection.Close();
        }


        //checks if this message is in the database
        public bool MessageExists(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM messages WHERE id=@id";
            command.Parameters.AddWithValue("id", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            bool result = reader.Read();
            connection.Close();
            return result;
        }



        public List<Message> GetListFilteredMessages(string user_name, int folderId, DateTime sentAt)
        {

            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM messages CROSS JOIN users  WHERE  users.user_name LIKE '%' || @user_name || '%' AND folder_id=@folder_id AND sent_at>@sent_at AND messages.user_id=users.id";
            command.Parameters.AddWithValue("user_name", user_name);
            command.Parameters.AddWithValue("folder_id", folderId);
            command.Parameters.AddWithValue("sent_at", sentAt);
            NpgsqlDataReader reader = command.ExecuteReader();
            List<Message> messages = new List<Message>();
            while (reader.Read())
            {
                Message message = new Message();
                message = ParseMessageData(reader, message);
                messages.Add(message);
            }

            reader.Close();
            connection.Close();
            return messages;


        }
    }

    public class User
    {
        public int id;
        public string user_name;
        public string full_name;
        public DateTime date_of_birth;

        public User(int id, string user_name, string full_name, DateTime date_of_birth)
        {
            this.id = id;
            this.user_name = user_name;
            this.full_name = full_name;
            this.date_of_birth = date_of_birth;
        }

        public User()
        {
            this.id = default;
            this.user_name = default;
            this.full_name = default;
            this.date_of_birth = default;
        }

        public override string ToString()
        {
            return $"Id:{id} | Username:{user_name} | Fullname:{full_name} | Date of birth :{date_of_birth.ToShortDateString()} ";
        }
    }

    public class UserRepository
    {

        private NpgsqlConnection connection;
        public UserRepository(NpgsqlConnection connection)
        {
            this.connection = connection;
        }

        //returns the number of users in the database
        public long GetCount()
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM users";
            long count = (long)command.ExecuteScalar();
            connection.Close();
            return count;

        }

        //checks if the user exists by his ID (necessary for generation)
        public bool UserExists(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE id=@id";
            command.Parameters.AddWithValue("id", id);
            NpgsqlDataReader reader = command.ExecuteReader();
            bool result = reader.Read();
            connection.Close();
            return result;

        }


        //adds a new user to the database
        public int Insert(User user)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText =
            @" INSERT INTO users ( user_name, full_name,  date_of_birth )
                VALUES ( @user_name, @full_name, @date_of_birth) RETURNING id;
        ";
            command.Parameters.AddWithValue("user_name", user.user_name);
            command.Parameters.AddWithValue("full_name", user.full_name);
            command.Parameters.AddWithValue("date_of_birth", user.date_of_birth);
            int newId = (int)command.ExecuteScalar();
            connection.Close();
            return newId;
        }


        //finds the user in the database by his username  and returns it
        public User GetUser(string userName, string fullName)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE user_name = @user_name AND full_name=@full_name";
            command.Parameters.AddWithValue("user_name", userName);
            command.Parameters.AddWithValue("full_name", fullName);
            NpgsqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                User user = ParseUser(reader);
                connection.Close();
                return user;
            }
            reader.Close();
            connection.Close();
            return null;

        }


        //deletes the user by his ID
        public bool Delete(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM users WHERE id=@id";
            command.Parameters.AddWithValue("@id", id);
            int nChanges = command.ExecuteNonQuery();
            connection.Close();
            return nChanges == 1;
        }


        //updates the user
        public bool Update(User user, int userId)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE users SET user_name=@user_name , full_name=@full_name , date_of_birth=@date_of_birth WHERE id=@id";
            command.Parameters.AddWithValue("id", userId);
            command.Parameters.AddWithValue("user_name", user.user_name);
            command.Parameters.AddWithValue("full_name", user.full_name);
            command.Parameters.AddWithValue("date_of_birth", user.date_of_birth);
            int nChanged = command.ExecuteNonQuery();
            connection.Close();
            return nChanged == 1;

        }


        //parses user data that came from the database
        private User ParseUser(NpgsqlDataReader reader)
        {
            User user = new User();
            user.id = reader.GetInt32(0);
            user.user_name = reader.GetString(1);
            user.full_name = reader.GetString(2);
            user.date_of_birth = reader.GetDateTime(3);
            return user;
        }


        public void Generate(int numberOfUsers)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"INSERT into users (user_name, full_name, date_of_birth)
                SELECT chr(trunc(65+random()*25)::int) || chr(trunc(65+random()*25)::int) || chr(trunc(65+random()*25)::int),
                substr(md5(random()::text), 1, 8),
                date(timestamp '2000-01-01' + random() * (timestamp '2021-11-09' - timestamp'2000-01-01'))
                FROM generate_series(1, @numberOfUsers) ";
            command.Parameters.AddWithValue("numberOfUsers", numberOfUsers);

            int res = command.ExecuteNonQuery();
            connection.Close();
        }


        public User GetByUserId(int id)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE id = @id";
            command.Parameters.AddWithValue("id", id);
            NpgsqlDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                User user = ParseUser(reader);
                connection.Close();
                return user;

            }

            reader.Close();
            connection.Close();
            return null;

        }
    }






}