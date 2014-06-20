CREATE PROCEDURE [dbo].[RestartJob_prc]
(
	@job_id uniqueidentifier = null
,	@job_name sysname = null
)
AS
BEGIN
	if(@job_id is null)
	begin
		select @job_id = j.job_id
		from msdb.dbo.sysjobs j 
		where j.name = @job_name;
	end
	
	exec dbo.StopJob_prc @job_id = @job_id;
	exec dbo.StartJob_prc @job_id = @job_id;
END