SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CancelEmailTracking]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[CancelEmailTracking]
END
GO

CREATE PROCEDURE [dbo].[CancelEmailTracking] 
	@CourseId NVARCHAR(50),
	@NotificationType INT,
	@ItemId NVARCHAR(50),
	@Status VARCHAR(15) = 'cancel'
AS
BEGIN

    UPDATE [dbo].[EmailTracking] SET [Status] = @Status
		WHERE CourseId = @CourseId AND
		NotificationType = ISNULL(@NotificationType, NotificationType) AND
		ItemId = ISNULL(@ItemId, ItemId)
END

GO


