
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[UpdateVideoNoteStatus]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[UpdateVideoNoteStatus]
END
GO

CREATE Procedure [dbo].[UpdateVideoNoteStatus]
(
	@noteId uniqueidentifier,
	@isDeleted bit
)
As
Begin
	Update VideoNotes 
	Set 
		IsDeleted = @isDeleted 
	    Where Id = @noteId
End
GO
