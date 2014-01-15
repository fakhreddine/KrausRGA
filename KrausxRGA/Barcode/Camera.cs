using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using WindowsInput;

namespace KrausRGA.Barcode
{
   public static class Camera
    {
       public static void TakePhoto()
       {
           try
           {
               InputSimulator.SimulateKeyPress(VirtualKeyCode.LWIN);
               Thread.Sleep(600);
               InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_C);
               InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_A);
               InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_M);
               InputSimulator.SimulateKeyPress(VirtualKeyCode.VK_E);
               Thread.Sleep(600);
               InputSimulator.SimulateKeyPress(VirtualKeyCode.RETURN);
               Thread.Sleep(3000);
               InputSimulator.SimulateKeyPress(VirtualKeyCode.SPACE);
               Thread.Sleep(600);
               InputSimulator.SimulateKeyDown(VirtualKeyCode.LWIN);
               Thread.Sleep(600);
               InputSimulator.SimulateKeyPress(VirtualKeyCode.TAB);
               Thread.Sleep(600);
               InputSimulator.SimulateKeyUp(VirtualKeyCode.LWIN);
           }
           catch (Exception)
           { }
       }
       
    }
}
