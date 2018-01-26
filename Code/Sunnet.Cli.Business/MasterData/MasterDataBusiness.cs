using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Leason
 * Computer:		Leason-PC
 * Domain:			Leason-pc
 * CreatedOn:		2014/8/26 10:40:10
 * Description:		Create MasterDataBusiness
 * Version History:	Created,2014/8/26 10:40:10
 * 
 * 
 **************************************************************************/
using StructureMap.Query;
using Sunnet.Cli.Business.Common.Enum;
using Sunnet.Cli.Core;
using Sunnet.Cli.Core.Common.Enums;
using Sunnet.Cli.Core.MasterData;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Framework.Core.Tool;
using Sunnet.Framework.Extensions;

namespace Sunnet.Cli.Business.MasterData
{
    public class MasterDataBusiness
    {
        private readonly IMasterDataContract _masterData;

        public MasterDataBusiness(EFUnitOfWorkContext unit = null)
        {
            _masterData = DomainFacade.CreateMasterDataServer(unit);
        }

        #region Funding
        public FundingEntity NewFundingEntity()
        {
            return _masterData.NewFundingEntity();
        }

        public OperationResult InsertFunding(FundingEntity entity)
        {
            return _masterData.InsertFunding(entity);
        }

        public OperationResult DeleteFunding(int id)
        {
            return _masterData.DeleteFunding(id);
        }

        public OperationResult UpdateFunding(FundingEntity entity)
        {
            return _masterData.UpdateFunding(entity);
        }

        public FundingEntity GetFunding(int id)
        {
            return _masterData.GetFunding(id);

        }
        #endregion

        #region State
        public StateEntity NewStateEntity()
        {
            return _masterData.NewStateEntity();
        }

        public OperationResult InsertState(StateEntity entity)
        {
            return _masterData.InsertState(entity);
        }

        public OperationResult DeleteState(int id)
        {
            return _masterData.DeleteState(id);
        }

        public OperationResult UpdateState(StateEntity entity)
        {
            return _masterData.UpdateState(entity);
        }

        public StateEntity GetState(int id)
        {
            return _masterData.GetState(id);
        }
        #endregion

        #region Country
        public CountryEntity GetCountry(int id)
        {
            return _masterData.GetCountry(id);
        }
        public IQueryable<CountryEntity> GetAllCountries()
        {
            return _masterData.AllCountries.OrderBy(e => e.Name);
        }
        #endregion

        #region County
        public CountyEntity NewCountyEntity()
        {
            return _masterData.NewCountyEntity();
        }

        public OperationResult InsertCounty(CountyEntity entity)
        {
            return _masterData.InsertCounty(entity);
        }

        public OperationResult DeleteCounty(int id)
        {
            return _masterData.DeleteCounty(id);
        }

        public OperationResult UpdateCounty(CountyEntity entity)
        {
            return _masterData.UpdateCounty(entity);
        }

        public CountyEntity GetCounty(int id)
        {
            return _masterData.GetCounty(id);
        }
        #endregion

        #region Title
        public TitleEntity NewTitleEntity()
        {
            return _masterData.NewTitleEntity();
        }

        public OperationResult InsertTitle(TitleEntity entity)
        {
            return _masterData.InsertTitle(entity);
        }

        public OperationResult DeleteTitle(int id)
        {
            return _masterData.DeleteTitle(id);
        }

        public OperationResult UpdateTitle(TitleEntity entity)
        {
            return _masterData.UpdateTitle(entity);
        }

        public TitleEntity GetTitle(int id)
        {
            return _masterData.GetTitle(id);
        }
        #endregion

        #region Curriculum

        public CurriculumEntity NewCurriculumEntity()
        {
            return _masterData.NewCurriculumEntity();
        }

        public OperationResult InsertCurriculum(CurriculumEntity entity)
        {
            return _masterData.InsertCurriculum(entity);
        }

        public OperationResult UpdateCurriculum(CurriculumEntity entity)
        {
            return _masterData.UpdateCurriculum(entity);
        }

        public CurriculumEntity GetCurriculum(int id)
        {
            return _masterData.GetCurriculum(id);
        }

        #endregion

        #region Language
        public LanguageEntity GetLanguage(int id)
        {
            return _masterData.GetLanguage(id);
        }

