using System;
using System.Windows.Documents;
using System.Windows.Input;

namespace EvernoteClone.ViewModels.Commands
{
    internal class UnderlineOrRevertCommand : ICommand
    {
        private readonly Action<TextSelection> _underlineOrRevert;

        public event EventHandler? CanExecuteChanged;

        public UnderlineOrRevertCommand(Action<TextSelection> underlineOrRevert)
        {
            _underlineOrRevert = underlineOrRevert;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is TextSelection selection)
            {
                _underlineOrRevert(selection);
            }
        }
    }
}
