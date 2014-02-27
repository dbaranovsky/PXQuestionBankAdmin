

if NOT EXISTS (SELECT * FROM sysindexes WHERE name = '_dta_stat_UserTypes_1')
begin
CREATE STATISTICS [_dta_stat_UserTypes_1] ON [dbo].[UserTypes]([CourseId], [Type])
end 
GO

if NOT EXISTS (SELECT * FROM sysindexes WHERE name = '_dta_stat_UserTypes_2')
begin
CREATE STATISTICS [_dta_stat_UserTypes_2] ON [dbo].[UserTypes]([Type], [UserId], [CourseId])
end 
GO

if NOT EXISTS (SELECT * FROM sysindexes WHERE name = '_dta_stat_UserTypes_3')
begin
CREATE STATISTICS [_dta_stat_UserTypes_3] ON [dbo].[UserTypes]([Id], [UserId], [CourseId])
end 
GO


if NOT EXISTS (SELECT * FROM sysindexes WHERE name = '_dta_index_UserTypes_User_Course_Type')
begin
CREATE NONCLUSTERED INDEX [_dta_index_UserTypes_User_Course_Type] ON [dbo].[UserTypes] 
(
	[UserId] ASC,
	[CourseId] ASC,
	[Type] ASC
)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
end 
GO



