
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS WHERE id = OBJECT_ID(N'[dbo].[UpdateSharedCourse]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[UpdateSharedCourse]	
END
GO

CREATE PROCEDURE [dbo].[UpdateSharedCourse]
	@courseId BIGINT,
	@userId BIGINT,
	@anonymousName varchar(50),
	@note text
AS
BEGIN
	SET NOCOUNT ON;

	SELECT @note =			ISNULL(@note,			''),
		   @anonymousName = ISNULL(@anonymousName,	'');

	UPDATE	[dbo].[SharedCourses]
	SET		AnonymousName	= @anonymousName, 
			Modified		= current_timestamp, 
			Note			= @note
	WHERE	CourseId	= @courseId 
		AND UserId		= @userId;

END

GO
