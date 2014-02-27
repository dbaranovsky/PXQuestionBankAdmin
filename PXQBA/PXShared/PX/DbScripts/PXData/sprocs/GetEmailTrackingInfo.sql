SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEmailTrackingInfo]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetEmailTrackingInfo]
GO


CREATE PROCEDURE [dbo].[GetEmailTrackingInfo] 
	@ItemId NVARCHAR(50),
	@Status VARCHAR(15) = 'add',
	@CourseId VARCHAR(50) = ''
AS
BEGIN
	DECLARE @Subject VARCHAR(4000)
	DECLARE @Body VARCHAR(4000)
	DECLARE @EmailId BIGINT
	
	IF(@CourseId = '')
	BEGIN
		IF(@Status = '')
		BEGIN
			SELECT TOP (1) @EmailId = EmailId FROM [dbo].[EmailTracking]	
				WHERE ItemId = @ItemId
		END
		ELSE
		BEGIN
			SELECT TOP (1) @EmailId = EmailId FROM [dbo].[EmailTracking]	
				WHERE ItemId = @ItemId AND [Status] = @Status
		END
	END
	ELSE
	BEGIN
		IF(@Status = '')
		BEGIN
			SELECT TOP (1) @EmailId = EmailId FROM [dbo].[EmailTracking]	
				WHERE ItemId = @ItemId 
				AND CourseId = @CourseId
		END
		ELSE
		BEGIN
			SELECT TOP (1) @EmailId = EmailId FROM [dbo].[EmailTracking]	
				WHERE ItemId = @ItemId AND [Status] = @Status
				AND CourseId = @CourseId
		END
	END
	
	SELECT @Body = ISNULL(MAX([Value]), '') FROM [dbo].[Emailvalues]
		WHERE TrackingEmailId = @EmailId AND [Key] = 'BodyText'
	
	SELECT @Subject = ISNULL(MAX([Value]), '') FROM [dbo].[Emailvalues]
		WHERE TrackingEmailId = @EmailId AND [Key] = 'SubjectText'
	
	SELECT CourseId, ProductId, InstructorId, SendOnDate, [Status], NotificationType, 
		ItemId, @Subject AS EmailSubject, @Body AS EmailBody
		FROM [dbo].[EmailTracking] WHERE EmailId = @EmailId
END

GO