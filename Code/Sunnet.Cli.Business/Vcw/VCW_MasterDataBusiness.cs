using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Joe
 * Computer:		JOE-PC
 * Domain:			Joe-pc
 * CreatedOn:		2015/3/17 
 * Description:		Please input class summary
 * Version History:	Created,2015/3/17 
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Vcw;
using Sunnet.Cli.Core;
using Sunnet.Cli.Business.Common.Enum;
using Sunnet.Cli.Core.Vcw.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Framework;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Sunnet.Framework.Log;
using StructureMap;
using Sunnet.Cli.Business.Vcw.Models;
using System.IO;
using System.Web;
using System.Web.Caching;

namespace Sunnet.Cli.Business.Vcw
{
    public class VCW_MasterDataBusiness
    {
        private readonly IVcwContract _server;
        private readonly ISunnetLog _logger;

        public VCW_MasterDataBusiness(VCWUnitOfWorkContext unit = null)
        {
            _server = DomainFacade.CreateVcwService(unit);
            _logger = ObjectFactory.GetInstance<ISunnetLog>();
        }

        public UploadTypeEntity GetUploadType(int id)
        {
            return _server.GetUploadType(id);
        }

        public IEnumerable<SelectItemModelOther> GetAllUploadTypes()
        {
            return _server.UploadTypes.Select(e => new SelectItemModelOther()
            {
                ID = e.ID,
                Name = e.Name,
                Status = e.Status
            });
        }

        public List<SelectItemModel> GetActiveUploadTypes()
        {
            List<SelectItemModel> list_Master = HttpRuntime.Cache["VcwTools" + BaseDataModel.UploadTypes.ToString()] as List<SelectItemModel>;
            if (list_Master == null)
            {
                if (File.Exists(SFConfig.CacheFileDependency_VcwTool) == false)
                    File.Create(SFConfig.CacheFileDependency_VcwTool).Close();
                list_Master = _server.UploadTypes.Where(u => u.Status == EntityStatus.Active)
                .Select(u => new SelectItemModel
                {
                    ID = u.ID,
                    Name = u.Name
                }).Distinct().ToList();
                HttpRuntime.Cache.Insert("VcwTools" + BaseDataModel.UploadTypes.ToString(), list_Master, new CacheDependency(SFConfig.CacheFileDependency_VcwTool));
            }

            return list_Master;
        }

        public OperationResult AddUploadType(UploadTypeEntity uploadtype)
        {
            ClearCache();
            return _server.AddUploadType(uploadtype);
        }

        public OperationResult UpdateUploadType(UploadTypeEntity uploadtype)
        {
            ClearCache();
            return _server.UpdateUploadType(uploadtype);
        }

        public SessionEntity GetSession(int id)
        {
            return _server.GetSession(id);
        }

        public IEnumerable<SelectItemModelOther> GetAllSessions()
        {
            return _server.Sessions.Select(e => new SelectItemModelOther()
            {
                ID = e.ID,
                Name = e.Name,
                Status = e.Status
            });
        }

        public List<SelectItemModel> GetActiveSessions()
        {
            List<SelectItemModel> list_Master = HttpRuntime.Cache["VcwTools" + BaseDataModel.Sessions.ToString()] as List<SelectItemModel>;
            if (list_Master == null)
            {
                if (File.Exists(SFConfig.CacheFileDependency_VcwTool) == false)
                    File.Create(SFConfig.CacheFileDependency_VcwTool).Close();
                list_Master = _server.Sessions.Where(u => u.Status == EntityStatus.Active)
                .Select(u => new SelectItemModel
                {
                    ID = u.ID,
                    Name = u.Name
                }).Distinct().ToList();
                HttpRuntime.Cache.Insert("VcwTools" + BaseDataModel.Sessions.ToString(), list_Master, new CacheDependency(SFConfig.CacheFileDependency_VcwTool));
            }
            return list_Master;
        }

        public OperationResult AddSession(SessionEntity session)
        {
            ClearCache();
            return _server.AddSession(session);
        }

        public OperationResult UpdateSession(SessionEntity session)
        {
            ClearCache();
            return _server.UpdateSession(session);
        }


        public WaveEntity GetWave(int id)
        {
            return _server.GetWave(id);
        }

        public IEnumerable<SelectItemModelOther> GetAllWaves()
        {
            return _server.Waves.Select(e => new SelectItemModelOther()
            {
                ID = e.ID,
                Name = e.Name,
                Status = e.Status
            });
        }

