using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Collections;
using Npgsql;

public class Folder
{
    [Key]
    public int id { get; set; }

    public string folder_name { get; set; }
    

    public int Userid {get; set;}
    public ICollection<Message> Messages { get; set; }


    public Folder()
    {
        this.id = default;
        this.Userid = default;
        this.folder_name = "";
        this.Messages= new HashSet<Message>();

    }

    public override string ToString()
    {
        return $"Id : {id} | Folder name: {folder_name} | User id: {Userid} ";
    }

}


public class FolderRepository
{
    private DatabaseContext db;
     private NpgsqlConnection connection;
    public FolderRepository(DatabaseContext databaseContext,   NpgsqlConnection connection)
    {
        this.db = databaseContext;
        this.connection=connection;
    }

    // adds a folder to the database
    public void Insert(Folder folder)
    {
        User user = db.users.Find(folder.Userid);
        if (user != null)
        {
            db.folders.Add(folder);
            user.Folders.Add(folder);
            db.SaveChanges();
        }
    }


    //returns a folder on its id
    public Folder GetByFolderId(int id)
    {

        Folder folder = db.folders.Find(id);
        return folder;

    }




    // updates the folder
    public bool Update(Folder folder, int folderId)
    {
        var newFolder = db.folders.Find(folderId);
        newFolder.Messages = folder.Messages;
        newFolder.folder_name = folder.folder_name;
        newFolder.Userid = folder.Userid;
        int res = db.SaveChanges();
        return res == 1;

    }

    //removes the folder by id
    public bool Delete(int folderId)
    {

        Folder folder = GetByFolderId(folderId);
        if (folder != null)
        {
            db.folders.Remove(folder);
            db.SaveChanges();
            return true;
        }
        return false;

    }

    public void DeleteAllByUserId(int userId)
    {
         var folders = db.folders.Where(p => p.Userid == userId);
         foreach (Folder folder in folders)
         {
             db.folders.Remove(folder);
         }
         db.SaveChanges();

    }


   public void Generate(int numberOfFolders)
    {
            Random random = new Random();
            int usersCount = db.users.Count();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuv";
            for (int i = 0; i < numberOfFolders; i++)
            {
                Folder folder = new Folder();
                var stringChars = new char[8];
                for (int j = 0; j < stringChars.Length; j++)
                {
                    stringChars[j] = chars[random.Next(chars.Length)];
                }

                var finalString = new String(stringChars);
                folder.folder_name=finalString;
                int userId = 0;
                do
                {
                    userId = random.Next(usersCount + 1);
                }
                while ( db.users.Find(userId) == null);

                folder.Userid = userId;
                db.folders.Add(folder);
                db.SaveChanges();
            }

    }

  
   

   public List<Folder> GetListOfFilteredFolders(string folder_name, string user_name)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM folders CROSS JOIN users WHERE folders.folder_name LIKE '%' || @folder_name || '%' AND users.user_name LIKE '%' || @user_name || '%' AND users.id=folders.UserId  ";
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

            // AND folders.Userid=users.id 
        }

    private Folder ParseFolderData(NpgsqlDataReader reader, Folder folder)
        {
            folder.id = reader.GetInt32(0);
            folder.Userid = reader.GetInt32(2);
            folder.folder_name = reader.GetString(1);
            return folder;
        }

}

public class Message
{
    [Key]
    public int id { get; set; }
    public int Userid {get; set;}

    public string message_subject { get; set; }


    public int Folderid {get; set;}


    public Message(int id, int user_id, int folder_id, string message_subject, DateTime sent_at)
    {
        this.id = id;
        this.Userid = user_id;
        this.Folderid = folder_id;
        this.message_subject = message_subject;
    }

    public Message()
    {
        this.id = default;
        this.Userid = default;
        this.Folderid = default;
        this.message_subject = "";
        
    }

    public override string ToString()
    {
        return $"Id : {id}  |  Message subject : {message_subject} ";
    }

}

public class MessageRepository
{
    private DatabaseContext db;
     private NpgsqlConnection connection;
    public MessageRepository(DatabaseContext databaseContext,  NpgsqlConnection connection)
    {
        this.db = databaseContext;
        this.connection=connection;
    }

    // adds a message to the database
    public void Insert(Message message)
    {
       
        User user = db.users.Find(message.Userid);
        Folder folder = db.folders.Find(message.Folderid);
        
        if (user != null && folder != null)
        {
            db.messages.Add(message);
            user.Messages.Add(message);
            folder.Messages.Add(message);
            db.SaveChanges();
        }

    }


    //returns a message on its id
    public Message GetByMessageId(int id)
    {
        Message message = db.messages.Find(id);
        return message;

    }
    

    // updates the message
    public bool Update(Message message, int messageId)
    {
        var newMessage = db.messages.Find(messageId);
        newMessage.message_subject = message.message_subject;
        newMessage.Userid = message.Userid;
        newMessage.Folderid = message.Folderid;
        int res = db.SaveChanges();
        return res == 1;
    }


