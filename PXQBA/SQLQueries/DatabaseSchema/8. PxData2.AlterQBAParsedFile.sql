USE PXData2
GO

ALTER TABLE dbo.QBAParsedFile 
ADD ResourcesData VARBINARY(MAX)

USE PXData2
GO

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

USE PXData2
GO
