using System;
using System.Collections.Generic;
using System.Text;

namespace ProtectEye.conf
{
    class Config
    {
        private string password = "我认怂";
        private decimal duration = 50;
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

        public decimal Duration
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
