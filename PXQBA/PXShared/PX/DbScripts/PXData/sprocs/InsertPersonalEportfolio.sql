SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertPersonalEportfolio]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[InsertPersonalEportfolio]
END
GO

-- =============================================
-- Author:		Kumar
-- Create date: 04/10/2012
-- Description:	Insert data into "[PersonalEportfolioDashboard]" table
-- =============================================
CREATE PROCEDURE [dbo].[InsertPersonalEportfolio]
	@User_id BIGINT,
	@User_Ref_Id BIGINT,
	@Domain_Id BIGINT,
	@Dashboard_Id BIGINT
	AS
	BEGIN
		-- SET NOCOUNT ON added to prevent extra result sets from
		SET NOCOUNT ON;
		
		Insert Into dbo.PersonalEportfolioDashboard([User_id],User_Ref_Id,Domain_Id,Dashboard_Id)
		Values(@User_id,@User_Ref_Id,@Domain_Id,@Dashboard_Id)
	END
GO
