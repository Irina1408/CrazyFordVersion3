using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazyFord.Data
{
    public interface IColumn
    {
        int Count { get; }                          //количество карт в столбце
        bool IsFull { get; }                        //заполнена ли колонка
        bool IsEmpty { get; }                       //пуста ли колонка

        void AddCard(Card card);
        void DeleteCard(Card card);
        void Clear();
    }
}
