
/****** Object:  Table [dbo].[EmailTemplate]    Script Date: Apr 05 2013 16:32:52 ******/
IF NOT EXISTS (SELECT 1 FROM [dbo].[EmailTemplate] WHERE Id = 3)
BEGIN
	INSERT INTO [dbo].[EmailTemplate] (Id, TemplateText, TemplateHtml)
	VALUES (3, '{customBody}', '{customBody}')
END
ELSE
BEGIN
	UPDATE [dbo].[EmailTemplate] SET TemplateText = '{customBody}', TemplateHtml = '{customBody}'
	WHERE Id = 3
END
  
GO