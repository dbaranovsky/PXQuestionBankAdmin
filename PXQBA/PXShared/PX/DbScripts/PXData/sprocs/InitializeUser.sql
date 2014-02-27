SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[InitializeUser]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[InitializeUser]
END
GO

CREATE PROCEDURE [dbo].[InitializeUser]
(	
	@userId NVARCHAR(50),
	@firstName nvarchar(50),
	@lastName nvarchar(50),
	@courseId NVARCHAR(50),
	@userType int = 1
)
AS
BEGIN

	if not exists (select UserId from Users where UserId = @userId)
	begin
		insert Users (UserId, FirstName, LastName) values( @userId, @firstName, @lastName)
	end
	
	if @courseId is not NULL and @courseId <> ''
	begin
		if not exists (select Id from UserNoteSettings where UserId = @userId and CourseId = @courseId)
		begin
			insert UserNoteSettings (UserId, CourseId, ShowMyNotes, ShowInstructorNotes, ShowMyHighlights, ShowInstructorHighlights, HighlightColor)
			values (@userId, @courseId, 1, 1, 1, 1, 'color-1')
		end
		
		if not exists (select Id from UserTypes where UserId = @userId and CourseId = @courseId)
		begin
			insert UserTypes (UserId, CourseId, [Type])
			values (@userId, @courseId, @userType)
		end
	end
	
END
GO
