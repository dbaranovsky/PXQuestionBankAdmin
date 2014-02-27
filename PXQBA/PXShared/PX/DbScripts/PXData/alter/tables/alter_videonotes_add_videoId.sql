-- Adds the videoid column if it doesn't already exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'VideoNotes' AND COLUMN_NAME = 'VideoId')
BEGIN
	ALTER TABLE VideoNotes ADD VideoId NVARCHAR(50)
END

GO