using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using StructureMap;
using Sunnet.Framework;
using Sunnet.Framework.Log;

namespace Sunnet.Cli.UIBase
{
    public class BundleConfig
    {
        static long Key = DateTime.Now.Ticks;
        public static string UpdateKey
        {
            get
            {
                return Key.ToString();
            }
        }

        public static string GetCndPath(string relativePath)
        {
            return string.Format("{0}{1}?v={2}", SFConfig.StaticDomain, relativePath, UpdateKey);
        }
        public static string GetCndPath(string relativePath, string domain)
        {
            return string.Format("{0}{1}?v={2}", domain, relativePath, UpdateKey);
        }

        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            //bundles.IgnoreList.Ignore("*-vsdoc.js");
            //bundles.IgnoreList.Ignore("*intellisense.js");

            bundles.Add(new ScriptBundle("~/scripts/modernizr", GetCndPath("scripts/modernizr")).Include(
                "~/Content/scripts/modernizr-2.7.2.js"));

            bundles.Add(new ScriptBundle("~/scripts/modernizr/offline", GetCndPath("scripts/modernizr/offline", SFConfig.AssessmentDomain)).Include(
                "~/Content/scripts/modernizr-2.7.2.js"));
            bundles.Add(new ScriptBundle("~/scripts/modernizr/practiceoffline", GetCndPath("scripts/modernizr/practiceoffline", SFConfig.PracticeDomain)).Include(
                "~/Content/scripts/modernizr-2.7.2.js"));


            bundles.Add(new ScriptBundle("~/scripts/ie68", GetCndPath("scripts/ie68")).Include(
                "~/Content/scripts/html5shiv.min.js",
                "~/Content/scripts/respond.min.js",
                "~/Content/scripts/respond.matchmedia.addListener.min.js"));
#if DEBUG
            bundles.Add(new ScriptBundle("~/scripts/jquery", GetCndPath("scripts/jquery")).Include(
                "~/Content/scripts/jquery-1.10.2.js"));
            bundles.Add(new ScriptBundle("~/scripts/jquery/offline", GetCndPath("scripts/jquery/offline", SFConfig.AssessmentDomain)).Include(
                "~/Content/scripts/jquery-1.10.2.js"));
            bundles.Add(new ScriptBundle("~/scripts/jquery/practiceoffline", GetCndPath("scripts/jquery/practiceoffline", SFConfig.PracticeDomain)).Include(
                "~/Content/scripts/jquery-1.10.2.js"));

            bundles.Add(new ScriptBundle("~/scripts/hammer", GetCndPath("scripts/jquery")).Include(
                "~/Content/scripts/hammer.js",
                "~/Content/scripts/jquery.hammer.js"));
            bundles.Add(new ScriptBundle("~/scripts/hammer/offline", GetCndPath("scripts/hammer/offline", SFConfig.AssessmentDomain)).Include(
                "~/Content/scripts/hammer.js",
                "~/Content/scripts/jquery.hammer.js"));

            bundles.Add(new ScriptBundle("~/scripts/jquery_val", GetCndPath("scripts/jquery_val")).Include(
                "~/Content/scripts/jquery.validate.js",
                "~/Content/scripts/additional-methods.js",
                "~/Content/scripts/jquery.validate.unobtrusive.js",
                "~/Content/scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/scripts/jquery_val/offline", GetCndPath("scripts/jquery_val/offline", SFConfig.AssessmentDomain)).Include(
                "~/Content/scripts/jquery.validate.js",
                "~/Content/scripts/additional-methods.js",
                "~/Content/scripts/jquery.validate.unobtrusive.js",
                "~/Content/scripts/jquery.unobtrusive-ajax.js"));
            bundles.Add(new ScriptBundle("~/scripts/jquery_val/practiceoffline", GetCndPath("scripts/jquery_val/practiceoffline", SFConfig.PracticeDomain)).Include(
                "~/Content/scripts/jquery.validate.js",
                "~/Content/scripts/additional-methods.js",
                "~/Content/scripts/jquery.validate.unobtrusive.js",
                "~/Content/scripts/jquery.unobtrusive-ajax.js"));

