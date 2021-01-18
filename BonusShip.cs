using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
namespace Lab3SpaceInvaders
{
    class BonusShip
    {

        #region Properties and fields
        private const int HorizontalInterval = 5;
        private const int VerticalInterval = 30;
        private bool alive = true;
        private DateTime deadTimerBegin;
        private DateTime deadTimerEnd;
        private TimeSpan span1;
        private int framesSkipped = 0;
        public Point Location { get; private set; }
        private Point startingLocation;
        public int score = 500;
        public Bitmap image;
        public Rectangle Area
        {
            get
            {
                return new Rectangle(Location, new Size(image.Size.Width, image.Size.Height));
            }
        }
        #endregion

        #region Constructor
        public BonusShip(Point Location)
        {
            this.Location = Location;
            image = Properties.Resources.BonusShip;
            startingLocation = Location;
        }
        #endregion

        #region BonusShipHit()
        /// <summary>
        /// Starts the dead timer. Sets alive to false.
        /// </summary>
        public void BonusShipHit()
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

        #region Draw(Graphics g)
        /// <summary>
        /// Draws the BonusShip
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            if (alive)
                g.DrawImage(image, Location);
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

        #region Move(Direction direction)
        /// <summary>
        /// Moves the BonusShip
        /// </summary>
        /// <param name="direction"></param>
        public void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Left:
                    Location = new Point(Location.X - HorizontalInterval, Location.Y);
                    break;
                case Direction.Right:
                    Location = new Point(Location.X + HorizontalInterval, Location.Y);
                    break;
                case Direction.Up:
                    Location = new Point(Location.X, Location.Y - VerticalInterval);
                    break;
                case Direction.Down:
                    Location = new Point(Location.X, Location.Y + VerticalInterval);
                    break;
            }
        }
        #endregion

        #region Direction FindDirection()
        /// <summary>
        /// Assumes the ClientRectangle.Width is above 1000
        /// </summary>
        /// <returns>Left if on right side, right if on left side</returns>
        public Direction FindDirection()
        {
            if (Math.Abs(startingLocation.X - 0) > Math.Abs(startingLocation.X - 1920))
            {
                return Direction.Left;
            }
            else
            {
                return Direction.Right;
            }
        }
        #endregion
    }
}
