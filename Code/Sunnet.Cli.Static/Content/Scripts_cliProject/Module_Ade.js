
var ImageType = {
    Selectable: 1,
    NonSelectable: 2
};

function getCutoffScore(FromYear, FromMonth, ToYear, ToMonth, score, ID, wave,
    BenchmarkId, LowerScore, HigherScore, ShowOnGroup, benchmarks, lineNum) {
    function AdeCutoffScore() { };

    var model = {};
    model.constructor = AdeCutoffScore;

    model.ID = ID || 0;
    model.FromYear = FromYear || 0;
    model.FromMonth = FromMonth || 0;
    model.ToYear = ToYear || 0;
    model.ToMonth = ToMonth || 0;
    model.Wave = wave || 1;

    model.fromAge = function () {
        return +model.FromYear * 12 + +model.FromMonth;
    }
    model.toAge = function () {
        return +model.ToYear * 12 + +model.ToMonth;
    }
    if (model.toAge() < model.fromAge()) {
        model.toYear = +model.FromYear + 1;
        model.toMonth = 0;
    }
    model.CutOffScore = score || 0;

    model.hasError = ko.observable(false);
    model.LowerScore = LowerScore || 0;
    model.HigherScore = HigherScore || 0;
    model.BenchmarkId = BenchmarkId;
    model.ShowOnGroup = ShowOnGroup || false;

    var defaultBenmark = getBenchmark(0, "Please select...", "#c3c3c3", 0, "");
    if (benchmarks) {
        if (BenchmarkId && BenchmarkId != 0) {
            for (var i = 0; i < benchmarks.length; i++) {
                if (benchmarks[i].ID == BenchmarkId) {
                    defaultBenmark = benchmarks[i];
                    break;
                }
            }
        }
    }
    model.selectedBenchmark = ko.observable(defaultBenmark);
    model.selectedBenchmark().ID = BenchmarkId;
    model.lineNum = ko.observable(lineNum || 0);
    return model;
}
function getAdeScoreModel(defaultScores, formId, scoreControlId, deletedControlId, jsonBenchmarks) {
    function AdeScoreModel() { };
    var self = {};
    self.constructor = AdeScoreModel;

    var defaultBenmark = getBenchmark(0, "Please select...", "#c3c3c3", 0, "");
    self.benchmarks = ko.observableArray([defaultBenmark]);
    if (jsonBenchmarks && jsonBenchmarks.length) {
        for (var i = 0; i < jsonBenchmarks.length; i++) {
            var benchmark = getBenchmark(
                jsonBenchmarks[i].ID,
                jsonBenchmarks[i].LabelText,
                jsonBenchmarks[i].Color,
                jsonBenchmarks[i].BlackWhite.value,
                jsonBenchmarks[i].BlackWhite.text);
            self.benchmarks.push(benchmark);
        }
    }

    self.benchmarkChange = function (score) {
        score.BenchmarkId = score.selectedBenchmark().ID;
    };

    self.scores = ko.observableArray([]);
    self.$form = $("#" + formId);
    self.$scores = self.$form.find("#" + scoreControlId);
    self.$deleted = self.$form.find("#" + deletedControlId);
    self.wave1 = ko.observable(false);
    self.wave2 = ko.observable(false);
    self.wave3 = ko.observable(false);

    self.waveNum1 = ko.observable(0);
    self.waveNum2 = ko.observable(0);
    self.waveNum3 = ko.observable(0);

    self.newScore = function (wave) {
        var newScore = getCutoffScore();
        newScore.Wave = wave;
        if (wave == 1) {
            newScore.lineNum(self.waveNum1());
            self.waveNum1(self.waveNum1() + 1);
        }
        else if (wave == 2) {
            newScore.lineNum(self.waveNum2());
            self.waveNum2(self.waveNum2() + 1);
        }
        else if (wave == 3) {
            newScore.lineNum(self.waveNum3());
            self.waveNum3(self.waveNum3() + 1);
        }
        self.scores.push(newScore);
    }
    self.deleteScore = function (score) {
        var $del = self.$deleted;
        $del.val($del.val() + "," + score.ID);
        self.scores.remove(score);

        if (score.Wave == 1 && self.waveNum1() > 0) {
            self.waveNum1(self.waveNum1() - 1);
        }
        else if (score.Wave == 2 && self.waveNum2() > 0) {
            self.waveNum2(self.waveNum2() - 1);
        }
        else if (score.Wave == 3 && self.waveNum3() > 0) {
            self.waveNum3(self.waveNum3() - 1);
        }

        self.resetLineNum();
        self.ensureScoreofWave(score.Wave);
    }
    self.ensureScoreofWave = function (wave) {
        var exists = false;
        $.each(self.scores(), function (index, score) {
            if (score.Wave == wave) {
                exists = true;
                return false;
            }
        });
        if (exists == false) {
            self.newScore(wave);
        }
    }
    if (defaultScores && defaultScores.length) {
        for (var i = 0; i < defaultScores.length; i++) {
            var wave = defaultScores[i].Wave.value;
            var waveNum = self.waveNum1();
            if (wave == 2) {
                waveNum = self.waveNum2();
            }
            else if (wave == 3) {
                waveNum = self.waveNum3();
            }
            self.scores.push(getCutoffScore(
                defaultScores[i].FromYear,
                defaultScores[i].FromMonth,
                defaultScores[i].ToYear,
                defaultScores[i].ToMonth,
                defaultScores[i].CutOffScore,
                defaultScores[i].ID,
                defaultScores[i].Wave.value,
                defaultScores[i].BenchmarkId,
                defaultScores[i].LowerScore,
                defaultScores[i].HigherScore,
                defaultScores[i].ShowOnGroup,
                self.benchmarks(),
                waveNum));

            if (wave == 1) {
                self.waveNum1(self.waveNum1() + 1);
            }
            else if (wave == 2) {
                self.waveNum2(self.waveNum2() + 1);
            }
            else if (wave == 3) {
                self.waveNum3(self.waveNum3() + 1);
            }
        }
    }
    self.ensureScoreofWave(1);
    self.ensureScoreofWave(2);
    self.ensureScoreofWave(3);

    self.resetLineNum = function () {
        self.waveNum1(0);
        self.waveNum2(0);
        self.waveNum3(0);
        for (var i = 0; i < self.scores().length; i++) {
            var wave = self.scores()[i].Wave;
            if (wave == 1) {
                self.scores()[i].lineNum(self.waveNum1());
                self.waveNum1(self.waveNum1() + 1);
            }
            else if (wave == 2) {
                self.scores()[i].lineNum(self.waveNum2());
                self.waveNum2(self.waveNum2() + 1);
            }
            else if (wave == 3) {
                self.scores()[i].lineNum(self.waveNum3());
                self.waveNum3(self.waveNum3() + 1);
            }
        }
    }

    self.prapareSubmitData = function () {
        var validated = true;
        var total = self.scores().length;
        if (typeof (CKupdate) != "undefined") {
            CKupdate();
        }
        if (total > 0) {
            for (var i = 0; i < total; i++) {
                if (self.scores()[i].toAge() < self.scores()[i].fromAge()) {
                    self.scores()[i].hasError(true);
                    validated = false;
                    showMessage("hint", "Assessment_CutoffScore_order");
                    break;
                } else {
                    self.scores()[i].hasError(false);
                }
                //if (i > 0) {
                //    if (self.scores()[i - 1].toAge() > self.scores()[i].fromAge()
                //        && self.scores()[i - 1].Wave == self.scores()[i].Wave
                //        ) {
                //        self.scores()[i].hasError(true);
                //        showMessage("hint", "Assessment_CutoffScore_order");
                //        validated = false;
                //        break;
                //    } else {
                //        self.scores()[i].hasError(false);
                //    }
                //}
                if (Number(self.scores()[i].LowerScore) > Number(self.scores()[i].HigherScore)) {
                    self.scores()[i].hasError(true);
                    showMessage("hint", "Assessment_Score_Range");
                    validated = false;
                    break;
                } else {
                    self.scores()[i].hasError(false);
                }
                //if (i > 0) {
                //    if (Number(self.scores()[i - 1].HigherScore) >= Number(self.scores()[i].LowerScore)
                //        && self.scores()[i - 1].Wave == self.scores()[i].Wave
                //        ) {
                //        self.scores()[i].hasError(true);
                //        showMessage("hint", "Assessment_Score_Range");
                //        validated = false;
                //        break;
                //    } else {
                //        self.scores()[i].hasError(false);
                //    }
                //}
            }
        }
        if (validated) {
            self.$scores.val(JSON.stringify(self.scores()));
            return true;
        }
        return false;
    }
    self.submit = function () {
        if (self.prapareSubmitData())
            self.$form.submit();
    }

    return self;
};



