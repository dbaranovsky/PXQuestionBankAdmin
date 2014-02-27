-- stores the relationships between items in a given scope.
-- The scope is loosely related to the taxonomy dimensions in SiteBuilder. e.g. Global, Product, and Item

set ansi_nulls on
go

set quoted_identifier on
go

if object_id('dbo.[TaxonomyItem]','U') is null
begin

	create table [dbo].TaxonomyItem (
		[Id] bigint identity(1,1) not null,
		[ScopeId] bigint not null,
		[ItemId] nvarchar(512) not null,
		[TaxonomyId] bigint not null,
		[ItemTitle] nvarchar(1024) not null,
		constraint [pk_TaxonomyItem] primary key clustered
		(
			[Id] asc
	    ) with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [PRIMARY]
	) on [PRIMARY]	
	
	-- many-to-one with RelationshipScope
	alter table [dbo].TaxonomyItem with check add constraint [fk_TaxonomyItem_TaxonomyScope] foreign key ([ScopeId])
	references [dbo].[TaxonomyScope] ([Id])

	alter table [dbo].TaxonomyItem check constraint [fk_TaxonomyItem_TaxonomyScope]
	
	-- many-to-one with Taxonomy
	alter table [dbo].TaxonomyItem with check add constraint [fk_TaxonomyItem_Taxonomy] foreign key ([TaxonomyId])
	references [dbo].[Taxonomy] ([Id])

	alter table [dbo].TaxonomyItem check constraint [fk_TaxonomyItem_Taxonomy]
end

go