            bundles.Add(new ScriptBundle("~/scripts/bootstrap", GetCndPath("scripts/bootstrap")).Include(
                "~/Content/scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/scripts/bootstrap/offline", GetCndPath("scripts/bootstrap/offline", SFConfig.AssessmentDomain)).Include(
                "~/Content/scripts/bootstrap.js"));
            bundles.Add(new ScriptBundle("~/scripts/bootstrap/practiceoffline", GetCndPath("scripts/bootstrap/practiceoffline", SFConfig.PracticeDomain)).Include(
                "~/Content/scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/scripts/knockout", GetCndPath("scripts/knockout")).Include(
                "~/Content/scripts/knockout-3.2.0.debug.js",
                "~/Content/scripts/knockout.mapping-latest.debug.js"));

            bundles.Add(new ScriptBundle("~/scripts/knockout/offline", GetCndPath("scripts/knockout/offline", SFConfig.AssessmentDomain)).Include(
                "~/Content/scripts/knockout-3.2.0.debug.js",
                "~/Content/scripts/knockout.mapping-latest.debug.js"));
            bundles.Add(new ScriptBundle("~/scripts/knockout/practiceoffline", GetCndPath("scripts/knockout/practiceoffline", SFConfig.PracticeDomain)).Include(
                "~/Content/scripts/knockout-3.2.0.debug.js",
                "~/Content/scripts/knockout.mapping-latest.debug.js"));

            bundles.Add(new ScriptBundle("~/scripts/upload", GetCndPath("scripts/upload")).Include(
                "~/Content/scripts/webuploader/webuploader.js",
                "~/Content/scripts/webuploader/uploader.js"));

            bundles.Add(new ScriptBundle("~/scripts/format", GetCndPath("scripts/format")).Include(
                "~/Content/scripts/datetime/Cli_WdatePicke.js",
                "~/Content/scripts/jquery.maskedinput.js",
                "~/Content/scripts_cliProject/jQuery.maskedInput.js"));

            bundles.Add(new ScriptBundle("~/scripts/format/offline", GetCndPath("scripts/format/offline", SFConfig.AssessmentDomain)).Include(
                "~/Content/scripts/datetime/Cli_WdatePicke.js",
                "~/Content/scripts/jquery.maskedinput.js",
                "~/Content/scripts_cliProject/jQuery.maskedInput.js"));
            bundles.Add(new ScriptBundle("~/scripts/format/practiceoffline", GetCndPath("scripts/format/practiceoffline", SFConfig.PracticeDomain)).Include(
                "~/Content/scripts/datetime/Cli_WdatePicke.js",
                "~/Content/scripts/jquery.maskedinput.js",
                "~/Content/scripts_cliProject/jQuery.maskedInput.js"));
#endif
#if !DEBUG
            bundles.Add(new ScriptBundle("~/scripts/jquery", GetCndPath("scripts/jquery")).Include(
                "~/Content/scripts/jquery-1.10.2.min.js"));

            bundles.Add(new ScriptBundle("~/scripts/jquery/offline", GetCndPath("scripts/jquery/offline", SFConfig.AssessmentDomain)).Include(
                "~/Content/scripts/jquery-1.10.2.min.js"));
            bundles.Add(new ScriptBundle("~/scripts/jquery/practiceoffline", GetCndPath("scripts/jquery/practiceoffline", SFConfig.PracticeDomain)).Include(
                "~/Content/scripts/jquery-1.10.2.min.js"));
            
            bundles.Add(new ScriptBundle("~/scripts/hammer", GetCndPath("scripts/hammer")).Include(
                "~/Content/scripts/hammer.min.js",
                "~/Content/scripts/jquery.hammer.js"));

            bundles.Add(new ScriptBundle("~/scripts/hammer/offline", GetCndPath("scripts/hammer/offline", SFConfig.AssessmentDomain)).Include(
                "~/Content/scripts/hammer.min.js",
                "~/Content/scripts/jquery.hammer.js"));

            bundles.Add(new ScriptBundle("~/scripts/jquery_val", GetCndPath("scripts/jquery_val")).Include(
                "~/Content/scripts/jquery.validate.min.js",
                "~/Content/scripts/additional-methods.min.js",
                "~/Content/scripts/jquery.validate.unobtrusive.min.js",
                "~/Content/scripts/jquery.unobtrusive-ajax.min.js"));

             


