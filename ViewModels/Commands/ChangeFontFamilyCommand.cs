using System;
using System.Windows.Documents;
using System.Windows.Input;

namespace EvernoteClone.ViewModels.Commands
{
    internal class ChangeFontFamilyCommand : ICommand
    {
        private readonly Action<TextSelection> _changeFontFamily;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public ChangeFontFamilyCommand(Action<TextSelection> changeFontFamily)
        {
            _changeFontFamily = changeFontFamily;
        }

        public bool CanExecute(object? parameter)
        {
            return parameter is TextSelection selection && !string.IsNullOrEmpty(selection.Text);
        }

        public void Execute(object? parameter)
        {
            if (parameter is TextSelection selection)
            {
                _changeFontFamily(selection);
            }
        }
    }
}
