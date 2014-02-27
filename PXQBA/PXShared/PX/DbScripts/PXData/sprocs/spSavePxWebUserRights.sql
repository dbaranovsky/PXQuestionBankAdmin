/****** Object:  StoredProcedure [dbo].[SavePxWebUserRights]    Script Date: 09/18/2012 10:57:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SavePxWebUserRights]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SavePxWebUserRights]
GO


Create PROCEDURE [dbo].[SavePxWebUserRights]

@CourseId nvarchar(50) , 
@UserId nvarchar(50) , 
@PxWebRightId int  ,
@Rights bigint  
	
AS
BEGIN
	SET NOCOUNT ON
	IF EXISTS(SELECT Rights  FROM dbo.PxWebUserRights 
			  WHERE UserId=@UserId 
			  AND CourseId = @CourseId
			  AND PxWebRightId = @PxWebRightId)
		 
		 UPDATE dbo.PxWebUserRights 
		 SET Rights = @Rights
		 WHERE UserId=@UserId 
		 AND CourseId = @CourseId
		 AND PxWebRightId = @PxWebRightId
	
	ELSE
		 INSERT INTO dbo.PxWebUserRights(CourseId, UserID, PxWebRightId, Rights)
								VALUES (@CourseId, @UserId,@PxWebRightId,@Rights)
END

GO


