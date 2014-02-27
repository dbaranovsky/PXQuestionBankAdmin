
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS WHERE id = OBJECT_ID(N'[dbo].[GetSharedCourseDefinitionItems]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetSharedCourseDefinitionItems]	
END
GO

CREATE PROCEDURE [dbo].[GetSharedCourseDefinitionItems]
	@entityId BIGINT,
	@userId BIGINT
AS
BEGIN

	SELECT sci.SharedCourseId, sci.ItemId, sci.IsActive
	FROM   [dbo].[SharedCourseItems] sci
	INNER JOIN [dbo].[SharedCourses] sc 
		ON sc.SharedCourseId = sci.SharedCourseId
	WHERE  (sc.UserId   = @userId   OR  @userId   IS NULL)
	   AND (sc.CourseId = @entityId OR  @entityId IS NULL) 
	   AND NOT (@userId IS NULL AND @entityId IS NULL);
	
END

GO
