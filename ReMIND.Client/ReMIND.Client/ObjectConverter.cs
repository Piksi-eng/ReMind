using System;
using System.Collections.Generic;
using System.Linq;

namespace ReMIND.Client
{
    public static class ObjectConverter
    {
        public static List<T> ToTypedList<T>(object[] objectArray)
        {
            Type finalType = typeof(T);
            T[] typedArray = (T[])Array.CreateInstance(finalType, objectArray.Length);
            Array.Copy(objectArray, typedArray, objectArray.Length);
            return typedArray.ToList();
        }
        public static T ToTypedInstance<T>(object objectInstance)
        {
            Type finalType = typeof(T);
            object[] objectArray = new object[1];
            objectArray[0] = objectInstance;
            T[] typedArray = (T[])Array.CreateInstance(finalType, objectArray.Length);
            Array.Copy(objectArray, typedArray, objectArray.Length);
            T typedInstance = typedArray[0];
            return typedInstance;//test if this really works
        }
    }
}
