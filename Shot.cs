using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lab3SpaceInvaders
{
    class Shot
    {
        #region Properties and fields
        private Random random;
        private const int moveIntervalUp = 10;
        private int moveIntervalDown = 10;
        private int variableMoveInterval = 10;
        private Point chargeMove;
        private Point portalMove;
        private int width = 5;
        private int height = 17;
        private int framesSkipped = 0;
        public Point Location { get; private set; }
        public Rectangle HitBox { get; private set; }
        private Direction direction;
        private bool boomerang;
        private int chargeMoveVariance = 0;
        private bool charge;
        private bool homing;
        private bool bossShot = false;
        private Boss boss;
        private bool bossExists = false;
        private int bossNumber;
        private bool throughPortal = false;
        private List<Invader> invaders;
        private List<BonusShip> bonusShips;
        private List<Point> closestShips;
        private Rectangle boundaries;
        private Point explodeMove;
        private int color = 0;
        #endregion

        #region Constructor
        public Shot(Point location, Direction direction, Rectangle boundaries, bool boomerang, bool charge, bool homing,
            List<Invader> invaders, List<BonusShip> bonusShips, Boss boss, Random random
            )
        {
            this.boss = boss;
            this.invaders = invaders;
            this.bonusShips = bonusShips;
            this.homing = homing;
            this.random = random;
            this.boomerang = boomerang;
            this.charge = charge;
            closestShips = new List<Point>();
            if (charge)
            {
                chargeMove = new Point(random.Next(-3, 4), random.Next(7, 11));

            }
            this.Location = location;
            this.direction = direction;
            this.boundaries = boundaries;
            if (boss != null)
            {
                bossExists = true;
            }
            HitBox = new Rectangle(Location, new Size(width, height));
        }
        #endregion

        #region Default shot constructor
        public Shot(Point location, Direction direction, Rectangle boundaries)
        {
            boomerang = false;
            charge = false;
            this.Location = location;
            this.direction = direction;
            this.boundaries = boundaries;
            HitBox = new Rectangle(Location, new Size(width, height));
        }
        #endregion

        #region Constructor for boss
        public Shot(Point location, Direction direction, Rectangle boundaries, bool boss, int bossNumber, bool throughPortal, double rotateAngle, int color, Random random)
        {
            this.bossNumber = bossNumber;
            bossShot = boss;
            boomerang = false;
            charge = false;
            this.Location = location;
            this.direction = direction;
            this.boundaries = boundaries;
            HitBox = new Rectangle(Location, new Size(width, height));
            explodeMove = new Point(random.Next(-10, 10), random.Next(-10, 10));
            this.throughPortal = throughPortal;
            this.color = color;
            if (boss)
            {
                moveIntervalDown += 5;
            }
            if (throughPortal)
            {
                SetPortalMoveInterval(rotateAngle);
                if (Math.Abs(portalMove.X) > Math.Abs(portalMove.Y))
                {
                    width = 15;
                    height = 5;
                }
            }
        }
        #endregion

        public void Hide()
        {
            Location = new Point(-10, -10);
        }

        public bool GetThroughPortal()
        {
            return throughPortal;
        }

        //public void SetLocation(Point point)
        //{
        //    if (!throughPortal)
        //    {
        //        Location = point;
        //    }
        //}

        public void SetPortalMoveInterval(double rotateAngle)
        {
            if (rotateAngle > 0)
            {
                portalMove = new Point(-(int)(Math.Sin(rotateAngle) * 15), (int)(Math.Cos(rotateAngle) * 15));
            }
            if (rotateAngle <= 0)
            {
                portalMove = new Point(-(int)(Math.Sin(rotateAngle) * 15), (int)(Math.Cos(rotateAngle) * 15));
            }

        }

        #region Draw(Graphics g)
        /// <summary>
        /// Draws the shot. If boomerang, shots are purple. If not, shots are yellow.
        /// </summary>
        /// <param name="g"></param>
        public void Draw(Graphics g)
        {
            if (boomerang)
            {
                g.FillRectangle(Brushes.Purple, new Rectangle(Location, new Size(width, height)));
                return;
            }
            if (color == 0)
            { 
                g.FillRectangle(Brushes.Yellow, new Rectangle(Location, new Size(width, height)));
            }
            else if (color == 1)
            {
                g.FillRectangle(Brushes.Red, new Rectangle(Location, new Size(width, height)));
            }
        }
        #endregion

        #region Move()
        /// <summary>
        /// Moves shots based on if boomerang, charge, and homing are on.
        /// </summary>
        /// <returns>true if shot is within boundaries, false if not</returns>
        public bool Move()
        {
            if (bossNumber == 3 && throughPortal)
            {
                Location = new Point(Location.X + portalMove.X, Location.Y + portalMove.Y);
                if (boundaries.Contains(Location) || Location.X == -10)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            if (!boomerang && !charge && !homing)
            {
                if (direction == Direction.Down)
                {
                    Location = new Point(Location.X, Location.Y + moveIntervalDown);
                }
                else if (direction == Direction.Up)
                {
                    Location = new Point(Location.X, Location.Y - moveIntervalUp);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            if (boomerang && !charge && !homing)
            {
                BoomerangMove();
            }
            if (!boomerang && charge && !homing)
            {
                ChargeMove();
            }
            if (boomerang && charge && !homing)
            {
                ChargeMove();
                BoomerangMove();
                Location = new Point(Location.X, Location.Y + moveIntervalUp);
            }
            if (!boomerang && !charge && homing)
            {
                HomingMove();
            }
            if (boomerang && !charge && homing)
            {
                BoomerangMove();
                HomingMove();
                Location = new Point(Location.X, Location.Y + moveIntervalUp);
            }
            if (!boomerang && charge && homing)
            {
                ChargeMove();
                HomingMove();
                Location = new Point(Location.X, Location.Y + moveIntervalUp);
            }
            if (boomerang && charge && homing)
            {
                ChargeMove();
                BoomerangMove();
                HomingMove();
                Location = new Point(Location.X, Location.Y + 2 * moveIntervalUp);
            }
            if (bossNumber == 2)
            {
                if (Location.Y >= 600)
                {
                    Location = new Point(Location.X + explodeMove.X, Location.Y + explodeMove.Y);
                }
            }
            if (boundaries.Contains(Location))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Move() helpers

        #region HomingMove()
        /// <summary>
        /// Move() helper. Finds the closest enemy and stores it in closestShip. If no enemies within 500 pixels in the x coordinate, goes straight up. 
        /// If the closest ship is to the right, add 2 to x, if it's to the left, minus 2 to x.
        /// Clears closestShip.
        /// </summary>
        private void HomingMove()
        {
            if (bossExists)
            {
                //assuming boss is 200 pixels
                if (boss.Location.X + random.Next(50, boss.Area.Width -50) - Location.X == 0)
                {
                    Location = new Point(Location.X, Location.Y);
                }
                else if (boss.Location.X + random.Next(50, boss.Area.Width - 50) - Location.X > 0)
                {
                    Location = new Point(Location.X + 2, Location.Y);
                }
                else if (boss.Location.X + random.Next(50, boss.Area.Width - 50) - Location.X < 0)
                {
                    Location = new Point(Location.X - 2, Location.Y);
                }
            }
            FindClosestEnemy();
            if (closestShips.Count == 0)
            {
                Location = new Point(Location.X, Location.Y - moveIntervalUp);
                return;
            }
            if (closestShips[0].X > 500)
            {
                closestShips.Remove(closestShips[0]);
            }
            if (closestShips.Count == 0)
            {
                Location = new Point(Location.X, Location.Y - moveIntervalUp);
                return;
            }

            if (Location.X - closestShips[0].X > Location.X)
            {
                Location = new Point(Location.X - 2, Location.Y);
            }
            else if (Location.X - closestShips[0].X < Location.X)
            {
                Location = new Point(Location.X + 2, Location.Y);
            }

            Location = new Point(Location.X, Location.Y - moveIntervalUp);
            closestShips.Clear();
        }
        #endregion

        #region FindClosestEnemy()
        /// <summary>
        /// HomingMove() helper. Loops through invaders. Compares the center of each invader with the location of the shot.
        /// If it is within 500 pixels in the x axis, stores it in closestShip field.
        /// </summary>
        private void FindClosestEnemy()
        {
            Point closestSoFar;
            
            foreach (Invader invader in invaders)
            {
                closestSoFar = new Point(invader.Location.X + invader.Area.Width/2 - Location.X, 
                    invader.Location.Y + invader.Area.Height/2 - Location.Y);
                if (closestShips.Count == 0)
                {
                    closestShips.Add(closestSoFar);
                }
                if (Math.Abs(closestSoFar.X) < 500)
                {
                    if (Math.Abs(closestSoFar.X) < Math.Abs(closestShips[0].X))
                    {
                        closestShips.Clear();
                        closestShips.Add(closestSoFar);
                    }
                    else if (Math.Abs(closestSoFar.X) == Math.Abs(closestShips[0].X))
                    {
                        closestShips.Add(closestSoFar);
                    }
                }
            }
        }
        #endregion

        #region ChargeMove()
        /// <summary>
        /// Move() helper. chargeMove is a point in a rectangle in front of playerShip. Add chargeMove to Location
        /// </summary>
        private void ChargeMove()
        {
            //chargeMove is a random point in the rectangle (-3, 7), (3, 10). 
            // If that is all the variance the shots tend to overlap eachother because there is only 28 different possible spots.
            // To fix this, add random numbers to the x and y coordinates at the beginning of the shot

            //if (Location.Y <= 950)
            //{
            if (chargeMoveVariance <= 10)
                {
                    if (random.Next(2) == 0)
                    {
                        //chargeMove = new Point(chargeMove.X + random.Next(3), chargeMove.Y);
                        Location = new Point(Location.X + random.Next(-5, 5), Location.Y - random.Next(-10, 10));
                        chargeMoveVariance++;
                    }
                    if (random.Next(2) == 1)
                    {
                        //chargeMove = new Point(chargeMove.X - random.Next(3), chargeMove.Y);
                        Location = new Point(Location.X + random.Next(-5, 5), Location.Y - random.Next(-10, 10));
                        chargeMoveVariance++;
                    }
                }
            //}
            Location = new Point(Location.X + chargeMove.X, Location.Y - chargeMove.Y);
        }
        #endregion

        #region BoomerangMove()
        /// <summary>
        /// Move() helper. If boomerang is close to the top of the screen, -1 to variable move interval. 
        /// It should eventually start travelling the opposite direction at the same speed.
        /// </summary>
        private void BoomerangMove()
        {
            if (Location.Y - 150 <= 0)
            {
                Location = new Point(Location.X, Location.Y - variableMoveInterval);
                variableMoveInterval--;

                //want it to slow down when it changes directions
                if (-2 <= variableMoveInterval && 2 >= variableMoveInterval)
                {
                    framesSkipped++;
                    if (framesSkipped == 3)
                    {
                        variableMoveInterval--;
                        framesSkipped = 0;
                    }
                    variableMoveInterval++;
                }
            }
            else
            {
                Location = new Point(Location.X, Location.Y - variableMoveInterval);
            }
        }
        #endregion

        #endregion
    }
}
