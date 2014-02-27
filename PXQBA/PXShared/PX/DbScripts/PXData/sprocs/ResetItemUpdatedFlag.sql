
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[ResetItemUpdatedFlag]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[ResetItemUpdatedFlag]
END
GO

CREATE PROCEDURE [dbo].[ResetItemUpdatedFlag]
(
	@courseId nvarchar(50),
	@enrollmentId nvarchar(50), 
	@itemId nvarchar(50),
	@status tinyint = 0
)
AS
BEGIN
	UPDATE dbo.[ItemUpdates]
	SET [Status] = @status
	WHERE CourseId = @courseId AND EnrollmentId = @enrollmentId AND [ItemId] = @itemId
END

GO
