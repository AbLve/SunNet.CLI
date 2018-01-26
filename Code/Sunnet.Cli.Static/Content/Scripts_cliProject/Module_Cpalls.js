


function getExecHelper() {
    var app = {};
    app.messages = {
        noName: "Unknown"
    };
    app.keys = {
        execCpalls: "ExecCpalls"
    };
    app.log = function () {
        if (window._DEBUG === true) {
            var params = [];
            for (var i = 0; i < arguments.length; i++) {
                params.push(arguments[i]);
            }
            console.log.apply(console, params);
        }
    };

    app.fillPath = function (fileName) {
        var fullPath;
        var prefix = window._staticDomain_.replace(/\/$/ig, "");
        if (fileName) {
            if (fileName.charAt(0) === "/" || fileName.indexOf(":") > 0) {
                fullPath = fileName;
            } else {
                fullPath = prefix + "/upload/" + fileName.toLowerCase();
            }
        }
        return fullPath;
    };
    app.audioQueue = [];
    app.audioPlayingQueue = [];
    app.stopAudio = function () {
        if (app.audioPlayingQueue.length > 0) {   //新增本身的audio不停止播放
            var originIndex = 0;//记录延迟执行前的index值
            for (var i = 0; i < app.audioPlayingQueue.length; i++) {
                app.audioPlayingQueue[i].volume = 0;//静音，以免影响下一题音频播放
            }
        }
    }

    app.getAudioPlayer = function (file, onLoad) {
        if (file) {
            var audio = new Audio();
            audio.addEventListener('ended', function () {
                var tmpIndex = 0;
                if ((tmpIndex = $.inArray(audio, app.audioPlayingQueue)) > -1)
                    app.audioPlayingQueue.splice($.inArray(audio, app.audioPlayingQueue), 1)
            }, false);
            audio.addEventListener('playing', function () {
                for (var i = 0; i < app.audioPlayingQueue.length; i++) {
                    app.audioPlayingQueue[i].currentTime = app.audioPlayingQueue[i].duration;
                }
                app.audioPlayingQueue.length = 0;
                app.audioPlayingQueue.push(audio);
            }, false);

            this.log("start: " + audio.src);
            if ($.support.mobile) {

                audio._src = this.fillPath(file);
                this.audioQueue.push(audio);
                app.log("iOS, trigger loaded to countinue");
                if (onLoad) onLoad();

            } else {
                audio.src = this.fillPath(file);
                audio.oncanplaythrough = function () {
                    app.log("loaded(oncanplaythrough): " + audio.src);
                    if (onLoad) onLoad();
                };
                audio.onerror = function () {
                    console.error("error during loading:", audio.src);
                    if (onLoad) onLoad();
                };
            }
            return audio;
        }
        return null;
    };

    app.getImage = function (file, onLoad) {
        if (file) {
            var img = new Image();
            img.src = this.fillPath(file);
            img.onload = function () {
                app.log("loaded: " + img.src);
                if (onLoad) onLoad();
            }
            img.onerror = function () {
                console.error("error during loading:", img.src);
                if (onLoad) onLoad();
            }
            this.log("start: " + img.src);
            return img;
        }
        return null;
    };
    app.getImgElement = function (file) {
        if (file) {
            var img = "<img generator='script' class='img' src='" + this.fillPath(file) + "' alt='image' />";
            return img;
        }
        return "";
    };
    app.getHtml = function (file, onLoad) {
        var targetUrl = file.toLowerCase().indexOf("http") >= 0 ? file : this.fillPath(file);
        $.get(targetUrl, onLoad, 'html');
    }

    app.getLocalStorage = function (key, valueIfNo) {
        var value = localStorage.getItem(key);
        if (value) {
            value = JSON.parse(value);
        } else {
            value = valueIfNo;
        }
        return value;
    };
    app.setLocalStorage = function (key, value) {
        if (typeof (value) == "object")
            value = JSON.stringify(value);
        localStorage.setItem(key, value);
    };
    app.clearLocalStorage = function (key) {
        if (key) {
            localStorage.removeItem(key);
        } else {
            localStorage.clear();
        }
    };

    app.requestFullscreen = function () {
        var de = document.documentElement;

        if (de.requestFullscreen) {
            de.requestFullscreen();
        } else if (de.msRequestFullscreen) {
            de.msRequestFullscreen();
        } else if (de.mozRequestFullScreen) {
            de.mozRequestFullScreen();
        } else if (de.webkitRequestFullScreen) {
            de.webkitRequestFullScreen();

        }
    };

    app.exitFullscreen = function () {
        var de = document;
        if (de.exitFullscreen) {
            de.exitFullscreen();
        } else if (de.msExitFullscreen) {
            de.msExitFullscreen();
        } else if (de.mozCancelFullScreen) {
            de.mozCancelFullScreen();
        } else if (de.webkitCancelFullScreen) {
            de.webkitCancelFullScreen();
        }
    };

    var isFullscreen = false;
    app.switchFullscreen = function () {

        if (isFullscreen) {
            isFullscreen = false;
            app.exitFullscreen();
        } else {
            isFullscreen = true;
            app.requestFullscreen();
        }
    };
    app.waitingImg = location.origin + "/Images/time.png";
    app.waitingHtml = "<img src='" + app.waitingImg + "' alt='Time' generator='static code' class='time' disabled='disabled' data-bind='clickBubble: false' alt='image'/>";

    //没有用到 ? begin
    app.audioProcessed = false;
    app.startProcessAudio = function () {
        this.log("startProcessAudio");
        var loaderEvent = new LoadEvents();
        if (this.audioQueue && this.audioQueue.length) {
            loaderEvent.waitting(this.audioQueue.length);
            var audio;
            while (audio = this.audioQueue.shift()) {
                +(function (cAudio) {
                    audio.defaultMuted = true;
                    audio.muted = true;
                    cAudio.src = cAudio._src;
                    app.log("start download: %s", cAudio.src);
                    cAudio.onloadedmetadata = function () {
                        app.log("onloadedmetadata: %s", cAudio.src);
                        cAudio.pause();
                    };
                    cAudio.oncanplaythrough = function () {
                        app.log("oncanplaythrough: %s", cAudio.src);
                        cAudio.pause();
                        cAudio.currentTime = 0;
                        cAudio.muted = false;

                        loaderEvent.loadedOne();
                        loaderEvent.loadCompleted();

                        cAudio.oncanplaythrough = null;
                    };
                    cAudio.play();
                })(audio);
            }
        } else {
            setTimeout(function () {
                loaderEvent.loadCompleted("No audios waitting");
            }, 10);
        }
        return loaderEvent.promise();
    };
    //没有用到 ？ end

    //兼容ie和firefox:为了解决在当前item暂停时，再次开始后该audio在这两个浏览器中不播放
    app.overrideAudioSrc = function (audio) {
        if (audio && audio.player) {
            audio.player.src = audio.player.src;
            if (audio.player.volume == 0) {
                audio.player.volume = 1;
            }
        }
    };

    return app;
}

var execHelper = getExecHelper();
var keyboard_event_code = {
    left: 37,
    right: 39
}
var Cpalls_AnswerType = {
    /// <summary>
    /// Cot
    /// </summary>
    None: 1,
    /// <summary>
    /// Rapid Expressive
    /// </summary>
    YesNo: 2,
    /// <summary>
    /// PA
    /// </summary>
    PaText: 3,
    /// <summary>
    /// Quadrant,Multiple choices,Receptive with promp,Receptive without prompt
    /// </summary>
    PictureAudio: 4,
    /// <summary>
    /// Cec,Checklist
    /// </summary>
    Cec: 5,
    /// <summary>
    /// TxkeaExpressiveItem
    /// </summary>
    TxkeaExpressive: 6
}

var Cpalls_Status = {
    /// <summary>
    /// 初始化:未开始
    /// </summary>
    Initialised: 1,
    1: "Initialised",

    /// <summary>
    /// 暂停
    /// </summary>
    Paused: 2,
    2: "Paused",

    /// <summary>
    /// 已执行完毕
    /// </summary>
    Finished: 3,
    3: "Finished",

    /// <summary>
    /// 锁定
    /// </summary>
    Locked: 4,
    4: "Locked"
};

var Cpalls_Order = {
    Sequenced: 2,
    Random: 1
};

var Cpalls_Measure_ShowType = {
    Sequenced: 2,
    List: 1
};
var Cpalls_Direction = {
    Portrait: 1,
    Landscape: 2
};

var Txkea_SelectionType = {
    SingleSelect: 1,
    MultiSelect: 2
}

var Txkea_Scoring = {
    AllorNone: 1,
    Partial: 2
}

var Txkea_BreakCondition = {
    StopCondition: 1,
    IncorrectResponse: 2
}

var Txkea_ImageSequence = {
    Fixed: 2,
    Random: 1
}

function LoadEvents() {
    var that = this;
    this.loadCount = 0;
    this.loaded = 0;

    this.waitingOne = function () {
        this.loadCount++;
    };

    this.waitting = function (count) {
        this.loadCount = this.loadCount + count;
    };

    this.loadedOne = function () {
        this.loaded++;
    }

    this.loadEvent = $.Deferred();
    this.loadCompleted = function (msg) {
        if (this.loadCount <= this.loaded) {
            execHelper.log(msg, "completed, total:", this.loadCount, ", loaded:" + this.loaded);
            this.loadEvent.resolve();
        } else {
            execHelper.log(msg, "pending, total:", this.loadCount, ", loaded:" + this.loaded);
        }
    }

    this.promise = function () {
        return this.loadEvent.promise();
    }
}

function getAnswerModel(defaultValues) {
    function AnswerModel() { };
    var model = new AnswerModel();
    var loader = new LoadEvents();
    model.prepare = false;

    model.id = isNull("ID", defaultValues, 0);

    model.loadCount = 0;
    model.audio = {
        file: isNull("Audio", defaultValues, ""),
        timeout: isNull("AudioTime", defaultValues, 0),
        player: null,
        timer: null
    };
    if (model.audio.file) {
        loader.waitingOne();
    }
    model.responseAudio = {
        file: isNull("ResponseAudio", defaultValues, ""),
        timeout: 0,
        player: null,
        timer: null
    };
    if (model.responseAudio.file) {
        loader.waitingOne();
    }
    model.picture = {
        file: isNull("Picture", defaultValues, ""),
        timeout: isNull("PictureTime", defaultValues, 0),
        fullImgUrl: "",
        img: null,
        timer: null
    };
    if (model.picture.file) {
        loader.waitingOne();
        model.picture.fullImgUrl = execHelper.fillPath(model.picture.file);
    }

    model.text = {
        text: isNull("Text", defaultValues, 0),
        timeout: isNull("TextTime", defaultValues, 0),
        timer: null
    }

    model.score = isNull("Score", defaultValues, 0);
    model.isCorrect = isNull("IsCorrect", defaultValues, false);
    model.imageType = isNull("ImageType", defaultValues, { value: 1 }).value;
    model.score = isNull("Score", defaultValues, 0);
    model.sequenceNumber = isNull("SequenceNumber", defaultValues, 0);

    model.prepared = function () {
        if (model.audio.file) {
            model.audio.player = execHelper.getAudioPlayer(model.audio.file, function () {
                loader.loadedOne();
                loader.loadCompleted("answer %s audio", model.id);
            });
        }
        if (model.responseAudio.file) {
            model.responseAudio.player = execHelper.getAudioPlayer(model.responseAudio.file, function () {
                loader.loadedOne();
                loader.loadCompleted("answer %s responseAudio", model.id);
            });
        }
        if (model.picture.file) {
            model.picture.img = execHelper.getImage(model.picture.file, function () {
                loader.loadedOne();
                loader.loadCompleted("answer %s picture", model.id);
            });
            model.picture.html = execHelper.getImgElement(model.picture.file);
        }
        setTimeout(function () {
            loader.loadCompleted("answer %s no resource", model.id);
        }, 0);
        return loader.promise();
    };

    model.choosed = ko.observable(false);

    return model;
}

