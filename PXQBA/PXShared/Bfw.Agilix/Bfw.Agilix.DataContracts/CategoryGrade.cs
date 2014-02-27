using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Dlap.Session;
using System.Xml.Linq;
using System.Runtime.Serialization;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents grade per gradebook category
    /// </summary>
    public class CategoryGrade : IDlapEntityParser
    {
        #region Properties

        /// <summary>
        /// The category id.
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// The category name.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// score the user received, with curving applied.
        /// </summary>
        [DataMember]
        public double Achieved { get; protected set; }

        /// <summary>
        /// Maximum possible score, with curing applied.
        /// </summary>
        [DataMember]
        public double Possible { get; protected set; }

        /// <summary>
        /// The score the user achieved in letter form.
        /// </summary>
        [DataMember]
        public string Letter { get; protected set; }

        #endregion

        #region IDlapEntityParser Members

        /// <summary>
        /// Parses an XElement into internal object state. This allows for objects to be decomposed from
        /// parts of Dlap responses.
        /// </summary>
        /// <param name="element">element that contains the state to parse</param>
        /// <remarks></remarks>
        public void ParseEntity(XElement element)
        {
            if (null != element)
            {
                var id = element.Attribute(ElStrings.Id);
                var name = element.Attribute(ElStrings.Name);

                if (null != id)
                {
                    Id = id.Value;
                }

                if (null != name)
                {
                    Name = name.Value;
                }

                // Populate the rest of the Grade object
                var achieved = element.Attribute(ElStrings.Achieved);
                var possible = element.Attribute(ElStrings.possible);
                var letter = element.Attribute(ElStrings.Letter);
                
                if (achieved != null)
                {
                    Achieved = Math.Round(Double.Parse(achieved.Value), 2);
                }

                if (possible != null)
                {
                    Possible = Double.Parse(possible.Value);
                }

                if (letter != null)
                {
                    Letter = letter.Value;
                }
            }
        }

        #endregion
    }
}
