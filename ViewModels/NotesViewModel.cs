using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using EvernoteClone.Models;
using EvernoteClone.ViewModels.Commands;
using EvernoteClone.ViewModels.Helpers;

namespace EvernoteClone.ViewModels
{
    internal class NotesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Notebook> Notebooks { get; } = [];

        private int? _selectedNotebookId;
        public int? SelectedNotebookId
        {
            get => _selectedNotebookId;
            set
            {
                if (_selectedNotebookId == value) return;

                Notes.Clear();

                if ((_selectedNotebookId = value) is int notebookId &&
                    DatabaseHelper.GetNotes(notebookId) is List<Note> notes)
                {
                    foreach (Note note in notes)
                    {
                        Notes.Add(note);
                    }
                }

                RaisePropertyChanged();
            }
        }

        public ObservableCollection<Note> Notes { get; } = [];

        private Visibility _renameTextBoxVisibility = Visibility.Collapsed;
        public Visibility RenameTextBoxVisibility
        {
            get => _renameTextBoxVisibility;
            private set
            {
                _renameTextBoxVisibility = value;
                RaisePropertyChanged();
            }
        }

        private bool _isBoldButtonChecked;
        public bool IsBoldButtonChecked
        {
            get => _isBoldButtonChecked;
            set
            {
                _isBoldButtonChecked = value;
                RaisePropertyChanged();
            }
        }

        private bool _isItalicButtonChecked;
        public bool IsItalicButtonChecked
        {
            get => _isItalicButtonChecked;
            set
            {
                _isItalicButtonChecked = value;
                RaisePropertyChanged();
            }
        }