function getItemModel(defaultValues) {
    function ItemModel() { };

    var model = new ItemModel();
    var loader = model.loader = new LoadEvents();
    model.prepare = false;
    model.timeoutTimer = null; //item之间的暂停控制

    model.startClickEvent = null;
    model.startClick = 0; //item 的前 0.5s 点击无效

    model.itemId = isNull("ItemId", defaultValues, 0);
    model.answerType = isNull("AnswerType", defaultValues, { text: "", value: 0 }).value;
    model.label = isNull("Label", defaultValues, execHelper.messages.noName);
    model.timed = isNull("Timed", defaultValues, false);
    model.timeout = 0;
    model.timeouted = false; //item 是否已经因超时而未做题
    model.waitTime = isNull("WaitTime", defaultValues, 0);
    model.pauseTime = isNull("PauseTime", defaultValues, 0);
    model.scored = isNull("Scored", defaultValues, false);
    model.score = isNull("Score", defaultValues, 0);
    model.description = isNull("Description", defaultValues, '') + '&nbsp;';

    model.goal = ko.observable(isNull("Goal", defaultValues, 0));
    model.status = ko.observable(isNull("Status", defaultValues, { value: Cpalls_Status.Initialised }).value);

    model.isCorrect = ko.observable(isNull("IsCorrect", defaultValues, false));
    model.selectedAnswers = ko.observableArray(isNull("SelectedAnswers", defaultValues, []));
    model.randomAnswer = window.isNull("RandomAnswer", defaultValues, false);
    model.isPractice = isNull("IsPractice", defaultValues, false);
    model.showAtTestResume = isNull("ShowAtTestResume", defaultValues, false);
    model.running = new IntervalTimer(100);
    model.unEndPictureCount = ko.observable(0);
    model.unEndAudioCount = ko.observable(0);
    //未播放完的audio数量和未显示的图片数量，该值为常量，用于后退重置时使用
    model.unEndPictureLength = 0;
    model.unEndAudioLength = 0;
    //在结果页中的索引
    model.resultIndex = isNull("ResultIndex", defaultValues, 0);

    model.executed = ko.observable(isNull("Executed", defaultValues, 0)); //表示是执行过的 Item, 因为有Item 跳转逻辑，有些Item 是不会执行的; 执行的为 True ，跳过的 Item 的为 False

    model.lastItemIndex = ko.observable(isNull("LastItemIndex", defaultValues, 0)); //标记上一个item 的index ，为 click:previousItem 服务

    model.type = isNull("Type", defaultValues, {
        text: "",
        value: 0
    }).value;

    model.answers = [];
    var answers = isNull("Answers", defaultValues, null);
    if (answers && answers.length) {
        var totalAnswers = answers.length;
        var orderProcessor = GetOrderProcessor(totalAnswers, model.randomAnswer ? Cpalls_Order.Random : Cpalls_Order.Sequenced);
        model.unEndAudioLength = answers.filter(function (obj) { return obj.Audio }).length;
        model.unEndAudioCount(model.unEndAudioLength);
        for (var i = 0; i < totalAnswers; i++) {
            var answer = getAnswerModel(answers[orderProcessor.next()]);
            if (model.selectedAnswers().indexOf(answer.id) >= 0) {
                answer.choosed(true);
            }
            model.answers.push(answer);
        }
    }

    model.newResource = function (url) { };

    var preparedIndex = 0;
    var answerLoaderEvents = $.Deferred();
    model.answersPrepared = function () {
        if (preparedIndex < model.answers.length) {
            jQuery.when(model.answers[preparedIndex].prepared()).done(function () {
                execHelper.log("item[" + model.label + "] answer " + preparedIndex, " prepared, total: ", model.answers.length);
                model.answers[preparedIndex].prepare = true;
                preparedIndex++;
                model.answersPrepared();
            });
        } else {
            execHelper.log("item[" + model.label + "] answers all prepared, total: ", model.answers.length);
            setTimeout(function () {
                answerLoaderEvents.resolve();
            }, 0);
        }
        return answerLoaderEvents.promise();
    };

    model.prepared = function () {
        return this.answersPrepared();
    };

    ItemModel.prototype.audioPrepared = function () {
        var model = this;
        $.when(model.answersPrepared()).done(function () {
            var waitingResource = false;
            if (model.target && model.target.audio) {
                waitingResource = true;
                model.target.player = execHelper.getAudioPlayer(model.target.audio, function () {
                    execHelper.log("item [" + model.label + "] target loaded.");
                    loader.loadedOne();
                    loader.loadCompleted("item [" + model.label + "] target");
                });
            }
            if (!waitingResource) {
                setTimeout(function () {
                    loader.loadCompleted("item [" + model.label + "] no resources");
                }, 0);
            } else {
                execHelper.log("start loading item [" + model.label + "] resouces.");
            }
        });
        return loader.promise();
    };

    ItemModel.prototype.promptPrepared = function () {
        var model = this;

        $.when(model.answersPrepared()).done(function () {
            var waitingResource = false;
            if (model.prompt && model.prompt.picture && model.prompt.picture.file) {
                waitingResource = true;
                model.prompt.picture.img = execHelper.getImage(model.prompt.picture.file, function () {
                    execHelper.log("item [" + model.label + "] prompt img loaded.");
                    loader.loadedOne();
                    loader.loadCompleted("item [" + model.label + "] prompt img");
                });
                model.prompt.picture.html = execHelper.getImgElement(model.prompt.picture.file);
            }
            if (model.prompt && model.prompt.audio && model.prompt.audio.file) {
                waitingResource = true;
                model.prompt.audio.player = execHelper.getAudioPlayer(model.prompt.audio.file, function () {
                    execHelper.log("item [" + model.label + "] prompt audio loaded.");
                    loader.loadedOne();
                    loader.loadCompleted("item [" + model.label + "] prompt audio");
                });
            }
            if (model.target && model.target.audio) {
                waitingResource = true;
                model.target.player = execHelper.getAudioPlayer(model.target.audio, function () {
                    execHelper.log("item [" + model.label + "] target loaded.");
                    loader.loadedOne();
                    loader.loadCompleted("item [" + model.label + "] target");
                });
            }
            if (model.instructionAudio && model.instructionAudio.file) {
                waitingResource = true;
                model.instructionAudio.player = execHelper.getAudioPlayer(model.instructionAudio.file, function () {
                    execHelper.log("item [" + model.label + "] instructionAudio loaded.");
                    loader.loadedOne();
                    loader.loadCompleted("item [" + model.label + "] instructionAudio");
                });
            }
            if (!waitingResource) {
                setTimeout(function () {
                    loader.loadCompleted("item [" + model.label + "] no resources");
                }, 0);
            } else {
                execHelper.log("start loading item [" + model.label + "] resouces.");
            }
        });
        return loader.promise();
    };

    model.isShowing = ko.observable(false);
    model.showed = ko.observable(false);
    model.isShowing.subscribe(function (visible) {
        model.showed(true);
        if (visible) {
            model.running.reset();
            model.running.start();
        } else {
            model.running.pause();
            //由于audio需要重新播放，此时要重置audio播放数量
            if (model.type == Ade_ItemType.TxkeaReceptive.value) {
                model.unEndAudioCount(model.unEndAudioLength);
                execHelper.stopAudio();
            }
            if (model.type == Ade_ItemType.TxkeaExpressive.value) {
                execHelper.stopAudio(); //将当前播放的audio强制设置为结束，以跳过后续执行
            }
        }
    });

    model.showInResultPage = ko.computed(function () {
        return model.scored && model.executed()
            && (model.showed() || model.pauseTime > 0 || model.type === Ade_ItemType.Checklist.value);
    }, model);

    model.getGoal = function () {
        return model.goal();
    };

    model.finished = function () {
        return true;
    };

    model.needDoAssessment = function () {
        return this.status() !== Cpalls_Status.Finished || (model.isPractice && model.showAtTestResume);
    }

    return model;
}

function getChecklistItemModel(defaultValues) {
    var model = getItemModel(defaultValues);

    var props = isNull("Props", defaultValues, {});

    model.target = {
        text: isNull("TargetText", props, "") + '&nbsp;'
    };
    model.multiChoice = isNull("IsMultiChoice", props, false);
    model.isRequired = isNull("IsRequired", props, false);
    model.maxAnswerCount = isNull("Response", props, 0);
    model.direction = isNull("Direction", props, { value: 0 }).value;

    return model;
}

function getPaItemModel(defaultValues) {
    var model = getItemModel(defaultValues);

    var props = isNull("Props", defaultValues, {});

    model.target = {
        text: isNull("TargetText", props, "") + '&nbsp;',
        textTimeout: isNull("TargetTextTimeout", props, 0),
        textTimer: null,
        audio: isNull("TargetAudio", props, 0),
        audioTimeout: isNull("TargetAudioTimeout", props, 0),
        player: null,
        audioTimer: null
    };
    model.multiChoice = isNull("IsMultiChoice", props, false);
    if (model.target.audio) {
        model.loader.waitingOne();
        model.prepared = model.audioPrepared;
    }
    return model;
}

function getDirectionItemModel(defaultValues) {
    var model = getItemModel(defaultValues);

    var props = isNull("Props", defaultValues, {});

    model.directionText = isNull("DirectionText", props, "");

    return model;
}

function getMultipleChoicesItemModel(defaultValues) {
    var model = getItemModel(defaultValues);
    var loader = model.loader;

    var props = isNull("Props", defaultValues, {});
    model.timeout = isNull("Timeout", props, 0);
    model.target = {
        text: isNull("TargetText", props, "") + '&nbsp;',
        textTimeout: isNull("TargetTextTimeout", props, 0),
        textTimer: null,
        audio: isNull("TargetAudio", props, 0),
        audioTimeout: isNull("TargetAudioTimeout", props, 0),
        player: null,
        audioTimer: null
    };
    if (model.target.audio) {
        loader.waitingOne();
        model.prepared = model.audioPrepared;
    }
    model.multiChoice = true;
    model.maxAnswerCount = isNull("Response", props, 0);

    return model;
}

function getQuadrantItemModel(defaultValues) {
    var model = getItemModel(defaultValues);

    var props = isNull("Props", defaultValues, {});
    model.timeout = isNull("Timeout", props, 0);
    model.target = {
        text: isNull("TargetText", props, "") + '&nbsp;',
        textTimeout: isNull("TargetTextTimeout", props, 0),
        textTimer: null,
        audio: isNull("TargetAudio", props, 0),
        audioTimeout: isNull("TargetAudioTimeout", props, 0),
        player: null,
        audioTimer: null
    };
    if (model.target.audio) {
        model.loader.waitingOne();
        model.prepared = model.audioPrepared;
    }
    return model;
}

function getRapidExpressiveItemModel(defaultValues) {
    var model = getItemModel(defaultValues);

    var props = isNull("Props", defaultValues, {});
    model.timeout = isNull("Timeout", props, 0);
    model.target = {
        text: isNull("TargetText", props, "") + '&nbsp;',
        textTimeout: isNull("TargetTextTimeout", props, 0),
        textTimer: null,
        audio: isNull("TargetAudio", props, 0),
        audioTimeout: isNull("TargetAudioTimeout", props, 0),
        player: null,
        audioTimer: null
    };
    if (model.target.audio) {
        model.loader.waitingOne();
        model.prepared = model.audioPrepared;
    }
    return model;
}

function getReceptiveItemModel(defaultValues) {
    var model = getItemModel(defaultValues);

    var props = isNull("Props", defaultValues, {});

    model.target = {
        text: isNull("TargetText", props, "") + '&nbsp;',
        textTimeout: isNull("TargetTextTimeout", props, 0),
        textTimer: null,
        audio: isNull("TargetAudio", props, 0),
        audioTimeout: isNull("TargetAudioTimeout", props, 0),
        player: null,
        audioTimer: null
    };
    if (model.target.audio) {
        model.loader.waitingOne();
        model.prepared = model.audioPrepared;
    }
    return model;
}

function getReceptivePromptItemModel(defaultValues) {
    var model = getItemModel(defaultValues);

    var props = isNull("Props", defaultValues, {});

    model.target = {
        text: isNull("TargetText", props, "") + '&nbsp;',
        textTimeout: isNull("TargetTextTimeout", props, 0),
        textTimer: null,
        audio: isNull("TargetAudio", props, 0),
        audioTimeout: isNull("TargetAudioTimeout", props, 0),
        player: null,
        audioTimer: null
    };
    if (model.target.audio) {
        model.loader.waitingOne();
    }
    model.prompt = {
        text: {
            text: isNull("PromptText", props, ""),
            timeout: isNull("PromptTextTimeout", props, 0),
            timer: null
        },
        picture: {
            file: isNull("PromptPicture", props, ""),
            timeout: isNull("PromptPictureTimeout", props, 0),
            img: null,
            fullImgUrl: "",
            timer: null
        },
        audio: {
            file: isNull("PromptAudio", props, ""),
            timeout: isNull("PromptAudioTimeout", props, 0),
            player: null,
            timer: null
        }
    };
    model.prepared = model.promptPrepared;

    if (model.target.audio) {
        model.loader.waitingOne();
    }
    if (model.prompt.picture && model.prompt.picture.file) {
        model.loader.waitingOne();
        model.prompt.picture.fullImgUrl = execHelper.fillPath(model.prompt.picture.file);
    }
    if (model.prompt.audio && model.prompt.audio.audio) {
        model.loader.waitingOne();
    }
    return model;
}

function getResponseOption(defaultValues) {
    function ResponseOption() { };

    var model = new ResponseOption();

    model.uuid = window.guid("Opinion");
    model.id = isNull("Id", defaultValues, 0);

    model.type = isNull("Type", defaultValues, {}).value;

    model.keyword = isNull("Keyword", defaultValues, "");
    //var words = model.keyword.split(/;|,/ig);
    // 只会有一个单词或词组，不会有逗号与分号分隔了
    var words = model.keyword;
    model.from = isNull("From", defaultValues, "");
    model.to = isNull("To", defaultValues, "");
    model.score = isNull("Score", defaultValues, "");
    model.responseId = isNull("ResponseId", defaultValues, 0);

    model.match = function (content) {
        var match, i;
        var userInputs = content.split(/;|,/ig);
        if (model.type === TypedResponseType.Text) {
            match = 0;
            for (i = 0; i < userInputs.length; i++) {
                if (words.trim().toUpperCase() === userInputs[i].trim().toUpperCase()) {
                    match = 1;
                    break;
                }
            }
            if (match > 0) {
                return model.score;
            }
        } else if (model.type === TypedResponseType.Numerical) {
            var input = +content;
            if (input > 0) {
                match = model.from <= input && input <= model.to;
            }
            if (match) {
                return model.score;
            }
        }
        return 0;
    };

    return model;
}

function getResponse(defaultValues) {
    function Response() { };
    var model = new Response();

    model.uuid = window.guid("Response");
    model.id = isNull("Id", defaultValues, 0);
    model.itemId = isNull("ItemId", defaultValues, 0);
    model.required = isNull("Required", defaultValues, false);
    model.alerting = ko.observable(false);

    model.type = isNull("Type", defaultValues, {}).value;

    model.length = isNull("Length", defaultValues, "");
    model.text = isNull("Text", defaultValues, "");
    model.picture = isNull("Picture", defaultValues, "");
    model.textTimeIn = isNull("TextTimeIn", defaultValues, 0);
    model.pictureTimeIn = isNull("PictureTimeIn", defaultValues, 0);

    model.typeText = TypedResponseType[this.type] + " (" + this.Length + ")";

    model.lengthText = this.type == TypedResponseType.Text ? "Max Characters" : "Number of decimals";


    model.options = [];

    if (defaultValues && defaultValues.Options) {
        for (var i = 0; i < defaultValues.Options.length; i++) {
            model.options.push(getResponseOption(defaultValues.Options[i]));
        }
    } else {
        console.error("Options is null");
    }
    var loader = new LoadEvents();
    if (model.picture) {
        loader.waitingOne();
    }

    model.prepared = function () {
        if (model.picture) {
            model.img = execHelper.getImage(model.picture, function () {
                loader.loadedOne();
                loader.loadCompleted("response picture");
            });
            model.html = execHelper.getImgElement(model.picture);
        } else {
            setTimeout(function () {
                loader.loadCompleted("response no picture");
            }, 0);
        }
        return loader.promise();
    };

    model.content = ko.observable("");
    model.isCorrect = ko.observable(false);
    model.content.subscribe(function (newVal) {
        if (newVal) {
            model.alerting(false);
        } else {
            model.alerting(true);
        }
    });
    var _goal = -1;
    model.getGoal = function () {
        if (_goal > -1) {
            return _goal;
        }
        var goal = 0, matchScore;
        if (model.options && model.options.length) {
            $.each(model.options, function (index, opinion) {
                if (!model.content().length) {
                    return;
                }
                matchScore = opinion.match(model.content());
                if (matchScore) {
                    model.isCorrect(true);
                    if (opinion.type === TypedResponseType.Text) {
                        goal = goal + matchScore;
                    }
                    else if (opinion.type === TypedResponseType.Numerical) {
                        if (matchScore > goal) {
                            goal = matchScore;
                        }
                    }
                }
            });
        }
        _goal = goal;
        return goal;
    };

    return model;
}

function getEditResopnse(defaultValues, details) {
    var item = getResponse(defaultValues);
    if (details) {
        if (typeof (details) === "string") {
            details = JSON.parse(details);
        }
        if (details[item.id]) {
            item.content(details[item.id].Content);
            item.isCorrect(details[item.id].IsCorrect);
        }
    }

    return item;
}


function getTypedResponseItemModel(defaultValues) {
    var model = getItemModel(defaultValues);
    var loader = model.loader;

    var props = isNull("Props", defaultValues, {});
    model.timeout = isNull("Timeout", props, 0);

    model.target = {
        text: isNull("TargetText", props, "") + '&nbsp;',
        textTimeout: isNull("TargetTextTimeout", props, 0),
        textTimer: null,
        audio: isNull("TargetAudio", props, 0),
        audioTimeout: isNull("TargetAudioTimeout", props, 0),
        player: null,
        audioTimer: null
    };
    if (model.target.audio) {
        model.loader.waitingOne();
    }

    var details = window.isNull("Details", defaultValues, "{}");
    if (details) {
        details = JSON.parse(details);
    }
    model.responses = [];
    if (props && props.Responses && props.Responses.length) {
        for (var i = 0; i < props.Responses.length; i++) {
            var response = getEditResopnse(props.Responses[i], details);
            model.responses.push(response);
        }
    } else {
        console.error("Responses is null");
    }
    model.showingResponse = ko.observable(false);
    model.responseTemplate = "_cpalls_Item_" + model.type + "_Response_" + model.responses.length;
    model.responseResultTemplate = "_cpalls_ItemResult_" + model.type + "_Response_" + model.responses.length;

    var preparedResponseIndex = 0;
    var responseLoaderEvents = $.Deferred();
    model.responsesPrepared = function () {
        if (preparedResponseIndex < model.responses.length) {
            jQuery.when(model.responses[preparedResponseIndex].prepared()).done(function () {
                execHelper.log("item[" + model.label + "] response " + preparedResponseIndex, " prepared.");
                model.responses[preparedResponseIndex].prepare = true;
                preparedResponseIndex++;
                model.responsesPrepared();
            });
        } else {
            setTimeout(function () {
                loader.loadCompleted("item [" + model.label + "] no responses");
                responseLoaderEvents.resolve();
            }, 0);
        }
        return responseLoaderEvents.promise();
    };

    var loaderEvents = $.Deferred();
    model.prepared = function () {
        $.when(model.audioPrepared(), model.responsesPrepared()).done(function () {
            loaderEvents.resolve();
        });
        return loaderEvents.promise();
    };

    model.getGoal = function () {
        model.goal(0);
        $.each(model.responses, function (index, res) {
            model.goal(model.goal() + res.getGoal());
        });
        model.isCorrect(model.goal() > 0);
        return model.goal();
    };

    model.finished = function () {
        var unFinished = model.responses.filter(function (res) {
            var unfinished = res.required && !res.content();
            if (unfinished) {
                res.alerting(true);
            }
            return unfinished;
        });
        if (unFinished.length) {
            model.showingResponse(true);
        }
        return unFinished.length <= 0;
    };
    return model;
}

