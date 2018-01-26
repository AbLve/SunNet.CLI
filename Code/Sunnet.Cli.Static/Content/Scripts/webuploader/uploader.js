function getUploaderPrefix() {
    return "";
}
function getToFolder() {
    return "cli";
}
WebUploader.Uploader.register({
    'before-send-file': 'beforeSendFile',
    'before-send': 'beforeSend'
}, {
    beforeSendFile: function (file) {
        var uploader = this;
        //console.log("before-send-file");
        var serverurl = this.options.server;
        var deferred = WebUploader.Deferred();
        file.prefix = getUploaderPrefix();
        var data = $.extend({}, {
            id: file.id,
            name: file.name,
            type: file.type,
            lastModifiedDate: file.lastModifiedDate,
            size: file.size,
            chunk: 0,
            chunks: 0,
            md5: file.md5,
            prefix: getUploaderPrefix(),
            tofolder: getToFolder()
        });
        jQuery.ajax({
            url: serverurl,
            type: "post",
            data: data,
            success: function (response, textStatus, xhr) {
                var result = jQuery.parseJSON(response || {});
                if (xhr.statusText == "Existed") {
                     file.serverName = result.file;
                    uploader.owner.skipFile(file);
                }
                deferred.resolve();
            },
            error: function () {
                deferred.resolve();
            }
        });
        return deferred.promise();
    },
    beforeSend: function (block) {
        //console.log("before-send-chunk");
        var serverurl = this.options.server;
        var deferred = WebUploader.Deferred();
        var file = block.file;
        file.prefix = getUploaderPrefix();
        var data = $.extend({}, {
            id: file.id,
            name: file.name,
            type: file.type,
            lastModifiedDate: file.lastModifiedDate,
            size: file.size,
            chunk: block.chunk,
            chunks: block.chunks,
            md5: file.md5,
            prefix: getUploaderPrefix(),
            tofolder: getToFolder()
        });
        jQuery.ajax({
            url: serverurl,
            type: "post",
            data: data,
            success: function (response, textStatus, xhr) {
                if (xhr.statusText == "Existed") {
                    var result = jQuery.parseJSON(response || {});
                    if (result.success) {
                        file.serverName = result.file;
                    }
                    deferred.reject();
                }
                else
                    deferred.resolve();
            },
            error: function () {
                deferred.resolve();
            }
        });
        return deferred.promise();
    }
});
var SunnetWebUploader = {
    defaultOptions: {
        chunked: true,
        threads: 2,
        chunkSize: 2097152, //2M
        resize: null,
        compress: false,
        duplicate: 1,
        multiple: false,
        prepareNextFile: true,
        fileNumLimit: 1,
        showProgress: false,
        // swf文件路径
        swf: 'Content/Scripts/webuploader/Uploader.swf',
        // 文件接收服务端 
        server: 'Uploader/FileUploader.ashx',
        // 选择文件的按钮 可选 
        // 内部根据当前运行是创建，可能是input元素，也可能是flash.
        pick: '#picker'
    },
    registDomain: function (domain) {
        this.defaultOptions.swf = domain + this.defaultOptions.swf;
        this.defaultOptions.server = domain + this.defaultOptions.server;
    },

    getOptions: function (options) {
        return jQuery.extend({}, this.defaultOptions, options);
    },
    getSuffix: function (files) {
        if (!files || files.length == 0)
            return "";
        var lastIndex = files.length - 1;
        var lastFile = files[lastIndex];
        var lastFilename = lastFile.name;
        var pre, suf, indexNow;

        // /(.*)\((\d)\)(\.\w*)$/.exec("ab.c(1).jpg") = ["ab.c(1).jpg", "ab.c", "1", ".jpg"]
        var matchItems = /(.*)\((\d)\)(\.\w*)$/.exec(lastFilename);
        if (matchItems && matchItems.length) {
            pre = matchItems[1];
            suf = matchItems[3];
            indexNow = +matchItems[2];
        }
        else {
            // /(.*)(\.\w*)$/.exec("ab.c.jpg") = ["ab.c.jpg", "ab.c", ".jpg"]
            matchItems = /(.*)(\.\w*)$/.exec(lastFilename);
            if (matchItems && matchItems.length) {
                pre = matchItems[1];
                suf = matchItems[2];
                indexNow = 0;
            }
        }
        jQuery.each(files, function (findex, file) {
            if (findex < lastIndex) {
                var matchItems2 = /(.*)\((\d)\)(\.\w*)$/.exec(file.name);
                if (matchItems2 && matchItems2.length) {
                    if (matchItems2[1] == pre && matchItems2[3] == suf) {
                        indexNow = Math.max(indexNow, +matchItems2[2]);
                        indexNow++;
                    }
                } else {
                    matchItems2 = /(.*)(\.\w*)$/.exec(file.name);
                    if (matchItems2 && matchItems2.length) {
                        if (matchItems2[1] == pre && matchItems2[2] == suf) {
                            indexNow = 1;
                        }
                    }
                }
            }
        });
        return pre + (indexNow == 0 ? "" : "(" + indexNow + ")") + suf;
    },

    /*
     * 页面需要重写getUploaderPrefix函数来返回每个页面自己的上传前缀
     * 参数说明：内部已经处理了一些统一的参数，每个页面可定制参数
     * options参数至少需要：
     * pick:选择文件触发器
     * container:文件夹列表容器
     * targetField:上传成功之后赋值给哪个控件(仅使用于单文件上传，多文件请自己重写uploadSuccess,uploadComplete事件)
     * 可选参数：
     * speed:速度提示
     * uploadbutton:触发上传的按钮
     * submitbutton:表单提交按钮（data:clicked 为true表示，已经点击提交）
     * filetemplate:文件列表模版
     * showProgress:是否显示每个文件上传进度
     * autoUpload: 是否自动上传， 原生态的 auto 参数不建议使用
     */
    CreateWebUploader: function (options) {
        var $list, $uploadbutton, $submitbutton, template, $speed, $picker, $form, showProgress, $targetField, autoUpload,
        prefix = getUploaderPrefix(), tofolder = getToFolder();

        if (options.pick) {
            $picker = $(options.pick);
            $form = $picker.closest("form");
        }
        if (options.container) $list = $(options.container);

        var uploadButton = options["uploadbutton"] || options["uploadButton"];
        if (uploadButton) $uploadbutton = $form.find(uploadButton);

        var submitButton = options["submitbutton"] || options["submitButton"];
        if (submitButton) $submitbutton = $form.find(submitButton);

        if (options.speed) $speed = $form.find(options.speed);

        var targetField = options["targetField"] || options["targetfield"];
        if (targetField) $targetField = $form.find(targetField);

        var autoUpload = options["autoUpload"] || options["autoupload"];


        template = options.filetemplate || '<div id="<% this.id %>" class="item">' +
            '<span class="info"><% this.name %> (<% WebUploader.formatSize(this.size) %>)</span>&nbsp;: &nbsp;' +
            '<span class="state">Waiting</span>&nbsp;&nbsp;<span class="delete" title="Remove this file"><a href="javascript:;">&times;</a></span>' +
            '</div>';
        showProgress = options.showProgress || false;
        options = this.getOptions(options);
        var uploader = WebUploader.create(options);
        // 删除文件
        $list && $list.on("click", ".delete", function () {
            var $this = $(this);
            var $file = $this.closest("div.item");
            var fileID = $file.attr("id");
            //console.log(fileID);
            uploader.removeFile(fileID);
            $file.remove();
            $targetField.val('');
        });
        // 分块上传之前
        uploader.on('uploadBeforeSend', function (block, params) {
            //console.log("uploadBeforeSend");
            block.file.prefix = params.prefix = prefix;
            block.file.tofolder = params.tofolder = tofolder;
            params.md5 = block.file.md5;
        });
        // 当有文件添加进来的时候
        uploader.on('fileQueued', function (file) {
            //console.log("fileQueued");
            if (!uploader.md5Queue) {
                uploader.md5Queue = 0;
            }
            uploader.md5Queue++;
            $submitbutton.prop("disabled", true);
            $uploadbutton && $uploadbutton.prop("disabled", true);

            //console.log(file.source.ruid, file.source.uid);
            file.prefix = getUploaderPrefix();
            file.name = SunnetWebUploader.getSuffix(uploader.getFiles());
            $list.append(TemplateEngine(template, file));

            file.md5 = WebUploader.guid("s");
            file.setStatus("queued");
            uploader.md5File(file).then(function (val) {
                file.md5 = val;
                uploader.md5Queue--;
                console.log(file.name, "md5 done");
                if (uploader.md5Queue === 0) {
                    $submitbutton.prop("disabled", false);
                    $uploadbutton && $uploadbutton.prop("disabled", false);

                    if ($.isFunction(options.md5Completed)) { //文件越大，md5准备的时间越长
                        options.md5Completed();
                    } else { //自动上传
                        if (autoUpload) {
                            setTimeout(function () {
                                if (uploader.state === 'uploading') {
                                    uploader.stop();
                                } else {
                                    uploader.upload();
                                    timeStart = new Date();
                                }
                            }, 100);
                        }
                    }
                }
            });           
        });

        var totalTime = 0, totalBytes = 0, timeStart;
        // 文件上传过程中创建进度条实时显示。
        uploader.on('uploadProgress', function (file, percentage) {
            var $li = $form.find('#' + file.id),
                $percent = $li.find('.progress .progress-bar');
            totalTime += (new Date() - timeStart) / 1000;
            if (totalTime < 1) totalTime = 1;
            totalBytes += file.size * percentage / (1000);
            if ($speed) {
                $speed.html("Average speed：" + (totalBytes / totalTime).toFixed(2) + "KB/S   Uploaded:" + (percentage * 100).toFixed(2) + "%").show();
            }
            if (showProgress) {
                // 避免重复创建
                if (!$percent.length) {
                    $percent = $('<div class="progress progress-striped active">' +
                        '<div class="progress-bar" role="progressbar" style="width: 0%;">' +
                        '</div>' +
                        '</div>').appendTo($li).find('.progress-bar');
                }
                $li.find('.state').text('Uploading');
                $percent.css('width', percentage * 100 + '%');
                if (percentage >= 1) {
                    $('#' + file.id).find('.state').text('Being processed...');
                }
            }
        });
        uploader.on('uploadSuccess', function (file, result) {
            debugger;
           //console.log("uploadSuccess",arguments);
            var newFileName = "";
            if (result) {
                if (result.success) {
                    $form.find('#' + file.id).find('.state').text('Uploaded');
                    //$targetField && $targetField.val(tofolder + "/" + prefix + result.file).change();
                    newFileName = tofolder + "/" + prefix + result.file;
                    file.serverName = result.file;

                } else {
                    $form.find('#' + file.id).find('.state').text(result.msg);
                }
            }
            else if (file.serverName) {
                $form.find('#' + file.id).find('.state').text('Uploaded');
                //$targetField && $targetField.val(tofolder + "/" + prefix + file.serverName).change();
                newFileName = tofolder + "/" + prefix + file.serverName;
            }
            if (newFileName) {
                file.dbName = newFileName;

                var existedFiles = $targetField.val();
                if (!$targetField.data("files")) {
                    existedFiles = "";
                }
                else {
                    existedFiles = existedFiles + "|";
                    $targetField.data("files", true);
                }
                existedFiles = existedFiles + newFileName;
                $targetField.val(existedFiles).change();
            }

            var uploaded = $submitbutton.data("uploaded") || 0;
            uploaded++;
            $submitbutton.data("uploaded", uploaded);
        });
        uploader.on('uploadError', function (file) {
            debugger;
            //console.log("uploadError", arguments);
            var $state = $('#' + file.id).find('.state');
            $state.text($state.text() + ', upload failed.');
            if ($submitbutton.data("clicked")) {
                $submitbutton.click();
            }
        });
        uploader.on('error', function (type) {
            debugger;
            switch (type) {
                case 'Q_TYPE_DENIED':
                    showMessage("fail", "VCW_NotCorretExtensions");
                    return false;
                case 'F_EXCEED_SIZE':
                    showMessage("fail", "Maximum files size: 2 MB");
                    return false;
            }
            return false;
        });
        uploader.on('uploadComplete', function (file) {
            $form.find('#' + file.id).find('.progress').fadeOut();
            if ($submitbutton.data("clicked")) {
                $submitbutton.click();
            }
        });
        if ($uploadbutton) {
            $uploadbutton.on('click', function () {
                if (uploader.state === 'uploading') {
                    uploader.stop();
                } else {
                    uploader.upload();
                    timeStart = new Date();
                }
                return false;
            });
        }
        setTimeout(function() {
            $("[style='width: 100%; height: 100%; display: block; cursor: pointer; background: transparent; color: transparent;']").html('select file');
        },200);
        return uploader;
    }
}

SunnetWebUploader.registDomain(window._staticDomain_);