        public List<SelectItemModel> GetActiveWaves()
        {
            List<SelectItemModel> list_Master = HttpRuntime.Cache["VcwTools" + BaseDataModel.Waves.ToString()] as List<SelectItemModel>;
            if (list_Master == null)
            {
                if (File.Exists(SFConfig.CacheFileDependency_VcwTool) == false)
                    File.Create(SFConfig.CacheFileDependency_VcwTool).Close();
                list_Master = _server.Waves.Where(u => u.Status == EntityStatus.Active)
                .Select(u => new SelectItemModel
                {
                    ID = u.ID,
                    Name = u.Name
                }).Distinct().ToList();
                HttpRuntime.Cache.Insert("VcwTools" + BaseDataModel.Waves.ToString(), list_Master, new CacheDependency(SFConfig.CacheFileDependency_VcwTool));
            }
            return list_Master;
        }

        public OperationResult AddWave(WaveEntity wave)
        {
            ClearCache();
            return _server.AddWave(wave);
        }

        public OperationResult UpdateWave(WaveEntity wave)
        {
            ClearCache();
            return _server.UpdateWave(wave);
        }

        public Context_DataEntity GetContext_Data(int id)
        {
            return _server.GetContext(id);
        }

        public IEnumerable<SelectItemModelOther> GetAllContext_Datas()
        {
            return _server.Context_Datas.Select(e => new SelectItemModelOther()
            {
                ID = e.ID,
                Name = e.Name,
                Status = e.Status
            });
        }

        public List<SelectItemModel> GetActiveContext_Datas()
        {
            List<SelectItemModel> list_Master = HttpRuntime.Cache["VcwTools" + BaseDataModel.Context_Datas.ToString()] as List<SelectItemModel>;
            if (list_Master == null)
            {
                if (File.Exists(SFConfig.CacheFileDependency_VcwTool) == false)
                    File.Create(SFConfig.CacheFileDependency_VcwTool).Close();
                list_Master = _server.Context_Datas.Where(u => u.Status == EntityStatus.Active)
                .Select(u => new SelectItemModel
                {
                    ID = u.ID,
                    Name = u.Name
                }).Distinct().ToList();
                //如果有other选项，则把other选项放在最后面
                SelectItemModel other = list_Master.Where(r => r.Name.Trim().ToLower() == SFConfig.ContextOther).FirstOrDefault();
                if (other != null)
                {
                    list_Master.Remove(other);
                    list_Master.Add(other);
                }
                HttpRuntime.Cache.Insert("VcwTools" + BaseDataModel.Context_Datas.ToString(), list_Master, new CacheDependency(SFConfig.CacheFileDependency_VcwTool));
            }
            return list_Master;
        }

        public OperationResult AddContext_Data(Context_DataEntity Context)
        {
            ClearCache();
            return _server.AddContext(Context);
        }

        public OperationResult UpdateContext_Data(Context_DataEntity Context)
        {
            ClearCache();
            return _server.UpdateContext(Context);
        }



        public Assignment_Content_DataEntity GetAssignment_Content_Data(int id)
        {
            return _server.GetAssignmentContentData(id);
        }

        public IEnumerable<SelectItemModelOther> GetAllAssignment_Content_Datas()
        {
            return _server.Assignment_Content_Datas.Select(e => new SelectItemModelOther()
            {
                ID = e.ID,
                Name = e.Name,
                Status = e.Status
            });
        }

        public List<SelectItemModel> GetActiveAssignment_Content_Datas()
        {
            List<SelectItemModel> list_Master = HttpRuntime.Cache["VcwTools" + BaseDataModel.Assignment_Content_Datas.ToString()] as List<SelectItemModel>;
            if (list_Master == null)
            {
                if (File.Exists(SFConfig.CacheFileDependency_VcwTool) == false)
                    File.Create(SFConfig.CacheFileDependency_VcwTool).Close();
                list_Master = _server.Assignment_Content_Datas.Where(u => u.Status == EntityStatus.Active)
                .Select(u => new SelectItemModel
                {
                    ID = u.ID,
                    Name = u.Name
                }).Distinct().ToList();
                //如果有other选项，则把other选项放在最后面
                SelectItemModel other = list_Master.Where(r => r.Name.Trim().ToLower() == SFConfig.AssignmentContentOther).FirstOrDefault();
                if (other != null)
                {
                    list_Master.Remove(other);
                    list_Master.Add(other);
                }
                HttpRuntime.Cache.Insert("VcwTools" + BaseDataModel.Assignment_Content_Datas.ToString()
                    , list_Master, new CacheDependency(SFConfig.CacheFileDependency_VcwTool));
            }
            return list_Master;
        }

        public OperationResult AddAssignment_Content_Data(Assignment_Content_DataEntity Assignment_Content_Data)
        {
            ClearCache();
            return _server.AddAssignmentContentData(Assignment_Content_Data);
        }

