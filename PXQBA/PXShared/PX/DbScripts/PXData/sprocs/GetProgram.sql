SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetProgram]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[GetProgram]
END
GO

CREATE PROCEDURE [dbo].[GetProgram]

	@Domain_Id BIGINT

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
	
END

GO


