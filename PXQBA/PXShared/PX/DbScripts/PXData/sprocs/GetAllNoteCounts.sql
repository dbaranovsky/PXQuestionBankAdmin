
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM sysobjects WHERE id = object_id(N'[dbo].[GetAllNoteCounts]') AND OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[GetAllNoteCounts]
END
GO

CREATE PROCEDURE [dbo].[GetAllNoteCounts](@reqXml XML) AS
BEGIN
DECLARE @NotesDetail TABLE (
		itemId NVARCHAR(50),
		reviewId NVARCHAR(50),
		courseId NVARCHAR(50),
		userId NVARCHAR(50),		
		enrollmentId NVARCHAR(50)
		 )
				

		
INSERT INTO @NotesDetail(itemId ,
		reviewId ,courseId ,userId ,enrollmentId ) 
SELECT
	TempXML.Node.value('itemId[1]','NVARCHAR(50)'),
	TempXML.Node.value('reviewId[1]','NVARCHAR(50)'),
	TempXML.Node.value('courseId[1]','NVARCHAR(50)'),
	TempXML.Node.value('userId[1]','NVARCHAR(50)'),
	TempXML.Node.value('enrollmentId[1]','NVARCHAR(50)')	
FROM @reqXml.nodes('/EportfolioNotesSearch/notesSearch') AS TempXML(Node) 

/*
SELECT item.itemid,(select notecount from dbo.GetEportfolioNoteCount(item.itemId,item.reviewId,item.courseId,item.userId,item.enrollmentId)) as noteCount
FROM @NotesDetail AS item */

SELECT item.itemid,note.noteCount 
FROM @NotesDetail AS item 
CROSS APPLY dbo.[GetEportfolioNoteCount](item.itemId,item.reviewId,item.courseId,item.userId,item.enrollmentId)
AS note


END
GO