@set _date=%date%
@set _time=%time:~0,8%
@set _year=%_date:~10,4%

@rem reads build number from file
@set /p _build=< build 
@rem increments build number
@set /a _build=%_build%+1 
@rem saves build number into file
@echo %_build%>build

@rem fills resulting files
@echo %_build%>Views/Shared/Build.cshtml
@echo %_year%>Views/Shared/Year.cshtml
@echo %date% %_time%>Views/Shared/Date.cshtml

@rem cleans variables
@set _build=
@set _date=
@set _time=
@set _year=