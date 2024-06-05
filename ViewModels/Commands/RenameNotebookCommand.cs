using System;
using System.Windows.Input;

namespace EvernoteClone.ViewModels.Commands
{
    internal class RenameNotebookCommand : ICommand
    {
        private readonly Action<int, string> _renameNotebook;

        public event EventHandler? CanExecuteChanged;

        public RenameNotebookCommand(Action<int, string> renameNotebook)
        {
            _renameNotebook = renameNotebook;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is object[] and [int id, string newTitle])
            {
                _renameNotebook(id, newTitle);
            }
        }
    }
}
