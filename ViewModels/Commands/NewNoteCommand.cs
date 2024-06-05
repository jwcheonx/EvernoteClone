using System;
using System.Windows.Input;

namespace EvernoteClone.ViewModels.Commands
{
    internal class NewNoteCommand : ICommand
    {
        private readonly Action<int> _addNote;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public NewNoteCommand(Action<int> addNote)
        {
            _addNote = addNote;
        }

        public bool CanExecute(object? parameter)
        {
            return parameter is int;
        }

        public void Execute(object? parameter)
        {
            if (parameter is int notebookId)
            {
                _addNote(notebookId);
            }
        }
    }
}
