using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// Entity that gets the user program details.
    /// </summary>
    [DataContract]
    public class UserProgram
    {
        /// <summary>
        /// program id
        /// </summary>
        [DataMember]
        public long ProgramId { get; set; }

        /// <summary>
        /// gets the user id for the program manager
        /// </summary>
        [DataMember]
        public long ManagerId { get; set; }

        /// <summary>
        /// gets the user id for the instructor
        /// </summary>
        [DataMember]
        public long? UserId { get; set; }

        /// <summary>
        /// gets the dashboard course id for the program maneger
        /// </summary>
        [DataMember]
        public long? ProgramDashboardId { get; set; }

        /// <summary>
        /// gets the dashboard course id for the instructor
        /// </summary>
        [DataMember]
        public long? UserDashboardId { get; set; }
    }

    /// <summary>
    /// Entity that gets the user program details from the DB table UserPrograms.
    /// </summary>
    [DataContract]
    public class UserProgramByDomain
    {
        /// <summary>
        /// program id
        /// </summary>
        [DataMember]
        public long ProgramId { get; set; }

        /// <summary>
        /// gets the reference user id for the instructor
        /// </summary>
        [DataMember]
        public long UserRefId { get; set; }

        /// <summary>
        /// gets the user id for the instructor
        /// </summary>
        [DataMember]
        public long UserId { get; set; }

        /// <summary>
        /// gets the domain id for the instructor
        /// </summary>
        [DataMember]
        public long UserDomainId { get; set; }

        /// <summary>
        /// gets the dashboard course id for the instructor
        /// </summary>
        [DataMember]
        public long? UserDashboardId { get; set; }
    }
}