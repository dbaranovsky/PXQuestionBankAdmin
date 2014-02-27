-- saves an existing taxonomy scope or adds a new one

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[SaveTaxonomy]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[SaveTaxonomy]
END
GO

CREATE PROCEDURE [dbo].[SaveTaxonomy]
(
	@title nvarchar(512),
	@externalId nvarchar(256)
)
AS
BEGIN	
	if not exists (select Id from Taxonomy where ExternalId = @externalId)
	begin
		insert Taxonomy (Title, ExternalId) values (@title, @externalId)
		select @@IDENTITY as Id, @externalId as ExternalId, @title as Title
	end	
	else
	begin
		select Id, ExternalId, Title from Taxonomy where ExternalId = @externalId
	end
END

GO