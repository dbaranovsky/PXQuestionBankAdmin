CREATE PROCEDURE raxs_login_getUserPackages(@userid as int) as
BEGIN

	SET NOCOUNT ON

SELECT  
	tblPackage.PackageID, 
	tblUserAssignments.Expiration 
FROM         tblUserAssignments WITH (nolock) 
             INNER JOIN  tblPackage WITH (nolock) ON tblUserAssignments.PackageID = tblPackage.PackageID 
WHERE     (tblUserAssignments.UserID = @userid) 
ORDER BY tblUserAssignments.Expiration DESC


END

GO
