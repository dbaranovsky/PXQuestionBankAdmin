
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[GetDashboardId2]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetDashboardId2]
END

GO
CREATE PROCEDURE [dbo].[GetDashboardId2]
	@ProductCourse_Id VARCHAR(150),
	@Dashboard_type VARCHAR(150),
	@Ref_id VARCHAR(150)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;
	
IF (@ProductCourse_Id IS NOT NULL AND @Dashboard_type IS NOT NULL AND @Ref_id IS NOT NULL)	
	BEGIN
		SELECT *
		FROM [dbo].[Dashboard] 
		WHERE 
			[ProductCourse_Id] = @ProductCourse_Id	AND		
			[Dashboard_type] = @Dashboard_type AND 
			[User_Ref_Id] = @Ref_id
	END
	

END
GO


