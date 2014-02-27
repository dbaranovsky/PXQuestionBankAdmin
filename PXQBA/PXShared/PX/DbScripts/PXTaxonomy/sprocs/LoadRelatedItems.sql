-- loads data for every taxonomy item related to the given item
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[LoadRelatedItems]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[LoadRelatedItems]
END
GO

CREATE PROCEDURE [dbo].[LoadRelatedItems]
(
	@relatedTo nvarchar(512),
	@scope nvarchar(256)
)
as
begin

	select rti.ItemId as ItemId,
	       rti.ItemTitle as ItemTitle, 
	       ti.ItemId as RelatedItemId, 
	       t.ExternalId as TaxonomyId,
	       t.Title as TaxonomyTitle,
	       ts.ExternalId as ScopeId
	from TaxonomyItem ti
			inner join
		 TaxonomyItem rti on ti.TaxonomyId = rti.TaxonomyId and ti.ScopeId = rti.ScopeId
			inner join
		 Taxonomy t on t.Id = rti.TaxonomyId
			inner join
		 TaxonomyScope ts on ts.Id = rti.ScopeId and ts.ExternalId = @scope
	where ti.ItemId = @relatedTo and rti.ItemId <> @relatedTo

end

go