USE PXData2
GO

CREATE TABLE dbo.QBARole(
  Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
  CourseId NVARCHAR(4000) NULL,
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
  @roleName   NVARCHAR(MAX)
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
  @courseId NVARCHAR(MAX)
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

CREATE TYPE dbo.QBACapabilityList
AS TABLE
(
  Id INT
);
GO

CREATE PROCEDURE dbo.UpdateQBARoleCapabilities
(
  @roleId INT,
  @capabilityIds AS dbo.QBACapabilityList READONLY
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

CREATE PROCEDURE dbo.GetUsersForQBA()
AS
BEGIN
  
 SELECT
  r.UserId,
  u.FirstName,
  u.LastName,
  COUNT(ru.RoleId)
FROM
  PxWebUserRights r
  LEFT JOIN UserQBARole ru ON r.UserId = ru.UserId
  JOIN Users u ON r.UserId = u.UserId
WHERE
  r.RightId = 1
GROUP BY
  r.UserId,
  u.FirstName,
  u.LastName
END
GO

USE PXData2
GO

CREATE PROCEDURE dbo.GetUserCourses
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

CREATE PROCEDURE dbo.GetUserRoles
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
