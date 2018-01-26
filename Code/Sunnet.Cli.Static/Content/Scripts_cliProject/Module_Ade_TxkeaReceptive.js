jQuery(function () {
    $(window).scroll(function (obj) {
        if (selectedObj && $("#divShow").css("display") == "block") {
            ShowSize(selectedObj);
        }
    });

    $("#BackgroundFillColor").blur(function () {
        if ($("#BackgroundImage").val() && $("#BackgroundFillColor").val()) { //选择了背景图，手动输入背景色离开时，给出提示
            $.when(waitingConfirm("ChangeCanvasBgImgtoBgColor", "Ok", "Cancel"))
                                .done(function () {
                                    CanvasBgColorChange();
                                }).fail(function () {
                                    $("#BackgroundFillColor").val("");
                                    $("#BackgroundFillColor").parent().find("div[class='evo-pointer evo-colorind']").css("background-color", "");
                                });
        }
        else {
            if (!$("#BackgroundImage").val() && $("#BackgroundFillColor").val()) {
                CanvasBgColorChange();
            }
            if (!$("#BackgroundImage").val() && !$("#BackgroundFillColor").val()) {
                CanvasBgColorChange();
            }
        }
    });


    $('#NumberOfImages').change(function () {
        var number = Number($('#NumberOfImages').val());
        itemModel.ImageCount(getAnswersByCount(number, itemModel.ImageCount()));
        setTimeout(function () {
            $("div.tab-content>.tab-pane:not(:first)").removeClass("active").removeClass("in");
            $("div.tab-content>.tab-pane:first").addClass("active").addClass("in");
            $("div.nav-tab>ul.nav-container>li:not(:first)").removeClass("active");
            { $("div.nav-tab>ul.nav-container>li:first").addClass("active"); }
        }, 200);
        //将数量变为1时，若之前选择为多选，则改变成单选
        if (number == 1 && itemModel.SelectionType() == 2) {
            itemModel.SelectionType(1);
        }
    });


    $("#txtCanvaLeft").on("change blur", function () {
        var width = $("#txtCanvaWidth").val() / 100.00 * canvas.width;
        var newLeft = $("#txtCanvaLeft").val() / 100.00 * canvas.width - width/2.00;
        $("#hidCanvaLeft").val(newLeft);
    });
    $("#txtCanvaTop").on("change blur", function () {
        var height = $("#txtCanvaHeight").val() / 100.00 * canvas.height;
        var newTop = $("#txtCanvaTop").val() / 100.00 * canvas.height - height/2.00;
        $("#hidCanvaTop").val(newTop);
    });

    $("#txtCanvaWidth").on("change blur", function () {
        var width = $("#txtCanvaWidth").val() / 100.00 * canvas.width;
        if (preObjWidth != width) {
            var rate = (selectedObj.target.scaleY * selectedObj.target.height) / (selectedObj.target.scaleX * selectedObj.target.width);
            $("#hidCanvaWidth").val(width);
            $("#hidCanvaHeight").val(Math.floor($("#hidCanvaWidth").val() * rate));
            $("#txtCanvaHeight").val(($("#hidCanvaHeight").val()/canvas.height*100.00).toFixed(2));
        }
    });

    $("#txtCanvaHeight").on("change blur", function () {
        var height = $("#txtCanvaHeight").val() / 100.00 * canvas.height;
        if (preObjHeight != height) {
            var rate = (selectedObj.target.scaleX * selectedObj.target.width) / (selectedObj.target.scaleY * selectedObj.target.height);
            //$("#txtCanvaWidth").val(Math.floor($("#txtCanvaHeight").val() * rate));
            $("#hidCanvaHeight").val(height);
            $("#hidCanvaWidth").val(Math.floor($("#hidCanvaHeight").val() * rate));
            $("#txtCanvaWidth").val(($("#hidCanvaWidth").val() / canvas.height * 100.00).toFixed(2));
        }
    });

    $("#txtCanvaHeight").on("focus", function () {
        preObjHeight = $("#txtCanvaHeight").val()/100.00*canvas.height;
    });

    $("#txtCanvaWidth").on("focus", function () {
        preObjWidth = $("#txtCanvaWidth").val()/100.00*canvas.width;
    });

    setTimeout(function () {
        $("div.tab-content>.tab-pane:not(:first)").removeClass("active").removeClass("in");
    }, 200);

    //resize   process only last one
    var rtime = new Date();
    var timeout = false;
    var delta = 200;
    $(window).resize(function () {
        if (typeof (canvas) != "undefined")
        {
            canvas.discardActiveObject();
            canvas.fire('canvas:cleared');

            rtime = new Date();
            if (timeout === false) {
                timeout = true;
                setTimeout(resizeend, delta);
            }
        }
    });

    //resize   process only last one
    function resizeend() {
        if (new Date() - rtime < delta) {
            setTimeout(resizeend, delta);
        } else {
            timeout = false;
            if (canvas) {
                if (typeof (itemModel) != "undefined") {
                    if (itemModel.step && itemModel.step() == 4) {  //engage receptive item 时，要等到layout界面变化时，再重置canvas
                        CanvasResize(JSON.stringify(canvas.toDatalessJSON(new Array("id", "sort", "lockUniScaling", "selectable",
                            "lockScalingX", "lockScalingY", "hasRotatingPoint", "cornerSize"))),
                            canvas.width, "content-body-cav");
                    }
                }
            }
        }
    }

    CKEDITOR.replace('InstructionText', { toolbar: 'Cli' });

    var hasuploaderBackgroundImage = false;

    var uploaderBackgroundImage = SunnetWebUploader.CreateWebUploader({
        pick: "#btnBackgroundFill",
        container: "#BackgroundFillList",
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

    uploaderBackgroundImage.on("beforeFileQueued", function (file) {
        if ($("#BackgroundFillColor").val() != "") {
            if (hasuploaderBackgroundImage) {
                hasuploaderBackgroundImage = false;
                return true;
            }
            $.when(waitingConfirm("UpdateCanvasBackImg", "Ok", "Cancel"))
          .done(function () {
              hasuploaderBackgroundImage = true;
              uploaderBackgroundImage.addFiles(file);
          });
            return false;
        }
        else if ($("#divBackgroundFill").css("display") != "none") {
            if (hasuploaderBackgroundImage) {
                hasuploaderBackgroundImage = false;
                return true;
            }
            $.when(waitingConfirm("UpdateCanvasBackColor", "Ok", "Cancel"))
          .done(function () {
              hasuploaderBackgroundImage = true;
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
                var $backgroundImage = $("#divBackgroundFill");
                $backgroundImage.css("display", "");
                $backgroundImage.children().get(0).src = '/upload/' + getToFolder() + "/" + getUploaderPrefix() + result.file;
                uploaderBackgroundImage.disable();
                $("#BackgroundFillColor").val("");
                $("#BackgroundFillColor").next().css("background-color", "");
                canvas.setBackgroundColor("");

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
            var $backgroundImage = $("#divBackgroundFill");
            $backgroundImage.css("display", "");
            $backgroundImage.children().get(0).src = '/upload/' + file.dbName;
            uploaderBackgroundImage.disable();
            $("#BackgroundFillColor").val("");
            $("#BackgroundFillColor").next().css("background-color", "");
            canvas.setBackgroundColor("");
            canvas.setBackgroundImage(uploadUrl + '/upload/' + file.dbName, canvas.renderAll.bind(canvas), {
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
        $("#divBackgroundFill").css("display", "none");
        $("#BackgroundImage").val("");
        canvas.renderAll();
    });

    // Instruction Audio  begin
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
    // Instruction Audio end
})

function getReceptiveAnswerModel(defaultValues) {
    function AnswerModel() { };

    AnswerModel.prototype = getUploaderHelper();
    var self = new AnswerModel();

    self.ID = isNull("ID", defaultValues, 0);
    self.ItemId = isNull("ItemId", defaultValues, 0);

    self.Picture = ko.observable(isNull("Picture", defaultValues, ""));
    if (self.ID > 0)
        self.PictureTime = ko.observable(isNull("PictureTime", defaultValues, ""));
    else
        self.PictureTime = ko.observable(1000);
    self.Audio = ko.observable(isNull("Audio", defaultValues, ""));
    self.AudioTime = ko.observable(isNull("AudioTime", defaultValues, ""));
    self.IsCorrect = ko.observable(isNull("IsCorrect", defaultValues, false));
    self.Score = isNull("Score", defaultValues, "");
    self.Text = isNull("Text", defaultValues, "");
    self.TextTime = isNull("TextTime", defaultValues, "");
    self.Value = isNull("Value", defaultValues, "");
    self.Maps = isNull("Maps", defaultValues, "");
    self.ImageType = ko.observable(isNull("ImageType", defaultValues, 1));
    self.SameasImageDelay = ko.observable(self.PictureTime() == self.AudioTime() && self.PictureTime() > 0);
    self.SequenceNumber = isNull("SequenceNumber", defaultValues, 0);
    self.ResponseAudio = ko.observable(isNull("ResponseAudio", defaultValues, ""));

    self.PictureTime.subscribe(function (newVal) {
        if (self.SameasImageDelay()) {
            self.AudioTime(newVal);
        }
    });

    self.SameasImageDelay.subscribe(function (newVal) {
        if (newVal) {
            self.AudioTime(self.PictureTime());
        }
    });

    self.hasError = ko.observable(false);

    return self;
}

function getAnswersByCount(answerCount, preAnswers) {
    while (preAnswers.length < answerCount) {
        preAnswers.push(getReceptiveAnswerModel(AnswerDefault.Others));
    }
    while (preAnswers.length > answerCount) {
        preAnswers.pop();
    }
    return preAnswers;
}

function CloseSize() {
    var $divShow = $("#divShow");
    if ($divShow.css('display') == 'block')
        $divShow.css('display', 'none');
}


function ShowSize(obj) {
    if (obj.target && obj.target.id && obj.target.id != '') {
        selectedObj = obj;

        var readWith = obj.target.width * obj.target.scaleX;
        var readHeight = obj.target.height * obj.target.scaleY;

        //if out of right , then left
        var left = obj.target.left + readWith + $("#cav_layout").offset().left + 10;
        if (left + $("#divShow").width() > canvas.width + $("#cav_layout").offset().left) {
            left = obj.target.left - $("#divShow").width() + $("#cav_layout").offset().left - 10;
            if (left < 0)
                left = 0;
            $("span.arrow-left-bot").attr("class", "arrow-right-bot");
            $("span.arrow-left-top").attr("class", "arrow-right-top");
        }
        else {
            if ($("span.arrow-right-bot").length > 0)
                $("span.arrow-right-bot").attr("class", "arrow-left-bot");
            if ($("span.arrow-right-top").length > 0)
                $("span.arrow-right-top").attr("class", "arrow-left-top");
        }

        var $divShow = $("#divShow");
        $divShow.css('display', 'block');
        $divShow.css('top', obj.target.top + $("#cav_layout").offset().top - $(document).scrollTop());
        $divShow.css('left', left);

        $("#hidCanvaWidth").val(readWith);
        $("#hidCanvaHeight").val(readHeight);
        $("#hidCanvaLeft").val(obj.target.left);
        $("#hidCanvaTop").val(obj.target.top);

        $("#txtCanvaHeight").val((readHeight / canvas.height * 100.00).toFixed(2));
        $("#txtCanvaWidth").val((readWith / canvas.width * 100.00).toFixed(2));
        $("#txtCanvaLeft").val(((obj.target.left + readWith / 2.00) / canvas.width * 100.00).toFixed(2));
        $("#txtCanvaTop").val(((obj.target.top + readHeight / 2.00) / canvas.height * 100.00).toFixed(2));


        $("#spanCanvasWidth").html(canvas.width.toFixed(2) + "px");
        $("#spanCanvasHeight").html(canvas.height.toFixed(2) + "px");
        preScroll = $("#divShow").offset().top;
    }
}

function ResizeObjSize() {
    if (selectedObj && selectedObj.target) {
        //var nHeight = Number($("#txtCanvaHeight").val());
        //var nWidth = Number($("#txtCanvaWidth").val());
        //var nLeft = Number($("#txtCanvaLeft").val());
        //var nTop = Number($("#txtCanvaTop").val());

        var nHeight = Number($("#hidCanvaHeight").val());
        var nWidth = Number($("#hidCanvaWidth").val());
        //var nLeft = Number($("#hidCanvaLeft").val());
        //var nTop = Number($("#hidCanvaTop").val());
        var nLeft = Number($("#txtCanvaLeft").val()) / 100.00 * canvas.width - nWidth / 2.00;//Number($("#hidCanvaLeft").val()) ;
        var nTop = Number($("#txtCanvaTop").val()) / 100.00 * canvas.height - nHeight / 2.00; //Number($("#hidCanvaTop").val());
        var selectedCanvasObj = canvas._objects.filter(function (obj) { return obj.id == selectedObj.target.id });

        var rate = (selectedObj.target.scaleY * selectedObj.target.height) / (selectedObj.target.scaleX * selectedObj.target.width);
        if (nWidth > canvas.width) {
            nWidth = canvas.width - minPadding;
            nHeight = Math.floor(nWidth * rate);
        }
        if (nHeight > canvas.height) {
            nHeight = canvas.height - minPadding;
            nWidth = Math.floor(nHeight / rate);
        }

        if (selectedCanvasObj.length > 0) {
            if (selectedObj.target.height * selectedObj.target.scaleY != nHeight ||
                     selectedObj.target.width * selectedObj.target.scaleX != nWidth
                  || selectedObj.target.left != nLeft ||
                        selectedObj.target.top != nTop)
            {
                var newscaleY = nHeight / selectedObj.target.height;
                var newscaleX = nWidth / selectedObj.target.width;
                if (newscaleY > 0)
                    selectedCanvasObj[0].scaleY = newscaleY;
                if (newscaleX > 0)
                    selectedCanvasObj[0].scaleX = newscaleX;

                selectedCanvasObj[0].left = nLeft;
                selectedCanvasObj[0].top = nTop;

                selectedObj.target.scaleX = newscaleX;
                selectedObj.target.scaleY = newscaleY;
                selectedObj.target.left = nLeft;
                selectedObj.target.top = nTop;

                selectedCanvasObj[0].setCoords();
                canvas.renderAll();
                RePositionLayout(selectedObj);
                ShowSize(selectedObj);
            }
        }
    }
}

function fileDequeuedTargetImage(model) {
    model.Picture("");
    model.PictureTime(0);
}

function uploadSuccessTargetImage(model, img, id) {
    model.Picture(img);
    $("#" + id).valid();
}

function fileDequeuedTargetAudio(model) {
    model.Audio("");
    model.AudioTime(0);
    model.SameasImageDelay(false);
}

function uploadSuccessTargetAudio(model, img, id) {
    model.Audio(img);
    $("#" + id).valid();
}

function uploadSuccessResponseTargetAudio(model, audio, id) {
    model.ResponseAudio(audio);
    $("#" + id).valid();
}

function fileDequeuedResponseTargetAudio(model) {
    model.ResponseAudio("");
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

function chooseLayout(o) {
    if ($(o).val() == 2) {
        $("#divExistLayout").css("display", "");
    }
    else {  //custom
        var tmpLayoutId = $("#LayoutId").val();
        if (tmpLayoutId != "" && tmpLayoutId != "0") {
            $.when(waitingConfirm("ChangetoCustom", "Ok", "Cancel"))
              .done(function () {
                  $("#LayoutId").val("0");
                  if (typeof (jsonModel) != 'undefined')   //确保该值为最新值
                  {
                      jsonModel.LayoutBackgroundImage = "";
                      jsonModel.LayoutBackgroundColor = "";
                      jsonModel.LayoutId = 0;
                  }
                  $("#divExistLayout").css("display", "none");
                  $("#radioLayout1").prop("checked", true);
                  setCustomCanvas();
              });
            $("#radioLayout2").prop("checked", true);
        }
        else
            $("#divExistLayout").css("display", "none");
    }
}

function checkStepOne() {
    //Clone时第一步都要提交
    if ($("input[name=ID]").val() == 0 && $("#copyId").val() > 0)
        return true;

    updateCkeditor();
    if (jsonModel.Label != $("#Label").val())
        return true;
    if (jsonModel.Status.value != $("#Status").val() && jsonModel.ItemLayout)
        return true;
    if (jsonModel.IsPractice != $("#IsPractice").prop("checked"))
        return true;
    if (jsonModel.GrayedOutDelay != $("#GrayedOutDelay").prop("checked"))
        return true;
    if (jsonModel.BackgroundFill != $("#BackgroundFillColor").val() || jsonModel.BackgroundImage != $("#BackgroundImage").val())
        return true;
    //html 标签的空格显示不同
    if (encodeURI(jsonModel.InstructionText).replace(/D%0/g, "") != encodeURI($("#InstructionText").val()))
        return true;
    if (jsonModel.InstructionAudio != $("#InstructionAudio").val())
        return true;
    if (jsonModel.NumberOfImages != $("#NumberOfImages").val())
        return true;
    if (jsonModel.SelectionType.value != $("input[name='SelectionType']:checked").val())
        return true;

    var oldBranchingScoreList = [];
    if (itemModel.oldBranchingScores)
        oldBranchingScoreList = JSON.parse(itemModel.oldBranchingScores);

    for (i = 0; i < itemModel.branchingScores().length; i++) {
        if (itemModel.branchingScores()[i].IsDeleted)
            return true;
        if (itemModel.branchingScores()[i].ID == 0)
            return true;

        var findItem = oldBranchingScoreList.filter(function (item) {
            return item.ID == itemModel.branchingScores()[i].ID
            });
        if (findItem && findItem.length > 0) {
            if (findItem[0].From == itemModel.branchingScores()[i].From
                && findItem[0].To == itemModel.branchingScores()[i].To
                && findItem[0].SkipItemId == itemModel.branchingScores()[i].SkipItemId) {
                continue;
            }
            return true;
        }
        else
            return true;
        }
    return false;
}

function checkStepTwo() {
    if (jsonModel.ImageSequence.value != $("input[name='ImageSequence']:checked").attr("realValue"))
        return true;
    if (jsonModel.OverallTimeOut.toString() != $("input[name='OverallTimeOut']:checked").val().toLocaleLowerCase())
        return true;
    if ($("#TimeoutValue").length > 0 && jsonModel.TimeoutValue != $("#TimeoutValue").val())
        return true;
    if ($("input[name='BreakCondition']").length > 0 && jsonModel.BreakCondition.value != $("input[name='BreakCondition']:checked").val())
        return true;
    if (jsonModel.BreakCondition.value == 1 && jsonModel.StopConditionX != $("#StopConditionX").val())
        return true;
    if (jsonModel.BreakCondition.value == 1 && jsonModel.StopConditionY != $("#StopConditionY").val())
        return true;
    if ($("input[name='Scoring']").length > 0 && jsonModel.Scoring.value != $("input[name='Scoring']:checked").attr("realValue"))
        return true;
    return false;
}

function checkStepThree() {
    if (itemModel.ImageCount().length != jsonModel.Answers.length) {
        return true;
    }
    for (var i = 0; i < itemModel.ImageCount().length; i++) {
        var curAnswer = itemModel.ImageCount()[i];
        var preAnswer = jsonModel.Answers[i];

        if (preAnswer.ImageType.value != curAnswer.ImageType())
            return true;
        if (preAnswer.Picture != curAnswer.Picture())
            return true;
        if (preAnswer.PictureTime != curAnswer.PictureTime())
            return true;
        if (preAnswer.Audio != curAnswer.Audio())
            return true;
        if (preAnswer.AudioTime != curAnswer.AudioTime())
            return true;
        if (preAnswer.ResponseAudio != curAnswer.ResponseAudio())
            return true;
        if (preAnswer.IsCorrect != curAnswer.IsCorrect())
            return true;
        if (preAnswer.Score != curAnswer.Score)
            return true;
        if (preAnswer.SequenceNumber != curAnswer.SequenceNumber)
            return true;
        }
    return false;
}

//此方法必须要定义，用于选择之后的操作
function CanvasBgColorChange() {
    if ($("#BackgroundFillColor").val() != "")
        canvas.backgroundColor = $("#BackgroundFillColor").val();
        else {
        if ($("#LayoutId").val() != "" && $("#LayoutId").val() != "0")
            canvas.backgroundColor = jsonModel.LayoutBackgroundFill;
            else
            delete canvas["backgroundColor"];
            }
    $("#BackgroundImage").val("");

    delete canvas["backgroundImage"];
    $("#divBackgroundFill").css("display", "none");
    if ($("#BackgroundFillList") && $("#BackgroundFillList").find(".delete").length > 0) {
        $("#BackgroundFillList").find(".delete").get(0).click()
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