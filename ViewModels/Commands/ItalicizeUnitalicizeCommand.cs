using System;
using System.Windows.Documents;
using System.Windows.Input;

namespace EvernoteClone.ViewModels.Commands
{
    internal class ItalicizeUnitalicizeCommand : ICommand
    {
        private readonly Action<TextSelection> _italicizeUnitalicize;

        public event EventHandler? CanExecuteChanged;

        public ItalicizeUnitalicizeCommand(Action<TextSelection> italicizeUnitalicize)
        {
            _italicizeUnitalicize = italicizeUnitalicize;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is TextSelection selection)
            {
                _italicizeUnitalicize(selection);
            }
        }
    }
}
