SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[SP_PXAP_ClearLogs]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[SP_PXAP_ClearLogs]
END
GO

CREATE PROCEDURE [dbo].[SP_PXAP_ClearLogs]
(@Severity VARCHAR(200))
AS
BEGIN

	BEGIN TRY
	
		BEGIN transaction
	
		declare @deleted table ( id int )

		delete catlog
		output deleted.[LogID] into @deleted
		from dbo.[CategoryLog] catlog
				inner join
			 dbo.[Log] o on o.[LogID] = catlog.[LogID]
		where o.[Severity] = @Severity
		
		delete o
		from dbo.[Log] o
				inner join
			 @deleted d on d.id = o.[LogID]
			 
		commit
	END TRY
	BEGIN CATCH
		ROLLBACK
	END CATCH
	
END
