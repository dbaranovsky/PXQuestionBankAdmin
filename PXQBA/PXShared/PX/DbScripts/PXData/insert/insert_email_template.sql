SET NOCOUNT ON
GO

IF NOT EXISTS (SELECT 1 FROM [dbo].[EmailTemplate] WHERE Id = 1)
BEGIN
	INSERT INTO [dbo].[EmailTemplate] (Id, TemplateText, TemplateHtml)
	VALUES (1, 'Dear {studentname}, ' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) + 'You have a due date coming up for "{coursetitle}". Here are the details:' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) + 'What: {itemtitle}' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) + 'When: {itemduedate}' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) + 'Questions? Send me an e-mail: {instructoremail}', 'Dear {studentname}, Your assignment "{itemtitle}" on the course "{coursetitle}" is due on "{itemduedate}". Please contact your instructor {instructorname} on {instructoremail} for further details. This is a custom message "{customBody}"')
END
ELSE
BEGIN
	UPDATE [dbo].[EmailTemplate] SET TemplateText = 'Dear {studentname}, ' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) + 'You have a due date coming up for "{coursetitle}". Here are the details:' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) + 'What: {itemtitle}' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) + 'When: {itemduedate}' + CHAR(13) + CHAR(10) + CHAR(13) + CHAR(10) + 'Questions? Send me an e-mail: {instructoremail}', TemplateHtml = 'Dear {studentname}, Your assignment "{itemtitle}" on the course "{coursetitle}" is due on "{itemduedate}". Please contact your instructor {instructorname} on {instructoremail} for further details. This is a custom message "{customBody}"'
	WHERE Id = 1
END
GO

