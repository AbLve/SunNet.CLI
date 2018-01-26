function SelectItemModel(text, value) {
    this.text = text;
    this.value = value;
}

var statusHelper = {
    Active: {
        value: 1,
        text: "Active"
    },
    Inactive: { value: 2, text: "Inactive" },
    theOther: function (value) {
        if (value == 1) {
            return jsHelper.status.Inactive;
        }
        return jsHelper.status.Active;
    }

}
window._message_ = {
    // 操作成功提示消息的标题 Updated by us
    success: {
        title: "Great",
        message: "Your updates have been saved.",
        className: "success",
        iconClass: "icon-ok"
    },
    successOnlyMsg: {
        title: "Great",
        message: "",
        className: "success",
        iconClass: "icon-ok"
    },
    // 操作失败时提示消息的标题 Updated by us
    fail: {
        title: "Sorry",
        message: "Operation has failed.",
        className: "danger",
        iconClass: "icon-remove"
    },
    // 警告提示消息的标题 Updated by us
    warning: {
        title: "Warning",
        message: "Warning: ",
        className: "warning",
        iconClass: "icon-warning-sign"
    },

    // 客户端程序执行时,显示消息的标题,例如JS代码验证逻辑不通过
    // 用户在填写表单时,一些客户端的逻辑验证提示标题(尚未开始提交表单)
    hint: {
        title: "Sorry",
        message: "Please fix issues below.",
        className: "warning",
        iconClass: "icon-warning-sign"
    },

    // 搜索域黑名单 正则表达式 (除：# & ' - _ + 以外的ASCII特殊字符）
    // 搜索时会移除这些字符的搜索
    search_Blacklist: /(?:!|"|\$|%|\(|\)|\*|,|\/|:|;|<|=|>|\?|@|\[|\\|]|\^|`|\{|\||\}|~|(&#))/gi,

    // 调试时使用, 如果没有在该文件中定义KEY, 而调用相应KEY,则提示此内容,提醒开发人员在该文件中加入相应KEY
    message_notfound: "Message not found, please add message to JsMsgs.js file. If you are a terminal user, please contact administrator.",

    403: "you have no permission to access the content you requested.",

    //Ipad 上执行时，如果不能自动关闭窗口，提醒用户关闭
    auto_close: "Please close this window.",
    // 从下面开始是自定义的Key, 开发人员请不要修改上面的内容

    // Assessment | Measure 的Cutoff Score 年龄段输入错误
    Assessment_CutoffScore_order: "Invalid age interval. ",

    // Assessment | Measure 的Cutoff Score 的 Total Score Range输入错误
    Assessment_Score_Range: "Invalid total score range.",

    // 在这里添加更多

    // 表单元素的值改变之后, 要关闭弹出窗口时提示的消息 Updated by us
    confirmToLeave_formChanged: "The data has been changed. Do you want to leave this page without saving?",
    // 表单元素的值改变之后, 发生页面导航时提示的消息 Updated by us
    confirmToRedirect_formChanged: "The data has been changed. Do you want to leave this page without saving?",

    // 删除数据时提示的消息 Updated by us
    confirmToDelete: "Do you want to delete [{0}]?",

    // PA类型的Items超出了最大项数(10)
    over_Max_Pa_ItemCount: "You can have 10 items at most. ",

    // 添加PA类型的Item时候，Text文本必须填写当前项才能增加下一项
    CEC_PA_Fill_Item: "Please add items in order. ",

    // Multiple Choices 必须填写一项(答案)，上传一个图片
    Mul_Choices_Required: "At least one picture. ",

    // 设置某一项为正确答案,则该项必须包含图片
    CorrectAnswer_Need_Picture: "The image is required for the correct answer.",

    // 某些Items最多有一个正确答案
    AtMost_One_CorrectAnswer: "Please have only one correct answer. ",

    // Multiple Choices 必须完整填写(Picture , Time, Audio, Time)
    Mul_Choices_Complete: "Please complete the item.",

    // 某些Items必须有正确答案 Updated by us
    Item_Need_Correct_Answer: "Please specify the correct answer(s).",

    // copy from existing item, 必须选择一项内容
    Please_Select_an_Item: "Please select an item.",

    //Txkea Receptive item设置为Multi Select时提示
    Multiple_CorrectAnswer: "Please have multiple correct answers. ",

    //Txkea Receptive item设置Overall Timeout为No时提示
    OverallTimeouttoNo: "Only the Break Condition will trigger for the System to move to the Next item",

    //Txkea 选择背景图提示
    UpdateCanvasBackImg: "Uploading a background image will clear out your selection for background color fill. Would you still like to proceed?",

    //Txkea 选择背景色提示
    UpdateCanvasBackColor: "Selecting a new background image will clear out the background image you had previously selected. Would you still like to proceed?",

    //Txkea 将背景图片替换为背景色时提示
    ChangeCanvasBgImgtoBgColor: "Selecting a background color fill will clear out the background image you have selected. Would you still like to proceed?",

    //Txkea Layout改变为自定义提示
    ChangetoCustom: "Are you sure you want to change to custom layout?",

    //Txkea 删除response提示
    DeleteResponse: "Are you sure you want to delete this response?",

    //Txkea Receptive item 所有selectable的Answer需要放在一起
    SelectableGroup: "All selectable answers should be grouped together in blocks",

    //Txkea Answer的图片和Audio必须要填写一项
    TxkeaAtleastOneItem: "Please upload at least one image or audio for Image[i]",

    //Txkea item的InstructionText和InstructionAudio必须要填写一项
    TxkeaCheckforInstruction: "Please upload Instruction Audio or fill Instruction Text for Instruction option",

    //Txkea Receptive 验证score值提示
    TxkeaCorrectScore: "Please input numeric score values",

    //Txkea Item Logic to 值比 from 值小时提示
    TxkeaSkipIncorrect: "The “To” value must equal or greater than “From” value",

    //Txkea Item Logic重复时提示
    TxkeaSkipOverlap: "The values of “To” and “From” cannot overlap with others",

    //Ade Change Item Order时提示
    AdeChangeOrder: "Changing index could potentially interfere with previously linked items and potentially cause infinite loop",

    // 新建school 的时候发现同名情况，需要确认是否继续
    SchoolNameExistsAssign: "Another school with the same name already exists. Do you want to assign community?",
    SchoolNameExistsRequest: "Another school with the same name already exists. Do you want to send request to the Principal?",
    SchoolNameExistsAlert: "Another school with the same name already exists.",
    SchoolRequestHasSent: "School request has been sent.",
    SchoolRelationshipExistsAlert: "The relationship between this school and the selected community exists.",
    SchoolRelationshipPendingAlert: "The relationship between this school and the selected community is pending.",

    // 在创建classroom 的时候要考虑是否重名
    ClassroomExists: "Another classroom with the same name already exists. Do you want to continue?",

    //添加SchoolSpecialist时，UserName和Email重复时提示
    SchoolSpecialistExists: "Another user with the same name and email already exists!",

    // 在创建Toolbox 的时候要考虑是否重名
    IsSameName: "Another [{0}] with the same name already exists.",

    // 请选Measure ,CPALLS+ 页面
    Selected_Measure: "Please select the measures you wish to administer by clicking their heading.<Br>i.e. Click on \“Listening\” \“Rhyming I\” \“Rhyming II\” etc. ",
    Selected_Measure_All_Done: "All the measures selected have been finished/excluded for <b>{FirstName} {LastName}</b>",
    // 执行CPALLS+时, 中途离开或者关闭窗口时的提醒
    CPALLS_Leave_Confirm: "Do you want to leave this page without saving the assessment data?",

    // 执行CPALLS+时, 点击Back时的提醒 updated by us
    CPALLS_Go_Back: "You are about to leave the assessment. The data will be saved, and when you return, you will resume where you left off.",

    // 执行CPALLS+时, 点击Quit and discard 时的提示updated by us
    CPALLS_quit: "You are about to leave the assessment. The data will be discard, and when you return, you will start over.",

    // 执行CPALLS+结束, 数据已保存,窗口即将关闭updated by us
    CPALLS_Over: "Assessment is over, all the data needed has been saved, the window is closing.",

    // Invalidate Measure 时提示的确认消息(数据已保存)
    CPALLS_Invalidate_Measure: "The assessment result will be deleted and cannot be restored once its been invalidated. Do you want to continue?",

    // Invalidate Measure 时提示的确认消息(执行是,数据尚未保存)
    CPALLS_Invalidate_Unsaved: "Are you sure to invalidate this assessment?",

    // {object} may be Item or Measure
    CPALLS_Preview_Timeout: "The {object} has been timed out if you are doing the assessment. Would you like to close the window?",

    // Unlock Assessment时,--需要清空所有数据--暂不清空
    unlock_Assessment: "You can edit the assessment after it is unlocked. But you need to make sure all changes are safe to the historical data. Continue? ",

    Cpalls_Report_need_CommunityId: "Please select a Community/District.",

    Cpalls_Report_need_SchoolId: "Please select a School.",

    Cpalls_Report_need_SchoolYear: "Please select School Year.",

    Cpalls_Report_need_ClassId: "Demo class cannot export completion report.",

    //VCW Coach Assignment文件在队列中且并未上传
    Vcw_Files_Inqueue: "There are several files not uploaded in the queue of Coach Assignment File.Click Start Upload button to upload the files or the x next to Waiting to remove the files.",

    //VCW Coach Assignment文件未上传
    Vcw_Files_Noupload: "Please upload one Coach Assignment File at least!",

    //VCW Watch文件在队列中且并未上传
    Vcw_WatchFiles_Inqueue: "There are several files not uploaded in the queue of Watch File.Click Start Upload button to upload the files or the x next to Waiting to remove the files.",

    //VCW Watch文件未上传
    Vcw_WatchFiles_Noupload: "Please upload one Watch File at least!",

    //VCW PM文件在队列中且并未上传
    Vcw_PMFiles_Inqueue: "There are several files not uploaded in the queue of PM Assignment File.Click Start Upload button to upload the files or the x next to Waiting to remove the files.",

    //VCW PM文件未上传
    Vcw_PMFiles_Noupload: "Please upload one PM Assignment File at least!",

    //VCW File未上传
    Vcw_File_Noupload: "Please upload one File at least!",

    //VCW File文件在队列中并且未上传
    Vcw_File_Inqueue: "There are several files not uploaded in the queue of File.Click Start Upload button to upload the files or the x next to Waiting to remove the files.",

    //VCW 未选择assignment时点击删除
    Vcw_Assignments_Noselect: "Please select one assignment at least!",

    //VCW 未选择video时点击删除
    Vcw_Videos_Noselect: "Please select one file at least!",

    //Coach-Teacher-General上传文件时未选择Teacher
    Vcw_Teachers_Noselect: "Please select one teacher at least!",

    //PM-Coach-General上传文件时未选择Coach
    Vcw_Coaches_Noselect: "Please select one coach at least!",

    //VCW feedback文件未上传
    Vcw_Feedback_Noupload: "Please upload one Feedback File at least!",

    //VCW feedback文件在队列中并且未上传
    Vcw_Feedback_Inqueue: "There are several files not uploaded in the queue of Feedback.Click Start Upload button to upload the files or the x next to Waiting to remove the files.",

    //VCW VIPdocument文件在队列中并且未上传
    Vcw_VIPDocument_Inqueue: "There are several files not uploaded in the queue of VIP Documents.Click Start Upload button to upload the files or the x next to Waiting to remove the files.",

    //导出报表时未选择Custom Score
    Export_Select_CustomScore: "Please select custom score(s) first.",

    //导出报表时未选择Measure
    Export_Select_Measure: "Please select measure(s) first.",

    //导出报表时未选择Wave(Cpalls Growth 报表)
    Export_Select_Wave: "Please select wave(s) first.",

    //导出Growth 报表，需要指定类型：Average Scores / % Meeting Benchmarks
    Growth_Report_Type: "Please specify the report type.",

    Noselect: "Please select one item at least!",

    //VCW导出Excel时确认提示
    VCW_ExportToExcelConfirm: "Are you sure to export to Excel?",

    //VCW导出Pdf时确认提示
    VCW_ExportToPdfConfirm: "Are you sure to export to PDF?",

    //VCW添加不可播放的文件时，提示是否继续上传
    VCW_ConfirmUpload: "This type of file cannot play in the video player. Do you want to continue the upload?",

    //VCW TeacherVIPAssignment上传文件非Video文件时，给出提示
    VCW_TeacherVIPUpload: "Please upload a video file for this assignment.",

    VCW_NotCorretExtension: "Please select a file with correct extension.",

    VCW_NotCorretExtensions: "Please select file(s) with correct extension.",

    One_Wave: "You can export one wave at most, please check the measures you selected.",

    // Offline : override local data
    Override_Local_Data: "Local data has been changed, continue the operation will override the data saved, confirm?",

    Complete_Current_Student: "Please complete the current student's assessment first.",

    Internet_not_available: "You are not connected to the Internet.",
    Sync_error: "Something is wrong, please try again later, if this error occurred serval times, please contact the administrator.",
    Sync_done: "Student[{Firstname} {Lastname}] data has been synchronized.",
    Sync_need_Login: "Please log in to CLI Engage before syncing data.",
    need_Sync_Before_goOnline: "You have offline results that need syncing. Please sync data before leaving this page to save results.",
    Offline_Expired: "Offline data expired, but it's still saved in local browser, if you want to sync the data, please contact the administrator",
    //Cec AssessmentDate
    EnterAssessmentDate: 'Please enter Assessment Date.',
    LeftMoreItems: '{0} Items Left. All Items must be checked.',
    LeftOneItem: '{0} Item Left. All Items must be checked.',

    Cot_At_Least_One: "Please select at least one goal item.",
    Cot_Finalize_Confirm: "After the COT is finalized, no change can be made. Do you want to finalize this COT?",
    Cot_Generate_Stg_Report_Confirm: "Generating this PDF report <b>will not Save</b> changes to the Short Term Goals page. Click <b>Cancel</b> to return to the Short Term Goals page and Save changes.",
    Cot_New_Orders_Saved: "New orders of the items have been saved.",
    pin_Complexity: "The pin code must consist of 6-10 characters, at least one number and one UPPER case letter.",
    pin_Required: "Please input a PIN Code to protect your offline data.",
    pin_Error: "Incorrect pin entered. Please enter correct pin or go back to the Online Assessment page and click the Reset Pin button.",

    reset_STG_Confirm: "Are you sure that you want to delete the last Short Term Goal Report? <br/>Once deleted, the observation cannot be recovered.",
    reset_BOY_COT_Confirm: "Are you sure that you want to unfinalize the BOY COT?",
    reset_MOY_COT_Confirm: "Are you sure that you want to unfinalize the MOY COT?",
    Cot_Sync_done: "Teacher [{Firstname} {Lastname}] data has been synchronized.",
    Cot_Sort_Top: "The category is already at the top of the list.",
    Cot_Report_Need_Measure: "Please select at least one Strategy",
    Cot_Report_Need_Teacher: "Please select at least one Teacher",

    Trs_Sync_done: "School [{ID}] data has been synchronized.",

    add_Favorite_Fail: "The operation is rejected by browser, please add to favorite manually.", //updated by david,
    trs_All_items_required: "All items required.",
    trs_Invalidate_Assessment_Confirm: "Are you sure?",
    trs_Invalidate_Assessment_Confirm_Delete: "Invalidate means<b> delete</b> this assessment<b> permanently</b>, because it's generated from school page, Are you sure?",
    trs_Preview_Tips: "You need to click save button first if you want to preview latest result.",
    trs_Delete_Confirm: "Are your sure you want to delete the assessment, you can not restore the assessment.",
    trs_ObservationLength: "Please type correct value for Length of Observation",
    trs_ClassSameAssessorAndMentor: "The TRS Class Assessor cannot be the same TRS Class Mentor, please select a different user",
    trs_NoClass: "There is no class in the school, can not start assessment.",
    trs_ClassWithoutCiild: "The age of children along with the # of children must be entered on the class management page for class(es): {0}.",
    trs_AllNAChecked: "N/A for the entire assessment is not allowed. This assessment {0}.",
    trs_SelectAccreditations: "Please select the accreditation",

    //data process
    dataprocess_Delete_Confirm: "Are your sure you want to delete the data?",

    warning_change_facility_type: "Edits to the <strong>Facility Type</strong> will reset the following information for all classes under this school: <strong>Type of Class</strong>.",
    multiple_Fields_Required: "Fill the required fields and make sure all fields are valid.", // eg: Roster page

    confirm_To_Offline: "Are you sure you want to start offline progress?",

    confirm_To_Offline_Changed: "Offline data has been changed without synchronized, please synchronize the changed data, then you can cache new data.",

    //结束时间必须大于开始时间
    stopGreaterStart: "Stop date must be greater than start date",

    // Typed Response Item: At most 9 Responses, {Items}会被替换成为真实的限制数量
    ade_Typed_Response_Most_9_Responses: "You can have {Items} responses at most.",

    //Export多表数据导出，至少选择一个字段
    Export_NoSelect: "Please select one field at least!",
    export_SaveTemp: "Do you want to save you choices as a Template",
    reportTempNameExists: "Another Report Template with the same name already exists!",

    // Export PDF faild, in (Measure result page)
    exportPdf_Failed: "Export pdf failed, please try again later, if this error ocurred serval times, please contact the administrator.",

    studentExists: "Another student with the same firstname,lastname and birthdate in this community already exists.",

    endDateCompareStartDate: "The start date cannot be later than the end date",

    //同一学校下存在同名的Class
    ClassExists: "A class with this name already exists at this school. Do you want to continue?",


    //Report保存模板
    report_SaveTemp: "Your report request has been recorded, you will get an email containing a link to the report later. Do you want to save you choices as a Template?",
    //IP地址验证
    CheckIP: "Please fill in the correct IP address.",

    //Ade 查看Layout之前需先保存
    ade_Layout_Tips: "You need to click save button first if you want to design for latest layout.",

    //Ade 所有answer都需要picture
    ade_AnswerRequired: "Please upload picture for each answer.",

    ade_IncorrectValue: "Please input correct {item} values.",

    //CheckSFTP
    SFTPTestFail: "Connect SFTP failed.Please check your SFTP information.",
    SFTPTestSuccess: "Test connect SFTP success.",
    SFTPWithoutTest: "The SFTP configuration information without verification or validate fails, please try again.",
    //David 06/19/2017
    ProcessingReport: "Your report will be ready soon."

};
