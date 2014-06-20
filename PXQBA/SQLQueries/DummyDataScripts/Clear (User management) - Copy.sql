
USE PXData2
GO


drop table QBARoleCapability
drop table UserQBARole
drop table QBARole
drop table QBACourse

drop procedure GetQBARolesForCourse
drop procedure AddQBARole
drop procedure UpdateQBARole
drop procedure DeleteQBARole
drop procedure UpdateQBARoleCapabilities
drop type dbo.QBACapabilityList
drop procedure dbo.GetQBARoleCapabilities
drop procedure dbo.GetQBAUsers
drop procedure dbo.GetQBAUserRoles
drop procedure dbo.GetQBACourses
drop procedure dbo.GetQBAUserCourses
drop procedure dbo.GetUsersForQBA
drop procedure dbo.GetUserCourses