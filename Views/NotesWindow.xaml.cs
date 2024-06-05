using System.Windows.Controls;

namespace EvernoteClone.Views
{
    internal partial class NotesWindow
    {
        public NotesWindow()
        {
            InitializeComponent();
        }

        private void FontSizeComboBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            if (NoteContentTextBox.Selection.Text == string.Empty ||
                ((ComboBox) sender).SelectedItem is null)
            {
                return;
            }

            NoteContentTextBox.Selection.ApplyPropertyValue(
                FontSizeProperty,
                ((ComboBox) sender).Text
            );
        }
    }
}
