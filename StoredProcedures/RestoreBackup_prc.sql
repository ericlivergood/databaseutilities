create procedure RestoreBackup_prc
(
	@db sysname = null
,	@backupFile nvarchar(max)
,	@keepExisting bit = 0
,	@restoreReadOnly bit = 0
,	@keptDBName nvarchar(max) = null OUTPUT
,	@killConnections bit = 1
,	@printCommandsOnly bit = 0
,	@useBackupDatabaseName bit = 0
,	@online bit = 1
)
as
begin
	if(@db is null and @useBackupDatabaseName = 0)
	begin
		raiserror('Need to know how to name the database, specify a name in @db, or set @useBackupDatabaseName to 1', 19, 0);
		return;
	end

	declare @sql varchar(max), @movesql varchar(max), @olddb varchar(max), @backupTime datetime;

	declare @dt varchar(100);
	set @dt = convert(varchar, getdate(), 112)+ replace(CONVERT(varchar, getdate(), 108), ':', '');
	set @olddb = @db + '_' + @dt;

	if(exists(select 1 from sys.databases where name = @db and is_published = 1))
	begin
		raiserror('Database is replicated.  Disable replication (using sp_replicationdboption) or specify a different database name.', 18, 0);
	end


	declare @logDir nvarchar(max), @dataDir nvarchar(max)

	select @logDir = logDirectory, @dataDir = dataDirectory
	from dbo.DefaultDirectory_vw;

	if(@useBackupDatabaseName = 1)
	begin

		if(object_id('tempdb..#header') is not null)
			drop table #header;
		
		create table #header 
		(
			[BackupName] nvarchar(128) NULL
		,	[BackupDescription] nvarchar(255) NULL
		,	[BackupType] smallint NULL
		,	[ExpirationDate] datetime NULL
		,	[Compressed] bit NULL
		,	[Position] smallint NULL
		,	[DeviceType] tinyint NULL
		,	[UserName] nvarchar(128) NULL
		,	[ServerName] nvarchar(128) NULL
		,	[DatabaseName] nvarchar(128) NULL
		,	[DatabaseVersion] int NULL
		,	[DatabaseCreationDate] datetime NULL
		,	[BackupSize] numeric(20,0) NULL
		,	[FirstLSN] numeric(25,0) NULL
		,	[LastLSN] numeric(25,0) NULL
		,	[CheckpointLSN] numeric(25,0) NULL
		,	[DatabaseBackupLSN] numeric(25,0) NULL
		,	[BackupStartDate] datetime NULL
		,	[BackupFinishDate] datetime NULL
		,	[SortOrder] smallint NULL
		,	[CodePage] smallint NULL
		,	[UnicodeLocaleId] int NULL
		,	[UnicodeComparisonStyle] int NULL
		,	[CompatibilityLevel] tinyint NULL
		,	[SoftwareVendorId] int NULL
		,	[SoftwareVersionMajor] int NULL
		,	[SoftwareVersionMinor] int NULL
		,	[SoftwareVersionBuild] int NULL
		,	[MachineName] nvarchar(128) NULL
		,	[Flags] int NULL
		,	[BindingID] uniqueidentifier NULL
		,	[RecoveryForkID] uniqueidentifier NULL
		,	[Collation] nvarchar(128) NULL
		,	[FamilyGUID] uniqueidentifier NULL
		,	[HasBulkLoggedData] bit NULL
		,	[IsSnapshot] bit NULL
		,	[IsReadOnly] bit NULL
		,	[IsSingleUser] bit NULL
		,	[HasBackupChecksums] bit NULL
		,	[IsDamaged] bit NULL
		,	[BeginsLogChain] bit NULL
		,	[HasIncompleteMetaData] bit NULL
		,	[IsForceOffline] bit NULL
		,	[IsCopyOnly] bit NULL
		,	[FirstRecoveryForkID] uniqueidentifier NULL
		,	[ForkPointLSN] numeric(25,0) NULL
		,	[RecoveryModel] nvarchar(60) NULL
		,	[DifferentialBaseLSN] numeric(25,0) NULL
		,	[DifferentialBaseGUID] uniqueidentifier NULL
		,	[BackupTypeDescription] nvarchar(60) NULL
		,	[BackupSetGUID] uniqueidentifier NULL
		,	[CompressedBackupSize] bigint NULL
		,
		);

		set @sql = 'restore HEADERONLY
			from disk = '''+@backupFile+'''
		';

		insert into #header
		exec(@sql);
		
		select top 1 @db = DatabaseName
		from #header
		
		if(@db in('master', 'msdb', 'model', 'tempdb', 'distribution'))
		begin
			raiserror('You are trying to restore a system database.', 19, 0);
			return;
		end
	end

	select top 1 @backupTime = lastModifiedDate
	from files_fn(dbo.path_GetDirectory_fn(@backupFile), dbo.path_GetFileName_fn(@backupFile), 0);

	if(object_id('tempdb..#filelist') is not null)
		drop table #filelist;
		
	create table #filelist 
	(
		LogicalName varchar(128)
	,	[PhysicalName] varchar(512)
	,	[Type] varchar
	,	[FileGroupName] varchar(128)
	,	[Size] varchar(128)
	,	[MaxSize] varchar(128)
	,	[FileId]varchar(128)
	,	[CreateLSN]varchar(128)
	,	[DropLSN]varchar(128)
	,	[UniqueId]varchar(128)
	,	[ReadOnlyLSN]varchar(128)
	,	[ReadWriteLSN]varchar(128)
	,	[BackupSizeInBytes]varchar(128)
	,	[SourceBlockSize]varchar(128)
	,	[FileGroupId]varchar(128)
	,	[LogGroupGUID]varchar(128)
	,	[DifferentialBaseLSN]varchar(128)
	,	[DifferentialBaseGUID]varchar(128)
	,	[IsReadOnly]varchar(128)
	,	[IsPresent]varchar(128)
	);

	set @sql = 'restore FILELISTONLY
		from disk = '''+@backupFile+'''
	';

	insert into #filelist
	exec(@sql);


	if(exists(select 1 from sys.databases where name = @db + '_new'))
	begin
		set @sql = 'drop database '+@db + '_new;';
		
		exec(@sql)		
	end


	set @moveSql = (
			select 'move '''+LogicalName+''' to '''+ 
				case [Type] 
					when 'L' then @logDir 
					else @dataDir
				end
				+
				replace(dbo.path_GetFileName_fn(PhysicalName), dbo.path_GetExtension_fn(physicalName), '' ) + '_'+@dt+dbo.path_GetExtension_fn(physicalName)
				+ ''''+
				case 
					when FileId = (select max(FileId) from #filelist) 
						then char(10) 
					else ',' + CHAR(10) 
				end
			from #filelist
				
			order by FileId
			for xml path('')
		);

	if @online = 0
	begin
		exec DropDatabase_prc @db
	end

	set @sql = 'restore database ' + @db + '_new
		from disk = '''+@backupFile+'''
		with REPLACE,
	' + @movesql;

	if(@printCommandsOnly = 1)
	begin
		print @sql;
	end
	else
	begin
		exec(@sql);
	end

	set @sql = ''
	if
	(
		@killConnections = 1 
		and exists(select 1 from sys.databases where name = @db)
	)
	begin
		set @sql = '
		ALTER DATABASE ' + @db + '
		SET SINGLE_USER
		WITH ROLLBACK IMMEDIATE;
		'

		if(@printCommandsOnly = 1)
		begin
			print @sql;
		end
		else
		begin
			exec(@sql);
		end		
	end
	
	if(exists(select 1 from sys.databases where name = @db))
	begin
	
		declare @oldDBsql nvarchar(max) = '';
		
		if(@keepExisting = 1)
		begin
			set @oldDBsql = 'alter database '+@db+' MODIFY NAME = '+@olddb+';';
			set @keptDBName = @olddb;
		end
		else
		begin
			exec DropDatabase_prc @db
		end


		if(@killConnections = 1 AND @keepExisting = 1 AND @online = 1) 
		begin
			set @sql = '

			set xact_abort on;
			
			declare @retries int, @success bit, @error nvarchar(max);
			set @retries = 5;
			set @success = 0;

			while(@retries > 0 and @success = 0)
			begin

				alter database '+@db+' 
				set multi_user
				with rollback immediate

				begin try
					'+@oldDBSql+'
					set @success = 1;
				end try
				begin catch
					set @retries = @retries - 1;
					if(@retries = 0)
					begin
						declare @msg nvarchar(max);
						set @msg = ERROR_MESSAGE();
						raiserror(@msg, 18, 0)
					end
				end catch
			end
		';
		end
		else 
		begin
			set @sql = @oldDBSql;
		end
		
	end

	set @sql = @sql + '
		exec sp_rename '''+@db+'_new'', '''+@db+''', ''database'';
	';


	if(@restoreReadOnly = 1)
	begin

		set @sql = '
		alter database '+@db+' set read_only;
		'
	end	
		
	if(@printCommandsOnly = 1)
	begin
		print @sql;
	end
	else
	begin
		print @sql;
		exec(@sql);
	end

	declare @status nvarchar(max);
	set @status = 'Restored backup of ' + @db + ' from backup file:
' + @backupFile + '
taken at ' + convert(nvarchar(20), @backupTime, 100);
	print @status;
end