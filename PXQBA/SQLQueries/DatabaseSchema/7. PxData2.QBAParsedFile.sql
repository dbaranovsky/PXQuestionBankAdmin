USE PXData2
GO

CREATE TABLE dbo.QBAParsedFile(
  Id BIGINT IDENTITY,
  FileName NVARCHAR(4000),
  QuestionsData NVARCHAR(MAX) NOT NULL,
  Status INT,
  CONSTRAINT PK_QBAParsedFile PRIMARY KEY (Id)
) 
GO

USE PXData2
GO

CREATE PROCEDURE dbo.AddQBAParsedFile
(
  @questionsData  NVARCHAR(max),
  @fileName nvarchar(4000),
  @status int
)
AS
BEGIN
  INSERT INTO QBAParsedFile(FileName, QuestionsData, Status)
  VALUES
    (@fileName, @questionsData, @status)
  
  SELECT SCOPE_IDENTITY() AS ID
  
END
GO

USE PXData2
GO

CREATE PROCEDURE dbo.GetQBAParsedFile
(
  @id  bigint
)
AS
BEGIN
   SELECT 
    *
  FROM
    QBAParsedFile
  
END
GO

USE PXData2
GO

CREATE PROCEDURE dbo.SetQBAParsedFile
(
  @id bigint,
  @status int
)
AS
BEGIN
  UPDATE QBAParsedFile
  SET Status = @status
  WHERE Id = @id
  
  SELECT @id
END
GO