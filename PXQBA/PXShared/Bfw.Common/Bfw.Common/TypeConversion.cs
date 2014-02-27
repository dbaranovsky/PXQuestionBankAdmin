using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Bfw.Common
{
    /// <summary>
    /// Static methods to help with type conversion.
    /// </summary>
    public static class TypeConversion
    {
        /// <summary>
        /// Converts an object between two types.
        /// </summary>
        /// <typeparam name="SrcType">The source type.</typeparam>
        /// <typeparam name="DestType">The destination type.</typeparam>
        /// <param name="Source">The object to convert.</param>
        /// <returns></returns>
        public static DestType ConvertType<SrcType, DestType>(SrcType Source) where DestType : class, new()
        {
            return ConvertType<SrcType, DestType>(Source, null, null);
        }

        /// <summary>
        /// Converts an object between two types.
        /// </summary>
        /// <typeparam name="SrcType">The source type.</typeparam>
        /// <typeparam name="DestType">The destination type.</typeparam>
        /// <param name="Source">The object to convert.</param>
        /// <param name="SrcDestMapping">The mapping of source to destination.</param>
        /// <returns></returns>
        public static DestType ConvertType<SrcType, DestType>(SrcType Source, Dictionary<string, string> SrcDestMapping) where DestType : class, new()
        {
            return ConvertType<SrcType, DestType>(Source, SrcDestMapping, null);
        }

        /// <summary>
        /// Gets a named mapped through a dictionary.
        /// </summary>
        /// <param name="map">The mapping.</param>
        /// <param name="origName">Name original name.</param>
        /// <returns></returns>
        private static string GetMappedName(Dictionary<string, string> map, string origName)
        {
            if ((map != null) && (map.ContainsKey(origName)))
            {
                return map[origName];
            }
            return origName;
        }
        /// <summary>
        /// Get Value of a property by name
        /// </summary>
        /// <param name="src"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        /// <summary>
        /// Uses reflection to convert an object to a destination type, e.g. transfers all the properties and members they have in common.
        /// </summary>
        /// <typeparam name="SrcType">Source Type.</typeparam>
        /// <typeparam name="DestType">Destination Type.</typeparam>
        /// <param name="Source">Object to convert.</param>
        /// <param name="SrcDestMap">Mapping between source and destination property names. Null if no mapping exist.</param>
        /// <param name="Dest">Destination object or null if it should be created.</param>
        /// <returns>An object where as many properties and fields as possible have been transferred from Source.</returns>
        private static DestType ConvertType<SrcType, DestType>(SrcType Source, Dictionary<string, string> SrcDestMap, DestType Dest) where DestType : class
        {
            if (Source == null)
            {
                return null;
            }
            // Create object if it doesn't exist.
            DestType dstVar = Dest;
            if (dstVar == null)
            {
                dstVar = Activator.CreateInstance<DestType>();
            }

            // Loop through Source's public properties.
            Type srcTp = typeof(SrcType);
            PropertyInfo[] props = srcTp.GetProperties(
                                        System.Reflection.BindingFlags.Public |
                                        System.Reflection.BindingFlags.Instance |
                                        System.Reflection.BindingFlags.Static |
                                        System.Reflection.BindingFlags.GetProperty
                                   );

            foreach (PropertyInfo p in props)
            {
                // Check if destination type has a settable property of the same type.
                PropertyInfo pDest = typeof(DestType).GetProperty(GetMappedName(SrcDestMap, p.Name), p.PropertyType);
                if ((pDest != null) && (pDest.CanWrite))
                {
                    pDest.SetValue(dstVar, p.GetValue(Source, null), null);
                }
                else if(pDest == null)
                {
                    //if property not found, attempt to convert property via recursion
                    var pSrcType = p.PropertyType;
                    var pDestType = typeof (DestType).GetProperty(GetMappedName(SrcDestMap, p.Name)).PropertyType;
                    //recursive call to convert property type
                    MethodInfo ConvertTypeMethod = typeof(TypeConversion).GetMethods().First(m => m.ToString().Trim() == "DestType ConvertType[SrcType,DestType](SrcType)")
                        .MakeGenericMethod(new Type[] {pSrcType, pDestType});
                    object pValue = ConvertTypeMethod.Invoke(null, new object[]{p.GetValue(Source, null)});

                    pDest = typeof(DestType).GetProperty(GetMappedName(SrcDestMap, p.Name), pDestType);
                    if (pDest != null)
                    {
                        pDest.SetValue(dstVar, pValue, null);
                    }
                }
            }

            // Loop through Source' public fields.
            FieldInfo[] mems = srcTp.GetFields();
            foreach (FieldInfo fi in mems)
            {
                FieldInfo mDest = typeof(DestType).GetField(GetMappedName(SrcDestMap, fi.Name));
                if ((mDest != null) && (fi.FieldType == mDest.FieldType))
                {
                    mDest.SetValue(dstVar, fi.GetValue(Source));
                }
            }

            return dstVar;
        }
    }
}