            bundles.Add(new ScriptBundle("~/scripts/jquery_val/offline", GetCndPath("scripts/jquery_val/offline",SFConfig.AssessmentDomain)).Include(
                "~/Content/scripts/jquery.validate.min.js",
                "~/Content/scripts/additional-methods.min.js",
                "~/Content/scripts/jquery.validate.unobtrusive.min.js",
                "~/Content/scripts/jquery.unobtrusive-ajax.min.js"));
            bundles.Add(new ScriptBundle("~/scripts/jquery_val/practiceoffline", GetCndPath("scripts/jquery_val/practiceoffline", SFConfig.PracticeDomain)).Include(
                "~/Content/scripts/jquery.validate.min.js",
                "~/Content/scripts/additional-methods.min.js",
                "~/Content/scripts/jquery.validate.unobtrusive.min.js",
                "~/Content/scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new ScriptBundle("~/scripts/bootstrap", GetCndPath("scripts/bootstrap")).Include(
                "~/Content/scripts/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/scripts/bootstrap/offline", GetCndPath("scripts/bootstrap/offline", SFConfig.AssessmentDomain)).Include(
                "~/Content/scripts/bootstrap.min.js"));
            bundles.Add(new ScriptBundle("~/scripts/bootstrap/practiceoffline", GetCndPath("scripts/bootstrap/practiceoffline", SFConfig.PracticeDomain)).Include(
                "~/Content/scripts/bootstrap.min.js"));

            bundles.Add(new ScriptBundle("~/scripts/knockout", GetCndPath("scripts/knockout")).Include(
                "~/Content/scripts/knockout-3.2.0.js",
                "~/Content/scripts/knockout.mapping-latest.js"));

            bundles.Add(new ScriptBundle("~/scripts/knockout/offline", GetCndPath("scripts/knockout/offline", SFConfig.AssessmentDomain)).Include(
                "~/Content/scripts/knockout-3.2.0.js",
                "~/Content/scripts/knockout.mapping-latest.js"));
            bundles.Add(new ScriptBundle("~/scripts/knockout/practiceoffline", GetCndPath("scripts/knockout/practiceoffline", SFConfig.PracticeDomain)).Include(
                "~/Content/scripts/knockout-3.2.0.js",
                "~/Content/scripts/knockout.mapping-latest.js"));

            bundles.Add(new ScriptBundle("~/scripts/upload", GetCndPath("scripts/upload")).Include(
                "~/Content/scripts/webuploader/webuploader.min.js",
                "~/Content/scripts/webuploader/uploader.js"));

            bundles.Add(new ScriptBundle("~/scripts/format", GetCndPath("scripts/format")).Include(
                "~/Content/scripts/datetime/Cli_WdatePicke.js",
                "~/Content/scripts/jquery.maskedinput.min.js",
                "~/Content/scripts_cliProject/jQuery.maskedInput.js"));

            bundles.Add(new ScriptBundle("~/scripts/format/offline", GetCndPath("scripts/format/offline",SFConfig.AssessmentDomain)).Include(
                "~/Content/scripts/datetime/Cli_WdatePicke.js",
                "~/Content/scripts/jquery.maskedinput.min.js",
                "~/Content/scripts_cliProject/jQuery.maskedInput.js"));
            bundles.Add(new ScriptBundle("~/scripts/format/practiceoffline", GetCndPath("scripts/format/practiceoffline", SFConfig.PracticeDomain)).Include(
                "~/Content/scripts/datetime/Cli_WdatePicke.js",
                "~/Content/scripts/jquery.maskedinput.min.js",
                "~/Content/scripts_cliProject/jQuery.maskedInput.js"));
#endif


            bundles.Add(new ScriptBundle("~/scripts/ckeditor", GetCndPath("scripts/ckeditor")).Include(
                "~/Content/lib/ckeditor/ckeditor.js",
                "~/Content/lib/ckeditor/styles.js",
                "~/Content/lib/ckeditor/config.js"));

            bundles.Add(new ScriptBundle("~/scripts/ckeditor/offline", GetCndPath("scripts/ckeditor/offline", SFConfig.AssessmentDomain)).Include(
                "~/Content/lib/ckeditor/ckeditor.js",
                "~/Content/lib/ckeditor/styles.js",
                "~/Content/lib/ckeditor/config.js"));