function getTxKeaResponseOption(defaultValues) {
    function ResponseOption() { };
    var model = new ResponseOption();
    model.ID = isNull("ID", defaultValues, 0);
    model.ResponseId = isNull("ResponseId", defaultValues, 0);
    model.IsCorrect = ko.observable(false);
    model.Lable = isNull("Lable", defaultValues, "");
    model.AddTextbox = isNull("AddTextbox", defaultValues, false);
    model.otherText = ko.observable("");
    model.Score = isNull("Score", defaultValues, 0);

    model.changeRadio = function (p, d, c) {
        if (p.Type == 3) { //type="radio"
            for (var i = 0; i < p.Options().length; i++) {
                p.Options()[i].IsCorrect(false);
            }
            d.IsCorrect(true);
            p.content(d.ID);
            return true;
        }
        else if (p.Type == 4) { //checkbox
            var tmpContent = "";
            for (var i = 0; i < p.Options().length; i++) {
                if (p.Options()[i].IsCorrect()) {
                    tmpContent += p.Options()[i].ID + ",";
                }
            }
            p.content(tmpContent);
            return true;
        }
    };

    return model;

}

function getTxKeaResponse(defaultValues) {
    function Response() { };

    var model = new Response();

    model.ID = isNull("ID", defaultValues, 0);
    model.ItemId = isNull("ItemId", defaultValues, 0);
    model.Text = isNull("Text", defaultValues, "");
    model.required = isNull("Mandatory", defaultValues, true);
    model.content = ko.observable("");
    model.Type = isNull("Type", defaultValues, {}).value;
    model.Options = ko.observableArray([]);
    model.alerting = ko.observable(false);

    model.content.subscribe(function (newVal) {
        if (model.required) {
            if (newVal) {
                model.alerting(false);
            } else {
                model.alerting(true);
            }
        }
    });


    if (defaultValues && defaultValues.Options) {
        for (var i = 0; i < defaultValues.Options.length; i++) {
            model.Options.push(getTxKeaResponseOption(defaultValues.Options[i]));
        }
    }
    return model;
}


function getTxkeaExpressiveItemModel(defaultValues) {
    var model = getItemModel(defaultValues);

    var props = isNull("Props", defaultValues, {});

    model.cpallsItemLayout = props.CpallsItemLayout;
    model.screenWidth = props.ScreenWidth;
    model.screenHeight = props.ScreenHeight;
    model.instructionText = props.InstructionText;
    model.overallTimeOut = props.OverallTimeOut;
    model.timeout = props.TimeoutValue;
    model.responseType = props.ResponseType ? props.ResponseType.value : 1;

    model.answerType = model.responseType == 1 ? Cpalls_AnswerType.YesNo : Cpalls_AnswerType.TxkeaExpressive;

    model.ShowResponse = ko.observable(false);

    model.responses = ko.observableArray([]);

    model.branchingScores = props.BranchingScores;

    var details = window.isNull("Details", defaultValues, "{}");
    if (details) {
        details = JSON.parse(details);
    }
    model.answers = [];
    var answers = props.ImageList;
    if (answers && answers.length) {
        var totalAnswers = answers.length;
        var orderProcessor = GetOrderProcessor(totalAnswers, model.randomAnswer ? Cpalls_Order.Random : Cpalls_Order.Sequenced);
        for (var i = 0; i < totalAnswers; i++) {
            var answer = getAnswerModel(answers[orderProcessor.next()]);
            model.answers.push(answer);
        }
    }
    if (props && props.Responses) {
        for (var i = 0; i < props.Responses.length; i++) {
            var tmpResponse = getTxKeaResponse(props.Responses[i])
            if (details && details[props.Responses[i].ID]) {
                if (tmpResponse.Type == 1) {//text
                    if (details[props.Responses[i].ID][0] && details[props.Responses[i].ID][0].othertxt)
                        tmpResponse.content(details[props.Responses[i].ID][0].othertxt);
                }
                else {
                    var tmpOptionIds = new Array();
                    for (var j = 0; j < details[props.Responses[i].ID].length; j++) {
                        for (var k = 0; k < tmpResponse.Options().length; k++) {
                            if (details[props.Responses[i].ID][j].optionId == tmpResponse.Options()[k].ID) {
                                tmpResponse.Options()[k].IsCorrect(true);
                                tmpResponse.Options()[k].otherText(details[props.Responses[i].ID][j].othertxt);
                                tmpOptionIds.push(tmpResponse.Options()[k].ID);
                                break;
                            }
                        }
                    }
                    tmpResponse.content(tmpOptionIds.join(","));
                }
            }
            model.responses.push(tmpResponse);
        }
    }

    model.itemLayout = {
        background: '',
        backgroundImage: '',
        ImageList: new Array()
    };

    model.responseBackground = {
        background: props.ResponseBackgroundFill,
        backgroundImage: execHelper.fillPath(props.ResponseBackgroundImage)
    };

    model.instructionPosition = {
        top: 80,
        left: 1
    };

    model.instructionAudio = {
        file: props.InstructionAudio,
        timeout: props.InstructionAudioTimeDelay,
        timer: null
    };

    if (model.cpallsItemLayout) {
        var jsonLayout = JSON.parse(model.cpallsItemLayout);
        if (jsonLayout.backgroundImage) {
            model.itemLayout["backgroundImage"] = jsonLayout.backgroundImage;
            //转换路径，以在offline时使用
            var bgImg = model.itemLayout["backgroundImage"].src;
            if (bgImg && bgImg.indexOf("upload/") > 0) {
                model.itemLayout["backgroundImage"].src = execHelper.fillPath(bgImg.substring(bgImg.indexOf('upload/') + 7));
            }
        }
        else {
            if (jsonLayout.background)
                model.itemLayout["background"] = jsonLayout.background;
        }
        if (jsonLayout.objects && jsonLayout.objects.length) {
            var instructionObj = jsonLayout.objects.filter(function (obj) {
                return obj.id == 0
            })[0];
            if (instructionObj) {
                model.instructionPosition["top"] = ((instructionObj.top / model.screenHeight) * 100);//instructionObj.top > 30 ? instructionObj.top + 70 : 80;
                model.instructionPosition["left"] = ((instructionObj.left) / model.screenWidth) * 100;
            }
            jsonLayout.objects = jsonLayout.objects.filter(function (obj) {
                return obj.id > 0
            });//type==image
            for (var i = 0; i < jsonLayout.objects.length; i++) {
                var object = jsonLayout.objects[i];
                var answer = model.answers.filter(function (obj) {
                    return obj.id == object.id
                })[0];
                if (answer) {
                    var newWidth;
                    var newHeight;
                    var newTop = (object.top / model.screenHeight) * 100;// object.top / model.screenHeight >= 0.88 ? 88 : (object.top / model.screenHeight) * 100;
                    var newLeft = (object.left / model.screenWidth) * 100;//object.left / model.screenWidth >= 0.88 ? 88 : (object.left / model.screenWidth) * 100;
                    var rate = object.width / model.screenWidth * 100;
                    if (rate == 0)//计算出错的情况下读取之前的比例
                        rate = 1;
                    //object.left / model.screenWidth >= 0.88 ? 88 : (object.left / model.screenWidth) * 100;
                    model.itemLayout["ImageList"].push({
                        "id": object.id,
                        "src": answer.picture.fullImgUrl,
                        "top": newTop,//object.top + 70,
                        "left": newLeft,
                        "width": (object.width / model.screenWidth * 100),
                        "height": (object.height / model.screenHeight * 100),
                        "timeout": answer.picture.timeout,
                        "choosed": ko.observable(false),
                        "notDelay": answer.picture.timeout == 0 && i == 0
                        //第一个显示且没有延迟，则不需要延迟
                    });
                }
            }
        }
    }

    model.txkeaSwitch = function () {
        if (model.ShowResponse())
            model.ShowResponse(false);
        else
            model.ShowResponse(true);
    }

    model.finished = function () {
        if (model.answerType == Cpalls_AnswerType.YesNo) return true;
        var unFinished = model.responses().filter(function (res) {
            var unfinished = res.required && !res.content();
            if (unfinished) {
                res.alerting(true);
            }
            return unfinished;
        });

        return unFinished.length <= 0;
    };

    model.getGoal = function () {
        model.goal(0);
        if (model.answerType == Cpalls_AnswerType.YesNo) {
            if (model.isCorrect())
                model.goal(1);
            return model.goal();
        }

        if (model.responses && model.responses()[0].Type != 1) { //只有第一个response 计分，并且不是 text类型的
            if (model.responses()[0].Type == 3) { //radio
                for (var i = 0; i < model.responses()[0].Options().length; i++) {
                    if (model.responses()[0].Options()[i].IsCorrect()) {
                        model.goal(model.responses()[0].Options()[i].Score);
                        break;
                    }
                }
            } else { //checkbox
                for (var i = 0; i < model.responses()[0].Options().length; i++) {
                    if (model.responses()[0].Options()[i].IsCorrect()) {
                        model.goal(model.responses()[0].Options()[i].Score + model.goal());
                    }
                }
            }
        }
        model.isCorrect(model.goal() > 0);
        return model.goal();
    };

    model.prepared = model.promptPrepared;

    model.showItem = function () {
        if (model.isShowing()) { //跳入上一个或者下一个时，不继续执行
            if (model.answers && model.answers.length) {
                model.showPicture(0);
            }
            else {
                model.showAudio();
            }
        }
    }

    model.ResetQuestion = function () {
        model.ShowResponse(false);
        //model.executed(false);
        model.isShowing(false);
        execHelper.stopAudio();
        execModel.resetItem();
        model.timeoutTimer = null;
        $("#divContent").height($("#divContent").width() * (565 / 1024));
        if ($("#divResponseContent").length != 0) {
            $("#divResponseContent").height($("#divContent").height());
        }
        execModel.status(Exec_Status.going);
        execModel.currentItemIndex.notifySubscribers();
        if (model.needDoAssessment()) {
            model.isShowing(true);
            execModel.switchTimer();
            $("#divContent").height($("#divContent").width() * (565 / 1024));
            if ($("#divResponseContent").length != 0) {
                $("#divResponseContent").height($("#divContent").height());
            }

        }
    }
    model.showPicture = function (index) {
        if (model.isShowing()) { //跳入上一个或者下一个时，不继续执行
            if (index < model.answers.length) {
                var targetAnswer = model.answers[index];
                if (targetAnswer.picture) {
                    targetAnswer.picture.timer = new TimeoutTimer(+(targetAnswer.picture.timeout));
                    $.when(targetAnswer.picture.timer.timeUp()).done(function () {
                        $("#img_txkea_" + targetAnswer.id).show();
                        model.showAudio(index);
                    });
                    targetAnswer.picture.timer.start();
                }
                else {
                    $("#img_txkea_" + targetAnswer.id).show();
                    model.showAudio(index);
                }
            } //answer显示完成，播放Instruction Audio
            else {
                if (model.instructionAudio && model.instructionAudio.file && model.instructionAudio.player) {
                    if (+(model.instructionAudio.timeout) > 0) {
                        model.instructionAudio.timer = new TimeoutTimer(+(model.instructionAudio.timeout));
                        $.when(model.instructionAudio.timer.timeUp()).done(function () {
                            if (model.isShowing()) {
                                execHelper.overrideAudioSrc(model.instructionAudio);
                                model.instructionAudio.player.play();
                            }
                        });
                        model.instructionAudio.timer.start();
                    } else {
                        execHelper.overrideAudioSrc(model.instructionAudio);
                        model.instructionAudio.player.play();
                    }
                }
            }
        }
    }

    model.showAudio = function (index) {
        if (model.isShowing()) { //跳入上一个或者下一个时，不继续执行
            var targetAnswer = model.answers[index];
            if (targetAnswer.audio && targetAnswer.audio.file && targetAnswer.audio.player) {
                targetAnswer.audio.player.onended = function () {
                    if (model.isShowing()) {  //当点击暂停时，不继续执行
                        model.showPicture(index + 1);
                    }
                };
                if (+(targetAnswer.audio.timeout) > 0) {
                    targetAnswer.audio.timer = new TimeoutTimer(+(targetAnswer.audio.timeout));
                    $.when(targetAnswer.audio.timer.timeUp()).done(function () {
                        execHelper.overrideAudioSrc(targetAnswer.audio);
                        targetAnswer.audio.player.play();
                    });
                    targetAnswer.audio.timer.start();
                } else {
                    execHelper.overrideAudioSrc(targetAnswer.audio);
                    targetAnswer.audio.player.play();
                }
            }
            else {
                model.showPicture(index + 1);
            }
        }
    }

    return model;
}



