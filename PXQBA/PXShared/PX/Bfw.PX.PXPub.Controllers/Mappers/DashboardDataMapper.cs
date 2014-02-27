using Bfw.PX.PXPub.Models;
using BizDC = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.PXPub.Controllers.Mappers
{
    public static class DashboardDataMapper
    {
        /// <summary>
        /// Converts the DashboardItem (Data Contract) to DashboardItem (Model)
        /// </summary>
        /// <param name="Biz"></param>
        /// <returns></returns>
        public static DashboardItem ToDashboardItem(this BizDC.DashboardItem Biz)
        {
            var Model = new DashboardItem();

            if (null != Biz)
            {
                Model.CourseId = Biz.CourseId;
                Model.CourseTitle = Biz.CourseTitle;
                Model.OwnerId = Biz.OwnerId;
                Model.OwnerFirstName = Biz.OwnerFirstName;
                Model.OwnerLastName = Biz.OwnerLastName;
                Model.OwnerName = Biz.OwnerName;
                Model.OwnerEmail = Biz.OwnerEmail;
                Model.Notes = Biz.Notes;
                Model.Users = Biz.Users;
                Model.Status = Biz.Status;
                Model.Count = Biz.Count;
                Model.DomainId = Biz.DomainId;
                Model.DomainName = Biz.DomainName;
                Model.Level = Biz.Level;
                if (Biz.Course != null)
                {
                    Model.Course = Biz.Course.ToCourse();
                    Model.Course.SchoolName = Biz.DomainName;
                }
            }

            return Model;
        }
    }
}
