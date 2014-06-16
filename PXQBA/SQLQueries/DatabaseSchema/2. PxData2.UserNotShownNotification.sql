USE PXData2
GO

CREATE TABLE dbo.UserNotShownNotification(
  Id BIGINT IDENTITY,
  NotificationType NVARCHAR(MAX) NOT NULL,

  UserId NVARCHAR(MAX) NOT NULL,
  CONSTRAINT PK_UserNotShownNotification PRIMARY KEY (Id)
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
  @notificationType NVARCHAR(MAX)
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