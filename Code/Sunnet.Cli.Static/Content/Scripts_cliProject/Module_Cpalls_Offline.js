function getCpallsOfflineApp(userId) {
    if (typeof (getOfflineAppForModule) !== "function") {
        throw Error("need reference js file: Module_Offline.js");
    }
    // require Module_Offline.js
    var app = getOfflineAppForModule("Cpalls", userId, {
        checkOnline: "/Offline/Index/Online",
        sync: "/Offline/Index/Sync",
        exec: "/Cpalls/Execute/Go?offline=true",
        view: "/Cpalls/Execute/ViewOffline"
    });
    app.values.Key.execAssessment = "ExecCpalls";

    // global configuration after data saved.
    var baseWriteData = app.writeLocalData;

    // write cpalls data
    app.writeLocalData = function (dataSource) {
        var that = this;
        this.clearCachedData();
        this.setItem(this.values.Key.assessment, dataSource.assessment);
        var studentIds = [];
        $.each(dataSource.students, function (index, student) {
            studentIds.push(student.studentId);
            that.setItem(that.values.Key.target + student.studentId, student);
        });
        this.setItem(this.values.Key.targets, studentIds);

        if ("onlineUrl" in dataSource) {
            this.setItem(this.values.Key.onlineUrl, dataSource.onlineUrl);
        }
        baseWriteData.apply(this);
        return this;
    };

    app.resetStudentMeasureRelation = function (cachedStudent) {
        var headers = this.viewModel && this.viewModel.headers;
        if (headers) {
            for (var measureId in cachedStudent.measures) {
                var header = headers.measureByKey && headers.measureByKey[measureId];
                if (header && header.parent) {
                    cachedStudent.measures[measureId]["parentId"] = header.parent.id;
                }
            }
        }
    };

    app.saveStudentScores = function (studentId, executedMeasures) {
        this.resumePin();
        var key = this.values.Key.target + studentId;
        var cachedStudent = this.targetByKey[studentId];
        console.log(cachedStudent);
        var assessmentMeasures = {};
        var i, item, executedItem, goal, lastScore;

        for (i = 0; i < this.assessment.Measures.length; i++) {
            assessmentMeasures[this.assessment.Measures[i].MeasureId] = this.assessment.Measures[i];
        }

        function initItems(cachedMeasure, assessmentMeasure) {
            if (!cachedMeasure.items || !cachedMeasure.items.length) {
                cachedMeasure.items = $.map(assessmentMeasure.Items, function (eItem) {
                    return {
                        createdOn: date,
                        goal: 0,
                        isCorrect: true,
                        itemId: eItem.ItemId,
                        pauseTime: 0,
                        scored: eItem.Scored,
                        score: eItem.Score,
                        type: eItem.Type,
                        selectedAnswers: "",
                        siId: 0,
                        status: {
                            text: Cpalls_Status[Cpalls_Status.Initialised],
                            value: Cpalls_Status.Initialised
                        },
                        updatedOn: date
                    };
                });
            }
        }

        this.resetStudentMeasureRelation(cachedStudent);
        var date = new Date().Format("MM/dd/yyyy HH:mm:ss");
        if (cachedStudent) {
            //console.log("student found.", cachedStudent, cachedStudent.measures);
            var changed = false;
            for (var measureId in cachedStudent.measures) {
                var cacheMeasure = cachedStudent.measures[measureId];
                var executedMeasure = executedMeasures.measureByKey[measureId];
                if (executedMeasure) {
                    changed = true;
                    cacheMeasure.changed = true;
                    // 需要更新
                    lastScore = cacheMeasure.goal;
                    cacheMeasure.createdOn = executedMeasure.createdOn || cacheMeasure.createdOn;
                    cacheMeasure.updatedOn = executedMeasure.updatedOn;
                    initItems(cacheMeasure, assessmentMeasures[cacheMeasure.measureId]);
                    switch (executedMeasure.status.value) {
                        case Cpalls_Status.Initialised:
                            // Invalidate
                            cacheMeasure.status.value = Cpalls_Status.Initialised;
                            cacheMeasure.status.text = Cpalls_Status[cacheMeasure.status.value];
                            cacheMeasure.goal = -1;
                            cacheMeasure.comment = "";

                            for (i = 0; i < cacheMeasure.items.length; i++) {
                                item = cacheMeasure.items[i];
                                item.status.value = Cpalls_Status.Initialised;
                                item.status.text = Cpalls_Status[Cpalls_Status.Initialised];
                                item.isCorrect = false;
                                item.selectedAnswers = "";
                                item.pauseTime = 0;
                                item.goal = 0;
                                item.details = "";
                                item.executed = true;
                                item.lastItemIndex = 0;
                            }
                            break;
                        case Cpalls_Status.Paused:
                            cacheMeasure.status.value = Cpalls_Status.Paused;
                            cacheMeasure.status.text = Cpalls_Status[cacheMeasure.status.value];
                            cacheMeasure.pauseTime = executedMeasure.pauseTime;
                            for (i = 0; i < cacheMeasure.items.length; i++) {
                                item = cacheMeasure.items[i];
                                executedItem = executedMeasure.itemByKey[item.itemId];
                                if (executedItem) {
                                    item.status.value = Cpalls_Status.Finished;
                                    item.status.text = Cpalls_Status[item.status.value];
                                    item.isCorrect = executedItem.isCorrect;
                                    item.goal = executedItem.goal;
                                    item.pauseTime = executedItem.pauseTime;
                                    item.updatedOn = executedItem.updatedOn;
                                    item.details = executedItem.details;
                                    item.selectedAnswers = executedItem.selectedAnswers;
                                    item.executed = executedItem.executed;
                                    item.lastItemIndex = executedItem.lastItemIndex;
                                    item.resultIndex = executedItem.resultIndex;
                                    if (executedMeasure.items.indexOf(executedItem) === executedMeasure.items.length - 1) {
                                        // Paused on this item(the last one)
                                        item.isCorrect = false;
                                        item.goal = 0;
                                        item.status.value = Cpalls_Status.Paused;
                                        item.status.text = Cpalls_Status[item.status.value];
                                        if (item.type.value == Ade_ItemType.TxkeaReceptive.value || item.type.value == Ade_ItemType.TxkeaExpressive.value) {
                                            item.details = executedItem.details;
                                        }
                                        else {
                                            item.details = "";
                                        }
                                    }
                                }
                            }
                            cacheMeasure.goal = -1;
                            break;
                        case Cpalls_Status.Finished:
                            goal = -1;
                            totalScore = -1;
                            cacheMeasure.comment = executedMeasure.comment;
                            cacheMeasure.status.value = Cpalls_Status.Finished;
                            cacheMeasure.status.text = Cpalls_Status[cacheMeasure.status.value];
                            cacheMeasure.pauseTime = 0;
                            for (i = 0; i < cacheMeasure.items.length; i++) {
                                item = cacheMeasure.items[i];
                                executedItem = executedMeasure.itemByKey[item.itemId];
                                if (executedItem) {
                                    item.status.value = Cpalls_Status.Finished;
                                    item.status.text = Cpalls_Status[item.status.value];
                                    item.isCorrect = executedItem.isCorrect;
                                    item.goal = executedItem.goal;
                                    item.pauseTime = executedItem.pauseTime;
                                    item.updatedOn = executedItem.updatedOn;
                                    item.details = executedItem.details;
                                    item.selectedAnswers = executedItem.selectedAnswers;
                                    item.executed = executedItem.executed;
                                    item.lastItemIndex = executedItem.lastItemIndex;
                                    item.resultIndex = executedItem.resultIndex;

                                    //计算当前measure得到的分数
                                    if (executedItem.scored) {
                                        if (goal < 0) {
                                            goal = 0;
                                        }
                                        goal += item.goal;
                                    }
                                    //计算当前measure的总分（所有做过的item分数总和）
                                    if (item.scored && item.executed) {
                                        if (totalScore < 0) {
                                            totalScore = 0;
                                        }
                                        totalScore += item.score;
                                    }
                                }
                            }
                            if (goal >= 0) {
                                cacheMeasure.goal = Number(goal.toFixed(2));
                            }
                            if (totalScore >= 0) {
                                cacheMeasure.totalScore = Number(totalScore.toFixed(2));
                            }
                            break;
                        default: break;
                    }
                    if (cacheMeasure.parentId) {
                        var parent = cachedStudent.measures[cacheMeasure.parentId];
                        parent.changed = true;
                        if (parent.goal < 0) {
                            parent.goal = 0;
                        }
                        if (cacheMeasure.goal < 0) {
                            // invalidated, paused
                            if (lastScore >= 0) {
                                parent.goal -= lastScore;
                            }
                        } else {
                            parent.goal += cacheMeasure.goal - lastScore;
                        }
                        // invalidate, paused
                        if (parent.goal <= 0 && cacheMeasure.status.value !== Cpalls_Status.Finished) {
                            parent.goal = -1;
                        }
                    }
                }
            }
            console.log("changed:", changed);
            if (changed) {
                console.log(cachedStudent);
                cachedStudent.changed = true;
                this.setItem(key, cachedStudent);
                this.checkChangeStatus(true);
                setTimeout(function () {
                    location.reload();
                }, 500);
            }
        }
    };

    app.saveStudentStatus = function (studentId, studentMeasure) {
        this.resumePin();
        //console.log("start update student:", studentId,"studentMeasure:",studentMeasure);
        var key = this.values.Key.target + studentId;
        var cachedStudent = this.targetByKey[studentId];
        if (cachedStudent) {
            //console.log("student found.", cachedStudent, cachedStudent.measures);
            var changed = false;
            for (var measureId in cachedStudent.measures) {
                var measure = cachedStudent.measures[measureId];
                //console.log("checking:", measureId);
                if (studentMeasure && studentMeasure.id == measureId
                    && studentMeasure.status.value() != measure.status.value) {
                    //console.log(measureId + "need upadate");
                    measure.changed = true;
                    measure.status.value = studentMeasure.status.value();
                    changed = true;
                    break;
                } else {
                    //console.log(measureId + "not upadated");
                }
            }
            //console.log("changed:",changed);
            if (changed) {
                cachedStudent.changed = true;
                this.setItem(key, cachedStudent);
                this.checkChangeStatus(true);
            }
        }
        //console.log("update student over",cachedStudent);
    };

    app.prepareData = function (student, measureIds, forExec) {
        var assessment = this.getItem(this.values.Key.assessment);
        assessment.StudentId = student.studentId;
        assessment.Student.ID = student.studentId;
        assessment.Student.Name = student.firstName + " " + student.lastName;
        assessment.Student.Birthday = student.birthday;
        assessment.allMeasures = assessment.Measures.slice(0);
        assessment.offline = true;
        assessment.Measures.length = 0;
        
        var isExistAudio = false;
        //mobile
        if (navigator.userAgent.toLowerCase().match(/(ipod|ipad|iphone|android|coolpad|mmp|smartphone|midp|wap|xoom|symbian|j2me|blackberry|win ce)/i) != null) {
            for (var i = 0; i < assessment.allMeasures.length; i++) {
                var measureForExec = assessment.allMeasures[i];
                if (measureIds.indexOf(measureForExec.MeasureId) >= 0) {
                    for (var j = 0; j < measureForExec.Items.length; j++) {
                        var props = measureForExec.Items[j].Props;
                        var answers = measureForExec.Items[j].Answers;
                        if ((props.InstructionAudio != undefined && props.InstructionAudio != '')
                            || (props.TargetAudio != undefined && props.TargetAudio != '')
                            || (props.PromptAudio != undefined && props.PromptAudio != '')) {
                            isExistAudio = true;
                            break;
                        }
                        for (var k = 0; k < answers.length; k++) {
                            if ((answers[k].Audio != undefined && answers[k].Audio != '')
                                || (answers[k].ResponseAudio != undefined && answers[k].ResponseAudio != '')) {
                                isExistAudio = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (isExistAudio) {
                window.showMessage('warning', 'Because of limitations in the way that CLI Engage interacts with tablets, ' +
                                            'the CLI Engage platform will give a poor user experience for measures with audio files.  ' +
                                            'We encourage you to use a desktop or laptop for performing assessments on these measures.');
                return isExistAudio;
            }
        }

        for (var i = 0; i < assessment.allMeasures.length; i++) {
            var measureForExec = assessment.allMeasures[i];
            var studentMeasure = student.measures[measureForExec.MeasureId];
            var canExec = !forExec || (studentMeasure && studentMeasure.isTotal == false && (studentMeasure.status.value() == Cpalls_Status.Initialised
                                        || studentMeasure.status.value() == Cpalls_Status.Paused));
            if (canExec && measureIds.indexOf(measureForExec.MeasureId) >= 0
                && measureForExec.Items.length) {
                if (studentMeasure.items && studentMeasure.items.length) {
                    for (var k = 0; k < measureForExec.Items.length; k++) {
                        var itemForExec = measureForExec.Items[k];
                        var stuItem = studentMeasure.items.filter(function (filterItem) {
                            return filterItem.itemId == itemForExec.ItemId;
                        })[0];
                        if (stuItem && stuItem.itemId == itemForExec.ItemId) {
                            itemForExec.Goal = stuItem.goal;
                            itemForExec.IsCorrect = stuItem.isCorrect;
                            itemForExec.PauseTime = stuItem.pauseTime;
                            itemForExec.Scored = stuItem.scored;
                            itemForExec.SelectedAnswers = [];
                            var selectedAnswers = stuItem.selectedAnswers.split(",");
                            for (var l = 0; l < selectedAnswers.length; l++) {
                                itemForExec.SelectedAnswers.push(+selectedAnswers[l]);
                            }
                            itemForExec.Status = stuItem.status;
                            itemForExec.Details = stuItem.details;
                            itemForExec.Executed = stuItem.executed;
                            itemForExec.LastItemIndex = stuItem.lastItemIndex;
                            itemForExec.ResultIndex = stuItem.resultIndex > 0 ? stuItem.resultIndex : itemForExec.ResultIndex;
                            itemForExec.Deleted = false;
                        } else {
                            itemForExec.Deleted = true;
                            continue;
                        }
                    }

                }
                else {  //若没有做过，则将每个item的Executed属性设置为true
                    for (var j = 0; j < measureForExec.Items.length; j++) {
                        var itemForExec = measureForExec.Items[j];
                        itemForExec.Executed = true;
                        itemForExec.Deleted = false;
                    }
                }
                //过滤掉未做过的题目
                measureForExec.Items = measureForExec.Items.filter(function (obj) { return obj.Deleted == false });
                measureForExec.Status.value = studentMeasure.status.value();
                measureForExec.Comment = studentMeasure.comment;
                measureForExec.Benchmark = studentMeasure.benchmark;
                measureForExec.Goal = studentMeasure.goal;
                if (measureForExec.Goal < 0)
                    measureForExec.Goal = 0;
                measureForExec.AgeGroup = studentMeasure.ageGroup;
                measureForExec.BenchmarkText = studentMeasure.benchmarkText;
                assessment.Measures.push(measureForExec);
            }
        }
        this.setItem(this.values.Key.execAssessment, assessment);
    };

    app.playMeasure = function (student, measureIds) {
        var isExistAudio = this.prepareData(student, measureIds, true);
        if (!isExistAudio)
            window.open(this.values.Url.exec);
    };

    app.viewMeasure = function (studentModel, measureId) {
        var isExistAudio = this.prepareData(studentModel, [measureId], false);
        if (!isExistAudio)
        window.open(this.values.Url.view);
    };

    app.syncStudents = function (studentId, syncNext, startIndex) {
        var that = this;
        if (arguments.length == 1) {
            syncNext = false;
            startIndex = 0;
        }
        if (arguments.length == 2 && typeof (syncNext) == "boolean") {
            startIndex = 0;
        }
        if (arguments.length == 2 && typeof (syncNext) == "number") {
            syncNext = true;
        }

        if (this.network.online() === false) {
            window.showMessage("warning", "Internet_not_available");
            return;
        }
        if (this.network.logged() === false) {
            $.when(window.waitingAlert("warning", "Sync_need_Login")).done(function () {
                app.markSyncing();
                $("#modalLogin").modal("show");
            });
            return;
        }

        for (var i = startIndex; i < this.targets.length; i++) {
            var cachedStudent = this.targets[i];
            var studentModel = this.viewModel.students[i];
            var hasNext = i < (this.viewModel.students.length - 1);
            if (cachedStudent.studentId == studentId) {
                if (cachedStudent.changed) {
                    $.when(that.syncedStudent(cachedStudent, studentModel)).done(function () {
                        if (syncNext && hasNext) {
                            that.syncStudents(that.targetIds[i + 1], syncNext, i + 1);
                        }
                        app.onSynced();
                    });
                    break;
                } else {
                    if (syncNext && hasNext) {
                        this.syncStudents(that.targetIds[i + 1], syncNext, i + 1);
                    }
                }
            }
        }
    };

    app.syncedStudent = function (cachedStudent, studentModel) {
        var that = this;
        var deferred = $.Deferred();

        var assessment = this.getItem(this.values.Key.assessment);
        var params = {
            assessmentId: assessment.AssessmentId,
            studentId: cachedStudent.studentId,
            wave: assessment.Wave.value,
            year: assessment.SchoolYear,
            measures: "",
            items: ""
        };

        var measuresForUpdate = [];
        var itemsForUpdate = [];
        for (var measureId in cachedStudent.measures) {
            var mfu = cachedStudent.measures[measureId];
            if (mfu.changed) {
                measuresForUpdate.push({
                    MeasureId: mfu.measureId,
                    PauseTime: mfu.pauseTime,
                    Status: mfu.status.value,
                    UpdatedOn: mfu.updatedOn,
                    Comment: mfu.comment,
                    TotalScore: mfu.totalScore
                });
                for (var j = 0; j < mfu.items.length; j++) {
                    var ifu = mfu.items[j];
                    itemsForUpdate.push({
                        ItemId: ifu.itemId,
                        IsCorrect: ifu.isCorrect,
                        SelectedAnswers: ifu.selectedAnswers,
                        Goal: ifu.goal,
                        Scored: ifu.scored,
                        PauseTime: ifu.pauseTime,
                        Details: ifu.details,
                        Executed: ifu.executed,
                        LastItemIndex: ifu.lastItemIndex,
                        ResultIndex: ifu.resultIndex
                    });
                }
            }
        }
        if (measuresForUpdate.length) {
            studentModel.syncStatus(this.values.Status.Syncing);
            params.measures = JSON.stringify(measuresForUpdate);
            params.items = JSON.stringify(itemsForUpdate);
            params.schoolId = app.assessment.SchoolId;
            $.ajax({
                url: this.values.Url.sync,
                cache: false,
                type: "post",
                data: params,
                dataType: "json"
            }).success(function (response) {
                if (response.success) {
                    studentModel.syncStatus(app.values.Status.Synced);
                    var successMsg = window.getErrorMessage("Sync_done");
                    successMsg = successMsg.replace("{Firstname}", cachedStudent.firstName)
                        .replace("{Lastname}", cachedStudent.lastName);

                    for (var measureId in cachedStudent.measures) {
                        var cachedMeasure = cachedStudent.measures[measureId];
                        if (cachedMeasure.changed) {
                            cachedMeasure.changed = false;
                        }
                    }
                    cachedStudent.changed = false;
                    that.setItem(app.values.Key.target + cachedStudent.studentId, cachedStudent);

                    that.log("success", successMsg);
                } else {
                    that.log("danger", window.getErrorMessage("Sync_error"));
                    studentModel.syncStatus(app.values.Status.Error);
                }
            }).error(function () {
                that.log("danger", window.getErrorMessage("Sync_error"));
                studentModel.syncStatus(app.values.Status.Error);
            }).complete(function () {
                deferred.resolve();
                that.onSynced();
            });
        } else {
            studentModel.syncStatus(this.values.Status.Cached);
            cachedStudent.changed = false;
            that.setItem(this.values.Key.target + cachedStudent.studentId, cachedStudent);

            that.log("info", "Nothing need to sync");
            deferred.resolve();
        }
        return deferred.promise();
    };

    app.onSynced = function () {
        this.checkChangeStatus();
    };

    app.viewModel = {
        selectedMeasureId: ko.observableArray(),
        getMeasureData: function (student, measureHeader) {
            return student.measureModels[measureHeader.id];
        },
        getMeasureTemplate: function (student, measureHeader) {
            var finalMeasure = student.measureModels[measureHeader.id];
            if (finalMeasure) {
                if (finalMeasure.parent
                    && finalMeasure.isTotal !== true
                    && finalMeasure.parent.visible() == false) {
                    return "_td_Null";
                }
                if (finalMeasure.isTotal) {
                    if (typeof (finalMeasure.goal) == "number" && finalMeasure.goal >= 0) {
                        return "_td_Total_Score";
                    }
                    else {
                        return "_td_Total";
                    }
                }
                if (finalMeasure.status.value() == Cpalls_Status.Initialised) {
                    return "_td_Play";
                }
                if (finalMeasure.status.value() == Cpalls_Status.Paused) {
                    return "_td_Paused";
                }
                if (finalMeasure.status.value() == Cpalls_Status.Locked) {
                    return "_td_Locked";
                }
                if (finalMeasure.status.value() == Cpalls_Status.Finished) {
                    return "_td_Score";
                }
            }
            return "_td_Null";
        },
        switchLock: function (studentMeasure, student) {
            if (studentMeasure.status.value() == Cpalls_Status.Locked) {
                studentMeasure.status.value(Cpalls_Status.Initialised);
                app.saveStudentStatus(student.studentId, studentMeasure);

                studentMeasure.excluded(studentMeasure.excluded() - 1);
                studentMeasure.initialised(studentMeasure.initialised() + 1);

                student.changed(true);
            } else if (studentMeasure.status.value() == Cpalls_Status.Initialised) {
                studentMeasure.status.value(Cpalls_Status.Locked);
                app.saveStudentStatus(student.studentId, studentMeasure);

                studentMeasure.excluded(studentMeasure.excluded() + 1);
                studentMeasure.initialised(studentMeasure.initialised() - 1);

                student.changed(true);
            }
        },
        selectHeader: function (measureHeader) {
            if (measureHeader.isTotal) {
                if (measureHeader.parent) {
                    measureHeader.parent.visible(!measureHeader.parent.visible());
                }
            }
            measureHeader.selected(!measureHeader.selected());
            if (measureHeader.isParent) {
                if (measureHeader.childrenModel && measureHeader.childrenModel.length) {
                    $.each(measureHeader.childrenModel, function (index, childMeasure) {
                        childMeasure.selected(measureHeader.selected());
                    });
                }
            }
        },
        playMeasure: function (studentMeasure, student) {
            app.setItem("keepMeasureSelected", "");
            app.playMeasure(student, [studentMeasure.measureId]);
        },
        playAllMeasure: function (student) {
            var measureIds = [];
            var cannotExecute = 0;
            $.each(app.viewModel.headers.allMeasures, function (index, measure) {
                var studentMeasureInit = student.measures
                    && student.measures[measure.id]
                    && student.measures[measure.id].status.value() !== Cpalls_Status.Initialised;

                if (!measure.isParent
                    && !measure.isTotal
                    && measure.selected()) {

                    if (studentMeasureInit) {
                        cannotExecute++;
                    }

                    measureIds.push(measure.id);
                }
            });
            //console.log("Play All", measureIds,student);
            if (measureIds.length === 0) {
                window.showMessage("warning", "Selected_Measure");
            }
            else if (measureIds.length <= cannotExecute) {
                var msg = window.getErrorMessage("Selected_Measure_All_Done")
                    .replace("{FirstName}", student.firstName).replace("{LastName}", student.lastName);
                window.showMessage("warning", msg);
            } else {
                app.setItem("keepMeasureSelected", measureIds);
                app.playMeasure(student, measureIds);
            }
        },
        syncStudent: function (studentModel) {
            jQuery.ajax(
                 {
                     url: "/Offline/Index/Online",
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
                 if (studentModel.syncStatus() == app.values.Status.Changed) {
                     app.syncStudents(studentModel.studentId, false);
                 } else {
                     studentModel.syncStatus(app.values.Status.Cached);
                 }
             }

             );
        },
        syncAllStudent: function () {
            jQuery.ajax(
                {
                    url: "/Offline/Index/Online",
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
                app.syncStudents(app.targetIds[0], true);
            }

            );


            //app.syncStudents(app.targetIds[0], true);
        },
        network: app.network,
        showSyncAll: ko.computed(function () {
            return this.changed();
        }, app),
        switchExcludedAll: function (measure) {
            var chooseMeasureId = measure.id;
            if (measure.showExcludeBtn()) {
                var sourceStatus = -1;
                var targetStatus = -1;
                var initialisedIncrement = 0;
                var excludedIncrement = 0;
                if (measure.initialised() > 0) {
                    sourceStatus = Cpalls_Status.Initialised;
                    targetStatus = Cpalls_Status.Locked;
                    initialisedIncrement = -1;
                    excludedIncrement = 1;
                } else if (measure.excluded() > 0) {
                    sourceStatus = Cpalls_Status.Locked;
                    targetStatus = Cpalls_Status.Initialised;
                    initialisedIncrement = 1;
                    excludedIncrement = -1;
                }
                for (var i = 0; i < app.targets.length; i++) {
                    var stu = app.targets[i];
                    var stuModel = app.viewModel.studentById[stu.studentId];

                    for (var measureId in stu.measures) {
                        var measure = stu.measures[measureId];
                        if (measure.measureId == chooseMeasureId && measure.status.value == sourceStatus) {
                            var measureModel = stuModel.measureModels[measureId];
                            measureModel.status.value(targetStatus);

                            app.saveStudentStatus(stu.studentId, measureModel);

                            stuModel.changed(true);
                            stuModel.syncStatus(app.values.Status.Changed);
                            app.checkChangeStatus(true);

                            measureModel.initialised(measureModel.initialised() + initialisedIncrement);
                            measureModel.excluded(measureModel.excluded() + excludedIncrement);
                        }
                    }
                }
            }
        },
        viewResult: function (studentModel, measureModel) {
            app.viewMeasure(studentModel, measureModel.measureId);
        }
    };

    var baseInit = app.init;
    app.init = function () {
        var that = this;
        baseInit.apply(app);
        if (this.status > this.values.Status.None) {

            var assessment = this.assessment;
            $.extend(this.viewModel, {
                assessment: assessment.Name,
                community: assessment.CommunityName,
                school: assessment.SchoolName,
                year: assessment.SchoolYear,
                wave: assessment.Wave.text,
                className: assessment.Class.Name,
                teachers: assessment.Class.Teachers,
                schoolYear: assessment.SchoolYear,
                LegendUIFilePath: assessment.LegendUIFilePath,
                LegendUIText: assessment.LegendUIText,
                LegendUITextPosition: assessment.LegendUITextPosition
            });
            var parentHeader = new MeaureHeaderModel();
            $.each(assessment.Measures, function (index, measure) {
                parentHeader.add(measure);
            });
            parentHeader.processLastParent();

            this.viewModel.headers = parentHeader;

            this.viewModel.students = [];
            this.viewModel.studentById = {};
            if (this.targetIds && this.targetIds.length) {
                $.each(this.targetIds, function (index, studentId) {
                    var student = that.getItem(that.values.Key.target + studentId);
                    var studentModel = new StudentModel(student, parentHeader, that);
                    if (studentModel.changed()) {
                        that.changed(true);
                    }
                    that.viewModel.students.push(studentModel);
                    that.viewModel.studentById[studentId] = studentModel;
                });
            }

            app.network.logged.subscribe(function (logged) {
                if (logged) {
                    if (app.shouldStartSyncing() && app.viewModel.showSyncAll()) {
                        app.viewModel.syncAllStudent();
                    }
                }
            });
        }
    }

    return app;
}

function MeasureHeaderParent(id, name, total, isParent, totalScored, children, visible) {
    this.id = id;
    this.name = name;
    this.total = total;
    this.totalScored = totalScored;
    this.isParent = isParent;

    this.children = children;
    this.childrenModel = [];
    this.visible = ko.observable(visible || true);
    this.selected = ko.observable(false);

    this.excluded = ko.observable(0);
    this.initialised = ko.observable(0);
    this.showExcludeBtn = ko.computed(function () {
        return this.excluded() + this.initialised() > 0;
    }, this);

    this.parent = null;
    this.isTotal = false;
}
function MeasureHeader(id, name, total, totalScored) {
    this.id = id;
    this.name = name;
    this.total = total;
    this.totalScored = totalScored;
    this.parent = null;
    this.isTotal = false;
    this.isFirstOfParent = false;
    this.isLastOfParent = false;
    this.selected = ko.observable(false);

    this.excluded = ko.observable(0);
    this.initialised = ko.observable(0);
    this.showExcludeBtn = ko.computed(function () {
        return this.excluded() + this.initialised() > 0;
    }, this);
}
function MeaureHeaderModel() {
    this.allMeasures = [];
    this.measureByKey = {};
    this.parents = [];
    this.measures = [];
    var lastParent = null;
    this.add = function (measure) {
        //console.log(measure);
        var id = measure.MeasureId;
        var name = measure.Name;
        var totalScore = measure.TotalScore;
        var totalScored = measure.TotalScored;
        var isParent = measure.IsParent;
        var children = measure.Children;
        var parentId = measure.Parent.ID;

        if (parentId == 1) {
            //console.log("new parent",lastParent);
            if (lastParent) {
                if (lastParent.isParent) {
                    lastParent.children++;
                    var mea = new MeasureHeader(lastParent.id, "Total", lastParent.total, lastParent.totalScored);
                    //console.log("add total",mea," for ", lastParent);
                    mea.parent = lastParent;
                    lastParent.childrenModel.push(mea);
                    mea.isTotal = true;
                    mea.isLastOfParent = true;
                    this.measures.push(mea);
                    this.allMeasures.push(mea);
                    this.measureByKey[mea.id] = mea;
                }
                lastParent = null;
            }
            lastParent = new MeasureHeaderParent(id, name, totalScore, isParent, totalScored, children, true);
            this.parents.push(lastParent);
            if (!isParent) {
                this.allMeasures.push(lastParent);
                this.measureByKey[lastParent.id] = lastParent;
            }
        } else if (parentId > 1) {
            //console.log("new child", measure);
            if (lastParent == null) {
                console.error("Local data error.");
            }

            var mea = new MeasureHeader(id, name, totalScore, totalScored);
            mea.parent = lastParent;
            if (lastParent.childrenModel.length == 0)
                mea.isFirstOfParent = true;
            lastParent.childrenModel.push(mea);
            this.measures.push(mea);
            this.allMeasures.push(mea);
            this.measureByKey[mea.id] = mea;
        }
    };
    this.processLastParent = function () {
        if (lastParent != null) {
            if (lastParent.isParent) {
                lastParent.children++;
                var mea = new MeasureHeader(lastParent.id, "Total", lastParent.total, lastParent.totalScored);
                //console.log("add total",mea," for ", lastParent);
                mea.parent = lastParent;
                mea.isTotal = true;
                mea.isLastOfParent = true;
                lastParent.childrenModel.push(mea);
                this.measures.push(mea);
                this.allMeasures.push(mea);
                this.measureByKey[mea.id] = mea;
            }
            lastParent = null;
        }
    };
}

function StudentModel(cachedStudent, parentHeader, offlineApp) {
    var that = this;
    $.extend(that, cachedStudent);
    this.measureModels = {};
    $.each(cachedStudent.measures, function (measureId, measure) {
        var header = parentHeader.measureByKey[measure.measureId];
        if (header) {
            var model = new StudentMeasureModel(cachedStudent, header, offlineApp);
            that.measureModels[model.measureId] = model;
        }
    });
    if (cachedStudent.changed) {
        this.syncStatus = ko.observable(offlineApp.values.Status.Changed);
    } else {
        this.syncStatus = ko.observable(offlineApp.values.Status.Cached);
    }
    this.changed = ko.observable(this.changed);
}

function StudentMeasureModel(student, measureHeader, offlineApp) {
    var that = this;

    $.extend(that, measureHeader, student.measures[measureHeader.id]);
    if (typeof (this.status.value) == "number") {
        this.status.value = ko.observable(this.status.value);
    }
    if (this.status.value() == Cpalls_Status.Locked) {
        measureHeader.excluded(measureHeader.excluded() + 1);
    }
    if (this.status.value() == Cpalls_Status.Initialised) {
        measureHeader.initialised(measureHeader.initialised() + 1);
    }

    this.showText = function () {
        if (typeof (this.goal) == "number" && !isNaN(+this.goal)) {
            this.goal = +this.goal;
        }
        if (this.goal || typeof (this.goal) == "number") {
            return this.goal;
        } else if (this.status.value() == Cpalls_Status.Finished) {
            this.goal = 0;
            return this.goal;
        }
        return "-";
    };

    if (this.changed && offlineApp) {
        outerloop:
            for (var i = 0; i < offlineApp.assessment.Measures.length; i++) {
                var measure = offlineApp.assessment.Measures[i];
                if (measure.MeasureId == measureHeader.id) {
                    var cutOffScores = measure.CutOffScores;
                    for (var j = 0; j < cutOffScores.length; j++) {
                        var cutOffScore = cutOffScores[j];
                        if (cutOffScore.BenchmarkId == 0 || cutOffScore.BenchmarkColor == "" || cutOffScore.Wave.value != offlineApp.viewModel.wave)
                            continue;
                        var stuAge = student.age;
                        if (stuAge >= cutOffScore.FromAge && stuAge < cutOffScore.ToAge && this.goal >= cutOffScore.LowerScore && this.goal <= cutOffScore.HigherScore) {
                            this.benchmarkId = cutOffScore.BenchmarkId;
                            this.benchmarkColor = cutOffScore.BenchmarkColor;
                            this.benchamrkText = cutOffScore.BenchmarkLabel;
                            this.ageGroup = "testAge1111";
                            break outerloop;
                        }
                    }
                }
            }
    }

    //if (!this.hasCutoffScores) {
    //    this.className = ReportTheme.Missing_Bentchmark_ClassName;
    //}
    //else if (this.benchmark < 0) {
    //    this.className = this.lightColor ? ReportTheme.TE3_Light_ClassName : ReportTheme.TE3_ClassName;
    //}
    //else if (this.goal < this.benchmark) {
    //    if (student.age >= 4) {
    //        this.className = this.lightColor ? ReportTheme.GE4_Light_ClassName : ReportTheme.GE4_ClassName;
    //    }
    //    else {
    //        this.className = this.lightColor ? ReportTheme.TE4_Light_ClassName : ReportTheme.TE4_ClassName;
    //    }
    //}
    //else {
    //    this.className = this.lightColor ? ReportTheme.Passed_Light_ClassName : ReportTheme.Passed_ClassName;
    //}
}

var ReportTheme = {
    Missing_Bentchmark_ClassName: "cpalls_no_benchmark",
    GE4_ClassName: "cpalls_four",
    GE4_Light_ClassName: "cpalls_four_light",
    TE4_ClassName: "cpalls_three",
    TE4_Light_ClassName: "cpalls_three_light",
    TE3_ClassName: "cpalls_three_less",
    TE3_Light_ClassName: "cpalls_three_less_light",
    Passed_ClassName: "cpalls_normal",
    Passed_Light_ClassName: "cpalls_normal_light"
}