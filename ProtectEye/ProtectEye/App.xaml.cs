using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ProtectEye
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Application.Current.DispatcherUnhandledException += (sender, e) =>
            {
                e.Handled = true;
                System.IO.File.AppendAllText("logs.txt", "exception occurs: =============>\n" + e.Exception.StackTrace, Encoding.UTF8);

            };
        }
    }
}
