
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserShares' and COLUMN_NAME = 'NotesEnabled')
BEGIN
	ALTER TABLE UserShares ADD NotesEnabled bit default 0 not null
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserShares' and COLUMN_NAME = 'Enabled')
begin
	exec sp_rename 'UserShares.[Enabled]', 'HighlightsEnabled', 'COLUMN'
end