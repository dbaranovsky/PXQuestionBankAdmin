SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SPLIT]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[SPLIT]
GO


CREATE FUNCTION [dbo].[SPLIT] 
      (  @DELIMITER VARCHAR(5), 
         @LIST      VARCHAR(MAX) 
      ) 
      
      RETURNS @TABLEOFVALUES TABLE 
         (  ROWID   SMALLINT IDENTITY(1,1), 
            [VALUE] VARCHAR(MAX) 
         ) 
   AS 
      BEGIN
       
         DECLARE @LENSTRING INT 
    
         WHILE LEN( @LIST ) > 0 
            BEGIN 
            
               SELECT @LENSTRING = 
                  (CASE CHARINDEX( @DELIMITER, @LIST ) 
                      WHEN 0 THEN LEN( @LIST ) 
                      ELSE ( CHARINDEX( @DELIMITER, @LIST ) -1 )
                   END
                  ) 
                                   
               INSERT INTO @TABLEOFVALUES 
                  SELECT SUBSTRING( @LIST, 1, @LENSTRING )
                   
               SELECT @LIST = 
                  (CASE ( LEN( @LIST ) - @LENSTRING ) 
                      WHEN 0 THEN '' 
                      ELSE RIGHT( @LIST, LEN( @LIST ) - @LENSTRING - 1 ) 
                   END
                  ) 
            END
             
         RETURN 
         
      END 
      
 