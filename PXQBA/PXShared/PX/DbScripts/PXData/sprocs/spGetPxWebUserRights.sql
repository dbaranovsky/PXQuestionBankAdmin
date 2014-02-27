/****** Object:  StoredProcedure [dbo].[GetPxWebUserRights]    Script Date: 09/18/2012 10:57:04 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPxWebUserRights]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPxWebUserRights]
GO

Create PROCEDURE [dbo].[GetPxWebUserRights]

@UserId nvarchar(50), 
@CourseId nvarchar(50) 

AS
BEGIN	
	SET NOCOUNT ON;

	SELECT PxWebRightId, Rights FROM dbo.PxWebUserRights 
	WHERE UserId=@UserId
	AND CourseId = @CourseId
    
END
GO


