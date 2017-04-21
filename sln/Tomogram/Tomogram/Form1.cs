using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Tomogram
{
    public partial class Form1 : Form
    {
        Bin bin = new Bin();
        View view = new View();
        bool loaded = false;
        int currentLayer = 0;
        int FrameCount = 0;
        bool needReload = false;
        DateTime NextFPSUpdate = DateTime.Now.AddSeconds(1);

        public Form1()
        {
            InitializeComponent();
        }
        
        //private void Form1_Load(object sender, EventArgs e)
        //{
        //    Application.Idle += Application_Idle;
        //}

        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if(dialog.ShowDialog() == DialogResult.OK)
            {
                string str = dialog.FileName;
                bin.readBin(str);
                view.SetupView(glControl1.Width, glControl1.Height);
                loaded = true;
                glControl1.Invalidate();
            }
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if(loaded)
            {
                if (radioButton1.Checked)
                {
                    view.DrowQuads(currentLayer);
                    glControl1.SwapBuffers();
                }
                else
                {
                    if (needReload)
                    {
                        view.generateTextureImage(currentLayer);
                        view.Load2DTexture();
                        needReload = false;
                    }
                    view.DrawTexture();
                    glControl1.SwapBuffers();
                }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            currentLayer = trackBar1.Value;
            displayFPS(); 
            glControl1.Invalidate();
            needReload = true;
        }

        //void Application_Idle(object sender, EventArgs e)
        //{
        //    while (glControl1.IsIdle)
        //    {
        //        displayFPS();
        //        glControl1.Invalidate();
        //    }
        //}

        void displayFPS()
        {
            if (DateTime.Now >= NextFPSUpdate)
            {
                this.Text = String.Format("CT Visualizer (fps = {0})", FrameCount);
                NextFPSUpdate = DateTime.Now.AddSeconds(1);
                FrameCount = 0;
            }
            FrameCount++;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
                radioButton1.Checked = false;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
                radioButton2.Checked = false;
        }
    }
}
