SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateProgram]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[UpdateProgram]
END
GO

-- =============================================
-- Author:		Prasanna
-- Create date: 04/10/2012
-- Description:	Update "[Programs]" data
-- =============================================
CREATE PROCEDURE [dbo].[UpdateProgram]
	@Program_id BIGINT,
	@Dashboard_id BIGINT,
	@Program_manager_id BIGINT,
	@Program_manager_ref_id BIGINT,
	@Program_manager_domain_id BIGINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;
	
	DECLARE @Matching_id BIGINT
	
	--Get the matching program id
	IF (@Program_id IS NOT NULL)
	BEGIN
		SELECT @Matching_id = @Program_id
	END
	ELSE IF	(@Program_manager_id IS NOT NULL)
	BEGIN
		SELECT @Matching_id = MAX(Program_id) FROM [dbo].[Programs] WHERE 
			[Program_manager_id] = @Program_manager_id
	END
	ELSE IF (@Program_manager_ref_id IS NOT NULL AND @Program_manager_domain_id IS NOT NULL)	
	BEGIN
		SELECT @Matching_id = MAX(Program_id) FROM [dbo].[Programs] WHERE 
			[Program_manager_ref_id] = @Program_manager_ref_id AND
			[Program_manager_domain_id] = @Program_manager_domain_id
	END
		
		
	-- Insert Program
	UPDATE [dbo].[Programs] SET 
		[Dashboard_id] = @Dashboard_id, 
		[Program_manager_id] = ISNULL(@Program_manager_id, [Program_manager_id]),  
		[Program_manager_ref_id] = ISNULL(@Program_manager_ref_id, [Program_manager_ref_id]), 
		[Program_manager_domain_id] = ISNULL(@Program_manager_domain_id, [Program_manager_domain_id])
		WHERE
		[Program_id] = @Matching_id
		
END
GO