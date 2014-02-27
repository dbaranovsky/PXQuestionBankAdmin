
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS WHERE id = OBJECT_ID(N'[dbo].[UpdateSharedCourseItem]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[UpdateSharedCourseItem]	
END
GO

CREATE Procedure [dbo].[UpdateSharedCourseItem]
(
	@courseId BIGINT,
	@SharingUserId BIGINT,
	@ItemId VARCHAR(50),
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

		UPDATE	[dbo].[SharedCourseItems]
		SET		IsActive = @isActive, 
				Modified = current_timestamp
		WHERE	SharedCourseId	= @SharedCourseId
			AND	ItemId			= @ItemId

	END
End

GO
