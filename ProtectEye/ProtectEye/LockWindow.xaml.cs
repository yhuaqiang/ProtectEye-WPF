﻿using System;
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

namespace ProtectEye
{
    /// <summary>
    /// LockWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LockWindow : Window
    {
        private DispatcherTimer timer;
        private int waitDuration = 10;
        private int tmpWaitDuration = 0;

        public DoMonitor doMonitor;

        public LockWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Init();
            this.InitTimer();
        }

        private void Init()
        {
            this.WindowState = System.Windows.WindowState.Maximized;
        }
        private void InitTimer()
        {
            this.timer = new DispatcherTimer();
            this.timer.Interval = TimeSpan.FromSeconds(1);
            this.timer.Tick += (sender, e) =>
            {
                this.WaitForClose();
            };
        }
        private void WaitForClose()
        {
            Console.WriteLine("{0} sencods to close", this.tmpWaitDuration);
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
            this.Show();
            Console.WriteLine("lock");
            this.tmpWaitDuration = this.waitDuration;
            this.WaitForClose();
            this.timer.Start();
        }

        public void Unlock()
        {
            Console.WriteLine("unlock");
            this.timer.Stop();
            this.Hide();
            //
            this.doMonitor();
        }
    }
}