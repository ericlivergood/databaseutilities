create function dbo.GetLocalTimeFromUTC_fn
(
	@timeToConvert datetime
,	@toTimeZoneId smallint
)
returns datetime
with schemabinding
as
begin
	return(
		select dbo.GetLocalTime_fn(@timeToConvert, @toTimeZoneId, tz.timeZoneId)
		from dbo.TimeZone_tbl tz
		where tz.windowsTimeZoneId = 'UTC'
	)

end