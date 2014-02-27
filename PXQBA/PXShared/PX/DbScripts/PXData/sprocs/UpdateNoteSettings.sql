SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[UpdateNoteSettings]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[UpdateNoteSettings]
END
GO

-- this query will update a user's note settings
CREATE PROCEDURE [dbo].[UpdateNoteSettings]
(	
	@studentId NVARCHAR(50),
	@courseId NVARCHAR(50),
	@sharerId nvarchar(50) = NULL,
	@sharerHighlights bit = NULL,
	@sharerNotes bit = NULL,
	@myHighlights bit = NULL,
	@myNotes bit = NULL,
	@instHighlights bit = NULL,
	@instNotes bit = NULL
)
AS
BEGIN

	if @sharerId is NULL
	begin
		if @myHighlights is not NULL
		begin
			update UserNoteSettings
			set ShowMyHighlights = @myHighlights
			where UserId = @studentId and CourseId = @courseId
		end
		
		if @myNotes is not NULL
		begin
			update UserNoteSettings
			set ShowMyNotes = @myNotes
			where UserId = @studentId and CourseId = @courseId
		end
		
		if @instHighlights is not NULL
		begin
			update UserNoteSettings
			set ShowInstructorHighlights = @instHighlights
			where UserId = @studentId and CourseId = @courseId
		end
		
		if @instNotes is not NULL
		begin
			update UserNoteSettings
			set ShowInstructorNotes = @instNotes
			where UserId = @studentId and CourseId = @courseId
		end
	end
	else
	begin
	
		if not exists (select us.Id from UserShares us where us.CourseId = @courseId and us.UserId = @sharerId and us.SharedUserId = @studentId)
		begin
			insert UserShares (UserId, SharedUserId, CourseId, HighlightsEnabled, HighlightColor, NotesEnabled)
			values (@sharerId, @studentId, @courseId, ISNULL(@sharerHighlights, 0), 'color-1', ISNULL(@sharerNotes, 0))
		end
		else
		begin	
			if @sharerHighlights is not NULL
			begin
				update UserShares
				set HighlightsEnabled = @sharerHighlights
				where UserId = @sharerId and SharedUserId = @studentId and CourseId = @courseId
			end
			
			if @sharerNotes is not NULL
			begin
				update UserShares
				set NotesEnabled = @sharerNotes
				where UserId = @sharerId and SharedUserId = @studentId and CourseId = @courseId
			end
		end
	end
END
GO
