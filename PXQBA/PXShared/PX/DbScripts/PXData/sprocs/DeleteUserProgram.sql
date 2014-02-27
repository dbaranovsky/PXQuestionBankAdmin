SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteUserProgram]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[DeleteUserProgram]
END
GO

-- =============================================
-- Author:		Prasanna
-- Create date: 04/10/2012
-- Description:	Delete "[UserPrograms]" data
-- =============================================
CREATE PROCEDURE [dbo].[DeleteUserProgram]
	@Program_id BIGINT,
	@User_id BIGINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;
		
	-- Insert Program
	DELETE FROM [dbo].[UserPrograms] WHERE
		[Program_id] = ISNULL(@Program_id, [Program_id]) AND
		[User_id] = ISNULL(@User_id, [User_id])
		
END
GO