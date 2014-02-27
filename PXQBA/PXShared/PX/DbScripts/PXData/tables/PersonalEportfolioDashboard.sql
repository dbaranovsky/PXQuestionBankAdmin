
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID('dbo.[PersonalEportfolioDashboard]','U') IS NULL
BEGIN

	CREATE TABLE [dbo].[PersonalEportfolioDashboard](
	[Personal_Eportfolio_Id] [int] IDENTITY(1,1) NOT NULL,
	[User_id] [int] NOT NULL,
	[User_Ref_Id] [int] NOT NULL,
	[Domain_Id] [int] NOT NULL,
	[Dashboard_Id] [int] NOT NULL,
	[Course_type] [varchar](MAX) NOT NULL
	 CONSTRAINT [PK_PersonalEportfolioDashboard] PRIMARY KEY CLUSTERED 
	(
		[Personal_Eportfolio_Id] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

END

GO