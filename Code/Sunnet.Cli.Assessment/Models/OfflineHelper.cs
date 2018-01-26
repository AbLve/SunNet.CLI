using System;
using System.Linq;

#region Using

using System.Collections.Generic;

#endregion

/**************************************************************************
 * Developer: 		JackZhang
 * Computer:		JACKZ
 * Domain:			JackZ
 * CreatedOn:		6/15/2015 11:39:22
 * Description:		Please input class summary
 * Version History:	Created,6/15/2015 11:39:22
 *
 *
 **************************************************************************/

namespace Sunnet.Cli.Assessment.Models
{
    public sealed class OfflineHelper
    {
        /// <summary>
        /// 离线程序全局资源
        /// </summary>
        /// Author : JackZhang
        /// Date   : 6/15/2015 11:47:26
        public static List<string> GlobalResources
        {
            get
            {
                return new List<string>
                {
                    "/images/cli_logo.png",
                    "/images/UT.png",
                    "/images/CLI.png",
                    "/images/TSR.png",
                    "/content/images/help_pic.png",
                    "/Content/images/icon_play_more.png",
                    "/Content/images/top_bg.png",
                    "/images/ReDesign/cliheadericon.png",
                    "/images/ReDesign/Banner/CLIe-001_HOME-banner.png",
                    "/images/ReDesign/icon-tele.png",
                    "/images/ReDesign/icon-phone.png",
                    "/images/ReDesign/UTH_CLI_co_brand_wht.png",
                    "/images/ReDesign/facebookrnd.png",
                    "/images/ReDesign/twitterrnd.png",
                    "/images/ReDesign/youtubernd.png",
                    "/images/ReDesign/CLI-bg.png",
                    "/images/ReDesign/Banner/CLIe-HOME-banner_tinystrip.png",
                    "/favicon.ico",
                    "/Content/fonts/fontawesome-webfont.woff?v=3.2.1",
                    "/Content/fonts/fontawesome-webfont.eot?v=3.2.1",
                    "/Content/fonts/fontawesome-webfont.eot?#iefix&v=3.2.1",
                    "/Content/fonts/fontawesome-webfont.svg?v=3.2.1",
                    "/Content/fonts/fontawesome-webfont.ttf?v=3.2.1",
                    "/Content/fonts/glyphicons-halflings-regular.woff",
                    "/Content/fonts/glyphicons-halflings-regular.eot",
                    "/Content/fonts/glyphicons-halflings-regular.svg",
                    "/Content/fonts/glyphicons-halflings-regular.ttf",
                    "/Images/time.png"
                };
            }
        }

        /// <summary>
        /// 离线程序HTML编辑框资源
        /// </summary>
        /// Author : JackZhang
        /// Date   : 6/15/2015 11:47:44
        public static List<string> CKEditorResources
        {
            get
            {
                return new List<string>
                {
                    "/Content/lib/ckeditor/plugins/clipboard/dialogs/paste.js?t=E7KD",
                    "/Content/lib/ckeditor/config.js",
                    "/Content/lib/ckeditor/config.js?t=E7KD",
                    "/Content/lib/ckeditor/lang/en.js?t=E7KD",
                    "/Content/lib/ckeditor/skins/moono/icons.png",
                    "/Content/lib/ckeditor/skins/moono/icons_hidpi.png",
                    "/Content/lib/ckeditor/skins/moono/images/arrow.png",
                    "/Content/lib/ckeditor/skins/moono/images/close.png",
                    "/Content/lib/ckeditor/skins/moono/images/lock-open.png",
                    "/Content/lib/ckeditor/skins/moono/images/lock.png",
                    "/Content/lib/ckeditor/skins/moono/images/refresh.png",
                    "/Content/lib/ckeditor/skins/moono/images/arrow.png",
                    "/Content/lib/ckeditor/skins/moono/images/hidpi/close.png",
                    "/Content/lib/ckeditor/skins/moono/images/hidpi/lock-open.png",
                    "/Content/lib/ckeditor/skins/moono/images/hidpi/lock.png",
                    "/Content/lib/ckeditor/skins/moono/images/hidpi/refresh.png",
                    "/Content/lib/ckeditor/skins/moono/dialog.css?t=E7KD",
                    "/Content/lib/ckeditor/skins/moono/dialog_ie.css?t=E7KD",
                    "/Content/lib/ckeditor/skins/moono/dialog_ie7.css?t=E7KD",
                    "/Content/lib/ckeditor/skins/moono/dialog_ie8.css?t=E7KD",
                    "/Content/lib/ckeditor/skins/moono/dialog_iequirks.css?t=E7KD",
                    "/Content/lib/ckeditor/skins/moono/editor.css?t=E7KD",
                    "/Content/lib/ckeditor/skins/moono/editor_gecko.css?t=E7KD",
                    "/Content/lib/ckeditor/skins/moono/editor_ie.css?t=E7KD",
                    "/Content/lib/ckeditor/skins/moono/editor_ie7.css?t=E7KD",
                    "/Content/lib/ckeditor/skins/moono/editor_ie8.css?t=E7KD",
                    "/Content/lib/ckeditor/skins/moono/editor_iequirks.css?t=E7KD",
                    "/Content/lib/ckeditor/plugins/colordialog/dialogs/colordialog.js?t=E7KD",
                    "/Content/lib/ckeditor/contents.css?t=E7KD"
                };
            }
        }

        /// <summary>
        /// 离线程序日期框资源
        /// </summary>
        /// Author : JackZhang
        /// Date   : 6/15/2015 11:47:57
        public static List<string> DatetimeBoxResources
        {
            get
            {
                return new List<string>
                {
                    "/Content/Scripts/datetime/lang/en.js",
                    "/Content/Scripts/datetime/skin/WdatePicker.css",
                    "/Content/Scripts/datetime/skin/twoer/datepicker.css",
                    "/Content/Scripts/datetime/calendar.js",
                    "/Content/Scripts/datetime/calendar.js?",
                    "/Content/Scripts/datetime/skin/twoer/img.gif"
                };
            }
        }

        public static List<string> SplitResources(string resources)
        {
            var res =
                resources.Replace("<link href=\"", "")
                    .Replace("\" rel=\"stylesheet\"/>", "")
                    .Replace("<script src=\"", "")
                    .Replace("\"></script>", "");
            return res.Split("\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
        }
    }
}