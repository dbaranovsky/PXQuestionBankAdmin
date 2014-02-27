
/****** Object:  StoredProcedure [dbo].[VideoHasNotes]    Script Date: 10/26/2012 10:44:05 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[VideoHasNotes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[VideoHasNotes]
GO

/****** Object:  StoredProcedure [dbo].[VideoHasNotes]    Script Date: 10/26/2012 10:44:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[VideoHasNotes]
(	
	@assignmentId NVARCHAR(50),
	@videoId NVARCHAR(50)
)
AS
BEGIN
	IF EXISTS(SELECT Id FROM dbo.VideoNotes WHERE ItemId = @assignmentId AND VideoId = @videoId AND IsDeleted = 0)
		RETURN 1;
	ELSE
		RETURN 0;
END


GO


