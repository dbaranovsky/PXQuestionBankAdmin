SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[GetVideoNotes]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetVideoNotes]
END
GO

CREATE PROCEDURE [dbo].[GetVideoNotes]
(	
	@reqXml XML
)
AS
BEGIN

	DECLARE @courseId NVARCHAR(50),
		@itemId NVARCHAR(50),		
		@noteId NVARCHAR(50),				
		@currentUserId NVARCHAR(50),
		@videoId NVARCHAR(50),
		@studentUserId NVARCHAR(50),
		@showInstructorNotes bit,
		@showAllStudentNotes bit
		
	
	SELECT @courseId = Node.value('courseId[1]', 'nvarchar(50)'),
		   @itemId = Node.value('itemId[1]', 'nvarchar(50)'),		   
		   @noteId = Node.value('noteId[1]', 'nvarchar(50)'),		   
		   @currentUserId = Node.value('currentUserId[1]', 'nvarchar(50)'),		   
		   @videoId = Node.value('videoId[1]', 'nvarchar(50)'),
		   @studentUserId = Node.value('studentUserId[1]', 'nvarchar(50)'),
		   @studentUserId = Node.value('studentUserId[1]', 'nvarchar(50)'),
		   @showInstructorNotes = Node.value('userNote[1]/showInstructorNotes[1]', 'bit'),
		   @showAllStudentNotes = Node.value('userNote[1]/showAllStudentNotes[1]', 'bit')
	FROM @reqXml.nodes('/notesSearch') TempXML (Node)
	
	if @courseId IS NOT NULL AND @itemId IS NOT NULL
	begin
		if @showallStudentNotes = 1 
		begin
			if @showInstructorNotes = 1 
			begin
				SELECT u.FirstName, u.LastName, vn.Id as NoteId, 
				vn.UserId,vn.[text],vn.ItemId,vn.CourseId,vn.Created,vn.Modified, vn.AccessType,
				vn.MinTime, vn.MaxTime
				FROM VideoNotes vn  JOIN Users u ON u.UserId = vn.UserId 
				WHERE vn.CourseId = @courseId AND vn.ItemId = @itemId AND 
				vn.VideoId = @videoId and vn.IsDeleted = 'false'
			end
			else
				begin
					SELECT u.FirstName, u.LastName, vn.Id as NoteId, 
					vn.UserId,vn.[text],vn.ItemId,vn.CourseId,vn.Created,vn.Modified, vn.AccessType,
					vn.MinTime, vn.MaxTime
					FROM VideoNotes vn  JOIN Users u ON u.UserId = vn.UserId 
					WHERE vn.CourseId = @courseId AND vn.ItemId = @itemId AND 
					vn.VideoId = @videoId and vn.IsDeleted = 'false' AND 
					vn.AccessType != 3
				end
		end
		else if @showInstructorNotes = 1
		begin
			SELECT u.FirstName, u.LastName, vn.Id as NoteId, 
			vn.UserId,vn.[text],vn.ItemId,vn.CourseId,vn.Created,vn.Modified, vn.AccessType,
			vn.MinTime, vn.MaxTime
			FROM VideoNotes vn  JOIN Users u ON u.UserId = vn.UserId 
			WHERE (vn.UserId = @currentUserId or vn.AccessType=3 or vn.UserId = @studentUserId) AND vn.CourseId = @courseId AND 
			vn.VideoId = @videoId and vn.IsDeleted = 'false'
		end
		else
		begin 
			SELECT u.FirstName, u.LastName, vn.Id as NoteId, 
			vn.UserId,vn.[text],vn.ItemId,vn.CourseId,vn.Created,vn.Modified, vn.AccessType,
			vn.MinTime, vn.MaxTime
			FROM VideoNotes vn  JOIN Users u ON u.UserId = vn.UserId 
			WHERE (vn.UserId = @currentUserId or vn.UserId = @studentUserId) AND vn.CourseId = @courseId AND 
			vn.VideoId = @videoId and vn.IsDeleted = 'false'
		end
	END
END
GO

