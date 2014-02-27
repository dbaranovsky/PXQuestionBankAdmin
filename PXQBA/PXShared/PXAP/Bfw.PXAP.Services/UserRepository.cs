using System.Linq;
using System.Web.Security;
using Bfw.PXAP.Models.Domain;
using Bfw.PXAP.ServiceContracts;
using System.Web.Profile;
using System;

namespace Bfw.PXAP.Services
{
    public class UserRepository : IUserRepository
    {
        public IQueryable<User> GetUsers()
        {
            var membershipUsers = Membership.GetAllUsers();

            var users = from MembershipUser membershipUser in membershipUsers
                        select new User {
                            UserName = membershipUser.UserName,
                            Email = membershipUser.Email,
                            Name = (string)ProfileBase.Create(membershipUser.UserName).GetPropertyValue("Name")
                        };

            return users.AsQueryable();
        }

        public User GetUser(string username)
        {
            var membershipUser = Membership.GetUser(username);

            var user = new User {
                UserName = membershipUser.UserName,
                Email = membershipUser.Email,
                Name = (string)ProfileBase.Create(membershipUser.UserName).GetPropertyValue("Name")
            };

            return user;
        }

        public void SaveUser(User user, bool createUser = false)
        {
            var membershipUser = Membership.GetUser(user.UserName);

            if (membershipUser == null)
            {
                membershipUser = Membership.CreateUser(user.UserName, user.NewPassword, user.Email);
            }
            else if (createUser)
            {
                throw new Exception("User with that username already exists.");
            }
            else
            {
                if (user.CurrentPassword != null)
                {
                    if (!membershipUser.ChangePassword(user.CurrentPassword, user.NewPassword))
                    {
                        throw new Exception("Incorrect current password.");
                    }
                }

                membershipUser.Email = user.Email;
                Membership.UpdateUser(membershipUser);
            }

            var profile = ProfileBase.Create(membershipUser.UserName);
            profile.SetPropertyValue("Name", user.Name);
            profile.Save();
        }


        public void DeleteUser(string userName)
        {
            Membership.DeleteUser(userName, true);
        }
    }
}