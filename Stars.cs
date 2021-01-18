using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Lab3SpaceInvaders
{
    class Stars
    {
        #region Properties and fields

        #region Star struct
        private struct Star
        {
            #region Star struct properties and fields
            public Point point;
            public Pen pen;
            #endregion

            #region Star struct constructor
            public Star(Point point, Pen pen)
            {
                this.point = point;
                this.pen = pen;
            }
            #endregion
        }
        #endregion

        List<Star> stars;
        Rectangle boundaries;
        Random random;
        #endregion

        #region Constructor
        public Stars(Rectangle boundaries, Random random)
        {
            this.boundaries = boundaries;
            this.random = random;
            stars = new List<Star>();
            for (int i = 0; i < 299; i++)
            {
                stars.Add(new Star(new Point(random.Next(boundaries.Width), random.Next(boundaries.Height)), Pens.White));
            }
        }
        #endregion

        #region Draw(Graphic g)
        /// <summary>
        /// Draw each star as a white circle
        /// </summary>
        /// <param name="g">Graphics</param>
        public void Draw(Graphics g)
        {
            int size = 3; // can also use random.Next(2,5) to simulate shining but i like it static
            for (int i = 0; i < stars.Count; i++)
                g.FillEllipse(Brushes.White, new Rectangle(stars[i].point, new Size(size, size)));
        }
        #endregion

        #region Twinkle(Random random)
        /// <summary>
        /// Remove 5 stars and add 5
        /// </summary>
        /// <param name="random">random instance</param>
        public void Twinkle(Random random)
        {
            for (int i = 0; i < 5; i++)
                stars.Remove(stars[random.Next(stars.Count)]);
            for (int j = 0; j < 5; j++)
                stars.Add(new Star(new Point(random.Next(boundaries.Width), random.Next(boundaries.Height)), Pens.White));
        }
        #endregion  
    }
}
