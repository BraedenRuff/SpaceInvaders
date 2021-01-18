using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WMPLib;

namespace Lab3SpaceInvaders
{
    public partial class Form1 : Form
    {
        #region Properties and fields
        WindowsMediaPlayer player = new WindowsMediaPlayer();
        WindowsMediaPlayer player2 = new WindowsMediaPlayer();
        Font arial24bold = new Font("Arial", 24, FontStyle.Bold);
        Font arial16bold = new Font("Arial", 16, FontStyle.Bold);
        Font arial72bold = new Font("Arial", 72, FontStyle.Bold);
        private int holdFrame = 0;
        private bool shiftHeld = false;
        private bool controlHeld = false;
        private Game game;
        private bool inUpgradeShop = false;
        private bool inMainMenu = true;
        private bool inWeaponShop = false;
        private bool inLeaderBoard = false;
        private bool inRecordName = false;
        private int recordNameLocation;
        private List<int> highestWave;
        private int totalScore = 0;
        private int animationCell = 0;
        private int numShots = 2;
        private int piercing = 0;
        private bool boomerangEquipped = false;
        private bool chargeEquipped = false;
        private bool fireable = false;
        private bool chargeBall = false;
        private bool homingEquipped = false;
        private bool boomerang = false;
        private bool charge = false;
        private bool homing = false;
        private bool gamePaused = false;
        private int rateOfFire = 0;
        private Point PCQuality = new Point(10, 700);
        private DateTime waitTimerBegin;
        private DateTime waitTimerEnd;
        private TimeSpan span1;
        private DateTime waitTimer2Begin;
        private DateTime waitTimer2End;
        private TimeSpan span2;
        private DateTime backgroundTimerBegin;
        private DateTime backgroundTimerEnd;
        private TimeSpan span3;
        private Random random;
        private List<Keys> keysPressed = new List<Keys>();
        private List<String> recordName = new List<String>();
        private Stars stars;
        private Point showLocation;
        #endregion

        #region Constructor
        public Form1()
        {
            InitializeComponent();
            player.URL = "2017-12-08_21-56-04.mp4";
            player.controls.stop();
            player2.URL = "Dubmood - Chiptune.mp3";
            random = new Random();
            stars = new Stars(ClientRectangle, random);
            highestWave = new List<int>();
            recordName.Add("---");
            recordName.Add("---");
            recordName.Add("---");
            recordName.Add("---");
        }
        #endregion

        #region Charge()
        /// <summary>
        /// Charge shots based on timespan. Sets chargeBall and fireable to true if enough time has passed.
        /// </summary>
        private void Charge()
        {
            if (waitTimer2Begin.Year != DateTime.Now.Year)
                waitTimer2Begin = DateTime.Now;
            waitTimer2End = DateTime.Now;
            span2 = waitTimer2End - waitTimer2Begin;
            if (span2.Seconds >= 1)
            {
                fireable = true;
            }
            if (span2.Seconds >= 0)
            {
                chargeBall = true;
            }
        }
        #endregion

        #region Form1_KeyDown(object sender, KeyEventArgs e)
        /// <summary>
        /// Tracks what keys are pressed down. Most recent press is at the first index
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">key pressed</param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!inLeaderBoard)
            { 
                if (e.KeyCode == Keys.ShiftKey)
                    shiftHeld = true;
                if (e.KeyCode == Keys.ControlKey)
                    controlHeld = true;
                if (e.KeyCode == Keys.P)
                {
                    if (game == null)
                    {
                        if (gamePaused)
                        {
                            gamePaused = false;
                            PauseGame();
                        }
                        return;
                    }
                    if (!gamePaused)
                    {
                        gamePaused = true;
                        PauseGame();
                    }
                    else
                    {
                        gamePaused = false;
                        PauseGame();
                    }
                }
                if (e.KeyCode == Keys.D1 && boomerang == true)
                {
                    if (boomerangEquipped == true)
                    {
                        boomerangEquipped = false;
                        if (game != null)
                        {
                            game.SwitchBoomerang();
                        }
                    }
                    else
                    {
                        boomerangEquipped = true;
                        if (game != null)
                        {
                            game.SwitchBoomerang();
                        }
                    }
                }
                if (e.KeyCode == Keys.D2 && charge == true)
                {
                    if (chargeEquipped == true)
                    {
                        chargeEquipped = false;
                        if (game != null)
                        {
                            game.SwitchCharge();
                        }
                    }
                    else
                    {
                        chargeEquipped = true;
                        if (game != null)
                        {
                            game.SwitchCharge();
                        }
                    }
                }
                if (e.KeyCode == Keys.D3 && homing == true)
                {
                    if (homingEquipped == true)
                    {
                        homingEquipped = false;
                        if (game != null)
                        {
                            game.SwitchHoming();
                        }
                    }
                    else
                    {
                        homingEquipped = true;
                        if (game != null)
                        {
                            game.SwitchHoming();
                        }
                    }
                }
                if (e.KeyCode == Keys.Enter)
                {
                    if (inMainMenu)
                    {
                        inMainMenu = false;
                        game = new Game(ClientRectangle, piercing, numShots, boomerangEquipped, chargeEquipped, homingEquipped, random);
                    }
                }
                if (keysPressed.Contains(e.KeyCode))
                    keysPressed.Remove(e.KeyCode);
                keysPressed.Add(e.KeyCode);
            }
            else
            {
                if (e.KeyCode == Keys.Enter)
                {
                    inLeaderBoard = false;
                    inMainMenu = true;
                }
            }
        }
        #endregion

