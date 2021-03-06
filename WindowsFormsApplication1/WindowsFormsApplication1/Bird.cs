﻿using System.Drawing;
using System.Drawing.Drawing2D;
using System.Timers;

namespace AeraAvis
{
    class Bird
    {
        private Image birdImage;
        private Image currentImage;
        private Rectangle position;
        private Point point;
        private Size size;
        private int birdX;
        private int birdY;
        public int diff;
        private int angle;
        public bool intersect;
        Timer timer;

        public Bird(int x, int y)
        {
            birdImage = Properties.Resources.ActorNormalRes;
            size = new Size(50, 50);
            birdX = x / 2 - 50 / 2;
            birdY = y / 2 - 50 / 2;
            point = new Point(birdX, birdY);
            position = new Rectangle(point, size);
            angle = 0;
            currentImage = birdImage;
            intersect = false;
            timer = new Timer(7500);
            timer.Elapsed += Start;

        }

        private void Start(object sender, ElapsedEventArgs e)
        {
            intersect = false;
            size = new Size(50, 50);
            currentImage = Properties.Resources.ActorNormalRes;
            birdImage = currentImage;
            timer.Stop();
           // throw new NotImplementedException();
        }

        public void SetSize(Size newSIze)
        {
            size = newSIze;
        }

        public Point GetPoint()
        {
            return point;
        }

        public Size GetSize()
        {
            return size;
        }

        /// <summary>
        /// method to rotate an image either clockwise or counter-clockwise
        /// </summary>
        /// <param name="img">the image to be rotated</param>
        /// <param name="rotationAngle">the angle (in degrees).
        /// NOTE: 
        /// Positive values will rotate clockwise
        /// negative values will rotate counter-clockwise
        /// </param>
        /// <returns></returns>
        public Image RotateImage(Image img, float rotationAngle)
        {
            //create an empty Bitmap image
            Bitmap bmp = new Bitmap(img.Width, img.Height);
            Rectangle rec = new Rectangle(0, 0, img.Width, img.Height);

            //turn the Bitmap into a Graphics object
            Graphics gfx = Graphics.FromImage(bmp);

            //now we set the rotation point to the center of our image
            gfx.TranslateTransform((float)bmp.Width / 2, (float)bmp.Height / 2);

            //now rotate the image
            gfx.RotateTransform(rotationAngle);

            gfx.TranslateTransform(-(float)bmp.Width / 2, -(float)bmp.Height / 2);

            //set the InterpolationMode to HighQualityBicubic so to ensure a high
            //quality image once it is transformed to the specified size

            gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //now draw our new image onto the graphics object
            gfx.DrawImage(img, rec);

            //dispose of our Graphics object
            gfx.Dispose();

            //return the image
            return bmp;
        }

        private void Rotate(int degrees)
        {
            if(angle <= 90)
            birdImage = RotateImage(birdImage, degrees);
        }

        private void MoveUp()
        {
            birdY -= 11;

            birdImage = currentImage;
            Rotate(-30);

            diff = 0;
            angle = 0;
        }

        private void MoveDown()
        {
            birdY += 8;
            angle += 10;
            Rotate(10);
        }

       
        public void Move(bool direction)
        {
            if (direction) MoveUp();
            else MoveDown();
            point = new Point(birdX, birdY);
            position = new Rectangle(point, size);
        }

        public void Fly(Graphics g)
        {
            g.DrawImage(birdImage, position);
        }

        public Rectangle GetPosition()
        {
            return position;
        }

        public void PowerUp(Image img)
        {
            timer.Start();
            currentImage = img;
            birdImage = currentImage;
            intersect = true;
           // await Task.Delay(TimeSpan.FromSeconds(7));
        }
        
                
         
    }
}
