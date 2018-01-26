
/****** Object:  StoredProcedure [dbo].[ImportDataVerify]    Script Date: 2014/10/17 16:21:54 ******/
/****** Object:  StoredProcedure [dbo].[ImportClass]    Script Date: 2014/10/10 23:52:38 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ImportDataVerify]'))
	DROP PROCEDURE [dbo].[ImportDataVerify]
GO

/****** Object:  StoredProcedure [dbo].[ImportDataVerify]    Script Date: 2014/10/17 16:21:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[ImportDataVerify]
	@ID INT
AS
BEGIN
	UPDATE DataGroups SET SchoolFail=(SELECT COUNT(*) FROM dbo.DataSchools WHERE GroupId=@ID AND Status=4) 
		,SchoolSuccess=(SELECT COUNT(*) FROM dbo.DataSchools WHERE GroupId=@ID AND Status=3) 
		,TeacherFail = ( SELECT COUNT(0) FROM (SELECT Teacher_Number FROM dbo.DataStudents WHERE GroupId=@ID AND Status=4  GROUP BY Teacher_Number ) AS A)
		,TeacherSuccess = ( SELECT COUNT(0) FROM (SELECT Teacher_Number FROM dbo.DataStudents WHERE GroupId=@ID AND Status=3  GROUP BY Teacher_Number ) AS A)
		,StudentFail = (SELECT COUNT(*) FROM dbo.DataStudents WHERE GroupId=@ID and Student_TSDS_ID !=''   AND Status=4) 
		,StudentSuccess = (SELECT COUNT(*) FROM dbo.DataStudents WHERE GroupId=@ID and Student_TSDS_ID !=''   AND Status=3) 
		WHERE ID = @ID

	IF EXISTS (SELECT * FROM DataGroups WHERE ID = @id AND (SchoolTotal= SchoolSuccess) AND (TeacherTotal= TeacherSuccess) AND (StudentTotal= StudentSuccess)) 
		UPDATE DataGroups SET Status=3 WHERE ID = @id
	ELSE 
		UPDATE DataGroups SET Status=1 WHERE ID = @id
END

GO


