CREATE PROCEDURE raxs_Split(

    @sInputList varchar(8000) -- List of delimited items
  , @Delimiter char(1) = ',' -- delimiter that separates items
)

AS BEGIN

SET NOCOUNT ON

DECLARE @Item Varchar(8000)

CREATE TABLE #List(Item varchar(8000)) -- Create a temporary table

--IF CHARINDEX(@Delimiter,@sInputList,0) <> 0
--BEGIN
WHILE CHARINDEX(@Delimiter,@sInputList,0) <> 0
BEGIN
SELECT
@Item=RTRIM(LTRIM(SUBSTRING(@sInputList,1,CHARINDEX(@Delimiter,@sInputList,0
)-1))),
@sInputList=RTRIM(LTRIM(SUBSTRING(@sInputList,CHARINDEX(@Delimiter,@sInputList,0)+1,LEN(@sInputList))))

IF LEN(@Item) > 0
INSERT INTO #List SELECT @Item

END
--END

IF LEN(@sInputList) > 0
INSERT INTO #List SELECT @sInputList -- Put the last item in

select item from #list
END


GO
