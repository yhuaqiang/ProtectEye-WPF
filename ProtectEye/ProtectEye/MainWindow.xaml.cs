﻿using ProtectEye.conf;
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

namespace ProtectEye
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Forms.NotifyIcon notifyIcon;

        private Config config;

        public Config Config { get { return this.config; } }

        public MainWindow()
        {
            InitializeComponent();


        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.InitNofityIcon();
            this.Init();
        }

        private void Init()
        {
            this.config = ConfigHelper.Load();
            this.ShowInTaskbar = false;
            this.lblPassword.Content = string.Format("当前密码: {0}", this.config.Password);
            this.lblDuration.Content = string.Format("当前间隔: {0}", this.config.Duration);
            this.sldDuration.Value = Convert.ToDouble(this.config.Duration);
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
                this.notifyIcon.Visible = false;
                this.Close();
            };
            menu.MenuItems.Add(settingItem);
            menu.MenuItems.Add(exitItem);
            this.notifyIcon.ContextMenu = menu;
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
            this.Hide();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void checkbox_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == this.cbPassword)
            {
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




    }
}
