function getDateString(strDate, format) {
    if (!format) {
        format = "MM/dd/yyyy";
    }
    if ("01/01/0001" == strDate)
        return "";
    var date = parseDate(strDate, format);
    var minDate = parseDate("01/01/2014");
    if (date > minDate) {
        return date.Format(format);
    }
    return "";
}
var CotStatus = {
    Wave: {
        Initialised: 1,
        Saved: 10,
        Finalized: 20
    },
    StgReport: {
        Initialised: 1,
        Saved: 10,
        Completed: 20,
        /// <summary>
        /// Reset Short Term Goals
        /// </summary>
        Deleted: 40
    }
}
var CotWave = {
    None: 0,
    BOY: 1,
    MOY: 2
}

function CotAssessment(defaultValues) {
    var that = this;
    this.id = window.isNull("ID", defaultValues, 0);
    this.teacherId = window.isNull("TeacherId", defaultValues, 0);
    this.assessmentId = window.isNull("AssessmentId", defaultValues, 0);
    this.schoolYear = window.isNull("SchoolYear", defaultValues, 0);
    this.measures = [];
    var measures = window.isNull("Measures", defaultValues, []);
    for (var i = 0; i < measures.length; i++) {
        this.measures.push(new CotMeasure(measures[i]));
    }

    this.showFullTargetText = ko.observable(false);
    this.toggleFullTargetText = function () {
        that.showFullTargetText(!that.showFullTargetText());
    }

    this.showActiveGoalsOnly = ko.observable(false);
    this.toggleActiveGoals = function () {
        that.showActiveGoalsOnly(!that.showActiveGoalsOnly());
    };

    this.prapare = function (isCreateStgReport) {
        var items = [];
        var savedItems = [];
        var i, j, k, item, itemForUpdate;
        for (i = 0; i < this.measures.length; i++) {
            var measure = this.measures[i];
            if (measure.items && measure.items.length) {
                for (j = 0; j < measure.items.length; j++) {
                    item = measure.items[j];
                    item.saved(false);
                    itemForUpdate = item.itemForUpdate(isCreateStgReport);
                    if (itemForUpdate) {
                        items.push(itemForUpdate);
                        savedItems.push(item);
                    }
                }
            }
            if (measure.children && measure.children.length) {
                for (k = 0; k < measure.children.length; k++) {
                    var child = measure.children[k];
                    if (child.items && child.items.length) {
                        for (j = 0; j < child.items.length; j++) {
                            item = child.items[j];
                            item.saved(false);
                            itemForUpdate = item.itemForUpdate(isCreateStgReport);
                            if (itemForUpdate) {
                                items.push(itemForUpdate);
                                savedItems.push(item);
                            }
                        }
                    }
                }
            }
        }
        return {
            itemsForUpdate: items,
            savedItems: savedItems
        };
    };
};

function CotMeasure(defaultValues) {
    this.id = window.isNull("ID", defaultValues, 0);
    this.visible = window.isNull("Visible", defaultValues, false);
    this.measureId = window.isNull("MeasureId", defaultValues, 0);
    this.name = window.isNull("Name", defaultValues, "");
    this.children = false;
    this.parent = false;
    this.items = false;

    var items = window.isNull("Items", defaultValues, false);
    if (items && items.length) {
        this.items = [];
        for (var i = 0; i < items.length; i++) {
            this.items.push(new CotItem(items[i]));
        }
    }


    var children = window.isNull("Children", defaultValues, false);
    if (children && children.length) {
        this.children = [];
        for (var i = 0; i < children.length; i++) {
            var child = new CotMeasure(children[i]);
            child.parent = this;
            this.children.push(child);
        }
    }
    this.links = [];
    if (defaultValues.Links && defaultValues.Links.length) {
        for (var i = 0; i < defaultValues.Links.length; i++) {
            this.links.push({
                text: defaultValues.Links[i].DisplayText,
                url: defaultValues.Links[i].Link
            });
        }
    };

    this.isActive = ko.computed(function () {
        if (this.visible) {
            var isShow = false;
            if (this.children) {
                $.each(this.children, function (index, mea) {
                    if (mea.isActive()) {
                        isShow = true;
                        return true;
                    }
                });

                if (isShow == false) {
                    if (this.items) {
                        $.each(this.items, function (index, opinion) {
                            if (opinion.isActive()) {
                                isShow = true;
                                return true;
                            }
                        });
                    }
                }
                return isShow;
            } else {
                $.each(this.items, function (index, opinion) {
                    if (opinion.isActive()) {
                        isShow = true;
                        return true;
                    }
                });
                return isShow;
            }
        }
        return false;
    }, this);
};

