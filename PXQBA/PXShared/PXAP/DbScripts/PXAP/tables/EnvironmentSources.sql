SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[EnvironmentSources]','U') IS NULL
BEGIN


CREATE TABLE [dbo].[EnvironmentSources](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EnvironmentId] [int] NOT NULL,
	[Source] [varchar](255) NOT NULL,
 CONSTRAINT [PK_EnvironmentSources] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]



SET ANSI_PADDING OFF


ALTER TABLE [dbo].[EnvironmentSources]  WITH CHECK ADD  CONSTRAINT [FK_EnvironmentSources_Environment] FOREIGN KEY([EnvironmentId])
REFERENCES [dbo].[Environment] ([EnvironmentId])


ALTER TABLE [dbo].[EnvironmentSources] CHECK CONSTRAINT [FK_EnvironmentSources_Environment]


END
GO
