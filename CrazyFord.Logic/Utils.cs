using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyFord.Logic
{
    static class Utils
    {
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