function CotItem(defaultValues) {
    var that = this;
    this.id = window.isNull("ID", defaultValues, 0);
    this.cotAssessmentId = window.isNull("CotAssessmentId", defaultValues, 0);
    this.itemId = window.isNull("ItemId", defaultValues, 0);
    this.cotItem = "<span class='note'>&nbsp;" + window.isNull("CotItemId", defaultValues, "Not set") + "</span>";
    this.level = window.isNull("Level", defaultValues, {});
    this.shortTargetText = window.isNull("ShortTargetText", defaultValues, "");
    this.fullTargetText = window.isNull("FullTargetText", defaultValues, "");
    this.goalSetDate = getDateString(window.isNull("GoalSetDate", defaultValues, ""));
    this.goalMetDate = ko.observable(getDateString(window.isNull("GoalMetDate", defaultValues, "")));


    this.boyObsDate = getDateString(window.isNull("BoyObsDate", defaultValues, ""));
    this.moyObsDate = getDateString(window.isNull("MoyObsDate", defaultValues, ""));
    this.cotUpdatedOn = ko.observable(getDateString(window.isNull("CotUpdatedOn", defaultValues, "")));
    this.goalMetDone = window.isNull("GoalMetDone", defaultValues, false);

    this.circleManual = window.isNull("CircleManual", defaultValues, "");
    this.mentoringGuide = window.isNull("MentoringGuide", defaultValues, "");
    this.prekindergartenGuidelines = window.isNull("PrekindergartenGuidelines", defaultValues, "");

    this.highlight = ko.computed(function () {
        var threeDate = (this.boyObsDate + this.moyObsDate + this.cotUpdatedOn()).length > 0;
        return threeDate || this.goalMetDone;
    }, that);

    // cot assessment
    this.observed = ko.observable(false);
    this.observed.subscribe(function (observed) {
        if (!observed)
            that.needSupport(false);
    });
    if (this.boyObsDate || this.moyObsDate) {
        this.observed(true);
    }

    // cot report
    this.changed = ko.observable(false);
    this.saved = ko.observable(false);
    this.highlightAfterSave = ko.computed(function () {
        return this.saved();
    }, this);
    this.waitingGoalMet = ko.observable(window.isNull("WaitingGoalMet", defaultValues, false));
    this.needSupport = ko.observable(window.isNull("NeedSupport", defaultValues, false));
    this.needSupportEnabled = ko.computed(function () {
        return (this.boyObsDate + this.moyObsDate + this.cotUpdatedOn()).length > 0;
    }, that);

    this.cotUpdatedOn.subscribe(function (newDate) {
        if (newDate) {
            that.needSupport(false);
        } else {
            that.needSupport(true);
        }
        that.changed(true);
    });

    this.needSupport.subscribe(function () {
        that.changed(true);
    });
    this.goalMetDate.subscribe(function (newDate) {
        if (newDate) {
            that.waitingGoalMet(false);
        } else {
            that.waitingGoalMet(true);
        }
        that.changed(true);
    });
    this.showMetDateBox = ko.computed(function () {
        //var onlyHaveSetDate = this.goalSetDate.length && !this.goalMetDate().length;
        var onlyHaveSetDate = this.goalSetDate.length && (!this.goalMetDate().length || (this.goalMetDate().length && that.changed()));
        if (onlyHaveSetDate) {
            return true;
        }
        return false;
    }, this);
    this.isActive = ko.computed(function () {
        var onlyHaveSetDate = this.goalSetDate.length && !this.goalMetDate().length;
        var needSupport = this.needSupport();
        return onlyHaveSetDate || needSupport || that.changed();
    }, this);

    this.itemForUpdate = function (isCreateStgReport) {
        var newItem;
        if (that.changed() || (isCreateStgReport === true && that.waitingGoalMet())) {
            newItem = {
                ItemId: that.itemId,
                NeedSupport: that.needSupport(),
                CotUpdatedOn: that.cotUpdatedOn() || "1/1/1753",
                GoalMetDate: that.goalMetDate() || "1/1/1753"
            };
            return newItem;
        }
        return false;
    };

    // stg report
    this.sort = window.isNull("Sort", defaultValues, 0);
    this.goalMetAble = window.isNull("GoalMetAble", defaultValues, false);
    this.goalMetAbled = false;
    if (this.goalMetDate) {
        {
            this.goalMetAbled = true;
            this.goalMetAble = false;
        }
    }

    this.visible = true;
    this.links = [];
    if (defaultValues.Links && defaultValues.Links.length) {
        for (var i = 0; i < defaultValues.Links.length; i++) {
            this.links.push({
                text: defaultValues.Links[i].DisplayText,
                url: defaultValues.Links[i].Link
            });
        }
    }
}

