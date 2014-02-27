SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserProgramByDomain]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[GetUserProgramByDomain]
END
GO

-- =============================================
-- Author:		Prasanna
-- Create date: 04/10/2012
-- Description:	select from "[UserPrograms]" table
-- =============================================
CREATE PROCEDURE [dbo].[GetUserProgramByDomain]
	@User_domain_id BIGINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;
	
	SELECT 
		[Program_id], 
		[User_dashboard_id],
		[User_id],
		[User_ref_id], 
		[User_domain_id]
	FROM 
		[dbo].[UserPrograms]
	WHERE 
		[User_domain_id] = @User_domain_id
END
GO
