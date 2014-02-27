-- saves an existing taxonomy scope or adds a new one

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[SaveTaxonomyScope]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[SaveTaxonomyScope]
END
GO

CREATE PROCEDURE [dbo].[SaveTaxonomyScope]
(
	@externalId nvarchar(256)
)
AS
BEGIN	
	if not exists (select Id from TaxonomyScope where ExternalId = @externalId)
	begin
		insert TaxonomyScope (ExternalId) values (@externalId)
		select @@IDENTITY as Id, @externalId as ExternalId
	end	
	else
	begin
		select Id, ExternalId from TaxonomyScope where ExternalId = @externalId
	end
END

GO