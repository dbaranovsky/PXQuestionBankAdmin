
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[GetReminderEmailConfiguration]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetReminderEmailConfiguration]
END
GO

CREATE PROCEDURE [dbo].[GetReminderEmailConfiguration]
	
AS
BEGIN
	SELECT SendInterval AS ReminderInterval, TrackingDbConnection AS ConnectionString,
	SMTPAddress AS MailerSMTPAddress, UserName AS MailerUserName, [Password] as MailerPassword
	FROM [dbo].[EmailServiceConfiguration]
END
GO
