
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[SelectPresentationAlias]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[SelectPresentationAlias]	
END
GO

CREATE PROCEDURE [dbo].[SelectPresentationAlias]
(
	@SearchType varchar(100),
	@AliasHash nvarchar(4000) = '',
	@CourseId bigint = 0
)
AS
BEGIN	
	IF(LOWER(@SearchType) = 'aliashash')
	BEGIN
		SELECT [AliasHash]
			  ,[Alias]
			  ,[HomeUrl]
			  ,[CourseId]
		  FROM [dbo].[PresentationAlias] WHERE [AliasHash] = LOWER(@AliasHash)
	END
	ELSE
	BEGIN
		SELECT [AliasHash]
			  ,[Alias]
			  ,[HomeUrl]
			  ,[CourseId]
		  FROM [dbo].[PresentationAlias] WHERE [CourseId] = @CourseId		
	END

END

GO
