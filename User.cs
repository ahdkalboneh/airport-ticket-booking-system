
public enum Role {Passenger, Manager}
public class User
{
    public int UserId {get; private set;} 
    public string Name {get; private set;}
    public string Password {get; private set;}
    public Role Role {get; private set;}

    public User(int userId, string name, string password, Role role)
    {
        UserId = userId;
        Name = name;
        Password = password;
        Role = role;
    }
}