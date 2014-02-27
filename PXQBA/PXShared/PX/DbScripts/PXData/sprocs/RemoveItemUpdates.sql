
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[RemoveItemUpdates]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[RemoveItemUpdates]
END
GO

CREATE PROCEDURE [dbo].[RemoveItemUpdates]
(
	@dateBefore datetime
)
AS
BEGIN
	DELETE FROM dbo.[ItemUpdates]
	WHERE [DateUpdated] < @dateBefore
END

GO
