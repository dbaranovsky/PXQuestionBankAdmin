using System.Collections.Generic;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;
using BizSC = Bfw.PX.Biz.ServiceContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers {
    public static class UserProfileMapper {

        /// <summary>
        /// Convert to an UserProfile DataContract to UserProfile Model.
        /// </summary>
        /// <param name="biz">The biz.</param>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        public static ProfileSummaryWidget ToUserProfile(this BizDC.UserProfileResponse biz)
        {

            ProfileSummaryWidget userProfile = new ProfileSummaryWidget();
            List<UserInfo> UsersSummaryList = new List<UserInfo>();
            foreach (var ui in biz.UserProfile)
            {
                UserInfo user = new UserInfo();
                user.Id = ui.UserId;
                user.ReferenceId = ui.ReferenceId;
                user.FirstName = ui.FirstName;
                user.LastName = ui.LastName;
                user.LastLogin = ui.LastLogin;
                user.AvatarUrl = ui.AvatarUrl;
                user.Email = ui.Email;
                UsersSummaryList.Add(user);
            }
            userProfile.UsersSummaryList = UsersSummaryList;
            return userProfile;
        }

        /// <summary>
        /// Converts to a UserProfile Model to UserProfile DataContract.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        public static BizDC.UserProfileResponse ToUserProfile(this ProfileSummaryWidget model)
        {
            UserProfileResponse biz = new UserProfileResponse()
                                          {
                                              UserProfile = new List<UserProfile>()
                                          };
            
            foreach (var ui in model.UsersSummaryList)
            {
                UserProfile user = new UserProfile();
                user.UserId = ui.Id;
                user.ReferenceId = ui.ReferenceId;
                user.FirstName = ui.FirstName;
                user.LastName = ui.LastName;
                user.LastLogin = ui.LastLogin;
                user.AvatarUrl = ui.AvatarUrl;
                user.Email = ui.Email;
                biz.UserProfile.Add(user);
            }

            return biz;
        }
    }
}