function getTxkeaReceptiveItemModel(defaultValues) {
    var model = getItemModel(defaultValues);
    var props = isNull("Props", defaultValues, {});
    model.isWaitingNext = false;//是否在等待跳到下一个item
    model.isCanNext = true;//是否可以进入下一题
    model.cpallsItemLayout = props.CpallsItemLayout;
    model.screenWidth = props.ScreenWidth;
    model.screenHeight = props.ScreenHeight;
    model.instructionText = props.InstructionText;
    model.timed = props.OverallTimeOut;
    model.timeout = props.TimeoutValue;
    model.breakCondition = props.BreakCondition;
    model.stopConditionX = props.StopConditionX;
    model.stopConditionY = props.StopConditionY;
    model.grayedOutDelay = props.GrayedOutDelay;
    model.branchingScores = props.BranchingScores;
    model.syncAnswer = true;
    model.scoring = props.Scoring ? props.Scoring.value : 0;
    model.selectionType = props.SelectionType ? props.SelectionType.value : 0;
    model.imageSequence = props.ImageSequence ? props.ImageSequence.value : Txkea_ImageSequence.Fixed;
    model.correctAnswerCount = model.answers.filter(function (obj) {
        return obj.isCorrect == true
    }).length;

    if (model.selectionType == Txkea_SelectionType.MultiSelect) {  //multi select
        model.multiChoice = true;
        //未选择时值为[]
        var detailString = isNull("Details", defaultValues, "");
        model.selectedDetailedAnswers = detailString == null ? [] : detailString.length > 2 ? JSON.parse(detailString) : [];
        //将图片地址属性添加到selectedDetailedAnswers中，用于结果页中的显示
        if (model.selectedDetailedAnswers.length > 0) {
            for (var i = 0; i < model.selectedDetailedAnswers.length; i++) {
                var detailedAnswer = model.selectedDetailedAnswers[i];
                var targetAnswer = model.answers.filter(function (obj) {
                    return obj.id == detailedAnswer.id
                })[0];
                if (targetAnswer && targetAnswer.picture && targetAnswer.picture.fullImgUrl) {
                    detailedAnswer.src = targetAnswer.picture.fullImgUrl;
                }
            }
        }
    }

    if (model.grayedOutDelay) {
        model.canChoose = ko.computed(function () { return model.unEndAudioCount() == 0 && model.unEndPictureCount() <= 0; });
    }
    else {
        model.canChoose = ko.observable(true);
    }
    //IE浏览器的filter属性值和别的浏览器不相同
    model.isIE = !!window.ActiveXObject || "ActiveXObject" in window;

    model.getGoal = function () {
        model.goal(0);
        model.isCorrect(false);
        if (model.selectedAnswers().length > 0) {
            if (model.selectionType == Txkea_SelectionType.SingleSelect) {  //答案为单选时，若选择正确则计分
                var selectedAnswer = model.answers.filter(function (obj) {
                    return obj.choosed() == true;
                })[0];
                if (selectedAnswer && selectedAnswer.isCorrect) {
                    model.goal(selectedAnswer.score);
                }
            }
            if (model.selectionType == Txkea_SelectionType.MultiSelect) {  //答案为多选时
                if (model.scoring == Txkea_Scoring.AllorNone) { //整体计分
                    //选中的正确答案
                    var selectedCorrectAnswers = model.selectedDetailedAnswers.filter(
                        function (obj) { return obj.isRealCorrect == true });
                    //所有正确答案
                    var correctAnswers = model.answers.filter(function (obj) {
                        return obj.isCorrect == true
                    });
                    if ((model.selectedDetailedAnswers.length == selectedCorrectAnswers.length)
                        && (selectedCorrectAnswers.length == correctAnswers.length)) { //全部答对
                        var totalScore = 0;
                        for (var i = 0; i < correctAnswers.length; i++) {
                            totalScore += correctAnswers[i].score;
                        }
                        // model.goal(model.score);
                        model.goal(totalScore);
                    }
                }
                if (model.scoring == Txkea_Scoring.Partial) {  //单独计分
                    $.each(model.selectedDetailedAnswers, function (j, answer) {
                        if (answer.isRealCorrect == true) {
                            var selectedAnswer = model.answers.filter(function (obj) {
                                return obj.id == answer.id
                            })[0];
                            model.goal(model.goal() + selectedAnswer.score);
                        }
                    });
                }
            }
            //分数大于0就记为正确
            if (model.goal() > 0)
                model.isCorrect(true);
        }
        else {
            model.goal(0);
        }
        return model.goal();

    }

    model.itemLayout = {
        background: '',
        backgroundImage: '',
        ImageList: new Array()
    };

    model.instructionPosition = {
        top: 80,
        left: 1  //百分比值
    };

    model.target = {
        text: model.instructionText + '&nbsp;'
    };

    model.instructionAudio = {
        file: props.InstructionAudio,
        timeout: 0,
        timer: null
    };

    if (model.instructionAudio.file) {
        model.unEndAudioLength += 1;
        model.unEndAudioCount(model.unEndAudioLength);
    }

    if (model.cpallsItemLayout) {
        var jsonLayout = JSON.parse(model.cpallsItemLayout);
        if (jsonLayout.backgroundImage) {
            model.itemLayout["backgroundImage"] = jsonLayout.backgroundImage;
            //转换路径，以在offline时使用
            var bgImg = model.itemLayout["backgroundImage"].src;
            if (bgImg && bgImg.indexOf("upload/") > 0) {
                model.itemLayout["backgroundImage"].src = execHelper.fillPath(bgImg.substring(bgImg.indexOf('upload/') + 7));
            }
        }
        else {
            if (jsonLayout.background)
                model.itemLayout["background"] = jsonLayout.background;
        }
        if (jsonLayout.objects && jsonLayout.objects.length) {
            var instructionObj = jsonLayout.objects.filter(function (obj) {
                return obj.id == 0
            })[0];
            if (instructionObj) {
                model.instructionPosition["top"] = ((instructionObj.top / model.screenHeight) * 100);
                //    model.instructionPosition["top"] = (instructionObj.top / model.screenHeight) * 100;//instructionObj.top > 30 ? instructionObj.top + 70 : 80;
                model.instructionPosition["left"] = ((instructionObj.left) / model.screenWidth) * 100;


                //model.instructionPosition["top"] = instructionObj.top > 30 ? instructionObj.top + 70 : 80;
                //model.instructionPosition["left"] = ((instructionObj.left + 30) / model.screenWidth) * 100;
                //该样式左右添加了30px的间距值
            }
            jsonLayout.objects = jsonLayout.objects.filter(function (obj) {
                return obj.id > 0;
            });//type==image

            for (var i = 0; i < jsonLayout.objects.length; i++) {
                var object = jsonLayout.objects[i];
                var answer = model.answers.filter(function (obj) {
                    return obj.id == object.id;
                })[0];
                if (!answer) {   //ItemLayout数据不对
                    model.itemLayout["ImageList"] = [];
                    break;
                }
                var randomAnswer = model.answers[i];
                var newLeft;
                var newTop;
                var newWidth;
                var newHeight;
                var rate;
                var newTimeout;
                var realIndex;//记录显示时的真实索引
                var audioTimeOut;
                if (randomAnswer.id == object.id) {  //顺序相同时，位置不重置
                    rate = 1;
                    //alert("width:" + document.body.clientWidth + " Height:" + document.body.clientHeight);
                    var widthRate = document.body.clientWidth / model.screenWidth;

                    rate = object.width / model.screenWidth * 100;
                    if (rate == 0)//计算出错的情况下读取之前的比例
                        rate = 1;
                    newTop = object.top;

                    newTop = (object.top / model.screenHeight) * 100;//object.top / model.screenHeight >= 0.88 ? 88 : (object.top / model.screenHeight) * 100;
                    newLeft = (object.left / model.screenWidth) * 100;//object.left / model.screenWidth >= 0.88 ? 88 : (object.left / model.screenWidth) * 100;

                    newTimeout = answer.picture.timeout;
                    realIndex = i;
                    audioTimeOut = answer.audio.timeout;
                }
                else { //位置重置 
                    //找出randomAnswer在jsonLayout.objects中的索引
                    var realIndex = jsonLayout.objects.indexOf(jsonLayout.objects.filter(function (obj) {
                        return obj.id == randomAnswer.id
                    })[0]);
                    if (!model.answers[realIndex]) {   //ItemLayout数据不对
                        model.itemLayout["ImageList"] = [];
                        break;
                    }
                    newTimeout = model.answers[realIndex].picture.timeout;
                    audioTimeOut = model.answers[realIndex].audio.timeout;
                    randomAnswer = jsonLayout.objects.filter(function (obj) {
                        return obj.id == randomAnswer.id;
                    })[0];
                    var widthRate = randomAnswer.width / object.width;
                    var heightRate = randomAnswer.height / object.height;
                    rate = widthRate < heightRate ? widthRate : heightRate;

                    rate = randomAnswer.width / model.screenWidth * 100;
                    if (rate == 0)//计算出错的情况下读取之前的比例
                        rate = 1;

                    var centerTop = randomAnswer.height / 2.00 + randomAnswer.top;
                    var centerLeft = randomAnswer.width / 2.00 + randomAnswer.left;

                    var newTop2 = centerTop - object.height / 2.00;
                    var newLeft2 = centerLeft - object.width / 2.00;


                    newLeft = (newLeft2 / model.screenWidth) * 100;//randomAnswer.left / model.screenWidth >= 0.88 ? 88 : (randomAnswer.left / model.screenWidth) * 100;
                    //randomAnswer.left / model.screenWidth >= 0.88 ? 88 : (randomAnswer.left / model.screenWidth) * 100;
                    newTop = (newTop2 / model.screenHeight) * 100;//newTop = randomAnswer.top / model.screenHeight >= 0.88 ? 88 : (randomAnswer.top / model.screenHeight) * 100; //.randomAnswer.top;
                    realIndex = realIndex;
                }

                if (newTimeout == 0) newTimeout = 1;//防止图片加载顺序变化
                model.itemLayout["ImageList"].push({
                    "id": object.id,
                    "imageType": answer.imageType,
                    "src": answer.picture.fullImgUrl,
                    "top": newTop,
                    "left": newLeft,//百分比值
                    "width": (object.width / model.screenWidth * 100), //object.width * rate,
                    "height": (object.height / model.screenHeight * 100),
                    "timeout": newTimeout,
                    "isCorrect": answer.isCorrect,
                    "choosed": ko.observable(answer.choosed()),
                    "score": answer.score,
                    "realIndex": realIndex,
                    "audioTimeOut": audioTimeOut,
                    "notDelay": newTimeout == 0 && realIndex == 0 && !model.instructionAudio.file
                    //若没有instruction,则不延迟；第一个显示且没有延迟，也不需要延迟
                });
            }

            model.unEndPictureLength = model.itemLayout["ImageList"].filter(function (obj) { return !obj.notDelay && obj.src }).length;
            model.unEndPictureCount(model.unEndPictureLength);
        }
    }

    //为answer对象添加MinNumber和MaxNumber属性
    if (model.answers && model.answers.length) {
        var realSeqNumber = 1;
        var sortedAnswers = JSON.parse(JSON.stringify(model.answers))
            .filter(function (obj) { return obj.isCorrect == true })
            .sort(function (a, b) {
                return a.sequenceNumber - b.sequenceNumber
            });
        var calculatedAnswers = [];
        for (var i = 0; i < sortedAnswers.length; i++) {
            var targetAnswer = sortedAnswers[i];
            var duplicateCount = sortedAnswers.filter(function (obj) {
                return obj.sequenceNumber == sortedAnswers[i].sequenceNumber
            }).length;
            if (duplicateCount == 1) {  //该组只有一个
                calculatedAnswers.push({
                    id: targetAnswer.id,
                    minSeqNumber: realSeqNumber, maxSeqNumber: realSeqNumber
                });
            }
            else { //有多个seqNumber相同 
                for (var j = 0; j < duplicateCount; j++) {

                    var targetAnswer = sortedAnswers[i + j];
                    calculatedAnswers.push({
                        id: targetAnswer.id,
                        minSeqNumber: realSeqNumber, maxSeqNumber: realSeqNumber + duplicateCount - 1
                    });
                }
                i = i + duplicateCount - 1; //跳过同组的
            }
            realSeqNumber = realSeqNumber + duplicateCount;
        }
        if (calculatedAnswers.length) {
            for (var i = 0; i < calculatedAnswers.length; i++) {
                var calculatedAnswer = calculatedAnswers[i];
                var targetAnswer = model.answers.filter(function (obj) { return obj.id == calculatedAnswer.id })[0];
                if (targetAnswer) {
                    targetAnswer.minSeqNumber = calculatedAnswer.minSeqNumber;
                    targetAnswer.maxSeqNumber = calculatedAnswer.maxSeqNumber;
                }
            }
        }
    }

    model.prepared = model.promptPrepared;

    model.showPicture = function (index) {
        //跳入上一个或者下一个时，不继续执行
        if (model.isShowing() && index < model.answers.length) {
            var sortAnswer = model.itemLayout.ImageList.filter(function (obj) {
                return obj.realIndex == index
            })[0];
            if (sortAnswer) {
                var targetAnswer = model.answers.filter(function (obj) {
                    return obj.id == sortAnswer.id;
                })[0];
                if (targetAnswer.picture) {
                    targetAnswer.picture.timer = new TimeoutTimer(+(sortAnswer.timeout));
                    $.when(targetAnswer.picture.timer.timeUp()).done(function () {
                        if ($("#img_txkea_" + targetAnswer.id).css("display") == "none") {
                            $("#img_txkea_" + targetAnswer.id).show();
                            model.unEndPictureCount(model.unEndPictureCount() - 1);
                        }
                        else {
                            model.unEndPictureCount(model.unEndPictureCount() - 1);
                        }
                        model.showAudio(index);
                    });
                    targetAnswer.picture.timer.start();
                }
                else {
                    if ($("#img_txkea_" + targetAnswer.id).css("display") == "none") {
                        $("#img_txkea_" + targetAnswer.id).show();
                        model.unEndPictureCount(model.unEndPictureCount() - 1);
                    }
                    model.showAudio(index);
                }
            }
        }
    }

    model.showAudio = function (index) {
        if (model.isShowing()) { //跳入上一个或者下一个时，不继续执行
            var sortAnswer = model.itemLayout.ImageList.filter(function (obj) {
                return obj.realIndex == index
            })[0];
            var targetAnswer = model.answers.filter(function (obj) {
                return obj.id == sortAnswer.id;
            })[0];

            if (targetAnswer.audio && targetAnswer.audio.file && targetAnswer.audio.player) {
                targetAnswer.audio.player.onended = function () {
                    if (model.isShowing()) {  //当点击暂停时，不继续执行
                        model.unEndAudioCount(model.unEndAudioCount() - 1);
                        model.showPicture(index + 1);
                    }
                };
                if (targetAnswer.audio) {
                    targetAnswer.audio.timer = new TimeoutTimer(+(sortAnswer.audioTimeOut));
                    $.when(targetAnswer.audio.timer.timeUp()).done(function () {
                        execHelper.overrideAudioSrc(targetAnswer.audio);
                        targetAnswer.audio.player.play();
                    });
                    targetAnswer.audio.timer.start();
                } else {
                    execHelper.overrideAudioSrc(targetAnswer.audio);
                    targetAnswer.audio.player.play();
                }
            }
            else {
                model.showPicture(index + 1);
            }
        }
    }

    model.showItem = function () {
        //跳入上一个或者下一个时，不继续执行
        if (model.isShowing() && model.answers && model.answers.length) {
            model.showPicture(0);
        }
    }

    return model;
}


function getItemFromFactory(defaultValues) {
    var itemModel;
    var type = isNull("Type", defaultValues, {
        text: "",
        value: 0
    }).value;
    switch (type) {
        case Ade_ItemType.Checklist.value:
            itemModel = getChecklistItemModel(defaultValues);
            break;
        case Ade_ItemType.Direction.value:
            itemModel = getDirectionItemModel(defaultValues);
            break;
        case Ade_ItemType.MultipleChoices.value:
            itemModel = getMultipleChoicesItemModel(defaultValues);
            break;
        case Ade_ItemType.Pa.value:
            itemModel = getPaItemModel(defaultValues);
            break;
        case Ade_ItemType.Quadrant.value:
            itemModel = getQuadrantItemModel(defaultValues);
            break;
        case Ade_ItemType.RapidExpressive.value:
            itemModel = getRapidExpressiveItemModel(defaultValues);
            break;
        case Ade_ItemType.Receptive.value:
            itemModel = getReceptiveItemModel(defaultValues);
            break;
        case Ade_ItemType.ReceptivePrompt.value:
            itemModel = getReceptivePromptItemModel(defaultValues);
            break;
        case Ade_ItemType.TypedResponse.value:
            itemModel = getTypedResponseItemModel(defaultValues);
            break;
        case Ade_ItemType.TxkeaExpressive.value:
            itemModel = getTxkeaExpressiveItemModel(defaultValues);
            break;
        case Ade_ItemType.TxkeaReceptive.value:
            itemModel = getTxkeaReceptiveItemModel(defaultValues);
            break;
        default:
            console.error("Item type %s not supported", type);
            break;
    }
    return itemModel;
}

