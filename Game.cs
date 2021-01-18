using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms; 

namespace Lab3SpaceInvaders
{
    class Game
    {
        // add sound effects

        #region Properties and fields
        Font arial14bold = new Font("Arial", 14, FontStyle.Bold);
        Font arial15bold = new Font("Arial", 15, FontStyle.Bold);
        Font arial24bold = new Font("Arial", 24, FontStyle.Bold);
        Font arial74bold = new Font("Arial", 74, FontStyle.Bold);
        bool atRightSide = false;
        bool atLeftSide = false;
        public int score { get; private set; } = 0;
        private int livesLeft = 3;
        private int wave = 0;
        private int framesSkipped = 0;
        private Rectangle boundaries;
        private Random random;
        private Direction invaderDirection = Direction.Right;
        private List<Invader> invaders;
        public PlayerShip playerShip { get; private set; }
        private List<Shot> playerShots;
        private List<Shot> invaderShots;
        private List<Shot> bossShots;
        private Stars stars;
        private DateTime waitTimerBegin;
        private DateTime waitTimerEnd;
        private TimeSpan span1;
        private List<Shield> shields;
        private int piercing;
        private int originalPiercing;
        private int numShots;
        private bool boomerang;
        private bool charge;
        private bool homing;
        private bool nextWave = false;
        private Boss boss;
        private double bossHealthBar;
        public bool Alive { get; set; } = true;
        private List<BonusShip> bonusShips;
        #endregion

        public void SwitchBoomerang()
        {
            if (boomerang)
            {
                boomerang = false;
            }
            else
            {
                boomerang = true;
            }
        }

        public void SwitchCharge()
        {
            if (charge)
            {
                charge = false;
            }
            else
            {
                charge = true;
            }
        }

        public void SwitchHoming()
        {
            if (homing)
            {
                homing = false;
            }
            else
            {
                homing = true;
            }
        }

