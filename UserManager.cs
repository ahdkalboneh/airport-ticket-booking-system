public class UserManager
{
    private readonly JsonFileStorage<User> Storage;
    private Dictionary<string, User> Users;

    public User? CurrentUser { get; private set; }

    public UserManager(
        JsonFileStorage<User> storage,
        Dictionary<string, User> users)
    {
        Storage = storage;
        Users = users;
    }

    public async Task<bool> Register(string username, string password)
    {
        username = username.Trim();

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        if (Users.ContainsKey(username))
            return false;

        int newId = Users.Count == 0 ? 1 : Users.Values.Max(user => user.UserId) + 1;
        User newUser = new User(newId, username, password, Role.Passenger);

        Users.Add(username, newUser);

        await Storage.WriteDictionary(Users);

        CurrentUser = newUser;

        return true;
    }


    public bool Login(string username, string password)
    {
        username = username.Trim();

        if (string.IsNullOrWhiteSpace(username) ||
            string.IsNullOrWhiteSpace(password))
            return false;

        if (!Users.TryGetValue(username, out User? user))
            return false;

        if (user.Password != password)
            return false;

        CurrentUser = user;

        return true;
    }
    public void Logout()
    {
        CurrentUser = null;
    }
}