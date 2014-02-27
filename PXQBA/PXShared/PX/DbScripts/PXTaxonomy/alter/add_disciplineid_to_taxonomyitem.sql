-- Adds the DisciplineID column if it doesn't already exists

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'TaxonomyItem' and COLUMN_NAME = 'DisciplineID')
begin
	alter table TaxonomyItem add DisciplineID nvarchar(50)
end

go