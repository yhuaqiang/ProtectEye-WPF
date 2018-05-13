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
using ProtectEye.util;

namespace ProtectEye
{
    /// <summary>
    /// LockWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LockWindow : Window
    {
        private Config config;
        private DispatcherTimer timer;
        private int waitDuration = 600; // 秒
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
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Init();
            this.InitTimer();
        }

        private void Init()
        {
            this.FullScreen();
            this.tbPassword.Visibility = Visibility.Hidden;
            this.btnUnlock.Width = 0;
            this.MouseDoubleClick += (sender, e) =>
            {
                this.tbPassword.Visibility = this.tbPassword.IsVisible ? Visibility.Hidden : Visibility.Visible;
                this.tbPassword.Focus();
            };
            this.Closing += (sender, e) =>
            {
                e.Cancel = true;
            };
        }

        private void FullScreen()
        {
            this.ShowInTaskbar = false;
            this.Topmost = true;
            this.WindowState = WindowState.Maximized;
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
                //this.Unlock();
            }
        }
        public void Lock()
        {
            try
            {
                Console.WriteLine("lock");
                this.Show();
                this.tmpWaitDuration = this.waitDuration;
                this.RefreshCountDown();
                this.timer.Start();
                this.tbPassword.Focus();
                //
                Hooker.StartHook();
            }
            catch (Exception ex)
            {
            }
        }
        public void Unlock()
        {
            try
            {
                Console.WriteLine("unlock");
                this.tbPassword.Clear();
                this.timer.Stop();
                this.Hide();
            }
            finally
            {
                Hooker.StopHook();
            }
            try
            {
                //调用委托,执行父窗口的事件
                this.doMonitor();
            }
            catch (Exception ex)
            {

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
    }
}
