SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[VideoNotes]','U') IS NULL
BEGIN
CREATE TABLE [dbo].[VideoNotes](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [nvarchar](50) NULL,
	[ItemId] [nvarchar](50) NOT NULL,
	[CourseId] [nvarchar](50) NOT NULL,
	[Text] [nvarchar](max) NULL,
	[Created] [datetime] NOT NULL,
	[Modified] [datetime] NOT NULL,
	[MinTime] [bigint] NULL,
	[MaxTime] [bigint] NULL,
	[IsInstructorNote] [bit] NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_VideoNotes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 85) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [dbo].[VideoNotes]  WITH CHECK ADD  CONSTRAINT [FK_VideoNotes_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])

ALTER TABLE [dbo].[VideoNotes] CHECK CONSTRAINT [FK_VideoNotes_Users]

ALTER TABLE [dbo].[VideoNotes] ADD  CONSTRAINT [DF_VideoNotes_IsInstructorNote]  DEFAULT ((0)) FOR [IsInstructorNote]

ALTER TABLE [dbo].[VideoNotes] ADD  CONSTRAINT [DF_VideoNotes_IsDeleted]  DEFAULT ((0)) FOR [IsDeleted]

END
GO


