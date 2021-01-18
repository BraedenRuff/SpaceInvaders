using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing; 

namespace Lab3SpaceInvaders
{
    class SuperRareBonusShip : BonusShip
    {
        #region constructor
        public SuperRareBonusShip(Point Location)
            :base(Location)
        {
            image = Properties.Resources.PlayerShip;
            score = 5000;
        }
        #endregion
    }
}
