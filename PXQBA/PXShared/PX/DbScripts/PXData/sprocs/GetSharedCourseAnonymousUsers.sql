
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS WHERE id = OBJECT_ID(N'[dbo].[GetSharedCourseAnonymousUsers]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetSharedCourseAnonymousUsers]	
END
GO

CREATE PROCEDURE [dbo].[GetSharedCourseAnonymousUsers]
(
	@courseId BIGINT
)
AS
BEGIN	

	SELECT UserId, AnonymousName FROM dbo.SharedCourses WHERE CourseId = @courseId AND AnonymousName <> ''
	 
END

GO
