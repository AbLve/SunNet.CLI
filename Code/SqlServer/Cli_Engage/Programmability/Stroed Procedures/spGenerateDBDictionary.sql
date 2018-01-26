GO
/****** Object:  StoredProcedure [dbo].[spGenerateDBDictionary]    Script Date: 10/12/2014 8:40:54 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:JOHIR
-- Create date: 01/12/2012
-- Description:	GENERATE DATA DICTIONARY FROM SQL SERVER
-- =============================================
ALTER proc [dbo].[spGenerateDBDictionary] 
AS
BEGIN

select a.name [Table],b.name [Attribute],c.name [DataType],b.isnullable [Allow Nulls?],CASE WHEN 
d.name is null THEN 0 ELSE 1 END [PKey?],
CASE WHEN e.parent_object_id is null THEN 0 ELSE 1 END [FKey?],CASE WHEN e.parent_object_id 
is null THEN '-' ELSE g.name  END [Ref Table],
CASE WHEN h.value is null THEN '-' ELSE h.value END [Description]
from sysobjects as a
join syscolumns as b on a.id = b.id
join systypes as c on b.xtype = c.xtype 
left join (SELECT  so.id,sc.colid,sc.name 
      FROM    syscolumns sc
      JOIN sysobjects so ON so.id = sc.id
      JOIN sysindexkeys si ON so.id = si.id 
                    and sc.colid = si.colid
      WHERE si.indid = 1) d on a.id = d.id and b.colid = d.colid
left join sys.foreign_key_columns as e on a.id = e.parent_object_id and b.colid = e.parent_column_id    
left join sys.objects as g on e.referenced_object_id = g.object_id  
left join sys.extended_properties as h on a.id = h.major_id and b.colid = h.minor_id
where a.type = 'U' order by a.name

END