using System.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace CustomSound
{
    internal static class Reflexion
    {
        public static object CallMethod(this object element, string methodName, params object[] args)
        {
            var mi = element.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (mi != null)
            {
                return mi.Invoke(element, args);
            }
            return null;
        }

        public static void SetField<T>(this object element, string fieldName, T value)
        {
            var prop = element.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                prop.SetValue(element, value);
            }
        }

        public static T GetFieldValueOrPerties<T>(this object element, string fieldName)
        {
            var prop = element.GetType().GetProperty(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (prop != null)
            {
                return (T)prop.GetValue(element);
            }
            FieldInfo field = element.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (field != null)
            {
                return (T)field.GetValue(element);
            }
            return default(T);
        }
    }
}
