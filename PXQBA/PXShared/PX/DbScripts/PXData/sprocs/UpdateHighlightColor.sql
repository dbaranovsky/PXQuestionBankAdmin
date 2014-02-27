
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[UpdateHighlightColor]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[UpdateHighlightColor]
END
GO

CREATE Procedure [dbo].[UpdateHighlightColor]
(
	@highlightId uniqueidentifier,
	@color nvarchar(50)
)
As
Begin
	Update Highlights 
	Set 
		[color] = @color,
		Modified = GETDATE()
	Where HighlightId = @highlightId
End
GO
