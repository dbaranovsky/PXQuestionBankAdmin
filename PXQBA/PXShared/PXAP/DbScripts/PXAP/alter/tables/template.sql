-- Adds the color column if it doesn't already exists

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'TABLE_NAME' and COLUMN_NAME = 'COLUMN_NAME')
begin
	--alter table Highlights add Color nvarchar(50)
end

go