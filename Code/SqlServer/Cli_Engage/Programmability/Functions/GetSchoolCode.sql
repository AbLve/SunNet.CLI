GO
/****** Object:  UserDefinedFunction [dbo].[GetSchoolCode]    Script Date: 10/12/2014 8:45:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [dbo].[GetSchoolCode] 
(
	 @schoolYear varchar(5)
)
RETURNS varchar(50)
AS
BEGIN
     DECLARE @count int
	 select @count = count(1) from Schools where SchoolYear = @schoolYear
	  
	  return 'EDD'+left(@schoolYear,2)+RIGHT('00000'+CONVERT(VARCHAR(50),(@count+1)),'5')
END