var getUploaderHelper = function () {
    function AdeUploaderHelper() { };

    var self = new AdeUploaderHelper();

    function getUploaderContainer(uploaderKey) {
        if (!uploaderKey) {
            uploaderKey = window.guid("UploaderKey");
        }
        if (!(uploaderKey in window)) {
            window[uploaderKey] = {};
        }
        return window[uploaderKey];
    };

    self.uploader = function () {
        var container = getUploaderContainer(window.guid("UploaderKey"));
        return function (key, uploader) {
            var uploaders_ = container;
            if (arguments.length == 1) {
                if (key in uploaders_) {
                    return uploaders_[key];
                }
                return null;
            } else if (arguments.length == 2) {
                uploaders_[key] = uploader;
            }
        };
    }();
    return self;
}

var getAnswerModel = function (defaultValues) {
    function AnswerModel() { };

    AnswerModel.prototype = getUploaderHelper();
    var self = new AnswerModel();

    self.ID = isNull("ID", defaultValues, 0);
    self.ItemId = isNull("ItemId", defaultValues, 0);

    self.Picture = isNull("Picture", defaultValues, "");
    self.PictureTime = isNull("PictureTime", defaultValues, "");
    self.Audio = isNull("Audio", defaultValues, "");
    self.AudioTime = isNull("AudioTime", defaultValues, "");
    self.IsCorrect = isNull("IsCorrect", defaultValues, false);
    self.Score = isNull("Score", defaultValues, "");
    self.Text = isNull("Text", defaultValues, "");
    self.TextTime = isNull("TextTime", defaultValues, "");
    self.Value = isNull("Value", defaultValues, "");
    self.Maps = isNull("Maps", defaultValues, "");
    self.ImageType = isNull("value", defaultValues.ImageType, ImageType.Selectable);
    self.SequenceNumber = isNull("SequenceNumber", defaultValues, 0);
    self.ResponseAudio = isNull("ResponseAudio", defaultValues, "");

    self.hasError = ko.observable(false);

    return self;
}


