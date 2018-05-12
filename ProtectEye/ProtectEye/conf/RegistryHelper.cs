using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ProtectEye.conf
{
    class RegistryHelper
    {
        public static void SetAutoStart(bool autostart)
        {
            RegistryKey aaKey =
                Registry.CurrentUser
                            .CreateSubKey("Software")
                            .CreateSubKey("microsoft")
                            .CreateSubKey("windows")
                            .CreateSubKey("currentversion")
                            .CreateSubKey("run");
            if (autostart)
            {
                aaKey.SetValue("ProtectEye", Application.ExecutablePath);
            }
            else
            {
                aaKey.DeleteValue("ProtectEye");
            }
        }
    }
}
