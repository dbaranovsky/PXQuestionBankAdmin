
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[AddNoteToTopNote]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[AddNoteToTopNote]
END
GO

CREATE PROCEDURE [dbo].[AddNoteToTopNote]
(
	@topNoteId uniqueidentifier,
	@noteText nvarchar(max),
	@description nvarchar(300),
	@noteType int,
	@itemId nvarchar(50),
	@reviewId nvarchar(50),
	@enrollmentId nvarchar(50),
	@public bit = 0,
	@userId nvarchar(50),
	@firstName nvarchar(50),
	@lastName nvarchar(50),
	@courseId nvarchar(50),
	@noteId uniqueidentifier OUT
)
As
Begin
	
	Set @noteID = NEWID()
	EXEC [dbo].[AddNote] @noteId,@noteText,@description,@noteType,1,@itemId,@reviewId,@enrollmentId,@public,@userId,@firstName,@lastName, @courseId
	EXEC [dbo].[SetTopNoteRelation] @topNoteId,@noteId
	
End
GO
