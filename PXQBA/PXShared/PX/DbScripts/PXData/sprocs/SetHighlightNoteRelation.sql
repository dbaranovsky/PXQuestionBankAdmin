
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[SetHighlightNoteRelation]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[SetHighlightNoteRelation]
END
GO

Create Procedure [dbo].[SetHighlightNoteRelation]
(
	@highlightId uniqueidentifier,
	@noteId uniqueidentifier
)
As
Begin
	Insert Into HighlightNotes(HighlightId,NoteId)
	Values(@highlightId,@noteId)
End
GO
