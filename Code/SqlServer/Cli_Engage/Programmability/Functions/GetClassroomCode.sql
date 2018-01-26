GO
/****** Object:  UserDefinedFunction [dbo].[GetClassroomCode]    Script Date: 10/12/2014 8:46:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [dbo].[GetClassroomCode] 
(
	 @schoolYear varchar(5)
)
RETURNS varchar(50)
AS
BEGIN
     DECLARE @count int
	 select @count = count(1) from Classrooms where SchoolYear = @schoolYear
	  
	  return 'ECR'+ left(@schoolYear,2)+RIGHT('00000'+CONVERT(VARCHAR(50),(@count+1)),'5')
END