function ContainsItem(sourceItems, selectedItemIds) {
    var contain = false;
    if (sourceItems && sourceItems.length) {
        for (var i = 0; i < sourceItems.length; i++) {
            if (selectedItemIds.indexOf(sourceItems[i].ItemId) >= 0 || selectedItemIds.indexOf(sourceItems[i].itemId) >= 0) {
                contain = true;
                break;
            }
        }
    }
    return contain;
}

function GetItemByItemId(sourceItems, itemId) {
    var item = null;
    if (sourceItems && sourceItems.length) {
        for (var i = 0; i < sourceItems.length; i++) {
            if (itemId == sourceItems[i].ItemId || itemId == sourceItems[i].itemId) {
                item = sourceItems[i];
                break;
            }
        }
    }
    return item;
}

function CotWaveModel(waveItems, defaultValues) {
    var that = this;
    var assessmentEntity = JSON.parse(localStorage.getItem(getCotOfflineApp.assessment));
    this.spentTime = window.isNull("SpentTime", defaultValues, "");
    this.status = window.isNull("Status", defaultValues, { value: CotStatus.Wave.Initialised, text: "Initialised" });
    this.cotAssessmentId = window.isNull("CotAssessmentId", defaultValues, 0);
    this.visitDate = getDateString(window.isNull("VisitDate", defaultValues, ""));
    this.wave = window.isNull("Wave", defaultValues, {});
    this.measures = [];

    var length = assessmentEntity.Measures.length;
    for (var i = 0; i < length; i++) {
        var measureEntity = assessmentEntity.Measures[i];

        if (measureEntity.Items && (itemsLength = measureEntity.Items.length)) {
            for (var itemIndex = 0; itemIndex < itemsLength; itemIndex++) {
                var itemEntity = measureEntity.Items[itemIndex];
                var itemRecord = GetItemByItemId(waveItems, itemEntity.ItemId);
                if (itemRecord) {
                    $.extend(itemEntity, itemRecord);
                }
            }
        }
        var childrenLength;
        if (measureEntity.Children && (childrenLength = measureEntity.Children.length)) {
            for (var childIndex = 0; childIndex < childrenLength; childIndex++) {
                if (measureEntity.Children[childIndex] && measureEntity.Children[childIndex].Items
                    && (itemsLength = measureEntity.Children[childIndex].Items.length))
                    for (itemIndex = 0; itemIndex < itemsLength; itemIndex++) {
                        itemEntity = measureEntity.Children[childIndex].Items[itemIndex];
                        itemRecord = GetItemByItemId(waveItems, itemEntity.ItemId);
                        if (itemRecord) {
                            $.extend(itemEntity, itemRecord);
                        }
                    }
            }
        }
        var measureModel = new CotMeasure(measureEntity);
        this.measures.push(measureModel);
    }
}

