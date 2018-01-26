
jQuery(function () {
    //  背景图片颜色设置 begin
    var uploaderBackgroundImage = SunnetWebUploader.CreateWebUploader({
        pick: "#btnBackgroundImage",
        container: "#BackgroundImageList",
        submitbutton: "#btnSubmit",
        targetField: "#BackgroundImage",
        fileSingleSizeLimit: 2097152, //2M
        accept: {
            extensions: "jpg,jpeg,gif,png,bmp",
            mimeTypes: "image/jpg,image/jpeg,image/gif,image/png,image/bmp"
        },
        autoUpload: true,
        showProgress: true
    });

    var hasUploaderBackgroundImage = false;
    uploaderBackgroundImage.on("beforeFileQueued", function (file) {
        if ($("#BackgroundFill").val() != "") {
            if (hasUploaderBackgroundImage) {
                hasUploaderBackgroundImage = false;
                return true;
            }
            $.when(waitingConfirm("UpdateCanvasBackImg", "Ok", "Cancel"))
          .done(function () {
              hasUploaderBackgroundImage = true;
              uploaderBackgroundImage.addFiles(file);
          });
            return false;
        }
        else if ($("#divBackgroundImage").css("display") != "none") {
            if (hasUploaderBackgroundImage) {
                hasUploaderBackgroundImage = false;
                return true;
            }
            $.when(waitingConfirm("UpdateCanvasBackColor", "Ok", "Cancel"))
          .done(function () {
              hasUploaderBackgroundImage = true;
              uploaderBackgroundImage.addFiles(file);
          });
            return false;
        }
        else {
            return true;
        }
    });

    uploaderBackgroundImage.on('uploadSuccess', function (file, result) {
        if (result) {
            if (result.success) {
                var uploadUrl = getUploadUrl();
                var $backgroundImage = $("#divBackgroundImage");
                $backgroundImage.css("display", "");
                $backgroundImage.children().get(0).src = uploadUrl + '/upload/' + getToFolder() + "/" + getUploaderPrefix() + result.file;
                uploaderBackgroundImage.disable();
                $("#BackgroundFill").val("");
                $("#BackgroundFill").next().css("background-color", "");

                delete canvas["backgroundColor"];
                canvas.setBackgroundImage(uploadUrl + '/upload/' + getToFolder() + "/" + getUploaderPrefix() + result.file, canvas.renderAll.bind(canvas), {
                    width: canvas.width,
                    height: canvas.height,
                    // Needed to position backgroundImage at 0/0
                    originX: 'left',
                    originY: 'top'
                });

            } else {
                $('#' + file.id).find('.state').text(result.msg);
            }
        } else if (file) {
            var uploadUrl = getUploadUrl();
            var $backgroundImage = $("#divBackgroundImage");
            $backgroundImage.css("display", "");
            $backgroundImage.children().get(0).src = '/upload/' + file.dbName;
            uploaderBackgroundImage.disable();
            $("#BackgroundFill").val("");
            $("#BackgroundFill").next().css("background-color", "");

            delete canvas["backgroundColor"];
            canvas.setBackgroundImage('/upload/' + file.dbName, canvas.renderAll.bind(canvas), {
                width: canvas.width,
                height: canvas.height,
                // Needed to position backgroundImage at 0/0
                originX: 'left',
                originY: 'top'
            });
        }
    });



    //从队列中删除文件时，启用选择按钮
    uploaderBackgroundImage.on('fileDequeued', function () {
        uploaderBackgroundImage.enable();
        $("#divBackgroundImage").css("display", "none");
        $("#BackgroundImage").val("");
        delete canvas["backgroundImage"];
        canvas.renderAll();
    });

    $("#BackgroundFill").blur(function () {
        if ($("#BackgroundImage").val() && $("#BackgroundFill").val()) { //选择了背景图，手动输入背景色离开时，给出提示
            $.when(waitingConfirm("ChangeCanvasBgImgtoBgColor", "Ok", "Cancel"))
                                .done(function () {
                                    CanvasBgColorChange();
                                }).fail(function () {
                                    $("#BackgroundFill").val("");
                                    $("#BackgroundFill").parent().find("div[class='evo-pointer evo-colorind']").css("background-color", "");
                                });
        }
        else {
            if (!$("#BackgroundImage").val() && $("#BackgroundFill").val()) {
                CanvasBgColorChange();
            }
            if (!$("#BackgroundImage").val() && !$("#BackgroundFill").val()) {
                CanvasBgColorChange();
            }
        }
    });
    //  背景图片颜色设置 end




    // Response 背景图片颜色设置 begin
    var uploaderResponseBackgroundImage = SunnetWebUploader.CreateWebUploader({
        pick: "#btnResponseBackgroundImage",
        container: "#ResponseBackgroundImageList",
        submitbutton: "#btnSubmit",
        targetField: "#ResponseBackgroundImage",
        fileSingleSizeLimit: 2097152, //2M
        accept: {
            extensions: "jpg,jpeg,gif,png,bmp",
            mimeTypes: "image/jpg,image/jpeg,image/gif,image/png,image/bmp"
        },
        autoUpload: true,
        showProgress: true
    });

    var hasUploaderResponseBackgroundImage = false;
    uploaderResponseBackgroundImage.on("beforeFileQueued", function (file) {
        if ($("#ResponseBackgroundFill").val() != "") {
            if (hasUploaderResponseBackgroundImage) {
                hasUploaderResponseBackgroundImage = false;
                return true;
            }
            $.when(waitingConfirm("UpdateCanvasBackImg", "Ok", "Cancel"))
          .done(function () {
              hasUploaderResponseBackgroundImage = true;
              uploaderResponseBackgroundImage.addFiles(file);
          });
            return false;
        }
        else if ($("#divResponseBackgroundImage").css("display") != "none") {
            if (hasUploaderResponseBackgroundImage) {
                hasUploaderResponseBackgroundImage = false;
                return true;
            }
            $.when(waitingConfirm("UpdateCanvasBackColor", "Ok", "Cancel"))
          .done(function () {
              hasUploaderResponseBackgroundImage = true;
              uploaderResponseBackgroundImage.addFiles(file);
          });
            return false;
        }
        else {
            return true;
        }
    });

    uploaderResponseBackgroundImage.on('uploadSuccess', function (file, result) {
        if (result) {
            if (result.success) {
                var uploadUrl = getUploadUrl();
                var $backgroundImage = $("#divResponseBackgroundImage");
                $backgroundImage.css("display", "");
                $backgroundImage.children().get(0).src = uploadUrl + '/upload/' + getToFolder() + "/" + getUploaderPrefix() + result.file;
                uploaderResponseBackgroundImage.disable();
                $("#ResponseBackgroundFill").val("");
                $("#ResponseBackgroundFill").next().css("background-color", "");
            } else {
                $('#' + file.id).find('.state').text(result.msg);
            }
        } else if (file) {
            var uploadUrl = getUploadUrl();
            var $backgroundImage = $("#divResponseBackgroundImage");
            $backgroundImage.css("display", "");
            $backgroundImage.children().get(0).src = '/upload/' + file.dbName;
            uploaderResponseBackgroundImage.disable();
            $("#ResponseBackgroundFill").val("");
            $("#ResponseBackgroundFill").next().css("background-color", "");
        }
    });

    //从队列中删除文件时，启用选择按钮
    uploaderResponseBackgroundImage.on('fileDequeued', function () {
        uploaderResponseBackgroundImage.enable();
        $("#divResponseBackgroundImage").css("display", "none");
        $("#ResponseBackgroundImage").val("");
    });

    $("#ResponseBackgroundFill").blur(function () {
        if ($("#ResponseBackgroundImage").val() && $("#ResponseBackgroundFill").val()) { //选择了背景图，手动输入背景色离开时，给出提示
            $.when(waitingConfirm("ChangeCanvasBgImgtoBgColor", "Ok", "Cancel"))
                                .done(function () {
                                    ResponseBgColorChange();
                                }).fail(function () {
                                    $("#ResponseBackgroundFill").val("");
                                    $("#ResponseBackgroundFill").parent().find("div[class='evo-pointer evo-colorind']").css("background-color", "");
                                });
        }
    });
    // Response 背景图片颜色设置 end


    // Instruction Audio 背景图片颜色设置 begin
    var uploaderInstructionAudio = SunnetWebUploader.CreateWebUploader({
        pick: "#btnPickTargetAudio",
        container: "#TargetAudioFilelist",
        submitbutton: "#btnSubmit",
        targetField: "#InstructionAudio",
        fileSingleSizeLimit: 2097152, //2M
        accept: {
            extensions: "mp3",
            mimeTypes: "audio/mpeg"
        },
        autoUpload: true,
        showProgress: true
    });


    uploaderInstructionAudio.on('uploadSuccess', function (file, result) {
        if (result) {
            if (result.success) {
                var uploadUrl = getUploadUrl();
                var $instructionAudio = $("#divInstructionAudio");
                $instructionAudio.css("display", "");
                $instructionAudio.children().get(0).href = uploadUrl + '/upload/' + getToFolder() + "/" + getUploaderPrefix() + result.file;
                uploaderInstructionAudio.disable();
            } else {
                $('#' + file.id).find('.state').text(result.msg);
            }
        } else if (file) {
            var uploadUrl = getUploadUrl();
            var $instructionAudio = $("#divInstructionAudio");
            $instructionAudio.css("display", "");
            $instructionAudio.children().get(0).href = '/upload/' + file.dbName;
            uploaderInstructionAudio.disable();
        }
    });

    //从队列中删除文件时，启用选择按钮
    uploaderInstructionAudio.on('fileDequeued', function () {
        uploaderInstructionAudio.enable();
        $("#divInstructionAudio").css("display", "none");
        $("#InstructionAudio").val("");
    });
    // Instruction Audio 背景图片颜色设置 end

});

