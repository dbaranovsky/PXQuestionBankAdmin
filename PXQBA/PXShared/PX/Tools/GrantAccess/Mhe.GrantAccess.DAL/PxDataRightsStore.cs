using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;

using Dapper;

using Mhe.GrantAccess.DAL.Contracts;
using Mhe.GrantAccess.DAL.Models;

namespace Mhe.GrantAccess.DAL
{
    public class PxDataRightsStore : IRightsStore
    {
        protected IDbConnection Connection { get; set; }

        public PxDataRightsStore(IDbConnection databaseConnection)
        {
            Connection = databaseConnection;
            Connection.Open();
        }

        /// <summary>
        /// Stores the specified rights.
        /// </summary>
        /// <param name="rights">Information about the rights that should be granted.</param>
        public void StoreRights(PxWebUserRights rights)
        {
            var rightsToGrant = FindRights(forUser: rights.UserId, andCourse: rights.CourseId, andComponent: rights.Component);

            if (rightsToGrant == null)
            {
                CreatePxWebUserRights(rights);
            }
            else
            {
                UpdatePxWebUserRights(rightsToGrant);
            }
        }

        /// <summary>
        /// Inserts a new PxWebuserRights record with the given information.
        /// </summary>
        /// <param name="rights">Information about the rights that should be granted.</param>
        private void CreatePxWebUserRights(PxWebUserRights rights)
        {
            if (string.IsNullOrEmpty(rights.CourseId))
                throw new ArgumentException("value cannot be null or empty", "rights.CourseId");

            if (string.IsNullOrEmpty(rights.UserId))
                throw new ArgumentException("value cannot be null or empty", "rights.UserId");

            if (rights.Component == null)
                throw new ArgumentNullException("rights.Component");

            if (string.IsNullOrEmpty(rights.Component.RightType))
                throw new ArgumentException("value cannot be null or empty", "rights.Component.RightType");

            Connection.Execute(@"
                insert PxWebUserRights (CourseId, UserId, Rights, PxWebRightId)
                values (@CourseId, @UserId, @Rights, (select PxWebRightId from PxWebRights where PxWebRightType = @RightType))",
                new
                {
                    CourseId = rights.CourseId,
                    UserId = rights.UserId,
                    Rights = rights.Rights,
                    RightType = rights.Component.RightType
                });
        }
        
        /// <summary>
        /// Updates an existing PxWebUserRights record with the given information.
        /// </summary>
        /// <param name="rights">Information about the rights that should be granted.</param>
        private void UpdatePxWebUserRights(PxWebUserRights rights)
        {
            if (!rights.Id.HasValue)
                throw new ArgumentException("value cannot be null", "rights.Id");

            if (string.IsNullOrEmpty(rights.CourseId))
                throw new ArgumentException("value cannot be null or empty", "rights.CourseId");

            if (string.IsNullOrEmpty(rights.UserId))
                throw new ArgumentException("value cannot be null or empty", "rights.UserId");

            if (rights.Component == null)
                throw new ArgumentNullException("rights.Component");

            if (string.IsNullOrEmpty(rights.Component.RightType))
                throw new ArgumentException("value cannot be null or empty", "rights.Component.RightType");

            Connection.Execute(@"
                update PxWebUserRights
                set CourseId = @CourseId,
                    UserId = @UserId,
                    PxWebRightId = (select PxWebRightId from PxWebRights where PxWebRightType = @RightType),
                    Rights = @Rights
                where Id = @Id",
                new
                {
                    CourseId = rights.CourseId,
                    UserId = rights.UserId,
                    RightType = rights.Component.RightType,
                    Rights = rights.Rights,
                    Id = rights.Id.Value
                });
        }

        /// <summary>
        /// Finds any rights for the given user, course, and component.
        /// </summary>
        /// <param name="forUser">Reference Id of the user to find rights for.</param>
        /// <param name="andCourse">Course Id to restrict results to.</param>
        /// <param name="andComponent">Component name to restrict results to.</param>
        /// <returns></returns>
        public PxWebUserRights FindRights(string forUser, string andCourse, PxWebRights andComponent)
        {
            if (string.IsNullOrEmpty(forUser))
                throw new ArgumentException("value cannot be null or empty", "forUser");

            if (string.IsNullOrEmpty(andCourse))
                throw new ArgumentException("value cannot be null or empty", "andCourse");

            if (andComponent == null)
                throw new ArgumentNullException("andComponent");

            if (string.IsNullOrEmpty(andComponent.RightType))
                throw new ArgumentException("value cannot be null or empty", "andComponent.RightType");

            var records = Connection.Query(@"
                select wr.Id, wr.CourseId, wr.UserId, wr.PxWebRightId, wr.Rights,
                       wrt.PxWebRightId, wrt.PxWebRightType
                from PxWebUserRights wr
		                inner join
                     PxWebRights wrt on wr.PxWebRightId = wrt.PxWebRightId
                where wr.UserId = @UserId 
                        AND 
                      wr.CourseId = @CourseId
                        AND
                      wrt.PxWebRightType = @RightType",
                new
                {
                    UserId = forUser,
                    CourseId = andCourse,
                    RightType = andComponent.RightType
                });

            var rights = (from record in records
                          select new PxWebUserRights
                          {
                              Id = record.Id,
                              CourseId = record.CourseId,
                              Rights = record.Rights,
                              UserId = record.UserId,
                              Component = new PxWebRights
                              {
                                  Id = record.PxWebRightId,
                                  RightType = record.PxWebRightType
                              }
                          }).FirstOrDefault();

            return rights;
        }
    }
}
