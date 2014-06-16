USE PXData2
GO

CREATE TABLE dbo.Keyword(
  Id BIGINT IDENTITY,
  CourseId NVARCHAR(MAX) NOT NULL,
  FieldName NVARCHAR(MAX) NOT NULL,
  Keyword NVARCHAR(MAX) NOT NULL,
  CONSTRAINT PK_Keyword PRIMARY KEY (Id)
) 
GO

USE PXData2
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
    Keyword
  WHERE
    CourseId = @courseId AND
    FieldName = @fieldName
END
GO



USE PXData2
GO

CREATE PROCEDURE dbo.AddKeyword
(
  @courseId   NVARCHAR(MAX),
  @fieldName NVARCHAR(MAX),
  @keyword NVARCHAR(MAX)
)
AS
BEGIN
  
IF NOT EXISTS (SELECT * FROM Keyword k WHERE k.CourseId=@courseId AND k.fieldName = @fieldName AND k.Keyword = @keyword)
BEGIN
  INSERT INTO Keyword(CourseId, FieldName, Keyword)
  VALUES
    (@courseId, @fieldName, @keyword)
END
 
END
GO