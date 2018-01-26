var ChangeStatus = {
    None: 0,
    Edited: 1,
    Deleted: 2,
    Added: 3
}

var ItemType = {
    Structural: 1,
    Process: 2
}

var Category = {};
Category[1] = { text: "Director And Staff Qualifications And Training", value: 1 };
Category[2] = { text: "Caregiver-Child Interactions", value: 2 };
Category[3] = { text: "Curriculum", value: 3 };
Category[4] = { text: "Nutrition, Indoor/Outdoor Environment", value: 4 };
Category[5] = { text: "Parent Education Involvement", value: 5 };


var AssessmentType = {};
AssessmentType[0] = { text: "", value: 0 };
AssessmentType[1] = { text: "Initial", value: 1 };
AssessmentType[2] = { text: "Recertification", value: 2 };
AssessmentType[3] = { text: "Facility Moves", value: 3 };
AssessmentType[4] = { text: "Facility Expansions and Splits", value: 4 };
AssessmentType[5] = { text: "Star Level Evaluation", value: 5 };
AssessmentType[6] = { text: "Annual Monitoring", value: 6 };

var Star = {};
Star[1] = { text: "Below 2 ★", value: 1 };
Star[2] = { text: "2 ★", value: 2 };
Star[3] = { text: "3 ★", value: 3 };
Star[4] = { text: "4 ★", value: 4 };

