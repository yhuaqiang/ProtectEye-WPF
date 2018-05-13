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

    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Forms.NotifyIcon notifyIcon;

        private DispatcherTimer timerMonitor = new DispatcherTimer();
        private DispatcherTimer timerAutoClick = new DispatcherTimer();

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
            this.InitMonitorTimer();
            this.InitAutoClickTimer();
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
            // checkbox事件
            this.cbPassword.Checked += this.checkbox_Checked;
            this.cbPassword.Unchecked += this.checkbox_Checked;
            this.cbDuration.Checked += this.checkbox_Checked;
            this.cbDuration.Unchecked += this.checkbox_Checked;
            this.cbDesktop.Checked += this.checkbox_Checked;
            this.cbDesktop.Unchecked += this.checkbox_Checked;
            this.cbAutoStart.Checked += this.checkbox_Checked;
            this.cbAutoStart.Unchecked += this.checkbox_Checked;
            //向子窗口传递委托
            this.lockWindow = new LockWindow(this.config);
            this.lockWindow.doMonitor = () =>
            {
                this.StartMonitor();
            };
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
        private void InitMonitorTimer()
        {
            this.timerMonitor.Tick += (sender, e) =>
            {
                // 
                this.StopMonitor();
                //
                this.lockWindow.Lock();
                //
            };

        }
        private void InitAutoClickTimer()
        {
            int autoClickDuration = 5;//5秒

            timerAutoClick.Interval = TimeSpan.FromSeconds(1);
            timerAutoClick.Tick += (sender, e) =>
            {
                if (autoClickDuration > 0)
                {
                    this.btnStart.Content = String.Format("开始({0})", autoClickDuration--);
                }
                else
                {
                    this.StopAutoClick();
                    this.doStart();
                }
            };
            timerAutoClick.Start();
        }
        private void HideNotifyIcon()
        {
            this.notifyIcon.Visible = false;
        }
       private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void doStart()
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
        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            this.doStart();
        }

        private void StartMonitor()
        {
            this.StopMonitor();
            Console.WriteLine("start monitor");
            this.notifyIcon.ShowBalloonTip(5000, "Y(^_^)Y", string.Format("{0}钟之后要休息下~", this.config.Duration), Forms.ToolTipIcon.Info);
            this.timerMonitor.Interval = TimeSpan.FromMinutes(Convert.ToDouble(this.config.Duration));
            this.timerMonitor.Interval = TimeSpan.FromSeconds(3f);
            this.timerMonitor.Start();
        }

        private void StopMonitor()
        {
            Console.WriteLine("stop monitor");
            this.timerMonitor.Stop();
        }
        private void StopAutoClick(){
            this.timerAutoClick.Stop();
            this.btnStart.Content = "开始";
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void checkbox_Checked(object sender, RoutedEventArgs e)
        {
            // 有点击是就停止自动点击计时器
            this.StopAutoClick();
            //
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
