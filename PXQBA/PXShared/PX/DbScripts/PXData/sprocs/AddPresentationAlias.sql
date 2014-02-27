
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[AddPresentationAlias]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[AddPresentationAlias]	
END
GO

CREATE PROCEDURE [dbo].[AddPresentationAlias]
(
@AliasHash nvarchar(4000),
@Alias nvarchar(4000),
@HomeUrl nvarchar(4000),
@CourseId bigint
)
AS
BEGIN	
	INSERT INTO [dbo].[PresentationAlias]
           ([AliasHash]
           ,[Alias]
           ,[HomeUrl]
           ,[CourseId])
     VALUES
           (LOWER(@AliasHash)
           ,LOWER(@Alias)
           ,LOWER(@HomeUrl)
           ,@CourseId)
END

GO
