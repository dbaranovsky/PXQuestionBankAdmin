
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[FlagItemAsUpdated]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[FlagItemAsUpdated]
END
GO

CREATE PROCEDURE [dbo].[FlagItemAsUpdated]
(
	@courseId nvarchar(50),
	@enrollmentId nvarchar(50), 
	@itemId nvarchar(50),
	@status tinyint = 1
)
AS
BEGIN
	--Check if student already has a record for that item
	IF (SELECT COUNT(1) FROM dbo.[ItemUpdates]
		WHERE CourseId = @courseId AND EnrollmentId = @enrollmentId AND [ItemId] = @itemId) > 0
	BEGIN
		-- Update the status flage and updated time
		UPDATE dbo.[ItemUpdates] SET DateUpdated = GETDATE(), [Status] = @status
		WHERE CourseId = @courseId AND EnrollmentId = @enrollmentId AND [ItemId] = @itemId
	END 
	ELSE
	BEGIN
		--Insert a record for the item
		INSERT INTO dbo.[ItemUpdates] (CourseId, EnrollmentId, [ItemId], DateUpdated, [Status])
		VALUES (@courseId, @enrollmentId, @itemId, GETDATE(), @status)
	END
END

GO
