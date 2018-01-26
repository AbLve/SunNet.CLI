//该js用于上传feedback文件

var uploader_feedback;
var submit_feedback;
var selectbtn_feedback;

function InitUpload_feedback() {

    uploader_feedback = SunnetWebUploader.CreateWebUploader({
        pick: "#picker_feedback",
        container: "#filelists_feedback",
        uploadbutton: "#ctlBtn_feedback",
        submitbutton: "#btnSubmit",
        multiple: false,
        fileNumLimit: 1,
        targetField: "#filetarget_feedback",
        accept:
        {
            title: "doc,picture,video",
            extensions: "doc,docx,xls,xlsx,csv,rtf,rtfd,txt,tab,ppt,pptx,pdf,png,gif,jpg,jpeg,mp4,wmv,mov,m4v",
            mimeTypes: "application/msword,application/msexcel,application/rtf,text/plain,application/vnd.ms-powerpoint,application/vnd.openxmlformats-officedocument.presentationml.presentation,image/png,image/gif,image/jpeg,application/pdf,video/mp4,video/x-ms-wmv,video/quicktime,video/x-m4v"
        },
        md5Completed: function (file) {    //文件准备好之后，可以点击上传按钮
            if (submit_feedback != undefined && submit_feedback != null) {
                uploader_feedback.disable();
                enable_uploadfeedbackbtn();
            }
            $("i[class='icon-spinner icon-spin']").attr("class", "icon-ok").attr("style", "color:limegreen");
            $("div[class='item'] span[class='state'][text='preparing']").each(function (index, obj) {
                $(obj).text('Waiting');
                $(obj).removeAttr("text");
            });
        },
        filetemplate: '<div id="<% this.id %>" class="item">' +
            '<i class="icon-spinner icon-spin" style="color:Orange"></i>&nbsp;' +
            '<span class="info"><% this.name %> (<% WebUploader.formatSize(this.size) %>)</span>&nbsp;: &nbsp;' +
            '<span class="state" text="preparing">Preparing</span>&nbsp;&nbsp;<span class="delete" title="Remove this file"><a href="javascript:;">&times;</a></span>' +
            '</div>'
    })
}

function getToFolder() {
    var d = new Date();
    return "vcw/" + d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + d.getDate();
}

$(function () {
    InitUpload_feedback();
    disable_uploadfeedbackbtn();

    submit_feedback = uploader_feedback.options.submitbutton;
    selectbtn_feedback = uploader_feedback.options.pick;

    uploader_feedback.on('uploadProgress', function (file, percentage) {
        var $li = $('#' + file.id),
            $percent = $li.find('.progress .progress-bar');

        // 避免重复创建
        if (!$percent.length) {
            $percent = $('<div class="progress progress-striped active">' +
              '<div class="progress-bar" role="progressbar" style="width: 0%">' +
              '</div>' +
            '</div>').appendTo($li).find('.progress-bar');
        }

        $li.find('p.state').text('uploading');

        $percent.css('width', percentage * 100 + '%');
    });

    //队列中添加文件时，禁用选择按钮
    uploader_feedback.on('fileQueued', function () {
        uploader_feedback.disable();
    });


    //从队列中删除文件时，启用选择按钮,禁用上传按钮
    uploader_feedback.on('fileDequeued', function () {
        uploader_feedback.enable();
        disable_uploadfeedbackbtn();
    });

    //当选择的文件类型不匹配时，给出提示信息
    uploader.on('error', function (type) {
        if (type == 'Q_TYPE_DENIED') {
            showMessage("fail", "VCW_NotCorretExtension");
            return false;
        }
    });

    CheckSelectFile_feedback();
})

//检查是否可以上传文件
function CheckSelectFile_feedback() {
    if ($("#uploader-list_feedback").length > 0) {
        if ($("#uploader-list_feedback").find("div[class=item]").length > 0) {
            $(selectbtn_feedback).addClass('webuploader-container-disable');//添加禁用选择按钮
            $(selectbtn_feedback).find("div:first").addClass('webuploader-pick-disable');
            $(selectbtn_feedback).find("div:first").css("z-index", "20"); //兼容ie10以下版本 ie10以下版本z-index为10
        }
        else {
            $(selectbtn_feedback).removeClass('webuploader-container-disable');
            $(selectbtn_feedback).find("div:first").removeClass('webuploader-pick-disable');
            if ($(selectbtn_feedback).find("div:eq(1)").length > 0) {
                $(selectbtn_feedback).find("div:first").css("z-index", "1");  //兼容ie10以下版本
                $(selectbtn_feedback).find("div:eq(1)").css("z-index", "10");
            }
        }
    }
}

//删除上传的文件
function RemoveFile_feedback(obj) {
    $(obj).closest('div').remove();
    CheckSelectFile_feedback();
}


function beforeSubmit_feedback(msg) {
    if (uploader_feedback.getFiles('queued').length > 0) {
        showMessage("hint", msg)
        return false;
    }
    GetFiles_feedback();
    return true;
};


function GetFiles_feedback() {
    var uploadFiles_feedback = "";
    var completeFiles_feedback = uploader_feedback.getFiles("complete");
    if (completeFiles_feedback.length > 0) {
        uploadFiles_feedback += completeFiles_feedback[0].name + "(" + WebUploader.formatSize(completeFiles_feedback[0].size) + ")" + "|" + completeFiles_feedback[0].dbName;
        $("#uploadfile_feedback").val(uploadFiles_feedback);
    }
}

//禁用上传按钮
function disable_uploadfeedbackbtn() {
    var btn = uploader_feedback.options.uploadbutton;
    $(btn).addClass("disabled");
    $(btn).addClass('webuploader-pick-disable');
}

//启用上传按钮
function enable_uploadfeedbackbtn() {
    var btn = uploader_feedback.options.uploadbutton;
    $(btn).removeClass('disabled');
    $(btn).removeClass('webuploader-pick-disable');
}
