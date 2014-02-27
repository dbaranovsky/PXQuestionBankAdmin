-- Adds the color column if it doesn't already exists

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'UserShares' and COLUMN_NAME = 'HighlightColor')
BEGIN
	ALTER TABLE UserShares ALTER COLUMN HighlightColor NVARCHAR(20) NULL
END

GO