    //removes the message by id
    public bool Delete(int messageId)
    {

        Message message = GetByMessageId(messageId);
        if (message != null)
        {
            db.messages.Remove(message);
            db.SaveChanges();
            return true;
        }
        return false;

    }

     public void DeleteAllByUserId(int userId)
    {
         var messages = db.messages.Where(p => p.Userid == userId);
         foreach (Message message in messages)
         {
             db.messages.Remove(message);
         }
         db.SaveChanges();

    }

     public void DeleteAllByFolderId(int folderId)
    {
         var messages = db.messages.Where(p => p.Folderid == folderId);
         foreach (Message message in messages)
         {
             db.messages.Remove(message);
         }
         db.SaveChanges();

    }

    public void Generate(int numberOfMessages)
    {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = $@"INSERT into messages ( message_subject, Userid, Folderid) 
            SELECT DISTINCT ON(Folderid)  substr ,  Userid, Folderid FROM   
            (SELECT  substr(md5(random()::text), 1, 8)  FROM generate_series(1, {numberOfMessages}) ) as result1,  
            (SELECT id as Userid FROM users ORDER BY random() LIMIT {numberOfMessages} ) as result2, 
            (SELECT  id as folderid FROM folders ORDER BY random() LIMIT {numberOfMessages} ) as result3";
            command.Parameters.AddWithValue("numberOfMessages", numberOfMessages);
            int res = command.ExecuteNonQuery();
            connection.Close();
    }

    public List<Message> GetListFilteredMessages(string user_name, int folderId)
        {

            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM messages CROSS JOIN users  WHERE  users.user_name LIKE '%' || @user_name || '%' AND Folderid=@folder_id  AND messages.Userid=users.id";
            command.Parameters.AddWithValue("user_name", user_name);
            command.Parameters.AddWithValue("folder_id", folderId);
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

        private Message ParseMessageData(NpgsqlDataReader reader, Message message)
        {

            message.id = reader.GetInt32(0);
            message.Userid = reader.GetInt32(2);
            message.message_subject = reader.GetString(1);
            message.Folderid = reader.GetInt32(3);
            return message;
        }

}

public class User
{
    [Key]
    public int id { get; set; }
    public string user_name { get; set; }

    public string full_name { get; set; }

    public ICollection<Folder> Folders { get; set; }
    public ICollection<Message> Messages { get; set; }


    public User()
    {
        this.id = default;
        this.user_name = "";
        this.full_name="";
        this.Folders= new HashSet<Folder> ();
        this.Messages= new HashSet<Message> ();
    }

    public override string ToString()
    {
        return $"Id:{id} | Username:{user_name} | Fullname:{full_name}  ";

    }
}

public class UserRepository
{
    private DatabaseContext db;
    private NpgsqlConnection connection;
    public UserRepository(DatabaseContext databaseContext, NpgsqlConnection connection)
    {
        this.db = databaseContext;
        this.connection=connection;
    }

    //adds a new user to the database
    public void Insert(User user)
    {
        db.users.Add(user);
        db.SaveChanges();
    }


    //deletes the user by his ID
    public bool Delete(int id)
    {

        User user = GetByUserId(id);
        if (user != null)
        {
            db.users.Remove(user);
            db.SaveChanges();
            return true;
        }
        return false;

    }


    //updates the user
    public bool Update(User user, int userId)
    {
        var newUser = db.users.Find(userId);
        newUser.user_name = user.user_name;
        newUser.full_name = user.full_name;
        int res = db.SaveChanges();
        return res == 1;
    }



    public User GetByUserId(int id)
    {
        User user = db.users.Find(id);
        return user;

    }

    public List<User> FindUser(string user_name, string full_name)
    {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE user_name LIKE '%' || @user_name || '%'  AND full_name LIKE '%' || @full_name || '%' ";
            command.Parameters.AddWithValue("user_name", user_name);
            command.Parameters.AddWithValue("full_name", full_name);
            NpgsqlDataReader reader = command.ExecuteReader();
             List<User> usersList = new List<User>();
            while (reader.Read())
            {
                User user = ParseUser(reader);
                usersList.Add(user);
            }
            reader.Close();
            connection.Close();
            return usersList;


    }

    private User ParseUser(NpgsqlDataReader reader)
        {
            User user = new User();
            user.id = reader.GetInt32(0);
            user.user_name = reader.GetString(1);
            user.full_name = reader.GetString(2);

            return user;
        }


   public void Generate(int numberOfUsers)
        {
            connection.Open();
            NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = @"INSERT into users (user_name, full_name)
                SELECT chr(trunc(65+random()*25)::int) || chr(trunc(65+random()*25)::int) || chr(trunc(65+random()*25)::int),
                substr(md5(random()::text), 1, 8)
                FROM generate_series(1, @numberOfUsers) ";
            command.Parameters.AddWithValue("numberOfUsers", numberOfUsers);

            int res = command.ExecuteNonQuery();
            connection.Close();
        }



}







