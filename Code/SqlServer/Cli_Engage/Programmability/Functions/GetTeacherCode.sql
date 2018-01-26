GO
/****** Object:  UserDefinedFunction [dbo].[GetTeacherCode]    Script Date: 10/12/2014 8:48:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [dbo].[GetTeacherCode] 
(
	 @schoolYear varchar(5)
)
RETURNS varchar(50)
AS
BEGIN
    DECLARE @count int
	select @count=count(1) from Teachers where SchoolYear=@schoolYear
	return 'ETE'+ left(@schoolYear,2)+RIGHT('00000'+CONVERT(VARCHAR(50),(@count+1)),'5')
END