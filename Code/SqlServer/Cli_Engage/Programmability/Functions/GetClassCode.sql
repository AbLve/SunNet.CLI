GO
/****** Object:  UserDefinedFunction [dbo].[GetClassCode]    Script Date: 10/12/2014 8:47:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION [dbo].[GetClassCode] 
(
	 @schoolYear varchar(5)
)
RETURNS varchar(50)
AS
BEGIN
     DECLARE @count int
	 select @count = count(1) from dbo.Classes where SchoolYear = @schoolYear
	  
	  return 'ECL'+ left(@schoolYear,2)+RIGHT('00000'+CONVERT(VARCHAR(50),(@count+1)),'5')
END
