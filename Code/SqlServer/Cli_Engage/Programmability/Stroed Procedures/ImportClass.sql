/****** Object:  StoredProcedure [dbo].[ImportClass]    Script Date: 2015/1/29 9:29:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[ImportClass]
	@CommunityId int,
	@SchoolId int,
	@ClassroomId int,
	@TeacherUserId int,
	@Teacher_TEA_ID VARCHAR(50) ,
	@Class_Day_Type TINYINT,
	@ClassId int output
AS
BEGIN
--	schoolYear and classCode
	DECLARE @schoolYear varchar(5),@classCode VARCHAR(32)
	SET @schoolYear=dbo.GetSchoolYear()
	SET @classCode=dbo.GetClassCode(@schoolYear)
--	className,teacherLastName 
	DECLARE @className VARCHAR(150),@lName varchar(140)
	SELECT @lName=u.LastName  FROM dbo.Teachers t LEFT JOIN dbo.Users u ON t.UserId = u.ID WHERE t.UserId=@TeacherUserId 
	IF(@Class_Day_Type=1)
		SET @className=@lName+'''s AM Class'
	ELSE IF(@Class_Day_Type=2)
		SET @className=@lName+'''s PM Class'
	ELSE
		SET @className=@lName+'''s Class'
--insert Class
	INSERT INTO dbo.Classes
	        ( CommunityId ,
	          SchoolId ,
	          ClassroomId ,
	          ClassId ,
	          Name ,
	          Status ,
	          StatusDate ,
	          SchoolYear ,
	          IsSameAsSchool ,
	          AtRiskPercent ,
	          DayType ,
	          CurriculumId ,
	          CurriculumOther ,
	          SupplementalCurriculumId ,
	          SupplementalCurriculumOther ,
	          MonitoringToolId ,
	          MonitoringToolOther ,
	          UsedEquipment ,
	          EquipmentNumber ,
	          ClassType ,			  
	          Notes ,
	          CreatedOn ,
	          UpdatedOn,
			  Previous_Teacher_TEA_ID,
			  Classlevel,
			  LeadTeacherId,
			  playgroundId,
			  TypeOfClass
	        )
	VALUES  ( @CommunityId , -- CommunityId - int
	          @SchoolId , -- SchoolId - int
	          @ClassroomId , -- ClassroomId - int
	          @classCode , -- ClassId - varchar(32)
	          @className , -- Name - varchar(150)
	          1 , -- Status - tinyint
	          GETDATE() , -- StatusDate - datetime
	          @schoolYear , -- SchoolYear - varchar(5)
	          0 , -- IsSameAsSchool - bit
	          0 , -- AtRiskPercent - int
	          @Class_Day_Type , -- DayType - tinyint
	          0 , -- CurriculumId - int
	          '' , -- CurriculumOther - varchar(150)
	          0 , -- SupplementalCurriculumId - int
	          '' , -- SupplementalCurriculumOther - varchar(150)
	          0 , -- MonitoringToolId - int
	          '' , -- MonitoringToolOther - varchar(150)
	          0 , -- UsedEquipment - tinyint
	          '' , -- EquipmentNumber - varchar(150)
	          0 , -- ClassType - tinyint
	          '' , -- Notes - varchar(600)
	          GETDATE() , -- CreatedOn - datetime
	          GETDATE() , -- UpdatedOn - datetime
			  @Teacher_TEA_ID,
			  '',
			  0,
			  0,
			  0
	        )			
	
	SELECT @ClassId= SCOPE_IDENTITY();

	RETURN @ClassId;
	    
END

	
 
