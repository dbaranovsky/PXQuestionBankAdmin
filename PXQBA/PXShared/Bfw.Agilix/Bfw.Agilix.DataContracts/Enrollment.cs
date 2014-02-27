using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Dlap.Session;
using System.Runtime.Serialization;
using System.Xml.Linq;
using Bfw.Common;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Represents the relationship between a user and an entity (such as a class or section).
    /// </summary>
    public class Enrollment : IDlapEntityParser
    {
        #region Data Members

        /// <summary>
        /// The ID of the Enrollment object
        /// </summary>
        [DataMember]
        public string Id { get; set; }

        /// <summary>
        /// The ID of the User
        /// </summary>
        [DataMember]
        public AgilixUser User { get; set; }

        /// <summary>
        /// The Item which the user is linked to
        /// </summary>
        [DataMember]
        public Course Course { get; set; }

        /// <summary>
        /// Gets or sets course id that is required for passing request to DLAP and getting course id info by parsing
        /// </summary>
        [DataMember]
        public string CourseId { get; set; }

        /// <summary>
        /// The overall grade for the 
        /// </summary>
        [DataMember]
        public Grade OverallGrade { get; set; }

        /// <summary>
        /// The percentage of gradable items that have been graded.
        /// </summary>
        public double PercentGraded { get; set; }

        /// <summary>
        /// The set of grades for the enrollment
        /// </summary>
        [DataMember]
        public IEnumerable<Grade> ItemGrades { get; set; }

        /// <summary>
        /// The set of grades per gradebook category for the enrollment
        /// </summary>
        [DataMember]
        public IEnumerable<CategoryGrade> CategoryGrades { get; protected set; }

        /// <summary>
        /// Information about the domain the enrollment belongs to
        /// </summary>
        [DataMember]
        public Domain Domain { get; set; }

        /// <summary>
        /// Priviliges of the enrollment
        /// </summary>
        [DataMember]
        public string Priviliges { get; set; }

        /// <summary>
        /// Information about the rights flags
        /// </summary>
        [DataMember]
        public Dlap.DlapRights Flags { get; set; }

        /// <summary>
        /// status of the enrollment
        /// </summary>
        [DataMember]
        public string Status { get; set; }

        /// <summary>
        /// The date when enrollment was done
        /// </summary>
        [DataMember]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// The date until when enrollment is valid
        /// </summary>
        [DataMember]
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Reference Id
        /// </summary>
        [DataMember]
        public string Reference { get; set; }

        /// <summary>
        /// optional member - to specify how to interpret flags
        /// </summary>
        [DataMember]
        public string Schema { get; set; }

        private string _dataString;

        [System.Runtime.Serialization.OnSerializing]
        private void OnSerializing(System.Runtime.Serialization.StreamingContext context)
        {
            if (_data != null)
            {
                _dataString = _data.ToString();
            }
        }

        [System.Runtime.Serialization.OnDeserialized]
        private void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
        {
            if (!string.IsNullOrEmpty(_dataString))
            {
                _data = XElement.Parse(_dataString);
            }
        }

        [NonSerialized]
        private XElement _data;

        /// <summary>
        /// XML item data read from agilix item retrieval.
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

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <remarks></remarks>
        public Enrollment()
        {
            Data = new XElement("data");
        }

        #endregion

        #region IDlapEntityParser Members

        /// <summary>
        /// See (~/Docs/Schema/User) for format
        /// </summary>
        /// <returns></returns>
        public XElement ToEntity()
        {
            var element = new XElement("enrollment");

            if (!string.IsNullOrEmpty(Id))
                element.Add(new XAttribute("enrollmentid", Id));

            if (User != null)
            {
                if (!string.IsNullOrEmpty(User.Id))
                    element.Add(new XAttribute("userid", User.Id));

                if (!string.IsNullOrEmpty(User.FirstName))
                    element.Add(new XAttribute("firstname", User.FirstName));

                if (!string.IsNullOrEmpty(User.LastName))
                    element.Add(new XAttribute("lastname", User.LastName));

                if (!string.IsNullOrEmpty(User.Reference))
                    element.Add(new XAttribute("userreference", User.Reference));

                if (!string.IsNullOrEmpty(User.UserName))
                    element.Add(new XAttribute("username", User.UserName));


                if (!string.IsNullOrEmpty(User.Email))
                    element.Add(new XAttribute("email", User.Email));

                if (User.Credentials != null)
                {
                    if (!string.IsNullOrEmpty(User.Credentials.UserSpace))
                        element.Add(new XAttribute("userspace", User.Credentials.UserSpace));
                }
            }

            if (Course != null)
            {
                if (!string.IsNullOrEmpty(Course.Id))
                    element.Add(new XAttribute("entityid", Course.Id));
            }
            else if (!string.IsNullOrEmpty(this.CourseId))
            {
                element.Add(new XAttribute("entityid", this.CourseId));
            }

            if (Domain != null)
            {
                if (!string.IsNullOrEmpty(Domain.Id))
                    element.Add(new XAttribute("domainid", Domain.Id));

                if (!string.IsNullOrEmpty(Domain.Name))
                    element.Add(new XAttribute("domainname", Domain.Name));
            }

            string flags = ((long)Flags).ToString();
            if (Flags == Bfw.Agilix.Dlap.DlapRights.None)
                flags = string.Empty;

            if (!string.IsNullOrEmpty(flags))
                element.Add(new XAttribute("flags", flags));

            if (!string.IsNullOrEmpty(Status))
                element.Add(new XAttribute("status", Status));

            if (!string.IsNullOrEmpty(StartDate.ToString()))
                element.Add(new XAttribute("startdate", StartDate));

            if (!string.IsNullOrEmpty(EndDate.ToString()))
                element.Add(new XAttribute("enddate", EndDate));

            if (!string.IsNullOrEmpty(Reference))
                element.Add(new XAttribute("reference", Reference));

            if (Data != null)
            {
                element.Add(Data);
            }

            return element;
        }

        #endregion

        #region IDlapEntityParser Members

        /// <summary>
        /// Parses an XElement into internal object state. This allows for objects to be decomposed from
        /// parts of Dlap responses.
        /// </summary>
        /// <param name="element">element that contains the state to parse</param>
        /// <remarks></remarks>
        public void ParseEntity(System.Xml.Linq.XElement element)
        {
            // Set the Enrollment items
            var id = element.Attribute("enrollmentid") ?? element.Attribute("id");
            var userid = element.Attribute("userid");
            var firstname = element.Attribute("firstname");
            var lastname = element.Attribute("lastname");
            var username = element.Attribute("username");
            var email = element.Attribute("email");
            var userspace = element.Attribute("userspace");

            // accomodates difference in schemas
            // http://dev.dlap.bfwpub.com/js/docs/#!/Schema/Enrollment
            // http://dev.dlap.bfwpub.com/js/docs/#!/Schema/EnrollmentEntity
            var courseid = element.Attribute("courseid") ?? element.Attribute("entityid");
            
            var domainid = element.Attribute("domainid");
            var domainname = element.Attribute("domainname");
            var priviliges = element.Attribute("privileges");
            var flags = element.Attribute("flags");
            var enrollmentstatus = element.Attribute("enrollmentstatus") ?? element.Attribute("status");
            var enrollmentstartdate = element.Attribute("enrollmentstartdate");
            var enrollmentenddate = element.Attribute("enrollmentenddate");
            var reference = element.Attribute("reference");
            var lastlogin = element.Attribute("lastlogindate");
            var userElm = element.Element("user");
            var userreference = element.Attribute("reference");

            if (userElm != null)
            {
                firstname = userElm.Attribute("firstname");
                lastname = userElm.Attribute("lastname");
                email = userElm.Attribute("email");
                userreference = userElm.Attribute("reference");
            }

            Data = element.Element("data");

            if (Data == null)
                Data = new XElement("data");

            if (null != id)
            {
                Id = id.Value;
            }

            User = new AgilixUser();

            if (null != userid)
            {
                User.Id = userid.Value;
            }

            if (null != firstname)
            {
                User.FirstName = firstname.Value;
            }

            if (null != lastname)
            {
                User.LastName = lastname.Value;
            }

            if (null != email)
            {
                User.Email = email.Value;
            }

            if (null != userreference)
            {
                User.Reference = userreference.Value;
                User.UserName = username != null ? username.Value : string.Empty;
            }

            if (null != lastlogin)
            {
                DateTime dt;
                if (DateTime.TryParse(lastlogin.Value, out dt))
                {
                    User.LastLogin = dt;
                }
            }

            User.Credentials = new Credentials();

            if (null != userspace)
            {
                User.Credentials.UserSpace = userspace.Value;
            }

            User.Domain = new Domain();
            User.Domain.Id = "";
            User.Domain.Name = "";

            this.Course = new Course();
            var entity = element.Element("entity");
            if (entity != null)
            {
                this.Course.ParseEntity(entity);
            }
            else
            {
                if (null != courseid)
                {
                    this.Course.Id = courseid.Value;
                }
            }

            if (courseid != null)
            {
                this.CourseId = (!string.IsNullOrEmpty(courseid.Value)) ? courseid.Value : string.Empty;
            }

            var gradesElement = element.Element("grades");
            if (null != gradesElement)
            {
                // Set the overall grade
                OverallGrade = new Grade();
                OverallGrade.ParseEntity(gradesElement);

                // Set the percent graded
                var complete = gradesElement.Attribute("complete");
                if (null != complete)
                {
                    double d;
                    if (Double.TryParse(complete.Value, out d))
                    {
                        PercentGraded = d;
                    }
                }

                // Set the grades
                ItemGrades = new List<Grade>();
                var itemElement = gradesElement.Element("items");
                IEnumerable<XElement> itemElements = null;

                if (null != itemElement)
                {
                    itemElements = itemElement.Elements("item");
                }

                if (null != itemElements)
                {
                    foreach (var itemElm in itemElements)
                    {
                        Grade g = new Grade();
                        g.ParseEntity(itemElm);
                        ((List<Grade>)ItemGrades).Add(g);
                    }
                }

                // Set the category grades
                CategoryGrades = new List<CategoryGrade>();
                var categoryElement = gradesElement.Element(ElStrings.Categories);
                IEnumerable<XElement> categoryElements = null;

                if (null != categoryElement)
                {
                    categoryElements = categoryElement.Elements(ElStrings.category);
                }

                if (null != categoryElements)
                {
                    foreach (var categoryElm in categoryElements)
                    {
                        CategoryGrade g = new CategoryGrade();
                        g.ParseEntity(categoryElm);
                        ((List<CategoryGrade>)CategoryGrades).Add(g);
                    }
                }
            }

            Domain = new Domain();
            var domainElement = element.Element("domain");
            if (domainElement != null)
            {
                Domain.ParseEntity(domainElement);
            }
            else
            {
                if (null != domainid)
                    Domain.Id = domainid.Value;

                if (null != domainname)
                    Domain.Name = domainname.Value;
            }

            if (priviliges != null)
            {
                Priviliges = priviliges.Value;
            }

            if (null != flags)
            {
                long fv = 0;
                if (long.TryParse(flags.Value, out fv))
                    Flags = (Dlap.DlapRights)Enum.Parse(typeof(Dlap.DlapRights), fv.ToString());
            }

            if (null != enrollmentstatus)
            {
                Status = enrollmentstatus.Value;
            }

            if (enrollmentstartdate == null)
            {
                enrollmentstartdate = element.Attribute("startdate");
            }

            if (null != enrollmentstartdate)
            {
                DateTime dto;
                if (DateTime.TryParse(enrollmentstartdate.Value, out dto))
                    StartDate = dto;
            }

            if (enrollmentenddate == null)
            {
                enrollmentenddate = element.Attribute("enddate");
            }

            if (null != enrollmentenddate)
            {
                DateTime dto;
                if (DateTime.TryParse(enrollmentenddate.Value, out dto))
                    EndDate = dto;
            }

            if (null != reference)
            {
                Reference = reference.Value;
            }
        }

        #endregion
    }
}
