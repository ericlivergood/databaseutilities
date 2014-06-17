create view repl_SchemaOptions_vw
as
select SchemaOption, cast(SchemaOption as binary(8)) as HexValue, Description
from (
select 0 as SchemaOption, 'Disables scripting by the Snapshot Agent and uses creation_script.' as Description union all
select 1 as SchemaOption, 'Generates the object creation script (CREATE TABLE, CREATE PROCEDURE, and so on). This value is the default for stored procedure articles.' as Description union all
select 2 as SchemaOption, 'Generates the stored procedures that propagate changes for the article, if defined.' as Description union all
select 4 as SchemaOption, 'Identity columns are scripted using the IDENTITY property.' as Description union all
select 8 as SchemaOption, 'Replicate timestamp columns. If not set, timestamp columns are replicated as binary.' as Description union all
select 16 as SchemaOption, 'Generates a corresponding clustered index. Even if this option is not set, indexes related to primary keys and unique constraints are generated if they are already defined on a published table.' as Description union all
select 32 as SchemaOption, 'Converts user-defined data types (UDT) to base data types at the Subscriber. This option cannot be used when there is a CHECK or DEFAULT constraint on a UDT column, if a UDT column is part of the primary key, or if a computed column references a UDT column. Not supported for Oracle Publishers.' as Description union all
select 64 as SchemaOption, 'Generates corresponding nonclustered indexes. Even if this option is not set, indexes related to primary keys and unique constraints are generated if they are already defined on a published table.' as Description union all
select 128 as SchemaOption, 'Replicates primary key constraints. Any indexes related to the constraint are also replicated, even if options 0x10 and 0x40 are not enabled.' as Description union all
select 256 as SchemaOption, 'Replicates user triggers on a table article, if defined. Not supported for Oracle Publishers.' as Description union all
select 512 as SchemaOption, 'Replicates foreign key constraints. If the referenced table is not part of a publication, all foreign key constraints on a published table are not replicated. Not supported for Oracle Publishers.' as Description union all
select 1024 as SchemaOption, 'Replicates check constraints. Not supported for Oracle Publishers.' as Description union all
select 2048 as SchemaOption, 'Replicates defaults. Not supported for Oracle Publishers.' as Description union all
select 4096 as SchemaOption, 'Replicates column-level collation.' as Description union all
select 8192 as SchemaOption, 'Replicates extended properties associated with the published article source object. Not supported for Oracle Publishers.' as Description union all
select 16384 as SchemaOption, 'Replicates UNIQUE constraints. Any indexes related to the constraint are also replicated, even if options 0x10 and 0x40 are not enabled.' as Description union all
select 32768 as SchemaOption, 'This option is not valid for SQL Server 2005 Publishers.' as Description union all
select 65536 as SchemaOption, 'Replicates CHECK constraints as NOT FOR REPLICATION so that the constraints are not enforced during synchronization.' as Description union all
select 131072 as SchemaOption, 'Replicates FOREIGN KEY constraints as NOT FOR REPLICATION so that the constraints are not enforced during synchronization.' as Description union all
select 262144 as SchemaOption, 'Replicates filegroups associated with a partitioned table or index.' as Description union all
select 524288 as SchemaOption, 'Replicates the partition scheme for a partitioned table.' as Description union all
select 1048576 as SchemaOption, 'Replicates the partition scheme for a partitioned index.' as Description union all
select 2097152 as SchemaOption, 'Replicates table statistics.' as Description union all
select 4194304 as SchemaOption, 'Default Bindings' as Description union all
select 8388608 as SchemaOption, 'Rule Bindings' as Description union all
select 16777216 as SchemaOption, 'Full-text index' as Description union all
select 33554432 as SchemaOption, 'XML schema collections bound to xml columns are not replicated.' as Description union all
select 67108864 as SchemaOption, 'Replicates indexes on xml columns.' as Description union all
select 134217728 as SchemaOption, 'Create any schemas not already present on the subscriber.' as Description union all
select 268435456 as SchemaOption, 'Converts xml columns to ntext on the Subscriber.' as Description union all
select 536870912 as SchemaOption, 'Converts large object data types introduced in SQL Server 2005 to data types supported on earlier versions of Microsoft SQL Server' as Description union all
select 1073741824 as SchemaOption, 'Replicate permissions.' as Description union all
select cast(2147483648 as bigint) as SchemaOption, 'Attempt to drop dependencies to any objects that are not part of the publication.' as Description union all
select cast(4294967296 as bigint) as SchemaOption, 'Use this option to replicate the FILESTREAM attribute if it is specified on varbinary(max) columns. Do not specify this option if you are replicating tables to SQL Server 2005 Subscribers. Replicating tables that have FILESTREAM columns to SQL Server 2000 Subscribers is not supported, regardless of how this schema option is set.' as Description union all
select cast(8589934592 as bigint) as SchemaOption, 'Converts date and time data types (date, time, datetimeoffset, and datetime2) introduced in SQL Server 2008 to data types that are supported on earlier versions of SQL Server' as Description union all
select cast(17179869184 as bigint) as SchemaOption, 'Replicates the compression option for data and indexes' as Description union all
select cast(34359738368 as bigint) as SchemaOption, 'Set this option to store FILESTREAM data on its own filegroup at the Subscriber. If this option is not set, FILESTREAM data is stored on the default filegroup. Replication does not create filegroups; therefore, if you set this option, you must create the filegroup before you apply the snapshot at the Subscriber.' as Description union all
select cast(68719476736 as bigint) as SchemaOption, 'Converts common language runtime (CLR) user-defined types (UDTs) that are larger than 8000 bytes to varbinary(max) so that columns of type UDT can be replicated to Subscribers that are running SQL Server 2005.' as Description union all
select cast(137438953472 as bigint) as SchemaOption, 'Converts the hierarchyid data type to varbinary(max) so that columns of type hierarchyid can be replicated to Subscribers that are running SQL Server 2005.' as Description union all
select cast(274877906944 as bigint) as SchemaOption, 'Replicates any filtered indexes on the table.' as Description union all
select cast(549755813888 as bigint) as SchemaOption, 'Converts the geography and geometry data types to varbinary(max) so that columns of these types can be replicated to Subscribers that are running SQL Server 2005.' as Description union all
select cast(1099511627776 as bigint) as SchemaOption, 'Replicates indexes on columns of type geography and geometry.' as Description union all
select cast(2199023255552 as bigint) as SchemaOption, 'Replicates the SPARSE attribute for columns.' as Description
) t