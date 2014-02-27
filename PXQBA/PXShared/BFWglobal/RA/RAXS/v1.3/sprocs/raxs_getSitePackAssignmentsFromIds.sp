
CREATE PROCEDURE raxs_getSitePackAssignmentsFromIds(@siteids as varchar(2000)) as
BEGIN

	SET NOCOUNT ON

	CREATE TABLE #List(Item varchar(8000)) -- Create a temporary table

	INSERT #List (Item) exec raxs_Split @siteids



SELECT DISTINCT 
	tblPackage.PackageID, 
	tblPackage.Description, 
	tblPackage.Type, 
	tblSiteAssignments.AssignmentID, 
	tblSiteAssignments.SiteID, 
	tblLevelType.LevelOfAccess 
FROM	tblPackage WITH (nolock) 
	INNER JOIN tblSiteAssignments (nolock) 
		ON tblPackage.PackageID = tblSiteAssignments.PackageID 
	INNER JOIN tblLevelType (nolock) 
		ON tblLevelType.LevelID = tblSiteAssignments.LevelID 
	INNER JOIN #List 
		ON tblSiteAssignments.SiteID = item
--where siteid in (select item from #list)
--WHERE tblSiteAssignments.SiteID IN (@siteids) 
ORDER BY tblSiteAssignments.SiteID, tblPackage.PackageID


END

GO
