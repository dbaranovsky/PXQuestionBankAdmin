CREATE PROCEDURE raxs_login_getUserClasses(@userid as int) as
BEGIN

	SET NOCOUNT ON

SELECT 
	c.ClassID, c.CreatorID, c.Code, 
	ca.UserID,
	cu.UserEmail AS CreatorEmail, cu.FirstName AS CreatorFName, cu.LastName AS CreatorLName
FROM
	tblClassAssignments AS ca WITH (NOLOCK)
	INNER JOIN tblClass AS c WITH (NOLOCK)
		ON ca.ClassID = c.ClassID
	INNER JOIN tblUserProfile AS cu WITH (NOLOCK)
		ON c.CreatorID = cu.UserID
WHERE ca.UserID = @userid


END
GO
