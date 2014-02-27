SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[GetNoteSettings]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetNoteSettings]
END
GO

-- this query loads all the current note settings for the user indicated by @studentId.
-- this includes the status off all shares the user currently has
CREATE PROCEDURE [dbo].[GetNoteSettings]
(	
	@studentId NVARCHAR(50),
	@courseId NVARCHAR(50),
	@itemId nvarchar(50) = null,
	@reviewId nvarchar(50) = null,
	@enrollmentId nvarchar(50) = null
)
AS
BEGIN

	create table #NoteSettings (
		StudentId nvarchar(50),
		FirstNameSharer nvarchar(50),
		LastNameSharer nvarchar(50),
		SharedStudentId nvarchar(50),
		FirstNameSharee nvarchar(50),
		LastNameSharee nvarchar(50),
		CourseId nvarchar(50),
		HighlightsEnabled bit,
		NotesEnabled bit,
		UserType int,
		ItemCount int default 0
	)

	create table #ItemCount (
		UserId nvarchar(50),
		ItemCount int default 0
	)
	
	create table #CousreInstructers (
		UserId nvarchar(50)
	)
	
	
	declare @sharedStudentId as nvarchar(50),
			@firstNameSharee as nvarchar(50),
			@lastNameSharee as nvarchar(50),
			@currentUserType as int
	
	-- in this case, "sharee" is always the user calling this sproc
	select @sharedStudentId = u.UserId,
		   @firstNameSharee = u.FirstName,
		   @lastNameSharee  = u.LastName
    from Users u
    where u.UserId = @studentId	
    
    select @currentUserType = [Type]
	from UserTypes
	where UserId = @studentId and CourseId = @courseId
	
	-- need to insert dummy records for "My Notes/Highlights" and "Instructor Notes/Highlights"
	insert #NoteSettings (StudentId, FirstNameSharer, LastNameSharer, SharedStudentId, FirstNameSharee, LastNameSharee, CourseId, HighlightsEnabled, NotesEnabled)
		select @sharedStudentId, 'Mine', '', @sharedStudentId, @firstNameSharee, @lastNameSharee, @courseId, uns.ShowMyHighlights, uns.ShowMyNotes
		from UserNoteSettings uns
		where uns.UserId = @sharedStudentId and uns.CourseId = @courseId
		
	insert #NoteSettings (StudentId, FirstNameSharer, LastNameSharer, SharedStudentId, FirstNameSharee, LastNameSharee, CourseId, HighlightsEnabled, NotesEnabled, UserType)
		select '0', 'Instructors', '', @sharedStudentId, @firstNameSharee, @lastNameSharee, @courseId, uns.ShowInstructorHighlights, uns.ShowInstructorNotes, 0
		from UserNoteSettings uns
		where uns.UserId = @sharedStudentId and uns.CourseId = @courseId
	
	if @currentUserType is null or @currentUserType = 1
	begin
		-- students will be shown "show" options only for students that have shared their notes with them
		insert #NoteSettings (StudentId, FirstNameSharer, LastNameSharer, SharedStudentId, FirstNameSharee, LastNameSharee, CourseId, HighlightsEnabled, NotesEnabled)
		select u.UserId, u.FirstName, u.LastName, @sharedStudentId, @firstNameSharee, @lastNameSharee, @courseId, us.HighlightsEnabled, us.NotesEnabled
		from UserShares us
				inner join
			 Users u on us.UserId = u.UserId
	    where us.SharedUserId = @sharedStudentId and us.CourseId = @courseId
	end
	else
	begin
		-- instructors will be shown "show" options for every student in the course
		insert #NoteSettings (StudentId, FirstNameSharer, LastNameSharer, SharedStudentId, FirstNameSharee, LastNameSharee, CourseId, HighlightsEnabled, NotesEnabled)
			select u.UserId, u.FirstName, u.LastName, @sharedStudentId, @firstNameSharee, @lastNameSharee, @courseId, ISNULL(us.HighlightsEnabled, 0), ISNULL(us.NotesEnabled, 0)
			from UserNoteSettings uns
					inner join
				 Users u on u.UserId = uns.UserId and uns.CourseId = @courseId
					left join
				 UserShares us on us.CourseId = @courseId and us.UserId = u.UserId and us.SharedUserId = @studentId
					left join
				 UserTypes ut on ut.CourseId = uns.CourseId and uns.UserId = ut.UserId					
			where (ut.[Type] is null or ut.[Type] = 1)
	end
	

	if @itemId is not null and @reviewId is null and @enrollmentId is null
	begin
		-- count of all "item" notes each user has
		insert #ItemCount (UserId, ItemCount)
			select a.UserId, COUNT(a.NoteId) as ItemCount
			from (
				select n.NoteId, n.UserId
				from ItemHighlights ih
				inner join Highlights h on ih.HighlightId = h.HighlightId
				inner join HighlightNotes hn on ih.HighlightId = hn.HighlightId and ih.ItemId = @itemId and ih.CourseId = @courseId
				inner join Note n on n.NoteId = hn.NoteId
				inner join #NoteSettings ns on n.UserId = ns.StudentId
				where n.[Status] <> 2 and (h.[Public] = 1 or ns.StudentId = @studentId)
				union
				select n.NoteId, n.UserId
				from ItemNotes i
				inner join Note n on i.NoteId = n.NoteId and i.ItemId = @itemId and i.CourseId = @courseId
				inner join #NoteSettings ns on n.UserId = ns.StudentId
				where n.[Status] <> 2 and (n.[Public] = 1 or ns.StudentId = @studentId)
			) a
			group by a.UserId
	end
	
	if @itemId is not null and @reviewId is not null and @enrollmentId is not null 
	begin
		-- count of all "item" notes each user has
		insert #ItemCount (UserId, ItemCount)
			select a.UserId, COUNT(a.NoteId) as ItemCount
			from (
				select n.NoteId, n.UserId
				from ReviewHighlights ih
				inner join Highlights h on ih.HighlightId = h.HighlightId
				inner join HighlightNotes hn on ih.HighlightId = hn.HighlightId and ih.ItemId = @itemId and ih.EnrollmentId = @enrollmentId
				inner join Note n on n.NoteId = hn.NoteId
				inner join #NoteSettings ns on n.UserId = ns.StudentId
				where n.[Status] <> 2 and (h.[Public] = 1 or ns.StudentId = @studentId)
				union
				select n.NoteId, n.UserId
				from ItemNotes i
				inner join Note n on i.NoteId = n.NoteId and i.ItemId = @itemId and i.CourseId = @courseId
				inner join #NoteSettings ns on n.UserId = ns.StudentId
				where n.[Status] <> 2 and (n.[Public] = 1 or ns.StudentId = @studentId)
			) a
			group by a.UserId
	end
	
	update #NoteSettings
	set ItemCount = ic.ItemCount
	from #ItemCount ic 
	where ic.UserId = StudentId
	
	update #NoteSettings
	set UserType = ic.Type
	from UserTypes ic 
	where ic.UserId = StudentId

	if @itemId is not null and @reviewId is null and @enrollmentId is null
	begin 
		-- all instructors for the course
		insert into #CousreInstructers
		select UserId
		from UserTypes 
		where CourseId = @courseId and [Type] = 0
		
		-- update the row for instructors (which has student id as 0) with the count of notes for all the instructors for this item
		update #NoteSettings set ItemCount =
		(select  COUNT(a.NoteId)
			from (
				select n.NoteId, n.UserId
				from ItemHighlights ih
				inner join Highlights h on ih.HighlightId = h.HighlightId
				inner join HighlightNotes hn on ih.HighlightId = hn.HighlightId and ih.ItemId = @itemId and ih.CourseId = @courseId
				inner join Note n on n.NoteId = hn.NoteId
				inner join #CousreInstructers ci on n.UserId =  ci.UserId
				where n.[Status] <> 2 and h.[Public] = 1
				union
				select n.NoteId, n.UserId
				from ItemNotes i
				inner join Note n on i.NoteId = n.NoteId and i.ItemId = @itemId and i.CourseId = @courseId
				inner join #CousreInstructers ci on n.UserId =  ci.UserId
				where n.[Status] <> 2 and n.[Public] = 1 
			) a
			group by a.UserId)
		where StudentId=0;	
	end
	

	--update #NoteSettings set ItemCount = (select SUM(ItemCount) from #NoteSettings where UserType=0) where StudentId=0;
	
	select * from #NoteSettings
	
	
	drop table #NoteSettings
	drop table #ItemCount
	drop table #CousreInstructers
	
END

GO

