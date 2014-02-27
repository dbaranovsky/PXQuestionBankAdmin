
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[UpdateNoteStatus]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[UpdateNoteStatus]
END
GO

CREATE Procedure [dbo].[UpdateNoteStatus]
(
	@highlightId uniqueidentifier = NULL,
	@noteId uniqueidentifier = NULL,
	@userId nvarchar(50) = NULL,
	@itemId nvarchar(50) = NULL,
	@reviewId nvarchar(50) = NULL,
	@courseId nvarchar(50) = NULL,
	@enrollmentId nvarchar(50) = NULL,
	@updateParentHighlight bit = 0,
	@status int
)
As
Begin

	create table #NotesToUpdate (
		nid uniqueidentifier
	)
	
	if @highlightId is not null
	begin
		if @updateParentHighlight = 1
		begin
		exec UpdateHighlightStatus @highlightId = @highlightId, @status = @status
		end
		insert #NotesToUpdate
		select hn.NoteId as nid
		from HighlightNotes hn
		where HighlightId = @highlightId
	end
	
	if @noteId is not NULL
	begin
		--specific note is being updated
		insert #NotesToUpdate
		select h.NoteId as nid
		from Note h
		where NoteId = @noteId
	end
	
	if @itemId is not NULL and @courseId is not NULL and @reviewId is NULL
	begin
		--all notes attached to an item are being updated
		insert #NotesToUpdate
		select h.NoteId as nid			
		from Note h
				inner join
			 ItemNotes ih on h.NoteId = ih.NoteId 
	    where ih.ItemId = @itemId and ih.CourseId = @courseId and h.UserId = @userId
	    
	    insert #NotesToUpdate
		select h.NoteId as nid			
		from ItemHighlights ih
				inner join
			 HighlightNotes hn on ih.HighlightId = hn.HighlightId
				inner join
			 Note h on h.NoteId = hn.NoteId
		where ih.ItemId = @itemId and ih.CourseId = @courseId and h.UserId = @userId
	end
	
	if @itemId is not NULL and @reviewId is not NULL and @enrollmentId is not NULL
	begin
		--all notes attached to a review		
		insert #NotesToUpdate
		select h.NoteId as nid
		from Note h
				inner join
			 ReviewNotes ih on h.NoteId = ih.NoteId
	    where ih.ItemId = @itemId and ih.ReviewId = @reviewId and ih.EnrollmentId = @enrollmentId and h.UserId = @userId
	    
	    insert #NotesToUpdate
		select h.NoteId as nid			
		from ReviewHighlights ih
				inner join
			 HighlightNotes hn on ih.HighlightId = hn.HighlightId
				inner join
			 Note h on h.NoteId = hn.NoteId
		where ih.ItemId = @itemId and ih.EnrollmentId = @enrollmentId and ih.ReviewId = @reviewId and h.UserId = @userId
	end
	
	if @itemId is not NULL and @enrollmentId is not null and @reviewId is NULL
	begin
		--all notes attached to a submission
		insert #NotesToUpdate
		select h.NoteId as nid	
		from Note h
				inner join
			 SubmissionNotes ih on h.NoteId = ih.NoteId
	    where ih.ItemId = @itemId and ih.EnrollmentId = @enrollmentId and h.UserId = @userId
	    
	    insert #NotesToUpdate
		select h.NoteId as nid			
		from SubmissionHighlights ih
				inner join
			 HighlightNotes hn on ih.HighlightId = hn.HighlightId
				inner join
			 Note h on h.NoteId = hn.NoteId
		where ih.ItemId = @itemId and ih.EnrollmentId = @enrollmentId and h.UserId = @userId
	end
	
	declare @userType int = 0
	if @userId is not null
	begin
		select @userType = [Type] from UserTypes where UserId = @userId and CourseId = @courseId
	end
	
	if @status = 2 and @userType <> 0 -- dont delete if note has a reply or delete if instructor.
	begin
		with HighlightIds(hid)
		as
		(
			select hn.HighlightId from HighlightNotes hn
			inner join #NotesToUpdate nu on nu.nid = hn.NoteId
		)

		delete from #NotesToUpdate
		where nid in 
		(
			-- Returns note ids which has reply from other users.
			select hn.NoteId from HighlightNotes hn
			inner join 
			(
				-- Returns all highlight ids which has a reply from other users.
				select h.hid from Note n
				inner join HighlightNotes hn on hn.NoteId = n.NoteId
				inner join HighlightIds h on h.hid = hn.HighlightId
				where n.[UserId] <> @userId
				group by h.hid
				having COUNT(1) > 0
			) h on h.hid = hn.HighlightId
		)
	end

	-- Update note status if its not locked or if user is instructor.
	Update Note
	Set 
		[Status] = @status,
		Modified = GETDATE()
    from Note n inner join #NotesToUpdate nu on n.NoteId = nu.nid
	where @userType = 0 or n.[Status] <> 1
	
	if @updateParentHighlight = 1
	begin
		update h
		set
			[Status] = @status,
			Modified = GETDATE()
		from Highlights h
		inner join HighlightNotes hn on hn.HighlightId = h.HighlightId
		inner join #NotesToUpdate hu on hn.NoteId = hu.nid
		inner join Note n on n.NoteId = hu.nid
		where n.UserId = h.UserId and (@userType = 0 or h.[Status] <> 1)
	end
	
	drop table #NotesToUpdate
	
End
GO