            bundles.Add(new ScriptBundle("~/scripts/cli", GetCndPath("scripts/cli")).Include(
                            "~/Content/scripts_cliProject/JsMsgs.js",
                            "~/Content/scripts_cliProject/prototype.js",
                            "~/Content/scripts_cliProject/cli.jQuery.js",
                            "~/Content/scripts_cliProject/bindingHandlers.js",
                            "~/Content/scripts_cliProject/ko_viewModel.js",
                            "~/Content/Scripts/html.sortable.js",
                            "~/Content/Scripts/jquery.tablednd.js"));
            bundles.Add(new ScriptBundle("~/scripts/cli/offline", GetCndPath("scripts/cli/offline", SFConfig.AssessmentDomain)).Include(
                            "~/Content/scripts_cliProject/JsMsgs.js",
                            "~/Content/scripts_cliProject/prototype.js",
                            "~/Content/scripts_cliProject/cli.jQuery.js",
                            "~/Content/scripts_cliProject/bindingHandlers.js",
                            "~/Content/scripts_cliProject/ko_viewModel.js",
                            "~/Content/Scripts/html.sortable.min.js",
                            "~/Content/Scripts/jquery.tablednd.min.js"));
            bundles.Add(new ScriptBundle("~/scripts/cli/practiceoffline", GetCndPath("scripts/cli/practiceoffline", SFConfig.PracticeDomain)).Include(
                            "~/Content/scripts_cliProject/JsMsgs.js",
                            "~/Content/scripts_cliProject/prototype.js",
                            "~/Content/scripts_cliProject/cli.jQuery.js",
                            "~/Content/scripts_cliProject/bindingHandlers.js",
                            "~/Content/scripts_cliProject/ko_viewModel.js",
                            "~/Content/Scripts/html.sortable.min.js",
                            "~/Content/Scripts/jquery.tablednd.min.js"));

            bundles.Add(new ScriptBundle("~/scripts/ade", GetCndPath("scripts/ade")).Include(
                "~/Content/scripts_cliProject/Module_AssPub.js",
                "~/Content/scripts_cliProject/Module_Ade.js"));


            bundles.Add(new ScriptBundle("~/scripts/report/assessment", GetCndPath("scripts/report/assessment")).Include(
                           "~/Content/scripts_cliProject/Module_AssReport.js"));

            bundles.Add(new ScriptBundle("~/scripts/module_school", GetCndPath("scripts/module_school")).Include(
                            "~/Content/scripts_cliProject/Module_school.js"));

            bundles.Add(new ScriptBundle("~/scripts/module_classroom", GetCndPath("scripts/module_classroom")).Include(
                       "~/Content/scripts_cliProject/Module_classroom.js"));

            bundles.Add(new ScriptBundle("~/scripts/module_student", GetCndPath("scripts/module_student")).Include(
                  "~/Content/scripts_cliProject/Module_student.js"));

            bundles.Add(new ScriptBundle("~/scripts/module_class", GetCndPath("scripts/module_class")).Include(
                      "~/Content/scripts_cliProject/Module_class.js"));

            bundles.Add(new ScriptBundle("~/scripts/module_community", GetCndPath("scripts/module_community")).Include(
                      "~/Content/scripts_cliProject/Module_community.js"));

            bundles.Add(new ScriptBundle("~/scripts/module_teacher", GetCndPath("scripts/module_teacher")).Include(
                      "~/Content/scripts_cliProject/Module_teacher.js"));

            bundles.Add(new ScriptBundle("~/scripts/module_communityspecialist", GetCndPath("scripts/module_communityspecialist")).Include(
                      "~/Content/scripts_cliProject/Module_communityspecialist.js"));

            bundles.Add(new ScriptBundle("~/scripts/public", GetCndPath("scripts/public")).Include(
                                  "~/Content/scripts/responsive-nav.js"));

            bundles.Add(new ScriptBundle("~/scripts/chart", GetCndPath("scripts/chart")).Include(
                                  "~/Content/scripts/echarts/echarts-plain-original.js",
                                  "~/Content/scripts/echarts/theme/blue.js",
                                  "~/Content/scripts/echarts/theme/dark.js",
                                  "~/Content/scripts/echarts/theme/gray.js",
                                  "~/Content/scripts/echarts/theme/green.js",
                                  "~/Content/scripts/echarts/theme/infographic.js",
                                  "~/Content/scripts/echarts/theme/macarons.js",
                                  "~/Content/scripts/echarts/theme/red.js",
                                  "~/Content/scripts/echarts/theme/shine.js"));

