
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[Note]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[Note](
		[NoteId] [uniqueidentifier] NOT NULL,
		[UserId] [nvarchar](50) NOT NULL,
		[Text] [nvarchar](max) NULL,
		[Public] [bit] NOT NULL,
		[Created] [datetime] NOT NULL,
		[Modified] [datetime] NOT NULL,
	 CONSTRAINT [PK_Note] PRIMARY KEY CLUSTERED 
	(
		[NoteId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[Note]  WITH CHECK ADD  CONSTRAINT [FK_Note_Users] FOREIGN KEY([UserId])
	REFERENCES [dbo].[Users] ([UserId])

	ALTER TABLE [dbo].[Note] CHECK CONSTRAINT [FK_Note_Users]

	ALTER TABLE [dbo].[Note] ADD  CONSTRAINT [DF_Note_Created]  DEFAULT (getdate()) FOR [Created]

	ALTER TABLE [dbo].[Note] ADD  CONSTRAINT [DF_Note_Modified]  DEFAULT (getdate()) FOR [Modified]

END

GO


