/****** Object:  StoredProcedure [dbo].[SaveAllPxWebUserRights]    Script Date: 09/18/2012 11:21:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveAllPxWebUserRights]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SaveAllPxWebUserRights]
GO

Create PROCEDURE [dbo].[SaveAllPxWebUserRights]
    @CourseId nvarchar(50),
	@UserId nvarchar(50),
	@AllAdminRights nvarchar(Max)
AS
	BEGIN

		SET NOCOUNT ON
	    
		DECLARE @tblKeyValuePair TABLE 
				 (  PxWebRightId VARCHAR(MAX) ,    
					Rights VARCHAR(MAX) 
				 ) 
				
		DECLARE @Delimiter varchar(5)
		DECLARE @KeyValueDelimiter varchar(5)

		SET @AllAdminRights=replace(replace(@AllAdminRights,',[', ''),'[', '')
		SET @Delimiter = ']'
		SET @KeyValueDelimiter = ','

		INSERT INTO @tblKeyValuePair
			SELECT DataKey, DataValue FROM dbo.SplitKeyValuePairsList(@DELIMITER, @KeyValueDelimiter, @AllAdminRights)
	     
		DELETE dbo.PxWebUserRights
			WHERE UserId = @UserId
			AND CourseId = @CourseId
			AND   PxWebRightId IN (SELECT PxWebRightId FROM @tblKeyValuePair)												  															  															  

		INSERT INTO PxWebUserRights (CourseId, UserId, PxWebRightId, Rights)
			SELECT @CourseId, @UserId , PxWebRightId, Rights FROM @tblKeyValuePair

		RETURN
	END

GO


