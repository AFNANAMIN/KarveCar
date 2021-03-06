﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NLog;

namespace KarveControlsTest
{
    /// <summary>
    /// Interaction logic for TestWindow.xaml
    /// </summary>
    public partial class TestWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public TestWindow()
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            InitializeComponent();
            w.Stop();
            long e = w.ElapsedMilliseconds;
            logger.Debug("Value in ms " + e);
        }
    }
}
