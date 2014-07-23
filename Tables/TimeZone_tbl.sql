create table TimeZone_tbl
(
	timeZoneId smallint not null
,	windowsTimeZoneId varchar(200) not null
,	displayName varchar(200) not null
,	baseUtcOffsetInHours decimal(9,3) not null
,	standardName varchar(200) not null
,	daylightName varchar(200) not null
,	constraint PK_TimeZone_tbl primary key(timeZoneId)
)
GO
create nonclustered index IX_windowsTimeZoneId on dbo.TimeZone_tbl(windowsTimeZoneId)