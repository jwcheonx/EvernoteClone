using System;
using System.IO;
using System.Windows.Documents;
using System.Windows.Input;

namespace EvernoteClone.ViewModels.Commands
{
    internal class LoadNoteCommand : ICommand
    {
        private readonly Action<string, TextPointer, TextPointer> _loadNote;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public LoadNoteCommand(Action<string, TextPointer, TextPointer> loadNote)
        {
            _loadNote = loadNote;
        }

        public bool CanExecute(object? parameter)
        {
            if (parameter is object?[] and [BlockCollection blocks, ..])
            {
                blocks.Clear();
            }

            return parameter is object[] and [_, string filename, ..] && File.Exists(filename);
        }

        public void Execute(object? parameter)
        {
            if (parameter is object[] and [_, string filename, TextPointer start, TextPointer end])
            {
                _loadNote(filename, start, end);
            }
        }
    }
}
