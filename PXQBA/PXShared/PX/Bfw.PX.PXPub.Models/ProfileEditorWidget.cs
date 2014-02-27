using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.PX.Biz.ServiceContracts;
using System.ComponentModel.DataAnnotations;

namespace Bfw.PX.PXPub.Models
{
    public class ProfileEditorWidget
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "First Name is Required")]
        [RegularExpression("^[a-zA-Z0-9_\\s-:;'\"$]+$", ErrorMessage = "Invalid Character. \n This field can \n only accept alphanumeric and -:;'\"$ characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage="Last Name is Required")]
        [RegularExpression("^[a-zA-Z0-9_\\s-:;'\"$]+$", ErrorMessage = "Invalid Character. \n This field can \n only accept alphanumeric and -:;'\"$ characters")]
        public string LastName { get; set; }

        public string Name() { return FirstName + " " + LastName;  }

        public string UserName { get; set; }

        [Required(ErrorMessage="Email is Required")]
        [EmailFormatValid(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        public string Image { get; set; }

        public string ImageUrl { get; set; }

        public AccessLevel AccessLevel { get; set; }

        public System.Web.HttpPostedFileBase docFile { get; set; }
    }

    public class EmailFormatValidAttribute : RegularExpressionAttribute
    {
        public EmailFormatValidAttribute() :
            base(@"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$")
        { }
    }

}
