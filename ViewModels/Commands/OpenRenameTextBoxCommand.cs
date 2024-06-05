using System;
using System.Windows.Input;

namespace EvernoteClone.ViewModels.Commands
{
    internal class OpenRenameTextBoxCommand : ICommand
    {
        private readonly Action _openRenameTextBox;

        public event EventHandler? CanExecuteChanged;

        public OpenRenameTextBoxCommand(Action openRenameTextBox)
        {
            _openRenameTextBox = openRenameTextBox;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _openRenameTextBox();
        }
    }
}