        #region Constructor 
        public Game(Rectangle boundaries, int piercing, int numShots, bool boomerang, bool charge, bool homing, Random random)
        {
            this.piercing = piercing;
            originalPiercing = piercing;
            this.numShots = numShots;
            this.boundaries = boundaries;
            this.random = random;
            this.boomerang = boomerang;
            this.charge = charge;
            this.homing = homing;
            playerShip = new PlayerShip(new Point(925, 950));
            invaders = new List<Invader>();
            shields = new List<Shield>();
            bonusShips = new List<BonusShip>();
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 5; j++)
                switch (j)
                { 
                    case 0:
                        invaders.Add(new Invader(ShipType.invader5, new Point(i * 100 + 100, j * 60 + 100), 50));
                        break;
                    case 1:
                        invaders.Add(new Invader(ShipType.invader4, new Point(i * 100 + 100, j * 60 + 100), 40));
                        break;
                    case 2:
                        invaders.Add(new Invader(ShipType.invader3, new Point(i * 100 + 100, j * 60 + 100), 30));
                        break;
                    case 3:
                        invaders.Add(new Invader(ShipType.invader2, new Point(i * 100 + 100, j * 60 + 100), 20));
                        break;
                    case 4:
                        invaders.Add(new Invader(ShipType.invader1, new Point(i * 100 + 100, j * 60 + 100), 10));
                        break;
                }
            }
            playerShots = new List<Shot>();
            invaderShots = new List<Shot>();
            bossShots = new List<Shot>();
            stars = new Stars(boundaries, random);
            shields.Add(new Shield(new Point(150, 825)));
            shields.Add(new Shield(new Point(650, 825)));
            shields.Add(new Shield(new Point(1150, 825)));
            shields.Add(new Shield(new Point(1550, 825)));
        }
        #endregion

        public int GetWave()
        {
            return wave + 1;
        }

        #region FireShot()
        /// <summary>
        /// Checks if you are alive and then spawns a shot if you do not have too many shots in the air or if you have charged shots. Removes if out of boundaries. 
        /// </summary>
        public void FireShot()
        {
            if (!Alive)
                return;
            if (!playerShip.Alive)
                return;
            if (playerShots.Count < numShots || charge)
            {
                if (charge)
                {
                    for (int i = 0; i < numShots; i++)
                    {
                        playerShots.Add(new Shot(new Point(playerShip.Location.X + 28, playerShip.Location.Y + 24), Direction.Up, 
                            boundaries, boomerang, charge, homing, invaders, bonusShips, boss, random));
                    }
                }
                playerShots.Add(new Shot(new Point(playerShip.Location.X + 28, playerShip.Location.Y + 24), Direction.Up, 
                    boundaries, boomerang, charge, homing, invaders, bonusShips, boss, random));
            }
            for (int i = 0; i < playerShots.Count; i++)
            {
                if (!boundaries.Contains(playerShots[i].Location))
                {
                    playerShots.Remove(playerShots[i]);
                }
            }
        }
        #endregion

        #region DecideBonusShip()
        /// <summary>
        /// Spawns an upgrade randomly on either side of the boundaries and decide what type of upgrade.
        /// </summary>
        public void DecideBonusShip()
        {
            Point spawnPoint;
            if (random.Next(2) == 0)
            {
                spawnPoint = new Point(10, random.Next(100, 700));
            }
            else
            {
                spawnPoint = new Point(1700, random.Next(100, 700));
            }
            if (random.Next(100) == 0)
            { 
                bonusShips.Add(new SuperRareBonusShip(spawnPoint));
            }
            else
            {
                bonusShips.Add(new BonusShip(spawnPoint));
            }
        }
        #endregion

        #region MoveInvadersCheck()
        /// <summary>
        /// Move invaders once every (7 - wave#) frames
        /// </summary>
        public void MoveInvadersCheck()
        {
            if (framesSkipped >= 7 - wave)
            {
                framesSkipped = 0;
                MoveInvaders();
            }
        }
        #endregion

        #region MoveShots()
        /// <summary>
        /// Moves every shot in playerShots, invaderShots, and bossShots
        /// </summary>
        public void MoveShots()
        {
            foreach (Shot shot in playerShots)
            {
                shot.Move();
            }
            foreach (Shot shot in invaderShots)
            {
                shot.Move();
            }
            foreach (Shot shot in bossShots)
            {
                shot.Move();
            }
        }
        #endregion

        #region SpawnAndMoveBonusShips()
        /// <summary>
        /// bonusShip spawn and movement logic
        /// </summary>
        public void SpawnAndMoveBonusShips()
        {
            if (random.Next(600) <= 1 && bonusShips.Count == 0)
            {
                DecideBonusShip();
            }
            for (int i = 0; i < bonusShips.Count; i++)
            {
                if (bonusShips[i].Alive())
                {
                    bonusShips[i].Move(bonusShips[i].FindDirection());
                    if (!boundaries.Contains(bonusShips[i].Location))
                    {
                        bonusShips.Remove(bonusShips[i]);
                    }
                }
            }
        }
        #endregion

        #region CheckForCollisions()
        /// <summary>
        /// Collision Check Logic
        /// </summary>
        public void CheckForCollisions()
        {
            for (int i = 0; i < playerShots.Count; i++)
            {
                for (int j = 0; j < invaders.Count; j++)
                {
                    if (CheckForInvaderCollisions(i, j))
                    {
                        invaders[j].InvaderHit();
                        piercing--;
                        if (piercing < 0)
                        {
                            playerShots.Remove(playerShots[i]);
                            piercing = originalPiercing;
                        }
                        break;
                    }
                }
            }
            for (int i = 0; i < playerShots.Count; i++)
            {
                if (boss == null)
                {
                    break;
                }
                if (CheckForBossCollisions(i))
                {
                    boss.RemoveHealth();
                    playerShots.Remove(playerShots[i]);
                    //
                    //Leaving this here in case I want piercing to do more damage to bosses
                    //
                    //piercing--; 
                    //if (piercing < 0) 
                    //{
                    //    playerShots.Remove(playerShots[i]);
                    //    piercing = originalPiercing;
                    //}
                }
                   
            }
            if (bonusShips.Count != 0)
            {

                for (int i = 0; i < playerShots.Count; i++)
                {
                    for (int j = 0; j < bonusShips.Count; j++)
                    {
                        if (CheckForUpgradeCollisions(i, j))
                        {
                            score += bonusShips[j].score;
                            bonusShips[j].BonusShipHit();
                            piercing--;
                            if (piercing < 0)
                            {
                                playerShots.Remove(playerShots[i]);
                                piercing = originalPiercing;
                            }
                            break;
                        }
                    }
                }
            }
            for (int i = 0; i < shields.Count; i++)
            {
                for (int j = 0; j < playerShots.Count; j++)
                {
                    if (CheckForPlayerShotShieldCollisions(i, j))
                    {
                        foreach (Rectangle rectangle in shields[i].shield)
                        {
                            if (rectangle.Contains(playerShots[j].Location))
                            {
                                shields[i].shield.Remove(rectangle);
                                playerShots.Remove(playerShots[j]);
                                break;
                            }
                        }

                    }
                }
            }
            for (int i = 0; i < shields.Count; i++)
            {
                for (int j = 0; j < bossShots.Count; j++)
                {
                    if (CheckForBossShotShieldCollisions(i, j))
                    {
                        foreach (Rectangle rectangle in shields[i].shield)
                        {
                            if (rectangle.Contains(bossShots[j].Location) || rectangle.Contains(new Point(bossShots[j].Location.X, bossShots[j].Location.Y + 6)))
                            {
                                shields[i].shield.Remove(rectangle);
                                bossShots.Remove(bossShots[j]);
                                break;
                            }
                        }

                    }
                }
            }
            for (int i = 0; i < shields.Count; i++)
            {
                for (int j = 0; j < invaderShots.Count; j++)
                {
                    if (CheckForInvaderShotShieldCollisions(i, j))
                    {
                        foreach (Rectangle rectangle in shields[i].shield)
                        {
                            if (rectangle.Contains(invaderShots[j].Location))
                            {
                                shields[i].shield.Remove(rectangle);
                                invaderShots.Remove(invaderShots[j]);
                                break;
                            }
                        }

                    }
                }
            }
            for (int i = 0; i < invaderShots.Count; i++)
            {
                if (CheckForPlayerCollisions(i))
                {
                    invaderShots.Remove(invaderShots[i]);
                    livesLeft--;
                    if (livesLeft < 0)
                    {
                        Alive = false;
                    }
                    else
                    {
                        Alive = true;
                    }
                }
            }
            for (int i = 0; i < bossShots.Count; i++)
            {
                if (CheckForPlayerCollisionsBoss(i))
                {
                    bossShots.Remove(bossShots[i]);
                    livesLeft--;
                    if (livesLeft < 0)
                    {
                        Alive = false;
                    }
                    else
                    {
                        Alive = true;
                    }
                }
            }
        }
        #endregion

        #region Check for collision helpers


        #region CheckForUpgradeCollisions(int i, int j)
        /// <summary>
        /// checks if a playerShot hits the bonusShip
        /// </summary>
        /// <param name="i">playerShots index</param>
        /// <param name="j">bonusShips index</param>
        /// <returns></returns>
        private bool CheckForUpgradeCollisions(int i, int j)
        {
            if (bonusShips[j].Area.Contains(playerShots[i].Location))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region CheckForInvaderCollisions(int i, int j)
        /// <summary>
        /// checks if a playerShot hits an invader
        /// </summary>
        /// <param name="i">playershots index</param>
        /// <param name="j">invaders index</param>
        /// <returns></returns>
        private bool CheckForInvaderCollisions(int i, int j)
        {
            if ((invaders[j].Area.Contains(playerShots[i].Location)) && invaders[j].Alive())
            {
                score += invaders[j].Score;
                return true;
            }
            // if want it to be for all four corners of the shot, but it lags
            //
            //if ((invaders[j].Area.Contains(playerShots[i].Location) || invaders[j].Area.Contains(new Point(playerShots[i].Location.X + 5, playerShots[i].Location.Y)) //for hitbox on all four corners of shot
            //    || invaders[j].Area.Contains(new Point(playerShots[i].Location.X + 5, playerShots[i].Location.Y + 15)) || invaders[j].Area.Contains(new Point(playerShots[i].Location.X, playerShots[i].Location.Y + 15))) && invaders[j].Alive())
            //{
            //    score += invaders[j].Score;
            //    return true;
            //}
                return false;
        }
        #endregion

        #region CheckForBossCollisions(int i)
        /// <summary>
        /// checks if playerShots hits the boss at any of the shots corners
        /// </summary>
        /// <param name="i">playerShots index</param>
        /// <returns></returns>
        private bool CheckForBossCollisions(int i)
        {
            if (boss.GetBossNumber() == 3)
            {
                if ((playerShots[i].Location.Y < boss.Location.Y + 10 || playerShots[i].Location.Y > boss.Location.Y + 250) || (playerShots[i].Location.X < boss.Location.X + 10 || playerShots[i].Location.X > boss.Location.X + 250))
                    return false;
                return true;
            }
            if ((playerShots[i].Location.Y < boss.Location.Y + 10 || playerShots[i].Location.Y > boss.Location.Y + 190) || (playerShots[i].Location.X < boss.Location.X + 10 || playerShots[i].Location.X > boss.Location.X + 190))
                return false;
            //want the hitbox to be more precise

            //if ((boss.Area.Contains(playerShots[i].Location) || boss.Area.Contains(new Point(playerShots[i].Location.X + 5, playerShots[i].Location.Y))
            //    || boss.Area.Contains(new Point(playerShots[i].Location.X + 5, playerShots[i].Location.Y + 15)) || boss.Area.Contains(new Point(playerShots[i].Location.X, playerShots[i].Location.Y + 15))))
            //{
            //    return true;
            //}
            return true;
        }
        #region CheckForBossShotShieldCollisions(int i, int j)
        /// <summary>
        /// checks if a boss shot hits the shield
        /// </summary>
        /// <param name="i">shields index</param>
        /// <param name="j">bossShots index</param>
        /// <returns></returns>
        private bool CheckForBossShotShieldCollisions(int i, int j)
        {
            foreach (Rectangle rectangle in shields[i].shield)
            {
                if (rectangle.Contains(bossShots[j].Location) || rectangle.Contains(new Point(bossShots[j].Location.X, bossShots[j].Location.Y + 6)))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
        #endregion

        #region CheckForPlayerShotShieldCollisions(int i, int j)
        /// <summary>
        /// checks if a playerShot hits the shield
        /// </summary>
        /// <param name="i">shileds index</param>
        /// <param name="j">playerShots index</param>
        /// <returns></returns>
        private bool CheckForPlayerShotShieldCollisions(int i, int j)
        {
            foreach (Rectangle rectangle in shields[i].shield)
            {
                if (rectangle.Contains(playerShots[j].Location))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region CheckForInvaderShotShieldCollisions(int i, int j)
        /// <summary>
        /// checks if an invaderShot hits the shield
        /// </summary>
        /// <param name="i">shields index</param>
        /// <param name="j">invaderShots index</param>
        /// <returns></returns>
        private bool CheckForInvaderShotShieldCollisions(int i, int j)
        {
            foreach (Rectangle rectangle in shields[i].shield)
            {
                if (rectangle.Contains(invaderShots[j].Location))
                {
                    return true;
                }
            }
            return false;
        }
        #endregion


        #region CheckForPlayerCollisions(int i)
        /// <summary>
        /// checks if any corner of an invader shot hit playerShip
        /// </summary>
        /// <param name="i">invaderShots index</param>
        /// <returns></returns>
        private bool CheckForPlayerCollisions(int i)
        {
            if ((playerShip.Area.Contains(invaderShots[i].Location)))
            {
                playerShip.Die();
                return true;
            }
            return false;
        }
        #endregion

        #region CheckForPlayerCollisionsBoss(int i)
        /// <summary>
        /// checks if any corner of a boss shot hit playerShip
        /// </summary>
        /// <param name="i">bossShots index</param>
        /// <returns></returns>
        private bool CheckForPlayerCollisionsBoss(int i)
        {
            if ((playerShip.Area.Contains(bossShots[i].Location) || playerShip.Area.Contains(new Point(bossShots[i].Location.X + 5, bossShots[i].Location.Y))
                || playerShip.Area.Contains(new Point(bossShots[i].Location.X + 5, bossShots[i].Location.Y + 15)) || playerShip.Area.Contains(new Point(bossShots[i].Location.X, bossShots[i].Location.Y + 15))))
            {
                playerShip.Die();
                return true;
            }
            return false;
        }
        #endregion


        #endregion

        #region SendNextWave()
        /// <summary>
        /// Checks if there are no invaders, and no boss. If the check is true, sets the nextWave field to true.
        /// If the nextWave field is true, it paints "Next Wave" on the form. After 2 seconds, nextWave is set to false.
        /// After 3 seconds, NextWave is called and the timer is reset.
        /// </summary>
        public void SendNextWave()
        {
            if (invaders.Count == 0 && boss == null)
            {
                if (waitTimerBegin.Year != DateTime.Now.Year)
                    waitTimerBegin = DateTime.Now;
                waitTimerEnd = DateTime.Now;
                span1 = waitTimerEnd - waitTimerBegin;
                if (span1.Seconds >= 3)
                {
                    NextWave();
                    nextWave = false;
                    waitTimerBegin = new DateTime();
                }
                else if (span1.Seconds <= 1)
                {
                    nextWave = true;
                }
                else
                {
                    nextWave = false;
                }
            }
        }
        #endregion

        #region RemoveInvaders()
        /// <summary>
        /// If invaders have been dead for 200 milliseconds, remove them
        /// </summary>
        private void RemoveInvaders()
        {
            for (int i = invaders.Count - 1; i >= 0; i--)
            {
                if (!invaders[i].Alive() && invaders[i].TimeDead().Milliseconds >= 200)
                {
                    invaders.Remove(invaders[i]);
                }
            }
        }
        #endregion

        #region RemoveBonusShips()
        /// <summary>
        /// If invaders have been dead for 200 milliseconds, remove them
        /// </summary>
        private void RemoveBonusShips()
        {
            for (int i = bonusShips.Count - 1; i >= 0; i--)
            {
                if (!bonusShips[i].Alive() && bonusShips[i].TimeDead().Milliseconds >= 200)
                {
                    bonusShips.Remove(bonusShips[i]);
                }
            }
        }
        #endregion

        #region Go()
        /// <summary>
        /// Checks if the player is still alive and invaders are not at the bottom, then handles the gameplay.
        /// Twinkles stars, removes invaders, removes bonusships, moves invaders, if there's a boss, move the boss and keep track of if it has been defeated, move shots,
        /// spawn and move bonus ships, allow the invaders to fireback, checks for collisions, and then tries to send next wave.
        /// </summary>
        public void Go()
        {
            CheckIfAtBottom();
            if (!Alive)
            {
                return;
            }
            else if (!playerShip.Alive)
            {
                return;
            }

            else
            {
                stars.Twinkle(random);
                RemoveInvaders();
                RemoveBonusShips();
                framesSkipped++;
                MoveInvadersCheck();
                if (boss != null)
                {
                    MoveBoss();
                    CheckIfBossDefeated();
                }

                MoveShots();
                SpawnAndMoveBonusShips();

                ReturnFire();
                CheckForCollisions();

                SendNextWave();
            }
        }
        #endregion

        #region CheckIfBossDefeated()
        /// <summary>
        /// Checks if boss' health is 0 or below
        /// </summary>
        private void CheckIfBossDefeated()
        {
            if (boss.GetHealth() <= 0)
            {
                boss = null;
            }
        }
        #endregion

        #region CheckIfAtBottom()
        /// <summary>
        /// checks if any invader is at the bottom of the screen, if true, お前はもう死んでいる
        /// </summary>
        private void CheckIfAtBottom()
        {
            foreach (Invader invader in invaders)
            {
                if (invader.Location.Y >= playerShip.Location.Y -20)
                {
                    livesLeft = 0;
                    Alive = false;
                }
            }
        }
        #endregion

        #region MovePlayer(Direction direction, bool shiftHeld, bool controlHeld)
        /// <summary>
        /// Moves playerShip. You can still move your playerShip if you are in the life losing animation, but not if you game over. 
        /// If at the boundaries, doesn't let playerShip move offscreen
        /// </summary>
        /// <param left or right arrow key="direction"></param>
        /// <param shift key="shiftHeld"></param>
        /// <param control key="controlHeld"></param>
        public void MovePlayer(Direction direction, bool shiftHeld, bool controlHeld)
        {
            if (!Alive)
                return;
            else
            {
                playerShip.Move(direction, shiftHeld, controlHeld);
                if (!boundaries.Contains(playerShip.Location) || !boundaries.Contains(new Point(playerShip.Location.X + 60)))
                {
                    if (direction == Direction.Left)
                    {
                        playerShip.Move(Direction.Right, shiftHeld, controlHeld);
                    }
                    else
                    {
                        playerShip.Move(Direction.Left, shiftHeld, controlHeld);
                    }
                }
            }
        }
        #endregion

        #region DrawPoints(Graphics g, Invader invader, Brush color)
        /// <summary>
        /// Draw helper. Draws how many points you get when you defeat an enemy
        /// </summary>
        /// <param name="g"></param>
        /// <param name="invader"></param>
        /// <param name="color"></param>
        private void DrawPoints(Graphics g, Invader invader, Brush color)
        {
            g.DrawString(invader.Score.ToString(), arial15bold, Brushes.Black, 
                new Point(invader.Location.X + invader.Area.Width / 2 - 2, invader.Location.Y + 20 - 1));
            g.DrawString(invader.Score.ToString(), arial14bold, color,
                new Point(invader.Location.X + invader.Area.Width / 2, invader.Location.Y + 20));
        }
        #endregion

        #region DrawBonusPoints(Graphics g, BonusShip bonusShip, int score)
        /// <summary>
        /// Draw helper. Draws how many points you get when you defeat a bonusShip
        /// </summary>
        /// <param name="g"></param>
        /// <param name="bonusShip"></param>
        /// <param name="score"></param>
        private void DrawBonusPoints(Graphics g, BonusShip bonusShip, int score)
        {
            if (score >= 100 && score < 1000)
            g.DrawString(bonusShip.score.ToString(), arial15bold, Brushes.Black,
                new Point(bonusShip.Location.X + bonusShip.Area.Width / 2 - 2, bonusShip.Location.Y + 20 - 1));
            g.DrawString(bonusShip.score.ToString(), arial14bold, Brushes.Cyan,
                new Point(bonusShip.Location.X + bonusShip.Area.Width / 3, bonusShip.Location.Y + 20));
            if (score >= 1000 && score < 10000)
                g.DrawString(bonusShip.score.ToString(), arial15bold, Brushes.Purple,
                    new Point(bonusShip.Location.X + bonusShip.Area.Width / 2 - 2, bonusShip.Location.Y + 20 - 1));
            g.DrawString(bonusShip.score.ToString(), arial14bold, Brushes.Green,
                new Point(bonusShip.Location.X + bonusShip.Area.Width / 4, bonusShip.Location.Y + 20));
        }
        #endregion

        #region Draw(Graphics g, int animationCell)
        /// <summary>
        /// Draw Everything in the game object, from the background to the shots, to the labels.
        /// </summary>
        /// <param name="g">graphics</param>
        /// <param name="animationCell">animation cell</param>
        public void Draw(Graphics g, int animationCell)
        {
            g.FillRectangle(Brushes.Black, boundaries);

            stars.Draw(g);

            foreach (Invader invader in invaders)
            {
                invader.Draw(g, animationCell);
                if (!invader.Alive())
                {
                    if (invader.Score == 10)
                    {
                        DrawPoints(g, invader, Brushes.Green);
                    }
                    else if (invader.Score == 20)
                    {
                        DrawPoints(g, invader, Brushes.Blue);
                    }
                    else if (invader.Score == 30)
                    {
                        DrawPoints(g, invader, Brushes.Yellow);
                    }
                    else if (invader.Score == 40)
                    {
                        DrawPoints(g, invader, Brushes.Violet);
                    }
                    else if (invader.Score == 50)
                    {
                        DrawPoints(g, invader, Brushes.Orange);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
            }

            if (boss != null)
                boss.Draw(g, animationCell);

            playerShip.Draw(g);

            foreach (Shot shot in playerShots)
                shot.Draw(g);

            foreach (Shot shot in invaderShots)
                shot.Draw(g);

            foreach (Shot shot in bossShots)
                shot.Draw(g);

            for (int i = 0; i < shields.Count; i++)
            {
                foreach (Rectangle rectangle in shields[i].shield)
                {
                    g.FillRectangle(Brushes.Green, rectangle);
                }
            }

            foreach (BonusShip bonusShip in bonusShips)
            {
                bonusShip.Draw(g);
                if (!bonusShip.Alive())
                {
                    DrawBonusPoints(g, bonusShip, bonusShip.score);

                }
            }

            g.DrawString("Wave: " + (wave + 1), arial24bold, Brushes.Gold, 10, 10);
            g.DrawString("Credits earned: " + score, arial24bold, Brushes.Gold, 10, 40);
            g.DrawString("Lives: ", arial24bold, Brushes.Gold, 1720, 10);

            if (nextWave)
                g.DrawString("Next Wave", arial74bold, Brushes.Cyan, 600, 400);

            for (int i = 0; i < livesLeft; i++)
            {
                g.DrawImage(Properties.Resources.PlayerShip, new Point(1720 + i* 70, 40));
            }

            if (boss != null)
            {
                g.FillRectangle(Brushes.Red, new Rectangle(new Point(200, 10), new Size((int)(boss.GetHealth()*bossHealthBar), 30)));
                g.DrawRectangle(Pens.White, new Rectangle(new Point(200, 10), new Size(1400, 30)));
            }

            if (!Alive)
            {
                 g.DrawString("GAME OVER", arial74bold, Brushes.Cyan, 600, 400);
                 g.DrawString("GAME OVER", arial74bold, Brushes.Gold, 605, 405);

            }
        }
        #endregion

        #region Twinkle()
        /// <summary>
        /// Calls the stars.Twinkle(random) method
        /// </summary>
        public void Twinkle()
        {
            stars.Twinkle(random);
        }
        #endregion

        #region MoveInvaders()
        /// <summary>
        /// Loops through each invader and checks if they are at a side. If they are, move down. 
        /// Otherwise, move left or right until you reach a side
        /// </summary>
        public void MoveInvaders()
        {
            for(int i = 0; i < invaders.Count; i++)
            {
                if (invaderDirection == Direction.Left)
                {
                    if (invaders[i].Location.X - 20 < 0)
                    {
                        invaderDirection = Direction.Down;
                        atLeftSide = true;
                    }
                    atRightSide = false;
                }
                if (invaderDirection == Direction.Right)
                {
                    if (invaders[i].Location.X + 100 > boundaries.Width)
                    {
                        invaderDirection = Direction.Down;
                        atRightSide = true;
                    }
                    atLeftSide = false;
                }
            }
            foreach (Invader invader in invaders)
            {
                invader.Move(invaderDirection);
            }
            if (invaderDirection == Direction.Down)
            {
                if (atRightSide)
                {
                    invaderDirection = Direction.Left;

                }
                if (atLeftSide)
                {
                    invaderDirection = Direction.Right;

                }
            }
        }
        #endregion

        #region MoveBoss()
        /// <summary>
        /// Moves the boss if there is one
        /// </summary>
        public void MoveBoss()
        {
            if (boss != null)
            {
                boss.Move();
            }
        }
        #endregion

        #region ReturnFire()
        /// <summary>
        /// Chooses bottom invader per row, and makes them fire progressively harder
        /// </summary>
        private void ReturnFire()
        {
            for (int i = 0; i < invaderShots.Count; i++)
            {
                if (!boundaries.Contains(invaderShots[i].Location))
                {
                    invaderShots.Remove(invaderShots[i]);
                }
            }

            if (invaderShots.Count == wave + 1)
            {
                return;
            }

            if (random.Next(1000) >= 0 + wave * 10 )
            {
                return;
            }

            if (invaders.Count == 0)
            {
                return;
            }

            var shooter = from invader in invaders
                          group invader by invader.Location.X into invaderGroups
                          orderby invaderGroups.Key descending
                          select invaderGroups;

            Invader actualShooter = shooter.ElementAt(random.Next(shooter.Count())).Last();
            invaderShots.Add(new Shot(new Point(actualShooter.Location.X + actualShooter.Area.Width / 3 * 2,
                actualShooter.Location.Y + actualShooter.Area.Height), Direction.Down, boundaries));
        }
        #endregion

        #region NextWave()
        /// <summary>
        /// Creates the enemies depending on what wave it is. Increases wave by 1
        /// </summary>
        public void NextWave()
        {
            invaders.Clear();
            wave++;
            if (wave >= 0 && wave < 9)
            {
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        switch (j)
                        {
                            case 0:
                                invaders.Add(new Invader(ShipType.invader5, new Point(i * 100 + 100, j * 60 + 100), 50));
                                break;
                            case 1:
                                invaders.Add(new Invader(ShipType.invader4, new Point(i * 100 + 100, j * 60 + 100), 40));
                                break;
                            case 2:
                                invaders.Add(new Invader(ShipType.invader3, new Point(i * 100 + 100, j * 60 + 100), 30));
                                break;
                            case 3:
                                invaders.Add(new Invader(ShipType.invader2, new Point(i * 100 + 100, j * 60 + 100), 20));
                                break;
                            case 4:
                                invaders.Add(new Invader(ShipType.invader1, new Point(i * 100 + 100, j * 60 + 100), 10));
                                break;
                        }
                    }
                }
            }
            invaderDirection = Direction.Right;
            if (wave == 9)
            {
                Boss1();
            }
            else if (wave >= 10 && wave < 19)
            {
                for (int i = 0; i < 7; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        switch (j)
                        {
                            case 0:
                                invaders.Add(new Invader(ShipType.invader5, new Point(i * 100 + 100, j * 60 + 100), 50));
                                break;
                            case 1:
                                invaders.Add(new Invader(ShipType.invader4, new Point(i * 100 + 100, j * 60 + 100), 40));
                                break;
                            case 2:
                                invaders.Add(new Invader(ShipType.invader3, new Point(i * 100 + 100, j * 60 + 100), 30));
                                break;
                            case 3:
                                invaders.Add(new Invader(ShipType.invader2, new Point(i * 100 + 100, j * 60 + 100), 20));
                                break;
                            case 4:
                                invaders.Add(new Invader(ShipType.invader1, new Point(i * 100 + 100, j * 60 + 100), 10));
                                break;
                        }
                    }
                }
            }
            else if (wave == 19)
            {
                Boss2();
            }
            else if (wave >= 20 && wave < 29)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        switch (j)
                        {
                            case 0:
                                invaders.Add(new Invader(ShipType.invader5, new Point(i * 100 + 100, j * 60 + 100), 50));
                                break;
                            case 1:
                                invaders.Add(new Invader(ShipType.invader4, new Point(i * 100 + 100, j * 60 + 100), 40));
                                break;
                            case 2:
                                invaders.Add(new Invader(ShipType.invader3, new Point(i * 100 + 100, j * 60 + 100), 30));
                                break;
                            case 3:
                                invaders.Add(new Invader(ShipType.invader2, new Point(i * 100 + 100, j * 60 + 100), 20));
                                break;
                            case 4:
                                invaders.Add(new Invader(ShipType.invader1, new Point(i * 100 + 100, j * 60 + 100), 10));
                                break;
                        }
                    }
                }
            }
            else if (wave == 29)
            {
                Boss3();
            }
            else if (wave >= 30)
            {
                for (int i = 0; i < 11; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        switch (j)
                        {
                            case 0:
                                invaders.Add(new Invader(ShipType.invader5, new Point(i * 100 + 100, j * 60 + 100), 50));
                                break;
                            case 1:
                                invaders.Add(new Invader(ShipType.invader4, new Point(i * 100 + 100, j * 60 + 100), 40));
                                break;
                            case 2:
                                invaders.Add(new Invader(ShipType.invader3, new Point(i * 100 + 100, j * 60 + 100), 30));
                                break;
                            case 3:
                                invaders.Add(new Invader(ShipType.invader2, new Point(i * 100 + 100, j * 60 + 100), 20));
                                break;
                            case 4:
                                invaders.Add(new Invader(ShipType.invader1, new Point(i * 100 + 100, j * 60 + 100), 10));
                                break;
                        }
                    }
                }
            }
        }
        #endregion

        #region Boss1()
        /// <summary>
        /// First boss spawner
        /// </summary>
        private void Boss1()
        {
            invaders.Clear();
            boss = new Boss(playerShip, new Point(20, 100), bossShots, boundaries, 150, 1, random);
            bossHealthBar = 1400D / boss.GetHealth();
        }
        #endregion

        #region Boss2()
        /// <summary>
        /// Second boss spawner
        /// </summary>
        private void Boss2()
        {
            invaders.Clear();
            boss = new Boss(playerShip, new Point(20, 100), bossShots, boundaries, 300, 2, random);
            bossHealthBar = 1400D / boss.GetHealth();
        }
        #endregion

        #region Boss3()
        /// <summary>
        /// Second boss spawner
        /// </summary>
        private void Boss3()
        {
            invaders.Clear();
            boss = new Boss(playerShip, new Point(20, 100), bossShots, boundaries, 500, 3, random);
            bossHealthBar = 1400D / boss.GetHealth();
            //MessageBox.Show(boss.Location.ToString());
        }
        #endregion
    }
}
