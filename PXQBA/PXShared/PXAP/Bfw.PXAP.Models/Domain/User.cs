namespace Bfw.PXAP.Models.Domain
{
    public class User
    {
        public string UserName { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string CurrentPassword { get; set; }

        public string NewPassword { get; set; }
    }
}
