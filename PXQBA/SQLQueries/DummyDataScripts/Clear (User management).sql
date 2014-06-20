USE PXData2
GO


drop table QBARoleCapability
drop table UserQBARole
drop table QBARole
drop table QBACourse

drop procedure GetQBARolesForCourse
drop procedure GetCourses
drop procedure AddQBARole
drop procedure UpdateQBARole
drop procedure DeleteQBARole
drop procedure UpdateQBARoleCapabilities
drop procedure dbo.GetQBARoleCapabilities
drop procedure dbo.GetQBAUsers
drop procedure dbo.GetQBAUserRoles
drop procedure dbo.GetQBACourses
drop procedure dbo.GetQBAUserCourses
drop procedure dbo.GetUsersForQBA
drop procedure dbo.GetUserCourses
drop procedure dbo.GetUserRoles
drop procedure dbo.GetQBAUserCoursesWithRoles
drop procedure dbo.UpdateQBAUserRoles
drop procedure dbo.GetQBAUserCapabilities
IF TYPE_ID(N'QBACapabilityList') IS NOT NULL drop type dbo.QBACapabilityList
IF TYPE_ID(N'QBAIdList') IS NOT NULL drop type dbo.QBAIdList