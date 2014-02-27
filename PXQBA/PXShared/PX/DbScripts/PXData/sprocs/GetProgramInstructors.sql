SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProgramInstructors]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetProgramInstructors]
GO

-- =============================================
-- Author:		Prasanna
-- Create date: 05/10/2012
-- Description:	get the list of instructors for a program
-- =============================================
CREATE PROCEDURE [dbo].[GetProgramInstructors] 
	(@Program_manager_id BIGINT, @User_domain_id BIGINT)
AS
BEGIN
	SELECT UP.[User_id], UP.User_dashboard_id FROM 
		[dbo].[Programs] P INNER JOIN [dbo].[UserPrograms] UP
		ON P.Program_id = UP.Program_id 
		WHERE 
		P.Program_manager_id = @Program_manager_id AND 
		UP.User_domain_id = ISNULL(@User_domain_id, UP.User_domain_id)
END
GO


