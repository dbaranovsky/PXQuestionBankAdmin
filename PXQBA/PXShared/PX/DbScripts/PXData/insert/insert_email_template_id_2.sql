
/****** Object:  Table [dbo].[EmailTemplate]    Script Date: 07/13/2012 13:32:52 ******/
IF NOT EXISTS (SELECT 1 FROM [dbo].[EmailTemplate] WHERE Id = 2)
BEGIN
	INSERT INTO [dbo].[EmailTemplate] (Id, TemplateText, TemplateHtml)
	VALUES (2, 'I just shared my e-Portfolio with you—click on the link below or copy and paste the url into your browser to explore.' +  CHAR(13) + CHAR(10) +  CHAR(13) + CHAR(10) +  '{customBody}', 'I just shared my e-Portfolio with you—click on the link below or copy and paste the url into your browser to explore. {customBody}')
END
ELSE
BEGIN
	UPDATE [dbo].[EmailTemplate] SET TemplateText = 'I just shared my e-Portfolio with you—click on the link below or copy and paste the url into your browser to explore.' +  CHAR(13) + CHAR(10) +  CHAR(13) + CHAR(10) +  '{customBody}', TemplateHtml = 'I just shared my e-Portfolio with you—click on the link below or copy and paste the url into your browser to explore. {customBody}'
	WHERE Id = 2
END
  
GO


