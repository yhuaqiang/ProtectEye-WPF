using System;
using System.Collections.Generic;
using System.Text;

namespace ProtectEye.conf
{
    public class Config
    {
        private string password = "我认怂";
        private int duration = 30; // 30分钟
        private int locking = 5;// 5分钟
        private bool desktop = true;
        private bool autoStart = false;

        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
            }
        }

        public int Duration
        {
            get
            {
                return this.duration;
            }
            set
            {
                this.duration = value;
            }
        }

        public int Locking
        {
            get
            {
                return this.locking;
            }
            set
            {
                this.locking = value;
            }
        }

        public bool IsShowDesktop
        {
            get
            {
                return this.desktop;
            }
            set
            {
                this.desktop = value;
            }
        }

        public bool IsAutoStart
        {
            get
            {
                return this.autoStart;
            }
            set
            {
                this.autoStart = value;
            }
        }

    }
}
