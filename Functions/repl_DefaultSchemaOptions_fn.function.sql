CREATE FUNCTION [dbo].[repl_DefaultSchemaOptions_fn]
()
returns binary(8)
AS
BEGIN
	declare @schemaOption binary(8)
	set @schemaOption = 0x0;
	set @schemaOption = dbo.repl_AddSchemaOption_fn(@schemaOption, 1); --Object creation
	set @schemaOption = dbo.repl_AddSchemaOption_fn(@schemaOption, 2); --create repl stored procs
	set @schemaOption = dbo.repl_AddSchemaOption_fn(@schemaOption, 16); --create clustered index
	set @schemaOption = dbo.repl_AddSchemaOption_fn(@schemaOption, 32); --convert UDDTs
	set @schemaOption = dbo.repl_AddSchemaOption_fn(@schemaOption, 64); --create nonclustered indexes
	set @schemaOption = dbo.repl_AddSchemaOption_fn(@schemaOption, 128); --Create Primary Keys
	set @schemaOption = dbo.repl_AddSchemaOption_fn(@schemaOption, 65536); --replicate check constraints
	set @schemaOption = dbo.repl_AddSchemaOption_fn(@schemaOption, 131072); --replicate FKs
	set @schemaOption = dbo.repl_AddSchemaOption_fn(@schemaOption, 134217728); --create schemas where they dont exist.
	set @schemaOption = dbo.repl_AddSchemaOption_fn(@schemaOption, 17179869184); --Replicates the compression option for data and indexes.
	return @schemaOption
END