AnswerDefault = {
    PA: {
        PictureTime: 0,
        AudioTime: 0,
        TextTime: 0
    },
    CEC: {
        PictureTime: 0,
        AudioTime: 0,
        TextTime: 0
    },
    Checklist: {
        PictureTime: 0,
        AudioTime: 0,
        TextTime: 0
    },
    Others: {
        Score: 0,
        TextTime: 0,
        PictureTime: 0,
        AudioTime: 0
    }
};

function getItemModel(defaultValues, controls) {
    function ItemModel() { };
    ItemModel.prototype = getUploaderHelper();
    var self = new ItemModel();

    self.uuid = window.guid("Item");

    self.MeasureId = 0;
    self.ID = 0;
    self.BasePath = "";

    self.Label = "";
    self.Scored = ko.observable(false);
    self.Score = 0;
    self.Timed = false;
    self.Sort = 0;
    self.Type = 0;

    self.CreatedBy = 0;
    self.CreatedOn = "";
    self.UpdatedBy = 0;
    self.UpdatedOn = "";

    // common
    self.Description = "";

    // cpalls+
    self.TargetText = "";
    self.TargetTextTimeout = 0;
    self.TargetAudio = "";
    self.TargetAudioTimeout = 0;

    // cec
    self.IsMultiChoice = false;
    self.ResponseCount = 0;
    // None = 0,Portrait = 1,Landscape = 2
    self.Direction = 0;

    // cot
    self.ShortTargetText = "";
    self.FullTargetText = "";
    self.PrekindergartenGuidelines = "";
    self.CircleManual = "";
    self.MentoringGuide = "";

    // ReceptivePrompt
    self.PromptPicture = "";
    self.PromptPictureTimeout = 0;
    self.PromptText = "";
    self.PromptTextTimeout = 0;
    self.PromptAudio = "";
    self.PromptAudioTimeout = 0;

    // Direction
    // Direction Text
    self.FullDescription = "";

    self.IsPractice = ko.observable(false);
    self.ShowAtTestResume = ko.observable(false);
    self.IsPractice.subscribe(function (isPractice) {
        if (isPractice) {
            self.Scored(false);
        } else {
            self.ShowAtTestResume(false);
        }
    });

    self.Answers = ko.observableArray([]);
    self.answerCount = 0;

    self.defaultAnswerOption = AnswerDefault.Others;

    self.init = function (defaultValues, controls) {
        self.ID = isNull("ID", defaultValues, 0);
        self.BasePath = isNull("BasePath", defaultValues, "");
        self.Scored(defaultValues.Scored);
        self.IsPractice(defaultValues.IsPractice);
        self.ShowAtTestResume(defaultValues.ShowAtTestResume);
        self.Type = (defaultValues && defaultValues.Type.value) || 0;

        switch (self.Type) {
            case Ade_ItemType.Direction.value:// Direction has no answers
                break;
            case Ade_ItemType.Cec.value:// Cec has at most 8 answers
                self.answerCount = 1;
                self.defaultAnswerOption = AnswerDefault.CEC;
                break;
            case Ade_ItemType.Checklist.value:// Checklist has at most 8 answers
                self.answerCount = 1;
                self.defaultAnswerOption = AnswerDefault.Checklist;
                break;
            case Ade_ItemType.Cot.value:// Cot has no answers
                break;
            case Ade_ItemType.MultipleChoices.value:
                self.answerCount = 8;
                // MultipleChoices has at most 8 answers
                break;
            case Ade_ItemType.Pa.value:
                // Pa has at most 8 answers
                self.answerCount = 1;
                self.defaultAnswerOption = AnswerDefault.PA;
                break;
            case Ade_ItemType.Quadrant.value:
                // Quadrant has 4 answers
                self.answerCount = 4;
                break;
            case Ade_ItemType.RapidExpressive.value:
                // RapidExpressive has 1 answers
                self.answerCount = 1;
                break;
            case Ade_ItemType.TypedResponse.value:
                self.answerCount = 1;
                break;
            case Ade_ItemType.Receptive.value:
                //Receptive:3 answers
                self.answerCount = 3;
                break;
            case Ade_ItemType.ReceptivePrompt.value:
                // ReceptivePrompt has 3 answers + 1 prompt picture.
                self.answerCount = 3;
                break;
            case Ade_ItemType.ObservableChoice.value:
                // Pa has at most 3 answers
                self.answerCount = 1;
                break;
            default:
                break;
        }
        if (self.ID < 1 || defaultValues.Answers.length) {
            if (defaultValues.Answers.length != 0) {
                self.answerCount = defaultValues.Answers.length;
                for (var i = 0; i < defaultValues.Answers.length; i++) {
                    self.Answers.push(getAnswerModel(defaultValues.Answers[i]));
                }
            } else {
                for (var i = 0; i < self.answerCount; i++) {
                    self.Answers.push(getAnswerModel(self.defaultAnswerOption));
                }
            }
        } else {
            for (var i = 0; i < defaultValues.Answers.length; i++) {
                self.Answers.push(getAnswerModel(defaultValues.Answers[i]));
            }
        }
    }

    self.formPrepared = function (viewModel, event) {
        updateCkeditor();
        var $sender = $(event.target);
        var $form = $sender.closest("form");
        var uploaders = [];
        var uploader = null;
        var waitingCount = 0;
        if ((uploader = viewModel.uploader("TargetAudioUploader"))) {
            waitingCount += uploader.getStats().queueNum;

            uploaders.push(viewModel.uploader("TargetAudioUploader"));
        }

        viewModel.answerCount = viewModel.Answers().length;
        var answers = viewModel.Answers(), answer;
        for (var i = 0; i < viewModel.answerCount; i++) {
            answer = answers[i];
            var picKey = "Answer" + i + "PictureUploader";
            if ((uploader = answer.uploader(picKey))) {
                waitingCount += uploader.getStats().queueNum;

                uploaders.push(answer.uploader(picKey));
            }
            var audioKey = "Answer" + i + "AudioUploader";
            if ((uploader = answer.uploader(audioKey))) {
                waitingCount += uploader.getStats().queueNum;

                uploaders.push(answer.uploader(audioKey));
            }
        }
        if (self.getCustomUploaders) {
            var customUploader = self.getCustomUploaders(viewModel);
            if (customUploader && customUploader.count && customUploader.uploaders) {
                waitingCount += customUploader.count;
                uploaders = uploaders.concat(customUploader.uploaders);
            }
        }

        var waiting = $sender.data("waiting") || 0;
        waiting += waitingCount;
        $sender.data("waiting", waiting);

        var uploaded = $sender.data("uploaded") || 0;
        $sender.data("clicked", true);
        if (uploaders.length) {
            for (var i = 0; i < uploaders.length; i++) {
                if (uploaders[i].getStats().queueNum > 0) {
                    if (uploaders[i].state == "ready") {
                        uploaders[i].upload();
                    }
                }
            }
        }
        return waiting == uploaded;
    }

    self.prepareAnswers = function () {
        var answers = self.Answers(), answer;
        for (var i = 0; i < self.answerCount; i++) {
            answer = answers[i];
            answer.Picture = isTrueValue("Picture", answer, "");
            answer.PictureTime = isTrueValue("PictureTime", answer, 0);
            answer.Audio = isTrueValue("Audio", answer, "");
            answer.AudioTime = isTrueValue("AudioTime", answer, 0);
            answer.Score = isTrueValue("Score", answer, 0);
            answer.TextTime = isTrueValue("TextTime", answer, 0);
        }
        return true;
    }

    self.submit = function (viewModel, event) {
        var $sender = $(event.target);
        var $form = $sender.closest("form");
        if (!self.formPrepared(viewModel, event)) {
            return false;
        }
        var res = $form.valid();
        if (res == false) {
            return false;
        }

        self.prepareAnswers();
        $form.find(controls.Answer).val(JSON.stringify(viewModel.Answers()));

        if (self.onBeforeSubmit) {
            self.onBeforeSubmit(viewModel);
        }

        $form.submit();
        return false;
    }

    self.newAnswer = function (data) {
        if (data.Text.length == 0) {
            data.hasError(true);
            showMessage("hint", "CEC_PA_Fill_Item");
            return false;
        } else {
            data.hasError(false);
        }
        if (self.Answers().length < 10) {
            self.Answers.push(getAnswerModel(self.defaultAnswerOption));
        } else {
            showMessage("hint", "over_Max_Pa_ItemCount");
        }
    };

    self.deleteAnswer = function (answer) {
        self.Answers.remove(answer);
        if (self.Answers().length == 0) {
            self.Answers.push(getAnswerModel(self.defaultAnswerOption));
        }
    }

    if (arguments.length) {
        self.init(defaultValues, controls);
    }

    return self;
}

