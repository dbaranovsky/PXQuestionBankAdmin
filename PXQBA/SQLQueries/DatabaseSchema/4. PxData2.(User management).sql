USE PXData2
GO

CREATE TABLE dbo.QBACourse(
  Id VARCHAR(4000) PRIMARY KEY NOT NULL,
)  ON [PRIMARY]
GO

USE PXData2
GO

CREATE PROCEDURE dbo.GetQBACourses
AS
BEGIN
 SELECT
   Id AS CourseId
 FROM
   QBACourse
END
GO

USE PXData2
GO

CREATE TABLE dbo.QBARole(
  Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
  CourseId VARCHAR(4000) NOT NULL FOREIGN KEY REFERENCES QBACourse(Id) ON DELETE CASCADE,
  Name NVARCHAR(4000) NOT NULL,
  CanEdit  bit NOT NULL DEFAULT(1),
)  ON [PRIMARY]
GO

USE PXData2
GO

CREATE PROCEDURE dbo.AddQBARole
(
  @roleName   NVARCHAR(4000),
  @courseId   NVARCHAR(4000),
  @canEdit bit
)
AS
BEGIN
 IF NOT EXISTS(SELECT * FROM QBARole WHERE Name = @roleName AND CourseId = @courseId)
 BEGIN
  INSERT INTO QBARole(Name, CourseId, CanEdit)
  VALUES
    (@roleName, @courseId, @canEdit)
  
  SELECT SCOPE_IDENTITY() AS ID
  END
  ELSE 
  BEGIN
    SELECT Id from QBARole WHERE Name = @roleName AND CourseId = @courseId
  END
END
GO

USE PXData2
GO

CREATE PROCEDURE dbo.UpdateQBARole
(
  @roleId INT,
  @roleName   NVARCHAR(4000)
)
AS
BEGIN
  UPDATE QBARole
  SET Name = @roleName
  WHERE Id = @roleId AND CanEdit = 1
  
  SELECT @roleId
END
GO

USE PXData2
GO

CREATE PROCEDURE dbo.DeleteQBARole
(
  @roleId   INT
)
AS
BEGIN
  DELETE FROM
    QBARole
  WHERE
    Id = @roleId AND
    CanEdit = 1
END
GO

USE PXData2
GO

CREATE PROCEDURE dbo.GetQBAUserCoursesWithRoles
(
  @userId NVARCHAR(4000)
)
AS
BEGIN
  SELECT DISTINCT
    c.Id AS CourseId,
    r.Id AS RoleId,
    r.Name AS RoleName,
    CASE WHEN u.UserId = @userId THEN u.UserId ELSE NULL END AS UserId
  FROM
    QBACourse c
    LEFT JOIN QBARole r ON c.Id = r.CourseId
    LEFT JOIN UserQBARole u ON u.RoleId = r.Id
END
GO

USE PXData2
GO

CREATE TABLE dbo.QBARoleCapability(
  RoleId  INT NOT NULL FOREIGN KEY REFERENCES QBARole(Id) ON DELETE CASCADE,
  CapabilityId  INT NOT NULL,
  CONSTRAINT PK_QBARoleCapability PRIMARY KEY (RoleId,CapabilityId)
) 
GO

USE PXData2
GO

CREATE PROCEDURE dbo.GetQBARolesForCourse
(
  @courseId NVARCHAR(4000)
)
AS
BEGIN
  
 SELECT
   r.Id,
   r.Name,
   r.CanEdit,
   COUNT(c.CapabilityId) AS [Count]
 FROM
   QBARole r
   LEFT JOIN QBARoleCapability c ON r.Id = c.RoleId
 WHERE
   CourseId IS NULL
   OR CourseId = @courseId
 GROUP BY
  r.Id,
  r.Name,
  r.CanEdit
END
GO

USE PXData2
GO

CREATE TYPE dbo.QBAIdList
AS TABLE
(
  Id INT
);
GO

USE PXData2
GO

CREATE PROCEDURE dbo.UpdateQBARoleCapabilities
(
  @roleId INT,
  @capabilityIds AS dbo.QBAIdList READONLY
)
AS
BEGIN

 DELETE FROM 
   QBARoleCapability
 WHERE
   RoleId = @roleId
 
 INSERT INTO QBARoleCapability(RoleId, CapabilityId)
 SELECT
   @roleId,
   Id
 FROM
   @capabilityIds
END
GO

USE PXData2
GO

CREATE PROCEDURE dbo.GetQBARoleCapabilities
(
  @roleId INT
)
AS
BEGIN
  
 SELECT
   r.Id,
   r.Name,
   r.CanEdit,
   c.CapabilityId
 FROM
   QBARole r
   LEFT JOIN QBARoleCapability c ON r.Id = c.RoleId
 WHERE
   r.Id = @roleId
END
GO

USE PXData2
GO

CREATE TABLE dbo.UserQBARole(
  UserId  NVARCHAR(4000) NOT NULL,
  RoleId  INT NOT NULL FOREIGN KEY REFERENCES QBARole(Id) ON DELETE CASCADE,
  CONSTRAINT PK_UserQBARole PRIMARY KEY (UserId, RoleId)
) 
GO

USE PXData2
GO

CREATE PROCEDURE dbo.GetQBAUsers
AS
BEGIN
 SELECT DISTINCT 
  UserId
INTO 
  #tmp
FROM
  PxWebUserRights
WHERE
  PxWebRightId = (SELECT TOP 1 PxWebRightId FROM PxWebRights WHERE PxWebRightType = 'QuestionBank')
 
SELECT
  t.UserId AS Id,
  SUM(CASE WHEN ru.RoleId IS NULL THEN 0 ELSE 1 END) AS Count
FROM
  #tmp t
  LEFT JOIN UserQBARole ru ON t.UserId = ru.UserId
GROUP BY
  t.UserId
DROP TABLE #tmp
END
GO


USE PXData2
GO

CREATE PROCEDURE dbo.GetQBAUserRoles
(
  @userId NVARCHAR(4000)
)
AS
BEGIN
  
 SELECT
   r.CourseId,
   r.Id,
   r.Name
 FROM
   UserQBARole u
   LEFT JOIN QBARole r ON r.Id = u.RoleId
 WHERE
   u.UserId = @userId
END
GO

CREATE PROCEDURE dbo.GetQBAUserCourses
(
  @userId NVARCHAR(4000)
)
AS
BEGIN
 SELECT
   r.CourseId
 FROM
   UserQBARole u
   LEFT JOIN QBARole r ON r.Id = u.RoleId
 WHERE
   u.UserId = @userId
END
GO

USE PXData2
GO

CREATE PROCEDURE dbo.UpdateQBAUserRoles
(
  @userId NVARCHAR(4000),
  @roleIds AS dbo.QBAIdList READONLY
)
AS
BEGIN

 DELETE FROM 
   UserQBARole
 WHERE
   UserId = @userId
 
 INSERT INTO UserQBARole(UserId, RoleId)
 SELECT
   @userId,
   Id
 FROM
   @roleIds
END
GO

USE PXData2
GO

CREATE PROCEDURE dbo.GetQBAUserCapabilities
(
  @userId INT,
  @courseId VARCHAR(4000)
)
AS
BEGIN
 
SELECT
   c.CapabilityId
 FROM
   UserQBARole u
   JOIN QBARole r ON r.Id = u.RoleId AND r.CourseId = @courseId
   JOIN QBARoleCapability c ON c.RoleId = r.Id
 WHERE
   u.UserId = @userId
   
END
GO
