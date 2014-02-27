-- Adds the Status column to the Note table to track whether a note is active or deleted

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'Note' and COLUMN_NAME = 'Status')
begin
	alter table Note add [Status] int default 0 not null
end

go