-- Adds the videoid column if it doesn't already exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Dashboard' AND COLUMN_NAME = 'ProductCourse_Id')
BEGIN
	ALTER TABLE Dashboard ADD ProductCourse_Id [varchar](100) NULL DEFAULT 'default'
END
