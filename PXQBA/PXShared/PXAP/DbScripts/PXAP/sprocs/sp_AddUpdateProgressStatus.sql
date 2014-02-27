SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id= OBJECT_ID(N'[dbo].[sp_AddUpdateProgressStatus]') AND
OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[sp_AddUpdateProgressStatus]
END
GO

CREATE PROCEDURE [dbo].[sp_AddUpdateProgressStatus]
	@ProcessId bigint,
	@Percentage int
AS
BEGIN

	DECLARE @status varchar(15)
	
	IF (@Percentage < 100)
		SET @status = 'processing'
	ELSE
		SET @status = 'complete'

	IF @ProcessId = 0
	BEGIN
		--Create a new entry
		insert into ProgressStatus(percent_complete, status)
		values (@Percentage, @status)
		
		select @ProcessId = SCOPE_IDENTITY()
		
	END
	ELSE
	BEGIN
		--Update the existing entry
		update ProgressStatus
		set status = @status,
			percent_complete = @Percentage
		where id = @ProcessId
	END
	
	--return the row
	select @ProcessId as ProcessId, @Percentage as Percentage, @status as Message

	
END
GO