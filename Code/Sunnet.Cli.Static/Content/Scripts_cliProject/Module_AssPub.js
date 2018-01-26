var Ade_ItemType = {
    Direction: { value: 9, text: "Direction", needCorrectAnswer: false },
    Cec: { value: 1, text: "CEC", needCorrectAnswer: false },
    Cot: { value: 2, text: "COT", needCorrectAnswer: false },
    MultipleChoices: { value: 3, text: "Multiple Choices", needCorrectAnswer: false, multiCorrectAnswer: true },
    Pa: { value: 4, text: "PA", needCorrectAnswer: false },
    Quadrant: { value: 5, text: "Quadrant", needCorrectAnswer: true },
    RapidExpressive: { value: 6, text: "Rapid Expressive", needCorrectAnswer: false },
    Receptive: { value: 7, text: "Receptive without prompt", needCorrectAnswer: true },
    ReceptivePrompt: { value: 8, text: "Receptive with prompt", needCorrectAnswer: true },
    Checklist: { value: 10, text: "Checklist", needCorrectAnswer: false },
    TypedResponse: { value: 11, text: "Typed Response", needCorrectAnswer: false },
    /*
         [Description("Multiple/Single Choice")]
        ObservableChoice = 14,
        [Description("Text Entry")]
        ObservableResponse = 15 
    */
    ObservableChoice: { value: 14, text: "Multiple/Single Choice", needCorrectAnswer: false },
    ObservableResponse: { value: 15, text: "Text Entry", needCorrectAnswer: false },
    TxkeaReceptive: {value: 12, text: "TX-KEA Receptive", needCorrectAnswer: false },
    TxkeaExpressive: {value: 13, text: "TX-KEA Expressive", needCorrectAnswer: false },
    getByKey: function (key) {
        var target = {};
        $.each(this, function (index, item) {
            if (item.value == key) {
                target = item;
                return false;
            }
        });
        return target;
    }
};


var TypedResponseType = {
    Text: 1,
    Numerical: 2,
    "1": "Text",
    "2": "Numercial"
};

var Cpalls_Urls = {
    studentView: "/Cpalls/Student?classId={classId}&assessmentId={assessmentId}&year={year}&wave={wave}",
    offline: "/Offline",
    executeOnline: "/Cpalls/Execute/Go",
    executeOffline: "/Cpalls/Execute/Go?offline=true",
    checkOnline: "/Offline/Index/Online",
    sync: "/Offline/Index/Sync"
};
var Timer_Type = {
    timeout: 0,
    interval: 1
};
var Timer_Status = {
    ready: 0,
    going: 1,
    paused: 2,
    competed: 3,
    executed: 4,
    cancelled: 5
};
function Timer() {
    this.type = "";
    this.timeout = 0;
    this.interval = 0;
    this.status = Timer_Status.ready;

    this.eventHandler = 0;

    this.runningTime = 0;
    this.runningEvent = 0;

    this.startOn = 0;
    this.lastTriggerOn = 0;
}

Timer.prototype.init = function (type, ms, callback) {
    if (type == Timer_Type.timeout) {
        this.type = Timer_Type.timeout;
        this.timeout = ms;
    }
    else if (type == Timer_Type.interval) {
        this.type = Timer_Type.interval;
        this.interval = ms;
        this.intervalCallback = callback;
    }
}

function TimeoutTimer(timeout) {
    var that = this;

    this.type = "";
    this.timeout = 0;
    this.interval = 0;
    this.status = Timer_Status.ready;

    this.eventHandler = 0;

    this.runningTime = 0;
    this.runningEvent = 0;

    this.startOn = 0;
    this.lastTriggerOn = 0;

    this.init(Timer_Type.timeout, timeout);
    var _d = $.Deferred();
    this.timeUp = function () {
        return _d.promise();
    };

    this.start = function () {
        that.startOn = that.lastTriggerOn = new Date();
        //console.log("start", that.startOn, "timeout:", this.timeout - this.runningTime);
        this.status = Timer_Status.going;
        this.eventHandler = setTimeout(function () {
            //console.log("end", new Date(), new Date() - that.startOn, "runningTime:", that.runningTime, "timeout:", that.timeout);
            that.status = Timer_Status.competed;
            _d.resolve();
        }, this.timeout - this.runningTime);

        this.runningEvent = setInterval(function () {
            //console.log("trigger", (new Date() - that.lastTriggerOn));
            that.runningTime += 100;
            that.lastTriggerOn += 100;
        }, 100);
    };
    this.pause = function () {
        this.status = Timer_Status.paused;
        clearInterval(this.runningEvent);
        clearTimeout(this.eventHandler);
    };
    this.cancel = function () {
        this.status = Timer_Status.cancelled;
        clearInterval(this.runningEvent);
        clearTimeout(this.eventHandler);
        _d.reject();
    };
}

