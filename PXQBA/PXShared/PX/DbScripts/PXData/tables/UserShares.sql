
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[UserShares]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[UserShares](
		[Id] [bigint] IDENTITY(1,1) NOT NULL,
		[UserId] [nvarchar](50) NOT NULL,
		[SharedUserId] [nvarchar](50) NOT NULL,
		[CourseId] [nvarchar](50) NOT NULL,
		[Enabled] [bit] NOT NULL,
		[HighlightColor] [nvarchar](20) NOT NULL,
	 CONSTRAINT [PK_UserShares] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[UserShares]  WITH CHECK ADD  CONSTRAINT [FK_UserShares_Users] FOREIGN KEY([UserId])
	REFERENCES [dbo].[Users] ([UserId])

	ALTER TABLE [dbo].[UserShares] CHECK CONSTRAINT [FK_UserShares_Users]

	ALTER TABLE [dbo].[UserShares]  WITH CHECK ADD  CONSTRAINT [FK_UserShares_Users_SharedUser] FOREIGN KEY([SharedUserId])
	REFERENCES [dbo].[Users] ([UserId])

	ALTER TABLE [dbo].[UserShares] CHECK CONSTRAINT [FK_UserShares_Users_SharedUser]

	ALTER TABLE [dbo].[UserShares] ADD  CONSTRAINT [DF_UserShares_Enabled]  DEFAULT ((0)) FOR [Enabled]

END

GO