function getCecItemModel(defaultValues, controls) {
    var self = getItemModel(defaultValues, controls);

    return self;
}

function getChecklistItemModel(defaultValues, controls) {
    var self = getItemModel(defaultValues, controls);

    return self;
}

function getPaItemModel(defaultValues, controls) {
    var self = getItemModel(defaultValues, controls);

    return self;
}
function getObserableChooiceItemModel(defaultValues, controls) {
    var self = getItemModel(defaultValues, controls);

    return self;
}
function getRapidExpressiveItemModel(defaultValues, controls) {
    var self = getItemModel(defaultValues, controls);

    return self;
}

function getReceptiveItemModel(defaultValues, controls) {
    var self = getItemModel(defaultValues, controls);
    self.OverallTimeOut = ko.observable(isNull("OverallTimeOut", defaultValues, true));

    self.validateAtMostOneCorrectAnswer = function (viewModel, event) {
        var answers = viewModel.Answers(), answer;
        if (!Ade_ItemType.getByKey(viewModel.Type).multiCorrectAnswer) {
            var correctAnswers = 0;
            for (var i = 0; i < viewModel.answerCount; i++) {
                answer = answers[i];
                if (answer.IsCorrect) {
                    correctAnswers++;
                }
            }
            if (correctAnswers > 1) {
                showMessage("hint", "AtMost_One_CorrectAnswer");
                return false;
            }
        }
        return true;
    };

    self.validateCorrectAnswerFilled = function (viewModel, event) {
        var answers = viewModel.Answers(), answer;
        if (viewModel.Type != Ade_ItemType.Direction.value &&
            viewModel.Type != Ade_ItemType.RapidExpressive.value &&
            viewModel.Type != Ade_ItemType.Pa.value &&
            viewModel.Type != Ade_ItemType.Cot.value &&
            viewModel.Type != Ade_ItemType.Cec.value) {
            var correctAnswerFilled = true;
            for (var i = 0; i < viewModel.answerCount; i++) {
                answer = answers[i];
                if (!answer.Picture && answer.IsCorrect) {
                    correctAnswerFilled = false;
                    break;
                }
            }
            if (!correctAnswerFilled) {
                showMessage("hint", "CorrectAnswer_Need_Picture");
                return false;
            }
        }
        return true;
    }

    self.validateCorrectAnswer = function (viewModel, event) {
        var answers = viewModel.Answers(), answer;
        if (Ade_ItemType.getByKey(viewModel.Type).needCorrectAnswer) {
            var hasCorrectAnswer = false;
            for (var i = 0; i < viewModel.answerCount; i++) {
                answer = answers[i];
                if (answer.Picture && answer.IsCorrect) {
                    hasCorrectAnswer = true;
                }
            }
            if (!hasCorrectAnswer) {
                showMessage("hint", "Item_Need_Correct_Answer");
                return false;
            }
        }
        return true;
    }

    self.submit = function (viewModel, event) {
        var $sender = $(event.target);
        var $form = $sender.closest("form");
        if (!self.formPrepared(viewModel, event)) {
            return false;
        }

        if ($form.valid() == false) {
            return false;
        }

        if (!self.validateAtMostOneCorrectAnswer(viewModel, event)) {
            return false;
        }

        if (!self.validateCorrectAnswerFilled(viewModel, event)) {
            return false;
        }

        if (!self.validateCorrectAnswer(viewModel, event)) {
            return false;
        }

        self.prepareAnswers();
        $form.find(controls.Answer).val(JSON.stringify(viewModel.Answers()));

        $form.submit();
        return false;
    }
    return self;
}

