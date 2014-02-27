CREATE PROCEDURE raxs_getPackBatchesFromIds(@packageids as varchar(2000)) as
BEGIN

	SET NOCOUNT ON

	CREATE TABLE #List(Item varchar(8000)) -- Create a temporary table

	INSERT #List (Item) exec raxs_Split @packageids



SELECT DISTINCT 
	tblBatchKey.BatchID, 
	tblBatchKey.PackageID, 
	tblBatchKey.Description, 
	tblBatchKey.UseByDate, 
	tblBatchKey.RelativeExpiration, 
	tblBatchKey.AbsoluteExpiration, 
	tblBatchKey.Suspended, 
	tblBatchKey.Type, 
	tblBatchKey.Price 
FROM	tblBatchKey WITH (nolock) 
	INNER JOIN #List 
		ON tblBatchKey.PackageID = item
--WHERE tblBatchKey.PackageID IN (& sPackageIDs &) 
ORDER BY tblBatchKey.PackageID, tblBatchKey.AbsoluteExpiration DESC


END

GO
