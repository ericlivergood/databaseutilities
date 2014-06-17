CREATE function repl_GetSchemaOptions_fn 
(
	@schemaOption binary(8)
)
returns table
as

	return (
		select 
			SchemaOption
		,	HexValue
		,	Description
		from dbo.repl_SchemaOptions_vw
		where @schemaOption & SchemaOption != 0
	)