function getParentMeasureModel(defaultValues) {
    function ParentMeasureModel() {
    };

    var model = new ParentMeasureModel();

    var loader = new LoadEvents();

    model.id = isNull("ID", defaultValues, 0);
    if (!window.parentMeasures) {
        window.parentMeasures = {
        };
    }
    if (model.id in window.parentMeasures) {
        return window.parentMeasures[model.id];
    } else {
        window.parentMeasures[model.id] = model;
    }

    model.name = isNull("Name", defaultValues, execHelper.messages.noName);
    model.startPage = {
        file: isNull("StartPageHtml", defaultValues, null),
        content: null,
        loaded: false,
        showed: false
    };

    model.endPage = {
        file: isNull("EndPageHtml", defaultValues, null),
        content: null,
        loaded: false,
        showed: false
    };
    if (model.id <= 1) {
        model.startPage.file = model.endPage.file = false;
    }
    else {
        if (model.startPage.file) {
            loader.waitingOne();
        }
        if (model.endPage.file) {
            loader.waitingOne();
        }
    }
    model.needShowStart = function () {
        if (model.id <= 1) {
            return false;
        }
        if (!model.startPage.file) {
            return false;
        }
        if (!model.startPage.loaded) {
            return false;
        }
        if (!model.startPage.content) {
            return false;
        }
        if (model.startPage.showed) {
            return false;
        }
        return true;
    };
    model.needShowEnd = function () {
        if (model.id <= 1) {
            return false;
        }
        if (!model.endPage.file) {
            return false;
        }
        if (!model.endPage.loaded) {
            return false;
        }
        if (!model.endPage.content) {
            return false;
        }
        if (model.endPage.showed) {
            return false;
        }
        return true;
    };

    model.prepare = false;
    model.prepared = function () {
        var waitingResource = false;
        if (model.startPage && model.startPage.file && model.startPage.loaded === false) {
            waitingResource = true;
            execHelper.getHtml(model.startPage.file, function (html) {
                execHelper.log("measure [" + model.name + "] start page loaded.");
                html = html.replace("/body/g", "div");
                model.startPage.content = html;
                model.startPage.loaded = true;
                loader.loadedOne();
                loader.loadCompleted("measure [" + model.name + "] start page");
            });
        } else {
            model.startPage.file = false;
        }
        if (model.endPage && model.endPage.file && model.endPage.loaded === false) {
            waitingResource = true;
            execHelper.getHtml(model.endPage.file, function (html) {
                execHelper.log("measure [" + model.name + "] end page loaded.");
                html = html.replace(/body/g, "div");
                model.endPage.content = html;
                model.endPage.loaded = true;
                loader.loadedOne();
                loader.loadCompleted("measure [" + model.name + "] end page");
            });
        } else {
            model.endPage.file = false;
        }
        if (!waitingResource) {
            setTimeout(function () {
                loader.loadCompleted("measure [" + model.name + "] no resources");
            }, 0);
        } else {
            execHelper.log("start measure [" + model.name + "]  resouces.");
        }
        return loader.promise();
    }

    return model;
}

function getMeasureModel(defaultValues, assessmentModel) {
    function MeasureModel() {
    };

    var model = new MeasureModel();
    var loader = new LoadEvents();
    model.prepare = false;

    model.id = isNull("MeasureId", defaultValues, 0);
    model.execId = isNull("ExecId", defaultValues, 0);
    model.name = isNull("Name", defaultValues, execHelper.messages.noName);
    model.timeout = isNull("Timeout", defaultValues, 0) * 1000;
    model.innerTime = isNull("InnerTime", defaultValues, 0);
    model.benchmark = isNull("Benchmark", defaultValues, 0);
    model.pauseTime = isNull("PauseTime", defaultValues, 0);
    model.goal = ko.observable(isNull("Goal", defaultValues, -1));
    model.totalScore = isNull("TotalScore", defaultValues, 0);
    model.totalScored = isNull("TotalScored", defaultValues, true);
    model.readonly = isNull("Readonly", defaultValues, false);
    model.showFinalizePage = isNull("ShowFinalizePage", defaultValues, false);
    model.orderType = isNull("OrderType", defaultValues, {
        value: 0
    }).value;
    model.startWith = isNull("StartWith", defaultValues, 0);
    model.showType = isNull("ShowType", defaultValues, {
        value: 0
    }).value;
    model.updatedOn = isNull("UpdatedOn", defaultValues, "");
    model.ageGroup = isNull("AgeGroup", defaultValues, "");
    model.comment = ko.observable(isNull("Comment", defaultValues, '') || '');
    model.status = isNull("Status", defaultValues, {
        value: Cpalls_Status.Initialised
    }).value;
    model.metBenchmark = isNull("BenchmarkText", defaultValues, "");
    model.previousButton = isNull("PreviousButton", defaultValues, false);
    model.nextButton = isNull("NextButton", defaultValues, false);
    model.stopButton = isNull("StopButton", defaultValues, false);

    model.parent = getParentMeasureModel(isNull("Parent", defaultValues, ""));
    model.items = [];
    var items = isNull("Items", defaultValues, []).slice(0);
    var item, realItemStard, orderProcessor, totalItems;
    if (items && items.length) {
        totalItems = items.length;
        realItemStard = false;
        for (var i = 0; i < totalItems; i++) {
            if (realItemStard) {
                item = items[orderProcessor.next()];
            } else {
                item = items.shift();
                if (item.IsPractice || item.Type.value === Ade_ItemType.Direction.value) {
                    // Items 在服务端必须排好顺序, 以保证
                    // Practice Item 首先显示, Direction Item 首先显示
                } else if (assessmentModel.mode === Exec_Mode.Exec) {
                    items.unshift(item);
                    realItemStard = true;
                    orderProcessor = GetOrderProcessor(items.length, model.orderType);
                    i--;
                    continue;
                }
            }
            model.items.push(getItemFromFactory(item));
        }
    }
    model.finalScore = ko.computed(function () {
        var total = '';
        if (isNumber(model.goal())) {
            total = model.goal() + 0;
        }
        if (!model.totalScored) {
            return total;
        }

        var measureTotal = 0;
        if (model.items) {
            model.items.forEach(function (obj) {
                if (obj.scored == true && obj.executed() == true) {
                    measureTotal += obj.score;
                }
            })
            //例如：当分数为12.2+0.2时，结果为12.399999999999999
            measureTotal = Number(measureTotal.toFixed(2));
            model.totalScore = measureTotal;
        }

        return total + ' / ' + measureTotal;
    }, model);
    model.startPage = {
        file: isNull("StartPageHtml", defaultValues, null),
        content: null
    };
    if (model.startPage.file) {
        loader.waitingOne();
    }
    model.endPage = {
        file: isNull("EndPageHtml", defaultValues, null),
        content: null
    };
    if (model.endPage.file) {
        loader.waitingOne();
    }

    var preparedIndex = 0;
    var itemLoaderEvents = $.Deferred();
    model.itemPrepared = function () {
        if (preparedIndex < model.items.length) {
            jQuery.when(model.items[preparedIndex].prepared()).done(function () {
                execHelper.log("item [" + model.items[preparedIndex].label + "],prepared.");
                model.items[preparedIndex].prepare = true;
                preparedIndex++;
                model.itemPrepared();
            });
        } else {
            setTimeout(function () {
                itemLoaderEvents.resolve();
            }, 0);
        }
        return itemLoaderEvents.promise();
    }

    model.prepared = function () {
        $.when(model.parent.prepared(), model.itemPrepared()).done(function () {
            execHelper.log("measure [" + model.name + "] started.");
            var waitingResource = false;
            if (model.startPage && model.startPage.file) {
                waitingResource = true;
                execHelper.getHtml(model.startPage.file, function (html) {
                    execHelper.log("measure [" + model.name + "] start page loaded.");
                    html = html.replace(/body/g, "div");
                    model.startPage.content = html;
                    loader.loadedOne();
                    loader.loadCompleted("measure [" + model.name + "] start page");
                });
            } else {
                model.startPage.file = false;
            }
            if (model.endPage && model.endPage.file) {
                waitingResource = true;
                execHelper.getHtml(model.endPage.file, function (html) {
                    execHelper.log("measure [" + model.name + "] end page loaded.");
                    html = html.replace(/body/g, "div");
                    model.endPage.content = html;
                    loader.loadedOne();
                    loader.loadCompleted("measure [" + model.name + "] end page");
                });
            } else {
                model.endPage.file = false;
            }

            if (!waitingResource) {
                setTimeout(function () {
                    loader.loadCompleted("measure [" + model.name + "] no resources");
                }, 0);
            } else {
                execHelper.log("start measure [" + model.name + "]  resouces.");
            }
        });
        return loader.promise();
    };

    //记录上一个item 的type ，用来判断当前item 是否需要显示 header
    var lastItemType = -1;
    model.needShowHeader = function (itemModel) {
        var i = 0;
        var tmpResult = false;
        var currentItemType = itemModel.type;
        if (itemModel.type == Ade_ItemType.Receptive.value) {
            currentItemType = Ade_ItemType.ReceptivePrompt.value;
        }
        if (itemModel.showInResultPage()) {
            tmpResult = currentItemType != lastItemType;
            lastItemType = currentItemType;
        }
        return tmpResult;
    }
    //用于判断measure是否有TxkeaExpressive或者TxkeaReceptive类型的item
    model.isTxkea = model.items.filter(function (obj) {
        return obj.type == Ade_ItemType.TxkeaExpressive.value
            || obj.type == Ade_ItemType.TxkeaReceptive.value
    }).length > 0;

    return model;
}


function getStudent(defaultValues) {
    var student = {};
    student.id = isNull("ID", defaultValues, 0);
    student.name = isNull("Name", defaultValues, execHelper.messages.noName);
    student.birthday = isNull("Birthday", defaultValues, "");
    return student;
}

function getClass(defaultValues) {
    var class1 = {};
    class1.id = isNull("ID", defaultValues, 0);
    class1.name = isNull("Name", defaultValues, execHelper.messages.noName);
    class1.homeroomTeacher = isNull("HomeroomTeacher", defaultValues, "");
    return class1;
}

Exec_Status = {
    //初始化
    initialised: 0,
    0: "Init",

    //已准备好
    prepared: 5,
    5: "Prepared",

    //已就绪(显示父级Measure的 Start Page,若没有 则直接下一步)
    ready0: 9,
    9: "Ready(Showing Parent Start Page)",

    //已就绪(显示当前的 Start Page,若没有 则直接下一步)
    ready: 10,
    10: "Ready",

    // 选择题目(Sequenced类型的Measure,用户可以指定从哪一题开始)
    // 已移除,功能从Index页面增加StartWith
    // choosing: 12,
    // 12: "Chossing Item",

    //进行中
    going: 20,
    20: "On going",

    //当前Measure已超时
    timeout: 30,
    30: "Time Out",

    //暂停中
    paused: 40,
    40: "Paused",

    //当前Measure已结束(显示当前MeasureEnd Page,若没有则直接下一步)
    complete: 50,
    50: "Complete",

    //正在显示Measure结果页面
    resulting: 60,
    60: "Showing Result",

    //当前Measure已结束(显示父级End Page,若没有则直接下一步)
    complete1: 61,
    61: "Complete(Showing Parent Measure End Page)",

    //所有Measure结束,Assessment Over
    over: 70,
    70: "Over",

    //Assessment执行结束,数据已保存,可以离开页面
    saved: 80,
    80: "Saved"
};

var Exec_Mode = {
    Exec: 1,
    View: 2,
    Preview: 3
};
var Exec_Network = {
    online: 1,
    offline: 2
};

