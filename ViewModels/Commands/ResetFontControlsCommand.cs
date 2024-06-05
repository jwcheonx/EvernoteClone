using System;
using System.Windows.Input;

namespace EvernoteClone.ViewModels.Commands
{
    internal class ResetFontControlsCommand : ICommand
    {
        private readonly Action _resetFontControls;

        public event EventHandler? CanExecuteChanged;

        public ResetFontControlsCommand(Action resetFontControls)
        {
            _resetFontControls = resetFontControls;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            _resetFontControls();
        }
    }
}
