create view DefaultDirectory_vw
as

	select 
	(	
		select top 1 
			dbo.path_GetDirectory_fn(physical_name)
		from sys.master_files
		where file_id = 1 
		group by dbo.path_GetDirectory_fn(physical_name)
		order by count(*) desc
	) + '\' as dataDirectory,
	(
		select top 1 
			dbo.path_GetDirectory_fn(physical_name)
		from sys.master_files
		where type = 1
		group by dbo.path_GetDirectory_fn(physical_name)
		order by count(*) desc
	) + '\' as logDirectory