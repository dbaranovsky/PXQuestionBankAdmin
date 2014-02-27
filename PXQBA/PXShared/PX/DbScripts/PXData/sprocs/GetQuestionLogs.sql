SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetQuestionLogs]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetQuestionLogs]
GO

Create PROCEDURE [dbo].[GetQuestionLogs](@questionId nvarchar(100))
AS
BEGIN
	
	Select QuestionLogs.*, Users.FirstName, Users.LastName  from QuestionLogs
		inner join Users
			on QuestionLogs.UserId = Users.UserId
	where QuestionId = @questionId
		order by created desc

END

GO


