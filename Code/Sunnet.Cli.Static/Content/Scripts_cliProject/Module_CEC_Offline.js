var CecStatus = {
    None: 0,
    Complete: 1,
    OfflineComplete: 2
}


function getCecOfflineApp(userId) {
    if (typeof (getOfflineAppForModule) !== "function") {
        throw Error("need reference js file: Module_Offline.js");
    }
    // require Module_Offline.js
    var app = getOfflineAppForModule("Cec", userId, {
        checkOnline: "/Cec/Offline/Online",
        sync: "/Cec/Offline/Sync",
        cec: "/Cec/Offline/Measure",
        report: "/Cec/Offline/CECReport",
        onLineUrlPrefix: "/Cec/CEC?assessmentId="
    });
    app.values.cecStatus = CecStatus;
    app.appendKey("Wave", "Wave");          // app.values.Key.Wave
    app.appendKey("ChangeCount", "ChangeCount");   // app.values.Key.ChangeCount
    app.values.minDate = "01/01/1753";
    app.values.WaveTranslater = {
        "BOY": 1,
        "MOY": 2,
        "EOY": 3,
        "1": "BOY",
        "2": "MOY",
        "3": "EOY"
    };
    // global configuration after data saved.
    var baseWriteData = app.writeLocalData;

    // write cpalls data
    app.writeLocalData = function (dataSource) {
        this.clearCachedData();

        baseWriteData.apply(this);
       
        this.setItem(this.values.Key.onlineUrl, this.values.Url.onLineUrlPrefix + dataSource.assessment.id);
        var teacherIds = [];
        for (var i = 0; i < dataSource.teachers.length; i++) {
            var teacher = dataSource.teachers[i];
            teacherIds.push(teacher.ID);
            this.setItem(this.values.Key.target + teacher.ID, teacher);
        }
        this.setItem(this.values.Key.targets, teacherIds);
        this.setItem(this.values.Key.assessment, dataSource.assessment);
    }

    app.viewModel = {
        network: app.network,
        viewResult: function (studentModel, measureModel) {
            app.viewMeasure(studentModel, measureModel.measureId);
        },
        teacher: null,
        teachers: null,
        cecitems: null,
        changed: ko.observable(false),
        changedCount: ko.observable(0),

        getBOYUrl: function (data) {
            app.setItem(app.values.Key.workingTarget, data.ID);
            app.setItem(app.values.Key.Wave, 1);
            location.href = app.values.Url.cec;
        },
        getBOYReport: function (data) {
            app.setItem(app.values.Key.workingTarget, data.ID);
            app.setItem(app.values.Key.Wave, 1);
            location.href = app.values.Url.report;
        },
        getMOYUrl: function (data) {
            app.setItem(app.values.Key.workingTarget, data.ID);
            app.setItem(app.values.Key.Wave, 2);
            location.href = app.values.Url.cec;
        },
        getMOYReport: function (data) {
            app.setItem(app.values.Key.workingTarget, data.ID);
            app.setItem(app.values.Key.Wave, 2);
            location.href = app.values.Url.report;
        },
        getEOYUrl: function (data) {
            app.setItem(app.values.Key.workingTarget, data.ID);
            app.setItem(app.values.Key.Wave, 3);
            location.href = app.values.Url.cec;
        },
        getEOYReport: function (data) {
            app.setItem(app.values.Key.workingTarget, data.ID);
            app.setItem(app.values.Key.Wave, 3);
            location.href = app.values.Url.report;
        },
        save: function (data) {
            //begin Validate
            var assessmentDate = $.trim($("#assessmentDate").val());
            if (assessmentDate.length == 0) {
                window.showMessage("hint", "EnterAssessmentDate");
                return false;
            }

            var itemMsg = "";
            $.each($(".divAnswer"), function (index, item) {
                if ($(item).find("[_isRequired=true]").is(":checked") == false) {
                    $.each($(item).find("[_isRequired=true]"), function (childIndex, childItem) {
                        //var itemName = $(this).attr("_itemName");
                        //itemMsg = itemMsg + itemName + "; ";
                        itemMsg = itemMsg + (index + 1) + "; ";
                        return false;
                    });
                }
            });
            if (itemMsg.length > 0) {
                itemMsg = itemMsg.substring(0, itemMsg.lastIndexOf(';'));
                itemMsg = "<br\><span style='color:#000'>The following items require a score before proceeding:<br /><br />Item " + itemMsg + "<span>";
                window.showMessage("warning", itemMsg);
                return false;
            }
            //end Validate

            var tmpTeacher = app.viewModel.teacher;
            var tmpWave = app.getItem(app.values.Key.Wave);
            var waveName = app.values.WaveTranslater[tmpWave];
            var tmpHistory = [];
            var dateObj = new Date();
            var date = dateObj.Format("MM/dd/yyyy HH:mm:ss");
            $.each($(":radio"), function (index, item) {
                if ($(item).prop("checked")) {
                    var tmpcecResult = {};
                    tmpcecResult.ItemId = $(item).attr("parentId");
                    tmpcecResult.AnswerId = $(item).attr("id");
                    tmpcecResult.Score = $(item).attr("score");
                    tmpcecResult.CreatedOn = date;
                    tmpcecResult.UpdatedOn = date;
                    tmpHistory.push(tmpcecResult);
                }
            });
            tmpTeacher[waveName] = assessmentDate;
            tmpTeacher[waveName + "Status"] = app.values.cecStatus.OfflineComplete;
            tmpTeacher[waveName + "History"] = tmpHistory;
            tmpTeacher.changed = true;

            app.setItem(app.values.Key.target + tmpTeacher.ID, tmpTeacher);

            window.showMessage("success");
            app.viewModel.status(app.values.cecStatus.OfflineComplete);
            app.checkChangeStatus(true);
            app.resumePin();
        },

        syncAll: function () {
            app.resumePin();

            jQuery.ajax(
                {
                    url: "/Cec/Offline/Online",
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
                if (!app.network.online())
                {
                    window.showMessage("warning", "Internet_not_available");
                    return;
                }
                if (!app.network.logged())
                {
                    $.when(window.waitingAlert("warning", "Sync_need_Login")).done(function ()
                    {
                       // app.markSyncing();
                        $("#modalLogin").modal("show");
                    });
                    return;
                }

                for (var i = 0 ; i < app.targets.length ; i++)
                {
                    var tmpTeacher = app.targets[i];
                    if (tmpTeacher.changed)
                    {
                        app.viewModel.sync(tmpTeacher);
                    } //if end
                }//for end
            }

            );


         
        },
        // 需要优化
        sync: function (teacherData)
        {
            if (!app.network.logged()) {
                jQuery.ajax(
                   {
                       url: "/Cec/Offline/Online",
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


                   if (!app.network.online()) {
                       window.showMessage("warning", "Internet_not_available");
                       return;
                   }

                   if (!app.network.logged()) {
                       $.when(window.waitingAlert("warning", "Sync_need_Login")).done(function () {
                           app.markSyncing();
                           $("#modalLogin").modal("show");
                       });
                       return;
                   }

                   var tmpAssessmentId = app.assessment.id;
                   teacherData.syncStatus(OfflineStatus.Syncing); //syncing

                   $.when(
                       $.post(app.values.Url.sync, {
                           teacherId: teacherData.ID,
                           assessmentId: tmpAssessmentId,
                           boyhistory: JSON.stringify(teacherData.BOYHistory),
                           boydate: teacherData.BOY,
                           boyStatus: teacherData.BOYStatus,

                           moyhistory: JSON.stringify(teacherData.MOYHistory),
                           moydate: teacherData.MOY,
                           moyStatus: teacherData.MOYStatus,

                           eoyhistory: JSON.stringify(teacherData.EOYHistory),
                           eoydate: teacherData.EOY,
                           eoyStatus: teacherData.EOYStatus
                       })
                       ).done(function (result) {
                           result = JSON.parse(result);
                           if (result.Success == 1) {
                               teacherData.syncStatus(OfflineStatus.Synced);
                               var msg = window.getErrorMessage("Cot_Sync_done");
                               app.log("success", msg.replace("{Firstname}", teacherData.FirstName).replace("{Lastname}", teacherData.LastName));
                               teacherData.changed = false;
                               app.setItem(app.values.Key.target + teacherData.ID, teacherData);
                               app.checkChangeStatus();
                           } else {
                               teacherData.syncStatus(OfflineStatus.Error);
                               app.log("danger", window.getErrorMessage("Sync_error"));
                           }
                       })
                   .fail(function (result) {
                       result = JSON.parse(result);
                       teacherData.syncStatus(OfflineStatus.Error);
                       app.log("danger", window.getErrorMessage("Sync_error"));
                   });
               }

               );
            }
            else {

                if (!app.network.online()) {
                    window.showMessage("warning", "Internet_not_available");
                    return;
                }

                if (!app.network.logged()) {
                    $.when(window.waitingAlert("warning", "Sync_need_Login")).done(function () {
                        app.markSyncing();
                        $("#modalLogin").modal("show");
                    });
                    return;
                }

                var tmpAssessmentId = app.assessment.id;
                teacherData.syncStatus(OfflineStatus.Syncing); //syncing

                $.when(
                    $.post(app.values.Url.sync, {
                        teacherId: teacherData.ID,
                        assessmentId: tmpAssessmentId,
                        boyhistory: JSON.stringify(teacherData.BOYHistory),
                        boydate: teacherData.BOY,
                        boyStatus: teacherData.BOYStatus,

                        moyhistory: JSON.stringify(teacherData.MOYHistory),
                        moydate: teacherData.MOY,
                        moyStatus: teacherData.MOYStatus,

                        eoyhistory: JSON.stringify(teacherData.EOYHistory),
                        eoydate: teacherData.EOY,
                        eoyStatus: teacherData.EOYStatus
                    })
                    ).done(function (result) {
                        result = JSON.parse(result);
                        if (result.Success == 1) {
                            teacherData.syncStatus(OfflineStatus.Synced);
                            var msg = window.getErrorMessage("Cot_Sync_done");
                            app.log("success", msg.replace("{Firstname}", teacherData.FirstName).replace("{Lastname}", teacherData.LastName));
                            teacherData.changed = false;
                            app.setItem(app.values.Key.target + teacherData.ID, teacherData);
                            app.checkChangeStatus();
                        } else {
                            teacherData.syncStatus(OfflineStatus.Error);
                            app.log("danger", window.getErrorMessage("Sync_error"));
                        }
                    })
                .fail(function (result) {
                    result = JSON.parse(result);
                    teacherData.syncStatus(OfflineStatus.Error);
                    app.log("danger", window.getErrorMessage("Sync_error"));
                });
            }

        },
        showSyncAll: ko.computed(function () {
            return app.changed();
        })
    };

    var baseInit = app.init;
    app.init = function () {
        baseInit.apply(app);
        var i, j, k, l, m;
        if (this.status > this.values.Status.None) {
            var workingTeacherID = this.getItem(this.values.Key.workingTarget);
            var wave = this.getItem(this.values.Key.Wave);

            for (i = 0 ; i < this.targets.length; i++) {
                var teacher = this.targets[i];
                if (teacher.BOYStatus == this.values.cecStatus.OfflineComplete
                    || teacher.MOYStatus == this.values.cecStatus.OfflineComplete
                    || teacher.EOYStatus == this.values.cecStatus.OfflineComplete) {
                    teacher.ChangeStauts = ko.observable(this.values.cecStatus.Complete);
                } else {
                    teacher.ChangeStauts = ko.observable(this.values.cecStatus.None);
                }
                if (teacher.changed) {
                    teacher.syncStatus = ko.observable(OfflineStatus.Changed);

                    this.changed(true);
                } else {
                    teacher.syncStatus = ko.observable(OfflineStatus.Cached);
                    teacher.changed = false;
                }
                if (teacher.ID === workingTeacherID) {
                    this.viewModel.teacher = teacher;
                    // BOYHistory, MOYHistory, EOYHistory
                    var waveName = this.values.WaveTranslater[wave];
                    var tmpHistory = teacher[waveName + "History"];
                    for (m = 0 ; m < this.assessment.items.length ; m++) {
                        var measure = this.assessment.items[m];
                        if (measure.Childer != null) {
                            for (j = 0; j < measure.Childer.length; j++) {
                                for (k = 0; k < measure.Childer[j].List.length; k++) {
                                    for (l = 0; l < tmpHistory.length ; l++) {
                                        if (tmpHistory[l].ItemId == measure.Childer[j].List[k].ItemId) {
                                            measure.Childer[j].List[k].Score = tmpHistory[l].Score;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else {
                            for (k = 0; k < measure.List.length; k++) {
                                for (l = 0; l < tmpHistory.length ; l++) {
                                    if (tmpHistory[l].ItemId == measure.List[k].ItemId) {
                                        measure.List[k].Score = tmpHistory[l].Score;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    var status = teacher[this.values.WaveTranslater[wave] + "Status"];
                    this.viewModel.status = ko.observable(status);

                    this.viewModel.reportTitle = this.getAssessmentName(this.assessment.name)+" " + waveName + " for " + teacher.FirstName + " " + teacher.LastName
                        + " at STANTON LC T2<br> School Year: " + this.assessment.year;
                }
            }

            $.extend(this.viewModel, {
                assessmentName: this.assessment.name,
                communityName: this.assessment.communityName,
                schoolName: this.assessment.schoolName,
                year: this.assessment.year,
                itemWave: this.values.WaveTranslater[wave],
                wave: wave
            });

            this.viewModel.changed = app.changed;

            this.viewModel.teachers = this.targets;
            this.viewModel.cecitems = this.assessment.items;


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
