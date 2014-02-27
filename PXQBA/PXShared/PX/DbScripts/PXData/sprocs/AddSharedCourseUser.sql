
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS WHERE id = OBJECT_ID(N'[dbo].[AddSharedCourseUser]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[AddSharedCourseUser]	
END
GO

CREATE PROCEDURE [dbo].[AddSharedCourseUser]
	@courseId BIGINT,
	@SharingUserId BIGINT,
	@SharedUserId BIGINT
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

		INSERT INTO [dbo].[SharedCourseUsers]
			(SharedCourseId, UserId, IsActive, Created, Modified)
			VALUES
			(@SharedCourseId, @SharedUserId, 1, current_timestamp, current_timestamp);

	end
END


GO
