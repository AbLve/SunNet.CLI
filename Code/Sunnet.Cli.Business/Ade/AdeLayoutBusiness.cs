using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe
 * CreatedOn:		2015/11/8
 * Description:		Add TxkeaReceptive Item
 * Version History:	Created,2015/11/8
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Business.Ade.Models;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Resources;
using System.Linq.Expressions;
using LinqKit;
using Sunnet.Framework.Core.Extensions;
using System.IO;

namespace Sunnet.Cli.Business.Ade
{
    public partial class AdeBusiness
    {
        public OperationResult SaveLayout(TxkeaLayoutModel model, string layoutPng, string savePath)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            TxkeaLayoutEntity entity;
            if (model.ID > 0)
            {
                entity = _adeContract.GetLayout(model.ID);
                entity.UpdatedBy = model.UpdatedBy;
                entity.UpdatedOn = model.UpdatedOn;
            }
            else
            {
                entity = new TxkeaLayoutEntity();
                entity.CreatedBy = model.CreatedBy;
                entity.CreatedOn = model.CreatedOn;
                entity.UpdatedBy = model.UpdatedBy;
                entity.UpdatedOn = model.UpdatedOn;
            }

            if (_adeContract.TxkeaLayouts.Any(r => r.ID != model.ID && r.Name == model.Name && r.IsDeleted == false))
            {
                result = new OperationResult(OperationResultType.Error, ResourceHelper.GetRM().GetInformation("Ade_SameLayoutName"));
                return result;
            }

            entity.Name = model.Name;
            entity.NumberOfImages = model.NumberOfImages;
            entity.BackgroundFill = model.BackgroundFill;
            entity.BackgroundFillType = model.BackgroundFillType;
            entity.Layout = model.Layout;
            entity.IsDeleted = model.IsDeleted;
            entity.ScreenWidth = model.ScreenWidth;
            entity.ScreenHeight = model.ScreenHeight;
            if (model.ID > 0)
                result = _adeContract.UpdateLayout(entity);

            else
                result = _adeContract.InsertLayout(entity);

            //生成图片
            if (result.ResultType == OperationResultType.Success && !string.IsNullOrEmpty(layoutPng))
            {
                if (!Directory.Exists(savePath))
                    Directory.CreateDirectory(savePath);
                //logger.Info(savePath);
                string imagePath = savePath + entity.ID + ".png";
                if (File.Exists(imagePath))
                    File.Delete(imagePath);
                FileStream fs = File.Create(imagePath);
                byte[] bytes = Convert.FromBase64String(layoutPng);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }

            return result;
        }

        public OperationResult SaveLayout(TxkeaLayoutEntity entity, int preId, string savePath)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);
            if (entity.ID > 0)
            {
                result = _adeContract.UpdateLayout(entity);
            }
            else
            {
                if (_adeContract.TxkeaLayouts.Any(r => r.ID != entity.ID && r.Name == entity.Name && r.IsDeleted == false))
                {
                    result = new OperationResult(OperationResultType.Error, ResourceHelper.GetRM().GetInformation("Ade_SameLayoutName"));
                    return result;
                }
                result = _adeContract.InsertLayout(entity);

                //生成图片
                if (result.ResultType == OperationResultType.Success)
                {
                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);
                    string preImagePath = savePath + preId + ".png";
                    string imagePath = savePath + entity.ID + ".png";
                    if (File.Exists(imagePath))
                        File.Delete(imagePath);
                    File.Copy(preImagePath, imagePath);
                }
            }
            return result;
        }

        public TxkeaLayoutModel GetLayoutModel(int id)
        {
            return _adeContract.TxkeaLayouts
                .Where(r => r.ID == id)
                .Select(SelectorEntityToItemModel).FirstOrDefault();
        }

        public TxkeaLayoutEntity GetLayout(int id)
        {
            return _adeContract.GetLayout(id);
        }

        public List<TxkeaLayoutModel> SearchLayouts(Expression<Func<TxkeaLayoutEntity, bool>> condition, out int total,
            string sort = "ID", string order = "DESC", int first = 0, int count = 10)
        {
            var query = _adeContract.TxkeaLayouts.AsExpandable()
                .Where(r => r.IsDeleted == false)
                .Where(condition).OrderBy(sort, order)
                .Select(SelectorEntityToItemModel);
            total = query.Count();
            var result = query.Skip(first).Take(count).ToList();
            return result;
        }

        private static Expression<Func<TxkeaLayoutEntity, TxkeaLayoutModel>> SelectorEntityToItemModel
        {
            get
            {
                return x => new TxkeaLayoutModel()
                {
                    ID = x.ID,
                    Name = x.Name,
                    NumberOfImages = x.NumberOfImages,
                    BackgroundFill = x.BackgroundFill,
                    BackgroundFillType = x.BackgroundFillType,
                    Layout = x.Layout,
                    IsDeleted = x.IsDeleted,
                    ScreenWidth = x.ScreenWidth,
                    ScreenHeight = x.ScreenHeight,
                    CreatedBy = x.CreatedBy,
                    CreatedOn = x.CreatedOn,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedOn = x.UpdatedOn,
                    RelatedItemsCount = x.TxkeaReceptiveItems.Count(r => r.IsDeleted == false) + x.TxkeaExpressiveItems.Count(r => r.IsDeleted == false)
                };
            }
        }

        public OperationResult DeleteLayout(int id, string savePath)
        {
            OperationResult result = new OperationResult(OperationResultType.Success);

            TxkeaLayoutEntity entity = _adeContract.GetLayout(id);
            if (entity.TxkeaExpressiveItems.Count(r => r.IsDeleted == false) > 0 || entity.TxkeaReceptiveItems.Count(r => r.IsDeleted == false) > 0)
            {
                result.ResultType = OperationResultType.Error;
                result.Message = "The layout template has been used, it can not be deleted.";
                return result;
            }
            if (entity == null)
                result.ResultType = OperationResultType.Error;
            else
            {
                entity.IsDeleted = true;
                result = _adeContract.UpdateLayout(entity);
            }
            //删除图片
            if (result.ResultType == OperationResultType.Success)
            {
                string imagePath = savePath + entity.ID + ".png";
                if (File.Exists(imagePath))
                    File.Delete(imagePath);
            }
            return result;
        }

        /// <summary>
        /// 获取所有Layout的操作人
        /// </summary>
        /// <returns></returns>
        public List<int> GetAllOperationUsers()
        {
            return _adeContract.TxkeaLayouts.Where(r => r.IsDeleted == false)
                .Select(r => r.UpdatedBy)
                .Concat(_adeContract.TxkeaLayouts.Where(x => x.IsDeleted == false)
                .Select(x => x.CreatedBy)).Distinct().ToList();
        }
    }
}