        public OperationResult UpdateAssignment_Content_Data(Assignment_Content_DataEntity Assignment_Content_Data)
        {
            ClearCache();
            return _server.UpdateAssignmentContentData(Assignment_Content_Data);
        }



        public Video_Content_DataEntity GetVideo_Content_Data(int id)
        {
            return _server.GetVideoContentData(id);
        }

        public IEnumerable<SelectItemModelOther> GetAllVideo_Content_Datas()
        {
            return _server.Video_Content_Datas.Select(e => new SelectItemModelOther()
            {
                ID = e.ID,
                Name = e.Name,
                Status = e.Status
            });
        }

        public List<SelectItemModel> GetActiveVideo_Content_Datas()
        {
            List<SelectItemModel> list_Master = HttpRuntime.Cache["VcwTools" + BaseDataModel.Video_Content_Datas.ToString()] as List<SelectItemModel>;
            if (list_Master == null)
            {
                if (File.Exists(SFConfig.CacheFileDependency_VcwTool) == false)
                    File.Create(SFConfig.CacheFileDependency_VcwTool).Close();
                list_Master = _server.Video_Content_Datas.Where(u => u.Status == EntityStatus.Active)
                .Select(u => new SelectItemModel
                {
                    ID = u.ID,
                    Name = u.Name
                }).Distinct().ToList();
                //如果有other选项，则把other选项放在最后面
                SelectItemModel other = list_Master.Where(r => r.Name.Trim().ToLower() == SFConfig.VideoContentOther).FirstOrDefault();
                if (other != null)
                {
                    list_Master.Remove(other);
                    list_Master.Add(other);
                }
                HttpRuntime.Cache.Insert("VcwTools" + BaseDataModel.Video_Content_Datas.ToString()
                    , list_Master, new CacheDependency(SFConfig.CacheFileDependency_VcwTool));
            }
            return list_Master;
        }

        public OperationResult AddVideo_Content_Data(Video_Content_DataEntity Video_Content_Data)
        {
            ClearCache();
            return _server.AddVideoContentData(Video_Content_Data);
        }

        public OperationResult UpdateVideo_Content_Data(Video_Content_DataEntity Video_Content_Data)
        {
            ClearCache();
            return _server.UpdateVideoContentData(Video_Content_Data);
        }



        public Video_Language_DataEntity GetVideo_Language_Data(int id)
        {
            return _server.GetVideoLanguageData(id);
        }

        public IEnumerable<SelectItemModelOther> GetAllVideo_Language_Datas()
        {
            return _server.Video_Language_Datas.Select(e => new SelectItemModelOther()
            {
                ID = e.ID,
                Name = e.Name,
                Status = e.Status
            });
        }

        public List<SelectItemModel> GetActiveVideo_Language_Datas()
        {
            List<SelectItemModel> list_Master = HttpRuntime.Cache["VcwTools" + BaseDataModel.Video_Language_Datas.ToString()] as List<SelectItemModel>;
            if (list_Master == null)
            {
                if (File.Exists(SFConfig.CacheFileDependency_VcwTool) == false)
                    File.Create(SFConfig.CacheFileDependency_VcwTool).Close();
                list_Master = _server.Video_Language_Datas.Where(u => u.Status == EntityStatus.Active)
                .Select(u => new SelectItemModel
                {
                    ID = u.ID,
                    Name = u.Name
                }).Distinct().ToList();
                HttpRuntime.Cache.Insert("VcwTools" + BaseDataModel.Video_Language_Datas.ToString()
                    , list_Master, new CacheDependency(SFConfig.CacheFileDependency_VcwTool));
            }
            return list_Master;
        }

        public OperationResult AddVideo_Language_Data(Video_Language_DataEntity Video_Language_Data)
        {
            ClearCache();
            return _server.AddVideoLanguageData(Video_Language_Data);
        }

        public OperationResult UpdateVideo_Language_Data(Video_Language_DataEntity Video_Language_Data)
        {
            ClearCache();
            return _server.UpdateVideoLanguageData(Video_Language_Data);
        }


        public CoachingStrategy_DataEntity GetCoachingStrategy_Data(int id)
        {
            return _server.GetCoachingStrategyData(id);
        }

        public IEnumerable<SelectItemModelOther> GetAllCoachingStrategy_Datas()
        {
            return _server.CoachingStrategy_Datas.Select(e => new SelectItemModelOther()
            {
                ID = e.ID,
                Name = e.Name,
                Status = e.Status
            });
        }

