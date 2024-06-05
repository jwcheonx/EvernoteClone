namespace EvernoteClone.Models
{
    internal class User
    {
        public int Id { get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; }
        public string Password { get; set; }

        public User(int id, string firstName, string lastName, string username, string password)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            Username = username;
            Password = password;
        }
    }
}
