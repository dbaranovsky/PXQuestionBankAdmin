using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using Bfw.Common.Collections;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.DataContracts
{
    [Serializable]
    /// <summary>
    /// Stores any necessary login credentials for a DLAP user
    /// </summary>
    [DataContract]
    public class Credentials
    {
        #region Data Members
        
        /// <summary>
        /// Agilix user's DLAP username
        /// </summary>
        [DataMember]
        public string Username { get; set; }

        /// <summary>
        /// Agilix user's DLAP password
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// Agilix user's DLAP password question
        /// </summary>
        [DataMember]
        public string PasswordQuestion { get; set; }

        /// <summary>
        /// Agilix user's DLAP password answer
        /// </summary>
        [DataMember]
        public string PasswordAnswer { get; set; }

        /// <summary>
        /// Agilix user's DLAP security token
        /// </summary>
        [DataMember]
        public string Token { get; set; }

        /// <summary>
        /// User's login prefix
        /// </summary>
        [DataMember]
        public string UserSpace { get; set; }

        #endregion
    }
}
