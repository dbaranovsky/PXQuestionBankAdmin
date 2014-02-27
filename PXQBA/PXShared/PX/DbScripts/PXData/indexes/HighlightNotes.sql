


if NOT EXISTS (SELECT name FROM sysindexes WHERE name = '_dta_stat_HighlightNotes_1')
begin
CREATE STATISTICS [_dta_stat_HighlightNotes_1] ON [dbo].[HighlightNotes]([NoteId], [HighlightId])
end
GO

if NOT EXISTS (SELECT * FROM sysindexes WHERE name = '_dta_index_HighlightNotes_Hightlight_Note')
begin
CREATE NONCLUSTERED INDEX [_dta_index_HighlightNotes_Hightlight_Note] ON [dbo].[HighlightNotes] 
(
	[HighlightId] ASC,
	[NoteId] ASC
)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
end
GO

