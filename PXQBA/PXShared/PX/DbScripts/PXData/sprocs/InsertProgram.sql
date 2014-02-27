SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertProgram]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[InsertProgram]
END
GO

-- =============================================
-- Author:		Prasanna
-- Create date: 04/10/2012
-- Description:	Insert data into "[Programs]" table
-- =============================================
CREATE PROCEDURE [dbo].[InsertProgram]
	@Dashboard_id BIGINT,
	@Program_manager_id BIGINT,
	@Program_manager_ref_id BIGINT,
	@Program_manager_domain_id BIGINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;
	
	-- Insert Program
	INSERT INTO [dbo].[Programs] (
		[Dashboard_id], 
		[Program_manager_id], 
		[Program_manager_ref_id], 
		[Program_manager_domain_id])
		VALUES (
		@Dashboard_id, 
		@Program_manager_id, 
		@Program_manager_ref_id, 
		@Program_manager_domain_id)	
		
	--Get the inserted Program id	
	SELECT @@IDENTITY
END
GO
