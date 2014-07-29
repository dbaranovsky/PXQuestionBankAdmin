
ALTER TABLE dbo.QBAParsedFile 
ADD ResourcesData VARBINARY(MAX)


ALTER PROCEDURE dbo.AddQBAParsedFile
(
  @questionsData NVARCHAR(MAX),
  @fileName      NVARCHAR(4000),
  @status        INT,
  @resourcesData VARBINARY(MAX)
)
AS
BEGIN
  INSERT INTO QBAParsedFile(FileName
                          , QuestionsData
                          , Status
                          , ResourcesData)
  VALUES
    (@fileName, @questionsData, @status, @resourcesData)

  SELECT scope_identity() AS ID

END
GO

