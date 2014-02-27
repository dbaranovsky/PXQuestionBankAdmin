SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddEmailTracking]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[AddEmailTracking]
END
GO

CREATE PROCEDURE [dbo].[AddEmailTracking] 
	@CourseId NVARCHAR(50),
	@ProductId NVARCHAR(50),
	@InstructorId INT,
	@SendOnDate DATETIME,
	@Status VARCHAR(15),
	@NotificationType INT,
	@ItemId NVARCHAR(50),	
	@Subject NVARCHAR(4000),
	@Body NVARCHAR(MAX),
	@TemplateId INT,
	@ToList NVARCHAR(4000) = ''
AS
BEGIN
	DECLARE @EmailId BIGINT

	IF EXISTS (SELECT ItemId FROM [dbo].[EmailTracking] WHERE ItemId = @ItemId and isnull(ItemID,'0') != '0')
	BEGIN
		SELECT TOP 1 @EmailId = EmailId FROM [dbo].[EmailTracking] WHERE ItemId = @ItemId
	
		UPDATE [dbo].[EmailTracking] SET 
			CourseId = ISNULL(@CourseId, CourseId), 
			ProductId = ISNULL(@ProductId, ProductId),
			InstructorId = ISNULL(@InstructorId, InstructorId), 
			SendOnDate = ISNULL(@SendOnDate, SendOnDate), 
			[Status] = ISNULL(@Status, [Status]), 
			NotificationType = ISNULL(@NotificationType, NotificationType)
		WHERE ItemId = @ItemId
		
		--Delete exisiting subject and body text
		DELETE FROM [dbo].[EmailValues] WHERE TrackingEmailId = @EmailId AND [Key] = 'SubjectText'
		DELETE FROM [dbo].[EmailValues] WHERE TrackingEmailId = @EmailId AND [Key] = 'BodyText'
		DELETE FROM [dbo].[EmailValues] WHERE TrackingEmailId = @EmailId AND [Key] = 'ToList'
	END
	ELSE
	BEGIN
		INSERT INTO [dbo].[EmailTracking]
			(CourseId, ProductId, InstructorId, SendOnDate, [Status], NotificationType, ItemId)	VALUES
			(@CourseId, @ProductId, @InstructorId, @SendOnDate, @Status, @NotificationType, @ItemId)
	
		SELECT @EmailId = @@IDENTITY
		
		--Insert value in email template mapping
		IF NOT EXISTS (SELECT 1 FROM [dbo].[EmailTemplateMapping] WHERE ProductId = @ProductId
			AND EventType = @NotificationType AND TemplateId = @TemplateId)
		BEGIN
			INSERT INTO [dbo].[EmailTemplateMapping] (TemplateId, ProductId, EventType)
				VALUES (@TemplateId, @ProductId, @NotificationType)
		END	
	END	
	
	--Insert subject if available
	IF (@Subject IS NOT NULL AND @Subject <> '')
	BEGIN
		INSERT INTO [dbo].[EmailValues] (TrackingEmailId, [Key], Value)
			VALUES (@EmailId, 'SubjectText', @Subject)
	END
	
	--Insert body if available
	IF (@Body IS NOT NULL AND @Body <> '')
	BEGIN
		INSERT INTO [dbo].[EmailValues] (TrackingEmailId, [Key], Value)
			VALUES (@EmailId, 'BodyText', @Body)
	END
	
	--Insert toList if available
	IF (@ToList IS NOT NULL AND @ToList <> '')
	BEGIN
		INSERT INTO [dbo].[EmailValues] (TrackingEmailId, [Key], Value)
			VALUES (@EmailId, 'ToList', @ToList)
	END
	
END



GO


