SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[SP_PXAP_GetLogs]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[SP_PXAP_GetLogs]
END
GO

CREATE PROCEDURE [dbo].[SP_PXAP_GetLogs]
(
 @Severity NVARCHAR(64)
,@CategoryName NVARCHAR(128)
,@Source NVARCHAR(500)
,@Message NVARCHAR(300)
,@StartDate NVARCHAR(30)
,@EndDate NVARCHAR(30)
)
AS
BEGIN

declare @sql varchar(2000)

set @sql = 'SELECT l.LogID, l.Severity,l.MachineName Source, l.[TimeStamp] [Time], c.CategoryName, SUBSTRING(l.[Message],1,30)+ ''...'' ' + ' [Message]
FROM dbo.[Log] l 
INNER JOIN 
	dbo.[CategoryLog] cl 
ON 
	l.LogID = cl.LogID
INNER JOIN 
	dbo.[Category] c 
ON 
	cl.CategoryID = c.CategoryID
WHERE 1=1 '

if (@Severity <> 'ALL') 
begin
	set @sql = @sql + 'AND Severity = ''' + @Severity + ''''
end

if (@CategoryName <> 'ALL') 
begin
	set @sql = @sql + ' AND CategoryName = ''' + @CategoryName + ''''
end

set @sql = @sql + ' AND MachineName IN ('  + @Source + ')' 

if((isnull(@StartDate, '') <> '') AND (isnull(@EndDate, '') <> ''))
set @sql = @sql + ' AND [Timestamp] BETWEEN ''' + @StartDate + ''' AND '''  +  @EndDate + ''''

if(isnull(@Message, '') <> '')
begin
	set @sql = @sql + ' AND [Message] like ''%' + @Message + '%'''
end

print (@sql)

exec (@sql)

END

