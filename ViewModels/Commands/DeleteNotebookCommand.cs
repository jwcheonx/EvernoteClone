using System;
using System.Windows.Input;
using EvernoteClone.Models;

namespace EvernoteClone.ViewModels.Commands
{
    internal class DeleteNotebookCommand : ICommand
    {
        private readonly Action<Notebook> _deleteNotebook;

        public event EventHandler? CanExecuteChanged;

        public DeleteNotebookCommand(Action<Notebook> deleteNotebook)
        {
            _deleteNotebook = deleteNotebook;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is Notebook notebook)
            {
                _deleteNotebook(notebook);
            }
        }
    }
}
