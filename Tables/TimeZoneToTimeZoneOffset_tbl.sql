create table TimeZoneToTimeZoneOffset_tbl
(
	fromTimeZoneId smallint not null
,	toTimeZoneId smallint not null
,	startDate smalldatetime not null
,	endDate smalldatetime not null
,	offsetInMinutes smallint not null
,	constraint PK_TimeZoneToTimeZoneOffset_tbl primary key(fromTimeZoneId, toTimeZoneId, startDate)
);
