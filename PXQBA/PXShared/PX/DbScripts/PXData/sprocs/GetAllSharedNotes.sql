SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[GetAllSharedNotes]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetAllSharedNotes]
END
GO

CREATE PROCEDURE [dbo].[GetAllSharedNotes]
(	
	@studentId NVARCHAR(50),
	@courseId NVARCHAR(50)
)
AS
BEGIN
	SELECT u.FirstName as 'FirstNameSharee', u.LastName as 'LastNameSharee' , us.SharedUserId as 'SharedStudentId'
	FROM UserShares us  JOIN Users u ON u.UserId = us.SharedUserId  
	WHERE us.UserId = @studentId AND us.CourseId = @courseId	
END
GO
