-- Adds the Status column to the Note table to track whether a note is active or deleted

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'Note' and COLUMN_NAME = 'Description')
begin
	alter table Note add [Description] nvarchar(300) default 0 not null
end

go