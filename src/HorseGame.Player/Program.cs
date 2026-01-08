using System;
using Gtk;

namespace HorseGame.Player
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            var app = new Application("org.HorseGame.Player", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            var win = new MainWindow();
            app.AddWindow(win);

            win.ShowAll();
            Application.Run();
        }
    }
}
