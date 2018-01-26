//该js用于上传单个文件

var uploader;
var submit;
var selectbtn;
var isVideo;
var isFeedback;

function InitUpload() {
    if (Boolean(isVideo)) {
        uploader = SunnetWebUploader.CreateWebUploader({
            pick: "#picker",
            container: "#filelists",
            uploadbutton: "#ctlBtn",
            submitbutton: "#btnSubmit",
            multiple: false,
            fileNumLimit: 1,
            targetField: "#filetarget",
            accept:
            {
                title: "video",
                extensions: "mp4,wmv,mov,m4v",
                mimeTypes: "video/mp4,video/x-ms-wmv,video/quicktime,video/x-m4v"
            },
            md5Completed: function (file) {    //文件准备好之后，可以点击上传按钮
                if (submit != undefined && submit != null) {
                    $(submit).attr("disabled", "disabled");
                    $(submit).removeClass("submit-btn");
                    $(submit).addClass("disabled");
                    uploader.disable();
                    enable_uploadbtn();
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
        });
    }
    else {
        uploader = SunnetWebUploader.CreateWebUploader({
            pick: "#picker",
            container: "#filelists",
            uploadbutton: "#ctlBtn",
            submitbutton: "#btnSubmit",
            multiple: false,
            fileNumLimit: 1,
            targetField: "#filetarget",
            accept:
            {
                title: "doc,picture,video",
                extensions: "doc,docx,xls,xlsx,csv,rtf,rtfd,txt,tab,ppt,pptx,pdf,png,gif,jpg,jpeg,mp4,wmv,mov,m4v",
                mimeTypes: "application/msword,application/msexcel,application/rtf,text/plain,application/vnd.ms-powerpoint,application/vnd.openxmlformats-officedocument.presentationml.presentation,image/png,image/gif,image/jpeg,application/pdf,video/mp4,video/x-ms-wmv,video/quicktime,video/x-m4v"
            },
            md5Completed: function () {    //文件准备好之后，可以点击上传按钮
                if (submit != undefined && submit != null) {
                    $(submit).attr("disabled", "disabled");
                    $(submit).removeClass("submit-btn");
                    $(submit).addClass("disabled");
                    uploader.disable();
                    enable_uploadbtn();
                }
                $("i[class='icon-spinner icon-spin']").attr("class", "icon-ok").attr("style", "color:limegreen");
                $("div[class='item'] span[class='state']").text('Waiting');
            },
            filetemplate: '<div id="<% this.id %>" class="item">' +
            '<i class="icon-spinner icon-spin" style="color:Orange"></i>&nbsp;' +
            '<span class="info"><% this.name %> (<% WebUploader.formatSize(this.size) %>)</span>&nbsp;: &nbsp;' +
            '<span class="state">Preparing</span>&nbsp;&nbsp;<span class="delete" title="Remove this file"><a href="javascript:;">&times;</a></span>' +
            '</div>'
        });
    }
}

function getToFolder() {
    var d = new Date();
    return "vcw/" + d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + d.getDate();
}

$(function () {
    InitUpload();
    disable_uploadbtn();

    submit = uploader.options.submitbutton;
    selectbtn = uploader.options.pick;

    uploader.on('uploadProgress', function (file, percentage) {
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

    //添加文件时，如果为不可播放文件，则提示
    uploader.on('beforeFileQueued', function (file) {
        if (!Boolean(isFeedback)) {
            var ext = file.ext.toLowerCase();
            if (uploader.options.accept[0].extensions.indexOf(ext) > -1) {
                if (ext.toLowerCase() == "wmv") {
                    var confirmMsg = window.getErrorMessage("VCW_ConfirmUpload");
                    if (confirm(confirmMsg)) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }
            else {
                if (Boolean(isVideo)) {
                    showMessage("hint", "VCW_TeacherVIPUpload");
                }
            }
        }
    });


    //从队列中删除文件时，启用提交和选择按钮,禁用上传按钮
    uploader.on('fileDequeued', function () {
        $(submit).removeAttr("disabled");
        $(submit).removeClass("disabled");
        $(submit).addClass("submit-btn");
        uploader.enable();
        disable_uploadbtn();
    });


    //文件上传成功后，启用提交按钮
    uploader.on('uploadSuccess', function (file, response) {
        if (submit != undefined && submit != null) {
            $(submit).removeAttr("disabled");
            $(submit).removeClass("disabled");
            $(submit).addClass("submit-btn");
        }
    });

    //队列中添加文件时，禁用提交和选择按钮
    uploader.on('fileQueued', function (file) {
        if (submit != undefined && submit != null) {
            $(submit).attr("disabled", "disabled");
            $(submit).removeClass("submit-btn");
            $(submit).addClass("disabled");
            uploader.disable();

            //将文件名称改为小写
            file.name = SunnetWebUploader.getSuffix(uploader.getFiles()).toLowerCase();
        }
    });

    //当选择的文件类型不匹配时，给出提示信息
    uploader.on('error', function (type) {
        if (type == 'Q_TYPE_DENIED') {
            showMessage("fail", "VCW_NotCorretExtension");
            return false;
        }
    });

    CheckSelectFile();
})

//检查是否可以上传文件
function CheckSelectFile() {
    if ($("#uploader-list").length > 0) {
        if ($("#uploader-list").find("div[class=item]").length > 0) {
            $(selectbtn).addClass('webuploader-container-disable');//添加禁用选择按钮
            $(selectbtn).find("div:first").addClass('webuploader-pick-disable');
            $(selectbtn).find("div:first").css("z-index", "20"); //兼容ie10以下版本 ie10以下版本z-index为10
        }
        else {
            $(selectbtn).removeClass('webuploader-container-disable');
            $(selectbtn).find("div:first").removeClass('webuploader-pick-disable');
            if ($(selectbtn).find("div:eq(1)").length > 0) {
                $(selectbtn).find("div:first").css("z-index", "1");  //兼容ie10以下版本
                $(selectbtn).find("div:eq(1)").css("z-index", "10");
            }
        }
    }
}

//删除上传的文件
function RemoveFile(obj) {
    $(obj).closest('div').remove();
    CheckSelectFile();
}

function beforeSubmit_new(sender) {
    if (uploader.getFiles('queued').length > 0) {
        showMessage("hint", "Vcw_File_Inqueue")
        return false;
    }
    if (uploader.getFiles('complete').length == 0) {
        showMessage("hint", "Vcw_File_Noupload"); //Please upload a file.
        return false;
    }
    GetFiles();
    return true;
};

function beforeSubmit_edit(sender) {
    if (uploader.getFiles('queued').length > 0) {
        showMessage("hint", "Vcw_File_Inqueue")
        return false;
    }
    GetFiles();
    return true;
};


function GetFiles() {
    var uploadFiles = "";
    var completeFiles = uploader.getFiles("complete");
    if (completeFiles.length > 0) {
        uploadFiles += completeFiles[0].name + "(" + WebUploader.formatSize(completeFiles[0].size) + ")" + "|" + completeFiles[0].dbName;
        $("#uploadfiles").val(uploadFiles);
    }
}


//禁用上传按钮
function disable_uploadbtn() {
    var btn = uploader.options.uploadbutton;
    $(btn).addClass("disabled");
    $(btn).addClass('webuploader-pick-disable');
}

//启用上传按钮
function enable_uploadbtn() {
    var btn = uploader.options.uploadbutton;
    $(btn).removeClass('disabled');
    $(btn).removeClass('webuploader-pick-disable');
}