        private bool _isUnderlineButtonChecked;
        public bool IsUnderlineButtonChecked
        {
            get => _isUnderlineButtonChecked;
            set
            {
                _isUnderlineButtonChecked = value;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<FontFamily> FontFamilies { get; } =
            Fonts.SystemFontFamilies.OrderBy(fontFamily => fontFamily.Source);

        private object? _selectedFontFamily = DependencyProperty.UnsetValue;
        public object? SelectedFontFamily
        {
            get => _selectedFontFamily;
            set
            {
                _selectedFontFamily = value;
                RaisePropertyChanged();
            }
        }

        public IEnumerable<double> FontSizes { get; } =
            [8, 9, 10, 10.5, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72];

        private object? _selectedFontSize = DependencyProperty.UnsetValue;
        public object? SelectedFontSize
        {
            get => _selectedFontSize;
            set
            {
                _selectedFontSize = value;
                RaisePropertyChanged();
            }
        }

        private int _characterCount;
        public int CharacterCount
        {
            get => _characterCount;
            private set
            {
                _characterCount = value;
                RaisePropertyChanged();
            }
        }

        public ICommand NewNotebookCommand { get; }
        public ICommand RenameNotebookCommand { get; }
        public ICommand OpenRenameTextBoxCommand { get; }
        public ICommand DeleteNotebookCommand { get; }

        public ICommand NewNoteCommand { get; }
        public ICommand DeleteNoteCommand { get; }

        public ICommand LoadNoteCommand { get; } = new LoadNoteCommand((filename, start, end) =>
        {
            using FileStream fs = File.Open(filename, FileMode.Open);
            new TextRange(start, end).Load(fs, DataFormats.Rtf);
        });

        public ICommand SaveNoteCommand { get; } = new SaveNoteCommand((note, start, end) =>
        {
            if (DatabaseHelper.UpdateNote(note) && !string.IsNullOrEmpty(note.Filename))
            {
                using FileStream fs = File.Create(note.Filename);
                new TextRange(start, end).Save(fs, DataFormats.Rtf);
            }
        });

        public ICommand BoldUnboldCommand { get; }
        public ICommand ItalicizeUnitalicizeCommand { get; }
        public ICommand UnderlineOrRevertCommand { get; }
        public ICommand ChangeFontFamilyCommand { get; }

        public ICommand EvaluateFontControlsCommand { get; }
        public ICommand ResetFontControlsCommand { get; }

        public ICommand CountCharactersCommand { get; }

        public ICommand RecognizeSpeechCommand { get; } = new RecognizeSpeechCommand();
        public ICommand ExitCommand { get; } = new ExitCommand();

        public event PropertyChangedEventHandler? PropertyChanged;

        public NotesViewModel()
        {
            NewNotebookCommand = new NewNotebookCommand(() =>
            {
                if (DatabaseHelper.AddNotebook() is Notebook notebook)
                {
                    Notebooks.Add(notebook);
                }
            });

            RenameNotebookCommand = new RenameNotebookCommand((id, newTitle) =>
            {
                RenameTextBoxVisibility = Visibility.Collapsed;
                DatabaseHelper.RenameNotebook(id, newTitle);
            });

            OpenRenameTextBoxCommand = new OpenRenameTextBoxCommand(() =>
            {
                RenameTextBoxVisibility = Visibility.Visible;
            });

            DeleteNotebookCommand = new DeleteNotebookCommand(notebook =>
            {
                if (DatabaseHelper.DeleteNotebook(notebook.Id))
                {
                    string dirname = Path.Combine(
                        DatabaseHelper.DatabaseDirname,
                        $"rtf\\{notebook.Id}"
                    );
                    if (Directory.Exists(dirname))
                    {
                        Directory.Delete(dirname, true);
                    }

                    Notebooks.Remove(notebook); // Sets SelectedNotebookId to null.
                }
            });

            NewNoteCommand = new NewNoteCommand(notebookId =>
            {
                if (DatabaseHelper.AddNote(notebookId) is Note note)
                {
                    // TODO: Insert the new note at the front.
                    Notes.Add(note);
                }
            });

            DeleteNoteCommand = new DeleteNoteCommand(note =>
            {
                if (DatabaseHelper.DeleteNote(note.Id))
                {
                    if (File.Exists(note.Filename))
                    {
                        // TODO: Handle an exception when the file is in use.
                        File.Delete(note.Filename);
                    }

                    Notes.Remove(note);
                }
            });

            BoldUnboldCommand = new BoldUnboldCommand(selection =>
            {
                selection.ApplyPropertyValue(
                    Control.FontWeightProperty,
                    IsBoldButtonChecked ? FontWeights.Bold : FontWeights.Normal
                );
            });

            ItalicizeUnitalicizeCommand = new ItalicizeUnitalicizeCommand(selection =>
            {
                selection.ApplyPropertyValue(
                    Control.FontStyleProperty,
                    IsItalicButtonChecked ? FontStyles.Italic : FontStyles.Normal
                );
            });

            UnderlineOrRevertCommand = new UnderlineOrRevertCommand(selection =>
            {
                if (IsUnderlineButtonChecked)
                {
                    selection.ApplyPropertyValue(
                        Inline.TextDecorationsProperty,
                        TextDecorations.Underline
                    );
                }
                else if (selection.GetPropertyValue(Inline.TextDecorationsProperty) is
                             TextDecorationCollection decorations &&
                         decorations.TryRemove(
                             TextDecorations.Underline,
                             out TextDecorationCollection reverted))
                {
                    selection.ApplyPropertyValue(Inline.TextDecorationsProperty, reverted);
                }
            });

            ChangeFontFamilyCommand = new ChangeFontFamilyCommand(selection =>
            {
                if (SelectedFontFamily is FontFamily fontFamily)
                {
                    selection.ApplyPropertyValue(Control.FontFamilyProperty, fontFamily);
                }
            });

            EvaluateFontControlsCommand = new EvaluateFontControlsCommand(selection =>
            {
                IsBoldButtonChecked = selection.GetPropertyValue(Control.FontWeightProperty) is
                    FontWeight fontWeight && fontWeight == FontWeights.Bold;

                IsItalicButtonChecked = selection.GetPropertyValue(Control.FontStyleProperty) is
                    FontStyle fontStyle && fontStyle == FontStyles.Italic;

                // FIXME
                IsUnderlineButtonChecked =
                    selection.GetPropertyValue(Inline.TextDecorationsProperty) is
                        TextDecorationCollection decorations &&
                    decorations == TextDecorations.Underline;

                SelectedFontFamily = selection.GetPropertyValue(Control.FontFamilyProperty);

                SelectedFontSize = selection.GetPropertyValue(Control.FontSizeProperty);
            });

            ResetFontControlsCommand = new ResetFontControlsCommand(() =>
            {
                IsBoldButtonChecked = false;
                IsItalicButtonChecked = false;
                IsUnderlineButtonChecked = false;

                SelectedFontFamily = DependencyProperty.UnsetValue;
                SelectedFontSize = DependencyProperty.UnsetValue;
            });

            CountCharactersCommand = new CountCharactersCommand((start, end) =>
            {
                CharacterCount = new TextRange(start, end).Text.Trim().Length;
            });

            if (DatabaseHelper.GetNotebooks() is List<Notebook> notebooks)
            {
                foreach (Notebook notebook in notebooks)
                {
                    Notebooks.Add(notebook);
                }
            }
        }

        private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