        #region Form1_KeyUp(object sender, KeyEventArgs e)
        /// <summary>
        /// Removes keys not pressed anymore. Charge shots fired here. Sets fireable and chargeBall to false. Resets waitTimer2Begin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">key released</param>
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                if (chargeEquipped)
                {
                    if (fireable)
                    {
                        game.FireShot();
                        fireable = false;
                        chargeBall = false;
                        waitTimer2Begin = new DateTime();
                    }
                    else
                    {
                        chargeBall = false;
                        waitTimer2Begin = new DateTime();
                    }
                }
                else
                {
                    rateOfFire = 4;
                }
            }
            if (keysPressed.Contains(e.KeyCode))
                keysPressed.Remove(e.KeyCode);
            if (!keysPressed.Contains(Keys.ShiftKey))
                shiftHeld = false;
            if (!keysPressed.Contains(Keys.ControlKey))
                controlHeld = false;
        }
        #endregion

        #region HandleBackgroundMusic()
        /// <summary>
        /// Makes background music loop.
        /// </summary>
        private void HandleBackgroundMusic()
        {
            player2.settings.volume = 10;
            if (backgroundTimerBegin.Year != DateTime.Now.Year)
                backgroundTimerBegin = DateTime.Now;
            backgroundTimerEnd = DateTime.Now;
            span3 = backgroundTimerEnd - backgroundTimerBegin;
            if (14 == span3.Seconds && 3 == span3.Minutes)
            {
                player2.controls.stop();
                player2.controls.play();
                backgroundTimerBegin = new DateTime();
            }
        }
        #endregion 

        #region gameTimer_Tick(object sender, EventArgs e)
        /// <summary>
        /// Tells game to go. Handles background music. Twinkles stars. Checks if the game needs to be paused. 
        /// Checks if you have been dead for 3 seconds(game over) and if true, brings you back to the main menu.
        /// Moves playerShip.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">gameTimer tick</param>
        private void gameTimer_Tick(object sender, EventArgs e)
        {
            HandleBackgroundMusic();
            stars.Twinkle(random);
            if (keysPressed.Contains(Keys.Space))
            {
                if (game != null)
                {
                    if (chargeEquipped)
                    {
                        Charge();
                    }
                    else
                    {
                        rateOfFire++;
                        if (rateOfFire == 5)
                        {
                            game.FireShot();
                            rateOfFire = 0;
                        }
                    }
                }
            }
            if (game != null)
            {
                PauseGame();
                if (!game.Alive)
                {
                    if (waitTimerBegin.Year != DateTime.Now.Year)
                        waitTimerBegin = DateTime.Now;
                    waitTimerEnd = DateTime.Now;
                    span1 = waitTimerEnd - waitTimerBegin;
                    if (span1.Seconds >= 3 && game.GetWave() <= 30)
                    {
                        totalScore += game.score;
                        game = null;
                        waitTimerBegin = new DateTime();
                        inMainMenu = true;
                        return;
                    }
                    else if (span1.Seconds >= 3 && game.GetWave() > 30)
                    {
                        totalScore += game.score;
                        if (game != null)
                        {
                            if (highestWave.Count != 0)
                            {
                                if (game.GetWave() >= highestWave[0])
                                {
                                    if (highestWave.Count == 3)
                                    {
                                        highestWave[2] = highestWave[1];
                                        highestWave[1] = highestWave[0];
                                        highestWave[0] = game.GetWave();
                                        if (game.GetWave() <= highestWave[0] && game.GetWave() <= highestWave[1] && game.GetWave() <= highestWave[2])
                                        {
                                            recordNameLocation = 3;
                                            goto jmp;
                                        }

                                        if (game.GetWave() <= highestWave[0] && game.GetWave() <= highestWave[1] && game.GetWave() > highestWave[2])
                                        {
                                            recordNameLocation = 1;
                                            goto jmp;
                                        }
                                        if (game.GetWave() <= highestWave[0] && game.GetWave() > highestWave[1] && game.GetWave() > highestWave[2])
                                        {
                                            recordNameLocation = 0;
                                            goto jmp;
                                        }
                                        if (game.GetWave() > highestWave[0] && game.GetWave() > highestWave[1] && game.GetWave() > highestWave[2])
                                        {
                                            recordNameLocation = 0;
                                            goto jmp;
                                        }
                                    }
                                    if (highestWave.Count == 2)
                                    {
                                        highestWave.Add(highestWave[1]);
                                        highestWave[1] = highestWave[0];
                                        highestWave[0] = game.GetWave();
                                        if (game.GetWave() <= highestWave[0] && game.GetWave() <= highestWave[1] && game.GetWave() <= highestWave[2])
                                        {
                                            recordNameLocation = 2;
                                            goto jmp;
                                        }
                                    }
                                    if (highestWave.Count == 1)
                                    {
                                        highestWave.Add(highestWave[0]);
                                        highestWave[0] = game.GetWave();
                                        if (game.GetWave() <= highestWave[0] && game.GetWave() <= highestWave[1])
                                        {
                                            recordNameLocation = 1;
                                            goto jmp;
                                        }
                                    }
                                    if (game.GetWave() > highestWave[0])
                                    {
                                        recordNameLocation = 0;
                                        goto jmp;
                                    }
                                }
                                else if (highestWave.Count >= 2)
                                {
                                    if (game.GetWave() > highestWave[1] && game.GetWave() <= highestWave[0])
                                    {
                                        if (highestWave.Count == 3)
                                        {
                                            highestWave[2] = highestWave[1];
                                            highestWave[1] = game.GetWave();

                                        }
                                        if (highestWave.Count == 2)
                                        {
                                            highestWave.Add(highestWave[1]);
                                            highestWave[1] = game.GetWave();

                                        }
                                        if (game.GetWave() > highestWave[1])
                                        {
                                            recordNameLocation = 1;
                                            goto jmp;
                                        }
                                    }
                                }
                                else if (highestWave.Count >= 3)
                                {
                                    if (game.GetWave() <= highestWave[1] && game.GetWave() > highestWave[2])
                                    {
                                        highestWave[2] = game.GetWave();
                                        recordNameLocation = 2;
                                        goto jmp;
                                    }
                                }
                                if (game.GetWave() > highestWave[2])
                                {
                                    recordNameLocation = 3;
                                    goto jmp;
                                }
                            }
                            else
                            {
                                highestWave.Add(game.GetWave());
                                recordNameLocation = 0;
                            }
                            jmp:
                            game = null;
                            waitTimerBegin = new DateTime();
                            inRecordName = true;
                            return;
                        }
                    }
                }
                game.Go();
                foreach (Keys key in keysPressed)
                {
                    if (key == Keys.Left)
                    {
                        game.MovePlayer(Direction.Left, shiftHeld, controlHeld);
                        return;
                    }
                    else if (key == Keys.Right)
                    {
                        game.MovePlayer(Direction.Right, shiftHeld, controlHeld);
                        return;
                    }
                }
            }
        }
        #endregion

        #region PauseGame()
        /// <summary>
        /// Stop gameTimer and pauses music, or start gameTimer and unPause music. Depending if gamePaused is true or not.
        /// </summary>
        private void PauseGame()
        {
            if (gamePaused)
            {
                gameTimer.Stop();
                player2.controls.pause();
            }
            else
            {
                if (gameTimer.Enabled)
                {
                    return;
                }
                gameTimer.Start();
                player2.controls.play();
            }
        }
        #endregion

        #region animationTimer_Tick(object sender, EventArgs e)
        /// <summary>
        /// How often the game animates. Also changes animationCell every 10 ticks. Calls Invalidate().
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">animationTimer tick</param>
        private void animationTimer_Tick(object sender, EventArgs e)
        {
            holdFrame++;
            if (animationCell == 0 && holdFrame >= 10)
            {
                holdFrame = 0;
                animationCell = 1;
            }
            else if (animationCell == 1 && holdFrame >= 10)
            {
                holdFrame = 0;
                animationCell = 0;
            }
            Invalidate();
        }
        #endregion

        #region Form1_Paint(object sender, PaintEventArgs e)
        /// <summary>
        /// Tells game to draw.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">form1 paint event</param>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (inMainMenu)
            {
                g.FillRectangle(Brushes.Black, ClientRectangle);
                stars.Draw(g);
                MakeMenuOption(g, 250, "Defend base");
                MakeMenuOption(g, 450, "Upgrade ship");
                MakeMenuOption(g, 650, "Quit");
                MakeMenuOption(g, 1550, 850, 300, 150, "+1000000 credits");
                MakeMenuOption(g, 10, 550, 300, 150, "My PC sucks");
                MakeMenuOption(g, 10, 700, 300, 150, "My PC is OK");
                MakeMenuOption(g, 10, 850, 300, 150, "My PC is fast");
                g.DrawRectangle(Pens.White, new Rectangle(PCQuality, new Size(300, 150)));
                g.DrawString("Credits: " + totalScore.ToString(), arial24bold, Brushes.Gold, new Point(10, 10));
                g.DrawImage(Properties.Resources.Directions1, 1200, 50);
                g.DrawImage(Properties.Resources.Directions2, 50, 50);
            }
            if (inUpgradeShop)
            {
                g.FillRectangle(Brushes.Black, ClientRectangle);
                stars.Draw(g);
                MakeMenuOption(g, 150, "+Number of shots", 1500, numShots.ToString());
                MakeMenuOption(g, 350, "+Piercing", 2000, piercing.ToString());
                MakeMenuOption(g, 550, "Weapon Shop");
                MakeMenuOption(g, 750, "Main Menu");
                g.DrawString("Credits: " + totalScore.ToString(), arial24bold, Brushes.Gold, new Point(10, 10));
            }
            if (inRecordName)
            {
                g.FillRectangle(Brushes.Firebrick, ClientRectangle);
                stars.Draw(g);
                textBox1.Location = new Point(ClientRectangle.Width / 2 - textBox1.Width / 2, ClientRectangle.Height / 2 - textBox1.Height / 2);
                if (textBox1.Visible == false)
                {
                    textBox1.Visible = true;
                    textBox1.Focus();
                    textBox1.BringToFront();
                }
                if (textBox1.Text.Length > 3)
                {
                    textBox1.Text = (textBox1.Text[0].ToString() + textBox1.Text[1].ToString() + textBox1.Text[2].ToString());
                }
                g.DrawImage(Properties.Resources.Directions3, 50, 50);
            }
            if (inLeaderBoard)
            {
                g.FillRectangle(Brushes.Firebrick, ClientRectangle);
                stars.Draw(g);
                DrawLeaderBoard(g);
                g.DrawImage(Properties.Resources.Directions4, 1220, 600);
            }
            if (inWeaponShop)
            {
                g.FillRectangle(Brushes.Black, ClientRectangle);
                stars.Draw(g);
                MakeMenuOption(g, 150, "Boomerang Shot", 20000, boomerang);
                if (boomerang)
                {
                    MakeMenuOption(g, ClientRectangle.Width / 2 + 300 / 2 + 10, 150, 180, 150, "Equip");
                    if (boomerangEquipped)
                    {
                        g.DrawRectangle(Pens.White, new Rectangle(new Point(ClientRectangle.Width / 2 + 300 / 2 + 10, 150), new Size(180, 150)));
                    }
                }
                MakeMenuOption(g, 350, "Charge Shot", 25000, charge);
                if (charge)
                {
                    MakeMenuOption(g, ClientRectangle.Width / 2 + 300 / 2 + 10, 350, 180, 150, "Equip");
                    if (chargeEquipped)
                    {
                        g.DrawRectangle(Pens.White, new Rectangle(new Point(ClientRectangle.Width / 2 + 300 / 2 + 10, 350), new Size(180, 150)));
                    }
                }
                MakeMenuOption(g, 550, "Homing Shot", 50000, homing);
                if (homing)
                {
                    MakeMenuOption(g, ClientRectangle.Width / 2 + 300 / 2 + 10, 550, 180, 150, "Equip");
                    if (homingEquipped)
                    {
                        g.DrawRectangle(Pens.White, new Rectangle(new Point(ClientRectangle.Width / 2 + 300 / 2 + 10, 550), new Size(180, 150)));
                    }
                }
                MakeMenuOption(g, 10, 850, 300, 150, "Upgrade Ship");
                MakeMenuOption(g, 1550, 850, 300, 150, "Main Menu");
                g.DrawString("Credits: " + totalScore.ToString(), arial24bold, Brushes.Gold, new Point(10, 10));
            }
            if (game != null)
            {
                game.Draw(g, animationCell);
                if (gamePaused)
                {
                    MakeMenuOption(g, 350, "Resume");
                    MakeMenuOption(g, 550, "Main menu");
                }
                if (game.Alive)
                {
                    if (chargeBall)
                    {
                        g.FillEllipse(Brushes.LightYellow, new Rectangle(new Point(game.playerShip.Location.X + game.playerShip.Area.Width / 2 + 2,
                            game.playerShip.Location.Y + game.playerShip.Area.Height / 3 - 2), new Size(10, 10)));
                    }
                    if (fireable)
                    {
                        g.FillEllipse(Brushes.LightYellow, new Rectangle(new Point(game.playerShip.Location.X + game.playerShip.Area.Width / 2 - 3,
                            game.playerShip.Location.Y + game.playerShip.Area.Height / 3 - 10), new Size(20, 20)));
                    }
                }
            }
            //if (showLocation != null)
            //{
            //    g.DrawString(showLocation.X.ToString() + " x: " + showLocation.Y.ToString() + " y: ", arial24bold, Brushes.Gold, 500, 500);
            //}
        }
        #endregion

        private void DrawLeaderBoard(Graphics g)
        {
            if (highestWave.Count == 0)
            {
                return;
            }
            if (highestWave.Count == 1)
            {
                g.FillRectangle(Brushes.DarkRed, new Rectangle(new Point(100, 100), new Size(ClientRectangle.Width - 200, 750)));
                g.DrawString("1: " + recordName[0] + " Highest Wave: " + highestWave[0], arial72bold, Brushes.Gold, 150, 150 + 10 + 50);
            }
            if (highestWave.Count == 2)
            {
                g.FillRectangle(Brushes.DarkRed, new Rectangle(new Point(100, 100), new Size(ClientRectangle.Width - 200, 750)));
                g.DrawString("1: " + recordName[0] + " Highest Wave: " + highestWave[0], arial72bold, Brushes.Gold, 150, 150 + 10 + 50);
                g.DrawString("2: " + recordName[1] + " Highest Wave: " + highestWave[1], arial72bold, Brushes.Gold, 150, 150 + 10 + 240);
            }
            else if (highestWave.Count > 2)
            {
                g.FillRectangle(Brushes.DarkRed, new Rectangle(new Point(100, 100), new Size(ClientRectangle.Width - 200, 750)));
                g.DrawString("1: " + recordName[0] + " Highest Wave: " + highestWave[0], arial72bold, Brushes.Gold, 150, 150 + 10 + 50);
                g.DrawString("2: " + recordName[1] + " Highest Wave: " + highestWave[1], arial72bold, Brushes.Gold, 150, 150 + 10 + 240);
                g.DrawString("3: " + recordName[2] + " Highest Wave: " + highestWave[2], arial72bold, Brushes.Gold, 150, 150 + 10 + 430);
            }
        }

        #region MenuOptions



        #region MakeMenuOption(Graphics g, int x, int y, int sizeX, int sizeY, string text)
        /// <summary>
        /// Most flexible constructor. Decide size of rectangle, and location
        /// </summary>
        /// <param name="g"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <param name="text"></param>
        private void MakeMenuOption(Graphics g, int x, int y, int sizeX, int sizeY, string text)
        {
            g.FillRectangle(Brushes.DarkRed, new Rectangle(new Point(x, y), new Size(sizeX, sizeY)));
            SizeF stringSize = g.MeasureString(text, arial24bold);
            int stringX = sizeX - (int)stringSize.Width;
            int stringY = sizeY - (int)stringSize.Height;
            g.DrawString(text, arial24bold, Brushes.White, new Point(x + stringX/2, y + stringY/2));
        }
        #endregion

        #region MakeMenuOption(Graphics g, int y, string text)
        /// <summary>
        /// Default constructor. Centers rectangles on X axis, decide where on the Y to put them and their text
        /// </summary>
        /// <param name="g"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        private void MakeMenuOption(Graphics g, int y, string text)
        {
            int sizeX = 300;
            int sizeY = 150;
            int x = ClientRectangle.Width / 2 - sizeX/2;
            g.FillRectangle(Brushes.DarkRed, new Rectangle(new Point(x, y), new Size(sizeX, sizeY)));
            SizeF stringSize = g.MeasureString(text, arial24bold);
            int stringX = sizeX - (int)stringSize.Width;
            int stringY = sizeY - (int)stringSize.Height;
            g.DrawString(text, arial24bold, Brushes.White, new Point(x + stringX / 2, y + stringY / 2));
        }
        #endregion

        #region MakeMenuOption(Graphics g, int y, string text, int cost, bool have)
        /// <summary>
        /// Constructor for if you are buying stuff.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        /// <param name="cost"></param>
        private void MakeMenuOption(Graphics g, int y, string text, int cost, bool have)
        {
            int sizeX = 300;
            int sizeY = 150;
            int x = ClientRectangle.Width / 2 - sizeX / 2;

            if (!have)
            {
                if (totalScore >= cost)
                {
                    g.FillRectangle(Brushes.DarkRed, new Rectangle(new Point(x, y), new Size(sizeX, sizeY)));
                }
                else
                {
                    g.FillRectangle(Brushes.Gray, new Rectangle(new Point(x, y), new Size(sizeX, sizeY)));
                }
                SizeF stringSize = g.MeasureString(text, arial24bold);
                int stringX = sizeX - (int)stringSize.Width;
                int stringY = sizeY - (int)stringSize.Height;
                g.DrawString(text, arial24bold, Brushes.White, new Point(x + stringX / 2, y + stringY / 3));
                SizeF stringSize2 = g.MeasureString("(" + cost + ")", arial16bold);
                int stringX2 = sizeX - (int)stringSize2.Width;
                int stringY2 = sizeY - (int)stringSize2.Height;
                g.DrawString("(" + cost + ")", arial16bold, Brushes.White, new Point(x + stringX2 / 2, y + stringY2 / 3 * 2));
            }
            else
            {
                g.FillRectangle(Brushes.Gray, new Rectangle(new Point(x, y), new Size(sizeX, sizeY)));
                SizeF boughtString = g.MeasureString("Bought", arial24bold);
                int boughtStringX = sizeX - (int)boughtString.Width;
                int boughtStringY = sizeY - (int)boughtString.Height;
                g.DrawString("Bought", arial24bold, Brushes.White, new Point(x + boughtStringX / 2, y + boughtStringY / 2));
            }
        }
        #endregion

        #region MakeMenuOption(Graphics g, int y, string text, int cost, string currentAmount)
        /// <summary>
        /// Constructor for if you are buying stuff.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="y"></param>
        /// <param name="text"></param>
        /// <param name="cost"></param>
        private void MakeMenuOption(Graphics g, int y, string text, int cost, string currentAmount)
        {
            int sizeX = 300;
            int sizeY = 150;
            int x = ClientRectangle.Width / 2 - sizeX / 2;

            if (totalScore >= cost)
            {
                g.FillRectangle(Brushes.DarkRed, new Rectangle(new Point(x, y), new Size(sizeX, sizeY)));
            }
            else
            {
                g.FillRectangle(Brushes.Gray, new Rectangle(new Point(x, y), new Size(sizeX, sizeY)));
            }
            SizeF stringSize = g.MeasureString(text, arial24bold);
            int stringX = sizeX - (int)stringSize.Width;
            int stringY = sizeY - (int)stringSize.Height;
            g.DrawString(text, arial24bold, Brushes.White, new Point(x + stringX / 2, y + stringY / 4));
            SizeF stringSize2 = g.MeasureString("(" + cost + ")", arial16bold);
            int stringX2 = sizeX - (int)stringSize2.Width;
            int stringY2 = sizeY - (int)stringSize2.Height;
            g.DrawString("(" + cost + ")", arial16bold, Brushes.White, new Point(x + stringX2 / 2, y + stringY2 / 4 * 2));
            SizeF stringSize3 = g.MeasureString(currentAmount, arial16bold);
            int stringX3 = sizeX - (int)stringSize3.Width;
            int stringY3 = sizeY - (int)stringSize3.Height;
            g.DrawString(currentAmount, arial16bold, Brushes.White, new Point(x + stringX3 / 2, y + stringY3 / 4 * 3));
        }
        #endregion


        #endregion

        #region Form1_Click(object sender, EventArgs e)
        /// <summary>
        /// Finds out which screen you are on and then determines what clicking regions of the form do
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Click(object sender, EventArgs e)
        {
            var relativePoint = this.PointToClient(Cursor.Position);

            #region gamePaused
            if (gamePaused)
            {
                Rectangle resumeRectangle = new Rectangle(new Point(ClientRectangle.Width / 2 - 150, 350), new Size(300, 150));
                Rectangle mainMenuRectangle = new Rectangle(new Point(ClientRectangle.Width / 2 - 150, 550), new Size(300, 150));
                if (resumeRectangle.Contains(relativePoint))
                {
                    gamePaused = false;
                    PauseGame();
                    return;
                }
                if (mainMenuRectangle.Contains(relativePoint))
                {
                    totalScore += game.score;
                    game = null;
                    inMainMenu = true;
                    gamePaused = false;
                    PauseGame();
                    return;
                }
            }
            #endregion

            #region inMainMenu
            if (inMainMenu)
            {
                Rectangle slowPCRectangle = new Rectangle(new Point(10, 550), new Size(300, 150));
                Rectangle OKPCRectangle = new Rectangle(new Point(10, 700), new Size(300, 150));
                Rectangle fastPCRectangle = new Rectangle(new Point(10, 850), new Size(300, 150));
                Rectangle CreditRectangle = new Rectangle(new Point(1550, 850), new Size(300, 150));
                Rectangle newGameRectangle = new Rectangle(new Point(ClientRectangle.Width / 2 - 150, 250), new Size(300, 150));
                Rectangle upgradeShipRectangle = new Rectangle(new Point(ClientRectangle.Width / 2 - 150, 450), new Size(300, 150));
                Rectangle quitRectangle = new Rectangle(new Point(ClientRectangle.Width / 2 - 150, 650), new Size(300, 150));
                if (newGameRectangle.Contains(relativePoint))
                {
                    inMainMenu = false;
                    game = new Game(ClientRectangle, piercing, numShots, boomerangEquipped, chargeEquipped, homingEquipped, random);
                    return;
                }
                if (upgradeShipRectangle.Contains(relativePoint))
                {
                    inMainMenu = false;
                    inUpgradeShop = true;
                    return;
                }
                if (quitRectangle.Contains(relativePoint))
                {
                    Application.Exit();
                }
                if (CreditRectangle.Contains(relativePoint))
                {
                    totalScore += 1000000;
                }
                if (slowPCRectangle.Contains(relativePoint))
                {
                    animationTimer.Interval = 33;
                    PCQuality = new Point(10, 550);
                }
                if (OKPCRectangle.Contains(relativePoint))
                {
                    animationTimer.Interval = 20;
                    PCQuality = new Point(10, 700);
                }
                if (fastPCRectangle.Contains(relativePoint))
                {
                    animationTimer.Interval = 10;
                    PCQuality = new Point(10, 850);
                }

            }
            #endregion

            #region inUpgradeShop
            if (inUpgradeShop)
            {
                Rectangle numberOfShots = new Rectangle(new Point(ClientRectangle.Width / 2 - 150, 150), new Size(300, 150));
                Rectangle numPiercing = new Rectangle(new Point(ClientRectangle.Width / 2 - 150, 350), new Size(300, 150));
                Rectangle weaponShopRectangle = new Rectangle(new Point(ClientRectangle.Width / 2 - 150, 550), new Size(300, 150));
                Rectangle mainMenu = new Rectangle(new Point(ClientRectangle.Width / 2 - 150, 750), new Size(300, 150));
                if (numPiercing.Contains(relativePoint))
                {
                    if (totalScore >= 2000)
                    {
                        totalScore -= 2000;
                        piercing++;
                    }
                    else
                    {
                        player.controls.stop();
                        player.controls.play();
                    }
                    return;
                }
                if (numberOfShots.Contains(relativePoint))
                {
                    if (totalScore >= 1500)
                    {
                        totalScore -= 1500;
                        numShots++;
                    }
                    else
                    {
                        player.controls.stop();
                        player.controls.play();
                    }
                    return;
                }
                if (weaponShopRectangle.Contains(relativePoint))
                {
                    inUpgradeShop = false;
                    inWeaponShop = true;
                    return;
                }
                if (mainMenu.Contains(relativePoint))
                {
                    inUpgradeShop = false;
                    inMainMenu = true;
                    return;
                }
            }
            #endregion

            #region inWeaponShop
            if (inWeaponShop)
            {
                Rectangle boomerangRectangle = new Rectangle(new Point(ClientRectangle.Width / 2 - 150, 150), new Size(300, 150));
                Rectangle chargeRectangle = new Rectangle(new Point(ClientRectangle.Width / 2 - 150, 350), new Size(300, 150));
                Rectangle homingRectangle = new Rectangle(new Point(ClientRectangle.Width / 2 - 150, 550), new Size(300, 150));
                Rectangle upgradeShopRectangle = new Rectangle(new Point(10, 850), new Size(300, 150));
                Rectangle mainMenu = new Rectangle(new Point(1550, 850), new Size(300, 150));
                Rectangle boomerangEquipRectangle = new Rectangle(new Point(ClientRectangle.Width / 2 + 300 / 2 + 10, 150), new Size(180, 150));
                Rectangle chargeEquipRectangle = new Rectangle(new Point(ClientRectangle.Width / 2 + 300 / 2 + 10, 350), new Size(180, 150));
                Rectangle homingEquipRectangle = new Rectangle(new Point(ClientRectangle.Width / 2 + 300 / 2 + 10, 550), new Size(180, 150));
                if (boomerangRectangle.Contains(relativePoint))
                {
                    if (totalScore >= 20000 && boomerang != true)
                    {
                        totalScore -= 20000;
                        boomerang = true;
                        boomerangEquipped = true;
                    }
                    else
                    {
                        player.controls.stop();
                        player.controls.play();
                    }
                    return;
                }
                if (chargeRectangle.Contains(relativePoint))
                {
                    if (totalScore >= 25000 && charge != true)
                    {
                        totalScore -= 25000;
                        charge = true;
                        chargeEquipped = true;
                        if (numShots <= 7)
                        {
                            numShots = 7;
                        }
                    }
                    else
                    {
                        player.controls.stop();
                        player.controls.play();
                    }
                    return;
                }
                if (homingRectangle.Contains(relativePoint))
                {
                    if (totalScore >= 50000 && homing != true)
                    {
                        totalScore -= 50000;
                        homing = true;
                        homingEquipped = true;
                    }
                    else
                    {
                        player.controls.stop();
                        player.controls.play();
                    }
                }
                if (upgradeShopRectangle.Contains(relativePoint))
                {
                    inWeaponShop = false;
                    inUpgradeShop = true;
                    return;
                }
                if (mainMenu.Contains(relativePoint))
                {
                    inWeaponShop = false;
                    inMainMenu = true;
                    return;
                }
                if (boomerangEquipRectangle.Contains(relativePoint) && boomerang)
                {
                    if (boomerangEquipped)
                    {
                        boomerangEquipped = false;
                        return;
                    }
                    else
                    {
                        boomerangEquipped = true;
                        return;
                    }
                    
                }
                if (chargeEquipRectangle.Contains(relativePoint) && charge)
                {
                    if (chargeEquipped)
                    {
                        chargeEquipped = false;
                        return;
                    }
                    else
                    {
                        chargeEquipped = true;
                        return;
                    }

                }
                if (homingEquipRectangle.Contains(relativePoint) && homing)
                {
                    if (homingEquipped)
                    {
                        homingEquipped = false;
                        return;
                    }
                    else
                    {
                        homingEquipped = true;
                        return;
                    }

                }
            }
            #endregion
        }
        #endregion

        #region Form1_MouseMove(object sender, MouseEventArgs e)
        /// <summary>
        /// Used so I know where to put stuff. Debug only.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            var relativePoint = this.PointToClient(Cursor.Position);
            showLocation = relativePoint;
        }
        #endregion

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                textBox1.Text = textBox1.Text.ToUpper();
                if (textBox1.Text == "")
                {
                    textBox1.Text = "---";
                }
                recordName[recordNameLocation] = textBox1.Text;
                textBox1.Visible = false;
                this.Focus();
                inRecordName = false;
                inLeaderBoard = true;
                textBox1.Text = "";
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }
    }
}
