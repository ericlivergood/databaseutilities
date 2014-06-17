CREATE function repl_AddSchemaOption_fn
(
	@schemaOption binary(8)
,	@schemaOptionToAdd bigint
)
returns binary(8)
as
begin
	return (
		case 
			when @schemaOption & @schemaOptionToAdd = 0 
				then cast(@schemaOption | @schemaOptionToAdd as binary(8)) 
			else @schemaOption 
		end
	)
end