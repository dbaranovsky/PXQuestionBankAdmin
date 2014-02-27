SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[SP_PXAP_SelectMessage]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[SP_PXAP_SelectMessage]
END
GO

CREATE PROCEDURE dbo.SP_PXAP_SelectMessage
(@LogID INT)
AS
BEGIN

SELECT [FormattedMessage] FROM dbo.[Log] WHERE LogID = @LogID

END


