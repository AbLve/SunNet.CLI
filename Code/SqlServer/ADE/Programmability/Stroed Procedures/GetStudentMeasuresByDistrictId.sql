GO

/****** Object:  StoredProcedure [dbo].[GetStudentMeasuresByDistrictId]    Script Date: 12/10/2014 10:11:18 ******/
IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = object_id('GetStudentMeasuresByDistrictId'))
DROP PROCEDURE [dbo].[GetStudentMeasuresByDistrictId]
GO

/****** Object:  StoredProcedure [dbo].[GetStudentMeasuresByDistrictId]    Script Date: 12/10/2014 10:11:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		Jack
-- Create date: 2014-12-06
-- Description:	Get Student Measure Records by DistrictId
-- =============================================
CREATE PROCEDURE [dbo].[GetStudentMeasuresByDistrictId]
	-- Add the parameters for the stored procedure here
	@AssessmentId int,
	@SchoolYear nvarchar(10),
	@DistrictId int,
	@Waves nvarchar(10)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DECLARE @Measures table(ID int,AssessmentId int,Name nvarchar(max),ApplyToWave nvarchar(max),ParentId int,Sort int,TotalScored bit,Children INT);
            INSERT INTO @Measures
            SELECT ID,AssessmentId ,Name ,ApplyToWave,ParentId ,Sort,TotalScored ,Children = (CASE ParentID WHEN 1 THEN (SELECT COUNT(ID) FROM Measures M2 WHERE M2.ParentId = M1.ID) ELSE 0 END)
            FROM Measures M1
            WHERE AssessmentId  = @AssessmentId AND IsDeleted = 0 AND Status = 1 ;

            SELECT SA.SchoolId,SA.StudentId, SA.Wave, SM.MeasureId, Goal = CASE M.TotalScored WHEN 1 THEN SM.Goal ELSE 0 END,SM.Benchmark  
			FROM [dbo].[V_StudentMeasures] SM LEFT JOIN [dbo].[StudentAssessments] SA ON SA.ID = SM.SAId 
            Inner Join Measures M on SM.MeasureId = M.ID 
            WHERE  SA.AssessmentId  = @AssessmentId 
			AND SA.SchoolYear = @SchoolYear
            AND SA.CDId = @DistrictId 
			AND CHARINDEX(CAST(SA.Wave AS VARCHAR),@Waves,0) > 0
			AND EXISTS (SELECT 1 FROM [dbo].[Cli_Engage__Schools] S WHERE SA.SchoolId = S.ID AND S.[Status] = 1)
            AND SM.Status = 3 
			

            UNION ALL
            SELECT SA.SchoolId,SA.StudentId, SA.Wave, SM.MeasureId, SM.Goal,SM.Benchmark  
			FROM  [dbo].[V_StudentMeasures] SM LEFT JOIN [dbo].[StudentAssessments] SA ON SA.ID = SM.SAId
            WHERE  AssessmentId  = @AssessmentId 
            AND EXISTS (SELECT 1 FROM @Measures ALL_MEASURES WHERE SM.MeasureId = ALL_MEASURES.ID AND ALL_MEASURES.Children>0)
            AND SA.SchoolYear = @SchoolYear
			AND SA.CDId = @DistrictId
			AND CHARINDEX(CAST(SA.Wave AS VARCHAR),@Waves,0) > 0
			AND EXISTS (SELECT 1 FROM [dbo].[Cli_Engage__Schools] S WHERE SA.SchoolId = S.ID AND S.[Status] = 1) ; 

END



GO

