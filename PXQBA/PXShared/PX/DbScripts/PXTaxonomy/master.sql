-- Please enable SQLCMD mode (In Microsoft SQL Server Management Studio, select 
--  "Query --> SQLCMD Mode" from the menu).
--
-- MODIFY:	(1) The database name (DATABASENAME)
--			(2) The path to the SQL Scripts \Database directory (PATH)
--******************************************************************************

--set these variables if running directly, BUT DO NOT CHECK THEM IN
--:setvar DATABASENAME "PXTaxonomy"
--:setvar PATH "C:\Development\MacMillan\PX\Trunk\PX\DbScripts\PXTaxonomy\"

if not exists(select * from sys.databases where name = '$(DATABASENAME)')
    :r $(PATH)"create.sql"

USE $(DATABASENAME);
GO

-- CREATE TABLES
:r $(PATH)"tables\Taxonomy.sql"
:r $(PATH)"tables\TaxonomyScope.sql"
:r $(PATH)"tables\TaxonomyItem.sql"

-- ALTER SCRIPTS

-- CREATE FUNCTIONS

-- CREATE SPROCS
:r $(PATH)"sprocs\SaveTaxonomy.sql"
:r $(PATH)"sprocs\SaveTaxonomyScope.sql"
:r $(PATH)"sprocs\SaveTaxonomyItem.sql"
:r $(PATH)"sprocs\LoadRelatedItems.sql"