        public OperationResult InsertLanguage(LanguageEntity entity)
        {
            return _masterData.InsertLanguage(entity);
        }

        public OperationResult UpdateLanguage(LanguageEntity entity)
        {
            return _masterData.UpdateLanguage(entity);
        }
        #endregion

        #region SelectList
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isActive">True时，返回有效状态的值；False，放回全部</param>
        /// <returns></returns>
        public IEnumerable<SelectItemModel> GetLanguageSelectList(bool isActive = true)
        {
            IQueryable<LanguageEntity> query = isActive
                ? _masterData.Languages.Where(o => o.Status == EntityStatus.Active)
                : _masterData.Languages;

            return query.Select(o => new SelectItemModel()
            {
                ID = o.ID,
                Name = o.Language
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isActive">True时，返回有效状态的数据；False时，返回全部状态的数据。</param>
        /// <returns></returns>
        public IEnumerable<SelectItemModel> GetFundingSelectList(bool isActive = true)
        {
            IQueryable<FundingEntity> query = isActive
                ? _masterData.Fundings.Where(o => o.Status == EntityStatus.Active)
                : _masterData.Fundings;

            return query.Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.Name
            });
        }

        public IEnumerable<SelectItemModel> GetStateSelectList()
        {
            return _masterData.States.Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.Name
            });
        }

        public IEnumerable<SelectItemModel> GetCountySelectList(int stateId = 0)
        {
            return _masterData.Counties.Where((o => o.StateId == stateId)).Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.Name
            });
        }

        public IEnumerable<CountyEntity> GetAllCounty()
        {
            return _masterData.Counties;
        }

        public IEnumerable<SelectItemModel> GetTitleSelectList(int modelId = 0, bool isActive = true)
        {
            return _masterData.Titles.Where(m => m.model == modelId || modelId == 0)
                .Where(o => o.Status == EntityStatus.Active || isActive == false)
                .Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.Name
            });
        }

        public IEnumerable<SelectItemModelOther> GetTitleSelectListOther(int modelId = 0)
        {
            return _masterData.Titles.Where(m => modelId == 0 || m.model == modelId).ToList()
                .Select(e => new SelectItemModelOther()
                {
                    ID = e.ID,
                    Name = e.Name,
                    Status = e.Status,
                    Other = e.des,
                    OtherId = e.model
                });
        }

        public IEnumerable<SelectItemModel> GetCurriculumSelectList(bool isActive = true)
        {
            IQueryable<CurriculumEntity> query = isActive
                ? _masterData.Curriculums.Where(o => o.Name.ToLower() != "none" && o.Status == EntityStatus.Active)
                : _masterData.Curriculums.Where(o => o.Name.ToLower() != "none");

            return query.Select(e => new SelectItemModel()
            {
                ID = e.ID,
                Name = e.Name
            });
        }

        public IEnumerable<SelectItemModel> GetCurriculumSelectListForSchool(bool isContansNone = true)
        {
            if (isContansNone)
            {
                return _masterData.Curriculums.Where(o => o.Name.ToLower() != "other").Select(e => new SelectItemModel()
                {
                    ID = e.ID,
                    Name = e.Name
                });
            }
            else
            {
                return _masterData.Curriculums.Where(o => o.Name.ToLower() != "none" && o.Name.ToLower() != "other").
                    Select(e => new SelectItemModel()
                {
                    ID = e.ID,
                    Name = e.Name
                });
            }

        }

        #endregion

        #region SelectListOther
        public IEnumerable<SelectItemModelOther> GetFundingSelectListOther()
        {
            return _masterData.Fundings.Select(e => new SelectItemModelOther()
            {
                ID = e.ID,
                Name = e.Name,
                Status = e.Status
            });
        }

        public IEnumerable<SelectItemModelOther> GetLanguageSelectListOther()
        {
            return _masterData.Languages.Select(e => new SelectItemModelOther()
            {
                ID = e.ID,
                Name = e.Language,
                Status = e.Status
            });
        }

        public IEnumerable<SelectItemModelOther> GetCurriculumSelectListOther()
        {
            return _masterData.Curriculums.Select(e => new SelectItemModelOther()
            {
                ID = e.ID,
                Name = e.Name,
                Status = e.Status
            });
        }
        #endregion

    }
}
