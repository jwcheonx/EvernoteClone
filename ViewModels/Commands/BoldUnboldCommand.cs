using System;
using System.Windows.Documents;
using System.Windows.Input;

namespace EvernoteClone.ViewModels.Commands
{
    internal class BoldUnboldCommand : ICommand
    {
        private readonly Action<TextSelection> _boldUnbold;

        public event EventHandler? CanExecuteChanged;

        public BoldUnboldCommand(Action<TextSelection> boldUnbold)
        {
            _boldUnbold = boldUnbold;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is TextSelection selection)
            {
                _boldUnbold(selection);
            }
        }
    }
}
