using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Ade;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Ade.Interfaces;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.Students.Model;
using Sunnet.Framework.Core.Base;

/**************************************************************************
 * Developer: 		Jack
 * Computer:		Jackz
 * Domain:			Jackz
 * CreatedOn:		08/11/2014 03:52:57
 * Description:		ItemBaseEntity's IRepository
 * Version History:	Created,08/11/2014 03:52:57
 * 
 * 
 **************************************************************************/
namespace Sunnet.Cli.Impl.Ade
{
    /// <summary>
    /// ItemBaseEntity's Repository
    /// </summary>
    public class ItemBaseRpst : EFRepositoryBase<ItemBaseEntity, int>, IItemBaseRpst
    {
        public ItemBaseRpst(IUnitOfWork unit)
        {
            this.UnitOfWork = unit;
        }

        public bool AdjustOrder(List<int> items)
        {
            var strSql = new StringBuilder();
            var index = 0;
            items.ForEach(x => strSql.AppendFormat("UPDATE [ItemBases] SET [Sort] = {0} WHERE ID = {1} ;", index++, x));
            return this.EFContext.DbContext.Database.ExecuteSqlCommand(strSql.ToString()) >= 0;
        }

        public IList<TxkeaReceptiveItemEntity> GetTxkeaReceptiveItemsForPlayMeasure(List<int> measureIds)
        {
            IList<TxkeaReceptiveItemEntity> list = new List<TxkeaReceptiveItemEntity>();
            if (measureIds == null || measureIds.Count == 0) return list;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select A.* from [TxkeaReceptiveItems] A join ItemBases B ON A.ID =B.ID where Type =12 AND B.MeasureId IN({0}) AND IsDeleted = 0 AND B.Status =1", string.Join(",", measureIds))
               ;
            //不能在此Dispose掉context，因为执行过此方法后，还会用到context
            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            using (SqlConnection conn = new SqlConnection(context.DbContext.Database.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                using (SqlCommand cmd = new SqlCommand(sb.ToString(), conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandTimeout = 20 * 60;  //Second
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new TxkeaReceptiveItemEntity()
                            {
                                ID = (int)reader["ID"],
                                ImageSequence = (OrderType)reader["ImageSequence"]
                            });
                        }
                    }
                }
            }
            return list;
        }

