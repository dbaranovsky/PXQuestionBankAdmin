SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddQuestionLog]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AddQuestionLog]
GO

CREATE Procedure [dbo].[AddQuestionLog]
(			
	@questionId nvarchar(100),	
    @courseId nvarchar(100),
	@userId nvarchar(50),
    @firstName nvarchar(50),
	@lastName nvarchar(50),
	@eventType int,
	@version varchar(10),
	@QuestionChanges xml
)
As
Begin	

	IF NOT EXISTS(Select UserId From Users Where UserId = @userId)
	Begin
		Insert Into Users(UserId,FirstName,LastName)
		Values(@userId,@firstName,@lastName)
	End
	
	INSERT INTO [QuestionLogs]
           ([QuestionId]
           ,[CourseId]
           ,[UserId]           
           ,[EventType]
           ,[Created], 
           [version], 
           [Changes])
     VALUES
           (@questionId
           ,@courseId
           ,@userId
           ,@eventType           
           ,GETDATE()
           ,@version,@QuestionChanges)
           
     return @@identity
	
End

GO