            bundles.Add(new ScriptBundle("~/scripts/mediaPlayer", GetCndPath("scripts/mediaPlayer")).Include(
                             "~/Content/scripts/mediaPlayer/flowplayer-3.2.6.min.js"));

            bundles.Add(new ScriptBundle("~/scripts/cot", GetCndPath("scripts/cot")).Include(
                                 "~/Content/scripts_cliProject/Module_Cot.js"));
            bundles.Add(new ScriptBundle("~/scripts/cot/report", GetCndPath("scripts/cot/report")).Include(
                "~/Content/scripts_cliProject/Module_Cot_Report.js"));

            bundles.Add(new ScriptBundle("~/scripts/observable", GetCndPath("scripts/observable")).Include(
                              "~/Content/scripts_cliProject/Module_Observable.js"));

            bundles.Add(new ScriptBundle("~/scripts/cpalls",
               GetCndPath("scripts/cpalls")).Include(
               "~/Content/Scripts/md5.js",
               "~/Content/Scripts/js.cookie.js",
                "~/Content/scripts_cliProject/Module_AssPub.js",
                "~/Content/scripts_cliProject/Module_Offline.js",
                "~/Content/scripts_cliProject/Module_Cpalls.js",
                "~/Content/scripts_cliProject/Module_Cpalls_Offline.js"));

            bundles.Add(new ScriptBundle("~/scripts/cpalls/offline",
               GetCndPath("scripts/cpalls/offline", SFConfig.AssessmentDomain)).Include(
               "~/Content/Scripts/md5.js",
               "~/Content/Scripts/js.cookie.js",
                "~/Content/scripts_cliProject/Module_AssPub.js",
                "~/Content/scripts_cliProject/Module_Offline.js",
                "~/Content/scripts_cliProject/Module_Cpalls.js",
                "~/Content/scripts_cliProject/Module_Cpalls_Offline.js"));

            bundles.Add(new ScriptBundle("~/scripts/cpalls/practiceoffline",
         GetCndPath("scripts/cpalls/practiceoffline", SFConfig.PracticeDomain)).Include(
         "~/Content/Scripts/md5.js",
         "~/Content/Scripts/js.cookie.js",
          "~/Content/scripts_cliProject/Module_AssPub.js",
          "~/Content/scripts_cliProject/Module_Offline.js",
          "~/Content/scripts_cliProject/Module_Cpalls.js",
          "~/Content/scripts_cliProject/Module_Practice_Cpalls_Offline.js"));

            bundles.Add(new ScriptBundle("~/scripts/cot/offline",
                GetCndPath("scripts/cot/offline", SFConfig.AssessmentDomain)).Include(
                "~/Content/Scripts/md5.js",
                    "~/Content/Scripts/js.cookie.js",
                    "~/Content/scripts_cliProject/Module_AssPub.js",
                    "~/Content/scripts_cliProject/Module_Cot.js",
                    "~/Content/scripts_cliProject/Module_Offline.js",
                    "~/Content/scripts_cliProject/Module_Cot_offline.js"));

            bundles.Add(new ScriptBundle("~/scripts/cec/offline",
                GetCndPath("scripts/cec/offline", SFConfig.AssessmentDomain)).Include(
                "~/Content/Scripts/md5.js",
                "~/Content/Scripts/js.cookie.js",
                    "~/Content/scripts_cliProject/Module_offline.js",
                    "~/Content/scripts_cliProject/Module_CEC_offline.js"));

            bundles.Add(new ScriptBundle("~/scripts/trs/offline",
                GetCndPath("scripts/trs/offline", SFConfig.AssessmentDomain)).Include(
                "~/Content/Scripts/md5.js",
                "~/Content/Scripts/js.cookie.js",
                "~/Content/scripts_cliProject/Module_trs.js",
                    "~/Content/scripts_cliProject/Module_offline.js",
                    "~/Content/scripts_cliProject/Module_TRS_offline.js"));

