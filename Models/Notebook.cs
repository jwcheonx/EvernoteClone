namespace EvernoteClone.Models
{
    internal class Notebook
    {
        public int Id { get; }
        public int? UserId { get; }
        public string Title { get; set; }

        public Notebook(int id, int? userId, string title)
        {
            Id = id;
            UserId = userId;
            Title = title;
        }
    }
}
