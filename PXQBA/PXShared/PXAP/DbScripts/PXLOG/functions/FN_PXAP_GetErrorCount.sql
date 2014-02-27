SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[FN_PXAP_GetErrorCount]') AND OBJECTPROPERTY(id, N'IsScalarFunction') = 1)
BEGIN
	DROP FUNCTION [dbo].[FN_PXAP_GetErrorCount]
END
GO
CREATE FUNCTION [dbo].[FN_PXAP_GetErrorCount]
(
	-- Add the parameters for the function here
	@ErrorCount BIGINT
)
RETURNS VARCHAR(20)
AS
BEGIN

	DECLARE @Result VARCHAR(20)
	DECLARE @s VARCHAR(20)
	DECLARE @length INT
	DECLARE @e FLOAT

	SET @s = CONVERT(VARCHAR(20), @ErrorCount)
	SET @length = LEN(@s)

	--check for millions
	IF(@length >= 7)
	BEGIN
		SET @e = @ErrorCount
		SET @Result = CONVERT(VARCHAR,ROUND(@e / 1000000,1)) + 'm'
	END
	--checks for K's (thousands)
	ELSE IF(@length >= 4 AND @length <= 6)
	BEGIN
		SET @Result = SUBSTRING(@s,1,@length-3) + 'k'
	END
	-- default case
	ELSE
	BEGIN
		SET @Result = @s
	END
	
	RETURN @Result

END

GO