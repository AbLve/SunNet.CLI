using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			Jackz
 * CreatedOn:		2014/8/18 21:41:50
 * Description:		Please input class summary
 * Version History:	Created,2014/8/18 21:41:50
 * 
 * 
 **************************************************************************/
using Sunnet.Cli.Core.Ade.Configurations;
using Sunnet.Cli.Core.Cec.Configurations;
using Sunnet.Cli.Core.Communities.Configurations;
using Sunnet.Cli.Core.Cot.Configurations;
using Sunnet.Cli.Core.Cpalls.Configurations;
using Sunnet.Cli.Core.Observable.Configurations;
using Sunnet.Cli.Core.Reports.Configurations;
using Sunnet.Cli.Core.Trs.Configurations;
using Sunnet.Cli.Core.Ade.Configurations.Items;
using Sunnet.Cli.Core.Ade.Entities;
using Sunnet.Cli.Core.Tsds.Configurations;
using Sunnet.Cli.Core.Vcw.Configurations;
using Sunnet.Cli.Core.Practices.Configurations;

namespace Sunnet.Cli.Core
{
    public class AdeDbContext : DbContext
    {
        public AdeDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

            Database.SetInitializer<AdeDbContext>(null);
        }

        public AdeDbContext()
            : base("AdeDbContext")
        {
            Database.SetInitializer<AdeDbContext>(null);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            // Ade : start
            new AdeLinkCnfg().RegistTo(modelBuilder.Configurations);
            new AnswerCnfg().RegistTo(modelBuilder.Configurations);
            new AssessmentCnfg().RegistTo(modelBuilder.Configurations);
            new CutOffScoreCnfg().RegistTo(modelBuilder.Configurations);
            new MeasureCnfg().RegistTo(modelBuilder.Configurations);
            new AssessmentReportCnfg().RegistTo(modelBuilder.Configurations);
            new AssessmentLegendCnfg().RegistTo(modelBuilder.Configurations);

            new ItemBaseCnfg().RegistTo(modelBuilder.Configurations);
            new TxkeaBupTaskCnfg().RegistTo(modelBuilder.Configurations);
            new TxkeaBupLogCnfg().RegistTo(modelBuilder.Configurations);
            // other item type
            new CecItemCnfg().RegistTo(modelBuilder.Configurations);
            new CotItemCnfg().RegistTo(modelBuilder.Configurations);
            new MultipleChoicesItemCnfg().RegistTo(modelBuilder.Configurations);
            new PaItemCnfg().RegistTo(modelBuilder.Configurations);
            new QuadrantItemCnfg().RegistTo(modelBuilder.Configurations);
            new RapidExpressiveItemCnfg().RegistTo(modelBuilder.Configurations);
            new ReceptiveItemCnfg().RegistTo(modelBuilder.Configurations);
            new ReceptivePromptItemCnfg().RegistTo(modelBuilder.Configurations);
            new ChecklistItemCnfg().RegistTo(modelBuilder.Configurations);
            new DirectionItemCnfg().RegistTo(modelBuilder.Configurations);

            new TypedResponseItemCnfg().RegistTo(modelBuilder.Configurations);
            new TypedResponseCnfg().RegistTo(modelBuilder.Configurations);
            new TypedResponseOptionCnfg().RegistTo(modelBuilder.Configurations);

            new TxkeaReceptiveItemCnfg().RegistTo(modelBuilder.Configurations);
            new TxkeaExpressiveItemCnfg().RegistTo(modelBuilder.Configurations);
            new TxkeaExpressiveResponseCnfg().RegistTo(modelBuilder.Configurations);
            new TxkeaExpressiveImageCnfg().RegistTo(modelBuilder.Configurations);
            new TxkeaExpressiveResponseOptionCnfg().RegistTo(modelBuilder.Configurations);

            new TxkeaLayoutCnfg().RegistTo(modelBuilder.Configurations);
            new BranchingScoreCnfg().RegistTo(modelBuilder.Configurations);

            new ObservableChoiceItemCnfg().RegistTo(modelBuilder.Configurations);
            new ObservableEntryItemCnfg().RegistTo(modelBuilder.Configurations);

            new BenchmarkCnfg().RegistTo(modelBuilder.Configurations);
            new PercentileRankLookupCnfg().RegistTo(modelBuilder.Configurations);



            new ScoreCnfg().RegistTo(modelBuilder.Configurations);
            new ScoreAgeOrWaveBandsCnfg().RegistTo(modelBuilder.Configurations);
            new ScoreMeasureOrDefineCoefficientsCnfg().RegistTo(modelBuilder.Configurations);

            // Ade : end




            // Assessment : start
            new StudentAssessmentCnfg().RegistTo(modelBuilder.Configurations);
            new StudentItemCnfg().RegistTo(modelBuilder.Configurations);
            new StudentMeasureCnfg().RegistTo(modelBuilder.Configurations);
            new CpallsSchoolCnfg().RegistTo(modelBuilder.Configurations);
            new CpallsSchoolMeasureCnfg().RegistTo(modelBuilder.Configurations);
            new CpallsClassCnfg().RegistTo(modelBuilder.Configurations);
            new CpallsClassMeasureCnfg().RegistTo(modelBuilder.Configurations);
            new CpallsStudentGroupCnfg().RegistTo(modelBuilder.Configurations);
            new MeasureClassGroupCnfg().RegistTo(modelBuilder.Configurations);
            // Assessment : end

            //Cec :start
            new CecHistoryCnfg().RegistTo(modelBuilder.Configurations);
            new CecResultCnfg().RegistTo(modelBuilder.Configurations);
            //Cec :end

            // Cot: start
            new CotAssessmentCnfg().RegistTo(modelBuilder.Configurations);
            new CotAssessmentItemCnfg().RegistTo(modelBuilder.Configurations);
            new CotAssessmentWaveItemCnfg().RegistTo(modelBuilder.Configurations);
            new CotStgReportCnfg().RegistTo(modelBuilder.Configurations);
            new CotWaveCnfg().RegistTo(modelBuilder.Configurations);
            new CotStgReportItemCnfg().RegistTo(modelBuilder.Configurations);
            new CotStgGroupCnfg().RegistTo(modelBuilder.Configurations);
            new CotStgGroupItemCnfg().RegistTo(modelBuilder.Configurations);
            // Cot: end


            //TRS begin
            new TRSAnswerCnfg().RegistTo(modelBuilder.Configurations);
            new TRSAssessmentCnfg().RegistTo(modelBuilder.Configurations);
            new TRSAssessmentItemCnfg().RegistTo(modelBuilder.Configurations);
            new TrsStarCnfg().RegistTo(modelBuilder.Configurations);
            new TRSItemCnfg().RegistTo(modelBuilder.Configurations);
            new TRSItemAnswerCnfg().RegistTo(modelBuilder.Configurations);
            new TRSSubcategoryCnfg().RegistTo(modelBuilder.Configurations);
            new TRSAssessmentClassCnfg().RegistTo(modelBuilder.Configurations);
            new TRSEventLogCnfg().RegistTo(modelBuilder.Configurations);
            new TRSNotificationCnfg().RegistTo(modelBuilder.Configurations);
            new TRSEventLogFileCnfg().RegistTo(modelBuilder.Configurations);
            //TRS end

            //Wavelog
            new WaveLogCnfg().RegistTo(modelBuilder.Configurations);

            new UserShownMeasuresCnfg().RegistTo(modelBuilder.Configurations);

            //Observable
            new ObservableAssessmentCnfg().RegistTo(modelBuilder.Configurations);
            new ObservableAssessmentItemCnfg().RegistTo(modelBuilder.Configurations);
            new ObservableItemsHistoryCnfg().RegistTo(modelBuilder.Configurations);

            //Tsds
            new TsdsAssessmentFileCnfg().RegistTo(modelBuilder.Configurations);
            new TsdsCnfg().RegistTo(modelBuilder.Configurations);
            new TsdsMapCnfg().RegistTo(modelBuilder.Configurations);
            new V_CommunityCnfg().RegistTo(modelBuilder.Configurations);
            new V_UserCnfg().RegistTo(modelBuilder.Configurations);

            //Practice
            new DemoStudentCnfg().RegistTo(modelBuilder.Configurations);
            new PracticeStudentAssessmentCnfg().RegistTo(modelBuilder.Configurations);
            new PracticeStudentMeasureCnfg().RegistTo(modelBuilder.Configurations);
            new PracticeStudentItemCnfg().RegistTo(modelBuilder.Configurations);
            new CustomGroupMyActivityCnfg().RegistTo(modelBuilder.Configurations);

            modelBuilder.Entity<ScoreMeasureOrDefineCoefficientsEntity>().Property(x => x.Coefficient).HasPrecision(6, 3);
        }
    }
}
