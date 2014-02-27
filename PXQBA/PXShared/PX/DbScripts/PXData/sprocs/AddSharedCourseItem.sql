
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS WHERE id = OBJECT_ID(N'[dbo].[AddSharedCourseItem]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[AddSharedCourseItem]	
END
GO

CREATE PROCEDURE [dbo].[AddSharedCourseItem]
	@courseId BIGINT,
	@SharingUserId BIGINT,
	@itemId VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @SharedCourseId BIGINT;
	
	SELECT @SharedCourseId = -1;
	
	SELECT	@SharedCourseId = SharedCourseId
	FROM	[dbo].[SharedCourses] 
	WHERE	CourseId = @courseId 
		AND UserId = @SharingUserId;

	IF @SharedCourseId > 0 BEGIN

		INSERT INTO [dbo].[SharedCourseItems]
			(SharedCourseId, ItemId, IsActive, Created, Modified)
			VALUES
			(@SharedCourseId, @itemId, 1, current_timestamp, current_timestamp);

	END
END

GO
