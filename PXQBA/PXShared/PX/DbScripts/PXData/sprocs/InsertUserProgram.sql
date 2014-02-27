SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertUserProgram]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[InsertUserProgram]
END
GO

-- =============================================
-- Author:		Prasanna
-- Create date: 04/10/2012
-- Description:	Insert data into "[UserPrograms]" table
-- =============================================
CREATE PROCEDURE [dbo].[InsertUserProgram]
	@Program_id BIGINT,
	@User_dashboard_id BIGINT,
	@User_id BIGINT,
	@User_ref_id BIGINT,
	@User_domain_id BIGINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;
	
	-- Insert Program
	INSERT INTO [dbo].[UserPrograms] (
		[Program_id],
		[User_dashboard_id], 
		[User_id], 
		[User_ref_id], 
		[User_domain_id])
		VALUES (
		@Program_id,
		@User_dashboard_id, 
		@User_id, 
		@User_ref_id, 
		@User_domain_id)
		
END
GO
