using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows.Documents;
using System.Xml.Linq;
using Bfw.Common;
using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.DataContracts
{
    [Serializable]
    /// <summary>
    /// Represents the Agilix User schema http://dev.dlap.bfwpub.com/Docs/Schema/User
    /// </summary>
    [DataContract]
    public class AgilixUser : IDlapEntityParser, IDlapEntityTransformer
    {
        private Dictionary<string,string> _gradebookViewFlagsCourse;
        private Dictionary<string, string> _gradebookSettingsFlagsCourse;

        #region Constructor

        public AgilixUser()
        {
            _gradebookViewFlagsCourse = new Dictionary<string,string>();
            _gradebookSettingsFlagsCourse = new Dictionary<string, string>();
        }
        #endregion
        
        #region Data Members

        /// <summary>
        /// Id of the user
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// Globally unique Id of the user
        /// </summary>
        [DataMember]
        public Guid? GlobalId { get; set; }

        /// <summary>
        /// User's first name
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// User's last name
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public string UserName { get; set; }

        /// <summary>
        /// User's e-mail address
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// User's external reference tag
        /// </summary>
        [DataMember]
        public string Reference { get; set; }        

        /// <summary>
        /// User's default gradebook ViewFlags
        /// XML element: pref_gradebook_viewflags
        /// </summary>
        [DataMember]
        public string GradebookViewFlags { get; set; }

        /// <summary>
        /// Users gradebook viewflags per course
        /// XML element: pref_gradebook_2121-viewflags
        /// </summary>
        public Dictionary<string, string> GradebookViewFlagsCourse
        {
            get { return _gradebookViewFlagsCourse; }
            set { _gradebookViewFlagsCourse = value; }
        }

        /// <summary>
        /// Users gradebook settings flags per course
        /// XML element: pref_gradebook_2121-gradeview
        /// </summary>
        public Dictionary<string, string> GradebookSettingsFlagsCourse
        {
            get { return _gradebookSettingsFlagsCourse; }
            set { _gradebookSettingsFlagsCourse = value; }
        }

        /// <summary>
        /// Information about the User's login credentials. This may only be partially populated
        /// depending on the exact circumstances (e.g. Password isn't typically returned in a DLAP
        /// response.
        /// </summary>
        [DataMember]
        public Credentials Credentials { get; set; }

        /// <summary>
        /// Date and time of the user's last login. Set to DateRule.MinDate if the user hasn't logged in
        /// </summary>
        [DataMember]
        public DateTime? LastLogin { get; set; }

        /// <summary>
        /// Information about the domain the user belongs to
        /// </summary>
        [DataMember]
        public Domain Domain { get; set; }

        private string _dataString;
        private string _propertiesString;

        [System.Runtime.Serialization.OnSerializing]
        private void OnSerializing(System.Runtime.Serialization.StreamingContext context)
        {
            if (_data != null)
            {
                _dataString = _data.ToString();
            }

            if (_properties != null)
            {
                _propertiesString = _properties.ToString();
            }
        }

        [System.Runtime.Serialization.OnDeserialized]
        private void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            if (!string.IsNullOrEmpty(_dataString))
            {
                _data = XElement.Parse(_dataString);
            }

            if (!string.IsNullOrEmpty(_propertiesString))
            {
                _properties = XElement.Parse(_propertiesString);
            }
        }

        [NonSerialized]
        private XElement _data;
        /// <summary>
        /// Contain the data element of user xml
        /// </summary>
        [DataMember]
        public XElement Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }

        [NonSerialized]
        private XElement _properties;
        /// <summary>
        /// Contains the properties in the XML format (XElement)
        /// </summary>
        public XElement Properties
        {
            get
            {
                return _properties;
            }
            set
            {
                _properties = value;
            }
        }

        #endregion
        
        #region IDlapEntityTransformer Members

        /// <summary>
        /// See http://dev.dlap.bfwpub.com/Docs/Schema/User for format
        /// </summary>
        /// <returns></returns>
        public XElement ToEntity()
        {
            var element = new XElement("user");

            if (!string.IsNullOrEmpty(Id))
            {
                element.Add(new XAttribute("userid", Id));
            }

            if (GlobalId.HasValue)
            {
                element.Add(new XAttribute("userguid", GlobalId.Value));
            }

            if (!string.IsNullOrEmpty(Email))
                element.Add(new XAttribute("email", Email));

            if (!string.IsNullOrEmpty(FirstName))
                element.Add(new XAttribute("firstname", FirstName));

            if (!string.IsNullOrEmpty(LastName))
                element.Add(new XAttribute("lastname", LastName));

            if (LastLogin.HasValue)
                element.Add(new XAttribute("lastlogindate", DateRule.Format(LastLogin.Value)));

            if (!string.IsNullOrEmpty(Reference))
                element.Add(new XAttribute("reference", Reference));

           if (Domain != null)
            {
                if (!string.IsNullOrEmpty(Domain.Id))
                    element.Add(new XAttribute("domainid", Domain.Id));

                if (!string.IsNullOrEmpty(Domain.Name))
                    element.Add(new XAttribute("domainname", Domain.Name));

                element.Add(new XAttribute("domain", Domain));
            }

            if (null != Credentials)
            {
                if (!string.IsNullOrEmpty(Credentials.Username))
                    element.Add(new XAttribute("username", Credentials.Username));

                if (!string.IsNullOrEmpty(Credentials.UserSpace))
                    element.Add(new XAttribute("userspace", Credentials.UserSpace));

                if (!string.IsNullOrEmpty(Credentials.Password))
                    element.Add(new XAttribute("password", Credentials.Password));
            }

       
            var data = new XElement("data");
            if (null != Properties)
            {
                data.Add(Properties);
            }
            if (!string.IsNullOrEmpty(GradebookViewFlags))
            {
                data.Add(new XElement("pref_gradebook_viewflags", GradebookViewFlags));
            }

            if (GradebookViewFlagsCourse.Count > 0)
            {
                foreach (var flag in GradebookViewFlagsCourse)
                {
                    var courseId = flag.Key;
                    var val = flag.Value;
                    data.Add(new XElement("pref_gradebook_" + courseId + "-viewflags", val));
                }
            }
            if (GradebookSettingsFlagsCourse.Count > 0)
            {
                foreach (var flag in GradebookSettingsFlagsCourse)
                {
                    var courseId = flag.Key;
                    var val = flag.Value;
                    data.Add(new XElement("pref_gradebook_" + courseId + "-gradeview", val));
                }
            }

            element.Add(data);

            return element;
        }

        #endregion

        #region IDlapEntityParser Members

        /// <summary>
        /// See http://dev.dlap.bfwpub.com/Docs/Schema/User for format
        /// </summary>
        /// <returns></returns>
        public void ParseEntity(XElement element)
        {
            if (null != element)
            {
                if ("user" != element.Name)
                    throw new DlapEntityFormatException("AgilixUser entity expects root element \"user\"");

                var id = element.Attribute("id");
                var gid = element.Attribute("guid");
                var email = element.Attribute("email");
                var first = element.Attribute("firstname");
                var last = element.Attribute("lastname");
                var logdate = element.Attribute("lastlogindate");
                var username = element.Attribute("username");

                XAttribute reference = element.Attribute("reference");
                XAttribute uname = element.Attribute("username");

                var uspace = element.Attribute("userspace");
                var did = element.Attribute("domainid");
                var dname = element.Attribute("domainname");
                var data = element.Element("data");
                var defaultGradebookViewflags = data != null ? data.Element("pref_gradebook_viewflags") : null;
                

                if (null == id)
                    id = element.Attribute("userid");

                if (null != id)
                {
                    Id = id.Value;
                }

                /*if (null == gid)
                    gid = element.Attribute("userguid");*/

                if (null != gid)
                {
                    try
                    {
                        GlobalId = new Guid(gid.Value);
                    }
                    catch
                    {
                        GlobalId = null;
                    }
                }

                if (null != email)
                {
                    Email = email.Value;
                }

                if (null != username)
                {
                    UserName = username.Value;
                }

                if (null != first)
                {
                    FirstName = first.Value;
                }

                if (null != last)
                {
                    LastName = last.Value;
                }

                if (null != logdate)
                {
                    DateTime t = DateRule.MinDate;
                    if (DateTime.TryParse(logdate.Value, out t))
                        LastLogin = t;
                }

                if (null != reference)
                    Reference = reference.Value;

                Credentials = new Credentials();

                if (null != uname)
                    Credentials.Username = uname.Value;

                if (null != uspace)
                    Credentials.UserSpace = uspace.Value;

                Domain = new Domain();

                if (null != did)
                {
                    Domain.Id = did.Value;
                }

                if (null != dname)
                    Domain.Name = dname.Value;

                if (null != data)
                    Data = data;
                else
                    Data = new XElement("data");

                if (null != defaultGradebookViewflags)
                {
                    GradebookViewFlags = defaultGradebookViewflags.Value;
                }
                
                foreach (XElement elem in Data.Elements())
                {
                    if (elem.Name.ToString().StartsWith("pref_gradebook_")
                        && elem.Name.ToString().EndsWith("-viewflags"))
                    {
                        var courseId =
                            elem.Name.ToString()
                                   .Replace("pref_gradebook_", string.Empty)
                                   .Replace("-viewflags", string.Empty);
                        var val = elem.Value;
                        GradebookViewFlagsCourse.Add(courseId,val);
                    }
                }

                foreach (XElement elem in Data.Elements())
                {
                    if (elem.Name.ToString().StartsWith("pref_gradebook_")
                        && elem.Name.ToString().EndsWith("-gradeview"))
                    {
                        var courseId =
                            elem.Name.ToString()
                                   .Replace("pref_gradebook_", string.Empty)
                                   .Replace("-gradeview", string.Empty);
                        var val = elem.Value;
                        GradebookSettingsFlagsCourse.Add(courseId, val);
                    }
                }
            }
        }

        #endregion        
    }
}
