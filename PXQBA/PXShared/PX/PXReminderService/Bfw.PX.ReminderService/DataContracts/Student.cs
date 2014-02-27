
namespace Bfw.PX.ReminderService.DataContracts
{
    /// <summary>
    /// reminder email student or instructor
    /// </summary>
    public class Sender
    {
        /// <summary>
        /// user id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// enrollment id
        /// </summary>
        public int EnrollmentId { get; set; }

        /// <summary>
        /// first name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// email id
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// studnet access/type
        /// </summary>
        public string Flags { get; set; }

        public override string ToString()
        {
            string str = string.Concat("User Id : ", Id.ToString(), 
                " , Enrollment Id : ", EnrollmentId.ToString(), 
                " , First Name : ", (string.IsNullOrEmpty(FirstName)? string.Empty: FirstName),
                " , Last Name : ", (string.IsNullOrEmpty(LastName) ? string.Empty : LastName),
                " , Email : ", (string.IsNullOrEmpty(Email) ? string.Empty : Email),
                " , Flags : ", (string.IsNullOrEmpty(Flags) ? string.Empty : Flags)
                ); 
            return str;
        }
    }
}
