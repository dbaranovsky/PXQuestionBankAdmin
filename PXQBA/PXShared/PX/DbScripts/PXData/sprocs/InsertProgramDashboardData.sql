/****** Object:  StoredProcedure [dbo].[InsertProgramDashboardData]    Script Date: 11/27/2012 10:23:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[InsertProgramDashboardData]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[InsertProgramDashboardData]
END
GO

CREATE PROCEDURE [dbo].[InsertProgramDashboardData]
	@User_id BIGINT,
	@User_Ref_Id BIGINT,
	@Domain_Id BIGINT,
	@Dashboard_Id BIGINT,
	@Course_Type VARCHAR(150),
	@Dashboard_Type VARCHAR(150),
	@Product_type VARCHAR(100),
	@ProductCourse_Id VARCHAR(100)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;

DECLARE @programid BIGINT
DECLARE @Dashboard_t VARCHAR(100)

--Get the program ID
	IF @Domain_id is not null 
	Begin
			SELECT TOP 1 @programId = Program_id
			FROM [dbo].[GenericPrograms] 
			WHERE
			[Domain_id] = @Domain_id
	End	
	
		SELECT @Dashboard_t = [Dashboard_Type] FROM [dbo].[Dashboard_Types] WHERE @Dashboard_Type = [Dashboard_Type]

	IF NOT EXISTS(SELECT * FROM [dbo].[Dashboard] WHERE 			
			[Domain_Id] = @Domain_id AND
			[Course_type] = @Course_type AND
			[Dashboard_type] = @Dashboard_type)	
		
	Begin
		
		Insert Into dbo.Dashboard([User_id],User_Ref_Id,Domain_Id,Dashboard_Id, Course_type, Dashboard_type, ProductCourse_Id)
		Values(@User_id,@User_Ref_Id,@Domain_Id,@Dashboard_Id, @Course_Type, @Dashboard_t, @ProductCourse_Id)
		
		
		END
		
		Insert Into dbo.ProgramDashboard(Program_id,Dashboard_id,Product_type)
		Values(@programId,@Dashboard_Id,@Product_type)
END

GO


