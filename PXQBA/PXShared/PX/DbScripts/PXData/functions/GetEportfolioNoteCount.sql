
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetEportfolioNoteCount]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetEportfolioNoteCount]
GO

CREATE function [dbo].[GetEportfolioNoteCount]  
(  
 @itemId VARCHAR(50) = '',  
 @reviewId VARCHAR(50) = '',  
 @courseId VARCHAR(50) = '',  
 @userId VARCHAR(50) = '',  
 @enrollmentId VARCHAR(50) = ''
 
)
  RETURNS @itemTable TABLE (HighlightCount NVARCHAR(50), NoteCount NVARCHAR(50))
  
BEGIN  

 DECLARE  
  @hCount INT,   
  @nCount INT  
  

 
  SELECT @hCount = COUNT(1)  
  FROM SubmissionHighlights sh  
  INNER JOIN Highlights h ON h.HighlightId = sh.HighlightId  
  Where   (@itemId = '' OR @itemId = sh.ItemId) AND  
    (@userId = '' OR @userId = h.UserId) AND  
    (@enrollmentId = '' OR @enrollmentId = sh.EnrollmentId) AND [Status] <> 2  
      
  SELECT @hCount += COUNT(1)  
  FROM ReviewHighlights rh  
  INNER JOIN Highlights h ON h.HighlightId = rh.HighlightId  
  Where   (@itemId = '' OR @itemId = rh.ItemId) AND   
    (@userId = '' OR @userId = h.UserId) AND  
    (@enrollmentId = '' OR @enrollmentId = rh.EnrollmentId) AND [Status] <> 2  
      
  SELECT @nCount = COUNT(1)  
  From Note n  
  INNER JOIN HighlightNotes hn ON hn.NoteId = n.NoteId  
  INNER JOIN SubmissionHighlights sh ON sh.HighlightId = hn.HighlightId  
  Where (@itemId = '' OR @itemId = sh.ItemId) AND  
    (@userId = '' OR @userId = n.UserId) AND  
    (@enrollmentId = '' OR @enrollmentId = sh.EnrollmentId)  AND [Status] <> 2  
        
  SELECT @nCount += COUNT(1)  
  From Note n  
  INNER JOIN HighlightNotes hn ON hn.NoteId = n.NoteId  
  INNER JOIN ReviewHighlights rh ON rh.HighlightId = hn.HighlightId  
  Where (@itemId = '' OR @itemId = rh.ItemId) AND  
    (@userId = '' OR @userId = n.UserId) AND  
    (@enrollmentId = '' OR @enrollmentId = rh.EnrollmentId)  AND [Status] <> 2  
      
  SELECT @nCount += COUNT(1)  
  From Note n  
  INNER JOIN SubmissionNotes sn ON sn.NoteId = n.NoteId  
  Where (@itemId = '' OR @itemId = sn.ItemId) AND  
    (@userId = '' OR @userId = n.UserId) AND  
    (@enrollmentId = '' OR @enrollmentId = sn.EnrollmentId) AND [Status] <> 2  
      
  SELECT @nCount += COUNT(1)  
  From Note n  
  INNER JOIN ReviewNotes rn ON rn.NoteId = n.NoteId  
  Where (@itemId = '' OR @itemId = rn.ItemId) AND  
    (@userId = '' OR @userId = n.UserId) AND  
    (@enrollmentId = '' OR @enrollmentId = rn.EnrollmentId) AND [Status] <> 2  
  
 INSERT @itemTable SELECT @hCount AS HighlightCount,@nCount AS NoteCount  
 RETURN 
END  
GO