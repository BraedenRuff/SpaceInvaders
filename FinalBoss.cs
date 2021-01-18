using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lab3SpaceInvaders
{
    class FinalBoss : Boss
    {
        private int framesSkipped = 0;
        private bool attacking = false;
        private Direction direction;
        public FinalBoss(PlayerShip playerShip, Point Location, List<Shot> bossShots, Rectangle boundaries, int health, int bossNumber, Random random) 
            : base(playerShip, Location, bossShots, boundaries, health, bossNumber, random)
        {

        }

        public override void Move()
        {
           
        }

        public override Bitmap BossImage(int animationCell)
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
    }
}
