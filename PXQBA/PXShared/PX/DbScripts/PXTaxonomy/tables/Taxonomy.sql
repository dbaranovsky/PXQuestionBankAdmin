-- stores each unique taxonomy along with a friendly name for it

set ansi_nulls on
go

set quoted_identifier on
go

if object_id('dbo.[Taxonomy]','U') is null
begin

	create table [dbo].Taxonomy (
		[Id] bigint identity(1,1) not null,
		[ExternalId] nvarchar(256) not null,
		[Title] nvarchar(512) not null,
		constraint [pk_Taxonomy] primary key clustered
		(
			[Id] asc
	    ) with (pad_index = off, statistics_norecompute = off, ignore_dup_key = off, allow_row_locks = on, allow_page_locks = on) on [PRIMARY]
	) on [PRIMARY]	
	
end

go