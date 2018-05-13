using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace ProtectEye.hook
{
    class MouseHook
    {
        //委托+事件（把钩到的消息封装为事件，由调用者处理）
        public delegate void MouseMoveHandler(object sender, MouseEventArgs e);
        public event MouseMoveHandler MouseMoveEvent;


        private Point point;
        private Point Point
        {
            get { return this.point; }
            set
            {
                if (this.point != value)
                {
                    this.point = value;
                    if (MouseMoveEvent != null)
                    {
                        var e = new MouseEventArgs(MouseButtons.None, 0, Convert.ToInt32(point.X), Convert.ToInt32(point.Y), 0);
                        MouseMoveEvent(this, e);
                    }
                }
            }
        }

        private int hHook;
        public const int WH_MOUSE_LL = 14;
        public Win32Api.HookProc hProc;
        public MouseHook()
        {
            this.point = new Point();
        }

        public int SetHook()
        {
            this.hProc = new Win32Api.HookProc(MouseHookProc);
            this.hHook = Win32Api.SetWindowsHookEx(WH_MOUSE_LL, hProc, IntPtr.Zero, 0);
            return hHook;
        }

        public void UnHook()
        {
            Win32Api.UnhookWindowsHookEx(hHook);
        }

        private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            Win32Api.MouseHookStruct mouseHookStruct =
                (Win32Api.MouseHookStruct)Marshal.PtrToStructure(lParam, typeof(Win32Api.MouseHookStruct));
            if (nCode >= 0)
            {
                this.Point = new Point(mouseHookStruct.pt.x, mouseHookStruct.pt.y);
            }
            return Win32Api.CallNextHookEx(hHook, nCode, wParam, lParam);
        }
    }
}
