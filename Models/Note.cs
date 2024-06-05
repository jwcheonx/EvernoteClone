using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EvernoteClone.Models
{
    internal class Note : INotifyPropertyChanged
    {
        public int Id { get; }
        public int NotebookId { get; }
        public string Title { get; set; }
        public long CreatedAt { get; }
        public string? Filename { get; set; }

        private long _updatedAt;
        public long UpdatedAt
        {
            get => _updatedAt;
            set
            {
                _updatedAt = value;
                RaisePropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public Note(int id, int notebookId, string title, long createdAt, long updatedAt,
            string? filename = null)
        {
            Id = id;
            NotebookId = notebookId;
            Title = title;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            Filename = filename;
        }

        private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
