using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/**************************************************************************
 * Developer: 		Lee
 * Computer:		Lee-PC
 * Domain:			Lee-pc
 * CreatedOn:		2014/8/25 20:27:20
 * Description:		Create IMasterDataContract
 * Version History:	Created,2014/8/25 20:27:20
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.MasterData.Interfaces;
using Sunnet.Framework.Core.Base;
using Sunnet.Framework.EmailSender;
using Sunnet.Framework.Encrypt;
using Sunnet.Framework.File;
using Sunnet.Framework.Log;
using Sunnet.Cli.Core.MasterData.Entities;
using Sunnet.Framework.Core.Tool;

namespace Sunnet.Cli.Core.MasterData
{
    internal class MasterDataServer : CoreServiceBase, IMasterDataContract
    {

        private readonly ILanguageRpst _languageRpst;
        private readonly IFundingRpst _fundingRpst;
        private readonly IStateRpst _stateRpst;
        private readonly ICountryRpst _countryRpst;
        private readonly ICountyRpst _countyRpst;
        private readonly ITitleRpst _titleRpst;
        private readonly ICurriculumRpst _curriculumRpst;

        public MasterDataServer(ISunnetLog log, IFile fileHelper, IEmailSender emailSender, IEncrypt encrypt,
        ILanguageRpst languageRpst, IStateRpst stateRpst, ICountryRpst countryRpst, IFundingRpst fundingRps
            , ICountyRpst countyRpst, ITitleRpst titleRpst, ICurriculumRpst curriculumRpst,
            IUnitOfWork unit)
        {
            _languageRpst = languageRpst;
            _fundingRpst = fundingRps;
            _countryRpst = countryRpst;
            _stateRpst = stateRpst;
            _countyRpst = countyRpst;
            _titleRpst = titleRpst;
            _curriculumRpst = curriculumRpst;
            UnitOfWork = unit;
        }


        #region IQueryable
        public IQueryable<LanguageEntity> Languages
        {
            get { return _languageRpst.Entities; }
        }

        public IQueryable<FundingEntity> Fundings
        {
            get { return _fundingRpst.Entities; }
        }

        public IQueryable<CountyEntity> Counties
        {
            get { return _countyRpst.Entities; }
        }

        public IQueryable<StateEntity> States
        {
            get { return _stateRpst.Entities; }
        }

        public IQueryable<CountryEntity> AllCountries
        {
            get { return _countryRpst.Entities; }
        }

        public IQueryable<TitleEntity> Titles
        {
            get { return _titleRpst.Entities; }
        }

        public IQueryable<CurriculumEntity> Curriculums
        {
            get { return _curriculumRpst.Entities; }
        }
        #endregion

        #region Language
        public LanguageEntity NewLanguageEntity()
        {
            return _languageRpst.Create();
        }

        public OperationResult InsertLanguage(LanguageEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _languageRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteLanguage(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _languageRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateLanguage(LanguageEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _languageRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public LanguageEntity GetLanguage(int id)
        {
            try
            {
                return _languageRpst.GetByKey(id);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return null;
            }
        }
        #endregion

        #region Funding
        public FundingEntity NewFundingEntity()
        {
            return _fundingRpst.Create();
        }

        public OperationResult InsertFunding(FundingEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _fundingRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteFunding(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _fundingRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateFunding(FundingEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _fundingRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public FundingEntity GetFunding(int id)
        {
            return _fundingRpst.GetByKey(id);
        }
        #endregion

        #region State
        public StateEntity NewStateEntity()
        {
            return _stateRpst.Create();
        }

        public OperationResult InsertState(StateEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _stateRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteState(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _stateRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateState(StateEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _stateRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public StateEntity GetState(int id)
        {
            return _stateRpst.GetByKey(id);
        }
        #endregion

        #region Country
        public CountryEntity GetCountry(int id)
        {
            return _countryRpst.GetByKey(id);
        }

        #endregion

        #region County
        public CountyEntity NewCountyEntity()
        {
            return _countyRpst.Create();
        }

        public OperationResult InsertCounty(CountyEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _countyRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteCounty(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _countyRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateCounty(CountyEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _countyRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public CountyEntity GetCounty(int id)
        {
            return _countyRpst.GetByKey(id);
        }
        #endregion

        #region Title
        public TitleEntity NewTitleEntity()
        {
            return _titleRpst.Create();
        }

        public OperationResult InsertTitle(TitleEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _titleRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult DeleteTitle(int id)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _titleRpst.Delete(id);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public OperationResult UpdateTitle(TitleEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _titleRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }

        public TitleEntity GetTitle(int id)
        {
            return _titleRpst.GetByKey(id);
        }
        #endregion

        #region Curriculum
        public CurriculumEntity NewCurriculumEntity()
        {
            return _curriculumRpst.Create();
        }

        public OperationResult InsertCurriculum(CurriculumEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _curriculumRpst.Insert(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public OperationResult UpdateCurriculum(CurriculumEntity entity)
        {
            var result = new OperationResult(OperationResultType.Success);
            try
            {
                _curriculumRpst.Update(entity);
            }
            catch (Exception ex)
            {
                result = ResultError(ex);
            }
            return result;
        }
        public CurriculumEntity GetCurriculum(int id)
        {
            return _curriculumRpst.GetByKey(id);
        }

        #endregion

    }
}
