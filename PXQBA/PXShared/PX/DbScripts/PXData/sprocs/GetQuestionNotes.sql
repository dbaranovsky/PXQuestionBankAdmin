SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetQuestionNotes]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetQuestionNotes]
GO

CREATE PROCEDURE [dbo].[GetQuestionNotes](@questionId nvarchar(100))
AS
BEGIN
	
	Select QuestionNotes.*, Users.FirstName, Users.LastName  from QuestionNotes
		inner join Users
			on QuestionNotes.UserId = Users.UserId
	where QuestionId = @questionId
		order by created desc

END

GO


