using System;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace LifeSim;
class Program
{
    [STAThread]
    static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Run the SkiaSharp rendering form
        Application.Run(new LifeRenderForm());
    }
}