function getTrsOfflineApp(userId) {
    if (typeof (getOfflineAppForModule) !== "function") {
        throw Error("need reference js file: Module_Offline.js");
    }
    // require Module_Offline.js
    var app = getOfflineAppForModule("Trs", userId, {
        checkOnline: "/Trs/Offline/Online",
        sync: "/Trs/Offline/Sync",
        onLineUrlPrefix: "/Trs",
        index: "/Trs/Offline",
        school: "/Trs/Offline/School",
        assessment: "/Trs/Offline/Assessment",
        verifiedStar: "/Trs/Offline/VerifiedStar",
        preview: "/Trs/Offline/Preview"
    });
    app.values.changeStatus = ChangeStatus;
    app.appendKey("WorkingTargetAssessment", "WorkingTargetAssessment");  //添加key时，需要添加扩展
    app.values.minDate = "01/01/1753";
    app.targetByAssKey = {};
    // global configuration after data saved.
    var baseWriteData = app.writeLocalData;

    // write cpalls data
    app.writeLocalData = function (dataSource) {
        this.clearCachedData();

        baseWriteData.apply(this);

        this.setItem(this.values.Key.onlineUrl, this.values.Url.onLineUrlPrefix);
        var schools = dataSource.schools;
        var schoolIds = [];
        for (var i = 0; i < schools.length; i++) {
            var school = schools[i];
            schoolIds.push(school.ID);
            for (var j = 0; j < school.AssessmentList.length; j++) {
                var assessment = school.AssessmentList[j];
                assessment.guid = window.guid("_Trs_Offline_");
                assessment.IsSync = false;
            }
            this.setItem(this.values.Key.target + school.ID, school);
        }
        this.setItem(this.values.Key.targets, schoolIds);
        this.setItem(this.values.Key.assessment, dataSource.assessment);
    },

    app.viewModel = {
        trsMinDate: new Date("2015-09-01"),
        network: app.network,
        currentSchool: {},
        currentAssessment: {},
        currentAssessmentItems: [],
        schools: [],

        goIndex: function () {
            location.href = app.values.Url.index
        },

        schoolIndex: function () {
            if (app.getItem(app.values.Key.WorkingTargetAssessment) || app.getItem(app.values.Key.WorkingTargetAssessment) == 0) {
                app.removeItem(app.values.Key.WorkingTargetAssessment);
            }
            if (app.getItem(app.values.Key.workingTarget)) {
                app.removeItem(app.values.Key.workingTarget);
            }
            location.href = app.values.Url.index
        },

        getSchoolUrl: function (data) {
            app.setItem(app.values.Key.workingTarget, data.ID);
            if (app.getItem(app.values.Key.WorkingTargetAssessment) || app.getItem(app.values.Key.WorkingTargetAssessment) == 0) {
                app.removeItem(app.values.Key.WorkingTargetAssessment);
            }
            location.href = app.values.Url.school;
        },

        getAssessmentUrl: function (data) {
            app.setItem(app.values.Key.WorkingTargetAssessment, data.guid);
            location.href = app.values.Url.assessment;
        },

        newAssessmentUrl: function () { //新增Assessment     
            app.setItem(app.values.Key.WorkingTargetAssessment, 0);
            location.href = app.values.Url.assessment;
        },

        syncAll: function () {
            app.resumePin();
            jQuery.ajax(
              {
                  url: "/Trs/Offline/Online",
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

              for (var i = 0 ; i < app.targets.length ; i++) {
                  var tmpSchool = app.targets[i];
                  if (tmpSchool.ChangeStatus) {
                      app.viewModel.sync(tmpSchool);
                  } //if end
              }//for end
          }

          );

           //////////////

          
        },

        sync: function (schoolData) {
            if (!app.network.logged()) {
                jQuery.ajax(
             {
                 url: "/Trs/Offline/Online",
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

             schoolData.syncStatus(OfflineStatus.Syncing); //syncing

             $.when(
                 $.post(app.values.Url.sync, {
                     schoolId: schoolData.ID,
                     trsTaStatus: schoolData.TrsTaStatus,
                     recertificationBy: schoolData.RecertificationBy,
                     starStatus: schoolData.StarStatus.value,
                     starDate: schoolData.StarDate,
                     verifiedStar: schoolData.VerifiedStar.value,
                     trsLastStatusChange: schoolData.TrsLastStatusChange,
                     addAssessments: JSON.stringify(schoolData.AssessmentList.filter(
                         function (obj) { return !(obj.IsSync) && obj.ChangeStatus == ChangeStatus.Added })),
                     deleteAssessments: JSON.stringify(schoolData.AssessmentList.filter(
                         function (obj) { return !(obj.IsSync) && obj.ChangeStatus == ChangeStatus.Deleted })),
                     updatAssessments: JSON.stringify(schoolData.AssessmentList.filter(
                         function (obj) { return !(obj.IsSync) && obj.ChangeStatus == ChangeStatus.Edited }))
                 })
                 ).done(function (result) {
                     if (result == "True") {
                         schoolData.syncStatus(OfflineStatus.Synced);
                         var msg = window.getErrorMessage("Trs_Sync_done");
                         app.log("success", msg.replace("{ID}", schoolData.Name));
                         $.each(schoolData.AssessmentList, function () {
                             this.IsSync = true;
                         })
                         schoolData.ChangeStatus = false;
                         app.setItem(app.values.Key.target + schoolData.ID, schoolData);
                         app.viewModel.showSyncAll(app.targets.filter(function (obj) { return obj.ChangeStatus }).length > 0);
                     } else {
                         schoolData.syncStatus(OfflineStatus.Error);
                         app.log("danger", window.getErrorMessage("Sync_error"));
                     }
                 })
             .fail(function (result) {
                 schoolData.syncStatus(OfflineStatus.Error);
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

                schoolData.syncStatus(OfflineStatus.Syncing); //syncing

                $.when(
                    $.post(app.values.Url.sync, {
                        schoolId: schoolData.ID,
                        trsTaStatus: schoolData.TrsTaStatus,
                        recertificationBy: schoolData.RecertificationBy,
                        starStatus: schoolData.StarStatus.value,
                        starDate: schoolData.StarDate,
                        verifiedStar: schoolData.VerifiedStar.value,
                        trsLastStatusChange: schoolData.TrsLastStatusChange,
                        addAssessments: JSON.stringify(schoolData.AssessmentList.filter(
                            function (obj) { return !(obj.IsSync) && obj.ChangeStatus == ChangeStatus.Added })),
                        deleteAssessments: JSON.stringify(schoolData.AssessmentList.filter(
                            function (obj) { return !(obj.IsSync) && obj.ChangeStatus == ChangeStatus.Deleted })),
                        updatAssessments: JSON.stringify(schoolData.AssessmentList.filter(
                            function (obj) { return !(obj.IsSync) && obj.ChangeStatus == ChangeStatus.Edited }))
                    })
                    ).done(function (result) {
                        if (result == "True") {
                            schoolData.syncStatus(OfflineStatus.Synced);
                            var msg = window.getErrorMessage("Trs_Sync_done");
                            app.log("success", msg.replace("{ID}", schoolData.Name));
                            $.each(schoolData.AssessmentList, function () {
                                this.IsSync = true;
                            })
                            schoolData.ChangeStatus = false;
                            app.setItem(app.values.Key.target + schoolData.ID, schoolData);
                            app.viewModel.showSyncAll(app.targets.filter(function (obj) { return obj.ChangeStatus }).length > 0);
                        } else {
                            schoolData.syncStatus(OfflineStatus.Error);
                            app.log("danger", window.getErrorMessage("Sync_error"));
                        }
                    })
                .fail(function (result) {
                    schoolData.syncStatus(OfflineStatus.Error);
                    app.log("danger", window.getErrorMessage("Sync_error"));
                });

            }

          
        },
        saveAssessment: function (data, model, taStatus) {
            var targetAssessment = app.targetByAssKey[app.getItem(app.values.Key.WorkingTargetAssessment)];
            app.changeModeltoEntity(data, targetAssessment ? targetAssessment.guid : 0, model, taStatus);
        },
        deleteAssessment: function (model) //assessment.guid
        {
            jQuery.when(waitingConfirm("Are you sure you want to delete this assessment?", "Yes", "No")).done(function () {
                var assessmentId = model.guid;
                var targetSchoolId = app.getItem(app.values.Key.workingTarget);
                var currentSchool = app.targetByKey[targetSchoolId];
                var assessment = app.targetByAssKey[assessmentId];
                if (assessment.Id) {   //Id大于0时，表示在线时已存在的数据
                    assessment.ChangeStatus = ChangeStatus.Deleted;
                    assessment.IsSync = false;
                    app.setItem(app.values.Key.target + targetSchoolId, currentSchool);
                }
                else {  //Id等于0时，表示离线时添加的数据,可直接删除
                    currentSchool.AssessmentList = currentSchool.AssessmentList.filter(function (obj) { return obj.guid != assessmentId });
                    app.setItem(app.values.Key.target + targetSchoolId, currentSchool);
                }
                $("#" + assessmentId).remove();
                app.setItem(app.values.Key.WorkingTargetAssessment, "");
                window.showMessage("success");
            });
        },
        showSyncAll: ko.observable(false)
    };

    var baseInit = app.init;
    app.init = function (url) {

        if (url && url == app.values.Url.assessment
            && !app.getItem(app.values.Key.WorkingTargetAssessment) && app.getItem(app.values.Key.WorkingTargetAssessment) != 0) {
            app.viewModel.goIndex();
        }
        if (url && url == app.values.Url.school && !app.getItem(app.values.Key.workingTarget)) {
            app.viewModel.goIndex();
        }

        baseInit.apply(app);

        if (this.status > this.values.Status.None) {
            this.viewModel.schools = this.targets;
            $.each(this.viewModel.schools, function (index, school) {
                school.ChangeStatus = school.AssessmentList.filter(function (obj) {
                    return !(obj.IsSync) && obj.ChangeStatus && obj.ChangeStatus != ChangeStatus.None
                }).length > 0;
                if (school.ChangeStatus)
                    school.syncStatus = ko.observable(OfflineStatus.Changed);
                else
                    school.syncStatus = ko.observable(OfflineStatus.Cached);
            })
            app.viewModel.showSyncAll(app.targets.filter(function (obj) { return obj.ChangeStatus }).length > 0);
            var targetSchoolId = app.getItem(app.values.Key.workingTarget);
            var currentSchool = app.targetByKey[targetSchoolId];
            if (currentSchool) {
                app.viewModel.currentSchool = currentSchool;
                for (var j = 0; j < currentSchool.AssessmentList.length; j++) {  //初始化Assessment时，构造targetByAssKey对象
                    var assessment = currentSchool.AssessmentList[j];
                    app.targetByAssKey[assessment.guid] = assessment;
                }
            }

            app.viewModel.currentAssessment = app.formatModel();

            app.network.logged.subscribe(function (logged) {
                if (logged) {
                    if (app.shouldStartSyncing() && app.viewModel.showSyncAll()) {
                        app.viewModel.syncAll();
                    }
                }
            });
        }
    };
    //将data转换成带有基本信息的model
    app.formatModel = function () {
        app.viewModel.currentAssessmentItems = [];
        var workingTargetAssessment = app.getItem(app.values.Key.WorkingTargetAssessment);
        var targetSchoolId = app.getItem(app.values.Key.workingTarget);
        var currentSchool = app.targetByKey[targetSchoolId];
        var newData = {};
        if (workingTargetAssessment || workingTargetAssessment == 0) {
            var data = {};
            if (workingTargetAssessment == 0)  //新增Assessment时
                data = JSON.parse(JSON.stringify(currentSchool.NewAssessment));
            else //编辑Assessment时
                data = JSON.parse(JSON.stringify(app.targetByAssKey[workingTargetAssessment]));

            if (data) {
                var assessment = app.getItem(app.values.Key.assessment);
                newData = data;
                $.each(newData.Categories_Offline, function (index, category) {
                    $.each(category, function (subIndex, subCategory) {
                        $.each(subCategory, function (itemIndex, item) {
                            var oldItem = item;
                            var itemId = this.ItemId;
                            var newItem = assessment.Items.filter(function (obj) { return obj.ItemId == itemId })[0];
                            $.extend(item, newItem, { Id: oldItem.Id }, { ItemId: oldItem.ItemId }  //将新属性覆盖
                                   , { AnswerId: oldItem.AnswerId }, { ClassId: oldItem.ClassId }
                                   , { Comments: oldItem.Comments }, { Filled: oldItem.AnswerId > 0 });
                            if (item.AnswerId > 0) {
                                var answer = assessment.Answers.filter(function (obj) { return obj.Id == item.AnswerId });
                                if (answer.length > 0) {
                                    item.AnswerText = answer[0].Text;
                                    item.Score = answer[0].Score;
                                }
                            }

                            app.viewModel.currentAssessmentItems.push(item);
                        })
                    })
                })
                $.each(newData.Classes, function (index, calss) {
                    $.each(calss.Categories_Offline, function (index, category) {
                        $.each(category, function (subIndex, subCategory) {
                            $.each(subCategory, function (itemIndex, item) {
                                var oldItem = item;
                                var itemId = this.ItemId;
                                var newItem = assessment.Items.filter(function (obj) { return obj.ItemId == itemId })[0];
                                $.extend(item, newItem, { Id: oldItem.Id }, { ItemId: oldItem.ItemId }  //将新属性覆盖
                                    , { AnswerId: oldItem.AnswerId }, { ClassId: oldItem.ClassId }
                                    , { Comments: oldItem.Comments }, { Filled: oldItem.AnswerId > 0 });
                                if (item.AnswerId > 0) {
                                    var answer = assessment.Answers.filter(function (obj) { return obj.Id == item.AnswerId });
                                    if (answer.length > 0) {
                                        item.AnswerText = answer[0].Text;
                                        item.Score = answer[0].Score;
                                    }
                                }
                                app.viewModel.currentAssessmentItems.push(item);
                            })
                        })
                    })
                })
            }
        }
        return newData;
    };


    //更新缓存
    app.changeModeltoEntity = function (items, assessmentId, model, taStatus) {
        var newItems = JSON.parse(items);
        var targetSchoolId = app.getItem(app.values.Key.workingTarget);
        var currentSchool = app.getItem(app.values.Key.target + targetSchoolId);

        var entity = {};
        if (!assessmentId) {   //新增时
            entity = JSON.parse(JSON.stringify(currentSchool.NewAssessment));
        }
        else {    //编辑时
            entity = app.targetByAssKey[assessmentId];
        }

        if (entity) {
            //更新Items
            $.each(entity.Categories_Offline, function () {
                $.each(this, function () {
                    $.each(this, function () {
                        var classId = this.ClassId;
                        var itemId = this.ItemId;
                        var item = newItems.filter(function (obj) { return obj.ClassId == classId && obj.ItemId == itemId });
                        if (item.length > 0) {
                            this.AnswerId = item[0].AnswerId;
                            this.Comments = item[0].Comments;
                            this.CreatedOn = new Date();
                            this.UpdatedOn = new Date();
                        }
                    })
                })
            });


            $.each(entity.Classes, function () {
                var newClassId = this.Id;
                var newClass = model.classes().filter(function (obj) { return obj.id == newClassId });
                if (newClass.length > 0) {
                    this.ObservationLength = newClass[0].observationLength;
                }
                $.each(this.Categories_Offline, function () {
                    $.each(this, function () {
                        $.each(this, function () {
                            var classId = this.ClassId;
                            var itemId = this.ItemId;
                            var item = newItems.filter(function (obj) { return obj.ClassId == classId && obj.ItemId == itemId });
                            if (item.length > 0) {
                                this.AnswerId = item[0].AnswerId;
                                this.Comments = item[0].Comments;
                                this.CreatedOn = new Date();
                                this.UpdatedOn = new Date();
                            }
                        })
                    })
                })
            });

            //更新实体信息
            entity.ApproveDate = model.approveDate || app.values.minDate;
            entity.DiscussDate = model.discussDate || app.values.minDate;
            entity.VisitDate = model.visitDate || app.values.minDate;
            entity.RecertificatedBy = model.recertificatedBy || app.values.minDate;
            entity.Status = model.status;
            entity.Type = AssessmentType[model.type()];
            entity.TaStatuses = [];
            if (taStatus.length > 0) {
                var taStatuses = taStatus.split(",");
                for (var i = 0; i < taStatuses.length; i++) {
                    var ta = {};
                    ta.value = parseInt(taStatuses[i]);
                    entity.TaStatuses.push(ta);
                }
            }
            entity.TaStatus = taStatus;
            entity.IsSync = false;

            if (!assessmentId) {    //新增时
                entity.ChangeStatus = ChangeStatus.Added; //更新Assessment修改状态
                entity.guid = window.guid("_Trs_Offline_");
                entity.CreatedOn = new Date();
                entity.UpdatedOn = new Date();
                currentSchool.AssessmentList.unshift(entity);
                app.setItem(app.values.Key.target + targetSchoolId, currentSchool);
                app.targetByKey[targetSchoolId] = currentSchool;
                //将之前的workingTargetAssessment设置为新的guid
                app.setItem(app.values.Key.WorkingTargetAssessment, entity.guid);
                app.targetByAssKey[entity.guid] = entity;
            }
            else {
                if (entity.ChangeStatus != ChangeStatus.Added) {
                    entity.ChangeStatus = ChangeStatus.Edited; //更新Assessment修改状态
                }
                entity.UpdatedOn = new Date();
                //重置key 为target的值
                app.setItem(app.values.Key.target + targetSchoolId, app.targetByKey[targetSchoolId]);
            }
        }
    };

    //计算星级
    app.updateStar = function (isSave) {
        var targetSchoolId = app.getItem(app.values.Key.workingTarget);
        var currentSchool = app.targetByKey[targetSchoolId];
        var assessment = app.getItem(app.values.Key.assessment);
        var currentAssessment = app.targetByAssKey[app.getItem(app.values.Key.WorkingTargetAssessment)];
        if (currentAssessment) {
            currentAssessment.StarOfCategory = {};
            var newData = JSON.parse(JSON.stringify(currentAssessment));
            var isNotMet = false;

            categorybreak:
                for (var category in newData.Categories_Offline) {
                    var items = [];
                    var scores = 0;
                    var categoryValue = 0;
                    for (var subCategory in newData.Categories_Offline[category]) {
                        for (var item in newData.Categories_Offline[category][subCategory]) {
                            var oldItem = newData.Categories_Offline[category][subCategory][item];
                            var itemId = oldItem.ItemId;
                            var answerId = oldItem.AnswerId;
                            var newItem = assessment.Items.filter(function (obj) { return obj.ItemId == itemId })[0];
                            var answer = assessment.Answers.filter(function (obj) { return obj.Id == answerId })[0];
                            $.extend(newData.Categories_Offline[category][subCategory][item]
                                , newItem, { Id: oldItem.Id }, { ItemId: oldItem.ItemId }  //将新属性覆盖
                                   , { AnswerId: oldItem.AnswerId }, { ClassId: oldItem.ClassId }
                                   , { Comments: oldItem.Comments }, { Score: answer ? answer.Score : -1 });
                            var item_format = newData.Categories_Offline[category][subCategory][item];
                            if (item_format.Type.value == 1 && item_format.AnswerId > 0 && item_format.Score == 0) {
                                isNotMet = true;
                                break categorybreak;
                            }
                            if (item_format.Type.value == 2 && item_format.AnswerId > 0 && item_format.Score >= 0) {
                                items.push(item_format);
                                categoryValue = item_format.Category.value;
                                scores += Number(item_format.Score);
                            }
                        }
                    }
                    if (items.length > 0) {
                        var average = scores / items.length;
                        currentAssessment.StarOfCategory[categoryValue] = app.getStar(average);
                    }
                    else {
                        currentAssessment.StarOfCategory[categoryValue] = Star[1];
                    }
                }

            if (isNotMet) {
                currentAssessment.StarOfCategory[1] = Star[1];
                currentAssessment.StarOfCategory[2] = Star[1];
                currentAssessment.StarOfCategory[3] = Star[1];
                currentAssessment.StarOfCategory[4] = Star[1];
                currentAssessment.StarOfCategory[5] = Star[1];
                currentAssessment.Star = Star[1];
            }
            else {
                var classCategories = {};
                classCategories[2] = "Category2";
                classCategories[3] = "Category3";
                classCategories[4] = "Category4";

                calsscategorybreak:
                    for (var classCategory in classCategories) {  //循环classCategories
                        var scoresOfItem = {};
                        for (var classs in newData.Classes) {  //循环class
                            var classCategoryValue = classCategories[classCategory];
                            for (var subCategory in newData.Classes[classs].Categories_Offline[classCategoryValue]) {  //循环class的classCategory
                                for (var item in newData.Classes[classs].Categories_Offline[classCategoryValue][subCategory]) {
                                    var oldItem = newData.Classes[classs].Categories_Offline[classCategoryValue][subCategory][item];
                                    var itemId = oldItem.ItemId;
                                    var answerId = oldItem.AnswerId;
                                    var newItem = assessment.Items.filter(function (obj) { return obj.ItemId == itemId })[0];
                                    var answer = assessment.Answers.filter(function (obj) { return obj.Id == answerId })[0];
                                    $.extend(newData.Classes[classs].Categories_Offline[classCategoryValue][subCategory][item],
                                        newItem, { Id: oldItem.Id }, { ItemId: oldItem.ItemId }  //将新属性覆盖
                                        , { AnswerId: oldItem.AnswerId }, { ClassId: oldItem.ClassId },
                                        { Comments: oldItem.Comments }, { Score: answer ? answer.Score : -1 });
                                    var item_format = newData.Classes[classs].Categories_Offline[classCategoryValue][subCategory][item];
                                    if (item_format.Type.value == 1 && item_format.AnswerId > 0 && item_format.Score == 0) {
                                        isNotMet = true;
                                        break calsscategorybreak;
                                    }
                                    if (item_format.Type.value == 2 && item_format.AnswerId > 0 && item_format.Score >= 0) {
                                        if (!scoresOfItem[item_format.ItemId]) {
                                            scoresOfItem[item_format.ItemId] = [item_format.Score];
                                        }
                                        else {
                                            scoresOfItem[item_format.ItemId].push(item_format.Score);
                                        }
                                    }
                                }
                            }
                        }
                        var scoreOfItem = {};
                        var scores = 0;
                        var count = 0;
                        $.each(scoresOfItem, function (index, item) {
                            scoreOfItem[index] = app.median(item);
                            scores += Number(app.median(item));
                            count += 1;
                        })
                        if (count > 0) {
                            var average = scores / count;
                            currentAssessment.StarOfCategory[classCategory] = app.getStar(average);
                        }
                        else {
                            currentAssessment.StarOfCategory[classCategory] = Star[1];
                        }
                    }

                if (isNotMet) {
                    currentAssessment.StarOfCategory[1] = Star[1];
                    currentAssessment.StarOfCategory[2] = Star[1];
                    currentAssessment.StarOfCategory[3] = Star[1];
                    currentAssessment.StarOfCategory[4] = Star[1];
                    currentAssessment.StarOfCategory[5] = Star[1];
                    currentAssessment.Star = Star[1];
                }
                else {
                    var minStar = 5;
                    $.each(currentAssessment.StarOfCategory, function (index, item) {
                        if (item.value < minStar) {
                            minStar = item.value;
                        }
                    })
                    currentAssessment.Star = Star[minStar];
                }
            }
            currentAssessment.UpdatedOn = new Date();
            if (isSave) {
                //更新School的Star信息
                currentSchool.TrsTaStatus = currentAssessment.TaStatus;
                currentSchool.RecertificationBy = currentAssessment.RecertificatedBy;
                if (currentAssessment.Type && currentAssessment.Type.value != AssessmentType[6].value
                    && currentAssessment.Star && currentAssessment.Star.value > 0) {
                    currentSchool.StarStatus = currentAssessment.Star;
                    currentSchool.StarDate = currentAssessment.ApproveDate;
                }
                app.setItem(app.values.Key.target + targetSchoolId, currentSchool);
                $("#starvalue").val(currentAssessment.Star.text);
            }
            else {
                app.viewModel.currentAssessment = currentAssessment;
            }
        }
    };
    app.updateVerifiedStar = function (star) {
        var targetSchoolId = app.getItem(app.values.Key.workingTarget);
        var currentSchool = app.targetByKey[targetSchoolId];
        var currentAssessment = app.targetByAssKey[app.getItem(app.values.Key.WorkingTargetAssessment)];
        currentAssessment.VerifiedStar = Star[star];
        currentAssessment.Status = TrsAssessmentStatus[2];
        if (currentAssessment.Type && currentAssessment.Type.value != AssessmentType[6].value) {
            currentSchool.VerifiedStar = currentAssessment.VerifiedStar;
            currentSchool.TrsLastStatusChange = new Date();
        }
        app.setItem(app.values.Key.target + targetSchoolId, app.targetByKey[targetSchoolId]);
    };
    app.getStar = function (average) {
        if (average < 1.8)
            return Star[2];
        if (1.8 <= average && average < 2.4)
            return Star[3];
        if (2.4 <= average)
            return Star[4];
        return Star[2];
    };
    app.median = function (scores) {
        if (!scores)
            return 0;
        if (scores.length == 0)
            return 0;
        if (scores.length == 1)
            return scores[0];
        var scoresList = scores.sort();
        var count = scoresList.length;
        if (count % 2 == 0) {
            var index = Math.floor(count / 2) - 1;
            return (scoresList[index] + scoresList[index + 1] + 0) / 2;
        }
        else {
            return scoresList[Math.floor(count / 2)];
        }
    };
    app.preview = function () {
        //先计算星级
        app.updateStar();
        //判断app.viewModel.currentAssessment是否有值
        if (app.viewModel.currentAssessment.ApproveDate) {
            if (app.viewModel.currentAssessmentItems.length > 0) {
                var allfilled = true;
                var items = JSON.parse(JSON.stringify(app.viewModel.currentAssessmentItems));
                var structuralCategories = [];
                var processCategories = [];

                //structural result
                var structuralItems = items.filter(function (obj) {
                    return obj.Type.value == ItemType.Structural && obj.SubCategoryType.value == ItemType.Structural && obj.AnswerId > 0
                });
                var categories_structural = {};
                $.each(structuralItems, function (index, item) {
                    if (item.AnswerId <= 0) {
                        allfilled = false;
                    }
                    if (!categories_structural[item.Category.value]) {
                        categories_structural[item.Category.value] = []; //添加Category                        
                    }
                    if (categories_structural[item.Category.value].indexOf(item.SubCategoryId) < 0) { //添加Category.Subcategory
                        categories_structural[item.Category.value].push(item.SubCategoryId);
                    }
                });
                $.each(categories_structural, function (index, category) {
                    var isNotMet = false;
                    var category_index = 0;
                    $.each(category, function (index_subcategory, subcategory) {
                        var structuralSubcategory = [];
                        var items = structuralItems.filter(function (obj) {
                            return obj.AnswerId > 0 && obj.Category.value == index && obj.SubCategoryId == subcategory
                        });
                        var items_distinct = [];
                        for (var i = 0; i < items.length; i++) {
                            if (items_distinct.filter(function (obj) { return obj.ItemId == items[i].ItemId }).length == 0) {
                                items[i].ClassItems = []; //定义items的ClassItems
                                items_distinct.push(items[i]);
                                if (items[i].Score == 0) {
                                    isNotMet = true;
                                }
                                structuralSubcategory.push(items[i]);
                            }
                        }
                        if (index != Category[1].value && index != Category[5].value) {
                            $.each(items_distinct, function (index_item, item) {
                                $.each(app.viewModel.currentAssessment.Classes, function (index_class, classes) {
                                    var model = items.filter(function (obj) { return obj.ItemId == item.ItemId && obj.ClassId == classes.Id });
                                    var classmodel_add = { ClassId: classes.Id, ClassName: classes.Name };
                                    if (model.length > 0) {
                                        classmodel_add.AnswerId = model[0].AnswerId;
                                        classmodel_add.AnswerText = model[0].AnswerText;
                                    }
                                    item.ClassItems.push(classmodel_add);
                                })
                            })
                        }
                        if (structuralSubcategory.length > 0) {
                            if (category_index == 0 && !structuralCategories[structuralCategories.length]) {
                                structuralCategories.push([]);
                                structuralCategories[structuralCategories.length - 1]
                                    .push({ IsNotMet: false, Category: index });
                                category_index++;
                            }
                            structuralCategories[structuralCategories.length - 1].push(structuralSubcategory);
                        }
                    })
                    structuralCategories[structuralCategories.length - 1][0].IsNotMet = isNotMet;
                });


                //process result
                var processItems = items.filter(function (obj) {
                    return obj.Type.value == ItemType.Process && obj.SubCategoryType.value == ItemType.Process
                });
                var categories_process = {};
                $.each(processItems, function (index, item) {
                    if (item.AnswerId <= 0) {
                        allfilled = false;
                    }
                    if (!categories_process[item.Category.value]) {
                        categories_process[item.Category.value] = [];
                    }
                    if (categories_process[item.Category.value].indexOf(item.SubCategoryId) < 0) { //添加Category.Subcategory
                        categories_process[item.Category.value].push(item.SubCategoryId);
                    }
                });
                $.each(categories_process, function (index, category) {
                    var category_index = 0;
                    $.each(category, function (index_subcategory, subcategory) {
                        var processSubcategory = [];
                        var items = processItems.filter(function (obj) {
                            return obj.AnswerId > 0 && obj.Category.value == index && obj.SubCategoryId == subcategory
                        });
                        var items_distinct = [];
                        for (var i = 0; i < items.length; i++) {
                            if (items_distinct.filter(function (obj) { return obj.ItemId == items[i].ItemId }).length == 0) {
                                items[i].ClassItems = []; //定义items的ClassItems
                                items_distinct.push(items[i]);
                                processSubcategory.push(items[i]);
                            }
                        }
                        if (index != Category[1].value && index != Category[5].value) {
                            $.each(items_distinct, function (index_item, item) {
                                $.each(app.viewModel.currentAssessment.Classes, function (index_class, classes) {
                                    var model = items.filter(function (obj) { return obj.ItemId == item.ItemId && obj.ClassId == classes.Id });
                                    var classmodel_add = { ClassId: classes.Id, ClassName: classes.Name };
                                    if (model.length > 0) {
                                        classmodel_add.Score = model[0].Score;
                                    }
                                    else {
                                        classmodel_add.Score = -1;
                                    }
                                    item.ClassItems.push(classmodel_add);
                                })
                            })
                        }
                        if (processSubcategory.length > 0) {
                            if (category_index == 0 && !processCategories[processCategories.length]) {
                                processCategories.push([]);
                                processCategories[processCategories.length - 1]
                                    .push({ Category: index, Star: app.viewModel.currentAssessment.StarOfCategory[index].text });
                                category_index++;
                            }
                            processCategories[processCategories.length - 1].push(processSubcategory);
                        }
                    })
                });
                if (!allfilled) {
                    app.viewModel.currentAssessment.Star = null;
                }
                app.viewModel.currentAssessment.StructuralCategories = structuralCategories;
                app.viewModel.currentAssessment.ProcessCategories = processCategories;
            }
        }
    };
    app.goOnline = function () {
        if (this.viewModel.showSyncAll && this.viewModel.showSyncAll()) {
            window.showMessage("warning", "need_Sync_Before_goOnline");
        } else {
            location.href = this.onlineUrl;
        }
    };
    return app;
}

