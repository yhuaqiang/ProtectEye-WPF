using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ProtectEye.conf
{
    class ConfigHelper
    {
        private static string FILE_NAME = Environment.CurrentDirectory + @"\conf.ini";
        private const string SECTION_NAME = "setting";

        [DllImport("kernel32")] // 写入配置文件的接口(使用byte[]可以解决中文编码问题)
        //private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        private static extern long WritePrivateProfileString(byte[] section, byte[] key, byte[] val, string filePath);

        [DllImport("kernel32")] // 读取配置文件的接口
        //private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);
        private static extern int GetPrivateProfileString(byte[] section, byte[] key, byte[] def, byte[] retVal, int size, string filePath);

        private static byte[] getBytes(string s)
        {
            return s == null ? null : Encoding.GetEncoding("utf-8").GetBytes(s);

        }
        // 向配置文件写入值
        public static void ProfileWriteValue(string key, string value)
        {
            long res = WritePrivateProfileString(getBytes(SECTION_NAME), getBytes(key), getBytes(value), FILE_NAME);
            Console.WriteLine(String.Format("set {0} = {1}, result: {2}", key, value, res));
        }
        // 读取配置文件的值
        public static string ProfileReadValue(string key)
        {
            int size = 1024;
            byte[] buffer = new byte[size];
            int count = GetPrivateProfileString(getBytes(SECTION_NAME), getBytes(key), getBytes(""), buffer, size, FILE_NAME);
            return Encoding.GetEncoding("utf-8").GetString(buffer, 0, count).Trim();
        }

        private static void CheckConfigFile()
        {
            if (!File.Exists(FILE_NAME))
            {
                new FileStream(FILE_NAME, FileMode.OpenOrCreate);
            }
        }
        public static Config Load()
        {
            CheckConfigFile();
            //
            Config c = new Config();
            string val;
            try
            {
                val = ProfileReadValue("password");
                if (!String.IsNullOrEmpty(val))
                {
                    c.Password = val;
                }
            }
            catch (Exception)
            {
            }
            try
            {
                val = ProfileReadValue("duration");
                if (!String.IsNullOrEmpty(val))
                {
                    c.Duration = Convert.ToDecimal(val);
                }
            }
            catch (Exception)
            {
            }
            try
            {
                val = ProfileReadValue("desktop");
                if (!String.IsNullOrEmpty(val))
                {
                    c.IsShowDesktop = Convert.ToBoolean(val);
                }
            }
            catch (Exception)
            {
            }
            try
            {
                val = ProfileReadValue("autostart");
                if (!String.IsNullOrEmpty(val))
                {
                    c.IsAutoStart = Convert.ToBoolean(val);
                }
            }
            catch (Exception)
            {
            }

            return c;
        }

        public static void Save(Config config)
        {
            CheckConfigFile();
            //
            ProfileWriteValue("password", config.Password);
            ProfileWriteValue("duration", config.Duration.ToString());
            ProfileWriteValue("desktop", config.IsShowDesktop.ToString());
            ProfileWriteValue("autostart", config.IsAutoStart.ToString());

            RegistryHelper.SetAutoStart(config.IsAutoStart);
        }

    }
}
