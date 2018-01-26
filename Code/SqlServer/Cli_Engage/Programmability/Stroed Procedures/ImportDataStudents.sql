  
/****** Object:  StoredProcedure [dbo].[ImportDataStudents]    Script Date: 2015/5/4 16:41:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
  -- Create date: <Create Date="",,>
    -- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[ImportDataStudents]
	@DataStudentId INT,
	@School_TEA_ID VARCHAR(100),
	@GroupId INT ,
	@InvitationEmail INT,
	@CommunityId INT,
	@SchoolId INT,
	@CreatedBy INT
AS
BEGIN
	
	BEGIN TRANSACTION;
	BEGIN TRY

	--Teacher
    DECLARE @Teacher_Number         VARCHAR(50)   
    DECLARE @Teacher_First_Name     VARCHAR(50)   
    DECLARE @Teacher_Middle_Name    VARCHAR(50)   
    DECLARE @Teacher_Last_Name      VARCHAR(50) 
	DECLARE @Teacher_Previous_Last_Name         VARCHAR(50)   

    DECLARE @Teacher_Phone_Number      VARCHAR(50)   
    DECLARE @Teacher_Phone_Type    TINYINT
    DECLARE @Teacher_Primary_Email      VARCHAR(50)
	DECLARE @Teacher_Secondary_Email   VARCHAR(50)  
	DECLARE @New_Teacher_Number    VARCHAR(50)  
	--Class
	DECLARE @Student_Class_Day_Type   TINYINT

	--Classroom
	DECLARE @ClassroomId INT
	DECLARE @classroomCode varchar(50)
	--Student
	DECLARE @Student_TSDS_ID   VARCHAR(50)   
	DECLARE @Student_First_Name   VARCHAR(50)   
	DECLARE @Student_Middle_Name   VARCHAR(50)   
	DECLARE @Student_Last_Name   VARCHAR(50)   
	DECLARE @Student_Birth_Date   DATETIME
	DECLARE @Student_Gender   TINYINT
	DECLARE @Student_Ethnicity   VARCHAR(100)	 

		SELECT 
			 @Teacher_Number=Teacher_Number
			,@Teacher_First_Name=Teacher_First_Name
			,@Teacher_Middle_Name= ISNULL(Teacher_Middle_Name,'')
			,@Teacher_Last_Name=Teacher_Last_Name
			,@Teacher_Previous_Last_Name = ISNULL(Teacher_Previous_Last_Name,'')
			,@Teacher_Phone_Number=Teacher_Phone_Number
			,@Teacher_Phone_Type =Teacher_Phone_Type
			,@Teacher_Primary_Email=Teacher_Primary_Email
			,@Teacher_Secondary_Email= ISNULL(Teacher_Secondary_Email ,'')
			,@New_Teacher_Number = ISNULL(New_Teacher_Number,'')
			,@Student_Class_Day_Type = Student_Class_Day_Type
			,@Student_TSDS_ID = Student_TSDS_ID
			,@Student_First_Name = Student_First_Name
			,@Student_Middle_Name = ISNULL(Student_Middle_Name,'')
			,@Student_Last_Name = Student_Last_Name
			,@Student_Birth_Date = Student_Birth_Date 
			,@Student_Gender = Student_Gender
			,@Student_Ethnicity = Student_Ethnicity
			FROM DataStudents WHERE ID=@DataStudentId	

		 
	DECLARE @schoolYear varchar(10)
	SET @schoolYear = dbo.GetSchoolYear()
		    
	---Teacher
	DECLARE @TeacherId INT
	DECLARE @TeacherUserId INT
	DECLARE @ClassId   INT
	DECLARE @TeacherNumber VARCHAR(100)
		 
	 IF NOT EXISTS (SELECT * FROM Teachers WHERE TeacherNumber = @Teacher_Number AND SchoolId =@SchoolId) 
			SELECT @TeacherId=0 ,@TeacherUserId =0
	  ELSE SELECT @TeacherUserId = UserId,@TeacherId=ID   FROM Teachers WHERE TeacherNumber = @Teacher_Number AND SchoolId =@SchoolId

	IF(@TeacherId = 0)
		BEGIN --insert Teachers and insert Class
			--insert Teachers

			INSERT INTO Users 
			(
				   [Role]
				  ,[GoogleId]
				  ,[FirstName]
				  ,[MiddleName]
				  ,[LastName]
				  ,[PreviousLastName]
				  ,[Status]
				  ,[StatusDate]
				  ,[PrimaryEmailAddress]
				  ,[SecondaryEmailAddress]
				  ,[PrimaryPhoneNumber]
				  ,[PrimaryNumberType]
				  ,[SecondaryPhoneNumber]
				  ,[SecondaryNumberType]
				  ,[FaxNumber]
				  ,[CreatedOn]
				  ,[UpdatedOn]
				  ,[IsDeleted]
				  ,[Sponsor]
				  ,[InvitationEmail]
				  ,[EmailExpireTime]
				  ,[Notes]
			)
			VALUES
			(
				145,
				'',
				@Teacher_First_Name,
				@Teacher_Middle_Name,
				@Teacher_Last_Name,
				@Teacher_Previous_Last_Name,
				1,
				GETDATE(),
				@Teacher_Primary_Email,
				@Teacher_Secondary_Email,
				@Teacher_Phone_Number,
				@Teacher_Phone_Type,
				'',
				'',
				'',
				GETDATE(),
				GETDATE(),
				0,
				@CreatedBy,
				(case when @InvitationEmail=1 then 2 else 1 end),
				GETDATE(),
				'BatchImport'
			)

			SELECT @TeacherUserId = SCOPE_IDENTITY()

			INSERT INTO Teachers 
			(
				   [UserId]
				  ,[TeacherId]
				  ,[CommunityId]
				  ,[SchoolId]
				  ,[SchoolYear]
				  ,[BirthDate]
				  ,[Gender]
				  ,[HomeMailingAddress]
				  ,[HomeMailingAddress2]
				  ,[City]
				  ,[CountyId]
				  ,[StateId]
				  ,[Zip]
				  ,[Ethnicity]
				  ,[EthnicityOther]
				  ,[PrimaryLanguageId]
				  ,[PrimaryLanguageOther]
				  ,[SecondaryLanguageId]
				  ,[SecondaryLanguageOther]
				  ,[TotalTeachingYears]
				  ,[AgeGroupOther]
				  ,[CurrentAgeGroupTeachingYears]
				  ,[TeacherType]
				  ,[TeacherTypeOther]
				  ,[PDOther]
				  ,[EducationLevel]
				  ,[CoachId]
				  ,[CoachAssignmentWay]
				  ,[CoachAssignmentWayOther]
				  ,[ECIRCLEAssignmentWay]
				  ,[ECIRCLEAssignmentWayOther]
				  ,[YearsInProjectId]
				  ,[CoreAndSupplemental]
				  ,[CoreAndSupplemental2]
				  ,[CoreAndSupplemental3]
				  ,[CoreAndSupplemental4]
				  ,[VendorCode]
				  ,[CoachingHours]
				  ,[EmployedBy]
				  ,[CLIFundingId]
				  ,[MediaRelease]
				  ,[IsAssessmentEquipment]
				  ,[TeacherNotes]
				  ,[TeacherNumber]
			)
			VALUES
			(
				@TeacherUserId,
				dbo.GetTeacherCode(dbo.GetSchoolYear()),
				@CommunityId,
				@SchoolId,
				dbo.GetSchoolYear(),
				'01/01/1753',
				0,
				'',
				'',
				'',
				0,
				0,
				'',
				0,
				'',
				0,
				'',	
				0,		
				'',		
				0,		
				'',		
				0,		
				0,		
				'',		
				'',		
				0,		
				0,		
				0,		
				'',		
				0,		
				'',		
				0,		
				'',		
				'',		
				'',		
				'',		
				0,		
				0.00,	
				0,		
				0,		
				0,		
				0,		
				'',		
				@Teacher_Number
			)
			
			SELECT @TeacherId = SCOPE_IDENTITY()		  
					   
		   SET @classroomCode = dbo.GetClassroomCode(@schoolYear)
		   INSERT INTO [dbo].[Classrooms]
			   ([ClassroomId]
			   ,[CommunityId]
			   ,[SchoolId]
			   ,[Name]
			   ,[Status]
			   ,[StatusDate]
			   ,[SchoolYear]
			   ,[InterventionStatus]
			   ,[InterventionOther]
			   ,[FundingId]
			   ,[KitId]
			   ,[KitUpdatedOn]
			   ,[FcNeedKitId]
			   ,[FcFundingId]
			   ,[Part1KitId]
			   ,[Part1KitUpdatedOn]
			   ,[Part1NeedKitId]
			   ,[Part1FundingId]
			   ,[Part2KitId]
			   ,[Part2KitUpdatedOn]
			   ,[Part2NeedKitId]
			   ,[Part2FundingId]
			   ,[StartupKitId]
			   ,[StartupKitUpdatedOn]
			   ,[StartupNeedKitId]
			   ,[StartupKitFundingId]
			   ,[CurriculumId]
			   ,[CurriculumUpdatedOn]
			   ,[NeedCurriculumId]
			   ,[NeedCurriculumUpdatedOn]
			   ,[CurriculumFundingId]
			   ,[DevelopingTalkersKitId]
			   ,[DevelopingTalkersKitUpdatedOn]
			   ,[DevelopingTalkersNeedKitId]
			   ,[DevelopingTalkerKitFundingId]
			   ,[FccKitId]
			   ,[FccKitUpdatedOn]
			   ,[FccNeedKitId]
			   ,[FccKitFundingId]
			   ,[InternetSpeed]
			   ,[InternetType]
			   ,[WirelessType]
			   ,[IsUsingInClassroom]
			   ,[ComputerNumber]
			   ,[IsInteractiveWhiteboard]
			   ,[MaterialsNotes]
			   ,[TechnologyNotes]
			   ,[CreatedOn]
			   ,[UpdatedOn])
		 VALUES
			   (@classroomCode
			   ,@CommunityId
			   ,@SchoolId
			   ,@Teacher_Last_Name + '''s Classroom'
			   ,1
			   ,getdate()
			   ,@schoolYear
			   ,0
			   ,''
			   ,0
			   ,0
			   ,'1753-01-01'
			   ,0
			   ,0
			   ,0
			   ,'1753-01-01'
			   ,0
			   ,0
			   ,0
			   ,'1753-01-01'
			   ,0
			   ,0
			   ,0
			   ,'1753-01-01'
			   ,0
			   ,0
			   ,0
			   ,'1753-01-01'
			   ,0
			   ,'1753-01-01'
			   ,0
			   ,0
			   ,'1753-01-01'
			   ,0
			   ,0
			   ,0
			   ,'1753-01-01'
			   ,0
			   ,0
			   ,0
			   ,0
			   ,0
			   ,0
			   ,0
			   ,0
			   ,''
			   ,''
			   ,GETDATE()
			   ,GETDATE())

		   SELECT @ClassroomId = SCOPE_IDENTITY() 	  

			---INSERT CLASS
			EXECUTE ImportClass @CommunityId,@SchoolId,@ClassroomId,@TeacherUserId,@Teacher_Number,@Student_Class_Day_Type,  @ClassId OUTPUT 

			INSERT INTO dbo.TeacherClassRelations
			         ( TeacherId, ClassId )
			 VALUES  ( @TeacherId, 
			           @ClassId  
			           )
			
		END --- Inser Teacher  end
	ELSE BEGIN

		IF NOT EXISTS (SELECT * FROM Classes WHERE Previous_Teacher_TEA_ID =@TeacherNumber AND SchoolId=@SchoolId)
			SELECT @ClassId=0 
	    ELSE SELECT @ClassId = Id   FROM Classes WHERE Previous_Teacher_TEA_ID =@TeacherNumber  AND SchoolId=@SchoolId


		IF(@ClassId =0)
		BEGIN			
			 IF NOT EXISTS (SELECT * FROM TeacherClassRelations WHERE TeacherId = @TeacherId)
			 SELECT @ClassId=0 
	         ELSE SELECT @ClassId = ClassId   FROM TeacherClassRelations WHERE TeacherId = @TeacherId
		END	
		
		IF(@ClassId = 0)
		BEGIN

		   SET @classroomCode = dbo.GetClassroomCode(@schoolYear)
		   INSERT INTO [dbo].[Classrooms]
			   ([ClassroomId]
			   ,[CommunityId]
			   ,[SchoolId]
			   ,[Name]
			   ,[Status]
			   ,[StatusDate]
			   ,[SchoolYear]
			   ,[InterventionStatus]
			   ,[InterventionOther]
			   ,[FundingId]
			   ,[KitId]
			   ,[KitUpdatedOn]
			   ,[FcNeedKitId]
			   ,[FcFundingId]
			   ,[Part1KitId]
			   ,[Part1KitUpdatedOn]
			   ,[Part1NeedKitId]
			   ,[Part1FundingId]
			   ,[Part2KitId]
			   ,[Part2KitUpdatedOn]
			   ,[Part2NeedKitId]
			   ,[Part2FundingId]
			   ,[StartupKitId]
			   ,[StartupKitUpdatedOn]
			   ,[StartupNeedKitId]
			   ,[StartupKitFundingId]
			   ,[CurriculumId]
			   ,[CurriculumUpdatedOn]
			   ,[NeedCurriculumId]
			   ,[NeedCurriculumUpdatedOn]
			   ,[CurriculumFundingId]
			   ,[DevelopingTalkersKitId]
			   ,[DevelopingTalkersKitUpdatedOn]
			   ,[DevelopingTalkersNeedKitId]
			   ,[DevelopingTalkerKitFundingId]
			   ,[FccKitId]
			   ,[FccKitUpdatedOn]
			   ,[FccNeedKitId]
			   ,[FccKitFundingId]
			   ,[InternetSpeed]
			   ,[InternetType]
			   ,[WirelessType]
			   ,[IsUsingInClassroom]
			   ,[ComputerNumber]
			   ,[IsInteractiveWhiteboard]
			   ,[MaterialsNotes]
			   ,[TechnologyNotes]
			   ,[CreatedOn]
			   ,[UpdatedOn])
		 VALUES
			   (@classroomCode
			   ,@CommunityId
			   ,@SchoolId
			   ,@Teacher_Last_Name + '''s Classroom'
			   ,1
			   ,getdate()
			   ,@schoolYear
			   ,0
			   ,''
			   ,0
			   ,0
			   ,'1753-01-01'
			   ,0
			   ,0
			   ,0
			   ,'1753-01-01'
			   ,0
			   ,0
			   ,0
			   ,'1753-01-01'
			   ,0
			   ,0
			   ,0
			   ,'1753-01-01'
			   ,0
			   ,0
			   ,0
			   ,'1753-01-01'
			   ,0
			   ,'1753-01-01'
			   ,0
			   ,0
			   ,'1753-01-01'
			   ,0
			   ,0
			   ,0
			   ,'1753-01-01'
			   ,0
			   ,0
			   ,0
			   ,0
			   ,0
			   ,0
			   ,0
			   ,0
			   ,''
			   ,''
			   ,GETDATE()
			   ,GETDATE())
		   SELECT @ClassroomId = SCOPE_IDENTITY() 	
		     
			-----INSERT CLASS
			EXECUTE ImportClass @CommunityId,@SchoolId,@ClassroomId,@TeacherUserId,@Teacher_Number,@Student_Class_Day_Type,  @ClassId OUTPUT 

			INSERT INTO dbo.TeacherClassRelations
				        ( TeacherId, ClassId )
				VALUES  ( @TeacherId, 
				        @ClassId  
				        )
		END	
	END


	---Student
	DECLARE @parentCode int 
	DECLARE @studentCode varchar(12)
	SET @studentCode = dbo.GetStudentCode(@schoolYear)
	DECLARE @RandMin int,@RandMax int
	set @RandMin=10000000
	set @RandMax=19999999
	SET @parentCode =(
	SELECT  TOP 1 RANDVALUE=ABS(CHECKSUM(NEWID()))%(1+@RandMax-@RandMin)+@RandMin
	FROM sys.objects T1,sys.objects T2)

	DECLARE @StudentId int

	IF NOT EXISTS (SELECT * FROM Students WHERE LocalStudentID = @Student_TSDS_ID AND  SchoolId =@SchoolId)
	SELECT @StudentId=0 
	ELSE SELECT @StudentId = ID   FROM Students WHERE LocalStudentID = @Student_TSDS_ID AND  SchoolId =@SchoolId


	IF(@StudentId = 0)
	BEGIN
		
		DECLARE @E1 TINYINT
		DECLARE @E2 VARCHAR(500)
		SET @E1 =0
		SET @E2 = ''
		SET @E1 = 
		   CASE   @Student_Ethnicity  WHEN 'African American' THEN   1
									  WHEN 'Alaskan' THEN   2
									  WHEN 'Native American' THEN   3
									  WHEN 'Indian' THEN   4
									  WHEN 'Asian' THEN   5
									  WHEN 'Caucasian' THEN   6
									  WHEN 'Hispanic' THEN   7
									  WHEN 'Multiracial' THEN   8
									  WHEN 'Other' THEN   9
		                              ELSE   0
		    END
		IF(@E1 = 9)
	       SET	@E2 =@Student_Ethnicity

			INSERT INTO [dbo].[Students]
           ([CommunityId]
           ,[SchoolId]
           ,[StudentId]
           ,[FirstName]
           ,[MiddleName]
           ,[LastName]
           ,[Status]
           ,[StatusDate]
           ,[SchoolYear]
           ,[BirthDate]
           ,[Gender]
           ,[Ethnicity]
           ,[EthnicityOther]
           ,[PrimaryLanguageId]
           ,[PrimaryLanguageOther]
           ,[SecondaryLanguageId]
           ,[SecondaryLanguageOther]
           ,[IsSendParent]
           ,[IsMediaRelease]
           ,[Notes]
           ,[CreatedOn]
           ,[UpdatedOn]
           ,[ParentCode]
           ,[AssessmentLanguage]
           ,[LocalStudentID]
		   ,TSDSStudentID
		   ,GradeLevel)
     VALUES
           (@CommunityId
           ,@SchoolId
           ,@studentCode
           ,@Student_First_Name
           ,@Student_Middle_Name
           ,@Student_Last_Name
           ,1
           ,GETDATE()
           ,@schoolYear
           ,@Student_Birth_Date
           ,@Student_Gender
           ,@E1
           ,@E2
           ,0
           ,''
           ,0
           ,''
           ,0
           ,2
           ,''
           ,GETDATE()
           ,GETDATE()
           ,CONVERT(VARCHAR(8),@parentCode)
           ,3
           ,@Student_TSDS_ID
		   ,'' --TSDSStudentID
		   ,0--Grade Level
		   )


		SELECT @StudentId = SCOPE_IDENTITY() 
	IF(@ClassId != 0)
	INSERT INTO StudentClassRelations(StudentId,ClassId)VALUES(@StudentId,@ClassId)

	END

	COMMIT TRANSACTION;   
		UPDATE DataStudents SET Status = 3 WHERE ID  =@DataStudentId
	END TRY
		
	BEGIN CATCH
	ROLLBACK TRANSACTION
		UPDATE DataStudents SET Status = 4 ,Remark = ERROR_MESSAGE()+ ERROR_LINE() WHERE ID  =@DataStudentId
	END CATCH

END
