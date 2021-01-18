using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lab3SpaceInvaders
{
    class PlayerShip
    {
        #region Properties and field
        private int moveInterval;
        private DateTime crushTimer;
        private DateTime currentTimer;
        private TimeSpan span1;
        private Bitmap image;
        private int deadShipHeight;
        private int timesDead = 0;
        public Point Location { get; private set; }
        public bool Alive { get; private set; }
        public Rectangle Area
        {
            get
            {
                return new Rectangle(Location, new Size(image.Size.Width, image.Size.Height));
            }
        }
        #endregion

        #region Constructor
        public PlayerShip (Point Location)
        {
            this.Location = Location;
            Alive = true;
            image = Properties.Resources.PlayerShip;
            deadShipHeight = Area.Height;
        }
        #endregion

        #region Move(Direction direction, bool shiftHeld, bool controlHeld)
        /// <summary>
        /// If shift is held, moves playerShip 2, if only arrow keys is held, moves playerShip 5, if control is held, moves playerShip 8.
        /// </summary>
        /// <param name="direction">left or right arrow key</param>
        /// <param name="shiftHeld">if shift is held</param>
        /// <param name="controlHeld">if control is held</param>
        public void Move(Direction direction, bool shiftHeld, bool controlHeld)
        {
            if (shiftHeld)
            {
                moveInterval = 2;
            }
            else
            {
                moveInterval = 5;
            }
            if (controlHeld)
            {
                moveInterval = 8;
            }
           
            switch (direction)
            {
                case Direction.Left:
                    Location = new Point(Location.X - moveInterval, Location.Y);
                    break;
                case Direction.Right:
                    Location = new Point(Location.X + moveInterval, Location.Y);
                    break;

            }
        }
        #endregion

        #region Draw(Graphics g)
        /// <summary>
        /// Checks if player ship is alive. If true, draws playerShip. If false, draws a flattening animation of the ship. If dead for 3 seconds, 
        /// and the playerShip has died less than 3 times, Make the playerShip alive.
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            if (Alive)
            {
                deadShipHeight = Area.Height;
                g.DrawImage(image, Location);
            }
            else
            {
                if (crushTimer.Year != DateTime.Now.Year)
                    crushTimer = DateTime.Now;
                currentTimer = DateTime.Now;
                span1 = currentTimer - crushTimer;
                if (span1.Seconds == 3 && timesDead <= 1)
                {
                    timesDead++;
                     Alive = true;
                     crushTimer = new DateTime();
                }
                else
                {
                    if (deadShipHeight > 7)
                    {
                        deadShipHeight--;
                    }
                    g.DrawImage(image, new Rectangle(new Point(Location.X, Location.Y + (Area.Height + Area.Height / 3 - deadShipHeight)), new Size(Area.Width + Area.Width / 3, deadShipHeight)));
                }
            }
        }
        #endregion

        #region Die()
        /// <summary>
        /// Sets alive to false
        /// </summary>
        public void Die()
        {
            Alive = false;
        }
        #endregion
    }
}