            bundles.Add(new ScriptBundle("~/scripts/FixedTable", GetCndPath("scripts/FixedTable")).Include(
                "~/Content/scripts_cliProject/JQuery_fixedtable.js"));

            bundles.Add(new ScriptBundle("~/scripts/FixedTableCpalls", GetCndPath("scripts/FixedTableCpalls")).Include(
                "~/Content/scripts_cliProject/JQuery_fixedtable_cpalls.js"));

            bundles.Add(new ScriptBundle("~/scripts/vcw_upload", GetCndPath("scripts/vcw_upload")).Include(
               "~/Content/scripts_cliProject/Vcw_Upload.js"));

            bundles.Add(new ScriptBundle("~/scripts/vcw_upload_file", GetCndPath("scripts/vcw_upload_file")).Include(
               "~/Content/scripts_cliProject/Vcw_Upload_File.js"));

            bundles.Add(new ScriptBundle("~/scripts/vcw_upload_feedback", GetCndPath("scripts/vcw_upload_feedback")).Include(
               "~/Content/scripts_cliProject/Vcw_Upload_Feedback.js"));

            bundles.Add(new ScriptBundle("~/scripts/vcwcommon", GetCndPath("scripts/vcwcommon")).Include(
               "~/Content/scripts_cliProject/VcwCommon.js"));

            bundles.Add(new ScriptBundle("~/scripts/trs", GetCndPath("scripts/trs")).Include(
               "~/Content/scripts_cliProject/Module_trs.js"));

            bundles.Add(new ScriptBundle("~/scripts/colorpicker", GetCndPath("scripts/colorpicker")).Include(
               "~/Content/scripts/jquery-ui.js",
               "~/Content/scripts/colorpicker/js/evol.colorpicker.js"));

            bundles.Add(new ScriptBundle("~/scripts/fabric", GetCndPath("scripts/fabric")).Include(
               "~/Content/scripts/fabric/fabric.js",
               "~/Content/scripts_cliProject/CanvasFunction.js"));

            bundles.Add(new ScriptBundle("~/scripts/fullscreen", GetCndPath("scripts/fullscreen")).Include(
               "~/Content/scripts_cliProject/FullScreen.js"));

            bundles.Add(new ScriptBundle("~/scripts/TxkeaExpressive", GetCndPath("scripts/TxkeaExpressive")).Include(
 "~/Content/scripts_cliProject/Module_Ade_TxkeaExpressive.js"));

            bundles.Add(new ScriptBundle("~/scripts/TxkeaReceptive", GetCndPath("scripts/TxkeaReceptive")).Include(
 "~/Content/scripts_cliProject/Module_Ade_TxkeaReceptive.js"));

            bundles.Add(new ScriptBundle("~/scripts/MultiSelect", GetCndPath("scripts/MultiSelect")).Include(
 "~/Content/Scripts/bootstrap-multiselect.js"));

            bundles.Add(new ScriptBundle("~/scripts/CustomScore", GetCndPath("scripts/CustomScore")).Include(
 "~/Content/scripts_cliProject/Module_Ade_CustomScore.js"));
            // css:start

            bundles.Add(new StyleBundle("~/css/basic", GetCndPath("css/basic")).Include(
                "~/Content/Site.css",
                "~/Content/lib/Font-Awesome-3.2.1/css/font-awesome.min.css",
                "~/Content/style/m_fixed_columns.css",
                "~/Content/style/m_inline_edit.css",
                "~/Content/style/layout.css",
                "~/Content/lib/bootstrap/css/bootstrap.css",
                "~/Content/style/module.css",
                "~/Content/style/home_module.css"
                ));

            bundles.Add(new StyleBundle("~/css/trs", GetCndPath("css/trs")).Include(
                "~/Content/style/trs.css"
                ));

            bundles.Add(new StyleBundle("~/css/trs/offline", GetCndPath("css/trs/offline", SFConfig.AssessmentDomain)).Include(
                "~/Content/style/trs.css"
                ));

            bundles.Add(new StyleBundle("~/css/assessment/offline", GetCndPath("css/assessment/offline", SFConfig.AssessmentDomain)).Include(
                "~/Content/style/m_offline.css"
               ));
            bundles.Add(new StyleBundle("~/css/assessment/practiceoffline", GetCndPath("css/assessment/practiceoffline", SFConfig.PracticeDomain)).Include(
                "~/Content/style/m_offline.css"
               ));

