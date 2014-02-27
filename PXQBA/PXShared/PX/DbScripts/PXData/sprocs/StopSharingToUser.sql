SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[StopSharingToUser]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[StopSharingToUser]
END
GO

CREATE PROCEDURE [dbo].[StopSharingToUser]
(	
	@studentId NVARCHAR(50),
	@sharedStudentId NVARCHAR(50),
	@courseId NVARCHAR(50)
)
AS
BEGIN	
		DELETE FROM UserShares WHERE UserId = @studentId AND CourseId = @courseId AND SharedUserId = @sharedStudentId
END
GO

