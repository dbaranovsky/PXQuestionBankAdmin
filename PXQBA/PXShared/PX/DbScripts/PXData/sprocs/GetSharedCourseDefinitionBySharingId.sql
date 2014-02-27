
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS WHERE id = OBJECT_ID(N'[dbo].[GetSharedCourseDefinitionBySharingId]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetSharedCourseDefinitionBySharingId]	
END
GO

CREATE PROCEDURE [dbo].[GetSharedCourseDefinitionBySharingId]
	@userId BIGINT
AS
BEGIN

	SELECT sc.SharedCourseId, sc.CourseId, sc.UserId
	FROM   [dbo].[SharedCourseUsers] scu
	INNER JOIN [dbo].[SharedCourses] sc
		ON	scu.SharedCourseId = sc.SharedCourseId
	WHERE  scu.UserId = @userId
	GROUP BY sc.SharedCourseId, sc.CourseId, sc.UserId

END

GO
