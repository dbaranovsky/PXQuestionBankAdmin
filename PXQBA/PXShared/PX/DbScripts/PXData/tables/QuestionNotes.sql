SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[QuestionNotes]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[QuestionNotes](
		[Id] [int] IDENTITY(1,1) NOT NULL,
		[QuestionId] [nvarchar](100) NULL,
		[CourseId] [nvarchar](100) NULL,
		[UserId] [nvarchar](50) NULL,
		[Text] [nvarchar](max) NULL,
		[AttachPath] [nvarchar](500) NULL,
		[Created] [datetime] NULL,
	 CONSTRAINT [PK_QuestionNotes] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 95) ON [PRIMARY]
	) ON [PRIMARY]


	ALTER TABLE [dbo].[QuestionNotes]  WITH CHECK ADD  CONSTRAINT [FK_QuestionNotes_Users] FOREIGN KEY([UserId])
	REFERENCES [dbo].[Users] ([UserId])


	ALTER TABLE [dbo].[QuestionNotes] CHECK CONSTRAINT [FK_QuestionNotes_Users]

END
