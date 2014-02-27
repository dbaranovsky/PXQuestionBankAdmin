--add the product id column to the table and remove email data as
-- those values are moved to a new table emailvalues

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EmailTracking' AND COLUMN_NAME = 'ProductId')
BEGIN
	ALTER TABLE [dbo].[EmailTracking] 
		ADD [ProductId] [nvarchar](50)
END
GO

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'EmailTracking' AND COLUMN_NAME = 'EmailData')
BEGIN
	ALTER TABLE [dbo].[EmailTracking] 
		DROP COLUMN [EmailData]
END
GO