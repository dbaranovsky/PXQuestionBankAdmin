/****** Object:  StoredProcedure [dbo].[GetProgramDashboardId]    Script Date: 11/27/2012 10:20:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[GetProgramDashboardId]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetProgramDashboardId]
END
GO

CREATE PROCEDURE [dbo].[GetProgramDashboardId]
	@Domain_id BIGINT,
	@Course_type VARCHAR(150),
	@ProductCourse VARCHAR(150)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;
	
	--Get the matching user id
DECLARE @programid BIGINT

	IF @Domain_id is not null 
	Begin
			SELECT TOP 1 @programId = Program_id
			FROM [dbo].[GenericPrograms] 
			WHERE
			[Domain_id] = @Domain_id
		
	End

	IF @programId is not null 
	Begin
			SELECT TOP 1 *
			FROM [dbo].[ProgramDashboard] pd
			JOIN [dbo].[Dashboard] d
			ON pd.Dashboard_id = d.Dashboard_Id
			WHERE
			pd.Program_id = @programId AND
			pd.[Product_type] = @Course_type AND
			d.ProductCourse_Id = @ProductCourse
		
	END

END

GO