
if not exists(select * from sys.databases where name = '$(DATABASENAME)')
begin
	CREATE DATABASE $(DATABASENAME)

	ALTER DATABASE $(DATABASENAME) SET COMPATIBILITY_LEVEL = 100

	IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
	begin
	EXEC $(DATABASENAME).[dbo].[sp_fulltext_database] @action = 'enable'
	end

	ALTER DATABASE $(DATABASENAME) SET ANSI_NULL_DEFAULT OFF 

	ALTER DATABASE $(DATABASENAME) SET ANSI_NULLS OFF 

	ALTER DATABASE $(DATABASENAME) SET ANSI_PADDING OFF 

	ALTER DATABASE $(DATABASENAME) SET ANSI_WARNINGS OFF 

	ALTER DATABASE $(DATABASENAME) SET ARITHABORT OFF 

	ALTER DATABASE $(DATABASENAME) SET AUTO_CLOSE OFF 

	ALTER DATABASE $(DATABASENAME) SET AUTO_CREATE_STATISTICS ON 

	ALTER DATABASE $(DATABASENAME) SET AUTO_SHRINK OFF 

	ALTER DATABASE $(DATABASENAME) SET AUTO_UPDATE_STATISTICS ON 

	ALTER DATABASE $(DATABASENAME) SET CURSOR_CLOSE_ON_COMMIT OFF 

	ALTER DATABASE $(DATABASENAME) SET CURSOR_DEFAULT  GLOBAL 

	ALTER DATABASE $(DATABASENAME) SET CONCAT_NULL_YIELDS_NULL OFF 

	ALTER DATABASE $(DATABASENAME) SET NUMERIC_ROUNDABORT OFF 

	ALTER DATABASE $(DATABASENAME) SET QUOTED_IDENTIFIER OFF 

	ALTER DATABASE $(DATABASENAME) SET RECURSIVE_TRIGGERS OFF 

	ALTER DATABASE $(DATABASENAME) SET  DISABLE_BROKER 

	ALTER DATABASE $(DATABASENAME) SET AUTO_UPDATE_STATISTICS_ASYNC OFF 

	ALTER DATABASE $(DATABASENAME) SET DATE_CORRELATION_OPTIMIZATION OFF 

	ALTER DATABASE $(DATABASENAME) SET TRUSTWORTHY OFF 

	ALTER DATABASE $(DATABASENAME) SET ALLOW_SNAPSHOT_ISOLATION OFF 

	ALTER DATABASE $(DATABASENAME) SET PARAMETERIZATION SIMPLE 

	ALTER DATABASE $(DATABASENAME) SET READ_COMMITTED_SNAPSHOT OFF 

	ALTER DATABASE $(DATABASENAME) SET HONOR_BROKER_PRIORITY OFF 

	ALTER DATABASE $(DATABASENAME) SET  READ_WRITE 

	ALTER DATABASE $(DATABASENAME) SET RECOVERY FULL 

	ALTER DATABASE $(DATABASENAME) SET  MULTI_USER 

	ALTER DATABASE $(DATABASENAME) SET PAGE_VERIFY CHECKSUM  

	ALTER DATABASE $(DATABASENAME) SET DB_CHAINING OFF 
end
GO