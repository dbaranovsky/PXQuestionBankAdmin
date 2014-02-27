SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateEmailTracking]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[UpdateEmailTracking]
END
GO

CREATE PROCEDURE [dbo].[UpdateEmailTracking] 
	@EmailId BIGINT,
	@CourseId NVARCHAR(50),
	@InstructorId INT,
	@EmailData XML,
	@SendOnDate DATETIME,
	@Status VARCHAR(15),
	@NotificationType INT,
	@ItemId NVARCHAR(50) 
AS
BEGIN
    UPDATE [dbo].[EmailTracking] SET
			CourseId = ISNULL(@CourseId, CourseId), InstructorId = ISNULL(@InstructorId, InstructorId), 
			EmailData = ISNULL(@EmailData, EmailData), SendOnDate = ISNULL(@SendOnDate, SendOnDate), 
			[Status] = ISNULL(@Status, [Status]), NotificationType = ISNULL(@NotificationType, NotificationType),
			ItemId = ISNULL(@ItemId, ItemId) WHERE EmailId = @EmailId
END

GO


