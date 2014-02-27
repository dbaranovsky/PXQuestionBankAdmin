GO

SET ANSI_PADDING OFF
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[Dashboard]','U') IS NULL
BEGIN
CREATE TABLE [dbo].[Dashboard](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[User_id] [int] NOT NULL,
	[User_Ref_Id] [int] NOT NULL,
	[Domain_Id] [int] NOT NULL,
	[Dashboard_Id] [int] NOT NULL,
	[Course_type] [varchar](100) NULL,
	[Dashboard_type] [varchar](150) NOT NULL,
	[ProductCourse_Id] [varchar](100) NOT NULL,
 CONSTRAINT [PK_DashboardKey] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 95) ON [PRIMARY]
) ON [PRIMARY]

END