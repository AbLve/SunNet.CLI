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
using Sunnet.Framework.Core.Tool;
using Sunnet.Cli.Core.MasterData.Entities;

namespace Sunnet.Cli.Core.MasterData
{
    public interface IMasterDataContract
    {
        IQueryable<LanguageEntity> Languages { get; }
        IQueryable<FundingEntity> Fundings { get; }
        IQueryable<StateEntity> States { get; }
        IQueryable<CountyEntity> Counties { get; }
        IQueryable<CountryEntity> AllCountries { get; }
        /// <summary>
        /// model: 1 = Community / District Primary Contact ; 2=Community / District Secondary Contact
        /// 3 =School Primary Contact 4=School Secondary  Contact  5=Community/District User Groups
        /// 6 = Community/District Specialist User Groups  7 =Principal / Director User Groups
        /// 8=School Specialist User Groups
        /// </summary>
        IQueryable<TitleEntity> Titles { get; }
        IQueryable<CurriculumEntity> Curriculums { get; }

        LanguageEntity NewLanguageEntity();
        OperationResult InsertLanguage(LanguageEntity entity);
        OperationResult DeleteLanguage(int id);
        OperationResult UpdateLanguage(LanguageEntity entity);
        LanguageEntity GetLanguage(int id);

        FundingEntity NewFundingEntity();
        OperationResult InsertFunding(FundingEntity entity);
        OperationResult DeleteFunding(int id);
        OperationResult UpdateFunding(FundingEntity entity);
        FundingEntity GetFunding(int id);

        StateEntity NewStateEntity();
        OperationResult InsertState(StateEntity entity);
        OperationResult DeleteState(int id);
        OperationResult UpdateState(StateEntity entity);
        StateEntity GetState(int id);

        CountryEntity GetCountry(int id);

        CountyEntity NewCountyEntity();
        OperationResult InsertCounty(CountyEntity entity);
        OperationResult DeleteCounty(int id);
        OperationResult UpdateCounty(CountyEntity entity);
        CountyEntity GetCounty(int id);

        TitleEntity NewTitleEntity();
        OperationResult InsertTitle(TitleEntity entity);
        OperationResult DeleteTitle(int id);
        OperationResult UpdateTitle(TitleEntity entity);
        TitleEntity GetTitle(int id);

        CurriculumEntity NewCurriculumEntity();
        CurriculumEntity GetCurriculum(int id);
        OperationResult InsertCurriculum(CurriculumEntity entity);
        OperationResult UpdateCurriculum(CurriculumEntity entity);

    }
}
