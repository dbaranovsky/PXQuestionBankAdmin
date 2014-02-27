if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_Highlights_1')
begin
CREATE STATISTICS [_dta_stat_Highlights_1] ON [dbo].[Highlights]([Status], [HighlightId], [UserId])
end 
GO

if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_Highlights_2')
begin
CREATE STATISTICS [_dta_stat_Highlights_2] ON [dbo].[Highlights]([Created], [HighlightId], [Public])
end 
GO

if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_Highlights_3')
begin
CREATE STATISTICS [_dta_stat_Highlights_3] ON [dbo].[Highlights]([Public], [HighlightId])
end 
GO

if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_Highlights_4')
begin
CREATE STATISTICS [_dta_stat_Highlights_4] ON [dbo].[Highlights]([HighlightId], [Status])
end 
GO

if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_Highlights_5')
begin
CREATE STATISTICS [_dta_stat_Highlights_5] ON [dbo].[Highlights]([HighlightId], [UserId])
end 
GO

if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_Highlights_6')
begin
CREATE STATISTICS [_dta_stat_Highlights_6] ON [dbo].[Highlights]([Created], [HighlightId], [Status])
end 
GO

if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_index_Highlights_HighlightId_Status')
begin
CREATE NONCLUSTERED INDEX [_dta_index_Highlights_HighlightId_Status] ON [dbo].[Highlights] 
(
	[HighlightId] ASC,
	[Status] ASC
)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
end 
GO

