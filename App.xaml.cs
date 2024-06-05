using EvernoteClone.ViewModels.Helpers;

namespace EvernoteClone
{
    internal partial class App
    {
        public App()
        {
            DatabaseHelper.InitializeDatabase();
        }
    }
}
