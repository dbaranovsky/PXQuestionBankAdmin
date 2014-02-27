CREATE PROCEDURE raxs_login_getUserProfile(@userid as int) as
BEGIN

	SET NOCOUNT ON

SELECT *
FROM
	tblUserProfile WITH (NOLOCK)
WHERE UserID = @userid


END
GO