function getReceptivePromptItemModel(defaultValues, controls) {
    var self = getReceptiveItemModel(defaultValues, controls);

    self.getCustomUploaders = function (viewModel) {
        var uploaders = {
            count: 0,
            uploaders: []
        }, uploader;

        if ((uploader = viewModel.uploader("PromptPictureUploader"))) {
            uploaders.count += uploader.getStats().queueNum;
            uploaders.uploaders.push(viewModel.uploader("PromptPictureUploader"));
        }
        if ((uploader = viewModel.uploader("PromptAudioUploader"))) {
            uploaders.count += uploader.getStats().queueNum;
            uploaders.uploaders.push(viewModel.uploader("PromptAudioUploader"));
        }
        return uploaders;
    };

    return self;
}

function getMultipleChoicesItemModel(defaultValues, controls) {
    var self = getReceptiveItemModel(defaultValues, controls);

    var countTo8 = 8 - self.Answers().length;
    for (var i = 0; i < countTo8; i++) {
        self.Answers.push(getAnswerModel(self.defaultAnswerOption));
    }

    self.validateAtLeastOneAnswer = function () {
        var answers = self.Answers(), answer;
        if (self.Type == Ade_ItemType.MultipleChoices.value) {
            var emptyItems = 0;
            for (var i = 0; i < self.answerCount; i++) {
                answer = answers[i];
                if (!answer.Picture) {
                    answer.Picture = "";
                    answer.PictureTime = 0;
                    answer.Audio = "";
                    answer.AudioTime = 0;
                    answer.Score = 0;
                    answer.TextTime = 0;
                    emptyItems++;
                }
            }
            if (emptyItems == 8) {
                answers[0].hasError(true);
                showMessage("hint", "Mul_Choices_Required");
                return false;
            }
        }
        return true;
    }

    self.submit = function (viewModel, event) {
        var $sender = $(event.target);
        var $form = $sender.closest("form");
        if (!self.formPrepared(viewModel, event)) {
            return false;
        }

        if ($form.valid() == false) {
            return false;
        }
        if (!self.validateAtLeastOneAnswer()) {
            return false;
        }
        if (!self.validateCorrectAnswerFilled(viewModel, event)) {
            return false;
        }
        if (!self.validateCorrectAnswer(viewModel, event)) {
            return false;
        }

        self.prepareAnswers();
        $form.find(controls.Answer).val(JSON.stringify(viewModel.Answers()));

        $form.submit();
        return false;
    }

    return self;
}

function getQuadrantItemModel(defaultValues, controls) {
    var self = getReceptiveItemModel(defaultValues, controls);

    return self;
}


function getResponseOption(defaultValues) {
    function ResponseOption() { };

    var self = new ResponseOption();

    self.uuid = window.guid("Opinion");
    self.Id = isNull("Id", defaultValues, 0);

    self.Type = ko.observable(isNull("Type", defaultValues, {}).value || TypedResponseType.Text);

    self.Keyword = isNull("Keyword", defaultValues, "");
    self.From = isNull("From", defaultValues, "");
    self.To = isNull("To", defaultValues, "");
    self.Score = isNull("Score", defaultValues, "");
    self.ResponseId = isNull("ResponseId", defaultValues, 0);

    self.Type.subscribe(function (value) {
        if (value == TypedResponseType.Text) {
            self.From = 0;
            self.To = 0;
        } else {
            self.Keyword = "";
        }
    });

    self.IsDeleted = ko.observable(isNull("IsDeleted", defaultValues, false));
    return self;
}

