﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Collections;

namespace Bfw.PXWebAPI.Models
{
    /// <summary>
    /// Represents a property value on a ContentItem.
    /// </summary>
    public class MetadataValue
    {
        /// <summary>
        /// The type of the property.
        /// </summary>
        public PropertyType Type { get; set; }

        /// <summary>
        /// The value of the property.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Populated if property is multivalued, null otherwise.
        /// </summary>
        public IEnumerable<object> Values { get; set; }

        /// <summary>
        /// Provides a way of quickly casting the value of the property to the given type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T As<T>()
        {
            return (T)Convert.ChangeType(Value, typeof(T));
        }

        /// <summary>
        /// Returns an IEnumerable containing a cast value for each element of Values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> EachAs<T>()
        {
            return Values.Map(v => (T)Convert.ChangeType(v, typeof(T)));
        }
    }
}
