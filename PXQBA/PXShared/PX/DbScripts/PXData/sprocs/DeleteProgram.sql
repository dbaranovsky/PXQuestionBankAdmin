SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteProgram]') AND type in (N'P', N'PC'))
BEGIN
	DROP PROCEDURE [dbo].[DeleteProgram]
END
GO

-- =============================================
-- Author:		Prasanna
-- Create date: 04/10/2012
-- Description:	Delete "[Programs]" data
-- =============================================
CREATE PROCEDURE [dbo].[DeleteProgram]
	@Program_id BIGINT,
	@Program_manager_id BIGINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;
		
	-- Insert Program
	DELETE FROM [dbo].[Programs] WHERE
		[Program_id] = ISNULL(@Program_id, [Program_id]) AND
		[Program_manager_id] = ISNULL(@Program_manager_id, [Program_manager_id])
		
END
GO