function initChooseLayout() {
    $("#LayoutId").val(jsonModel.LayoutId);
    if ($("#LayoutId").val() != "0" && $("#LayoutId").val() != "") {
        selectable = false;
        $("#radioLayout2").prop("checked", true);
        $("#divExistLayout").css("display", "");
    }
    else {
        selectable = true;
        $("#radioLayout1").prop("checked", true);
        $("#divExistLayout").css("display", "none");
    }
}

function addInstructionText(curCanvas, enable) {
    if (jsonModel.InstructionTextNoHtml) {
        var itemLabel = new fabric.Text(jsonModel.InstructionTextNoHtml, {
            left: curCanvas.InstructionLeft ? curCanvas.InstructionLeft : 90,
            top: curCanvas.InstructionTop ? curCanvas.InstructionTop : 10,
            fontFamily: 'Lato',
            fontSize: 20,
            backgroundColor: '#D1D1D1',
            selectable: !enable,
            lockScalingX: true,
            lockScalingY: true,
            hasRotatingPoint: false,
            lockUniScaling: true,
            cornerSize: 0
        });
        curCanvas.add(itemLabel);
    }
}

//此方法必须要定义，用于选择之后的操作(在evol.colorpicker.js中调用)
function CanvasBgColorChange() {
    delete canvas["backgroundImage"];
    if ($("#BackgroundFill").val() != "")
        canvas.backgroundColor = $("#BackgroundFill").val();
    else {
        if ($("#LayoutId").val() != "" && $("#LayoutId").val() != "0")
            canvas.backgroundColor = jsonModel.LayoutBackgroundFill;
        else
            delete canvas["backgroundColor"];
    }

    canvas.renderAll();

    $("#BackgroundImage").val("");
    $("#divBackgroundImage").css("display", "none");
    if ($("#BackgroundImageList") && $("#BackgroundImageList").find(".delete").length > 0) {
        $("#BackgroundImageList").find(".delete").get(0).click();
    }
}

function ResponseBgColorChange() {
    $("#ResponseBackgroundImage").val("");
    $("#divResponseBackgroundImage").css("display", "none");
    if ($("#ResponseBackgroundImageList") && $("#ResponseBackgroundImageList").find(".delete").length > 0) {
        $("#ResponseBackgroundImageList").find(".delete").get(0).click();
    }
}

function getBranchingScore(ID, from, to, itemId, skipItemId, index) {
    var model = {};
    model.uuid = index;
    model.ID = ID || 0;
    model.From = from || 0;
    model.To = to || 0;
    model.ItemId = itemId || 0;
    model.SkipItemId = skipItemId || 0;
    model.IsDeleted = false;
    model.IsVisible = ko.observable(false);
    model.hasError = ko.observable(false);
    return model;
}