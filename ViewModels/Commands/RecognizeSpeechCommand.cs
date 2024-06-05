using System;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.CognitiveServices.Speech;

namespace EvernoteClone.ViewModels.Commands
{
    internal class RecognizeSpeechCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public async void Execute(object? parameter)
        {
            if (parameter is RichTextBox rtb)
            {
                using SpeechRecognizer recognizer = new (
                    SpeechConfig.FromSubscription(
                        // TODO: Replace with Key Vault.
                        Environment.GetEnvironmentVariable("SpeechKey"),
                        Environment.GetEnvironmentVariable("SpeechRegion")
                    )
                );
                rtb.AppendText((await recognizer.RecognizeOnceAsync()).Text);
            }
        }
    }
}
