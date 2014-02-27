
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS WHERE id = OBJECT_ID(N'[dbo].[UpdateSharedCourseUser]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[UpdateSharedCourseUser]	
END
GO

CREATE Procedure [dbo].[UpdateSharedCourseUser]
(
	@courseId BIGINT,
	@SharingUserId BIGINT,
	@SharedUserId BIGINT,
	@isActive BIT
)
As
Begin
	DECLARE @SharedCourseId BIGINT;

	SELECT @SharedCourseId = -1;

	SELECT	@SharedCourseId = SharedCourseId
	FROM	[dbo].[SharedCourses] 
	WHERE	CourseId = @courseId 
		AND UserId = @SharingUserId;

	IF @SharedCourseId > 0 BEGIN

		UPDATE	[dbo].[SharedCourseUsers]
		SET		IsActive = @isActive, 
				Modified = current_timestamp
		WHERE	SharedCourseId	= @SharedCourseId
			AND	UserId			= @SharedUserId

	END
End

GO
