using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lab3SpaceInvaders
{
    class Shield
    {
        #region Proerties and fields
        public List<Rectangle> shield { get; set; }
        public Point Location { get; private set; }
        #endregion

        #region Constructor
        public Shield(Point Location)
        {
            this.Location = Location;
            shield = new List<Rectangle>();
            for (int i = 0; i < 100; i = i + 10)  //if it's laggy increase size of squares
            {
                for (int j = 0; j < 200; j = j + 10)
                {
                    shield.Add(new Rectangle(new Point(Location.X + j, Location.Y + i), new Size(10, 10)));
                }
            }

            #region hardcoding concave shape
            shield.Remove(shield[193]);
            shield.Remove(shield[192]);
            shield.Remove(shield[191]);
            shield.Remove(shield[190]);
            shield.Remove(shield[189]);
            shield.Remove(shield[188]);
            shield.Remove(shield[187]);
            shield.Remove(shield[186]);
            shield.Remove(shield[173]);
            shield.Remove(shield[172]);
            shield.Remove(shield[171]);
            shield.Remove(shield[170]);
            shield.Remove(shield[169]);
            shield.Remove(shield[168]);
            shield.Remove(shield[167]);
            shield.Remove(shield[166]);
            shield.Remove(shield[153]);
            shield.Remove(shield[152]);
            shield.Remove(shield[151]);
            shield.Remove(shield[150]);
            shield.Remove(shield[149]);
            shield.Remove(shield[148]);
            shield.Remove(shield[147]);
            shield.Remove(shield[146]);
            shield.Remove(shield[133]);
            shield.Remove(shield[132]);
            shield.Remove(shield[131]);
            shield.Remove(shield[130]);
            shield.Remove(shield[129]);
            shield.Remove(shield[128]);
            shield.Remove(shield[127]);
            shield.Remove(shield[126]);
            shield.Remove(shield[113]);
            shield.Remove(shield[112]);
            shield.Remove(shield[111]);
            shield.Remove(shield[110]);
            shield.Remove(shield[109]);
            shield.Remove(shield[108]);
            shield.Remove(shield[107]);
            shield.Remove(shield[106]);
            shield.Remove(shield[93]);
            shield.Remove(shield[92]);
            shield.Remove(shield[91]);
            shield.Remove(shield[90]);
            shield.Remove(shield[89]);
            shield.Remove(shield[88]);
            shield.Remove(shield[87]);
            shield.Remove(shield[86]);
            shield.Remove(shield[39]);
            shield.Remove(shield[20]);
            shield.Remove(shield[19]);
            shield.Remove(shield[18]);
            shield.Remove(shield[1]);
            shield.Remove(shield[0]);
            #endregion


        }
        #endregion
    }
}
