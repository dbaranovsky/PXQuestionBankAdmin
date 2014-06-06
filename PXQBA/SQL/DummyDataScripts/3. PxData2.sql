CREATE DATABASE PXData2;


USE PXData2
GO

CREATE TABLE dbo.QBANotes(
  Id BIGINT IDENTITY,
  NoteText NVARCHAR(MAX) NOT NULL,

  QuestionId NVARCHAR(MAX) NOT NULL,
  CONSTRAINT PK_QBANotes PRIMARY KEY (Id)
) 
GO

USE PXData2
GO

CREATE PROCEDURE dbo.GetQBANotes
(
  @questionId NVARCHAR(MAX)
)
AS
BEGIN
  SELECT *
  FROM
    QBANotes
  WHERE
    QuestionId = @questionId
END
GO



USE PXData2
GO

CREATE PROCEDURE dbo.CreateQBANote
(
  @noteText   NVARCHAR(MAX) = '',
  @questionId NVARCHAR(MAX),
  @noteId     INT           = NULL OUTPUT
)
AS
BEGIN
  INSERT INTO QBANotes(NoteText
                     , QuestionId)
  VALUES
    (@noteText, @questionId)

  SET @noteId = scope_identity();
  RETURN @noteId;
END
GO


USE PXData2
GO


CREATE PROCEDURE dbo.DeleteQBANote
(
  @noteId BIGINT
)
AS
BEGIN
  DELETE
  FROM
    QBANotes
  WHERE
    Id = @noteId
END
GO

USE PXData2
GO

CREATE PROCEDURE dbo.UpdateQBANote
(
  @noteId   BIGINT,
  @noteText   NVARCHAR(MAX)= ''
)
AS
BEGIN
  UPDATE QBANotes
  SET
    NoteText = @noteText
  WHERE
    Id = @noteId
END
GO

USE PXData2
GO

CREATE TABLE dbo.UserNotShownNotification(
  Id BIGINT IDENTITY,
  NotificationType NVARCHAR(MAX) NOT NULL,

  UserId NVARCHAR(MAX) NOT NULL,
  CONSTRAINT PK_QBANotes PRIMARY KEY (Id)
) 
GO

USE PXData2
GO

CREATE PROCEDURE dbo.GetUserNotShownNotifications
(
  @userId NVARCHAR(MAX)
)
AS
BEGIN
  SELECT *
  FROM
    UserNotShownNotification
  WHERE
    UserId = @userId
END
GO



USE PXData2
GO

CREATE PROCEDURE dbo.CreateUserNotShownNotification
(
  @userId   NVARCHAR(MAX),
  @notificationType NVARCHAR(MAX),
)
AS
BEGIN
  
IF NOT EXISTS (SELECT * FROM UserNotShownNotification n WHERE n.UserId=@userId AND n.NotificationType = @notificationType)
BEGIN
  INSERT INTO UserNotShownNotification(UserId, NotificationType)
  VALUES
    (@userId, @notificationType)
END
 
END
GO