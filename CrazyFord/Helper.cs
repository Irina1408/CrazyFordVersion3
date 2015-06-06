using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CrazyFord.Data;
using Brush = System.Windows.Media.Brush;

namespace CrazyFord
{
    class Helper
    {
        /// <summary>
        /// Load image from resource
        /// </summary>
        /// <param name="assemblyName">Assembly name has this image</param>
        /// <param name="resourceName">Path image in application</param>
        /// <returns>Image source</returns>
        public static ImageSource GetImageSourceFromResource(string resourceName, string assemblyName = null)
        {
            if (string.IsNullOrEmpty(assemblyName))
            {
                assemblyName = Assembly.GetCallingAssembly().GetName().Name;
            }

            Uri oUri = new Uri("pack://application:,,,/" + assemblyName + ";component/" + resourceName, UriKind.RelativeOrAbsolute);
            ImageSource tempIS = BitmapFrame.Create(oUri);
            oUri = null;
            return tempIS;
        }

        /// <summary>
        /// Load bitmap image from resource
        /// </summary>
        /// <param name="pathInApplication">Path image in application</param>
        /// <param name="assembly">Assemply has this image</param>
        /// <returns>Bitmap image/Image source</returns>
        //public static BitmapImage LoadBitmapFromResource(string pathInApplication, Assembly assembly = null)
        //{
        //    if (assembly == null)
        //    {
        //        assembly = Assembly.GetCallingAssembly();
        //    }

        //    if (pathInApplication[0] == '/')
        //    {
        //        pathInApplication = pathInApplication.Substring(1);
        //    }

        //    return new BitmapImage(new Uri(@"pack://application:,,,/" + assembly.GetName().Name + ";component/" + pathInApplication, UriKind.Absolute));
        //}

        /// <summary>
        /// Set image (not checked)
        /// </summary>
        /// <param name="image"></param>
        /// <param name="imageSource"></param>
        public static void SetImage(System.Windows.Controls.Image image, string imageSource)
        {
            image.SetValue(System.Windows.Controls.Image.SourceProperty, new ImageSourceConverter().ConvertFromString(imageSource));
        }

        /// <summary>
        /// Set all share settings for every image card
        /// </summary>
        /// <param name="cardImage">Image need this settings</param>
        public static void TuneCardImage(System.Windows.Controls.Image cardImage)
        {
            cardImage.Stretch = Stretch.Fill;
            cardImage.VerticalAlignment = VerticalAlignment.Top;
        }

        /// <summary>
        /// Return count cards that is in the game columns in the start 
        /// (each column has one more cards on the previous column has)
        /// </summary>
        /// <param name="countGameColumns">Count game columns</param>
        /// <returns>Count cards</returns>
        public static int GetStartCountCardInGame(int countGameColumns = 10)
        {
            if (countGameColumns > 1)
            {
                return countGameColumns + GetStartCountCardInGame(countGameColumns - 1);
            }
            else
            {
                return countGameColumns;
            }
        }

        public static ImageSource GetImageSourceCardFaceByCardData(DataCard dataCard, string assemblyName = null)
        {
            int numCardName = (int) dataCard.CardName + 1;
            string lear = dataCard.CardLear.ToString();
            return GetImageSourceFromResource("Resources/CardFace/" + dataCard.CardLear + "/" + numCardName + ".png", assemblyName);
        }

        public static int[] Shuffle(int[] mas)
        {
            bool[] assigned = new bool[mas.Length];
            Random sourceGen = new Random();

            for (int i = 0; i < mas.Length; i++)
            {
                int sourceCard = 0;
                bool foundCard = false;
                while (foundCard == false)
                {
                    sourceCard = sourceGen.Next(mas.Length);
                    if (assigned[sourceCard] == false)
                    {
                        foundCard = true;
                    }
                }
                assigned[sourceCard] = true;
                mas[i] = sourceCard;
            }

            return mas;
        }

    }
}
