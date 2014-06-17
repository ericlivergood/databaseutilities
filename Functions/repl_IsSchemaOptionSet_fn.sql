CREATE function repl_IsSchemaOptionSet_fn(@schemaOption binary(8), @schemaOptionToCheck bigint)
returns bit
as
begin
	return (case when @schemaOption & @schemaOptionToCheck = 0 then 0 else 1 end)
end

