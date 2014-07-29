CREATE PROCEDURE [dbo].[AddQBACourse]
(
  @courseId INT
)
AS
BEGIN
IF NOT EXISTS(SELECT * FROM QBACourse WHERE Id = @courseId)
 BEGIN
 INSERT INTO QBACourse(Id)
  VALUES
    (@courseId)
 END
 ELSE
 BEGIN
   SELECT @courseId
 END
END