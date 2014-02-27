-- Adds the Changes column to the [QuestionLog] table for Logging Question XML changes

if not exists (select * from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = 'QuestionLogs' and COLUMN_NAME = 'Changes')
begin
	alter table QuestionLogs add Changes xml null
end

go

