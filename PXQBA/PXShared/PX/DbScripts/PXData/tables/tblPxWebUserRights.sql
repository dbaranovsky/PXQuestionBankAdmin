
/****** Object:  Table [dbo].[PxWebUserRights]    Script Date: 09/18/2012 11:14:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[PxWebUserRights]','U') IS NULL
BEGIN

CREATE TABLE [dbo].[PxWebUserRights](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[CourseId] [nvarchar](50) NOT NULL,
	[UserId] [nvarchar](50) NOT NULL,
	[PxWebRightId] [int] NOT NULL,
	[Rights] [bigint] NOT NULL,
 CONSTRAINT [PK_PxWebUserRights] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[PxWebUserRights]  WITH CHECK ADD  CONSTRAINT [FK_PxWebUserRights_PxWebRights] FOREIGN KEY([PxWebRightId])
REFERENCES [dbo].[PxWebRights] ([PxWebRightId])

ALTER TABLE [dbo].[PxWebUserRights] CHECK CONSTRAINT [FK_PxWebUserRights_PxWebRights]

ALTER TABLE [dbo].[PxWebUserRights] ADD  CONSTRAINT [DF_PxWebUserRights_Rights]  DEFAULT ((0)) FOR [Rights]

END

GO