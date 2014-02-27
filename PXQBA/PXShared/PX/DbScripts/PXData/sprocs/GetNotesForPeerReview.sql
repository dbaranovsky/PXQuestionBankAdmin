SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[GetNotesForPeerReview]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetNotesForPeerReview]
END
GO
create Procedure [dbo].[GetNotesForPeerReview]
(
	@userId varchar(50),
	@enrollmentIds varchar(max)
)
as
begin
	select * into #Enrollment from CommaDelimitedToTable(@enrollmentIds)
	select hn.HighlightId,n.NoteId,n.UserId,n.[Text],n.[Description],n.[Public],n.[Status],rh.ReviewId,rh.ItemId,rh.EnrollmentId,n.Created,n.Modified
	from Note n
		inner join HighlightNotes hn on hn.NoteId = n.NoteId
		inner join ReviewHighlights rh on hn.HighlightId = rh.HighlightId
		inner join #Enrollment e on e.value = rh.EnrollmentId
	where n.UserId = @userId
	union all
	select null as HighlightId,n.NoteId,n.UserId,n.[Text],n.[Description],n.[Public],n.[Status],rn.ReviewId,rn.ItemId,rn.EnrollmentId,n.Created,n.Modified
	from Note n
		inner join ReviewNotes rn on rn.NoteId = n.NoteId
		inner join #Enrollment e on e.value = rn.EnrollmentId
	where n.UserId = @userId
	drop table #Enrollment
end
go