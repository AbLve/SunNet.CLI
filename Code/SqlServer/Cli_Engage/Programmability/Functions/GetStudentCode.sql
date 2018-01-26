GO
/****** Object:  UserDefinedFunction [dbo].[GetStudentCode]    Script Date: 10/12/2014 8:48:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
ALTER FUNCTION [dbo].[GetStudentCode] 
(
	 @schoolYear varchar(5)
)
RETURNS varchar(50)
AS
BEGIN
     DECLARE @count int
	 select @count = count(1) from Students where SchoolYear = @schoolYear
	  
	  return 'ESU'+ CONVERT(varchar(2), GETDATE(), 12)+RIGHT('0000000'+CONVERT(VARCHAR(50),(@count+1)),'7')
END

