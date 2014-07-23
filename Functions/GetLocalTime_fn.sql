create function dbo.GetLocalTime_fn
(
	@timeToConvert datetime
,	@toTimeZoneId smallint
,	@fromTimeZoneId smallint
)
returns datetime
with schemabinding
as
begin
	return(
		select 
			dateadd(minute, -1*o.offsetInMinutes, @timeToConvert)
		from dbo.TimeZoneToTimeZoneOffset_tbl o
		where o.fromTimeZoneId = @fromTimeZoneId
		and o.toTimeZoneId = @toTimeZoneId
		and @timeToConvert >= startDate 
		and @timeToConvert < endDate
	
	)

end