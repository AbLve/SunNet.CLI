function getCotOfflineApp(userId) {
    if (typeof (getOfflineAppForModule) !== "function") {
        throw Error("need reference js file: Module_Offline.js");
    }
    // require Module_Offline.js
    var app = getOfflineAppForModule("Cot", userId, {
        checkOnline: "/Cot/Offline/Online",
        sync: "/Cot/Offline/Sync",
        index: "/Cot/Offline/Index",
        teacher: "/Cot/Offline/Teacher",
        assessment: "/Cot/Offline/Assessment",
        report: "/Cot/Offline/Report",
        stg: "/Cot/Offline/Stg",
        stgResult: "/Cot/Offline/StgResult",
    });

    app.values.minDate = "01/01/1753";
    app.appendKey("stgReportType", "stgReportType");
    app.appendKey("stgReportId", "stgReportId");

    // global configuration after data saved.
    var baseWriteData = app.writeLocalData;

    // write cpalls data
    app.writeLocalData = function (dataSource) {
        this.clearCachedData();

        baseWriteData.apply(this);

        this.setItem(this.values.Key.assessment, dataSource.assessment);

        var teacherIds = [];
        for(var index in dataSource.teachers) {
            var teacher = dataSource.teachers[index];
            teacherIds.push(teacher.ID);
            this.setItem(this.values.Key.target + teacher.ID, teacher);
        };
        this.setItem(this.values.Key.targets, teacherIds);

        this.setItem(this.values.Key.cachedOn, new Date());
        this.setItem(this.values.Key.cachedStatus, this.values.Status.Cached);
        this.setItem(this.values.Key.onlineUrl, "/Cot/Teacher/All/" + dataSource.assessment.AssessmentId);

    };

    function prepareTeacherRecords(teacher) {
        if (!("Records" in teacher) || !teacher.Records) {
            teacher.Records = {};
        }
        if (!("Items" in teacher.Records)) {
            teacher.Records.Items = [];
        }
        if (!("Reports" in teacher.Records)) {
            teacher.Records.Reports = [];
        }
        if (!("TempItems" in teacher.Records)) {
            teacher.Records.TempItems = { BOY: [], MOY: [] };
        }
        if (!("Waves" in teacher.Records)) {
            teacher.Records.Waves = [];
        }
    };

    function fillAssessmentItems(waveModel, oldWaveValue, tmpItems, items) {
        for (i = 0; i < waveModel.measures.length; i++) {
            var measure = waveModel.measures[i];
            if (measure.items && measure.items.length) {
                for (var j = 0; j < measure.items.length; j++) {
                    var item = measure.items[j];
                    if (item.observed()) {
                        oldWaveValue.Items.push(item.itemId);
                        var oldTempItem = GetItemByItemId(tmpItems, item.itemId);
                        if (!oldTempItem) {
                            oldTempItem = {
                                ItemId: item.itemId,
                                NeedSupport: false
                            };
                            tmpItems.push(oldTempItem);
                        }
                        oldTempItem.changed = true;
                        oldTempItem.NeedSupport = item.needSupport();
                        if (oldWaveValue.Wave.value == CotWave.BOY) {
                            oldTempItem.BoyObsDate = oldWaveValue.VisitDate;
                        } else if (oldWaveValue.Wave.value == CotWave.MOY) {
                            oldTempItem.MoyObsDate = oldWaveValue.VisitDate;
                        }
                        if (oldWaveValue.Status.value == CotStatus.Wave.Finalized) {
                            var oldItem = GetItemByItemId(items, item.itemId);
                            if (!oldItem) {
                                oldItem = {};
                                items.push(oldItem);
                            }
                            oldItem.changed = true;
                            $.extend(oldItem, oldTempItem);
                        }
                    }
                }
            }
            if (measure.children && measure.children.length) {
                for (var k = 0; k < measure.children.length; k++) {
                    var child = measure.children[k];
                    if (child.items && child.items.length) {
                        for (var j = 0; j < child.items.length; j++) {
                            var item = child.items[j];
                            if (item.observed()) {
                                oldWaveValue.Items.push(item.itemId);
                                oldTempItem = GetItemByItemId(tmpItems, item.itemId);
                                if (!oldTempItem) {
                                    oldTempItem = {
                                        ItemId: item.itemId,
                                        NeedSupport: false
                                    };
                                    tmpItems.push(oldTempItem);
                                }
                                oldTempItem.changed = true;
                                oldTempItem.NeedSupport = item.needSupport();
                                if (oldWaveValue.Wave.value == CotWave.BOY) {
                                    oldTempItem.BoyObsDate = oldWaveValue.VisitDate;
                                } else if (oldWaveValue.Wave.value == CotWave.MOY) {
                                    oldTempItem.MoyObsDate = oldWaveValue.VisitDate;
                                }
                                if (oldWaveValue.Status.value == CotStatus.Wave.Finalized) {
                                    oldItem = GetItemByItemId(items, item.itemId);
                                    if (!oldItem) {
                                        oldItem = {};
                                        items.push(oldItem);
                                    }
                                    oldItem.changed = true;
                                    $.extend(oldItem, oldTempItem);
                                }
                            }
                        }
                    }
                }
            }
        }
    };

    app.saveTeacherAssessment = function (teacherModel, waveModel) {
        /// <summary>
        /// Save teacher assessment , according to teacherModel[CotTeacherModel]
        /// </summary>
        /// <param name="teacherModel" type="CotTeacherModel">
        /// A teacher model contains new value of teacher.
        /// </param>
        /// <param name="waveModel" type="CotWaveModel">
        /// A assessment need to save, could be BOY | MOY model of teacher.
        /// </param>
        this.resumePin();
        var teacher = this.targetByKey[teacherModel.id];
        if (!teacher) {
            return "Teacher is not existed.";
        }
        prepareTeacherRecords(teacher);
        var oldWaveValue = false;
        for (var i = 0; i < teacher.Records.Waves.length; i++) {
            if (teacher.Records.Waves[i].Wave.value == waveModel.wave.value) {
                oldWaveValue = teacher.Records.Waves[i];
                break;
            }
        }
        var dateObj = new Date();
        var date = dateObj.Format("MM/dd/yyyy HH:mm:ss");
        if (oldWaveValue === false) {
            oldWaveValue = {
                ID: 0,
                CotAssessmentId: 0,
                Wave: teacherModel.choosedWave(),
                Status: {},
                Items: [],
                FinalizedOn: this.values.minDate,
                CreatedOn: date,
                UpdatedOn: date
            };
            teacher.Records.Waves.push(oldWaveValue);
        }
        oldWaveValue.UpdatedOn = date;

        var tmpItems = teacher.Records.TempItems[oldWaveValue.Wave.text];
        var items = teacher.Records.Items;
        oldWaveValue.changed = true;
        if (waveModel.status.value == CotStatus.Wave.Finalized) {
            oldWaveValue.Status = {
                value: CotStatus.Wave.Finalized,
                text: "Finalized"
            };
            oldWaveValue.FinalizedOn = new Date().Format("MM/dd/yyyy HH:mm:ss");
        } else {
            oldWaveValue.Status = {
                value: CotStatus.Wave.Saved,
                text: "Saved"
            }
        }
        oldWaveValue.VisitDate = waveModel.visitDate;
        oldWaveValue.SpentTime = waveModel.spentTime;
        if (oldWaveValue.Status.value == CotStatus.Wave.Finalized) {
            if (oldWaveValue.Wave.value == CotWave.BOY) {
                teacher.CotWave1 = oldWaveValue.VisitDate;
            }
            if (oldWaveValue.Wave.value == CotWave.MOY) {
                teacher.CotWave2 = oldWaveValue.VisitDate;
            }
        }
        fillAssessmentItems(waveModel, oldWaveValue, tmpItems, items);
        teacher.changed = true;
        this.setItem(this.values.Key.target + teacher.ID, teacher);
        this.registeChange();
        return true;
    };

    app.updateCotItems = function (teacherModel, updatingItems) {
        this.resumePin();
        var teacher = this.targetByKey[teacherModel.id];
        if (!teacher) {
            return "Teacher is not existed.";
        }
        prepareTeacherRecords(teacher);
        var dateObj = new Date();
        var date = dateObj.Format("MM/dd/yyyy HH:mm:ss");
        var items = teacher.Records.Items;
        var length = updatingItems.length;
        for (var i = 0; i < length; i++) {
            var itemNew = updatingItems[i];
            if (itemNew.GoalSetDate != this.values.minDate && itemNew.GoalMetDate == this.values.minDate) {
                itemNew.WaitingGoalMet = true;
            }
            else
                itemNew.WaitingGoalMet = false;

            var itemEntity = GetItemByItemId(items, itemNew.ItemId);
            if (!itemEntity) {
                itemEntity = {
                    BoyObsDate: this.values.minDate,
                    CotAssessmentId: 0,
                    CotUpdatedOn: this.values.minDate,
                    GoalMetDate: this.values.minDate,
                    GoalSetDate: this.values.minDate,
                    ID: 0,
                    ItemId: 0,
                    MoyObsDate: this.values.minDate,
                    NeedSupport: false,
                    WaitingGoalMet: false,
                    CreatedOn: date,
                    UpdatedOn: date
                };
                items.push(itemEntity);
            } 
            itemEntity.UpdatedOn = date;
            $.extend(itemEntity, itemNew);
            console.log(itemEntity);
            itemEntity.changed = true;
        }
        teacher.changed = true;
        this.setItem(this.values.Key.target + teacher.ID, teacher);
        this.registeChange();
        return true;
    };

    app.createStgReport = function (teacherModel, itemIds) {
        this.resumePin();
        var teacher = this.targetByKey[teacherModel.id];
        if (!teacher) {
            return "Teacher is not existed.";
        }
        prepareTeacherRecords(teacher);

        var dateObj = new Date();
        var date = dateObj.Format("MM/dd/yyyy HH:mm:ss");
        var newReport = {
            AdditionalComments: "",
            CotAssessmentId: 0,
            GoalMetDate: this.values.minDate,
            GoalSetDate: this.values.minDate,
            Items: itemIds,
            OnMyOwn: "",
            Status: {
                value: CotStatus.StgReport.Initialised,
                text: "Initialised"
            },
            SpentTime: "0.00",
            ID: dateObj - 0,
            CreatedOn: date,
            UpdatedOn: date,
            WithSupport: ""
        };
        for (var i = 0; i < itemIds.length; i++) {
            var itemId = itemIds[i];
            var item = GetItemByItemId(teacher.Records.Items, itemId);
            if (!item) {
                item = {
                    BoyObsDate: this.values.minDate,
                    CotAssessmentId: 1,
                    CotUpdatedOn: this.values.minDate,
                    GoalMetDate: this.values.minDate,
                    GoalSetDate: this.values.minDate,
                    ID: 0,
                    ItemId: itemId,
                    MoyObsDate: this.values.minDate,
                    NeedSupport: false,
                    WaitingGoalMet: false,
                    GoalMetAble: false,
                    CreatedOn: date,
                    UpdatedOn: date
                };
                teacher.Records.Items.push(item);
            }
            item.UpdatedOn = date;

            item.GoalMetDone = false;
            item.changed = true;
            item.GoalSetDate = date;
            item.GoalMetDate = this.values.minDate;
        }
        newReport.changed = true;
        teacher.Records.Reports.unshift(newReport);
        teacher.changed = true;
        this.setItem(this.values.Key.target + teacher.ID, teacher);
        this.registeChange();
        return true;
    };

    app.saveStgReport = function (teacherModel, stgReportModel, choosedItems) {
        this.resumePin();
        var teacher = this.targetByKey[teacherModel.id];
        if (!teacher) {
            return "Teacher is not existed.";
        }
        prepareTeacherRecords(teacher);

        var report = false;
        if (teacher.Records.Reports.length) {
            report = teacher.Records.Reports[0];
        }
        if (report === false) {
            return "There is no reports existed.";
        }
        var items = [];
        for (var i = 0; i < teacher.Records.Items.length; i++) {
            var item = teacher.Records.Items[i];
            if (report.Items.indexOf(item.ItemId) >= 0) {
                items.push(item);
            }
        }
        if (stgReportModel.status.value == CotStatus.StgReport.Initialised) {
            //first time save
            report.GoalSetDate = stgReportModel.goalSetDate;
            report.SpentTime = stgReportModel.spentTime;
            report.OnMyOwn = stgReportModel.onMyOwn;
            report.WithSupport = stgReportModel.withSupport;
            report.AdditionalComments = stgReportModel.additionalComments;
            report.Status = { value: CotStatus.StgReport.Saved, text: "Saved" };

            for (var j = 0; j < items.length; j++) {
                items[j].GoalSetDate = stgReportModel.goalSetDate;
                items[j].GoalMetAble = true;
                items[j].WaitingGoalMet = true;
            }
        } else {
            report.GoalMetDate = stgReportModel.goalMetDate;
            for (j = 0; j < items.length; j++) {
                if (choosedItems.indexOf(items[j].ItemId) >= 0) {
                    items[j].GoalMetDate = stgReportModel.goalMetDate;
                    items[j].GoalMetDone = true;
                    items[j].WaitingGoalMet = false;
                }
            }
        }
        report.changed = true;

        teacher.changed = true;
        this.setItem(this.values.Key.target + teacher.ID, teacher);
        this.registeChange();
        return true;
    };

    app.sync = function (teacherModel) {
        var deferred = $.Deferred();
        if (!teacherModel.changed()) {
            setTimeout(function () {
                deferred.rejectWith(teacherModel, [{ type: "info", msg: "data has not been modified offline." }]);
            }, 10);
            return deferred.promise();
        }
        var teacher = this.targetByKey[teacherModel.id];
        if (!teacher) {
            setTimeout(function () {
                deferred.rejectWith(teacherModel, [{ type: "info", msg: "not existed." }]);
            }, 10);
            return deferred.promise();
        }
        prepareTeacherRecords(teacher);
        var items = [];
        var tmpItems = [];
        var waves = [];
        var reports = [];
        for (var i = 0; i < teacher.Records.Items.length; i++) {
            var item = teacher.Records.Items[i];
            if (item.changed) {
                items.push(item);
                item.changed = false;
            }
        }
        for (var wave in teacher.Records.TempItems) {
            for (var j = 0; j < teacher.Records.TempItems[wave].length; j++) {
                item = teacher.Records.TempItems[wave][j];
                if (item.changed) {
                    tmpItems.push($.extend({}, item, { Wave: wave }));
                    item.changed = false;
                }
            }
        }
        for (var m = 0; m < teacher.Records.Waves.length; m++) {
            var wave = teacher.Records.Waves[m];
            if (wave.changed) {
                waves.push($.extend({}, wave, { Status: wave.Status.value, Wave: wave.Wave.value }));
                wave.changed = false;
            }
        }
        for (var k = 0; k < teacher.Records.Reports.length; k++) {
            var report = teacher.Records.Reports[k];
            if (report.changed) {
                var newReport = $.extend({}, report, { Status: report.Status.value });
                if (newReport.ID > 2147483647) {
                    newReport.ID = 0;
                }
                var reportItems = [];
                for (var l = 0; l < report.Items.length; l++) {
                    reportItems.push({ ItemId: report.Items[l] });
                }
                newReport.ReportItems = reportItems;
                reports.push(newReport);
                report.changed = false;
            }
        }
        teacher.changed = false;
        $.post(this.values.Url.sync, {
            teacherId: teacher.ID,
            assessmentId: this.assessment.AssessmentId,
            items: JSON.stringify(items),
            tmpItems: JSON.stringify(tmpItems),
            waves: JSON.stringify(waves),
            reports: JSON.stringify(reports)
        }, function (response) {
            if (response.success) {
                app.setItem(app.values.Key.target + teacher.ID, teacher);
                deferred.resolveWith(teacherModel);
                app.checkChangeStatus();
            } else {
                deferred.rejectWith(teacherModel, { type: "danger", msg: response.msg });
            }
        }, "json").error(function (xhr, statusText, msg) {
            deferred.rejectWith(teacherModel, [{ type: "danger", msg: msg }]);
        });
        return deferred.promise();
    };

    app.viewModel = (function () {
        var that = {};
        that.teacher = null;
        that.teachers = [];
        that.teacherById = {};
        that.assessment = {};
        that.network = app.network;
        that.Url = app.values.Url;
        that.showSyncAll = ko.computed(function () {
            return app.changed();
        }, that);

        that.viewTeacher = function (teacher) {
            app.setItem(app.values.Key.workingTarget, teacher.id);
            location.href = app.values.Url.teacher;
        };

        that.gotoAssessment = function () {
            if (that.teacher && that.teacher.hasAssessmentTodo === true) {
                location.href = app.values.Url.assessment;
            }
        };
        that.gotoCotReport = function () {
            if (that.teacher && that.teacher.hasCotReport === true) {
                location.href = app.values.Url.report;
            }
        };
        that.gotoStgReport = function () {
            if (that.teacher && that.teacher.hasStgReport === true) {
                location.href = app.values.Url.stg;
            }
        };
        that.gotoTeacherFolder = function () {
            location.href = app.values.Url.teacher;
        };
        that.gotoStgResult = function(type,reportId) {
            app.setItem(app.values.Key.stgReportType, type);
            app.setItem(app.values.Key.stgReportId, reportId);
            if (that.teacher) {
                location.href = app.values.Url.stgResult;
            }
        };

        that.spentTimeOptions = (function (start, end, stepValue) {
            var options = [];
            while (start <= end) {
                var option = {
                    text: start.toFixed(2),
                    value: start.toFixed(2)
                }
                if (start === 0) {
                    option.value = "";
                }
                options.push(option);
                start += stepValue;
            }
            return options;
        })(0, 8, 0.25);

        that.saveAssessment = function (model,event) {
            var $form = $(event.target).closest("form");
            if ($form.length === 0) {
                return;
            }
            if (!that.teacher) {
                return;
            }
            if (!$form.valid()) {
                return;
            }
            $form.cliForm("loading");
            var result = app.saveTeacherAssessment(that.teacher, that.teacher.choosedWaveAssessment());
            if (result === true) {
                $.when(window.showMessage("success")).done(function () {
                    $("form[id]").cliForm("loaded");
                    location.reload();
                });
            } else {
                window.showMessage("fail", result);
            }
        };
        that.finalizeAssessment = function (model, event) {
            var $form = $(event.target).closest("form");
            if ($form.length === 0) {
                return;
            }
            if (!that.teacher) {
                return;
            }
            if (!$form.valid()) {
                return;
            }
            $("form[id]").cliForm("loading");
            that.teacher.choosedWaveAssessment().status = { value: CotStatus.Wave.Finalized, text: "Finalized" };
            var result = app.saveTeacherAssessment(that.teacher, that.teacher.choosedWaveAssessment());
            if (result === true) {
                $.when(window.showMessage("success")).done(function () {
                    $("form[id]").cliForm("loaded");
                    location.replace(app.values.Url.teacher);
                });
            } else {
                window.showMessage("fail", result);
            }
        };

        that.createStgReport = function () {
            if (!that.teacher) {
                return;
            }
            var items = [];
            var savedItems = [];
            var reportItems = [];
            var i, j, k, measure, child, item, itemForUpdate;
            for (i = 0; i < that.teacher.measures.length; i++) {
                measure = that.teacher.measures[i];
                if (measure.items && measure.items.length) {
                    for (j = 0; j < measure.items.length; j++) {
                        item = measure.items[j];
                        itemForUpdate = item.itemForUpdate(true);
                        if (itemForUpdate) {
                            items.push(itemForUpdate);
                            savedItems.push(item);
                        }
                        if (item.waitingGoalMet()) {
                            reportItems.push(item.itemId);
                        }
                    }
                }
                if (measure.children && measure.children.length) {
                    for (k = 0; k < measure.children.length; k++) {
                        child = measure.children[k];
                        if (child.items && child.items.length) {
                            for (j = 0; j < child.items.length; j++) {
                                item = child.items[j];
                                itemForUpdate = item.itemForUpdate(true);
                                if (itemForUpdate) {
                                    items.push(itemForUpdate);
                                    savedItems.push(item);
                                }
                                if (item.waitingGoalMet()) {
                                    reportItems.push(item.itemId);
                                }
                            }
                        }
                    }
                }
            }
            var result = true;
            if (items.length) {
                result = app.updateCotItems(that.teacher, items);
            }
            if (result === true) {
                if (reportItems.length < 1) {
                    window.showMessage("fail", "Cot_At_Least_One");
                    return;
                }
                result = app.createStgReport(that.teacher, reportItems);
                if (result === true) {
                    location.href = app.values.Url.stg;
                } else {
                    window.showMessage("fail", result);
                }
            } else {
                window.showMessage("fail", result);
            }
        };
        that.saveStgReport = function () {
            if (!that.teacher) {
                return;
            }
            updateCkeditor();
            var isSavedFirstTime = that.teacher.editingStgReport.status.value == CotStatus.StgReport.Initialised;
            var choosedItems = [];
            if (!isSavedFirstTime) {
                for (var i = 0; i < that.teacher.editingStgReport.measures.length; i++) {
                    var measure = that.teacher.editingStgReport.measures[i];
                    if (measure.items && measure.items.length) {
                        for (var j = 0; j < measure.items.length; j++) {
                            var item = measure.items[j];
                            if (item.goalMetAble && item.goalMetAbled) {
                                choosedItems.push(item.itemId);
                            }
                        }
                    }
                    if (measure.children && measure.children.length) {
                        for (var k = 0; k < measure.children.length; k++) {
                            var child = measure.children[k];
                            if (child.items && child.items.length) {
                                for (var j = 0; j < child.items.length; j++) {
                                    item = child.items[j];
                                    if (item.goalMetAble && item.goalMetAbled) {
                                        choosedItems.push(item.itemId);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            var result = app.saveStgReport(that.teacher, that.teacher.editingStgReport, choosedItems);
            if (result === true) {
                //location.reload();
                location.href = app.values.Url.teacher;
            } else {
                window.showMessage("fail", result);
            }
        };

        that.saveCotItems = function() {
            if (!that.teacher) {
                return;
            }
            var items = [];
            var savedItems = [];
            var itemForUpdate, i, j, k, measure, child, item;
            for (i = 0; i < that.teacher.measures.length; i++) {
                measure = that.teacher.measures[i];
                if (measure.items && measure.items.length) {
                    for (j = 0; j < measure.items.length; j++) {
                        item = measure.items[j];
                        item.saved(false);
                        itemForUpdate = item.itemForUpdate();
                        if (itemForUpdate) {
                            items.push(itemForUpdate);
                            savedItems.push(item);
                        }
                    }
                }
                if (measure.children && measure.children.length) {
                    for (k = 0; k < measure.children.length; k++) {
                        child = measure.children[k];
                        if (child.items && child.items.length) {
                            for (j = 0; j < child.items.length; j++) {
                                item = child.items[j];
                                item.saved(false);
                                itemForUpdate = item.itemForUpdate();
                                if (itemForUpdate) {
                                    items.push(itemForUpdate);
                                    savedItems.push(item);
                                }
                            }
                        }
                    }
                }
            }
            var result = app.updateCotItems(that.teacher, items);
            if (result === true) {
                $.each(savedItems, function(index, savedItem) {
                    savedItem.changed(false);

                    savedItem.saved(true);
                });
                window.scroll(0, 0);
                window.showMessage("success");
            } else {
                window.showMessage("fail", result);
            }
        };

        that.syncAll = function ()
        {

            jQuery.ajax(
             {
                 url: "/Cot/Offline/Sync",
                 cache: false,
                 dataType: "text"
             }
         ).success(
             function (response)
             {
                 var online = null;
                 try
                 {
                     online = JSON.parse(response);
                 } catch (e)
                 {

                 }
                 if (online && typeof (online) === "object")
                 {
                     var message = "Online now, " + " logged: <strong>" + (online.logged ? "Yes" : "No") + "</strong>, server: " + online.date;
                     //app.log(online.logged ? "info" : "warning", message);
                     if (app.network.online() === false)
                     {
                         app.log("success", message);
                     }
                     app.network.online(true);
                     app.network.logged(online.logged);
                     app.network.date("[" + online.date + "]");
                     return online.logged;
                 } else
                 {
                     if (app.network.online())
                     {
                         //console.log(arguments);
                         app.log("danger", response);
                     }

                     //app.log("info", "Offline now.");
                     app.network.online(false);
                     app.network.logged(false);
                     app.network.date("");
                 }
             }
         ).error(function (jqXhr, errorMsg, errorObj)
         {
             if (app.network.online())
             {
                 //console.log(arguments);
                 app.log("danger", jqXhr.responseText);
             }
             errorOccuredDuringNerwork = true;
             app.network.online(false);
             app.network.logged(false);
             app.network.date("");
         }).complete(function ()
         {
             for (var i = 0; i < that.teachers.length; i++)
             {
                 var teacherModel = that.teachers[i];
                 if (teacherModel.changed())
                 {
                     that.sync(teacherModel);
                 }
             }
         }

         );
        };
        that.sync = function (teacherModel) {
            if (!that.network.logged()) {
                jQuery.ajax(
          {
              url: "/Cot/Offline/Online",
              cache: false,
              dataType: "text"
          }
      ).success(
          function (response) {
              var online = null;
              try {
                  online = JSON.parse(response);
              } catch (e) {

              }
              if (online && typeof (online) === "object") {
                  var message = "Online now, " + " logged: <strong>" + (online.logged ? "Yes" : "No") + "</strong>, server: " + online.date;
                  //app.log(online.logged ? "info" : "warning", message);
                  if (app.network.online() === false) {
                      app.log("success", message);
                  }
                  app.network.online(true);
                  app.network.logged(online.logged);
                  app.network.date("[" + online.date + "]");
                  return online.logged;
              } else {
                  if (app.network.online()) {
                      //console.log(arguments);
                      app.log("danger", response);
                  }

                  //app.log("info", "Offline now.");
                  app.network.online(false);
                  app.network.logged(false);
                  app.network.date("");
              }
          }
      ).error(function (jqXhr, errorMsg, errorObj) {
          if (app.network.online()) {
              //console.log(arguments);
              app.log("danger", jqXhr.responseText);
          }
          errorOccuredDuringNerwork = true;
          app.network.online(false);
          app.network.logged(false);
          app.network.date("");
      }).complete(function () {
          if (!that.network.online()) {
              window.showMessage("warning", "Internet_not_available");
              return;
          }
          if (!that.network.logged()) {
              $.when(window.waitingAlert("warning", "Sync_need_Login")).done(function () {
                  //   app.markSyncing();
                  $("#modalLogin").modal("show");
              });
              return;
          }
          if (teacherModel.syncStatus() == app.values.Status.Changed) {
              teacherModel.syncStatus(app.values.Status.Syncing);
              $.when(app.sync(teacherModel)).done(function () {
                  var msg = window.getErrorMessage("Cot_Sync_done");
                  app.log("success", msg.replace("{Firstname}", that.teacher.firstname).replace("{Lastname}", that.teacher.lastname));
                  teacherModel.syncStatus(app.values.Status.Synced);
              }).fail(function (status) {
                  teacherModel.syncStatus(app.values.Status.Error);
                  if (status) {
                      app.log(status.type, teacherModel.toString() + ": " + status.msg);
                  }
                  if (arguments.length == 0) {
                      app.log("danger", window.getErrorMessage("Sync_error"));
                  }
              });
          }
      }

      );
            }
            else {
                if (!that.network.online()) {
                    window.showMessage("warning", "Internet_not_available");
                    return;
                }
                if (!that.network.logged()) {
                    $.when(window.waitingAlert("warning", "Sync_need_Login")).done(function () {
                        //   app.markSyncing();
                        $("#modalLogin").modal("show");
                    });
                    return;
                }
                if (teacherModel.syncStatus() == app.values.Status.Changed) {
                    teacherModel.syncStatus(app.values.Status.Syncing);
                    $.when(app.sync(teacherModel)).done(function () {
                        var msg = window.getErrorMessage("Cot_Sync_done");
                        app.log("success", msg.replace("{Firstname}", that.teacher.firstname).replace("{Lastname}", that.teacher.lastname));
                        teacherModel.syncStatus(app.values.Status.Synced);
                    }).fail(function (status) {
                        teacherModel.syncStatus(app.values.Status.Error);
                        if (status) {
                            app.log(status.type, teacherModel.toString() + ": " + status.msg);
                        }
                        if (arguments.length == 0) {
                            app.log("danger", window.getErrorMessage("Sync_error"));
                        }
                    });
                }
            }
           
        };

        return that;
    })();

    var baseInit = app.init;
    app.init = function () {
        baseInit.apply(app);

        if (this.status > this.values.Status.None) {
            this.viewModel.assessment = this.assessment;
            getCotOfflineApp.assessment = this.values.Key.assessment;
            var workingTeacherId = this.getItem(this.values.Key.workingTarget);
            if (this.targetIds && this.targetIds.length) {

                for (var i = 0; i < this.targetIds.length; i++) {
                    var teacherId = this.targetIds[i];
                    var teacher = this.targetByKey[teacherId];
                    if (teacher.changed) {
                        this.changed(true);
                    }
                    var teacherModel = new CotTeacherModel(this.assessment, teacher);
                    if (teacherId == workingTeacherId) {
                        this.viewModel.teacher = teacherModel = new CotTeacherModel(this.assessment, teacher, true);
                    }
                    this.viewModel.teachers.push(teacherModel);
                    this.viewModel.teacherById[teacherId] = teacherModel;
                }
            }

            app.network.logged.subscribe(function (logged) {
                if (logged) {
                    if (app.shouldStartSyncing() && app.viewModel.showSyncAll()) {
                        app.viewModel.syncAll();
                    }
                }
            });
        }
    };

    return app;
}