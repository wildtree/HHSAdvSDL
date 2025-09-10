using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using SDL2;
using HHSAdvSDL;

class Program
{
    public static void Main(string[] args)
    {
        ZSystem system = ZSystem.Instance;
        system.Init();
        system.Loop();
        system.Quit();
    }
}