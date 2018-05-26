using ProtectEye.conf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ProtectEye.hook;
using ProtectEye.util;
using System.Reflection;

namespace ProtectEye
{
    /// <summary>
    /// LockWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LockWindow : Window
    {
        private Config config;
        private DispatcherTimer timer;
        private int waitDuration = 0;
        private int tmpWaitDuration = 0;

        public Utils.DoMonitor doMonitor;

        public LockWindow()
        {
            InitializeComponent();
        }

        public LockWindow(Config config)
        {
            InitializeComponent();
            //
            this.config = config;
            //
            this.Init();
            this.InitTimer();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        
        }

        private void Init()
        {
            Console.WriteLine("lockwindow init...");
            this.FullScreen();
            this.tbPassword.Visibility = Visibility.Hidden;
            this.btnUnlock.Width = 0;
            this.MouseDoubleClick += (sender, e) =>
            {
                this.toggleUnlock();
            };
            this.KeyUp += (sender, e) =>
            {
                if (e.Key == Key.Enter || e.Key == Key.Escape)
                {
                    this.toggleUnlock();
                }
            };
            this.Closing += (sender, e) =>
            {
                e.Cancel = true;
            };
        }

        private void toggleUnlock()
        {
            this.tbPassword.Visibility = this.tbPassword.IsVisible ? Visibility.Hidden : Visibility.Visible;
            this.tbPassword.Focus();
        }

        private void FullScreen()
        {
            this.ShowInTaskbar = false;
            this.Topmost = true;
            this.WindowState = WindowState.Normal;
            this.WindowStyle = WindowStyle.None;
            this.ResizeMode = ResizeMode.NoResize;
            this.Left = this.Top = 0;
            this.Width = SystemParameters.PrimaryScreenWidth;
            this.Height = SystemParameters.PrimaryScreenHeight;
            this.Activate();
            this.Focus();
        }
        private void InitTimer()
        {
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(1);
            this.timer.Tick += (sender, e) =>
            {
                this.RefreshCountDown();
            };
        }
        private void RefreshCountDown()
        {
            // 总是全屏/置顶
            this.FullScreen();
            //Console.WriteLine("{0} sencods to close", this.tmpWaitDuration);
            int m = this.tmpWaitDuration / 60;
            int s = this.tmpWaitDuration % 60;
            this.lblCountDown.Content = string.Format("{0:00}:{1:00}", m, s);
            if (--this.tmpWaitDuration < 0)
            {
                this.Unlock();
            }
        }
        public void Lock()
        {
            try
            {
                Console.WriteLine("lock");
                // 显示桌面
                if (this.config.IsShowDesktop)
                {
                    this.ToggleDesktop(true);
                }

                this.waitDuration = this.config.Locking * 60;
                this.tmpWaitDuration = this.waitDuration;
                this.RefreshCountDown();
                this.timer.Start();
                this.tbPassword.Visibility = Visibility.Hidden;
                this.Show();
                //
                Hooker.GetInstance().StartHook();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
        public void Unlock()
        {
            try
            {
                Console.WriteLine("unlock");
                this.Hide();
                this.tbPassword.Clear();
                this.timer.Stop();

                if (this.config.IsShowDesktop)
                {
                    this.ToggleDesktop(false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Hooker.GetInstance().StopHook();
            }
            try
            {
                //调用委托,执行父窗口的事件
                this.doMonitor();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }
        private void btnUnlock_Click(object sender, RoutedEventArgs e)
        {
            string pswd = this.tbPassword.Text.Trim();
            if (pswd == this.config.Password)
            {
                this.Unlock();
            }
            else
            {
                Console.WriteLine("password incorrect");
            }
        }
        private void ToggleDesktop(bool show)
        {
            Type oleType = Type.GetTypeFromProgID("Shell.Application");
            object oleObject = Activator.CreateInstance(oleType);
            oleType.InvokeMember("ToggleDesktop", BindingFlags.InvokeMethod, null, oleObject, null);
        }
    }
}
