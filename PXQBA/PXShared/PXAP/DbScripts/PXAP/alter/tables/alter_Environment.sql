

-- Adds the color column if it doesn't already exists

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'Environment' and COLUMN_NAME = 'COLUMN_NAME')
begin
	alter table Environment add BrainHoneyDocs nvarchar(255)
end

go 