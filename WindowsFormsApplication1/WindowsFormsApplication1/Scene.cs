﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;

namespace AeraAvis
{
    class Scene
    {
        private Bird bird;
        private List<Image> poweredUpBird;
        private PowerUps powerUp;
        public int height = 300;      
        public int h;
        public readonly int MAX_HEIGHT = 607;
        public readonly int MAX_WIDTH = 470;
        public List<Pipe> pipes;
        private Random r;
        public bool stopTimer;
        private SoundPlayer grujo;
        private int score;
        private List<string> scores;
        private bool dead = false;

        public Scene(int width, int height)
        {
            bird = new Bird(width, height);
            powerUp = new PowerUps(width, height);
            poweredUpBird = new List<Image>();
            pipes = new List<Pipe>();
            stopTimer = false;
            grujo = new SoundPlayer(Properties.Resources.Grujo);
            score = 0;
            Init();
        }

        /// <summary>
        /// initialize the powwerUps list
        /// </summary>
        /// <returns></returns>
        private void Init()
        {
            poweredUpBird.Add(Properties.Resources.PowerUp1);
            poweredUpBird.Add(Properties.Resources.ActorSuper);
            poweredUpBird.Add(Properties.Resources.PowerUp3);
            Image tmp = AeraAvis.Properties.Resources.ActorNormalRes;
            poweredUpBird.Add(tmp);
            poweredUpBird.Add(tmp);
            r = new Random();
            PipesGeneration();
        }

        public void DrawBird(Graphics g)
        {
            bird.Fly(g);
        }

        /// <summary>
        /// method to move the bird up and down
        /// </summary>
        /// <param name="dir">flying direction</param>
        /// NOTE:
        /// true is up
        /// false is down
        /// <returns></returns>
        public void MoveBird(bool dir)
        {
            if (Reverse() && bird.intersect)
                dir = !dir;
            bird.Move(dir);
        }

        public void MovePowerUp()
        {
            powerUp.Move();
        }

        public void DrawPowerUp(Graphics g)
        {
            powerUp.Draw(g);
        }

        private Image PickPowerUp(int index)
        {
            switch (index)
            {
                case 3:
                    bird.SetSize(new Size(80, 80));
                    break;
                case 4:
                    bird.SetSize(new Size(30, 30));
                    break;
            }
            return poweredUpBird[index];
        }

        /// <summary>
        /// method to check if the bird hit the pipes or reached the borders
        /// </summary>
        /// <returns></returns>
        public void Intersect()
        {
            if (bird.GetPosition().IntersectsWith(powerUp.getPosition()))
            {
                powerUp.getPower();
                bird.PowerUp(PickPowerUp(powerUp.getIndex()));
            }

            if (Grujo())
            {
                grujo.PlayLooping();
                PipesGeneration(3);
            }
        }

        private void ReadAndWriteScore()
        {
            if (!dead)
            {
                scores = File.ReadAllLines("Scores.txt").ToList();
                scores.Add(score.ToString());
                scores.Sort();
                scores.RemoveAt(0);
                File.WriteAllLines("Scores.txt", scores);
            }
        }

        /// <summary>
        /// method to check if the bird should die
        /// </summary>
        /// <returns></returns>
        public bool ShouldDie()
        {
            bool flag = false;
            foreach (Pipe p in pipes)
            {
                Point point = new Point(bird.GetPoint().X + 5, bird.GetPoint().Y + 9);
                Size size = new Size(bird.GetSize().Width - 10, bird.GetSize().Height - 17);

                Rectangle c = new Rectangle(point, size);

                if (c.IntersectsWith(p.r) || bird.GetPoint().Y + bird.GetSize().Height <= 0 || bird.GetPoint().Y + bird.GetSize().Height >= MAX_HEIGHT)
                {
                    ReadAndWriteScore();
                    dead = true;
                    flag = true;
                    break;
                }

            }

            if (bird.GetPoint().Y + bird.GetSize().Height >= MAX_HEIGHT)
            {
                flag = stopTimer = true;
            }
            return flag;
        }

        private bool Reverse()
        {
            return powerUp.CheckReversed();
        }

        private bool Grujo()
        {
            return bird.intersect && powerUp.getIndex() == 0;
        }

        /// <summary>
        /// check if superman power up was picked up
        /// </summary>
        /// <returns></returns>
        public bool SuperMan()
        {
            return bird.intersect && powerUp.getIndex() == 1;
        }

        public int getY()
        {
            return bird.GetSize().Height;
        }

        public void MovePipe()
        {
            foreach (Pipe p in pipes)
            {
                p.Move();
            }
            if (pipes.Last().Position.X < 400)
            {
                PipesGeneration(2);
            }

        }

        /// <summary>
        /// method to generate pipes
        /// <param name="t">type of pipe that is created</param>
        /// NOTE:
        /// 1 on start
        /// 2 for regeneration
        /// 3 for Grujo pipe generation
        /// </summary>
        /// <returns></returns>
        public void PipesGeneration(int t = 0)
        {
           int vel = 190;
           if(t == 3)
            {
                pipes.Clear();
                vel = 400;
            }

            for (int i = 0; i < 5; i++)
            {
                if(t == 3)
                {
                    h = r.Next(40, 70);
                    vel = 450;
                }

                else
                {
                    h = r.Next(150, 350);
                }
              
                int x;
                if (t == 2)
                {
                    x = 550 + i * 150;
                }              
                else 
                {
                    x = 400 + i * 150;
                }

                Pipe p0 = new Pipe(new Point(x + 19, 40), h, 0);
                Pipe p1 = new Pipe(new Point(x, 0), 40, 1);
                Pipe p2 = new Pipe(new Point(x + 19, h + vel), MAX_HEIGHT - (h + vel), 2);
                Pipe p3 = new Pipe(new Point(x, MAX_HEIGHT - 30), 30, 3);
                pipes.Add(p1);
                pipes.Add(p2);
                pipes.Add(p0);
                pipes.Add(p3);
            }

        }

        public void DrawPipe(Graphics g)
        {
            foreach (Pipe p in pipes)
                p.Draw(g);
        }

        public void Check()
        {
            for (int i = pipes.Count - 1; i >= 0; i--)
            {
                if (pipes[i].Position.X < -90)
                {
                    pipes.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// method to check if bird passed the pipe
        /// </summary>
        /// <param name="count">current number of passed pipes</param>
        /// <returns></returns>
        public string PipePassed(string count)
        {
            score = Int32.Parse(count);
            foreach (Pipe p in pipes)
            {
                int x1 = p.Position.X + 25;
                int x2 = bird.GetPoint().X;
                if (x1 == x2)
                {
                    score++;
                    break;
                }
            }
            return score.ToString();
        }
    }
}
