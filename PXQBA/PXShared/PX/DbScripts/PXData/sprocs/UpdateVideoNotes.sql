
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[UpdateVideoNote]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[UpdateVideoNote]
END
GO

Create Procedure [dbo].[UpdateVideoNote]
(
	@videoNoteId uniqueidentifier,
	@noteText nvarchar(max)
)
As
Begin
	Update VideoNotes
	Set 
		[Text] = @noteText,
		Modified = GETDATE()
	Where Id = @videoNoteId
End
GO