        public List<SelectItemModel> GetActiveCoachingStrategy_Datas()
        {
            List<SelectItemModel> list_Master = HttpRuntime.Cache["VcwTools" + BaseDataModel.CoachingStrategy_Datas.ToString()] as List<SelectItemModel>;
            if (list_Master == null)
            {
                if (File.Exists(SFConfig.CacheFileDependency_VcwTool) == false)
                    File.Create(SFConfig.CacheFileDependency_VcwTool).Close();
                list_Master = _server.CoachingStrategy_Datas.Where(u => u.Status == EntityStatus.Active)
                .Select(u => new SelectItemModel
                {
                    ID = u.ID,
                    Name = u.Name
                }).Distinct().ToList();
                //如果有other选项，则把other选项放在最后面
                SelectItemModel other = list_Master.Where(r => r.Name.Trim().ToLower() == SFConfig.StrategyOther).FirstOrDefault();
                if (other != null)
                {
                    list_Master.Remove(other);
                    list_Master.Add(other);
                }
                HttpRuntime.Cache.Insert("VcwTools" + BaseDataModel.CoachingStrategy_Datas.ToString()
                    , list_Master, new CacheDependency(SFConfig.CacheFileDependency_VcwTool));
            }
            return list_Master;

        }

        /// <summary>
        /// Assignment中 CoachingStrategy_Datas 排除 other选项
        /// </summary>
        /// <returns></returns>
        public List<SelectItemModel> GetActiveAssignmentCoachingStrategy_Datas()
        {
            List<SelectItemModel> list_Master = HttpRuntime.Cache["VcwTools" + BaseDataModel.CoachingStrategy_Datas.ToString()] as List<SelectItemModel>;
            if (list_Master == null)
            {
                list_Master = _server.CoachingStrategy_Datas
                    .Where(r => r.Status == EntityStatus.Active && r.Name.Trim().ToLower() != SFConfig.StrategyOther)
                .Select(u => new SelectItemModel
                {
                    ID = u.ID,
                    Name = u.Name
                }).Distinct().ToList();
            }
            else
            {
                list_Master = list_Master.Where(r => r.Name.Trim().ToLower() != SFConfig.StrategyOther).ToList();
            }
            return list_Master;
        }

        public OperationResult AddCoachingStrategy_Data(CoachingStrategy_DataEntity CoachingStrategy_Data)
        {
            ClearCache();
            return _server.AddCoachingStrategyData(CoachingStrategy_Data);
        }

        public OperationResult UpdateCoachingStrategy_Data(CoachingStrategy_DataEntity CoachingStrategy_Data)
        {
            ClearCache();
            return _server.UpdateCoachingStrategyData(CoachingStrategy_Data);
        }



        public Video_SelectionList_DataEntity GetVideo_SelectionList_Data(int id)
        {
            return _server.GetVideoSelectionListData(id);
        }

        public IEnumerable<SelectItemModelOther> GetAllVideo_SelectionList_Datas()
        {
            return _server.Video_SelectionList_Datas.Select(e => new SelectItemModelOther()
            {
                ID = e.ID,
                Name = e.Name,
                Status = e.Status
            });
        }

        public List<SelectItemModel> GetActiveVideo_SelectionList_Datas()
        {
            List<SelectItemModel> list_Master = HttpRuntime.Cache["VcwTools" + BaseDataModel.Video_SelectionList_Datas.ToString()] as List<SelectItemModel>;
            if (list_Master == null)
            {
                if (File.Exists(SFConfig.CacheFileDependency_VcwTool) == false)
                    File.Create(SFConfig.CacheFileDependency_VcwTool).Close();
                list_Master = _server.Video_SelectionList_Datas.Where(u => u.Status == EntityStatus.Active)
                .Select(u => new SelectItemModel
                {
                    ID = u.ID,
                    Name = u.Name
                }).Distinct().ToList();

                HttpRuntime.Cache.Insert("VcwTools" + BaseDataModel.Video_SelectionList_Datas.ToString()
                    , list_Master, new CacheDependency(SFConfig.CacheFileDependency_VcwTool));
            }
            return list_Master;
        }

        public OperationResult AddVideo_SelectionList_Data(Video_SelectionList_DataEntity Video_SelectionList_Data)
        {
            ClearCache();
            return _server.AddVideoSelectionListData(Video_SelectionList_Data);
        }

        public OperationResult UpdateVideo_SelectionList_Data(Video_SelectionList_DataEntity Video_SelectionList_Data)
        {
            ClearCache();
            return _server.UpdateVideoSelectionListData(Video_SelectionList_Data);
        }

        /// <summary>
        /// 清除VCW站点基本数据缓存
        /// </summary>
        public void ClearCache()
        {
            try
            {
                if (File.Exists(SFConfig.CacheFileDependency_VcwTool))
                    File.Delete(SFConfig.CacheFileDependency_VcwTool);
            }
            catch (Exception ex)
            {
                _logger.Debug(ex.ToString());
            }
        }

        private bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }
    }
}
