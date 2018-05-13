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
using Forms = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ProtectEye
{

    // 通过委托实现消息传递,注意位置
    public delegate void DoMonitor();
    
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Forms.NotifyIcon notifyIcon;

        private DispatcherTimer timer;

        private Config config;

        private LockWindow lockWindow;


        public MainWindow()
        {
            InitializeComponent();


        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Init();
            this.InitNofityIcon();
            this.InitTimer();
        }
        private void InitConfig()
        {
            this.cbPassword.IsChecked = false;
            this.tbPassword.IsEnabled = (bool)this.cbPassword.IsChecked;
            this.lblPassword.Content = string.Format("当前密码: {0}", this.config.Password);
            this.lblDuration.Content = string.Format("当前间隔: {0}", this.config.Duration);
            this.sldDuration.Value = Convert.ToDouble(this.config.Duration);
            this.cbDesktop.IsChecked = this.config.IsShowDesktop;
            this.cbAutoStart.IsChecked = this.config.IsAutoStart;
        }
        private void Init()
        {
            this.Topmost = true;
            this.ShowInTaskbar = false;
            //
            this.config = ConfigHelper.Load();
            this.InitConfig();
            
            //事件
            this.Closing += (sender, e) =>
            {
                Console.WriteLine("main window closing");
                e.Cancel = true;
                this.Hide();
            };
            //向子窗口传递委托
            this.lockWindow = new LockWindow(this.config);
            this.lockWindow.doMonitor = () =>
            {
                this.StartMonitor();
            };
        }

        private void InitNofityIcon()
        {
            this.notifyIcon = new Forms.NotifyIcon();
            this.notifyIcon.Visible = true;
            this.notifyIcon.Icon = Properties.Resources.System;
            this.notifyIcon.MouseClick += (sender, e) =>
            {
                if (e.Button == Forms.MouseButtons.Left)
                {
                    this.Visibility = this.IsVisible ? Visibility.Hidden : Visibility.Visible;
                }
            };
            Forms.ContextMenu menu = new Forms.ContextMenu();
            Forms.MenuItem settingItem = new Forms.MenuItem();
            settingItem.Text = "设置...";
            settingItem.Click += (sender, e) =>
            {
                this.Visibility = Visibility.Visible;
            };
            Forms.MenuItem exitItem = new Forms.MenuItem();
            exitItem.Text = "退出";
            exitItem.Click += (sender, e) =>
            {
                this.Exit();                
            };
            menu.MenuItems.Add(settingItem);
            menu.MenuItems.Add(exitItem);
            this.notifyIcon.ContextMenu = menu;
        }

        private void HideNotifyIcon()
        {
            this.notifyIcon.Visible = false;
        }
        private void InitTimer()
        {
            this.timer = new DispatcherTimer();
            this.timer.Tick += (sender, e) =>
            {
                // 
                this.StopMonitor();
                //
                this.lockWindow.Lock();
                //

            };

        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (true == this.cbPassword.IsChecked)
            {
                string pswd = this.tbPassword.Text.Trim();
                if (string.IsNullOrEmpty(pswd))
                {
                    this.tbPassword.Focus();
                    return;
                }
                this.config.Password = pswd;
            }
            this.config.Duration = Convert.ToInt32(this.sldDuration.Value);
            this.config.IsShowDesktop = (bool)this.cbDesktop.IsChecked;
            this.config.IsAutoStart = (bool)this.cbAutoStart.IsChecked;
            ConfigHelper.Save(this.config);
            //
            this.InitConfig();
            this.Hide();
            this.StartMonitor();
        }

        private void StartMonitor()
        {
            this.StopMonitor();
            Console.WriteLine("start monitor");
            this.notifyIcon.ShowBalloonTip(5000, "Y(^_^)Y", string.Format("{0}钟之后要休息下~", this.config.Duration), Forms.ToolTipIcon.Info);
            this.timer.Interval = TimeSpan.FromMinutes(Convert.ToDouble(this.config.Duration));
            this.timer.Interval = TimeSpan.FromSeconds(5f);
            //this.timer.Start();
        }

        private void StopMonitor()
        {
            Console.WriteLine("stop monitor");
            this.timer.Stop();
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void checkbox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == this.cbPassword)
            {
                this.tbPassword.IsEnabled = (bool)this.cbPassword.IsChecked;
                if (true == this.cbPassword.IsChecked)
                {
                    this.tbPassword.Focus();
                }
                else
                {
                    this.tbPassword.Clear();
                }
            }
            else if (sender == this.cbDuration)
            {
                if (false == this.cbDuration.IsChecked)
                {
                    this.sldDuration.Value = Convert.ToDouble(this.config.Duration);
                }
            }
        }


        private void Exit()
        {
            this.HideNotifyIcon();
            System.Environment.Exit(System.Environment.ExitCode);
        }

    }
}
