
//该js用于上传多个文件且格式没有固定为video

var uploader;
var btn;

function InitUpload() {

    uploader = SunnetWebUploader.CreateWebUploader({
        pick: "#picker",
        container: "#filelists",
        uploadbutton: "#ctlBtn",
        submitbutton: "#btnSubmit",
        targetField: "#filetarget",
        multiple: true,
        fileNumLimit: 100,
        accept:
        {
            title: "doc,picture,video",
            extensions: "doc,docx,xls,xlsx,csv,rtf,rtfd,txt,tab,ppt,pptx,pdf,png,gif,jpg,jpeg,mp4,wmv,mov,m4v",
            mimeTypes: "application/msword,application/msexcel,application/rtf,text/plain,application/vnd.ms-powerpoint,application/vnd.openxmlformats-officedocument.presentationml.presentation,image/png,image/gif,image/jpeg,application/pdf,video/mp4,video/x-ms-wmv,video/quicktime,video/x-m4v"
        },
        md5Completed: function (file) {    //文件准备好之后，可以点击上传按钮
            if ($(btn).hasClass("disabled")) {
                enable_uploadfilebtn();
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

function getToFolder() {
    var d = new Date();
    return "vcw/" + d.getFullYear() + "-" + (d.getMonth() + 1) + "-" + d.getDate();
}

$(function () {
    InitUpload();
    btn = uploader.options.uploadbutton;
    disable_uploadfilebtn();

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

    //从队列中删除文件时，启用提交和选择按钮,禁用上传按钮
    uploader.on('fileDequeued', function () {
        if ($("#filelists")[0].children.length == 1) {
            disable_uploadfilebtn();
        }
    });

    //当选择的文件类型不匹配时，给出提示信息
    uploader.on('error', function (type) {
        if (type == 'Q_TYPE_DENIED') {
            showMessage("fail", "VCW_NotCorretExtensions");
            return false;
        }
    });
})


//删除上传的文件
function RemoveFile(obj) {
    $(obj).closest('div').remove();
}


function beforeSubmit(showMsg) {
    if ($("#filelists").length > 0) {
        if (uploader.getFiles('queued').length > 0) {
            showMessage("hint", showMsg)
            return false;
        }
        GetFiles();
        CheckUploadedFiles();
        return true;
    }
    else {
        return true;
    }
};

function GetFiles() {
    var uploadFiles = "[";
    var completeFiles = uploader.getFiles("complete");
    for (var i = 0; i < completeFiles.length; i++) {
        uploadFiles += "{" + '"FileName":"' + completeFiles[i].name + "(" + WebUploader.formatSize(completeFiles[i].size) + ")" + '",' + '"FilePath":"' + completeFiles[i].dbName + '"' + "},";
    }
    uploadFiles += "]";
    $("#uploadfiles").val(uploadFiles);
}


function CheckUploadedFiles() {
    var div_uploadedfiles = $("#div_uploadedfiles div");
    var div_length = div_uploadedfiles.length;
    var div_ids = "";
    for (var i = 0; i < div_length; i++) {
        if (i == div_length - 1) {
            div_ids += div_uploadedfiles[i].id;
        }
        else {
            div_ids += div_uploadedfiles[i].id + "|";
        }
    }
    $("#checkuploadedfiles").val(div_ids);
}


//禁用上传按钮
function disable_uploadfilebtn() {
    $(btn).addClass("disabled");
    $(btn).addClass('webuploader-pick-disable');
}

//启用上传按钮
function enable_uploadfilebtn() {
    $(btn).removeClass('disabled');
    $(btn).removeClass('webuploader-pick-disable');
}

