using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using GMap.NET.WindowsForms;
using GMap.NET;
using System.Drawing;
/// <summary>
/// 该文件来源自CSDN金左手，几处方法可能因版本不同进行了修改
/// http://blog.csdn.net/qq_17371831
/// 再次特别感谢！
/// </summary>
namespace GMapMarkerExt.Markers
{
    [Serializable]
    public class GMapMarkerDirection : GMapMarker, ISerializable
    {
        private float _ang = 0f;
        private Image _image = null;

        public Image Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                if (_image != null)
                {
                    this.Size = new Size(_image.Width, _image.Height);
                }
            }
        }

        public GMapMarkerDirection(PointLatLng pointLatLng, Image image, float angle)
            : base(pointLatLng)
        {
            this._image = image;
            this._ang = angle;
            if (this._image != null)
                this.Size = new Size(_image.Width, _image.Height);
            this.Offset = new Point(-this.Size.Width / 2, -this.Size.Height);
        }

        public override void OnRender(Graphics g)
        {

            g.DrawImageUnscaled(RotateImage(this._image, this._ang), LocalPosition.X, LocalPosition.Y);
        }

        //http://www.codeproject.com/KB/graphics/rotateimage.aspx
        //Author : James T. Johnson
        private static Bitmap RotateImage(Image image, float angle)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            const double pi2 = Math.PI / 2.0;

            // Why can't C# allow these to be const, or at least readonly
            // *sigh*  I'm starting to talk like Christian Graus :omg:
            double oldWidth = (double)image.Width;
            double oldHeight = (double)image.Height;

            // Convert degrees to radians
            double theta = ((double)angle) * Math.PI / 180.0;
            double locked_theta = theta;

            // Ensure theta is now [0, 2pi)
            while (locked_theta < 0.0)
                locked_theta += 2 * Math.PI;

            double newWidth, newHeight;
            int nWidth, nHeight; // The newWidth/newHeight expressed as ints

            #region Explaination of the calculations
            /*
             * The trig involved in calculating the new width and height
             * is fairly simple; the hard part was remembering that when 
             * PI/2 <= theta <= PI and 3PI/2 <= theta < 2PI the width and 
             * height are switched.
             * 
             * When you rotate a rectangle, r, the bounding box surrounding r
             * contains for right-triangles of empty space.  Each of the 
             * triangles hypotenuse's are a known length, either the width or
             * the height of r.  Because we know the length of the hypotenuse
             * and we have a known angle of rotation, we can use the trig
             * function identities to find the length of the other two sides.
             * 
             * sine = opposite/hypotenuse
             * cosine = adjacent/hypotenuse
             * 
             * solving for the unknown we get
             * 
             * opposite = sine * hypotenuse
             * adjacent = cosine * hypotenuse
             * 
             * Another interesting point about these triangles is that there
             * are only two different triangles. The proof for which is easy
             * to see, but its been too long since I've written a proof that
             * I can't explain it well enough to want to publish it.  
             * 
             * Just trust me when I say the triangles formed by the lengths 
             * width are always the same (for a given theta) and the same 
             * goes for the height of r.
             * 
             * Rather than associate the opposite/adjacent sides with the
             * width and height of the original bitmap, I'll associate them
             * based on their position.
             * 
             * adjacent/oppositeTop will refer to the triangles making up the 
             * upper right and lower left corners
             * 
             * adjacent/oppositeBottom will refer to the triangles making up 
             * the upper left and lower right corners
             * 
             * The names are based on the right side corners, because thats 
             * where I did my work on paper (the right side).
             * 
             * Now if you draw this out, you will see that the width of the 
             * bounding box is calculated by adding together adjacentTop and 
             * oppositeBottom while the height is calculate by adding 
             * together adjacentBottom and oppositeTop.
             */
            #endregion

            double adjacentTop, oppositeTop;
            double adjacentBottom, oppositeBottom;

            // We need to calculate the sides of the triangles based
            // on how much rotation is being done to the bitmap.
            //   Refer to the first paragraph in the explaination above for 
            //   reasons why.
            if ((locked_theta >= 0.0 && locked_theta < pi2) ||
                (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2)))
            {
                adjacentTop = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
                oppositeTop = Math.Abs(Math.Sin(locked_theta)) * oldWidth;

                adjacentBottom = Math.Abs(Math.Cos(locked_theta)) * oldHeight;
                oppositeBottom = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
            }
            else
            {
                adjacentTop = Math.Abs(Math.Sin(locked_theta)) * oldHeight;
                oppositeTop = Math.Abs(Math.Cos(locked_theta)) * oldHeight;

                adjacentBottom = Math.Abs(Math.Sin(locked_theta)) * oldWidth;
                oppositeBottom = Math.Abs(Math.Cos(locked_theta)) * oldWidth;
            }

            newWidth = adjacentTop + oppositeBottom;
            newHeight = adjacentBottom + oppositeTop;

            nWidth = (int)Math.Ceiling(newWidth);
            nHeight = (int)Math.Ceiling(newHeight);

            Bitmap rotatedBmp = new Bitmap(nWidth, nHeight);

            using (Graphics g = Graphics.FromImage(rotatedBmp))
            {
                // This array will be used to pass in the three points that 
                // make up the rotated image
                Point[] points;


                if (locked_theta >= 0.0 && locked_theta < pi2)
                {
                    points = new Point[] {
                                             new Point( (int) oppositeBottom, 0 ),
                                             new Point( nWidth, (int) oppositeTop ),
                                             new Point( 0, (int) adjacentBottom )
                                         };

                }
                else if (locked_theta >= pi2 && locked_theta < Math.PI)
                {
                    points = new Point[] {
                                             new Point( nWidth, (int) oppositeTop ),
                                             new Point( (int) adjacentTop, nHeight ),
                                             new Point( (int) oppositeBottom, 0 )
                                         };
                }
                else if (locked_theta >= Math.PI && locked_theta < (Math.PI + pi2))
                {
                    points = new Point[] {
                                             new Point( (int) adjacentTop, nHeight ),
                                             new Point( 0, (int) adjacentBottom ),
                                             new Point( nWidth, (int) oppositeTop )
                                         };
                }
                else
                {
                    points = new Point[] {
                                             new Point( 0, (int) adjacentBottom ),
                                             new Point( (int) oppositeBottom, 0 ),
                                             new Point( (int) adjacentTop, nHeight )
                                         };
                }

                g.DrawImage(image, points);
            }

            return rotatedBmp;
        }
    }
}