TimeoutTimer.prototype = new Timer();

function IntervalTimer(interval, callback) {
    var that = this;

    this.type = "";
    this.timeout = 0;
    this.interval = 0;
    this.status = Timer_Status.ready;

    this.eventHandler = 0;

    this.runningTime = 0;
    this.runningEvent = 0;

    this.startOn = 0;
    this.lastTriggerOn = 0;


    this.init(Timer_Type.interval, interval, callback);

    this.start = function () {
        if (that.status != Timer_Status.going) {
            clearInterval(this.eventHandler);
            clearInterval(this.runningEvent);

            that.startOn = that.lastTriggerOn = new Date();
            //console.log("start", that.startOn);
            this.status = Timer_Status.going;
            this.eventHandler = setInterval(function () {
                //console.log("trigger", new Date() - that.lastTriggerOn, that.runningTime);
                if ($.isFunction(that.intervalCallback)) {
                    that.intervalCallback();
                };
                that.status = Timer_Status.executed;
                that.lastTriggerOn = new Date();
            }, this.interval);

            this.runningEvent = setInterval(function () {
                that.runningTime += that.interval;
            }, this.interval);
        } else {
            // if forgot to stop timer
            clearInterval(that.runningEvent);
            clearInterval(that.eventHandler);
        }
    };
    this.pause = function () {
        //console.log("pause", new Date(), new Date() - that.lastTriggerOn, that.runningTime);
        that.status = Timer_Status.paused;
        clearInterval(that.runningEvent);
        clearInterval(that.eventHandler);
    };

    this.reset = function () {
        //console.log("reset", new Date(), that.runningTime);
        that.status = Timer_Status.ready;
        that.runningTime = 0;
        clearInterval(that.runningEvent);
        clearInterval(that.eventHandler);
    };
}

IntervalTimer.prototype = new Timer();



function GetOrderProcessor(total, type) {
    var order;
    if (type == Cpalls_Order.Random) {
        order = new RandomOrderProcessor();
    } else {
        order = new OrderedOrderProcessor();
    }
    order.init(total);
    return order;
}

function OrderProcessor() {
    this.total = 0;
    this.indexes = [];
    this.processed = [];
};

OrderProcessor.prototype.init = function (total) {
    this.total = total;
    for (var i = total; i > 0; i--) {   
        this.indexes.push(i - 1);
    }
    this.processed.length = 0;
};

function RandomOrderProcessor() {
    this.total = 0;
    this.indexes = [];
    this.processed = [];

    this.next = function () {
        var target;
        if (this.indexes.length == 0) {
            return target;
        }
        if (this.processed.length == 0 && this.indexes.length == 1) {
            target = this.indexes.pop();
        } else {
            var i = Math.floor(Math.random() * 100 % this.indexes.length);
            target = this.indexes.splice(i, 1).pop();
        }
        this.processed.push(target);
        return target;
    };
}

function OrderedOrderProcessor() {
    this.total = 0;
    this.indexes = [];
    this.processed = [];

    this.next = function () {
        var target = this.indexes.pop();
        if (target) {
            this.processed.push(target);
        }
        return target;
    };

}

RandomOrderProcessor.prototype = new OrderProcessor();
OrderedOrderProcessor.prototype = new OrderProcessor();


