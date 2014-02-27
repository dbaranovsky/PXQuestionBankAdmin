
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[InsertDashboardData2]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[InsertDashboardData2]
END
GO
CREATE PROCEDURE [dbo].[InsertDashboardData2]
	@User_id BIGINT,
	@User_Ref_Id BIGINT,
	@Domain_Id BIGINT,
	@Dashboard_Id BIGINT,
	@Course_Type VARCHAR(150),
	@Dashboard_Type VARCHAR(150),
	@ProductCourse_Id VARCHAR(100)
	AS

	BEGIN
		-- SET NOCOUNT ON added to prevent extra result sets from
		SET NOCOUNT ON;
		DECLARE @Dashboard_t VARCHAR(150)

		SELECT @Dashboard_t = [Dashboard_Type] FROM [dbo].[Dashboard_Types] WHERE @Dashboard_Type = [Dashboard_Type]
		
			IF NOT EXISTS(SELECT * FROM [dbo].[Dashboard] WHERE 
			[User_Ref_Id] = @User_ref_id AND
			[Domain_Id] = @Domain_id AND
			[Course_type] = @Course_type AND
			[Dashboard_type] = @Dashboard_type)	
		
	Begin
	Insert Into dbo.Dashboard([User_id],User_Ref_Id,Domain_Id,Dashboard_Id, Course_type, Dashboard_type, ProductCourse_Id)
		Values(@User_id,@User_Ref_Id,@Domain_Id,@Dashboard_Id, @Course_Type, @Dashboard_t, @ProductCourse_Id)
		
	End
		
			END

GO


