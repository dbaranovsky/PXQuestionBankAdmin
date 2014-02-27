
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[GetPresentationAlias]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetPresentationAlias]
END
GO

--checks the PresentationAlias table if Alias is already present, 
--if so generates a new Alias which has a numeric value appended to the orignial value.
CREATE PROCEDURE [dbo].[GetPresentationAlias]
(@Alias NVARCHAR(4000),
 @CourseId VARCHAR(100))
AS
BEGIN

	DECLARE @count INT	
	DECLARE @ModifiedAlias NVARCHAR(4000)
	DECLARE @CourseAliasExists BIT
	SET @count = 1
	SET @CourseAliasExists = 0
	
	SET @Alias = LOWER(@Alias)
	SET @ModifiedAlias = @Alias

	-- Check if the chosen Alias already exists for the current presentation course	
	IF(EXISTS(SELECT 1 FROM dbo.PresentationAlias WHERE  CourseId = @CourseId AND Alias like @Alias + '%'))
	BEGIN
		SELECT @ModifiedAlias = Alias FROM dbo.PresentationAlias WHERE  CourseId = @CourseId AND Alias like @Alias + '%'
		SET @CourseAliasExists = 1
	END
	
	
	WHILE(@CourseAliasExists = 0  AND EXISTS(SELECT 1 FROM dbo.PresentationAlias WHERE Alias = @ModifiedAlias ))
	BEGIN		
		SET @ModifiedAlias = @Alias + CAST(@count as NVARCHAR)
		SET @count = @count + 1		
	END 
	SELECT @ModifiedAlias ModifiedAlias, @CourseAliasExists CourseAliasExists
	
END
GO
















