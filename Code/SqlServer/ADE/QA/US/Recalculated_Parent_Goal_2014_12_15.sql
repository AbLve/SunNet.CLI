declare @AssessmentId int;
declare @Wave int;

/*
�ı������ֵ�����ֱ�ִ�в�ͬAssessment��ͬWave������
��ΪĿǰֻ��һ��SchoolYear���ԣ�����Ҫ�ò����ˡ�
QAվ���Ѿ�ִ�й���

һ����Ҫִ��������ϣ�
AssessmentId	Wave
9				1
9				2
9				3

10				1
10				2
10				3
*/

set @AssessmentId = 9;
set @Wave =1;

select distinct ParentId into #Parents 
from Measures 
where AssessmentId = @AssessmentId and ParentId > 1

--SELECT * FROM #Parents ; 

select MeasureId = ID, ParentId into #Measures 
from Measures 
where AssessmentId = @AssessmentId and ParentId > 1 AND TotalScored = 1

--SELECT * FROM #Measures ; 

select ROW_NUMBER() OVER( Order by ID) AS IndexId,SaId = SA.ID into #Waiting 
from StudentAssessments SA 
where SA.AssessmentId = @AssessmentId and SA.Wave = @Wave 
--AND SA.StudentId = 27171;

DECLARE @MaxId int;
DECLARE @Index int;
SET @Index = (SELECT MIN(IndexId) FROM #Waiting);
SET @MaxId = (SELECT Max(IndexId) FROM #Waiting);

DECLARE @SaId int;

WHILE(@Index <= @MaxId)
BEGIN 
	PRINT '--' + CAST(@Index AS VARCHAR) + '/' + CAST(@MaxId AS VARCHAR) + '---------------------------------';
	SELECT @SaId = SaId FROM #Waiting WHERE IndexId = @Index ;
	
	Update StudentMeasures set Goal = (SELECT SUM(Goal) FROM StudentMeasures SM WHERE SM.SAId = @SaId AND EXISTS(SELECT 1 FROM #Measures M WHERE M.MeasureId = SM.MeasureId AND M.ParentId = [StudentMeasures].MeasureId) AND SM.[Status] = 3)
	FROM StudentMeasures
	where SAId = @SaId
	and exists (select 1 from #Parents P where P.ParentId = [StudentMeasures].MeasureId ) 
	and exists (SELECT 1 FROM StudentMeasures SM WHERE SM.SAId = @SaId AND EXISTS(SELECT 1 FROM #Measures M WHERE M.MeasureId = SM.MeasureId AND M.ParentId = [StudentMeasures].MeasureId) AND SM.[Status] = 3); 

	Update StudentMeasures set Goal = -1
	FROM StudentMeasures
	where SAId = @SaId
	and exists (select 1 from #Parents P where P.ParentId = [StudentMeasures].MeasureId ) 
	and not exists (SELECT 1 FROM StudentMeasures SM WHERE SM.SAId = @SaId AND EXISTS(SELECT 1 FROM #Measures M WHERE M.MeasureId = SM.MeasureId AND M.ParentId = [StudentMeasures].MeasureId) AND SM.[Status] = 3);

	SET @Index = @Index + 1;
	--SET @Index = @MaxId + 1;
END

drop table #Parents
drop table #Measures
drop table #Waiting