            bundles.Add(new StyleBundle("~/css/basic/offline", GetCndPath("css/basic/offline", SFConfig.AssessmentDomain)).Include(
               "~/Content/Site.css",
               "~/Content/lib/bootstrap/css/bootstrap.css",
               "~/Content/lib/Font-Awesome-3.2.1/css/font-awesome.min.css",
               "~/Content/style/layout.css",
               "~/Content/style/module.css",
               "~/Content/style/m_fixed_columns.css",
               "~/Content/style/m_inline_edit.css",
               "~/Content/style/m_offline.css",
               "~/Content/lib/bootstrap/css/bootstrap.css",
                "~/Content/style/home_module.css"
               ));
            bundles.Add(new StyleBundle("~/css/basic/practiceoffline", GetCndPath("css/basic/practiceoffline", SFConfig.PracticeDomain)).Include(
               "~/Content/Site.css",
               "~/Content/lib/bootstrap/css/bootstrap.css",
               "~/Content/lib/Font-Awesome-3.2.1/css/font-awesome.min.css",
               "~/Content/style/layout.css",
               "~/Content/style/module.css",
               "~/Content/style/m_fixed_columns.css",
               "~/Content/style/m_inline_edit.css",
               "~/Content/style/m_offline.css",
               "~/Content/lib/bootstrap/css/bootstrap.css",
                "~/Content/style/home_module.css"
               ));

            bundles.Add(new StyleBundle("~/css/cpalls", GetCndPath("css/cpalls")).Include(
               "~/Content/style/module_Cpalls.css"
               ));
            bundles.Add(new StyleBundle("~/css/cpalls/offline", GetCndPath("css/cpalls/offline", SFConfig.AssessmentDomain)).Include(
               "~/Content/style/module_Cpalls.css"
               ));
            bundles.Add(new StyleBundle("~/css/cpalls/practiceoffline", GetCndPath("css/cpalls/practiceoffline", SFConfig.PracticeDomain)).Include(
               "~/Content/style/module_Cpalls.css"
               ));

            bundles.Add(new StyleBundle("~/css/public", GetCndPath("css/public")).Include(
                "~/Content/Site.css",
                "~/Content/lib/bootstrap/css/bootstrap.css",
                "~/Content/lib/bootstrap/css/bootstrap-theme.css",
                "~/Content/lib/Font-Awesome-3.2.1/css/font-awesome.min.css",
                "~/Content/style/layout_public.css"
                ));
            bundles.Add(new StyleBundle("~/css/newPublic", GetCndPath("css/newPublic")).Include(
                 "~/Content/Site.css",
            "~/Content/style/layout_public.css",
                  "~/Content/lib/Font-Awesome-3.2.1/css/font-awesome.min.css",
              "~/Content/lib/bootstrap/css/bootstrap.css"

              ));
            bundles.Add(new StyleBundle("~/css/signup", GetCndPath("css/signup")).Include(
                "~/Content/style/logup.css"));

            bundles.Add(new StyleBundle("~/css/home", GetCndPath("css/home")).Include(
                "~/Content/style/home.css"
                ));

            bundles.Add(new StyleBundle("~/css/vcw", GetCndPath("css/vcw")).Include(
               "~/Content/style/vcw.css"
               ));

            //Sam test
            bundles.Add(new ScriptBundle("~/scripts/jquery_form", GetCndPath("scripts/jquery_form")).Include(
                     "~/Content/scripts_cliProject/jquery.form.js"));

            bundles.Add(new StyleBundle("~/css/colorpicker", GetCndPath("css/colorpicker")).Include(
               "~/Content/style/jquery-ui.css",
               "~/Content/scripts/colorpicker/css/evol.colorpicker.css"
               ));

            bundles.Add(new StyleBundle("~/css/txkea", GetCndPath("css/txkea")).Include(
                           "~/Content/style/txkea.css"
                           ));

            bundles.Add(new StyleBundle("~/css/multiselect", GetCndPath("css/multiselect")).Include(
                           "~/Content/style/bootstrap-multiselect.css"
                           ));
        }
    }
}