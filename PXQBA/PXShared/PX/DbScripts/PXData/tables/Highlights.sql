
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[Highlights]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[Highlights](
		[HighlightId] [uniqueidentifier] NOT NULL,
		[Text] [nvarchar](max) NULL,
		[Description] [nvarchar](300) NULL,
		[Public] [bit] NOT NULL,
		[Status] [int] NOT NULL,
		[UserId] [nvarchar](50) NOT NULL,
		[Created] [datetime] NOT NULL,
		[Modified] [datetime] NOT NULL,
	 CONSTRAINT [PK_Highlights] PRIMARY KEY CLUSTERED 
	(
		[HighlightId] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[Highlights]  WITH CHECK ADD  CONSTRAINT [FK_Highlights_Users] FOREIGN KEY([UserId])
	REFERENCES [dbo].[Users] ([UserId])

	ALTER TABLE [dbo].[Highlights] CHECK CONSTRAINT [FK_Highlights_Users]

	ALTER TABLE [dbo].[Highlights] ADD  CONSTRAINT [DF_Highlights_Created]  DEFAULT (getdate()) FOR [Created]

	ALTER TABLE [dbo].[Highlights] ADD  CONSTRAINT [DF_Highlights_Modified]  DEFAULT (getdate()) FOR [Modified]

END

GO


