SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[sp_DeleteEnvironment]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[sp_DeleteEnvironment]
END
GO
CREATE PROCEDURE [dbo].[sp_DeleteEnvironment]
	@EnvironmentId int
AS
BEGIN
		
	delete from EnvironmentSources where EnvironmentId = @EnvironmentId
    delete from Environment where EnvironmentId = @EnvironmentId
		
END

GO
-- sp_DeleteEnvironment 80