function WaveMeasures(groups, checkWaves) {
    if (arguments.length == 1) {
        checkWaves = true;
    }

    var that = this;
    this.waves = {};
    this.wavesArr = [];
    for (var id in groups.waves) {
        if (groups.waves.hasOwnProperty(id)) {
            var wave = $.extend({}, groups.waves[id]);
            if (wave) {
                wave.count = ko.observable(0);
                wave.selectedMeasures = ko.observableArray([]);
                wave.percentage = ko.computed(function () {
                    var p = this.selectedMeasures().length * 100 / this.count();
                    if (isNaN(p)) {
                        return "0%";
                    }
                    if (this.selectedMeasures().length && p < 15) {
                        p = 15;
                    }
                    return p + "%";
                }, wave);
                wave.selected = ko.observable(false);
                that.waves[id] = wave;
                that.wavesArr.push(wave);
            }
        }
    }

    this.groups = [];
    for (id in groups.groups) {
        if (groups.groups.hasOwnProperty(id)) {
            var group = groups.groups[id];
            if (group) {
                group.allSelected = {};
                $.each(that.waves, function (id, wave) {
                    group.allSelected[id] = ko.observable(false);
                });

                group.visible = {};
                $.each(that.waves, function (id, wave) {
                    if (group.parent) {
                        group.visible[wave.id] = true;
                    } else {
                        group.visible[wave.id] = false;
                    }
                });
                $.each(group.measures, function (i2, measure) {
                    measure.selectedWaves = ko.observableArray([]);
                    $.each(that.waves, function (id, wave) {
                        if (measure.applyToWave.indexOf(wave.id) >= 0) {
                            wave.count(wave.count() + 1);
                            group.visible[wave.id] = true;
                        }
                    });

                });

                that.groups.push(group);

            }
        }
    }

    this.selectedWave = ko.observable(1);
    this.chooseWave = function (wave) {
        that.selectedWave(wave);
    };
    this.chooseGroup = function (group) {
        if (group.allSelected[that.selectedWave()]()) {
            $.each(group.measures, function (k, measure) {
                if (measure.selectedWaves().indexOf(that.selectedWave()) >= 0) {
                    measure.selectedWaves.remove(that.selectedWave());
                    that.waves[that.selectedWave()].selectedMeasures.remove(measure.id);
                }
            });
            group.allSelected[that.selectedWave()](false);
        } else {
            $.each(group.measures, function (k, measure) {
                if (measure.applyToWave.indexOf(that.selectedWave()) >= 0 &&
                    measure.selectedWaves().indexOf(that.selectedWave()) < 0) {
                    measure.selectedWaves.push(that.selectedWave());
                    that.waves[that.selectedWave()].selectedMeasures.push(measure.id);
                }
                if ($("#percentileRank").prop("checked")) {
                    $("[isRank='0']").find("a").each(function () {
                        if ($(this).attr("class") == "term-contents term-1") {
                            $(this).click();
                        }
                    });
                }
            });
            group.allSelected[that.selectedWave()](true);
        }
    };

    this.chooseMeasure = function (measure, group) {
        if (measure.applyToWave.indexOf(that.selectedWave()) >= 0 || !checkWaves) {
            if (measure.selectedWaves().indexOf(that.selectedWave()) >= 0) {
                measure.selectedWaves.remove(that.selectedWave());
                that.waves[that.selectedWave()].selectedMeasures.remove(measure.id);
            } else {
                measure.selectedWaves.push(that.selectedWave());
                that.waves[that.selectedWave()].selectedMeasures.push(measure.id);
            }
        }
    };
}

function scoreModel(assessmentId, scoreId, scoreName, scoreDomain) {
    this.AssessmentId = assessmentId;
    this.ScoreId = scoreId;
    this.ScoreName = scoreName;
    this.ScoreDomain = scoreDomain;
    this.IsSlected = ko.observable(false);
}
function ScoreViewModel(scores) {
    var self = this;
    this.scores = ko.observableArray([]);
    this.haveScores = ko.observable(false);
    this.allScoreSelected = ko.observable(false);
    this.scoreClick = function (item) {
        item.IsSlected(!item.IsSlected());
        self.allScoreSelected(false);
        if (item.IsSlected()) {
            var allSelected = self.scores().every(function (item, index) {
                return item.IsSlected() == true;
            });
            if (allSelected) {
                self.allScoreSelected(true);
            }
        }
    };
    this.selectAllScores = function () {
        if (self.allScoreSelected()) {
            $.each(self.scores(), function (i, e) {
                e.IsSlected(false);
            });
        } else {
            $.each(self.scores(), function (i, e) {
                e.IsSlected(true);
            });
        }
        self.allScoreSelected(!self.allScoreSelected());
    };

    this.init = function () {
        if (scores && scores.length) {
            self.haveScores(true);
            $.each(scores, function (i, e) {
                self.scores.push(new scoreModel(e.AssessmentId, e.ScoreId, e.ScoreName, e.ScoreDomain));
            });
        }
    }
    this.init();
}

