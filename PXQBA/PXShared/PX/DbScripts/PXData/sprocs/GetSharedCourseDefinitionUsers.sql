
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS WHERE id = OBJECT_ID(N'[dbo].[GetSharedCourseDefinitionUsers]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetSharedCourseDefinitionUsers]	
END
GO

CREATE PROCEDURE [dbo].[GetSharedCourseDefinitionUsers]
	@entityId BIGINT,
	@userId BIGINT
AS
BEGIN

	SELECT scu.SharedCourseId, scu.UserId, scu.IsActive
	FROM   [dbo].[SharedCourseUsers] scu
	INNER JOIN [dbo].[SharedCourses] sc 
		ON sc.SharedCourseId = scu.SharedCourseId
	WHERE  (sc.UserId   = @userId   OR  @userId   IS NULL)
	   AND (sc.CourseId = @entityId OR  @entityId IS NULL) 
	   AND NOT (@userId IS NULL AND @entityId IS NULL);
   
END

GO
