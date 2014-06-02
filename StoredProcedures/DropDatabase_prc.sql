create proc DropDatabase_prc
(
	@DatabaseName sysname
)
as
begin
	declare @sql nvarchar(255)

	set @sql =
	'alter database '+@DatabaseName+'
	set single_user
	with rollback immediate
	drop database '+@DatabaseName

	if exists(select 1 from sys.databases where name = @DatabaseName)
	begin
		exec sp_executesql @sql
	end
end