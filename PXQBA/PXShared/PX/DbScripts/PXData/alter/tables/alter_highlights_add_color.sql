-- Adds the color column if it doesn't already exists

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'Highlights' and COLUMN_NAME = 'Color')
begin
	alter table Highlights add Color nvarchar(50)
end

go