-- stores scopes used to group relationships along with an id that identifies them in an external system

set ansi_nulls on
go

set quoted_identifier on
go

if object_id('dbo.[TaxonomyScope]','U') is null
begin

	create table [dbo].TaxonomyScope (
		[Id] bigint identity(1,1) not null,
		[ExternalId] nvarchar(256) not null
		constraint [pk_TaxonomyScope] primary key clustered
		(
			[Id] asc
	    ) with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [PRIMARY]
	) on [PRIMARY]	
	
end

go