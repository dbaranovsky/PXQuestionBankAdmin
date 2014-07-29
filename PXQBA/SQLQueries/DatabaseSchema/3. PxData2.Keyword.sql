
CREATE TABLE dbo.QBAKeyword(
  Id BIGINT IDENTITY,
  CourseId NVARCHAR(MAX) NOT NULL,
  FieldName NVARCHAR(MAX) NOT NULL,
  Keyword NVARCHAR(MAX) NOT NULL,
  CONSTRAINT PK_QBAKeyword PRIMARY KEY (Id)
) 
GO

CREATE PROCEDURE dbo.GetKeywordList
(
  @courseId NVARCHAR(MAX),
  @fieldName NVARCHAR(MAX)
)
AS
BEGIN
  SELECT *
  FROM
    QBAKeyword
  WHERE
    CourseId = @courseId AND
    FieldName = @fieldName
END
GO


CREATE PROCEDURE dbo.AddKeyword
(
  @courseId   NVARCHAR(MAX),
  @fieldName NVARCHAR(MAX),
  @keyword NVARCHAR(MAX)
)
AS
BEGIN
  
IF NOT EXISTS (SELECT * FROM QBAKeyword k WHERE k.CourseId=@courseId AND k.fieldName = @fieldName AND k.Keyword = @keyword)
BEGIN
  INSERT INTO QBAKeyword(CourseId, FieldName, Keyword)
  VALUES
    (@courseId, @fieldName, @keyword)
END
 
END
GO