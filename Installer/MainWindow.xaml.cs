using Microsoft.Win32;
using System.Windows;
using System;
using System.Diagnostics;
using Octokit;
using System.Windows.Controls;

namespace VRCTC_Installer
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
    
        public string vrcPath = "";
        public string installTag = "";

        public MainWindow()
        {
            InitializeComponent();
        }

      
    }
}