function getResponse(defaultValues) {
    function Response() { };
    Response.prototype = getUploaderHelper();
    var self = new Response();

    self.uuid = window.guid("Response");
    self.Id = isNull("Id", defaultValues, 0);
    self.ItemId = isNull("ItemId", defaultValues, 0);
    self.Required = isNull("Required", defaultValues, true);

    self.Type = ko.observable((isNull("Type", defaultValues, {}).value || TypedResponseType.Text) + "");

    self.Length = isNull("Length", defaultValues, "");
    self.Text = isNull("Text", defaultValues, "");
    self.Picture = isNull("Picture", defaultValues, "");
    self.TextTimeIn = isNull("TextTimeIn", defaultValues, 0);
    self.PictureTimeIn = isNull("PictureTimeIn", defaultValues, 0);

    self.TypeText = ko.computed(function () {
        return TypedResponseType[this.Type()] + " (" + this.Length + ")";
    }, self);
    self.LengthText = ko.computed(function () {
        return this.Type() == TypedResponseType.Text ? "Max Characters" : "Number of decimals";
    }, self);

    self.Options = ko.observableArray([]);

    if (defaultValues && defaultValues.Options) {
        for (var i = 0; i < defaultValues.Options.length; i++) {
            self.Options.push(getResponseOption(defaultValues.Options[i]));
        }
    } else {
        var option = getResponseOption();
        option.Type(self.Type());
        self.Options.push(option);
    }

    self.newOption = function () {
        if (self.Options().length < 10) {
            var newOption = getResponseOption();
            newOption.Type(self.Type());
            self.Options.push(newOption);
        }
    };
    self.deleteOption = function (deleteOption) {
        deleteOption.IsDeleted(true);
        if (deleteOption.Id < 1) {
            self.Options.remove(deleteOption);
        }
        if (self.Options().length === 0) {
            self.newOption();
        }
    };

    self.Type.subscribe(function (type) {
        for (var i = 0; i < self.Options().length; i++) {
            self.Options()[i].Type(type);
        }
    });

    self.IsDeleted = ko.observable(isNull("IsDeleted", defaultValues, false));

    return self;
}

function getTypedResponseItemModel(defaultValues, controls) {
    var self = getItemModel(defaultValues, controls);
    var $form = $(controls.Answer).closest("form");
    self.Responses = ko.observableArray([]);;
    if (defaultValues && defaultValues.Responses && defaultValues.Responses.length) {
        for (var i = 0; i < defaultValues.Responses.length; i++) {
            var response = getResponse(defaultValues.Responses[i]);
            if (i === 0) {
            }
            self.Responses.push(response);
        }
    } else {
        self.Responses.push(getResponse());
    }
    self.activeResponses = ko.computed(function () {
        return self.Responses().filter(function (enumResponse) {
            return enumResponse.IsDeleted() === false;
        });
    }, self);

    self.newResponse = function () {
        if (self.activeResponses().length < 9) {
            var response = getResponse();
            self.Responses.push(response);

            if (self.responseCreated) {
                self.responseCreated(response, self.Responses().length - 1);
            }
        } else {
            var msg = window.getErrorMessage("ade_Typed_Response_Most_9_Responses").replace("{Items}", 9);
            window.showMessage("hint", msg);
        }
    };
    self.deleteResponse = function (deleteResponse, event) {
        var index = self.activeResponses().indexOf(deleteResponse);
        deleteResponse.IsDeleted(true);
        if (deleteResponse.Id < 1) {
            self.Responses.remove(deleteResponse);
        }
        if (self.activeResponses().length === 0) {
            self.newResponse();
        }
        if (self.responseDeleted) {
            self.responseDeleted(deleteResponse, index);
        }
    };

    self.getCustomUploaders = function (viewModel) {
        var uploaders = {
            count: 0,
            uploaders: []
        }, uploader;
        for (var j = 0; j < viewModel.Responses().length; j++) {
            var response = viewModel.Responses()[j];
            if ((uploader = response.uploader(response.uuid + "PictureUploader"))) {
                uploaders.count += uploader.getStats().queueNum;
                uploaders.uploaders.push(uploader);
            }
        }

        return uploaders;
    };

    self.onBeforeSubmit = function (viewModel) {
        $(controls.Responses).val(ko.toJSON(viewModel.Responses()));
    };

    return self;
}

