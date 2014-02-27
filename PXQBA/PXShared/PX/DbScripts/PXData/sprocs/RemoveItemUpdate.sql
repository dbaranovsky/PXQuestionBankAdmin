
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[RemoveItemUpdate]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[RemoveItemUpdate]
END
GO

CREATE PROCEDURE [dbo].[RemoveItemUpdate]
(
	@courseId nvarchar(50),
	@enrollmentId nvarchar(50), 
	@itemId nvarchar(50)
)
AS
BEGIN
	DELETE FROM dbo.[ItemUpdates]
	WHERE CourseId = @courseId AND EnrollmentId = @enrollmentId AND [ItemId] = @itemId
END

GO
