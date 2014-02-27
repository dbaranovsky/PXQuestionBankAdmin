SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateReminderEmailStatus]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateReminderEmailStatus]
GO


CREATE PROCEDURE [dbo].[UpdateReminderEmailStatus] 
	@TrackingEmailId BIGINT, 
	@SuccessEmails XML,
	@FailureEmails XML
AS
BEGIN
	
	SET NOCOUNT ON;

    DECLARE @SuccessEmailDetails TABLE (Email VARCHAR(50))
    DECLARE @FailureEmailDetails TABLE (Email VARCHAR(50))
    
    INSERT INTO @SuccessEmailDetails SELECT Node.value('.', 'VARCHAR (50)') 
		FROM @SuccessEmails.nodes('/emails/email') SuccessXml (Node)
	INSERT INTO @FailureEmailDetails SELECT Node.value('.', 'VARCHAR (50)') 
		FROM @FailureEmails.nodes('/emails/email') FailureXml (Node)
		
	--Update the email tracking status
    UPDATE [dbo].[EmailTracking] SET [Status] = CASE 
		WHEN ((SELECT COUNT(1) FROM @FailureEmailDetails) = 0) THEN 'sent'
		WHEN [Status] = 'failed1' THEN 'failed2'
		WHEN [Status] = 'failed2' THEN 'failed3'
		ELSE 'failed1' END WHERE EmailId = @TrackingEmailId
       
	--Update the email send history details
    INSERT INTO [dbo].[EmailSendHistory] SELECT @TrackingEmailId, Email, 'P' FROM @SuccessEmailDetails
		WHERE Email NOT IN (SELECT EmailAddress FROM [dbo].[EmailSendHistory] WHERE TrackingEmailId = @TrackingEmailId)
	
	UPDATE [dbo].[EmailSendHistory] SET [Status] = 'P' WHERE TrackingEmailId = @TrackingEmailId AND 
		[Status] = 'F' AND EmailAddress IN (SELECT Email FROM @SuccessEmailDetails)	
	
	INSERT INTO [dbo].[EmailSendHistory] SELECT @TrackingEmailId, Email, 'F' FROM @FailureEmailDetails
		WHERE Email NOT IN (SELECT EmailAddress FROM [dbo].[EmailSendHistory] WHERE TrackingEmailId = @TrackingEmailId)
	
END

GO