function CotStgReportModel(teacherItemEntities, defaultValues) {
    var assessmentEntity = JSON.parse(localStorage.getItem(getCotOfflineApp.assessment));

    this.id = window.isNull("ID", defaultValues, 0);
    this.additionalComments = window.isNull("AdditionalComments", defaultValues, "");
    this.onMyOwn = window.isNull("OnMyOwn", defaultValues, "");
    this.withSupport = window.isNull("WithSupport", defaultValues, "");

    this.spentTime = window.isNull("SpentTime", defaultValues, "");
    this.cotAssessmentId = window.isNull("CotAssessmentId", defaultValues, 0);
    this.createdOn = getDateString(window.isNull("CreatedOn", defaultValues, ""));
    this.updatedOn = getDateString(window.isNull("UpdatedOn", defaultValues, ""), "MM/dd/yyyy HH:mm:ss");
    this.goalMetDate = getDateString(window.isNull("GoalMetDate", defaultValues, ""));
    this.goalSetDate = getDateString(window.isNull("GoalSetDate", defaultValues, ""));
    this.status = window.isNull("Status", defaultValues, {});

    this.goalSetMode = this.status.value == CotStatus.StgReport.Initialised;
    this.goalMetMode = this.status.value == CotStatus.StgReport.Saved;

    this.measures = [];
    var itemIds = window.isNull("Items", defaultValues, []);
    
    var length = assessmentEntity.Measures.length;
    for (var i = 0; i < length; i++) {
        var measureEntity0 = assessmentEntity.Measures[i];
        // can not override the measure entity, because other pages need show all items.
        var measureEntity = $.extend({}, measureEntity0, { Items: measureEntity0.Items });
        var itemEntities = measureEntity.Items;
        var children = measureEntity.Children;
        measureEntity.Items = false;
        measureEntity.Children = false;
        measureEntity.Visible = false;

        var itemsLength = itemEntities.length;
        if (itemEntities && itemsLength) {
            for (var itemIndex = 0; itemIndex < itemsLength; itemIndex++) {
                var itemEntity = itemEntities[itemIndex];
                if (itemIds.indexOf(itemEntity.ItemId) >= 0) {
                    if (!measureEntity.Items) {
                        measureEntity.Items = [];
                    }
                    var reportItem = GetItemByItemId(teacherItemEntities, itemEntity.ItemId);
                    $.extend(itemEntity, reportItem, { GoalMetAble: this.status.value == CotStatus.StgReport.Saved });
                    measureEntity.Visible = true;
                    measureEntity.Items.push(itemEntity);
                }
            }
        }

        var childrenLength;
        if (children && (childrenLength = children.length)) {
            for (var childIndex = 0; childIndex < childrenLength; childIndex++) {
                var childEntity = $.extend({}, children[childIndex], { Items: children[childIndex].Items });
                var childItems = childEntity.Items;
                childEntity.Items = false;
                childEntity.Visible = false;
                if (childItems && (itemsLength = childItems.length)) {
                    for (itemIndex = 0; itemIndex < itemsLength; itemIndex++) {
                        itemEntity = childItems[itemIndex];
                        if (itemIds.indexOf(itemEntity.ItemId) >= 0) {
                            reportItem = GetItemByItemId(teacherItemEntities, itemEntity.ItemId);
                            $.extend(itemEntity, reportItem, { GoalMetAble: this.status.value == CotStatus.StgReport.Saved });
                            childEntity.Visible = true;
                            measureEntity.Visible = true;
                            if (!measureEntity.Children) {
                                measureEntity.Children = [];
                            }
                            if (!childEntity.Items) {
                                childEntity.Items = [];
                            }
                            childEntity.Items.push(itemEntity);
                        }
                    }
                }
                if (childEntity.Items) {
                    if (!measureEntity.Children) measureEntity.Children = [];
                    measureEntity.Children.push(childEntity);
                }
            }
        }
        var measureModel = new CotMeasure(measureEntity);

        this.measures.push(measureModel);

        this.groups = [];

    }
}

