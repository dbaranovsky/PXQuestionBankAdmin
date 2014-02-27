-- saves an existing taxonomy scope or adds a new one

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[SaveTaxonomyItem]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[SaveTaxonomyItem]
END
GO

CREATE PROCEDURE [dbo].[SaveTaxonomyItem]
(
	@scopeId bigint,
	@taxonomyId bigint,
	@itemId nvarchar(512),
	@itemTitle nvarchar(1024)
)
AS
BEGIN	
	if not exists (select Id from TaxonomyItem where ItemId = @itemId and TaxonomyId = @taxonomyId and ScopeId = @scopeId)
	begin
		insert TaxonomyItem (ItemId, ScopeId, TaxonomyId, ItemTitle) values (@itemId, @scopeId, @taxonomyId, @itemTitle)
		select @@IDENTITY as Id, @scopeId as ScopeId, @itemId as ItemId, @taxonomyId as TaxonomyId, @itemTitle as ItemTitle
	end	
	else
	begin
		select Id, ScopeId, ItemId, TaxonomyId, ItemTitle from TaxonomyItem where ItemId = @itemId and TaxonomyId = @taxonomyId and ScopeId = @scopeId
	end
END

GO