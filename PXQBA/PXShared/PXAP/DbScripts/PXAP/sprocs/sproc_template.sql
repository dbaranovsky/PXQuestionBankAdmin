SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[sp_storedprocname]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[sp_storedprocname]
END
GO
CREATE PROCEDURE [dbo].[sp_storedprocname]
	@arg1 varchar(1000)
AS
BEGIN
		-- stored proc code goes here
		
END

GO
