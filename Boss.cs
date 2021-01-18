using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Lab3SpaceInvaders
{
    class Boss
    {
        #region Properties and fields
        private int VerticalSpeed = 1;
        private int HorizontalSpeed = 1;
        private Bitmap image;
        private Rectangle portal;
        public Point Location { get; private set; }
        private PlayerShip playerShip;
        private Direction direction;
        private int framesSkipped = 0;
        private int health = 50;
        private int waitTimer = 0;
        private int chargeTimer = 0;
        private int portalTimer = 0;
        private int color = 0;
        private int bossShotsCount = 0;
        private int bossShotsCountComparer = 0;
        private bool throughPortal = false;
        private bool charged = true;
        private bool attacking = false;
        private bool changeDirection = false;
        private double rotateAngle;
        private double rotateAngleRadians;
        private Point portalEndPoint;
        private List<Shot> bossShots;
        private Rectangle boundaries;
        private Random random;
        private int fullyCharged = 3;
        public Rectangle Area
        {
            get
            {
                return new Rectangle(Location, image.Size);
            }
        }
        private int bossNumber;
        private Point boss2ShootingLocation;
        #endregion

        #region Constructor
        public Boss(PlayerShip playerShip, Point Location, List<Shot> bossShots, Rectangle boundaries, int health, int bossNumber, Random random)
        {
            this.random = random;
            this.bossNumber = bossNumber;
            this.health = health;
            this.boundaries = boundaries;
            this.bossShots = bossShots;
            this.Location = Location;
            this.playerShip = playerShip;
            image = BossImage(0);
            if (bossNumber == 3)
            {
                //charged = false;
                fullyCharged = 6;
            }
        }
        #endregion

        #region Move()
        /// <summary>
        /// For Boss 1:
        /// Finds out where player is, and moves toward him. Boss changes directions slowly. If the player direction is down,
        /// fire charge attack if it is ready. When attacking, remain attacking for full charge attack duration.
        /// Using this as a substitute Go() method. Removes bossShots if they go out of bounds. Attack duration is 2 seconds
        /// 
        /// For Boss 2:
        /// Same as boss 1, except he can move after he shoots his attack.
        /// </summary>
        public virtual void Move()
        {
            #region Boss1 and boss2
            if (bossNumber <= 2)
            {
                FindPlayerLocation();
                framesSkipped++;
                Charge();
                if (attacking)
                {
                    direction = Direction.Down;
                }
                switch (direction)
                {
                    case Direction.Left:
                        Location = new Point(Location.X + HorizontalSpeed, Location.Y);
                        if (HorizontalSpeed < 10)
                            if (framesSkipped >= 10)
                            {
                                if (Math.Abs(playerShip.Location.X - Location.X) < 100)
                                {
                                    if (HorizontalSpeed < 0)
                                    {
                                        HorizontalSpeed++;
                                    }
                                    else if (HorizontalSpeed > 0)
                                    {
                                        HorizontalSpeed--;
                                    }
                                }
                                HorizontalSpeed++;
                                framesSkipped = 0;
                            }
                        break;
                    case Direction.Right:
                        Location = new Point(Location.X + HorizontalSpeed, Location.Y);
                        if (HorizontalSpeed > -10)
                            if (framesSkipped >= 10)
                            {
                                if (Math.Abs(playerShip.Location.X - Location.X) < 100)
                                {
                                    if (HorizontalSpeed < 0)
                                    {
                                        HorizontalSpeed++;
                                    }
                                    else if (HorizontalSpeed > 0)
                                    {
                                        HorizontalSpeed--;
                                    }
                                }
                                HorizontalSpeed--;
                                framesSkipped = 0;
                            }
                        break;
                    case Direction.Down:

                        #region Boss1
                        if (bossNumber == 1)
                        {
                            if (charged)
                            {
                                HorizontalSpeed = 0;
                            }
                            Location = new Point(Location.X + HorizontalSpeed, Location.Y);
                            if (bossShots.Count > 0)
                            {
                                for (int j = bossShots.Count - 1; j >= 0; j--)
                                {
                                    if (!boundaries.Contains(bossShots[j].Location) && bossShots[j].Location.X != -10)
                                    {
                                        bossShots.Remove(bossShots[j]);
                                    }
                                }
                            }
                            if (!charged)
                            {
                                return;
                            }
                            waitTimer++;
                            if (waitTimer >= 200)
                            {
                                attacking = false;
                                charged = false;
                                waitTimer = 0;
                                chargeTimer = 0;
                            }
                            else
                            {
                                for (int i = 0; i < 40; i++)
                                {
                                  
                                   
                                    attacking = true;
                                    Attack();
                                    return;
                                   
                                }
                            }
                        }
                        #endregion

                        #region Boss2
                        else if (bossNumber == 2)
                        {
                            Location = new Point(Location.X + HorizontalSpeed, Location.Y);
                            if (bossShots.Count > 0)
                            {
                                for (int j = bossShots.Count - 1; j >= 0; j--)
                                {
                                    if (!boundaries.Contains(bossShots[j].Location) && bossShots[j].Location.X != -10)
                                    {
                                        bossShots.Remove(bossShots[j]);
                                    }
                                }
                            }
                            if (!charged)
                            {
                                return;
                            }
                            if (boss2ShootingLocation == new Point(0, 0))
                            {
                                boss2ShootingLocation = Location;
                            }
                            waitTimer++;
                            if (waitTimer >= 10)
                            {
                                boss2ShootingLocation = new Point(0, 0);
                                attacking = false;
                                charged = false;
                                waitTimer = 0;
                                chargeTimer = 0;
                            }
                            else
                            {
                                for (int i = 0; i < 40; i++)
                                {
                                    attacking = true;
                                    Attack();
                                    return;
                                }
                            }
                        }
                        #endregion

                        break;

                }
            }
            #endregion

            #region Boss3
            else if (bossNumber == 3)
            {
                RemoveBossShotsWhenInPortal();
                if (portalTimer >= 300 && bossShotsCountComparer != 0)
                {
                    throughPortal = true;
                }
                if (portalTimer == 600)
                {
                    bossShots.Clear();
                }
                // code which direction I am going in
                if (random.Next(2) == 0)
                {
                    direction = Direction.Left;
                }
                else
                {
                    direction = Direction.Right;
                }
                if (random.Next(100) <= 1)
                {
                    changeDirection = true;
                }
                if (changeDirection)
                {
                    changeDirection = false;
                    if (direction == Direction.Left)
                    {
                        direction = Direction.Right;
                    }
                    if (direction == Direction.Right)
                    {
                        direction = Direction.Left;
                    }
                }
                if (Location.X < 500)
                {
                    direction = Direction.Left;
                }
                if (Location.X > boundaries.Width - Area.Width - 520)
                {
                    direction = Direction.Right;
                }
                if (charged)
                {
                    direction = Direction.Down;
                }
                if (throughPortal)
                {
                    Attack();
                }
                framesSkipped++;
                Charge();
                if (attacking)
                {
                    direction = Direction.Down;
                }
                switch (direction)
                {
                    case Direction.Left:
                        Location = new Point(Location.X + HorizontalSpeed, Location.Y + VerticalSpeed);
                        if (HorizontalSpeed < 10)
                            if (framesSkipped >= 10)
                            {
                                if (Math.Abs(Location.X) < 100)
                                {
                                    if (HorizontalSpeed < 0)
                                    {
                                        HorizontalSpeed++;
                                    }
                                    else if (HorizontalSpeed > 0)
                                    {
                                        HorizontalSpeed--;
                                    }
                                }
                                HorizontalSpeed++;
                            }
                        if (Math.Abs(VerticalSpeed) < 5)
                        {
                            if (framesSkipped >= 10)
                            {
                                if (Location.Y > 200)
                                {
                                    VerticalSpeed--;
                                }
                                else if (Location.Y < 50)
                                {
                                    VerticalSpeed++;
                                }
                                framesSkipped = 0;
                            }
                        }
                        break;
                    case Direction.Right:
                        if (HorizontalSpeed > -10)
                            if (framesSkipped >= 10)
                            {
                                if (Math.Abs(Location.X - boundaries.Width) < 100)
                                {
                                    if (HorizontalSpeed < 0)
                                    {
                                        HorizontalSpeed++;
                                    }
                                    else if (HorizontalSpeed > 0)
                                    {
                                        HorizontalSpeed--;
                                    }
                                }
                                HorizontalSpeed--;
                            }
                        if (Math.Abs(VerticalSpeed) < 5)
                        {
                            if (framesSkipped >= 10)
                            {
                                if (Location.Y > 200)
                                {
                                    VerticalSpeed--;
                                    if (Location.Y > 400)
                                    {
                                        VerticalSpeed--;
                                    }
                                }
                                else if (Location.Y < 50)
                                {
                                    VerticalSpeed++;
                                }
                                framesSkipped = 0;
                            }
                        }
                        Location = new Point(Location.X + HorizontalSpeed, Location.Y + VerticalSpeed);
                        break;
                    case Direction.Down:
                        if (charged)
                        {
                            HorizontalSpeed = 0;
                            VerticalSpeed = 0;
                        }
                        Location = new Point(Location.X + HorizontalSpeed, Location.Y);
                        if (bossShots.Count > 0)
                        {
                            for (int j = bossShots.Count - 1; j >= 0; j--)
                            {
                                if (!boundaries.Contains(bossShots[j].Location) && bossShots[j].Location.X != -10)
                                {
                                    bossShots.Remove(bossShots[j]);
                                }
                            }
                        }
                        if (!charged)
                        {
                            VerticalSpeed = 1;
                            return;
                        }
                        waitTimer++;
                        if (waitTimer >= 80)
                        {
                            boss2ShootingLocation = new Point(0, 0);
                            attacking = false;
                            charged = false;
                            waitTimer = 0;
                            chargeTimer = 0;
                            HorizontalSpeed = 1;
                            VerticalSpeed = 1;
                        }
                        else
                        {
                            for (int i = 0; i < 40; i++)
                            {
                                attacking = true;
                                Attack();
                                return;
                            }
                        }
                        break;
                }
            }
            #endregion
        }
        #endregion

        #region Charge()
        /// <summary>
        /// Starts the timer if not already started. Ends different timer. If the difference is equal to 3 seconds, charged = true
        /// so next time direction = down, the boss fires.
        /// </summary>
        public void Charge()
        {
            chargeTimer++;
            if (chargeTimer >= fullyCharged * 100)
            {
                charged = true;
            }
        }
        #endregion

        #region Attack()
        /// <summary>
        /// If the boss is attacking, spawn a line of shots
        /// </summary>
        public void Attack()
        {
            //if (!attacking)
            //{
            //    return;
            //}
            if (bossNumber == 1)
            {
                for (int i = 60; i < Area.Width - 90; i += 5)
                {
                    bossShots.Add(new Shot(new Point(Location.X + i + 25, Location.Y + Area.Height / 4 * 3), Direction.Down, boundaries, true, bossNumber, throughPortal, rotateAngleRadians, 0, random));
                }
            }
            if (bossNumber == 2)
            {

                for (int i = 60; i < Area.Width - 90; i += 5)
                {
                    bossShots.Add(new Shot(new Point(boss2ShootingLocation.X + i + 25, boss2ShootingLocation.Y + Area.Height / 4 * 3), Direction.Down, boundaries, true, bossNumber, throughPortal, rotateAngleRadians, 0, random));
                }
            }
            if (bossNumber == 3)
            {
                
                if (!throughPortal)
                {
                    for (int i = 110; i < Area.Width - 140; i += 5)
                    {
                        bossShots.Add(new Shot(new Point(Location.X + i + 15, Location.Y + Area.Height / 4 * 3), Direction.Down, boundaries, true, bossNumber, throughPortal, rotateAngleRadians, 0, random));
                        bossShotsCount++;

                        //if (bossShotsCountComparer < bossShotsCount)
                        //{
                        //    goneInPortalTimerBegin = new DateTime();
                        //}
                    }
                    if (portalTimer >= 300 && portalTimer < 700)
                    {
                        throughPortal = true;
                    }
                }
                else
                {
                    for (int l = bossShots.Count - 1; l >= 0; l--)
                    {
                        if (!boundaries.Contains(bossShots[l].Location) && bossShots[l].Location.X != -10)
                        {
                            bossShots.Remove(bossShots[l]);
                        }
                    }
                    if (true)
                    {
                        for (int i = 1; i <= 10; i++)
                        {
                            if (bossShotsCountComparer == 0)
                            {
                                break;
                            }
                            bossShots.Add(new Shot(new Point(portalEndPoint.X  + (int)(50 * Math.Cos(rotateAngleRadians) * i / 10) + 40, 
                                portalEndPoint.Y + (int)(50 * Math.Sin(rotateAngleRadians) * i / 10) + 70), Direction.Down, boundaries, true, bossNumber, true, rotateAngleRadians, 0, random));
                            bossShotsCountComparer--;
                            //for (int j = bossShots.Count - 1; j >= 0; j--)
                            //{
                            //    bossShots[j].SetPortalMoveInterval(rotateAngle);
                            //}
                        }
                        //if (color == 0)
                        //{
                        //    color = 1;
                        //}
                        //else if (color == 1)
                        //{
                        //    color = 0;
                        //}
                    }
                    if (bossShots.Count == 0 && bossShotsCountComparer == 0)
                    {
                        throughPortal = false;
                    }
                }
            }
        }
        #endregion

        public int GetBossNumber()
        {
            return bossNumber; 
        }

        #region FindPlayerLocation()
        /// <summary>
        /// Finds the direction to the playerShip. If the middle of the playerShip is directly below the boss, direction = down
        /// </summary>
        public void FindPlayerLocation()
        {
            if (playerShip.Location.X - playerShip.Area.Width - Location.X > 10) //why is this not playerShip.Area.Width/2? Boss location is at the corner.
            {
                direction = Direction.Left; //I got these backwards but whatever, it works fine now.
            }
            else if (playerShip.Location.X - playerShip.Area.Width - Location.X < -10)
            {
                direction = Direction.Right;
            }
            else
            {
                direction = Direction.Down;
            }
        }
        #endregion

        public void PortalEndPoint()
        {
            if (portalTimer <= 2)
            {
                portalEndPoint = new Point(random.Next(boundaries.Width - 200), Location.Y + 400);
            }
        }

        public void RotateAngle()
        {
            if (true)
            {
                rotateAngle = Math.Atan2(-(playerShip.Location.X + playerShip.Area.Width/2 - (double)portalEndPoint.X - portal.Width/2), playerShip.Location.Y - (double)portalEndPoint.Y - portal.Height/2 - 75) * 180 / Math.PI;
                rotateAngleRadians = rotateAngle / 180 * Math.PI;
            }
        }

        #region DrawFirstPortal(Graphics g)
        private void DrawFirstPortal(Graphics g)
        {
            if (bossShots.Count >= 1)
            {
                if (!throughPortal)
                {
                    g.DrawImage(RotatePortal(0, g, portal), new Point(Location.X + Area.Width / 2 - 150 / 2, Location.Y + 400));
                }
            }
        }
        #endregion

        #region RemoveBossShotsWhenInPortal() 
        private void RemoveBossShotsWhenInPortal()
        {
            for (int i = bossShots.Count - 1; i >= 0; i--)
            {
                if (bossShots[i].Location.Y >= Location.Y + 460 && bossShots[i].Location.X > 0 && !throughPortal)
                {
                    bossShots.Remove(bossShots[i]);
                    bossShotsCountComparer++;
                }
            }
            if (bossShotsCountComparer > 0 || portalTimer > 0)
            {
                portalTimer++;
            }
        }
        #endregion

        #region DrawSecondPortal() 
        private void DrawSecondPortal(Graphics g)
        {
            if (bossShotsCountComparer != 0)
            {
                if (portalTimer >= 270 && bossShotsCountComparer != 0)
                {
                    g.DrawImage(RotatePortal((float)rotateAngle, g, portal), portalEndPoint);
                    for (int i = 0; i < 40; i++)
                    {
                        if (portalTimer <= i * 5)
                        {
                            Attack();
                            return;
                        }
                    }
                }
            }
            if (portalTimer > 550)
            {
                bossShotsCountComparer = 0;
                bossShotsCount = 0;
                throughPortal = false;
                portalTimer = 0;
            }
        }
        #endregion

        #region Draw(Graphics g, int animationCell)
        /// <summary>
        /// Draws the boss image with the correct animation (if I end up putting one in)
        /// </summary>
        /// <param name="g"></param>
        /// <param name="animationCell"></param>
        public virtual void Draw(Graphics g, int animationCell)
        {
            /*g.DrawImage(BossImage(animationCell), new Point(Location.X, Location.Y));*/ //it's drawing in a weird size. No idea why
            if (bossNumber != 3)
                g.DrawImageUnscaledAndClipped(BossImage(animationCell), new Rectangle(Location, new Size(200, 200)));
            if (bossNumber == 3)
            {
                g.DrawImageUnscaledAndClipped(BossImage(animationCell), new Rectangle(Location, new Size(300, 300)));
                portal = new Rectangle(new Point(Location.X + Area.Width / 2 - 50 / 2, Location.Y + 400), new Size(150, 50));
                DrawFirstPortal(g);

                PortalEndPoint();
                RotateAngle();
                DrawSecondPortal(g);
            }
        }
        #endregion

        #region  RotatePortal(Image img, float rotationAngle, Graphics g, Rectangle portal)
        private Image RotatePortal(Image img, float rotationAngle, Graphics g, Rectangle portal)
        {
            Bitmap bmp = new Bitmap(img.Width, img.Height);

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
            gfx.DrawImage(img, new Point(0, 0));

            //dispose of our Graphics object
            gfx.Dispose();

            //return the image
            return bmp;

        }
        #endregion

        #region RotatePortal(float rotationAngle, Graphics g, Rectangle portal)
        private Image RotatePortal(float rotationAngle, Graphics g, Rectangle portal)
        {
            Bitmap bmp = new Bitmap(portal.Width, portal.Width);

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
            gfx.FillEllipse(Brushes.Purple, new Rectangle(new Point(0, 50), portal.Size));

            //dispose of our Graphics object
            gfx.Dispose();

            //return the image
            return bmp;

        }
        #endregion

        #region getHealth()
        /// <summary>
        /// 
        /// </summary>
        /// <returns>health</returns>
        public int GetHealth()
        {
            return health;
        }
        #endregion

        #region RemoveHealth()
        /// <summary>
        /// Minus one from health
        /// </summary>
        public void RemoveHealth()
        {
            health--;
        }
        #endregion

        #region BossImage(int animationCell)
        /// <summary>
        /// 
        /// </summary>
        /// <param name="animationCell"></param>
        /// <returns>the boss image with the correct animation</returns>
        public virtual Bitmap BossImage(int animationCell)   // right now this doesn't do anything because I didn't make another boss image.
        {
            if (bossNumber == 1)
            {
                if (animationCell == 0)
                {
                    return Properties.Resources.Boss;
                }
                else
                {
                    return Properties.Resources.Boss;
                }
            }
            else if (bossNumber == 2)
            {
                if (animationCell == 0)
                {
                    return Properties.Resources.Boss2;
                }
                else
                {
                    return Properties.Resources.Boss2;
                }
            }
            else if (bossNumber == 3)
            {
                return Properties.Resources.Boss3;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
