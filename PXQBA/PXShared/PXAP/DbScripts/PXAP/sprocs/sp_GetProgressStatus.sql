SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id= OBJECT_ID(N'[dbo].[sp_GetProgressStatus]') AND
OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[sp_GetProgressStatus]
END
GO

CREATE PROCEDURE [dbo].[sp_GetProgressStatus]
	@ProcessId bigint
AS
BEGIN

declare @counter int = 0
	select @counter = COUNT(1) from dbo.ProgressStatus where id = @ProcessId
	
	if (@counter = 0)
	begin
		select -1 as ProcessId, -1 as Percentage, 'Unknown process' as Message
		return
	end
	
	select @ProcessId as ProcessId,
		   percent_complete as Percentage,
		   status as Message
	from dbo.ProgressStatus
	where id=@ProcessId

END
GO