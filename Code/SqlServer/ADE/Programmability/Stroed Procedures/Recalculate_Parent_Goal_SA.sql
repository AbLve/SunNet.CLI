GO

/****** Object:  StoredProcedure [dbo].[Recalculate_Parent_Goal_SA]    Script Date: 12/15/2014 17:32:09 ******/
IF EXISTS(SELECT 1 FROM sys.objects WHERE object_id = object_id('Recalculate_Parent_Goal_SA'))
DROP PROCEDURE [dbo].[Recalculate_Parent_Goal_SA]
GO

/****** Object:  StoredProcedure [dbo].[Recalculate_Parent_Goal_SA]    Script Date: 12/15/2014 17:32:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Jack
-- Create date: 2014_12_15
-- Description:	Re calc parent measure's goal of StudentMeasure
-- =============================================
CREATE PROCEDURE [dbo].[Recalculate_Parent_Goal_SA]
	-- Add the parameters for the stored procedure here
	@SaId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @AssessmentId int ;
	set @AssessmentId = 0;
	select @AssessmentId = AssessmentId from StudentAssessments where ID = @SaId ; 

	if @AssessmentId > 0
	begin
		select distinct ParentId into #Parents 
		from Measures 
		where AssessmentId = @AssessmentId and ParentId > 1

		--SELECT * FROM #Parents ; 

		select MeasureId = ID, ParentId into #Measures 
		from Measures 
		where AssessmentId = @AssessmentId and ParentId > 1 AND TotalScored = 1

		--SELECT * FROM #Measures ; 

		Update StudentMeasures set Goal = (SELECT SUM(CASE Goal WHEN -1 THEN 0 ELSE Goal END) 
											FROM StudentMeasures SM WHERE SM.SAId = @SaId 
												AND EXISTS(SELECT 1 FROM #Measures M WHERE M.MeasureId = SM.MeasureId AND M.ParentId = [StudentMeasures].MeasureId) AND SM.[Status] = 3)
		FROM StudentMeasures
		where SAId = @SaId
		and exists (select 1 from #Parents P where P.ParentId = [StudentMeasures].MeasureId ) 
		and exists (SELECT 1 FROM StudentMeasures SM WHERE SM.SAId = @SaId AND EXISTS(SELECT 1 FROM #Measures M WHERE M.MeasureId = SM.MeasureId AND M.ParentId = [StudentMeasures].MeasureId) AND SM.[Status] = 3); 
		
		Update StudentMeasures set Goal = -1 
		FROM StudentMeasures 
		where SAId = @SaId 
		and exists (select 1 from #Parents P where P.ParentId = [StudentMeasures].MeasureId ) 
		and not exists (SELECT 1 FROM StudentMeasures SM WHERE SM.SAId = @SaId AND EXISTS(SELECT 1 FROM #Measures M WHERE M.MeasureId = SM.MeasureId AND M.ParentId = [StudentMeasures].MeasureId) AND SM.[Status] = 3); 

		drop table #Parents
		drop table #Measures
	end
END

GO


