-- Adds the access type column if it doesn't already exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'VideoNotes' AND COLUMN_NAME = 'AccessType')
BEGIN
	ALTER TABLE VideoNotes ADD AccessType int
END

GO