function getKeaReceptiveItemModel(defaultValues, controls, answerCount) {
    var self = getReceptiveItemModel(defaultValues, controls);
    self.SelectionType = ko.observable(isNull("value", defaultValues.SelectionType, 1));
    self.IsStop = ko.observable(isNull("Stop", defaultValues, false));
    self.IsNext = ko.observable(isNull("NextButton", defaultValues, false));
    self.BreakCondition = ko.observable(isNull("value", defaultValues.BreakCondition, 2));
    self.IsCompleted = isNull("ItemLayout", defaultValues, false);
    self.branchingScores = ko.observableArray([]);
    self.oldBranchingScores = "";
    self.branchingItems = [];
    self.notNeedInitCanvas = ko.observable(false); //记录是否需要加载Canvas


    if (defaultValues.Answers.length == 0) {  //新增时，添加answers;编辑时，保持之前数据
        for (var i = 0; i < answerCount; i++) {
            self.Answers.push(getAnswerModel(self.defaultAnswerOption));
        }
    }

    self.submit = function (viewModel, event) {
        var $sender = $(event.target);
        var $form = $sender.closest("form");

        if (window.customValid && !customValid()) {
            return false;
        }
        //下一步时不判断答案数量
        if (!itemModel.isNext && window.ValidCorrectAnswer && !ValidCorrectAnswer())
            return false;

        if (!$form.valid()) {
            return false;
        }

        updateCkeditor();
        $form.find("#step").val(viewModel.step());

        var newImageCount = [];
        for (var i = 0; i < viewModel.ImageCount().length; i++) {
            var answer = viewModel.ImageCount()[i];
            var newAnswer = getAnswerModel(AnswerDefault.Others);
            newAnswer.ID = answer.ID;
            newAnswer.ItemId = answer.ItemId;
            newAnswer.Picture = answer.Picture();
            newAnswer.Audio = answer.Audio();
            newAnswer.ImageType = answer.ImageType();
            if (newAnswer.ImageType == 2)  //ImageType = Prompt
            {
                newAnswer.IsCorrect = false;
                newAnswer.Score = 0;
                newAnswer.SequenceNumber = 0;
            }
            else {
                newAnswer.IsCorrect = answer.IsCorrect();
                if (newAnswer.IsCorrect == true) {
                    newAnswer.Score = answer.Score;
                    newAnswer.SequenceNumber = answer.SequenceNumber;
                }
                else {
                    newAnswer.Score = 0;
                    newAnswer.SequenceNumber = 0;
                }
            }
            newAnswer.PictureTime = answer.PictureTime() ? answer.PictureTime() : 0;
            newAnswer.AudioTime = answer.AudioTime() ? answer.AudioTime() : 0;
            newAnswer.Text = answer.Text;
            newAnswer.TextTime = answer.TextTime;
            newAnswer.Value = answer.Value;
            newAnswer.Maps = answer.Maps;
            newAnswer.hasError = answer.hasError();
            newAnswer.ResponseAudio = answer.ResponseAudio();
            newImageCount.push(newAnswer);
        }

        $form.find(controls.Answer).val(JSON.stringify(newImageCount));
        GetLayouts("#ItemLayout", $("#BackgroundImage").val() ? $("#BackgroundImage").val() : jsonModel.LayoutBackgroundImage);
        $form.submit();
        return false;
    }

    self.previous = function (viewModel, event) {
        if (window.customValid && customValid()) {
            var curStep = viewModel.step();
            if (curStep >= 1) {
                viewModel.step(Number(curStep - 1));
                if (curStep == 4) //第3步时，设置隐藏的上传控件的宽和高
                {
                    $("div.webuploader-container div[style]").css({ width: '77px', height: '30px' });
                    $("#btnSubmit").data("clicked", false);
                    itemLayout = JSON.stringify(canvas.toDatalessJSON(new Array("id", "lockUniScaling", "hasRotatingPoint")));
                }

                if (curStep == 2) {
                    //chrome 浏览器中，当在第二步缩小浏览器时，编辑框宽度为0，需重新设置
                    if ($("iframe[class='cke_wysiwyg_frame cke_reset cke_focus']").length > 0 && $("iframe[class='cke_wysiwyg_frame cke_reset cke_focus']").width() == 0) {
                        $("iframe[class='cke_wysiwyg_frame cke_reset cke_focus']").width($("#InstructionText").width());
                    }
                }
            }
        }
        else {
            return false;
        }
    }

    self.step1 = function (viewModel, event) {
        viewModel.step(1);
    }

    self.next = function (viewModel, event) {

        var $sender = $(event.target);
        var $form = $sender.closest("form");
        viewModel.isNext = true;
        if (window.customValid && customValid()) {
            var curStep = viewModel.step();
            switch (curStep) {
                case 1:
                    if (checkStepOne() == false) {
                        viewModel.isNext = false;
                        viewModel.step(2);
                        return false;
                    }
                    break;
                case 2:
                    //第3步时，设置隐藏的上传控件的宽和高
                    $("div.webuploader-container div[style]").css({ width: '77px', height: '30px' });
                    if (checkStepTwo() == false) {
                        viewModel.isNext = false;
                        viewModel.step(3);
                        return false;
                    }
                    break;
                case 3:
                    if (checkStepThree() == false) {
                        viewModel.isNext = false;
                        CloseSize();
                        if (self.notNeedInitCanvas() == false) {
                            initChooseLayout();
                            InitPicture(jsonModel.ItemLayout, jsonModel.Answers, jsonModel.BackgroundImage ? getUploadUrl() + '/upload/' + jsonModel.BackgroundImage : "");
                            self.notNeedInitCanvas(true);
                        }
                        viewModel.step(4);
                        return false;
                    }
                    break;
            }
            event.target = $("#btnSubmit");
            self.submit(viewModel, event);
            return false;
        }
    }

    self.stepClick = function (viewModel, event, stepNum) {
        switch (stepNum) {
            case 1:
                viewModel.step(1);
                return false;
            case 2:
                viewModel.step(2);
                return false;
            case 3:
                //第3步时，设置隐藏的上传控件的宽和高
                $("div.webuploader-container div[style]").css({ width: '77px', height: '30px' });
                viewModel.step(3);
                return false;
            case 4:
                CloseSize();
                if (self.notNeedInitCanvas() == false) {
                    initChooseLayout();
                    InitPicture(jsonModel.ItemLayout, jsonModel.Answers, jsonModel.BackgroundImage ? getUploadUrl() + '/upload/' + jsonModel.BackgroundImage : "");
                    self.notNeedInitCanvas(true);
                }
                viewModel.step(4);
                return false;
        }
    }

    self.pushImageCount = function (defaultValues) {
        if (defaultValues) {
            self.ImageCount().length = 0;
            for (i = 0; i < defaultValues.length; i++) {
                self.ImageCount.push(getReceptiveAnswerModel(defaultValues[i]));
            }
        }
    };

    //begin branching score
    self.newBranchingScore = function () {
        var branchingIndex = 1;
        for (i = 0; i < self.branchingScores().length; i++) {
            if (self.branchingScores()[i].IsDeleted == false)
                branchingIndex++;
        }
        self.branchingScores.push(getBranchingScore(0, 0, 0, self.ID, 0, branchingIndex));
    }
    self.deleteBranchingScore = function (score) {
        if (score.ID == 0) {
            self.branchingScores.remove(score);
        }
        else {
            score.IsVisible(true);
            score.IsDeleted = true;
        }
        //让表单上绑定该属性的监视元素起作用
        self.branchingScores(self.branchingScores());
    }
    self.pushBranchingScore = function (defaultValues) {
        if (defaultValues) {
            self.branchingScores().length = 0;
            for (i = 0; i < defaultValues.length; i++) {
                self.branchingScores.push(getBranchingScore(defaultValues[i].ID, defaultValues[i].From, defaultValues[i].To, defaultValues[i].ItemId, defaultValues[i].SkipItemId, i + 1));
            }
        }
        self.oldBranchingScores = JSON.stringify(self.branchingScores());
        $("#BranchingScoreList").val(self.oldBranchingScores);
    }

    // end branching score

    return self;
}


