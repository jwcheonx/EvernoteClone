using System;
using System.Windows.Documents;
using System.Windows.Input;

namespace EvernoteClone.ViewModels.Commands
{
    internal class EvaluateFontControlsCommand : ICommand
    {
        private readonly Action<TextSelection> _evaluateFontControls;

        public event EventHandler? CanExecuteChanged;

        public EvaluateFontControlsCommand(Action<TextSelection> evaluateFontControls)
        {
            _evaluateFontControls = evaluateFontControls;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            if (parameter is TextSelection selection)
            {
                _evaluateFontControls(selection);
            }
        }
    }
}
