SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateUserProgram]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[UpdateUserProgram]
END
GO

-- =============================================
-- Author:		Prasanna
-- Create date: 04/10/2012
-- Description:	Update "[UpdateUserProgram]" data
-- =============================================
CREATE PROCEDURE [dbo].[UpdateUserProgram]
	@Program_id BIGINT,
	@User_dashboard_id BIGINT,
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
	
	-- Insert Program
	UPDATE [dbo].[UserPrograms] SET 
		[User_dashboard_id] = @User_dashboard_id,
		[User_id] = ISNULL(@User_id, [User_id]),  
		[User_ref_id] = ISNULL(@User_ref_id, [User_ref_id]), 
		[User_domain_id] = ISNULL(@User_domain_id, [User_domain_id])
		WHERE
		[Program_id] = @Program_id AND
		[User_id] = @Matching_id
		
END
GO
