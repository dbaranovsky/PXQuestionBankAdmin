if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_ItemHighlights_1')
begin
CREATE STATISTICS [_dta_stat_ItemHighlights_1] ON [dbo].[ItemHighlights]([ItemId], [HighlightId])
end
GO

if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_ItemHighlights_2')
begin
CREATE STATISTICS [_dta_stat_ItemHighlights_2] ON [dbo].[ItemHighlights]([CourseId], [ItemId])
end
GO

if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_ItemHighlights_3')
begin
CREATE STATISTICS [_dta_stat_ItemHighlights_3] ON [dbo].[ItemHighlights]([HighlightId], [CourseId], [ItemId])
end
GO

if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_ItemHighlights_4')
begin
CREATE STATISTICS [_dta_stat_ItemHighlights_4] ON [dbo].[ItemHighlights]([Id], [ItemId], [CourseId], [HighlightId])
end
GO

if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_index_ItemHighlights_Item_Course_Highlight')
begin
CREATE NONCLUSTERED INDEX [_dta_index_ItemHighlights_Item_Course_Highlight] ON [dbo].[ItemHighlights] 
(
	[ItemId] ASC,
	[CourseId] ASC,
	[HighlightId] ASC
)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
end
GO


