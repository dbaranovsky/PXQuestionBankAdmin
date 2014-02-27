
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS WHERE id = OBJECT_ID(N'[dbo].[AddSharedCourse]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[AddSharedCourse]	
END
GO

CREATE PROCEDURE [dbo].[AddSharedCourse]
	@courseId BIGINT,
	@userId BIGINT,
	@anonymousName varchar(50),
	@note text
AS
BEGIN
	SET NOCOUNT ON;

	SELECT @note =			ISNULL(@note,			''),
		   @anonymousName = ISNULL(@anonymousName,	'');

	INSERT INTO [dbo].[SharedCourses]
		(CourseId, UserId, AnonymousName, Created, Modified, Note)
		VALUES
		(@courseId, @userId, @anonymousName, current_timestamp, current_timestamp, @note);

END

GO
