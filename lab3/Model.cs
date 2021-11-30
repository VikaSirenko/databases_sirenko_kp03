using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

public class Folder
{
    [Key]
    public int Id { get; set; }

    public string Folder_name { get; set; }
    public DateTime Date_of_creation { get; set; }

    public int User_id;
    public ICollection<Message> Messages { get; set; }


    public Folder(int id, int user_id, string folder_name, DateTime date_of_creation)
    {
        this.Id = id;
        this.User_id = user_id;
        this.Folder_name = folder_name;
        this.Date_of_creation = date_of_creation;
    }

    public Folder()
    {
        this.Id = default;
        this.User_id = default;
        this.Folder_name = default;
        this.Date_of_creation = default;

    }

    public override string ToString()
    {
        return $"Id : {Id} | Folder name: {Folder_name} | User id: {User_id} | Date of creation : {Date_of_creation.ToShortDateString()}";
    }

}


public class FolderRepository
{
    private DatabaseContext db;
    public FolderRepository(DatabaseContext databaseContext)
    {
        this.db = databaseContext;
    }

    // adds a folder to the database
    public void Insert(Folder folder)
    {
        User user = db.users.Find(folder.User_id);
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
        newFolder.Folder_name = folder.Folder_name;
        newFolder.Date_of_creation = folder.Date_of_creation;
        newFolder.User_id = folder.User_id;
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
        ICollection<Folder> folders = db.folders.Where(p => p.user_id == userId);
        foreach (Folder folder in folders)
        {
            db.folders.Remove(folder);
        }
        db.SaveChanges();

    }

}

public class Message
{
    [Key]
    public int Id { get; set; }
    public int User_id;

    public string Message_subject { get; set; }

    public DateTime Sent_at { get; set; }

    public int Folder_id;


    public Message(int id, int user_id, int folder_id, string message_subject, DateTime sent_at)
    {
        this.Id = id;
        this.User_id = user_id;
        this.Folder_id = folder_id;
        this.Message_subject = message_subject;
        this.Sent_at = sent_at;
    }

    public Message()
    {
        this.Id = default;
        this.User_id = default;
        this.Folder_id = default;
        this.Message_subject = default;
        this.Sent_at = default;
    }

    public override string ToString()
    {
        return $"Id : {Id}  |  Message subject : {Message_subject} | Sent at : {Sent_at.ToShortDateString()}";
    }

}

public class MessageRepository
{
    private DatabaseContext db;
    public MessageRepository(DatabaseContext databaseContext)
    {
        this.db = databaseContext;
    }

    // adds a message to the database
    public void Insert(Message message)
    {
        User user = db.users.Find(message.User_id);
        Folder folder = db.folders.Find(message.Folder_id);
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
        newMessage.Message_subject = message.Message_subject;
        newMessage.Sent_at = message.Sent_at;
        newMessage.User_id = message.User_id;
        newMessage.Folder_id = message.Folder_id;
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

}

public class User
{
    [Key]
    public int Id { get; set; }
    public string User_name { get; set; }

    public string Full_name { get; set; }

    public DateTime Date_of_birth { get; set; }
    public ICollection<Folder> Folders { get; set; }
    public ICollection<Message> Messages { get; set; }


    public User(int id, string user_name, string full_name, DateTime date_of_birth)
    {
        this.Id = id;
        this.User_name = user_name;
        this.Full_name = full_name;
        this.Date_of_birth = date_of_birth;
    }

    public User()
    {
        this.Id = default;
        this.User_name = default;
        this.Full_name = default;
        this.Date_of_birth = default;
    }

    public override string ToString()
    {
        return $"Id:{Id} | Username:{User_name} | Fullname:{Full_name} | Date of birth :{Date_of_birth.ToShortDateString()} ";
    }
}

public class UserRepository
{
    private DatabaseContext db;
    public UserRepository(DatabaseContext databaseContext)
    {
        this.db = databaseContext;
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
        newUser.User_name = user.User_name;
        newUser.Full_name = user.Full_name;
        newUser.Date_of_birth = user.Date_of_birth;
        int res = db.SaveChanges();
        return res == 1;
    }



    public User GetByUserId(int id)
    {
        User user = db.users.Find(id);
        return user;

    }

}







