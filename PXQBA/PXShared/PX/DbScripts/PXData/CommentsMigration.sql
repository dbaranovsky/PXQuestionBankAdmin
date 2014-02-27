
Select 
	[highlightId],[highlightText],[dateAdded],[userId],[itemId],[courseId],[userAccessLevel],
	[isActive],[isShared],[highlightType],[secondaryId],[isGeneral],[description],[reviewId]
Into #tempHighlights
From 
	PxComments.dbo.Highlight

Select 
	[commentId],(Select FK_HighlightId From PxComments.dbo.HighlightComment Where FK_CommentId = commentId) As HighlightId
	,[userId],[commentText],[dateAdded],[userFirstName],[userLastName],[userAccessLevel]
Into #tempComments
From PxComments.dbo.Comment

Declare @highlightId int,@highlightText varchar(max),@hdateAdded datetime,@huserId int,@itemId varchar(250),
	@courseId int,@isActive bit,@isShared bit,@highlightType int,@secondaryId varchar(250),@isGeneral bit,
	@description varchar(300),@reviewId varchar(250),@commentId int,@cuserId int,@commentText varchar(160),
	@cdateAdded datetime,@userFirstName varchar(50),@userLastName varchar(50)
	

While (Select COUNT(*) From #tempHighlights) > 0
Begin
	Select Top 1 @highlightId = HighlightId,
		@highlightText = [highlightText],@hdateAdded = [dateAdded],@huserId = [userId],@itemId = [itemId],@courseId = [courseId],
		@isActive = [isActive],@isShared = [isShared],@highlightType = [highlightType],@secondaryId = [secondaryId],@isGeneral = [isGeneral],
		@description = [description],@reviewId = [reviewId]
	From #tempHighlights
	
	IF NOT EXISTS(SELECT UserId From Users Where UserId = @huserId)
	BEGIN
		Declare @hFirstName varchar(50), @hLastName varchar(50)
		Set @hFirstName = ''
		Set @hLastName = ''
		Select @hFirstName = userFirstName,@hLastName = userLastName From #tempComments Where userId = @huserId
		INSERT INTO Users(UserId,FirstName,LastName) Values(@huserId,@hFirstName,@hLastName)
	END
	
	Declare @HGuid uniqueidentifier
	Set @HGuid = NewId()
	
	Insert Into Highlights(HighlightId,Text,[Description],[Public],Status,UserId,Created,Modified)
	Values(@HGuid,@highlightText,@description,@isShared,CASE @isActive When 1 then 0 else 2 END,@huserId,@hdateAdded,@hdateAdded)
	
	If @isGeneral = 0 OR @highlightText <> ''
	BEGIN
		IF @highlightType = 1 -- Regular E book highlight
		Begin
			Insert Into ItemHighlights(HighlightId,ItemId,CourseId)
			Values(@HGuid,@itemId,@courseId)
		End
		ELSE IF @highlightType = 2 -- Writing Assignment Submission highlight
		Begin
			Insert Into SubmissionHighlights(HighlightId,ItemId,EnrollmentId)
			Values(@HGuid,@itemId,@secondaryId)
		End
		ELSE IF @highlightType = 3 AND @reviewId Is Not Null -- Peer Review highlight
		Begin
			Insert Into ReviewHighlights(HighlightId,ReviewId,EnrollmentId,ItemId)
			Values(@HGuid,@reviewId,@secondaryId,@itemId)
		End
	END
		
	While(Select COUNT(*) From #tempComments Where HighlightId = @highlightId) > 0
	BEGIN
		Select Top 1
		@commentId = [commentId],@cuserId = [userId],@commentText = [commentText],@cdateAdded = [dateAdded],
		@userFirstName = [userFirstName],@userLastName = [userLastName]
		From #tempComments
		Where HighlightId = @highlightId
	
		IF NOT EXISTS(SELECT UserId From Users Where UserId = @cuserId)
		BEGIN
			INSERT INTO Users(UserId,FirstName,LastName) Values(@huserId,@userFirstName,@userLastName)
		END
		
		Declare @NGuid uniqueidentifier
		Set @NGuid = NewId()
		
		Insert Into Note(NoteId,UserId,Text,[Public],Created,Modified)
		Values(@NGuid,@cuserId,@commentText,CASE @isActive When 1 then 0 else 2 END,@cdateAdded,@cdateAdded)
	
		If @isGeneral = 1 OR @highlightText = ''
		BEGIN
			IF @highlightType = 1 -- Regular E book highlight
			Begin
				Insert Into ItemNotes(ItemId,NoteId,CourseId)
				Values(@itemId,@NGuid,@courseId)
			End
			ELSE IF @highlightType = 2 -- Writing Assignment Submission highlight
			Begin
				Insert Into SubmissionNotes(NoteId,ItemId,EnrollmentId)
				Values(@NGuid,@itemId,@secondaryId)
			End
			ELSE IF @highlightType = 3 AND @reviewId Is Not Null -- Peer Review highlight
			Begin
				Insert Into ReviewNotes(NoteId,ReviewId,EnrollmentId,ItemId)
				Values(@NGuid,@reviewId,@secondaryId,@itemId)
			End
		END
		ELSE
		BEGIN
			Insert Into HighlightNotes(HighlightId,NoteId)
			Values(@HGuid,@NGuid)
		END
	Delete From #tempComments Where commentId = @commentId
	END
	
	Delete From #tempHighlights Where HighlightId = @highlightId
End

drop table #tempHighlights
drop table #tempComments