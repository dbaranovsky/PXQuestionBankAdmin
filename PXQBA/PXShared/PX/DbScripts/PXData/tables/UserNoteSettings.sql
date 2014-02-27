
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[UserNoteSettings]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[UserNoteSettings](
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[UserId] [nvarchar](50) NOT NULL,
		[CourseId] [nvarchar](50) NOT NULL,
		[ShowMyNotes] [bit] NOT NULL,
		[ShowInstructorNotes] [bit] NOT NULL,
		[ShowMyHighlights] [bit] NOT NULL,
		[ShowInstructorHighlights] [bit] NOT NULL,
		[HighlightColor] [nvarchar](20) NOT NULL,
	 CONSTRAINT [PK_UserNoteSettings] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[UserNoteSettings]  WITH CHECK ADD  CONSTRAINT [FK_UserNoteSettings_Users] FOREIGN KEY([UserId])
	REFERENCES [dbo].[Users] ([UserId])

	ALTER TABLE [dbo].[UserNoteSettings] CHECK CONSTRAINT [FK_UserNoteSettings_Users]

	ALTER TABLE [dbo].[UserNoteSettings] ADD  CONSTRAINT [DF_UserNoteSettings_ShowMyNotes]  DEFAULT ((1)) FOR [ShowMyNotes]

	ALTER TABLE [dbo].[UserNoteSettings] ADD  CONSTRAINT [DF_UserNoteSettings_ShowInstructorNotes]  DEFAULT ((1)) FOR [ShowInstructorNotes]

	ALTER TABLE [dbo].[UserNoteSettings] ADD  CONSTRAINT [DF_UserNoteSettings_ShowMyHighlights]  DEFAULT ((1)) FOR [ShowMyHighlights]

	ALTER TABLE [dbo].[UserNoteSettings] ADD  CONSTRAINT [DF_UserNoteSettings_ShowInstructorHighlights]  DEFAULT ((0)) FOR [ShowInstructorHighlights]

END

GO


