
if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_Note_1')
begin
CREATE STATISTICS [_dta_stat_Note_1] ON [dbo].[Note]([Created], [NoteId], [UserId])
end
GO

if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_Note_2')
begin
CREATE STATISTICS [_dta_stat_Note_2] ON [dbo].[Note]([Status], [UserId], [NoteId])
end
GO

if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_Note_3')
begin
CREATE STATISTICS [_dta_stat_Note_3] ON [dbo].[Note]([UserId], [Public], [Status])
end
GO

if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_Note_4')
begin
CREATE STATISTICS [_dta_stat_Note_4] ON [dbo].[Note]([NoteId], [UserId], [Public], [Status])
end
GO

if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_Note_5')
begin
CREATE STATISTICS [_dta_stat_Note_5] ON [dbo].[Note]([NoteId], [Status])
end
GO

if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_Note_6')
begin
CREATE STATISTICS [_dta_stat_Note_6] ON [dbo].[Note]([Public], [NoteId])
end
GO

if NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'MSIDX_Userid')
begin
Create index MSIDX_Userid on [dbo].[note](userid, status)
end
GO

