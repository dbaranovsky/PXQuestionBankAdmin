/****** Object:  StoredProcedure [dbo].[InsertProgram2]    Script Date: 11/27/2012 12:12:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InsertProgram2]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[InsertProgram2]
END
GO

CREATE PROCEDURE [dbo].[InsertProgram2]
	@Domain_id BIGINT
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

	IF @programId is null 
	Begin
		Insert Into GenericPrograms(Domain_id)
		Values(@Domain_id)
			
	END
END
