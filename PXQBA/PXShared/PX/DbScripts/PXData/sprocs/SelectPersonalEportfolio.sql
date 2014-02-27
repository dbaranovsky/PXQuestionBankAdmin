SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectPersonalEportfolio]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[SelectPersonalEportfolio]
END
GO

-- =============================================
-- Author:		Kumar
-- Create date: 04/10/2012
-- Description:	Select data from "[PersonalEportfolioDashboard]" table
-- =============================================
CREATE PROCEDURE [dbo].[SelectPersonalEportfolio]
	@User_Ref_Id BIGINT,
	@Domain_Id BIGINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;
	
	SELECT [User_id],User_Ref_Id,Domain_Id,Dashboard_Id
		FROM 
		dbo.PersonalEportfolioDashboard
		WHERE 
		User_Ref_Id = @User_Ref_Id and
		Domain_Id = @Domain_Id
		
END

GO
