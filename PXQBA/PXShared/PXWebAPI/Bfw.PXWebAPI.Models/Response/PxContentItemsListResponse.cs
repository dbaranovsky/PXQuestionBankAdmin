using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Bfw.PXWebAPI.Models.Response
{
	/// <summary>
	/// PxContentItemsListResponse
	/// </summary>
	/// <summary>
	/// Px Assignments Response
	/// </summary>
	/// <summary>
	/// Response
	/// </summary>
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.ApiCourseResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.ApiSearchResultDocListResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.BoolResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.ContentItemListResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.CourseListResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.CourseResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.DictionaryResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.DomainListResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.DomainResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.EnrolleeListResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.EnrollmentListResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.EnrollmentResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.FacetValueListResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.FacetedSearchResultsResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.GenericResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.GradeListResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.GradesItemListResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.OAuthResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.PxAssignmentsResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.PxContentItemsListResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.Response))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.SearchResultDocListResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.TableofContentsItemListResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.TupleDictionaryResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.UserEnrollmentDetailResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.UserEnrollmentListResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.UserEnrollmentResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.UserListResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.UserPackagesResponse))]
	[KnownType(typeof(Bfw.PXWebAPI.Models.Response.UserResponse))]
	[DataContract]
	public class PxContentItemsListResponse : Response
	{
		/// <summary>
		/// List of Bfw.PX.PXPub.Models.ContentItems
		/// </summary>
		[DataMember]
		public List<Bfw.PX.PXPub.Models.ContentItem> results { get; set; }
	}
}







