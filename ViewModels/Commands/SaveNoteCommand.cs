using System;
using System.Windows.Documents;
using System.Windows.Input;
using EvernoteClone.Models;

namespace EvernoteClone.ViewModels.Commands
{
    internal class SaveNoteCommand : ICommand
    {
        private readonly Action<Note, TextPointer, TextPointer> _saveNote;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public SaveNoteCommand(Action<Note, TextPointer, TextPointer> saveNote)
        {
            _saveNote = saveNote;
        }

        public bool CanExecute(object? parameter)
        {
            return parameter is object?[] and [Note, ..];
        }

        public void Execute(object? parameter)
        {
            if (parameter is object[] and [Note note, TextPointer start, TextPointer end])
            {
                _saveNote(note, start, end);
            }
        }
    }
}