function CotStgGroupModel(groupGoal) {
    this.id = window.isNull("ID", groupGoal, 0);
    this.cotStgReportId = window.isNull("CotStgReportId", groupGoal, 0);
    this.groupName = window.isNull("GroupName", groupGoal, "");
    this.onMyOwn = window.isNull("OnMyOwn", groupGoal, "");
    this.withSupport = window.isNull("WithSupport", groupGoal, "");

    //var itemEntities = groupGoal.Items;
    //var itemsLength = itemEntities.length;
    //if (itemEntities && itemsLength) {
    //    for (var itemIndex = 0; itemIndex < itemsLength; itemIndex++) {
    //        var itemEntity = itemEntities[itemIndex];
    //        if (itemIds.indexOf(itemEntity.ItemId) >= 0) {
    //            if (!measureEntity.Items) {
    //                measureEntity.Items = [];
    //            }
    //            var reportItem = GetItemByItemId(teacherItemEntities, itemEntity.ItemId);
    //            $.extend(itemEntity, reportItem, { GoalMetAble: this.status.value == CotStatus.StgReport.Saved });
    //            measureEntity.Visible = true;
    //            measureEntity.Items.push(itemEntity);
    //        }
    //    }
    //}
    var itemIds = window.isNull("Items", defaultValues, []);
}

