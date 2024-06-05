using System;
using System.Windows.Documents;
using System.Windows.Input;

namespace EvernoteClone.ViewModels.Commands
{
    internal class CountCharactersCommand : ICommand
    {
        private readonly Action<TextPointer, TextPointer> _countCharacters;

        public event EventHandler? CanExecuteChanged;

        public CountCharactersCommand(Action<TextPointer, TextPointer> countCharacters)
        {
            _countCharacters = countCharacters;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is object[] and [TextPointer start, TextPointer end])
            {
                _countCharacters(start, end);
            }
        }
    }
}
