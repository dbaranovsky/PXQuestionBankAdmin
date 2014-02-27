SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CommaDelimitedToTable]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[CommaDelimitedToTable]
GO
create function [dbo].[CommaDelimitedToTable]
(
	@list varchar(max)
)
returns 
@parsedTable table
(
	value nvarchar(50)
)
as
begin
	declare @value nvarchar(50), @pos int
	set @list = ltrim(rtrim(@list))+ ','
	set @pos = charindex(',', @list, 1)

	if replace(@list, ',', '') <> ''
	begin
		while @pos > 0
		begin
			set @value = ltrim(rtrim(left(@list, @pos - 1)))
			if @value <> ''
			begin
				insert into @parsedTable (value) 
				values (@value)
			end
			set @list = right(@list, len(@list) - @pos)
			set @pos = charindex(',', @list, 1)

		end
	end	
	return
end
GO
--select * from CommaDelimitedToTable ('111-237,444-487,356')