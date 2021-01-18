using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lab3SpaceInvaders
{
    class Invader
    {
        #region Properties and fields

        private const int HorizontalInterval = 10;
        private const int VerticalInterval = 40;

        private Bitmap image;
        private bool alive = true;
        private DateTime deadTimerBegin;
        private DateTime deadTimerEnd;
        private TimeSpan span1;
        private int framesSkipped = 0;
        public Point Location { get; private set; }
        public ShipType InvaderType { get; private set; }
        public Rectangle Area
        {
            get
            {
                return new Rectangle(Location, image.Size);
            }
        }
        public int Score { get; private set; }
        #endregion

        #region Constructor
        public Invader(ShipType invaderType, Point location, int score)
        {
            this.InvaderType = invaderType;
            this.Location = location;
            this.Score = score;
            image = InvaderImage(0);
        }
        #endregion

        #region Move(Direction direction)
        /// <summary>
        /// Checks if alive. If it is, moves in the direction parameter
        /// </summary>
        /// <param name="direction">invader direction</param>
        public void Move(Direction direction)
        {
            if (!alive)
            {
                return;
            }
            if (direction == Direction.Down)
            {
                Location = new Point(Location.X, Location.Y + VerticalInterval);
            }
            else if (direction == Direction.Left)
            {
                Location = new Point(Location.X - HorizontalInterval, Location.Y);
            }
            else if (direction == Direction.Right)
            {
                Location = new Point(Location.X + HorizontalInterval, Location.Y);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region InvaderHit()
        /// <summary>
        /// Starts the dead timer. Sets alive to false.
        /// </summary>
        public void InvaderHit()
        {
            if (deadTimerBegin.Year != DateTime.Now.Year)
                deadTimerBegin = DateTime.Now;
            alive = false;
        }
        #endregion

        #region Alive()
        /// <summary>
        /// Returns alive
        /// </summary>
        /// <returns>alive</returns>
        public bool Alive()
        {
            return alive;
        }
        #endregion

        #region Draw(Graphics g, int animationCell)
        /// <summary>
        /// If it's alive, draw the invader. If not, draw the explosion.
        /// </summary>
        /// <param name="g">graphics</param>
        /// <param name="animationCell">animation cell</param>
        public void Draw(Graphics g, int animationCell)
        {
            if (alive)
                g.DrawImage(InvaderImage(animationCell), Location);
            else
            {
                g.DrawImage(ExplosionImage(0), Location);
            }
        }
        #endregion

        #region ExplosionImage(int animationCell)
        /// <summary>
        /// Returns the correct explosion picture
        /// </summary>
        /// <param name="animationCell">animation cell</param>
        /// <returns></returns>
        private Bitmap ExplosionImage(int animationCell)
        {
            framesSkipped++;
            if (framesSkipped == 2)
            {
                animationCell = 1;
            }
            if (animationCell == 0)
            {
                return Properties.Resources.ExplosionPart2;
            }
            else
            {
                return Properties.Resources.ExplosionPart1;
            }
        }
        #endregion

        #region TimeDead()
        /// <summary>
        /// Sets deadTimerEnd to now. Sets span1 to deadTimerEnd - deadTimerBegin. 
        /// </summary>
        /// <returns>span1</returns>
        public TimeSpan TimeDead()
        {
            deadTimerEnd = DateTime.Now;
            span1 = deadTimerEnd - deadTimerBegin;
            return span1;
        }
        #endregion

        #region InvaderImage(int animationCell)
        /// <summary>
        /// Returns the correct invader image.
        /// </summary>
        /// <param name="animationCell"></param>
        /// <returns></returns>
        private Bitmap InvaderImage(int animationCell)
        {
            switch (InvaderType) {
                case ShipType.invader1:
                    if (animationCell == 0)
                    {
                        return Properties.Resources.Invader1Part1;
                    }
                    else
                    {
                        return Properties.Resources.Invader1Part2;
                    }
                case ShipType.invader2:
                    if (animationCell == 0)
                    {
                        return Properties.Resources.Invader2Part1;
                    }
                    else
                    {
                        return Properties.Resources.Invader2Part2;
                    }
                case ShipType.invader3:
                    if (animationCell == 0)
                    {
                        return Properties.Resources.Invader3Part1;
                    }
                    else
                    {
                        return Properties.Resources.Invader3Part2;
                    }
                case ShipType.invader4:
                    if (animationCell == 0)
                    {
                        return Properties.Resources.Invader4Part1;
                    }
                    else
                    {
                        return Properties.Resources.Invader4Part2;
                    }
                case ShipType.invader5:
                    if (animationCell == 0)
                    {
                        return Properties.Resources.Invader5Part1;
                    }
                    else
                    {
                        return Properties.Resources.Invader5Part2;
                    }
            }
            throw new NotImplementedException();
        }
        #endregion
    }
}
