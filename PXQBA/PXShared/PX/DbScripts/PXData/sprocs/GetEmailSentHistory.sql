
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[GetEmailSentHistory]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetEmailSentHistory]
END
GO

CREATE PROCEDURE [dbo].[GetEmailSentHistory]
	@TrackingEmailId BIGINT,
	@Status CHAR(1) = 'P'
AS
BEGIN
	SET NOCOUNT ON;

    SELECT TrackingEmailId, EmailAddress FROM [dbo].[EmailSendHistory]
		WHERE TrackingEmailId = @TrackingEmailId AND [Status] = @Status
END

GO
