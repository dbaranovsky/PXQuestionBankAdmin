
if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_UserNoteSettings_1')
begin
CREATE STATISTICS [_dta_stat_UserNoteSettings_1] ON [dbo].[UserNoteSettings]([CourseId], [UserId], [Id])
end
GO

if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_UserNoteSettings_2')
begin
CREATE STATISTICS [_dta_stat_UserNoteSettings_2] ON [dbo].[UserNoteSettings]([Id], [CourseId])
end
GO

if NOT EXISTS (SELECT * FROM sysindexes WHERE name = '_dta_index_UserNoteSettings_Course_User')
begin
CREATE NONCLUSTERED INDEX [_dta_index_UserNoteSettings_Course_User] ON [dbo].[UserNoteSettings] 
(
	[CourseId] ASC,
	[UserId] ASC
)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
end
GO