function getAssessmentModel(defaultValues) {
    function ExecAssessmentModel() {

    };

    var model = new ExecAssessmentModel();

    model.student = getStudent(isNull("Student", defaultValues, {}));
    model.classModel = getClass(isNull("Class", defaultValues, {}));

    model.execId = isNull("ExecId", defaultValues, 0);
    model.assessmentId = isNull("AssessmentId", defaultValues, 0);
    model.name = isNull("Name", defaultValues, execHelper.messages.noName);
    model.schoolName = isNull("SchoolName", defaultValues, execHelper.messages.noName);
    model.schoolId = isNull("SchoolId", defaultValues, 0);
    model.schoolYear = isNull("SchoolYear", defaultValues, "");
    model.wave = isNull("Wave", defaultValues, "");
    model.editable = isNull("InExecDate", defaultValues, false);
    model.orderType = isNull("OrderType", defaultValues, { value: 0 }).value;


    model.language = isNull("Language", defaultValues, {});
    model.communityName = isNull("CommunityName", defaultValues, "");
    //记录多个measure的Id，回到 student view页面时，保持 多个 measure 的选定
    model.keepSelectdMeasureIds = isNull("KeepSelectdMeasureIds", defaultValues, []);

    // 当前模式: 做,查看,预览, 不同模式有不同按钮显示
    model.mode = isNull("Mode", defaultValues, 0);
    model.online = true;
    if (defaultValues && defaultValues.offline && defaultValues.offline === true) {
        model.online = false;
    }

    model.measures = [];

    var measures = isNull("Measures", defaultValues, []);

    if (measures && measures.length) {
        var orderProcessor = GetOrderProcessor(measures.length, model.orderType);
        for (var i = 0; i < measures.length; i++) {
            var i_m = orderProcessor.next();
            var measure = measures[i_m];
            var m = getMeasureModel(measure, model);
            model.measures.push(m);
        }
    }

    // 定义当前执行状态, 根据不同状态判断屏幕上显示哪些内容
    model.status = ko.observable(Exec_Status.initialised);

    model.visible = ko.observable(false);

    var loadEvent = $.Deferred();
    var preparedIndex = 0;
    model.prepared = function () {
        execHelper.getImage(execHelper.waitingImg);
        if (preparedIndex < model.measures.length) {
            jQuery.when(model.measures[preparedIndex].prepared()).done(function () {
                execHelper.log("measure [" + model.measures[preparedIndex].name + "], prepared.");
                model.measures[preparedIndex].prepare = true;
                preparedIndex++;
                model.prepared();
            });
        } else {
            if (model.mode != Exec_Mode.View) {
                model.status(Exec_Status.prepared);
                model.visible(true);
                model.prepare = true;
                model.bindKeyboardEvents();
            }
            loadEvent.resolve();
        }
        return loadEvent.promise();
    }

    // bindings:start
    model.currentMeasureIndex = ko.observable(0);
    model.currentMeasure = ko.computed(function () {

        if (model.measureRunning) {
            model.measureRunning.reset();
        }

        return model.measures[model.currentMeasureIndex()];
    }, model);

    model.currentItemIndex = ko.observable(0);

    model.currentItem = ko.computed(function () {
        return model.measures[model.currentMeasureIndex()].items[model.currentItemIndex()];
    }, model);

    model.currentMeasureIndex.subscribe(function () {
        model.currentItemIndex(model.currentMeasure().startWith);
    });

    model.itemTemplate = function (item) {
        return "_cpalls_Item_" + item.type;
    };
    model.itemResultTemplate = function (item) {
        return "_cpalls_ItemResult_" + item.type;
    };
    // bindings:end

    // next/previous functions: start

    model.nextHtml = ko.computed(function () {
        var lastIndex = model.measures.length - 1;
        if (model.currentMeasureIndex() >= lastIndex) {
            var noNeedShowEnd = model.status() == Exec_Status.resulting && !model.currentMeasure().parent.needShowEnd();
            var showingEnd = model.status() == Exec_Status.complete1;
            if (noNeedShowEnd || showingEnd) {
                return "<i class='icon-ok'></i>Done";
            }
        }
        return "<i class='icon-hand-right'></i>Next";
    }, model);


    model.startCurrentMeasure = function () {
        if (model.status() == Exec_Status.prepared
            && model.currentMeasure().parent.needShowStart()
            && model.mode == Exec_Mode.Exec) {
            model.status(Exec_Status.ready0);
            // 如果需要显示父级Measure
            return;
        }
        if ((model.status() == Exec_Status.prepared || model.status() == Exec_Status.ready0)
            && model.currentMeasure().startPage.file
            && model.mode == Exec_Mode.Exec) {
            // 不需要显示父级Measure 或者已经显示了父级Measure
            model.status(Exec_Status.ready);
            return;
        }

        model.currentMeasure().parent.startPage.showed = true;

        model.status(Exec_Status.going);
        model.currentItemIndex(model.currentMeasure().startWith);
        model.currentItemIndex.notifySubscribers();

        if (model.currentItem().needDoAssessment()) {
            model.currentItem().isShowing(true);
            model.switchTimer();
            $("#divContent").height($("#divContent").width() * (565 / 1024));
            if ($("#divResponseContent").length != 0) {
                $("#divResponseContent").height($("#divContent").height());
            }

        } else {
            model.nextItem();
        }

    };

    model.nextMeasure = function () {
        var haveNextMeasure = model.currentMeasureIndex() < model.measures.length - 1;
        if (haveNextMeasure) {
            model.currentItemIndex(0);
            model.currentMeasureIndex(model.currentMeasureIndex() + 1);
            model.prepared();
            model.startCurrentMeasure();
        } else {
            execHelper.log("All measures over.");
            model.status(Exec_Status.over);
            model.syncServer();
        }
    };

    model.endCurrentMeasure = function () {
        var items = "";
        $.each(model.currentMeasure().items, function (index, item) {
            if (item.isRequired) {
                var hasChoosed = false;
                $.each(item.answers, function (i, answer) {
                    if (answer.choosed()) {
                        hasChoosed = true;
                    }

                });
                if (!hasChoosed) {
                    //items += item.label + "; ";
                    items += (index + 1) + "; ";
                }
            }
        });
        if (items != "") {
            items = items.substring(0, items.lastIndexOf(';'));
            showMessage("warning", "<br\><span style='color:#000'>The following items require a score before proceeding:<br /><br />Item " + items + "<span>");
            return;
        }
        if (model.currentMeasure().items == Ade_ItemType.Checklist.value)


            if (model.status() == Exec_Status.going && model.currentMeasure().endPage.file) {
                model.status(Exec_Status.complete);
                return;
            }
        if (model.status() == Exec_Status.going || model.status() == Exec_Status.complete) {
            model.calculateGoal();
            if (model.currentMeasure().showFinalizePage) {
                model.status(Exec_Status.resulting);
                model.currentMeasure().status = Cpalls_Status.Finished;
                return;
            } else {
                model.currentMeasure().status = Cpalls_Status.Finished;
                model.nextMeasure();
                return;
            }
        }
        var allMeasuresOver = model.currentMeasureIndex() + 1 >= model.measures.length;
        var nextMeasureIsNotSameParent = allMeasuresOver === false
            && model.measures[model.currentMeasureIndex() + 1].parent.id != model.currentMeasure().parent.id;
        var showParentEndPage = model.currentMeasure().parent.needShowEnd() && (allMeasuresOver || nextMeasureIsNotSameParent);
        if (model.status() == Exec_Status.resulting && showParentEndPage) {
            model.status(Exec_Status.complete1);
            return;
        }
        if (model.status() === Exec_Status.resulting || model.status() == Exec_Status.complete1) {
            model.nextMeasure();
            return;
        }
    };

    // 执行下一个Item逻辑
    model.switchToNextItem = function (clickEvent) {
        model.currentItem().isShowing(false);
        model.nextTimer && model.nextTimer.cancel();
        // when user click next, get over Checklist measure
        // but stay here if page was just loaded(and the measure was paused)
        if (model.currentItemIndex() == model.currentMeasure().items.length - 1
            || (clickEvent && model.currentMeasure().showType == Cpalls_Measure_ShowType.List)) {
            // current measure over
            // before next measure: show endpage, result page;
            execHelper.log("measure over.");
            model.currentItem().isShowing(false);
            model.pauseTimer();
            model.endCurrentMeasure();
        } else {
            var nextItemFinished = false;
            var nextIndex = model.currentItemIndex() + 1;

            var skipIndex = 0;

            if (model.currentItem().type == Ade_ItemType.TxkeaReceptive.value
                || model.currentItem().type == Ade_ItemType.TxkeaExpressive.value) {
                model.currentItem().getGoal();

                if (model.currentItem().branchingScores) {
                    var skipItemId;
                    for (var i = 0; i < model.currentItem().branchingScores.length; i++) {
                        if (model.currentItem().branchingScores[i].From <= model.currentItem().goal() && model.currentItem().branchingScores[i].To >= model.currentItem().goal()) {
                            skipItemId = model.currentItem().branchingScores[i].SkipItemId;
                            if (skipItemId == -1) //选择end选项，直接结束measure
                            {
                                model.pauseTimer();
                                //该item之后的item都标记为跳过
                                var itemLength = model.currentMeasure().items.length;
                                for (var i = nextIndex; i < itemLength; i++) {
                                    model.currentMeasure().items[i].executed(false);
                                }
                                model.endCurrentMeasure();
                                return;
                            }
                            break;
                        }
                    }
                    if (skipItemId) {
                        for (var i = nextIndex; i < model.currentMeasure().items.length; i++) {
                            if (model.currentMeasure().items[i].itemId == skipItemId) {
                                skipIndex = i;
                                model.currentItemIndex(skipIndex);
                                if (!model.currentItem().needDoAssessment()) {
                                    nextItemFinished = true;
                                    break;
                                }
                                model.currentItem().isShowing(true);
                                model.switchTimer();
                                window.scrollTo(0, 0);
                                $("#divContent").height($("#divContent").width() * (565 / 1024));
                                if ($("#divResponseContent").length != 0) {
                                    $("#divResponseContent").height($("#divContent").height());
                                }
                                break;
                            }
                        }
                    }
                }
            }

            if (skipIndex == 0) {
                for (; nextIndex < model.currentMeasure().items.length; nextIndex++) {
                    model.currentItemIndex(nextIndex);
                    if (!model.currentItem().needDoAssessment()) {
                        nextItemFinished = true;
                        break;
                    }
                    model.currentItem().isShowing(true);
                    model.switchTimer();
                    window.scrollTo(0, 0);
                    $("#divContent").height($("#divContent").width() * (565 / 1024));
                    if ($("#divResponseContent").length != 0) {
                        $("#divResponseContent").height($("#divContent").height());
                    }
                    break;
                }
            } else {
                $("#divContent").height($("#divContent").width() * (565 / 1024));
                if ($("#divResponseContent").length != 0) {
                    $("#divResponseContent").height($("#divContent").height());
                }

                for (var i = nextIndex; i < skipIndex; i++) {
                    model.currentMeasure().items[i].executed(false);
                }
            }
            model.currentItem().lastItemIndex(nextIndex - 1);

            // Checklist Measure 暂停回来之后不要跳到结果页面
            if (model.currentItemIndex() == model.currentMeasure().items.length - 1
                && model.currentMeasure().showType == Cpalls_Measure_ShowType.List) {
                nextItemFinished = false;
            }
            if (nextItemFinished) {
                model.switchToNextItem(clickEvent);
            }
        }
    };

    // 如果当前Item已完成(主要针对于验证Typed Response 类型的必填, 其他类型暂无验证), 则跳到下一个Item
    model.goToNext = function (clickEvent) {
        if (!model.currentItem().finished() && model.currentItem().status() != 3) {
            model.currentItem().timeoutTimer = null; //取消事件，通过验证后再次进入goToNext 方法
            return;
        }
        model.switchToNextItem(clickEvent);
    };

    // 跳到下一个Item, 或者在InnerTime之后跳到下一个Item
    model.nextItem = function (clickEvent) {
        //Receptive item 单独控制
        if (model.isWaitingNext)
            model.isWaitingNext = false;
        if (model.isCanNext)
            model.isCanNext = true;

        //控制 item 显示的前 0.5s 不能点击
        if (model.currentItem().startClickEvent && model.currentItem().startClick > 0) {
            //重置标记，中止自动执行
            model.nextTimer && model.nextTimer.cancel();

            ///需要遮罩
            if (model.currentMeasure().innerTime > 0) {
                if (model.currentItem().timeoutTimer) {
                    //已定义
                }
                else {
                    model.visible(false);
                    model.pauseTimer();  //部署时要开启
                    model.currentItem().timeoutTimer = new TimeoutTimer(this.currentMeasure().innerTime);
                    $.when(model.currentItem().timeoutTimer.timeUp()).done(function () {
                        model.visible(true);
                        model.goToNext(clickEvent);
                    });
                    model.currentItem().timeoutTimer.start();
                }
            } else {
                model.goToNext(clickEvent);
            }
        }
        else {
            switch (model.currentItem().type) {
                case Ade_ItemType.RapidExpressive.value:
                    model.markCurrentItem(false);
                    break;
                case Ade_ItemType.Quadrant.value:
                    $.each(model.currentItem().answers, function (i, a) {
                        a.choosed(false);
                    });
                    model.currentItem().selectedAnswers.removeAll();
                    break;
                case Ade_ItemType.MultipleChoices.value:
                    break;
                case Ade_ItemType.Pa.value:
                    break;
                case Ade_ItemType.Receptive.value:
                    $.each(model.currentItem().answers, function (i, a) {
                        a.choosed(false);
                    });
                    model.currentItem().selectedAnswers.removeAll();
                    break;
                case Ade_ItemType.ReceptivePrompt.value:
                    $.each(model.currentItem().answers, function (i, a) {
                        a.choosed(false);
                    });
                    model.currentItem().selectedAnswers.removeAll();
                    break;
                case Ade_ItemType.TypedResponse.value:
                    break;
                case Ade_ItemType.TxkeaReceptive.value:
                    break;
                case Ade_ItemType.TxkeaExpressive.value:
                    break;
            }
        }
    }


    // 是否显示 Next 按钮
    model.showNextButton = ko.computed(function () {
        if (model.mode != Exec_Mode.Exec) {
            return false;
        }
        if (model.status() == Exec_Status.ready0 || model.status() == Exec_Status.ready) {
            return true;
        }
        if (model.status() == Exec_Status.complete || model.status() == Exec_Status.complete1) {
            return true;
        }
        if (model.status() == Exec_Status.resulting) {
            return true;
        }
        if (model.status() == Exec_Status.over) {
            return true;
        }
        if (model.status() == Exec_Status.going) {
            //如果是pause之后过来的，首页应显示Next按钮
            if (model.currentMeasure().status == Cpalls_Status.Paused && model.currentItem().status() == Cpalls_Status.Finished) {
                return true;
            }
            else if (model.currentItem().type == Ade_ItemType.MultipleChoices.value
                || model.currentItem().type == Ade_ItemType.Pa.value
                || model.currentItem().type == Ade_ItemType.Direction.value
                || model.currentItem().type == Ade_ItemType.Checklist.value
                || model.currentItem().type == Ade_ItemType.TypedResponse.value
                || model.currentItem().type == Ade_ItemType.TxkeaReceptive.value
            ) {
                //此处需要通过measure属性控制  
                return model.currentMeasure().nextButton;
            }
            else if (model.currentItem().type == Ade_ItemType.TxkeaExpressive.value
                && model.currentMeasure().nextButton == true) {
                if (model.currentItem().status() == 3)
                    return true;
                else {
                    return model.currentItem().ShowResponse();
                }
            }

        }
        return false;
    }, model);

    model.next = function (_, event) {
        ///状态见 1279行
        switch (model.status()) {
            case Exec_Status.initialised:
                // 错误, 资源未加载, 此时不能执行任何程序
                break;
            case Exec_Status.ready0:
            case Exec_Status.ready:
                model.startCurrentMeasure();
                break;
            case Exec_Status.going:
                model.nextItem(event);
                break;
            case Exec_Status.complete:
            case Exec_Status.complete1:
            case Exec_Status.resulting:
                model.endCurrentMeasure();
                break;
            case Exec_Status.over:
                execHelper.log("assessment over.");
                model.syncServer();
                break;
            default: break;
        }
    };

    model.showPreviousButton = ko.computed(function () {
        if (this.status() != Exec_Status.going) {
            return false;
        }
        if (this.currentMeasure().timeout > 0) {
            return false;
        }
        if (this.currentItemIndex() == 0) {
            return false;
        }
        if (this.currentItem().timeout > 0) {
            return false;
        }
        if (this.currentMeasure().items[this.currentItemIndex() - 1].timeout > 0) {
            return false;
        }
        //previousButton是否显示由measure中的属性控制
        if (this.currentMeasure().previousButton != true) {
            return false;
        } else {
            if (model.currentItem().type == Ade_ItemType.TxkeaExpressive.value) {
                if (model.currentItem().responseType == 1) //Simple
                    return true;
                else
                    return model.currentItem().ShowResponse() == false;
            }
        }
        return true;
    }, model);

    model.resetItem = function () {
        model.currentItem().timeoutTimer = null;
        if (model.currentItem().answers) {
            $.each(model.currentItem().answers, function (i, a) {
                a.choosed(false);
            });
        }

        if (model.currentItem().type == Ade_ItemType.TxkeaExpressive.value) {
            if (model.currentItem().responseType == 1) { //Simple
            }
            else {
                if (model.currentItem().responses()) {
                    for (var i = 0; i < model.currentItem().responses().length; i++) {
                        for (var j = 0; j < model.currentItem().responses()[i].Options().length; j++) {
                            if (model.currentItem().responses()[i].Type == 1)
                                model.currentItem().responses()[i].content("");
                            else {
                                model.currentItem().responses()[i].Options()[j].IsCorrect(false);
                                model.currentItem().responses()[i].Options()[j].otherText("");
                            }
                        }
                        model.currentItem().responses()[i].content("");
                    }
                }
            }
        }

        if (model.currentItem().type == Ade_ItemType.TxkeaReceptive.value) {
            //清空选择记录
            if (model.currentItem().selectedDetailedAnswers) {
                model.currentItem().selectedDetailedAnswers = [];
            }
            //清空页面绑定的数据（重新构造的itemLayout.ImageList对象）
            if (model.currentItem().itemLayout && model.currentItem().itemLayout.ImageList
                && model.currentItem().itemLayout.ImageList.length) {
                $.each(model.currentItem().itemLayout.ImageList, function (i, a) {
                    a.choosed(false);
                });
            }
        }


        model.currentItem().selectedAnswers.removeAll();

        if (model.currentMeasure().measureRunning) {
            if (model.currentItem().status() == Cpalls_Status.Finished) {  //暂停&保存操作后写入了数据库，再重新做cpalls+ 时，后退处理
                model.currentMeasure().runningTime(model.currentMeasure().measureRunning.runningTime - model.currentItem().pauseTime);
            }
            else
                model.currentMeasure().runningTime(model.currentMeasure().measureRunning.runningTime - model.currentItem().running.runningTime);
        }
        else if (model.currentItem().status() == Cpalls_Status.Finished)//暂停&保存操作后写入了数据库，再重新做cpalls+ 时，后退处理
            model.currentItem().status(Cpalls_Status.Initialised);

        //重置未显示或播放完成的数量
        model.currentItem().unEndAudioCount(model.currentItem().unEndAudioLength);
        model.currentItem().unEndPictureCount(model.currentItem().unEndPictureLength);
        if (model.isWaitingNext)
            model.isWaitingNext = false;
        if (model.isCanNext)
            model.isCanNext = true;
    }

    model.previousItem = function () {
        var divContentWidth = $("#divContent").width();
        execHelper.stopAudio();
        this.resetItem(); //重置当前的 Item 
        this.currentItem().isShowing(false);


        for (var i = model.currentItem().lastItemIndex() + 1; i <= this.currentItemIndex() ; i++) {
            model.currentMeasure().items[i].executed(true);
        }
        this.currentItemIndex(model.currentItem().lastItemIndex());

        this.resetItem(); //重置后退后的 Item
        if (model.currentItem().type == Ade_ItemType.TxkeaExpressive.value
        ) {
            model.currentItem().ShowResponse(false);
        }
        this.currentItem().isShowing(true);
        window.scrollTo(0, 0);
        model.currentItem().timeoutTimer = null;

        $("#divContent").width(divContentWidth);
        $("#divContent").height($("#divContent").width() * (565 / 1024));
        if ($("#divResponseContent").length != 0) {
            $("#divResponseContent").height($("#divContent").height());
        }
        //此处需要重新判断是否显示Next，为了防止暂停后第一次加载时出现next
        if (model.currentItem().type == Ade_ItemType.MultipleChoices.value
            || model.currentItem().type == Ade_ItemType.Pa.value
            || model.currentItem().type == Ade_ItemType.Direction.value
            || model.currentItem().type == Ade_ItemType.Checklist.value
            || model.currentItem().type == Ade_ItemType.TypedResponse.value
            || model.currentItem().type == Ade_ItemType.TxkeaReceptive.value
        ) {

            if ($("#btn_next").length) {
                //此处需要通过measure属性控制 
                if (model.currentMeasure().nextButton) //临时解决方法                    
                    $("#btn_next").show();
                else
                    $("#btn_next").hide();
            }

        }
        else if (model.currentItem().type == Ade_ItemType.TxkeaExpressive.value
            && model.currentMeasure().nextButton == true) {
            if (model.currentItem().status() == 3) {
                if ($("#btn_next").length)
                    $("#btn_next").show();
            }
            else {
                if ($("#btn_next").length) {
                    //此处需要通过measure属性控制 
                    if (model.currentItem().ShowResponse()) //临时解决方法                    
                        $("#btn_next").show();
                    else
                        $("#btn_next").hide();
                }
            }
        }
        else {
            if ($("#btn_next").length)
                $("#btn_next").hide();
        }
    };


    model.showChooseButtons = ko.computed(function () {
        if (this.status() == Exec_Status.going
            && this.currentItem().isShowing()
            && this.currentItem().answerType == Cpalls_AnswerType.YesNo) {
            return true;
        }
        return false;
    }, model);

    model.choose = function (answer, item) {
        var targetItem = model.currentItem();
        if (item && item.answers) {
            targetItem = item;
        }
        if (!targetItem.multiChoice) {
            if (targetItem.type == Ade_ItemType.TxkeaReceptive.value
                && targetItem.selectionType == Txkea_SelectionType.SingleSelect && !model.currentMeasure().nextButton) {
                //TxkeaReceptive类型，单选并且没有next按钮时，只可点击一次，不需要取消选中
            }
            else {
                $.each(targetItem.answers, function (i, a) {
                    a.choosed(false);
                });
                targetItem.selectedAnswers.removeAll();
                if (targetItem.itemLayout && targetItem.itemLayout.ImageList) { //TxkeaReciptive类型绑定的是ImageList
                    $.each(targetItem.itemLayout.ImageList, function (i, a) {
                        a.choosed(false);
                    });
                }
            }

        }

        //Txkea Receptive类型选择需单独控制
        if (targetItem.type == Ade_ItemType.TxkeaReceptive.value && targetItem.selectionType == Txkea_SelectionType.MultiSelect) {
            if (!model.isWaitingNext) { //item将要跳转，但是需要等待response audio播放完成时
                //最大点击次数为正确答案的数量
                if (targetItem.selectedDetailedAnswers.length < targetItem.correctAnswerCount) {
                    var currentIndex = targetItem.selectedDetailedAnswers.length + 1;
                    var targetAnswer = targetItem.answers.filter(function (obj) { return obj.id == answer.id })[0];
                    if (answer.choosed()) {
                        //item选中后不可取消，但是要作为答案加入到选中答案数组中去,并标记是否正确
                        if (targetItem.selectedDetailedAnswers) {
                            var duplicateAnswer = JSON.parse(JSON.stringify(answer));
                            // 同一答案选中并且有一次被计为正确后，再选中时直接计为错误
                            if (answer.isRealCorrect == true ||
                                targetItem.selectedDetailedAnswers.filter(function (obj) {
                                    return obj.id == answer.id && obj.isRealCorrect == true
                            }).length > 0) {
                                duplicateAnswer.isRealCorrect = false;
                            }
                            else {
                                if (currentIndex >= targetAnswer.minSeqNumber && currentIndex <= targetAnswer.maxSeqNumber)
                                    duplicateAnswer.isRealCorrect = true;
                                else
                                    duplicateAnswer.isRealCorrect = false;
                            }
                            targetItem.selectedDetailedAnswers.push(duplicateAnswer);
                        }
                    } else {
                        targetAnswer.choosed(true);
                        answer.choosed(true);
                        model.switchAudio(targetItem, targetAnswer);
                        targetItem.selectedAnswers.push(answer.id);
                        if (targetItem.selectedDetailedAnswers) {  //Txkea Receptive item 
                            if (!answer.isCorrect) {     //错误答案直接设置为false
                                answer.isRealCorrect = false;
                            }
                            else {         //正确答案先标记是否正确                                
                                if (currentIndex >= targetAnswer.minSeqNumber && currentIndex <= targetAnswer.maxSeqNumber)
                                    answer.isRealCorrect = true;
                                else
                                    answer.isRealCorrect = false;
                            }
                            targetItem.selectedDetailedAnswers.push(answer);
                        }
                    }
                }
            }
        }
            //Txkea Receptive类型，单选并且没有next按钮时，选中一个就跳转
        else if (targetItem.type == Ade_ItemType.TxkeaReceptive.value
            && targetItem.selectionType == Txkea_SelectionType.SingleSelect && !model.currentMeasure().nextButton) {
            if (!model.isWaitingNext) { //item将要跳转，但是需要等待response audio播放完成时
                if (!answer.choosed()) {
                    var targetAnswer = targetItem.answers.filter(function (obj) { return obj.id == answer.id })[0];
                    targetAnswer.choosed(true);
                    answer.choosed(true);
                    model.switchAudio(targetItem, targetAnswer);
                    targetItem.selectedAnswers.push(answer.id);
                    if (targetItem.syncAnswer) { //传过来的不是answer对象，需要同步到answer中去
                        if (targetItem.selectedDetailedAnswers) {  //Txkea Receptive item 
                            targetItem.selectedDetailedAnswers.push(answer);
                        }
                    }
                }
            }
        }
        else {
            if (answer.choosed()) {
                answer.choosed(false);
                targetItem.selectedAnswers.remove(answer.id);
                if (targetItem.syncAnswer) { //传过来的不是answer对象，需要同步到answer中去
                    if (targetItem.selectedDetailedAnswers) {  //Txkea Receptive item
                        targetItem.selectedDetailedAnswers = targetItem.selectedDetailedAnswers.filter(function (obj) {
                            return answer.id != obj.id
                        });
                    }
                    targetItem.answers.filter(function (obj) {
                        return obj.id == answer.id
                    })[0].choosed(false);
                }
            } else {
                var targetAnswer = targetItem.answers.filter(function (obj) { return obj.id == answer.id })[0];
                targetAnswer.choosed(true);
                answer.choosed(true);
                model.switchAudio(targetItem, targetAnswer);
                targetItem.selectedAnswers.push(answer.id);
                if (targetItem.syncAnswer) { //传过来的不是answer对象，需要同步到answer中去
                    if (targetItem.selectedDetailedAnswers) {  //Txkea Receptive item 
                        targetItem.selectedDetailedAnswers.push(answer);
                    }
                }
            }
        }

        if (targetItem.type == Ade_ItemType.MultipleChoices.value) {
            if (targetItem.selectedAnswers().length > targetItem.maxAnswerCount && targetItem.maxAnswerCount > 0) {
                var unchoosing = targetItem.selectedAnswers.shift();
                $.each(targetItem.answers, function (_i, _answer) {
                    if (_answer.id == unchoosing) {
                        _answer.choosed(false);
                        return false;
                    }
                });
            }
        }

        if (targetItem.type == Ade_ItemType.Quadrant.value
            || targetItem.type == Ade_ItemType.Receptive.value
            || targetItem.type == Ade_ItemType.ReceptivePrompt.value) {
            if (answer.choosed()) {
                model.nextItem();
            }
        }

        //类型为TxkeaReceptive，可多选并且是做题状态时，会有跳出条件判断
        if (targetItem.type == Ade_ItemType.TxkeaReceptive.value
            && targetItem.selectionType == Txkea_SelectionType.MultiSelect
            && model.mode == Exec_Mode.Exec && !model.isWaitingNext) {
            var inCorrectCount = targetItem.selectedDetailedAnswers.filter
                (function (obj) { return obj.isRealCorrect == false }).length; //选择错误答案的数量

            //如果没有Next按钮，reach the maximum # of attempt (Number of correct answers)时跳转下一个item
            if (!model.currentMeasure().nextButton && targetItem.selectedDetailedAnswers.length >= targetItem.correctAnswerCount) {
                model.keaReceptiveNext();
                return;
            }
            if (targetItem.breakCondition.value == Txkea_BreakCondition.StopCondition) //stop condition
            {
                if (targetItem.selectedDetailedAnswers.length >= targetItem.stopConditionY && targetItem.stopConditionY >= targetItem.stopConditionX > 0) {
                    var curInCorrectCount = 0;
                    //选中答案数量等于Y值时，直接判断错误数量
                    if (targetItem.selectedDetailedAnswers.length == targetItem.stopConditionY || targetItem.stopConditionY == 1) {
                        curInCorrectCount = targetItem.selectedDetailedAnswers.filter
                            (function (obj) { return obj.isRealCorrect == false }).length;
                    }
                        //选中答案数量大于Y值时，判断最近Y值内错误数量
                    else {
                        curInCorrectCount = targetItem.selectedDetailedAnswers.filter
                            (function (obj, index) {
                                return (obj.isRealCorrect == false &&
                                    index >= targetItem.selectedDetailedAnswers.length - targetItem.stopConditionY)
                            }).length;
                    }
                    if (curInCorrectCount >= targetItem.stopConditionX) {
                        model.keaReceptiveNext();
                        return;
                    }
                }
            }
            if (targetItem.breakCondition.value == Txkea_BreakCondition.IncorrectResponse) { //incorrect response
                if (inCorrectCount > 0) {
                    model.keaReceptiveNext();
                    return;
                }
            }
        }

        //如果是单选，并且没有Next按钮时，选中一次就跳到下一个item
        if (targetItem.type == Ade_ItemType.TxkeaReceptive.value
            && targetItem.selectionType == Txkea_SelectionType.SingleSelect
            && model.mode == Exec_Mode.Exec && !model.currentMeasure().nextButton && !model.isWaitingNext) {
            model.keaReceptiveNext();
        }
    };

    //专用于keaReceptive时，跳转到下一题
    model.keaReceptiveNext = function () {

        if (model.isCanNext != false) //是否有audio正在播放
            model.nextItem();
        else //如果有audio正在播放，则会在播放完之后的回调函数中调用model.nextItem();
            model.isWaitingNext = true;
    }

    model.rightNext = function () {
        model.markCurrentItem(true);
        model.nextItem();
    };

    model.wrongNext = function () {
        model.markCurrentItem(false);
        model.nextItem();
    };

    model.markCurrentItem = function (isCorrect) {
        var targetItem = this.currentItem();
        if (isCorrect) {
            targetItem.isCorrect(true);
            targetItem.goal(targetItem.score);
            $.each(targetItem.answers, function (i, ans) {
                if (ans.isCorrect) {
                    targetItem.selectedAnswers.push(ans.id);
                    return false;
                }
            });
        } else {
            targetItem.isCorrect(false);
            targetItem.goal(0);
            targetItem.selectedAnswers.removeAll();
        }
    }

    model.showChecklistPanel = ko.computed(function () {
        return this.visible()
            && this.currentMeasure().showType == Cpalls_Measure_ShowType.List
            && this.status() == Exec_Status.going;
    }, model);
    // next/previous functions:end

    // runnint event
    model.measureRunning = new IntervalTimer(100, function () {
        if (model.currentMeasure().timeout > 0 && model.measureRunning.runningTime + model.currentMeasure().pauseTime > model.currentMeasure().timeout) {
            execHelper.log("measure " + model.currentMeasure().name, "time out.");
            model.pauseTimer();
            if (model.mode === Exec_Mode.Preview) {
                var msg = window.getErrorMessage("CPALLS_Preview_Timeout").replace("{object}", "measure");
                $.when(window.waitingConfirm(msg, "Close", "No")).done(function () {
                    model.close();
                });
            } else if (model.mode === Exec_Mode.Exec) {
                model.currentItem().isShowing(false);
                model.endCurrentMeasure();
            }
        }
        model.runningTime(model.measureRunning.runningTime);
    });
    model.runningTime = ko.observable(0);
    model.timeRemaining = ko.computed(function () {
        var seconds = Math.floor((model.currentMeasure().timeout - model.runningTime() - model.currentMeasure().pauseTime) / 1000);
        if (seconds < 0) {
            seconds = 0;
        }
        var mins = Math.floor(seconds / 60);
        seconds = seconds % 60;
        if (seconds < 10) seconds = "0" + seconds;
        return mins + ":" + seconds;
    }, model);

    model.nextTimer = null;

    model.currentItemIndex.subscribe(function () {
        execHelper.log("Next item");
        //item 前 0.5s 不能点击
        model.currentItem().startClickEvent = new TimeoutTimer(model.currentItem().waitTime);
        $.when(model.currentItem().startClickEvent.timeUp()).done(function () {
            model.currentItem().startClick = 1;
        });
        model.currentItem().startClickEvent.start();

        //取消当前item自动执行事件
        model.nextTimer && model.nextTimer.cancel();
        if (model.currentItem().timeout > 0) {


            execHelper.log("Time out started, will trigger in %s", model.currentItem().timeout);
            model.nextTimer = new TimeoutTimer(model.currentItem().timeout);
            $.when(model.nextTimer.timeUp()).done(function () {

                execHelper.log("Item %s time out.", model.currentItem().label);
                model.currentItem().timeouted = true;
                if (model.mode === Exec_Mode.Preview) {
                    model.pauseTimer();
                    var msg = window.getErrorMessage("CPALLS_Preview_Timeout").replace("{object}", "item");
                    $.when(window.waitingConfirm(msg, "Close", "No")).done(function () {
                        model.close();
                    }).fail(function () {
                        model.startTimer();
                    });
                } else if (model.mode === Exec_Mode.Exec) {
                    model.nextItem();
                }
            });
            model.nextTimer.start();
        }
    });


    model.startTimer = function () {
        model.measureRunning.start();
    };
    model.pauseTimer = function () {
        model.nextTimer && model.nextTimer.cancel();
        model.measureRunning.pause();
    };
    model.switchTimer = function () {
        // 根据当前Item的Timed属性, 设置是否累计Measure时间

        model.visible(true); // 切换Item 需要设置可见
        if (model.currentItem().timed) {
            execHelper.log("current item:", model.currentItem().label, ", start timer.");
            model.startTimer();
        } else {
            execHelper.log("current item:", model.currentItem().label, ", pause timer.");
            model.pauseTimer();
        }
    };

    model.resetRunning = function () {
        model.runningTime(0);
        model.measureRunning.reset();
    };


    // Pause & exit:Start
    model.pause = function () {
        if (model.status() === Exec_Status.going) {
            model.currentItem().isShowing(false);
        }

        model.visible(false);
        model.pauseTimer();
    };

    model.continue = function () {
        model.visible(true);
        //暂停之后再回到该界面时，点击 Quit and Discard 之后的Cancel操作，不应该再显示当前item
        if (model.status() === Exec_Status.going && model.currentItem().status() != Cpalls_Status.Finished) {
            model.currentItem().isShowing(true);
            $("#divContent").height($("#divContent").width() * (565 / 1024));
            if ($("#divResponseContent").length != 0) {
                $("#divResponseContent").height($("#divContent").height());
            }
        }
    };

    model.returnToAssessment = function () {
        if ($.isFunction(this.resumedFromPause)) {
            this.resumedFromPause();
        }
        model.continue();
    };

    model.showPauseButton = ko.computed(function () {
        if (model.currentMeasure().timeout > 0 || model.currentMeasure().orderType === Cpalls_Order.Random) {
            return false;
        }
        return model.status() == Exec_Status.going;
    }, model);

    model.showDiscardButton = ko.computed(function () {
        if (model.currentMeasure().stopButton != true)
            return false;
        return true;
    }, model);

    model.close = function () {
        model.saveExecutedMeasure();

        // iPad 上面的 Safari有时候不能自动关闭标签, 提醒用户手动关闭
        window.close();

        var tips = getErrorMessage("auto_close");
        setTimeout(function () { alert(tips); }, 10);
    };

    // 关闭网页或者跳转到之前页面
    model.goBack = function () {
        if (model.online) {
            if (model.mode == Exec_Mode.Preview) {
                model.close();
            } else {
                var studentViewUrl = Cpalls_Urls.studentView;
                studentViewUrl = studentViewUrl.replace("{classId}", model.classModel.id);
                studentViewUrl = studentViewUrl.replace("{assessmentId}", model.assessmentId);
                studentViewUrl = studentViewUrl.replace("{year}", "20" + model.schoolYear.slice(0, 2));
                studentViewUrl = studentViewUrl.replace("{wave}", model.wave.value);
                if (model.keepSelectdMeasureIds.length > 1) {
                    studentViewUrl += "&measures=" + model.keepSelectdMeasureIds.join(",");
                }
                location.replace(studentViewUrl);
            }
        } else {
            model.close();
        }
    };

    // Quit and Save
    model.saveAndQuit = function () {
        model.pause();

        $.when(waitingConfirm("CPALLS_Go_Back", "Save & Close", "Cancel")).done(function () {
            model.status(Exec_Status.paused);
            model.currentMeasure().status = Cpalls_Status.Paused;
            model.syncServer();
        }).fail(function () {
            model.continue();
        });
    };
    // Quit & Discard
    model.quit = function () {
        model.pause();
        $.when(waitingConfirm("CPALLS_quit", "Discard", "Cancel")).done(function () {
            model.currentMeasure().status = Cpalls_Status.Initialised;
            model.status(Exec_Status.over);
            model.syncServer();
        }).fail(function () {
            model.continue();
        });
    };
    // Pause & exit: end

    // Other Operations
    model.showAllItems = function () {
        if (model.mode == Exec_Mode.View) {
            $.each(model.currentMeasure().items, function (i, item) {
                if (item.scored && item.pauseTime > 0)
                    item.isShowing(true);
            });
        }
    };

    model.showInvalidate = ko.computed(function () {
        if (this.mode === Exec_Mode.View) {
            return this.editable;
        }

        return model.status() === Exec_Status.resulting;
    }, model);

    model.invalidate = function (measureModel) {

        function invalidateOnline() {
            $.post("/Cpalls/Execute/Invalidate", {
                measureId: measureModel.id,
                execAssessmentId: model.execId,
                schoolId: model.schoolId
            },
                function (response) {
                    if (response.success) {
                        model.goBack();
                    } else {
                        showMessage("fail", response.msg);
                    }
                }, 'json');
        }

        function invalidateOffline() {
            measureModel.status = Cpalls_Status.Initialised;
            model.needSaveExecutedMeasure = true;
            model.close();
        }

        function invalidateExec() {
            model.currentMeasure().status = Cpalls_Status.Initialised;
            model.status(Exec_Status.over);
            model.syncServer();
        }

        $.when(waitingConfirm("CPALLS_Invalidate_Measure", "Invalidate", "No")).done(function () {
            if (model.mode == Exec_Mode.View) {
                if (model.online) {
                    invalidateOnline();
                } else {
                    invalidateOffline();
                }
            } else {
                invalidateExec();
            }
        });
    };

    model.needSaveExecutedMeasure = false;
    model.saveExecutedMeasure = function () {
        if (model.online === false && model.needSaveExecutedMeasure) {
            var measuresForOffline = [
                {
                    MeasureId: model.currentMeasure().id,
                    Status: model.currentMeasure().status,
                    Comment: model.currentMeasure().comment()
                }];
            model.cpallsDoneArguments = [
                model, measuresForOffline
            ];
            window.opener && window.opener.cpallsDone && window.opener.cpallsDone.apply(window, model.cpallsDoneArguments);
        }
    };

    model.openComment = function (measureModel) {
        model.commentingMeasure(measureModel);
        if (model.online) {
            var url = "Comment/" + measureModel.execId + '?label=' + encodeURIComponent(measureModel.name);
            $("#modalSmall").modal({ remote: url });
        } else {
            $("#modalComment").modal("show");
        }
    };
    model.commentingMeasure = ko.observable();
    model.updateComment = function (vm, event) {
        model.needSaveExecutedMeasure = true;
        window.commentChanged = true;
        closeModal($(event.target));
    };

    model.includeImages = ko.observable(true);
    model.toggleIncludeImages = function () {
        model.includeImages(!model.includeImages());
    };
    model.getHeaderTemplate = function (itemModel) {
        // in this function this = measureModel that contains itemModel
        if (this.needShowHeader(itemModel)) {
            return "_cpalls_ItemHeader_" + itemModel.type;
        }
        return "Temp_Empty";
    };

    model.exportPdf = function () {
        var $frmPdf = $("#frmPdf");
        var pdfWindow = $frmPdf[0].contentWindow.window;
        var getReady = pdfWindow.getReady;
        if ($.isFunction(getReady)) {
            getReady(model);
        }
        var exportPdf = pdfWindow.exportPdf;
        if ($.isFunction(exportPdf)) {
            exportPdf();
        } else {
            window.showMessage("fail", "exportPdf_Failed");
        }
    };

    model.showCommentBtn = function () {
        return model.mode === Exec_Mode.View;
    };
    // Other Operations:end

    model.init = function () {
        if (model.mode === Exec_Mode.Exec || model.mode === Exec_Mode.Preview) {
            model.startCurrentMeasure();
        } else if (model.mode === Exec_Mode.View) {
            model.visible(true);
        }
        model.commentingMeasure(model.currentMeasure());
    };

    // Score, Post to server
    model.calculateGoal = function () {
        // 计算得分,答案
        if (model.currentMeasure().goal() < 0) {
            model.currentMeasure().goal(0);
        }
        var itemGoal = 0;
        var measureGoal = 0;
        $.each(model.currentMeasure().items, function (i, item) {
            itemGoal = 0;
            switch (item.type) {
                case Ade_ItemType.Direction.value:
                    item.isCorrect(false);
                    break;
                case Ade_ItemType.Checklist.value:
                    itemGoal = null;
                    item.isCorrect(false);
                    item.selectedAnswers.removeAll();
                    $.each(item.answers, function (j, answer) {
                        if (answer.choosed()) {
                            item.selectedAnswers.push(answer.id);
                            itemGoal += answer.score;
                        }
                    });
                    break;
                case Ade_ItemType.MultipleChoices.value:
                    item.isCorrect(false);
                    item.selectedAnswers.removeAll();
                    $.each(item.answers, function (j, answer) {
                        if (answer.choosed()) {
                            item.selectedAnswers.push(answer.id);
                            itemGoal += answer.score;
                        }
                    });
                    break;
                case Ade_ItemType.Pa.value:
                    item.isCorrect(false);
                    var rightCount = 0;
                    $.each(item.answers, function (j, answer) {
                        if (answer.choosed()) {
                            itemGoal += answer.score;
                            rightCount++;
                        }
                    });
                    if (rightCount == item.answers.length) {
                        item.isCorrect(true);
                    }
                    break;
                case Ade_ItemType.Quadrant.value:
                case Ade_ItemType.Receptive.value:
                case Ade_ItemType.ReceptivePrompt.value:
                    item.isCorrect(false);
                    $.each(item.answers, function (j, answer) {
                        if (answer.choosed() && answer.isCorrect) {
                            itemGoal = item.score;
                            item.isCorrect(true);
                            return false;
                        }
                    });
                    break;
                case Ade_ItemType.RapidExpressive.value:
                    if (item.isCorrect()) {
                        itemGoal = item.score;
                    }
                    break;
                case Ade_ItemType.TypedResponse.value:
                    itemGoal = item.getGoal();
                    break;
                case Ade_ItemType.TxkeaExpressive.value:
                    itemGoal = item.getGoal();
                    break;
                case Ade_ItemType.TxkeaReceptive.value:
                    itemGoal = item.getGoal();
                    break;
                default:
                    break;
            }
            if (item.scored) {
                if (itemGoal != null) {
                    //例如：当分数为12.2+0.2时，结果为12.399999999999999
                    itemGoal = Number(itemGoal.toFixed(2));
                    if (itemGoal >= 0)
                        measureGoal += itemGoal;
                }
            }
            item.goal(itemGoal);
        });
        model.currentMeasure().goal(Number(measureGoal.toFixed(2)));
    };
    model.syncServer = function () {
        if ($.isFunction(model.onSyncing)) {
            model.onSyncing();
        }
        if (model.student.id == 0) {
            model.status(Exec_Status.saved);
            model.goBack();
            return;
        }

        if (model.mode != Exec_Mode.Exec) {
            model.status(Exec_Status.saved);
            model.goBack();
            return;
        }
        var postStrings = "";
        var items = [];
        var measures = [];
        var postMeasures = "";

        $.each(model.measures, function (meaIndex, measure) {
            if (!measure.readonly) {
                //invalidate 的measure 不进入 
                if (measure.status < 2) return;

                var pausedTime = 0;
                $.each(measure.items, function (itemIndex, item) {
                    var completed = model.status() >= Exec_Status.over;
                    // Save Finished Items: 以前做过的, 和本次做过的。从第三个退回第二个保存时，第三个状态设置为未完成
                    var executedItem = meaIndex <= model.currentMeasureIndex()
                        && (item.pauseTime > 0 || item.running.runningTime > 0) && (itemIndex <= model.currentItemIndex() || meaIndex < model.currentMeasureIndex());
                    // Save All Items when pause Checklist
                    var checkListMeasure = measure.showType == Cpalls_Measure_ShowType.List && item.selectedAnswers().length;
                    if (completed || executedItem || checkListMeasure) {
                        var tempItem = {
                            ItemId: item.itemId,
                            IsCorrect: item.isCorrect(),
                            SelectedAnswers: item.selectedAnswers().join(","),
                            Goal: item.goal(),
                            Scored: item.scored,
                            PauseTime: item.running.runningTime || item.pauseTime,
                            Status: Cpalls_Status.Finished,
                            Executed: item.executed(),
                            LastItemIndex: item.lastItemIndex(),
                            ResultIndex: item.resultIndex
                        };
                        if (item.type === Ade_ItemType.TypedResponse.value) {
                            var details = {};
                            $.each(item.responses, function (index, response) {
                                details[response.id] = {
                                    Content: response.content(),
                                    IsCorrect: response.isCorrect()
                                };
                            });
                            tempItem.Details = JSON.stringify(details);
                        }
                        else if (item.type === Ade_ItemType.TxkeaExpressive.value) {
                            var details = {};
                            $.each(item.responses(), function (j, response) {
                                if (response.content() != "") {
                                    details[response.ID] = [];
                                    if (response.Type == 1)
                                        details[response.ID].push({ optionId: 0, othertxt: response.content() });
                                    else {
                                        $.each(response.Options(), function (k, option) {
                                            if (option.IsCorrect()) {
                                                details[response.ID].push({ optionId: option.ID, othertxt: option.otherText() });
                                            }
                                        });
                                    }
                                }
                            });
                            tempItem.Details = JSON.stringify(details);
                        }
                        else if (item.type === Ade_ItemType.TxkeaReceptive.value) {  //选择答案为点击的答案  
                            tempItem.Scored = 1;
                            if (item.selectionType == Txkea_SelectionType.MultiSelect) {
                                var selectedAnswerIds = '';
                                var details = [];
                                item.selectedDetailedAnswers.forEach(
                                    function (obj) {
                                        selectedAnswerIds += obj.id + ',';
                                        details.push({ id: obj.id, isRealCorrect: obj.isRealCorrect });
                                    });
                                if (selectedAnswerIds)
                                    selectedAnswerIds = selectedAnswerIds.substring(0, selectedAnswerIds.length - 1);

                                tempItem.SelectedAnswers = selectedAnswerIds;
                                tempItem.Details = JSON.stringify(details);
                            }
                        }

                        items.push(tempItem);
                        pausedTime += tempItem.PauseTime;
                    }
                });
                var measureToUpdate = {
                    MeasureId: measure.id,
                    PauseTime: pausedTime,
                    Items: items,
                    Status: Cpalls_Status.Finished,
                    Comment: measure.comment(),
                    TotalScore: measure.totalScore
                }
                if (measure.id === model.currentMeasure().id && model.status() === Exec_Status.paused) {
                    measureToUpdate.Status = Cpalls_Status.Paused;
                }
                measures.push(measureToUpdate);
            }
        });
        execHelper.log(measures, items);
        if (model.online) {
            measures = $.map(measures, function (m) {
                return {
                    MeasureId: m.MeasureId,
                    PauseTime: m.PauseTime,
                    Comment: m.Comment,
                    TotalScore: m.TotalScore
                };
            });
            postStrings = JSON.stringify(items);
            postMeasures = JSON.stringify(measures);
            var serverUrl = model.status() == Exec_Status.over ? "/Cpalls/Execute/Items" : "/Cpalls/Execute/Pause";
            execHelper.log("sending xhr: %s ...", serverUrl);
            jQuery.post(serverUrl, {
                execAssessmentId: model.execId,
                schoolId: model.schoolId,
                measures: postMeasures,
                items: postStrings,
                studentBirthday: model.student.birthday,
                wave: model.wave.value,
                schoolYear: model.schoolYear
            }, function (response) {
                if (response.success) {
                    model.status(Exec_Status.saved);
                    model.goBack();
                } else {
                    showMessage("fail", response.msg);
                }
            }, 'json');
        } else {
            model.cpallsDoneArguments = [model, measures];
            model.status(Exec_Status.saved);
            window.opener && window.opener.cpallsDone && window.opener.cpallsDone.apply(null, model.cpallsDoneArguments);
            model.close();
        }
    };

    // Utils
    model.fullscreen = function () {
        execHelper.switchFullscreen();
    };
    // Keyboard Events
    model.bindKeyboardEvents = function () {
        var $body = $("body");
        if (!$body.data("CPALLS_EVENTS")) {
            $body.on("keyup", function (event) {
                if (model.showChooseButtons()) {
                    if (model.visible()) {
                        if (event.which == keyboard_event_code.left) {
                            model.wrongNext();
                        }
                        if (event.which == keyboard_event_code.right) {
                            model.rightNext();
                        }
                    } else {
                        if (event.which == keyboard_event_code.left) {
                            model.markCurrentItem(false);
                        }
                        if (event.which == keyboard_event_code.right) {
                            model.markCurrentItem(true);
                        }

                    }
                } else if (model.showNextButton()) {
                    if (event.which == keyboard_event_code.right
                        && model.visible()
                    ) {
                        model.next();
                    }
                }
            });
            $body.data("CPALLS_EVENTS", true);
        };
    };

    model.characterLimit = function (data, event) {
        var $tmpComment = $(event.currentTarget);
        if ($tmpComment.val().length >= 150) {
            $tmpComment.next().css("display", "");
        }
        else {
            $tmpComment.next().css("display", "none");
        }
    };

    model.switchAudio = function (targetItem, targetAnswer) {
        //停止所有正在播放的answer responseAudio
        targetItem.answers.forEach(function (obj) {
            if (obj.responseAudio && obj.responseAudio.file && obj.responseAudio.player)
                obj.responseAudio.player.currentTime = obj.responseAudio.player.duration;
        });
        if (targetAnswer && targetAnswer.responseAudio && targetAnswer.responseAudio.file) {
            if (targetAnswer.responseAudio.player) {
                model.isCanNext = false;//是否可以进入下一题
                model.lastAnswerId = targetAnswer.id; //记录最后点击的answerID,用于避免点击下一题时同上一题的audio有重叠
                execHelper.overrideAudioSrc(targetAnswer.responseAudio);
                targetAnswer.responseAudio.player.onended = function () {
                    if (targetAnswer.id == model.lastAnswerId) {  //最后一次点击的播放完才跳转
                        model.isCanNext = true;
                        if (model.isWaitingNext) //是否要跳到下一个item
                        {
                            model.isWaitingNext = false;
                            model.nextItem();
                        }
                    }
                };
                targetAnswer.responseAudio.player.play(); //选中之后播放ResponseAudio               
            }
        }
    };
    return model;
}