function ParentMeasures(parents) {
    var parentMeasureExpanded = {};
    function visibilityManager(value) {
        var key = "_visibilityOfMeasure_";
        var result = {
            get: function (measureId) {
                if (this.hasOwnProperty(measureId)) {
                    return this[measureId];
                }
                return true;
            }
        };
        var url = location.pathname.toLowerCase();
        if (url == "/cpalls/student") {
            if (value) {
                localStorage.setItem(key, JSON.stringify(value));
                return;
            } else {
                $.extend(result, JSON.parse(localStorage.getItem(key)));
            }
        }
        return result;
    }
    var visible = visibilityManager() || {};
    $.each(parents, function (i, p) {
        parentMeasureExpanded[p.MeasureId] = {
            MeasureId: p.MeasureId,
            ParentId: p.ParentId,
            visible: ko.observable(visible.get(p.MeasureId)),
            _colspan: p.Subs,
            colspan: ko.observable(visible.get(p.MeasureId) ? p.Subs : 1),
            name: ko.observable(p.Name),
            selected: ko.observable(false),
            isParent: p.Subs > 0
        };
        parentMeasureExpanded[p.MeasureId].visible.subscribe(function (visible) {
            var visibilityOfMeasure = {};
            for (var measureId in parentMeasureExpanded) {
                if (+measureId > 0 && parentMeasureExpanded.hasOwnProperty(measureId)) {
                    visibilityOfMeasure[measureId] = parentMeasureExpanded[measureId].visible();
                }
            }
            visibilityManager(visibilityOfMeasure);
        });
    });
    return parentMeasureExpanded;
}


function getBenchmark(ID, LabelText, Color, BlackWhite, BlackWhiteText) {
    function AdeBenchmark() { };

    var model = {};
    model.constructor = AdeBenchmark;
    model.ID = ID || 0;
    model.LabelText = LabelText || '';
    model.Color = Color || '';
    model.BlackWhite = BlackWhite || 0;
    model.BlackWhiteText = BlackWhiteText || '';
    return model;
}

function getBenchmarkModel(jsonBenchmarks, formId, benchmarkControlId) {
    function AdeBenchmarkModel() { };

    var self = {};
    self.constructor = AdeBenchmarkModel;
    self.benchmarks = ko.observableArray([]);
    if (jsonBenchmarks && jsonBenchmarks.length) {
        for (var i = 0; i < jsonBenchmarks.length; i++) {
            var benchmark = getBenchmark(
                jsonBenchmarks[i].ID,
                jsonBenchmarks[i].LabelText, jsonBenchmarks[i].Color,
                jsonBenchmarks[i].BlackWhite.value);
            self.benchmarks.push(benchmark);
        }
    }
    self.$form = $("#" + formId);
    self.$benchmarks = self.$form.find("#" + benchmarkControlId);

    self.colorPick = function (index, color) {
        $("#BenchmarkColor" + index).colorpicker({
            color: color
        })
    };
    self.newBenchmark = function () {
        var newBenchmark = getBenchmark();
        newBenchmark.Color = "#c3c3c3";
        self.benchmarks.push(newBenchmark);
        self.colorPick(self.benchmarks().length, newBenchmark.Color);
    };
    self.deleteBenchmark = function (benchmark) {
        self.benchmarks.remove(benchmark);
        self.checkBenchmark();
    };
    self.checkBenchmark = function () {
        if (self.benchmarks().length == 0) {
            self.newBenchmark();
        }
    };
    self.checkBenchmark();
    self.submit = function () {
        if (uploadCover.getStats().queueNum > 0) {
            $("#btnSubmit").data("clicked", true);
            if (uploadCover.state == "ready") {
                uploadCover.upload();
            }
            return false;
        }
        self.$benchmarks.val(JSON.stringify(self.benchmarks()));
        //self.$form.submit();
        return false;
    }


    return self;
}
