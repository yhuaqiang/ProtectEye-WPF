using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtectEye.hook
{
    public class Hooker
    {
        private static Hooker hooker = new Hooker();
        MouseHook mouseHook = new MouseHook();


        public static Hooker GetInstance() {
            return hooker; 
        }
        private void StartMouseHook()
        {
            this.mouseHook.SetHook();
            this.mouseHook.MouseMoveEvent += (sender, e) =>
            {
                Console.WriteLine("mouse hook: x: {0}, y: {1}", e.Location.X, e.Location.Y);
            };
        }
        private void StopMouseHook()
        {
            this.mouseHook.UnHook();
        }
        private void StartKeyHook()
        {

        }
        private void StopKeyHook()
        {

        }
        public void StartHook()
        {
            Console.WriteLine("start hook");
            this.StartMouseHook();
            this.StartKeyHook();
        }

        public void StopHook()
        {
            Console.WriteLine("stop hook");
            this.StopMouseHook();
            this.StopKeyHook();
        }
    }
}
