
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS WHERE id = OBJECT_ID(N'[dbo].[GetSharedCourseDefinition]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetSharedCourseDefinition]	
END
GO

CREATE PROCEDURE [dbo].[GetSharedCourseDefinition]
	@entityId BIGINT,
	@userId BIGINT
AS
BEGIN

	SELECT SharedCourseId, CourseId, UserId, AnonymousName, Note
	FROM   [dbo].[SharedCourses]
	WHERE  (UserId   = @userId    OR  @userId   IS NULL)
	   AND (CourseId = @entityId  OR  @entityId IS NULL) 
	   AND NOT (@userId IS NULL AND @entityId IS NULL);

END
GO
