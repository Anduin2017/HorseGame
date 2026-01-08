using Gtk;

namespace HorseGame.Unified
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            var app = new Application("org.HorseGame.Unified", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new Windows.MainMenuWindow();
            app.AddWindow(win);

            win.ShowAll();
            Application.Run();
        }
    }
}
