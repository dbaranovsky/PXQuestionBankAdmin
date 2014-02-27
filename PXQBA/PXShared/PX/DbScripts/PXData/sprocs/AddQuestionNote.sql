SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddQuestionNote]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AddQuestionNote]
GO

CREATE Procedure [dbo].[AddQuestionNote]
(			
	@questionId nvarchar(100),	
    @courseId nvarchar(100),
	@userId nvarchar(50),
    @firstName nvarchar(50),
	@lastName nvarchar(50),
	@text nvarchar(max),
	@attachPath nvarchar(500)
)
As
Begin	

	IF NOT EXISTS(Select UserId From Users Where UserId = @userId)
	Begin
		Insert Into Users(UserId,FirstName,LastName)
		Values(@userId,@firstName,@lastName)
	End
	
	INSERT INTO [QuestionNotes]
           ([QuestionId]
           ,[CourseId]
           ,[UserId]
           ,[Text]
           ,[AttachPath]
           ,[Created])
     VALUES
           (@questionId
           ,@courseId
           ,@userId
           ,@text
           ,@attachPath
           ,GETDATE())
           
     return @@identity
	
End

GO


