using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Cache;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Brush = System.Windows.Media.Brush;

namespace CrazyFord
{
    using CrazyFord.Data;

    static class Helper
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
                assemblyName = Assembly.GetCallingAssembly().GetName().Name;

            Uri oUri = new Uri("pack://application:,,,/" + assemblyName + ";component/" + resourceName, UriKind.RelativeOrAbsolute);
            ImageSource tempIS = BitmapFrame.Create(oUri);
            oUri = null;
            return tempIS;
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
            int numCardName = (int)dataCard.CardName + 1;
            string lear = dataCard.CardLear.ToString();
            return GetImageSourceFromResource("Resources/CardFace/" + dataCard.CardLear + "/" + numCardName + ".png", assemblyName);
        }
    }

    static class IndexHelper
    {
        /// <summary>
        /// Get result column index in grid
        /// </summary>
        public static int GetResGridColIndex(this Grid grid, int iCol)
        {
            return (iCol + grid.ColumnDefinitions.Count / 2 - StaticGameData.CountResultColumns) * 2 + 1;
        }

        /// <summary>
        /// Get game column index in grid
        /// </summary>
        public static int GetGameGridColIndex(this Grid grid, int iCol)
        {
            return iCol * 2 + 1;
        }

        /// <summary>
        /// Get result column index by the grid column index
        /// </summary>
        public static int GetResColIndex(this Grid grid, int iColGrid)
        {
            return (iColGrid - 1) / 2 - grid.ColumnDefinitions.Count / 2 + StaticGameData.CountResultColumns;
        }

        /// <summary>
        /// Get game column index
        /// </summary>
        public static int GetGameColIndex(this Grid grid, int iColGrid)
        {
            return (iColGrid - 1) / 2;
        }
    }
}
