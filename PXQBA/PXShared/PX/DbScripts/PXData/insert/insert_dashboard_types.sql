/****** Object:  Table [dbo].[Dashboard_Types]   ******/



IF EXISTS (SELECT 1 FROM dbo.Dashboard_Types)


BEGIN
DELETE FROM dbo.Dashboard_Types


END

BEGIN

  INSERT INTO [dbo].[Dashboard_Types]
           ([Dashboard_type])
     VALUES
           ('program_dashboard')
		   
INSERT INTO [dbo].[Dashboard_Types]
           ([Dashboard_type])
     VALUES
           ('instructor_dashboard')

INSERT INTO [dbo].[Dashboard_Types]
           ([Dashboard_type])
     VALUES
           ('student_dashboard')


END
