
if NOT EXISTS (SELECT * FROM sysindexes WHERE name = '_dta_index_ItemUpdates_EnrollmentId_Status')
begin
CREATE NONCLUSTERED INDEX [_dta_index_ItemUpdates_EnrollmentId_Status] ON [dbo].[ItemUpdates] 
(
	[EnrollmentId] ASC,
	[Status] ASC
)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
end 
GO



