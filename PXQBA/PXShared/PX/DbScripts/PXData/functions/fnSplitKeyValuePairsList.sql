SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SplitKeyValuePairsList]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[SplitKeyValuePairsList]
GO

CREATE FUNCTION [dbo].[SplitKeyValuePairsList] 
      (  @DELIMITER VARCHAR(5), 
		 @KeyValueDelimiter VARCHAR(5), 
         @LIST      VARCHAR(MAX) 
      ) 
      
      RETURNS @TABLEOFVALUES TABLE 
         (  ROWID   SMALLINT IDENTITY(1,1), 
            DataKey VARCHAR(MAX) ,    
            DataValue VARCHAR(MAX) 
         ) 
   AS 
   
      BEGIN
       
         DECLARE @LENSTRING INT 
         DECLARE @KeyValuePair VARCHAR(MAX) 
         DECLARE @pos INT 
         
         WHILE LEN( @LIST ) > 0 
            BEGIN 
            
               SELECT @LENSTRING = 
                  (CASE CHARINDEX( @DELIMITER, @LIST ) 
                      WHEN 0 THEN LEN( @LIST ) 
                      ELSE ( CHARINDEX( @DELIMITER, @LIST ) -1 )
                   END
                  ) 
                  
               SELECT @KeyValuePair = SUBSTRING( @LIST, 1, @LENSTRING ) 
               SELECT @pos = charindex(@KeyValueDelimiter , @KeyValuePair, 1)
                                  
               INSERT INTO @TABLEOFVALUES (DataKey, DataValue)
               VALUES (Substring(@KeyValuePair, 1, @pos-1), 
                       Substring(@KeyValuePair, @pos+1, LEN(@KeyValuePair )) 
                       )   
                   
               SELECT @LIST = 
                  (CASE ( LEN( @LIST ) - @LENSTRING ) 
                      WHEN 0 THEN '' 
                      ELSE RIGHT( @LIST, LEN( @LIST ) - @LENSTRING - 1 ) 
                   END
                  ) 
            END 
            
                 
         RETURN 
      END 
      
 