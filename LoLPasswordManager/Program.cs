﻿using System;
using System.Windows.Forms;

namespace LoLPasswordManager
{
    static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      Application.EnableVisualStyles();
      //Application.SetHighDpiMode(HighDpiMode.SystemAware);
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new mainForm());
    }
  }
}