function CotTeacherModel(assessmentEntity, defaultValues, loadRecords) {
    var that = this;

    this.id = window.isNull("ID", defaultValues, 0);
    this.userId = window.isNull("UserID", defaultValues, 0);
    this.firstname = window.isNull("FirstName", defaultValues, "");
    this.lastname = window.isNull("LastName", defaultValues, "");
    this.yearsInProject = window.isNull("YearsInProject", defaultValues, "");
    this.coachFirstname = window.isNull("CoachFirstName", defaultValues, "");
    this.coachLastname = window.isNull("CoachLastName", defaultValues, "");
    this.schoolName = window.isNull("SchoolsText", defaultValues, "");
    this.communityName = window.isNull("CommunitiesText", defaultValues, "");
    this.cotWave1 = window.isNull("CotWave1", defaultValues, "");
    this.cotWave2 = window.isNull("CotWave2", defaultValues, "");
    this.schoolYear = window.isNull("SchoolYear", defaultValues, 0);

    this.syncStatus = ko.observable(OfflineStatus.Cached);

    this.changed = ko.observable(window.isNull("changed", defaultValues, false));
    if (this.changed()) {
        this.syncStatus(OfflineStatus.Changed);
    }
    this.toString = function () {
        return "Teacher[" + this.firstname + " " + this.lastname + "]";
    };

    this.showActiveGoalsOnly = ko.observable(false);
    this.toggleActiveGoals = function () {
        that.showActiveGoalsOnly(!that.showActiveGoalsOnly());
    };

    if (loadRecords) {
        this.hasAssessmentTodo = true;
        this.hasCotReport = false;
        this.hasStgReport = false;

        this.waves = {};
        var records = window.isNull("Records", defaultValues, {});
        this.availableWaves = [];
        this.choosedWave = ko.observable();
        var completed = 0;
        var shouldAppend = true;
        var boyStatus = CotStatus.Wave.Initialised;
        var moyStatus = CotStatus.Wave.Initialised;
        var wavesTodo = [CotWave.BOY, CotWave.MOY];
        for (var k = 0; k < wavesTodo.length; k++) {
            var w = wavesTodo[k];
            var waveEntity = null;
            if (records && records.Waves) {
                for (var i = 0; i < records.Waves.length; i++) {
                    if (records.Waves[i].Wave.value == w) {
                        waveEntity = records.Waves[i];
                        break;
                    }
                }
            }
            if (waveEntity) {
                var waveItems = records.TempItems[waveEntity.Wave.text];
                var model = new CotWaveModel(waveItems, waveEntity);
                this.waves[w] = model;
                if (model.wave.value == CotWave.BOY) {
                    boyStatus = model.status.value;
                    if (boyStatus === CotStatus.Wave.Finalized) {
                        completed++;
                    }
                }
                if (model.wave.value == CotWave.MOY) {
                    moyStatus = model.status.value;
                    if (moyStatus === CotStatus.Wave.Finalized) {
                        completed++;
                    }
                }
            } else {
                this.waves[w] = new CotWaveModel();
            }
        }

        this.hasAssessmentTodo = completed < 2;
        this.hasCotReport = completed > 0;

        this.waves[CotWave.None] = new CotWaveModel();
        if (moyStatus < CotStatus.Wave.Finalized && boyStatus !== CotStatus.Wave.Saved) {
            if (!this.waves[CotWave.MOY]) {
                this.waves[CotWave.MOY] = new CotWaveModel();
            }
            var m = { value: 2, text: "MOY" }
            this.choosedWave(m);
            this.availableWaves.unshift(m);
            if (moyStatus == CotStatus.Wave.Saved) {
                shouldAppend = false;
            }
        }
        if (shouldAppend && boyStatus < CotStatus.Wave.Finalized) {
            if (!this.waves[CotWave.BOY]) {
                this.waves[CotWave.BOY] = new CotWaveModel();
            }
            this.choosedWave({ value: 1, text: "BOY" });
            this.availableWaves.unshift(this.choosedWave());
        }
        if (this.availableWaves.length == 2) {
            this.choosedWave({ value: 0, text: "Please select..." });
            this.availableWaves.unshift(this.choosedWave());
        }
        this.choosedWaveAssessment = ko.computed(function () {
            var selectedWave = this.choosedWave();
            if (selectedWave && "value" in selectedWave) {
                return this.waves[this.choosedWave().value];
            }
            return {};
        }, that);

        this.stgReports = [];
        this.reportById = {};
        this.editingStgReport = false;
        var countOfDate = {};
        if (records && records.Reports) {
            for (var j = 0; j < records.Reports.length; j++) {
                var reportModel = new CotStgReportModel(records.Items, records.Reports[j]);
                if (this.editingStgReport == false) {
                    this.editingStgReport = reportModel;
                }

                if (!(reportModel.goalSetDate in countOfDate)) {
                    countOfDate[reportModel.goalSetDate] = 0;
                }
                countOfDate[reportModel.goalSetDate]++;

                this.stgReports.push(reportModel);
                this.reportById[reportModel.id] = reportModel;
            }
            for (j = 0; j < this.stgReports.length; j++) {
                reportModel = this.stgReports[j];
                countOfDate[reportModel.goalSetDate]--;

                if (countOfDate[reportModel.goalSetDate] <= 0) {
                    reportModel.suffix = "";
                } else {
                    reportModel.suffix = " (" + countOfDate[reportModel.goalSetDate] + ")";
                }
            }
        }
        this.hasStgReport = this.stgReports.length > 0;

        this.measures = [];
        var itemEntities = records && records.Items;
        var length = assessmentEntity.Measures.length;
        for (var i = 0; i < length; i++) {
            var measureEntity = assessmentEntity.Measures[i];
            var itemsLength;
            if (measureEntity.Items && (itemsLength = measureEntity.Items.length)) {
                for (var itemIndex = 0; itemIndex < itemsLength; itemIndex++) {
                    var itemEntity = measureEntity.Items[itemIndex];
                    var itemRecord = GetItemByItemId(itemEntities, itemEntity.ItemId);
                    if (itemRecord) {
                        $.extend(itemEntity, itemRecord);
                    }
                }
            }

            var childrenLength;
            if (measureEntity.Children && (childrenLength = measureEntity.Children.length)) {
                for (var childIndex = 0; childIndex < childrenLength; childIndex++) {
                    if (measureEntity.Children[childIndex] && measureEntity.Children[childIndex].Items &&
                    (itemsLength = measureEntity.Children[childIndex].Items.length))
                        for (itemIndex = 0; itemIndex < itemsLength; itemIndex++) {
                            itemEntity = measureEntity.Children[childIndex].Items[itemIndex];
                            itemRecord = GetItemByItemId(itemEntities, itemEntity.ItemId);
                            if (itemRecord) {
                                $.extend(itemEntity, itemRecord);
                            }
                        }
                }
            }
            var measureModel = new CotMeasure(measureEntity);
            this.measures.push(measureModel);
        }
    }
}