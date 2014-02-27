-- Adds the version column to the [QuestionLog] table for Logging Question version info

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'QuestionLogs' and COLUMN_NAME = 'version')
begin
	alter table QuestionLogs add version varchar(10) NULL
end

go

