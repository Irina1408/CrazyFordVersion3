using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace CrazyFord.Utils
{
    public static class Utils
    {
        public static HashSet<KeyValuePair<ItemPropertyInfo, object>> GetPropertiesInfo<T>(T t)
        {
            var result = new HashSet<KeyValuePair<ItemPropertyInfo, object>>();
            var properties = typeof (T).GetProperties();

            foreach (var p in properties)
            {
                var description = p.Name;
                foreach (var attribute in p.CustomAttributes.Where(attribute => attribute.AttributeType == typeof (DescriptionAttribute)))
                {
                    description = attribute.ToString();
                    break;
                }
                var pi = new ItemPropertyInfo(p.Name, p.PropertyType, description);
                result.Add(new KeyValuePair<ItemPropertyInfo, object>(pi, p.GetValue(t)));
            }

            return result;
        }

        public static void CopyProperties<T>(T from, ref T to)
        {
            //TODO: remove GetType(), change to T
            Type type = from.GetType();
            var properties = type.GetProperties();

            foreach (var p in properties.Where(p => p.CanRead && p.CanWrite))
            {
                p.SetValue(to, p.GetValue(@from));
            }
        }
    }
}
