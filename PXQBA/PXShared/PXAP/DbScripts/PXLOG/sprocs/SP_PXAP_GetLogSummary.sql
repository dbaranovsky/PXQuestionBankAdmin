SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[SP_PXAP_GetLogSummary]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[SP_PXAP_GetLogSummary]
END
GO

CREATE PROCEDURE [dbo].[SP_PXAP_GetLogSummary]
(@Source XML)
AS
BEGIN

DECLARE @days INT
DECLARE @weeks INT
DECLARE @startdate  DATETIME
DECLARE @endate  DATETIME
DECLARE @sources TABLE (SourceName VARCHAR(50)) 

SELECT @startdate = MIN([Timestamp]), @endate = GETDATE() FROM dbo.[Log]
SELECT @days = DATEDIFF(DAY,@startdate,@endate), @weeks = DATEDIFF(WEEK,@startdate,@endate)

INSERT INTO @sources 
	SELECT Sources.SourceName.value('.','VARCHAR(50)')
	FROM @Source.nodes('/sources/source') as Sources(SourceName)

SELECT	ISNULL(Today.Severity, AllDays.Severity) Severity, 
	dbo.FN_PXAP_GetErrorCount(ISNULL(Today.TotalToday,0)) TotalToday, 
	dbo.FN_PXAP_GetErrorCount(ISNULL(AllDays.AvgDay,0)) AvgDay,
	dbo.FN_PXAP_GetErrorCount(ISNULL(AllDays.AvgWeek,0)) AvgWeek, 
	dbo.FN_PXAP_GetErrorCount(ISNULL(AllDays.Total ,0)) Total
FROM 
	(SELECT [Severity], COUNT(1) TotalToday 
	FROM dbo.[Log] INNER JOIN @sources on MachineName = SourceName 
	WHERE DATEDIFF(DAY,[Timestamp],GETDATE()) = 0
	GROUP BY [Severity]) Today
FULL OUTER JOIN 
	(SELECT [Severity], COUNT(1)/@days AS AvgDay, COUNT(1)/@weeks AS AvgWeek, COUNT(1) Total 
	FROM dbo.[Log] INNER JOIN @sources on MachineName = SourceName
	GROUP BY [Severity]) AllDays 
ON Today.Severity = AllDays.Severity

END

GO
