﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {

        public int X { get; set; }
        public int Y { get; set; }
        private bool direction;
        private int time1;
        private Scene scene;
        private bool pressed;
        private bool isStarted = false;
        private bool stop;
        private SoundPlayer fly;

        public Form1()
        {
            InitializeComponent();
            X = Width;
            Y = Height;
            direction = true;
            scene = new Scene(Width, Height);
            stop = false;
            fly = new SoundPlayer(WindowsFormsApplication1.Properties.Resources.Up);
            
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //   Dead(e.Graphics);
            //      scene.ShouldDie(e.Graphics);
            if (scene.ShouldDie())
            {
                stop = true;
                Dead();
            }         
            scene.DrawPipe(e.Graphics);
            scene.DrawPowerUp(e.Graphics);
            scene.DrawBird(e.Graphics);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = time1;
            direction = false;
            if (time1 >= 30)
                time1 -= 5;
            timer2.Enabled = false;
            scene.MoveBird(direction);
            Invalidate();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            direction = true;
            scene.MoveBird(direction);
            Invalidate();
            timer1.Enabled = true;
            time1 = 65;
            timer3.Enabled = PowerUps.timerEnabled;
        }

        private void Dead()
        {
            timer2.Enabled = timer3.Enabled = timer4.Enabled = false;

            if (scene.stopTimer)
            {
                timer1.Enabled = false;
                MessageBox.Show("Umre");
            }

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            scene.MovePowerUp();
            scene.Intersect();
            Invalidate();            
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            scene.MovePipe();
            scene.Check();
            if (scene.Neso())
            {
                int x = Int32.Parse(label1.Text) + 1;
                label1.Text = x.ToString();
            }
            Invalidate();            
        }


        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (stop) return;
            if (e.KeyChar == '+' && !pressed)
            {
                fly.Play();
                if (!isStarted)
                { 
                    timer4.Start();
                    isStarted = true;
                }
                       
                timer1.Enabled = false;
                timer2.Enabled = true;
                time1 = 65;
                pressed = !scene.SuperMan();
            }
            
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            pressed = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
