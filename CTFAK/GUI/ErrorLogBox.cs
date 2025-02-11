﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using CTFAK.Utils;

namespace CTFAK.GUI
{
    public partial class ErrorLogBox : Form
    {
        public ErrorLogBox(Exception e)
        {
            InitializeComponent();
            textBox1.Text += $"{e.Message}\r\n\r\n\r\n";
            StackTrace st = new StackTrace(true);

            for(int i =0; i< st.FrameCount; i++ )
            {
                StackFrame sf = st.GetFrame(i);
                var filename = Path.GetFileNameWithoutExtension(sf.GetFileName());
                if (filename == null) filename = "UnknownFile";
                textBox1.Text +=
                    $" {(filename)} : {sf.GetMethod()}: Line {sf.GetFileLineNumber()}\r\n\r\n";
            }
            Logger.Log("ERROR: ",false,ConsoleColor.White,false);
            Logger.Log(textBox1.Text,false,ConsoleColor.White,false);

            //Console.ReadKey();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Logger.Log("Exiting because of exception");
            Environment.Exit(-1);
        }
    }
}