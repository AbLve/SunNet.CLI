GO
/****** Object:  UserDefinedFunction [dbo].[GetSchoolYear]    Script Date: 10/12/2014 8:45:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 
ALTER FUNCTION [dbo].[GetSchoolYear]
(
)
RETURNS varchar(50)
AS
BEGIN
	 declare @schoolYear varchar(10)
	  
			DECLARE @fullDate  varchar(6)
			DECLARE @year int
			DECLARE @month int
			DECLARE @day int

			select  @fullDate =  CONVERT(varchar(10), GETDATE(), 12)

			set @year = CONVERT(int,substring(@fulldate,1,2))
			set @month = CONVERT(int,substring(@fulldate,3,2))
			set @day = CONVERT(int,substring(@fulldate,5,2))
	if(@month >=8)
	set @schoolYear = convert(varchar(2),@year)+'-'+ convert(varchar(2),(@year+1))
	else
	set @schoolYear = convert(varchar(2),(@year-1))+'-'+ convert(varchar(2),@year)
	
	 RETURN @schoolYear

END
