SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserProgram]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[GetUserProgram]
END
GO

-- =============================================
-- Author:		Prasanna
-- Create date: 04/10/2012
-- Description:	select from "[UserPrograms]" table
-- =============================================
CREATE PROCEDURE [dbo].[GetUserProgram]
	@User_id BIGINT,
	@User_ref_id BIGINT,
	@User_domain_id BIGINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;
	
	DECLARE @Matching_id BIGINT
	
		--Get the matching user id
	IF	(@User_id IS NOT NULL)
	BEGIN
		SET @Matching_id = @User_id
	END
	ELSE IF (@User_ref_id IS NOT NULL AND @User_domain_id IS NOT NULL)	
	BEGIN
		SELECT TOP 1 @Matching_id = [User_id] FROM [dbo].[UserPrograms] WHERE 
			[User_ref_id] = @User_ref_id AND
			[User_domain_id] = @User_domain_id
	END
	
    IF (@User_domain_id IS NOT NULL)
    BEGIN 				
		SELECT  
			P.[Program_id], 
			[Dashboard_id], 
			P.[Program_manager_id],
			[User_dashboard_id],
			[User_id]
			FROM 
			[dbo].[Programs] P 
			LEFT JOIN [dbo].[UserPrograms] UP 
				ON P.[Program_id] = UP.[Program_id]
				and UP.User_id = @Matching_id
			WHERE 
			p.Program_manager_domain_id = @User_domain_id
	END
	ELSE
	BEGIN
		SELECT UP.[Program_id], 
		[Dashboard_id], 
		[Program_manager_id],
		[User_dashboard_id],
		[User_id]
		FROM 
		[dbo].[UserPrograms] UP INNER JOIN
		[dbo].[Programs] P ON P.[Program_id] = UP.[Program_id]
		WHERE 
		[User_id] = @Matching_id
	END
END
GO
