using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Contains useful extensions for the IDictionary<string, PropertyValue> collection type.
    /// </summary>
    public static class DataContractExtensions
    {
        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="props">The props dictionary collection.</param>
        /// <param name="name">The property name/key.</param>
        /// <param name="type">The property type to set.</param>
        /// <param name="val">The property type value.</param>
        public static void SetPropertyValue(this IDictionary<string, PropertyValue> props, string name, PropertyType type, object val)
        {
            PropertyValue prop = null;
            if (props.ContainsKey(name))
            {
                prop = props[name];
                prop.Value = val;
            }
            else
            {
                prop = new PropertyValue()
                {
                    Type = type,
                    Value = val
                };

                props[name] = prop;
            }
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="props">The props dictionary collection.</param>
        /// <param name="name">The property name/key.</param>
        /// <param name="type">The property type to set.</param>
        /// <param name="val">The collection of property type values.</param>
        public static void SetPropertyValue(this IDictionary<string, PropertyValue> props, string name, PropertyType type, IEnumerable<object> val)
        {
            PropertyValue prop = null;
            if (props.ContainsKey(name))
            {
                prop = props[name];
                prop.Values = val;
            }
            else
            {
                prop = new PropertyValue()
                {
                    Type = type,
                    Values = val
                };

                props[name] = prop;
            }
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="props">The props dictionary collection.</param>
        /// <param name="name">The property name/key.</param>
        /// <param name="ifNull">If null.</param>
        public static TValue GetPropertyValue<TValue>(this IDictionary<string, PropertyValue> props, string name, TValue ifNull)
        {
            TValue val = ifNull;

            if (props.ContainsKey(name))
            {
                return props[name].As<TValue>();
            }
            else
            {
                return val;
            }
        }

        /// <summary>
        /// Gets the property value.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="props">The props dictionary collection.</param>
        /// <param name="name">The property name/key.</param>
        /// <param name="ifNull">If null.</param>
        /// <returns></returns>
        public static IEnumerable<TValue> GetPropertyValue<TValue>(this IDictionary<string, PropertyValue> props, string name, IEnumerable<TValue> ifNull)
        {
            IEnumerable<TValue> val = ifNull;

            if (props.ContainsKey(name))
            {
                return props[name].EachAs<TValue>();
            }
            else
            {
                return val;
            }
        }

        public static void SetVisibilityForStudent(this IDictionary<string, PropertyValue> props, bool isHidden)
        {
            var visibility = props.GetPropertyValue<string>("bfw_visibility", "<bfw_visibility><roles><instructor /><student /></roles></bfw_visibility>");

            if (string.IsNullOrEmpty(visibility))
            {
                visibility = "<bfw_visibility><roles><instructor /><student /></roles></bfw_visibility>";
            }

            var node = XElement.Parse(visibility, LoadOptions.None);
            node.RemoveAll();

            var roles = new XElement("roles");
            roles.Add(new XElement("instructor"));

            if (!isHidden)
            {
                roles.Add(new XElement("student"));
            }

            node.Add(roles);
            props.SetPropertyValue("bfw_visibility", PropertyType.String, node.ToString());
        }

        public static Boolean IsHiddenFromStudent(this IDictionary<string, PropertyValue> props)
        {
            var visibility = props.GetPropertyValue<string>("bfw_visibility", "<bfw_visibility><roles><instructor /><student /></roles></bfw_visibility>");

            if (string.IsNullOrEmpty(visibility))
            {
                visibility = "<bfw_visibility><roles><instructor /><student /></roles></bfw_visibility>";
            }

            var node = XElement.Parse(visibility, LoadOptions.None);
            return (node.Descendants("student").Count() == 0);
        }
    }
}