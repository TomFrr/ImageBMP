using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pixel_Code
{
    internal class Pixel
    {
        #region attributs
        private int red;
        private int green;
        private int blue;
        #endregion

        #region accesseur
        public int Red
        {
            get { return red; }
            set { red = value; }
        }
        public int Green
        {
            get { return green; }
            set { green = value; }
        }
        public int Blue
        {
            get { return blue; }
            set { blue = value; }
        }
        #endregion

        #region constructeur
        public Pixel(int red, int green, int blue)
        {
            this.red = red;
            this.green = green;
            this.blue = blue;
        }
        #endregion
    }
}
