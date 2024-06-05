using System;
using System.Windows.Input;
using EvernoteClone.Models;

namespace EvernoteClone.ViewModels.Commands
{
    internal class DeleteNoteCommand : ICommand
    {
        private readonly Action<Note> _deleteNote;

        public event EventHandler? CanExecuteChanged;

        public DeleteNoteCommand(Action<Note> deleteNote)
        {
            _deleteNote = deleteNote;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is Note note)
            {
                _deleteNote(note);
            }
        }
    }
}