        public IList<ItemBaseEntity> GetItems(List<int> measureIds)
        {
            IList<ItemBaseEntity> listItemBase = new List<ItemBaseEntity>();
            if (measureIds == null || measureIds.Count == 0) return listItemBase;


            List<int> itemIds = new List<int>();
            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            using (SqlConnection conn = new SqlConnection(context.DbContext.Database.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                string itemSql =
                    string.Format("select * from ItemBases where MeasureId IN({0}) AND IsDeleted = 0 AND Status =1",
                        string.Join(",", measureIds));
                string itemChildSql = "";
                List<ItemBaseEntity> tempItemBaseList =
                    context.DbContext.Database.SqlQuery<ItemBaseEntity>(itemSql).ToList();
                var answers = GetAnswers(tempItemBaseList.Select(e => e.ID).ToList());
                var itemTypes = tempItemBaseList.GroupBy(e => e.Type);
                foreach (var type in itemTypes)
                {
                    itemChildSql = "SELECT Table1.*,I.* FROM {0} Table1 LEFT JOIN ItemBases I on I.ID=Table1.ID WHERE Type ={1} AND EXISTS (SELECT STT.id FROM dbo.SplitToTable('{2}') AS STT WHERE STT.id=I.ID) AND IsDeleted = 0 AND Status =1";
                    ItemType itemType = (ItemType)type.Key;
                    itemIds =
                        tempItemBaseList.Where(e => e.Type == itemType).Select(e => e.ID).ToList();
                    switch (itemType)
                    {
                        case ItemType.Direction:
                            //itemChildSql = string.Format(
                            //    "select Table1.*,I.* from DirectionItems Table1 left join ItemBases I on I.ID=Table1.ID where Type ={0} AND I.ID IN({1}) AND IsDeleted = 0 AND Status =1",
                            //    (int)itemType, string.Join(",", itemIds));
                            itemChildSql = string.Format(itemChildSql, "DirectionItems", (int)itemType, string.Join(",", itemIds));
                            List<DirectionItemEntity> listDirection = context.DbContext.Database.SqlQuery<DirectionItemEntity>(itemChildSql).ToList();
                            foreach (var entity in listDirection)
                            {
                                ItemBaseEntity itemBase = entity;
                                itemBase.Answers = answers.Where(e => e.ItemId == entity.ID).ToList();
                                listItemBase.Add(itemBase);
                            }
                            break;
                        case ItemType.TxkeaExpressive:
                            //itemChildSql = string.Format(
                            //    "select Table1.*,I.* from TxkeaExpressiveItems Table1 left join ItemBases I on I.ID=Table1.ID where Type ={0} AND I.ID IN({1}) AND IsDeleted = 0 AND Status =1",
                            //    (int)itemType, string.Join(",", itemIds));
                            itemChildSql = string.Format(itemChildSql, "TxkeaExpressiveItems", (int)itemType, string.Join(",", itemIds));
                            List<TxkeaExpressiveItemEntity> listTxkeaExpressive = context.DbContext.Database.SqlQuery<TxkeaExpressiveItemEntity>(itemChildSql).ToList();
                            var branchingScoresExpressive = GetTexkeaBranchingScores(itemIds);

                            IList<TxkeaExpressiveResponseEntity> expressiveResponses = GetExpressiveResponses(itemIds);
                            IList<TxkeaExpressiveResponseOptionEntity> expressiveResponseOptions =
                                GetExpressiveResponseOptions(expressiveResponses.Select(e => e.ID).ToList());
                            IList<TxkeaExpressiveImageEntity> images = GetExpressiveImages(itemIds);
                            foreach (var txkeaExpressiveResponseEntity in expressiveResponses)
                            {
                                txkeaExpressiveResponseEntity.Options =
                                    expressiveResponseOptions.Where(e => e.ResponseId == txkeaExpressiveResponseEntity.ID)
                                        .ToList();
                            }
                            foreach (var entity in listTxkeaExpressive)
                            {
                                ItemBaseEntity itemBase = entity;
                                itemBase.Answers = answers.Where(e => e.ItemId == entity.ID).ToList();
                                entity.Responses = expressiveResponses.Where(e => e.ItemId == itemBase.ID).ToList();
                                entity.ImageList = images.Where(e => e.ItemId == itemBase.ID).ToList();
                                itemBase.BranchingScores = branchingScoresExpressive.Where(e => e.ItemId == entity.ID).ToList();
                                listItemBase.Add(itemBase);
                            }
                            break;
                        case ItemType.ReceptivePrompt:
                            //itemChildSql = string.Format(
                            //    "select Table1.*,I.* from ReceptivePromptItems Table1 left join ItemBases I on I.ID=Table1.ID where Type ={0} AND I.ID IN({1}) AND IsDeleted = 0 AND Status =1",
                            //    (int)itemType, string.Join(",", itemIds));
                            itemChildSql = string.Format(itemChildSql, "ReceptivePromptItems", (int)itemType, string.Join(",", itemIds));
                            List<ReceptivePromptItemEntity> listReceptivePrompt = context.DbContext.Database.SqlQuery<ReceptivePromptItemEntity>(itemChildSql).ToList();
                            foreach (var entity in listReceptivePrompt)
                            {
                                ItemBaseEntity itemBase = entity;
                                itemBase.Answers = answers.Where(e => e.ItemId == entity.ID).ToList();
                                listItemBase.Add(itemBase);
                            }
                            break;
                        case ItemType.MultipleChoices:
                            //itemChildSql = string.Format(
                            //    "select Table1.*,I.* from MultipleChoicesItems Table1 left join ItemBases I on I.ID=Table1.ID where Type ={0} AND I.ID IN({1}) AND IsDeleted = 0 AND Status =1",
                            //    (int)itemType, string.Join(",", itemIds));
                            itemChildSql = string.Format(itemChildSql, "MultipleChoicesItems", (int)itemType, string.Join(",", itemIds));
                            List<MultipleChoicesItemEntity> listMultipleChoices = context.DbContext.Database.SqlQuery<MultipleChoicesItemEntity>(itemChildSql).ToList();
                            foreach (var entity in listMultipleChoices)
                            {
                                ItemBaseEntity itemBase = entity;
                                itemBase.Answers = answers.Where(e => e.ItemId == entity.ID).ToList();
                                listItemBase.Add(itemBase);
                            }
                            break;
                        case ItemType.Pa:
                            //itemChildSql = string.Format(
                            //    "select Table1.*,I.* from PaItems Table1 left join ItemBases I on I.ID=Table1.ID where Type ={0} AND I.ID IN({1}) AND IsDeleted = 0 AND Status =1",
                            //    (int)itemType, string.Join(",", itemIds));
                            itemChildSql = string.Format(itemChildSql, "PaItems", (int)itemType, string.Join(",", itemIds));
                            List<PaItemEntity> listPa = context.DbContext.Database.SqlQuery<PaItemEntity>(itemChildSql).ToList();
                            foreach (var entity in listPa)
                            {
                                ItemBaseEntity itemBase = entity;
                                itemBase.Answers = answers.Where(e => e.ItemId == entity.ID).ToList();
                                listItemBase.Add(itemBase);
                            }
                            break;
                        case ItemType.Quadrant:
                            //itemChildSql = string.Format(
                            //    "select Table1.*,I.* from QuadrantItems Table1 left join ItemBases I on I.ID=Table1.ID where Type ={0} AND I.ID IN({1}) AND IsDeleted = 0 AND Status =1",
                            //    (int)itemType, string.Join(",", itemIds));
                            itemChildSql = string.Format(itemChildSql, "QuadrantItems", (int)itemType, string.Join(",", itemIds));
                            List<QuadrantItemEntity> listQuadrant = context.DbContext.Database.SqlQuery<QuadrantItemEntity>(itemChildSql).ToList();
                            foreach (var entity in listQuadrant)
                            {
                                ItemBaseEntity itemBase = entity;
                                itemBase.Answers = answers.Where(e => e.ItemId == entity.ID).ToList();
                                listItemBase.Add(itemBase);
                            }
                            break;
                        case ItemType.RapidExpressive:
                            //itemChildSql = string.Format(
                            //    "select Table1.*,I.* from RapidExpressiveItems Table1 left join ItemBases I on I.ID=Table1.ID where Type ={0} AND I.ID IN({1}) AND IsDeleted = 0 AND Status =1",
                            //    (int)itemType, string.Join(",", itemIds));
                            itemChildSql = string.Format(itemChildSql, "RapidExpressiveItems", (int)itemType, string.Join(",", itemIds));
                            List<RapidExpressiveItemEntity> listRapidExpressive = context.DbContext.Database.SqlQuery<RapidExpressiveItemEntity>(itemChildSql).ToList();
                            foreach (var entity in listRapidExpressive)
                            {
                                ItemBaseEntity itemBase = entity;
                                itemBase.Answers = answers.Where(e => e.ItemId == entity.ID).ToList();
                                listItemBase.Add(itemBase);
                            }
                            break;
                        case ItemType.Checklist:
                            //itemChildSql = string.Format(
                            //    "select Table1.*,I.* from ChecklistItems Table1 left join ItemBases I on I.ID=Table1.ID where Type ={0} AND I.ID IN({1}) AND IsDeleted = 0 AND Status =1",
                            //    (int)itemType, string.Join(",", itemIds));
                            itemChildSql = string.Format(itemChildSql, "ChecklistItems", (int)itemType, string.Join(",", itemIds));
                            List<ChecklistItemEntity> listChecklist = context.DbContext.Database.SqlQuery<ChecklistItemEntity>(itemChildSql).ToList();
                            foreach (var entity in listChecklist)
                            {
                                ItemBaseEntity itemBase = entity;
                                itemBase.Answers = answers.Where(e => e.ItemId == entity.ID).ToList();
                                listItemBase.Add(itemBase);
                            }
                            break;
                        case ItemType.TypedResponse:
                            //itemChildSql = string.Format(
                            //    "select Table1.*,I.* from TypedResponseItems Table1 left join ItemBases I on I.ID=Table1.ID where Type ={0} AND I.ID IN({1}) AND IsDeleted = 0 AND Status =1",
                            //    (int)itemType, string.Join(",", itemIds));
                            itemChildSql = string.Format(itemChildSql, "TypedResponseItems", (int)itemType, string.Join(",", itemIds));
                            List<TypedResponseItemEntity> listTypedResponse = context.DbContext.Database.SqlQuery<TypedResponseItemEntity>(itemChildSql).ToList();

                            IList<TypedResponseEntity> typedResponses = GetTypedResponses(itemIds);
                            IList<TypedResponseOptionEntity> typedResponseOptions =
                                GetTypedResponseOptions(typedResponses.Select(e => e.ID).ToList());
                            foreach (var typedResponse in typedResponses)
                            {
                                typedResponse.Options =
                                    typedResponseOptions.Where(e => e.ResponseId == typedResponse.ID).ToList();
                            }
                            foreach (var entity in listTypedResponse)
                            {
                                ItemBaseEntity itemBase = entity;
                                itemBase.Answers = answers.Where(e => e.ItemId == entity.ID).ToList();
                                entity.Responses = typedResponses.Where(e => e.ItemId == itemBase.ID).ToList();
                                listItemBase.Add(itemBase);
                            }
                            break;
                        case ItemType.Receptive:
                            //itemChildSql = string.Format(
                            //    "select Table1.*,I.* from ReceptiveItems Table1 left join ItemBases I on I.ID=Table1.ID where Type ={0} AND I.ID IN({1}) AND IsDeleted = 0 AND Status =1",
                            //    (int)itemType, string.Join(",", itemIds));
                            itemChildSql = string.Format(itemChildSql, "ReceptiveItems", (int)itemType, string.Join(",", itemIds));
                            List<ReceptiveItemEntity> listReceptive = context.DbContext.Database.SqlQuery<ReceptiveItemEntity>(itemChildSql).ToList();
                            foreach (var entity in listReceptive)
                            {
                                ItemBaseEntity itemBase = entity;
                                itemBase.Answers = answers.Where(e => e.ItemId == entity.ID).ToList();
                                listItemBase.Add(itemBase);
                            }
                            break;
                        case ItemType.TxkeaReceptive:
                            //itemChildSql = string.Format(
                            //    "select Table1.*,I.* from TxkeaReceptiveItems Table1 left join ItemBases I on I.ID=Table1.ID where Type ={0} AND I.ID IN({1}) AND IsDeleted = 0 AND Status =1",
                            //    (int)itemType, string.Join(",", itemIds));
                            itemChildSql = string.Format(itemChildSql, "TxkeaReceptiveItems", (int)itemType, string.Join(",", itemIds));
                            List<TxkeaReceptiveItemEntity> listTxkeaReceptive = context.DbContext.Database.SqlQuery<TxkeaReceptiveItemEntity>(itemChildSql).ToList();
                            var branchingScoresReceptive = GetTexkeaBranchingScores(itemIds);
                            foreach (var entity in listTxkeaReceptive)
                            {
                                ItemBaseEntity itemBase = entity;
                                itemBase.Answers = answers.Where(e => e.ItemId == entity.ID).ToList();
                                itemBase.BranchingScores = branchingScoresReceptive.Where(e => e.ItemId == entity.ID).ToList();
                                listItemBase.Add(itemBase);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return listItemBase;
        }

        public IList<AnswerEntity> GetAnswers(List<int> itemIds)
        {
            IList<AnswerEntity> list = new List<AnswerEntity>();
            if (itemIds == null || itemIds.Count == 0) return list;

            StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("select * from Answers where ItemId IN({0})", string.Join(",", itemIds));
            sb.AppendFormat("SELECT * FROM dbo.Answers AS A WHERE EXISTS(SELECT STT.id FROM dbo.SplitToTable('{0}') AS STT WHERE STT.id=A.ItemId)", string.Join(",", itemIds));
            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            using (SqlConnection conn = new SqlConnection(context.DbContext.Database.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                list = context.DbContext.Database.SqlQuery<AnswerEntity>(sb.ToString()).ToList();
            }
            return list;
        }

        public IList<TypedResponseEntity> GetTypedResponses(List<int> itemIds)
        {
            IList<TypedResponseEntity> list = new List<TypedResponseEntity>();
            if (itemIds == null || itemIds.Count == 0) return list;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select * from TypedResonses TR where ItemId IN({0})", string.Join(",", itemIds))
                ;
            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            using (SqlConnection conn = new SqlConnection(context.DbContext.Database.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                list = context.DbContext.Database.SqlQuery<TypedResponseEntity>(sb.ToString()).ToList();
            }
            return list;
        }

        public IList<TypedResponseOptionEntity> GetTypedResponseOptions(List<int> responseIds)
        {
            IList<TypedResponseOptionEntity> list = new List<TypedResponseOptionEntity>();
            if (responseIds == null || responseIds.Count == 0) return list;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select * from TypedResponseOptions TRO where ResponseId IN({0})", string.Join(",", responseIds))
                ;
            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            using (SqlConnection conn = new SqlConnection(context.DbContext.Database.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                list = context.DbContext.Database.SqlQuery<TypedResponseOptionEntity>(sb.ToString()).ToList();
            }
            return list;
        }

        public IList<TxkeaExpressiveImageEntity> GetExpressiveImages(List<int> itemIds)
        {
            IList<TxkeaExpressiveImageEntity> list = new List<TxkeaExpressiveImageEntity>();
            if (itemIds == null || itemIds.Count == 0) return list;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select * from TxkeaExpressiveImages where ItemId IN({0})", string.Join(",", itemIds))
                ;
            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            using (SqlConnection conn = new SqlConnection(context.DbContext.Database.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                list = context.DbContext.Database.SqlQuery<TxkeaExpressiveImageEntity>(sb.ToString()).ToList();
            }
            return list;
        }
        public IList<TxkeaExpressiveResponseEntity> GetExpressiveResponses(List<int> itemIds)
        {
            IList<TxkeaExpressiveResponseEntity> list = new List<TxkeaExpressiveResponseEntity>();
            if (itemIds == null || itemIds.Count == 0) return list;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select * from TxkeaExpressiveResponses TXR where ItemId IN({0})", string.Join(",", itemIds))
                ;
            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            using (SqlConnection conn = new SqlConnection(context.DbContext.Database.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                list = context.DbContext.Database.SqlQuery<TxkeaExpressiveResponseEntity>(sb.ToString()).ToList();
            }
            return list;
        }

        public IList<TxkeaExpressiveResponseOptionEntity> GetExpressiveResponseOptions(List<int> responseIds)
        {
            IList<TxkeaExpressiveResponseOptionEntity> list = new List<TxkeaExpressiveResponseOptionEntity>();
            if (responseIds == null || responseIds.Count == 0) return list;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select * from TxkeaExpressiveResponseOptions TXRO where ResponseId IN({0})", string.Join(",", responseIds))
                ;
            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            using (SqlConnection conn = new SqlConnection(context.DbContext.Database.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                list = context.DbContext.Database.SqlQuery<TxkeaExpressiveResponseOptionEntity>(sb.ToString()).ToList();
            }
            return list;
        }

        public IList<BranchingScoreEntity> GetTexkeaBranchingScores(List<int> itemIds)
        {
            IList<BranchingScoreEntity> list = new List<BranchingScoreEntity>();
            if (itemIds == null || itemIds.Count == 0) return list;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("select * from BranchingScores where ItemId IN({0})", string.Join(",", itemIds));
            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            using (SqlConnection conn = new SqlConnection(context.DbContext.Database.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                list = context.DbContext.Database.SqlQuery<BranchingScoreEntity>(sb.ToString()).ToList();
            }
            return list;
        }

        public IList<ObservableChoiceEntity> GetObserveChoiceItems(List<int> itemIds)
        {
            List<ObservableChoiceEntity> listObservableChoices = new List<ObservableChoiceEntity>();

            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            using (SqlConnection conn = new SqlConnection(context.DbContext.Database.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                string itemSql = string.Format(
                    "select Table1.*,I.* from ObservableChoiceItems Table1 left join ItemBases I on I.ID=Table1.ID where Type ={0} AND I.ID IN({1}) AND IsDeleted = 0 AND Status =1",
                    (int)ItemType.ObservableChoice, string.Join(",", itemIds));
                listObservableChoices = context.DbContext.Database.SqlQuery<ObservableChoiceEntity>(itemSql).ToList();
            }
            return listObservableChoices;
        }

        public int GetIsExistMobileAudio(List<int> measureIds)
        {
            List<string> listAudios = new List<string>();
            AdeUnitOfWorkContext context = this.UnitOfWork as AdeUnitOfWorkContext;
            using (SqlConnection conn = new SqlConnection(context.DbContext.Database.Connection.ConnectionString))
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();
                string itemSql = string.Format(
                    "select count(*) from " +
                    "(" +
                    "select InstructionAudio from TxkeaExpressiveItems where InstructionAudio!='' and ID in {0} " +
                    "union select TargetAudio from ReceptiveItems where TargetAudio!='' and ID in {0} " +
                    "union select TargetAudio from TypedResponseItems where TargetAudio!='' and ID in {0} " +
                    "union select TargetAudio from RapidExpressiveItems where TargetAudio!='' and ID in {0} " +
                    "union select TargetAudio from QuadrantItems where TargetAudio!='' and ID in {0} " +
                    "union select TargetAudio from PaItems where TargetAudio!='' and ID in {0} " +
                    "union select TargetAudio from MultipleChoicesItems where TargetAudio!='' and ID in {0} " +
                    "union select TargetAudio from ReceptivePromptItems where TargetAudio!='' and ID in {0} " +
                    "union select PromptAudio from ReceptivePromptItems where PromptAudio!='' and ID in {0} " +
                    "union select InstructionAudio from TxkeaReceptiveItems where InstructionAudio!='' and ID in {0} " +
                    "union select Audio from Answers where Audio!='' and ItemId in {0} " +
                    "union select ResponseAudio from Answers where ResponseAudio!='' and ItemId in {0} " +
                    "union select TargetAudio from TxkeaExpressiveImages where TargetAudio!='' and ItemId in {0} " +
                    ") as AllAudio",
                    string.Format("(select ID from ItemBases where MeasureId IN ({0}) AND IsDeleted = 0 AND Status =1)",
                        string.Join(",", measureIds)));
                List<int> audios = context.DbContext.Database.SqlQuery<int>(itemSql).ToList();
                return audios[0];
            }
        }

        public bool ExecuteSql(string strSql)
        {
            if (string.IsNullOrEmpty(strSql))
                return false;
            else
                return this.EFContext.DbContext.Database.ExecuteSqlCommand(strSql.ToString()) >= 0;
        }
    }
}
