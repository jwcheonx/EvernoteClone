using System;
using System.Windows.Input;

namespace EvernoteClone.ViewModels.Commands
{
    internal class NewNotebookCommand : ICommand
    {
        private readonly Action _addNotebook;

        public event EventHandler? CanExecuteChanged;

        public NewNotebookCommand(Action addNotebook)
        {
            _addNotebook = addNotebook;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _addNotebook();
        }
    }
}
