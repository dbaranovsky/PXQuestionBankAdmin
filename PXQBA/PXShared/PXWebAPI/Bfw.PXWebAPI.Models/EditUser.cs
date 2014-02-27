using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    public class EditUser
    {
        /// <summary>
        /// User's first name.
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// User's last name.
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        /// User's e-mail address.
        /// </summary>
        [StringLength(150, ErrorMessage = "{0} cannot be longer than {1} characters.")]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
    }
}
