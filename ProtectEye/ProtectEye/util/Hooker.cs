using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtectEye.util
{
   public class Hooker
    {
       public static void StartHook()
       {
           Console.WriteLine("start hook");
       }

       public static void StopHook()
       {
           Console.WriteLine("stop hook");
       }
    }
}
