
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[SetTopNoteRelation]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[SetTopNoteRelation]
END
GO

CREATE PROCEDURE [dbo].[SetTopNoteRelation]
(
	@topNoteId uniqueidentifier,
	@noteId uniqueidentifier
)
AS
BEGIN
	INSERT INTO TopNotes(TopNoteId,NoteId)
	VALUES(@topNoteId,@noteId)
End
GO
