var Trs_Status = {
    Loaded: 10,
    Saving: 20,
    Priviewing: 25,
    Finalizing: 30,
    Submitted: 40
}
var Trs_SyncAnswer = {
    /// <summary>
    /// 无
    /// </summary>
    None: 1,

    /// <summary>
    /// 同一学校的相同Playground的Classroom需要共享答案, 没有Playground的Class不需要显示
    /// </summary>
    SamePlayground: 10,

    /// <summary>
    /// 同一学校所有的Classroom需要共享答案
    /// </summary>
    BetweenClass: 20
}

var unsaveTimeOut;

function TrsAssessmentModel(defaultValues, isOffline) {
    this.isOffline = isOffline;
    var that = this;
    this.id = window.isNull("Id", defaultValues, 0);
    this.school = new TrsSchoolModel(defaultValues["School"]);
    this.star = window.isNull("Star", defaultValues, { value: 0 });
    this.status = window.isNull("Status", defaultValues, { value: 1 });

    var taStatuses = window.isNull("TaStatuses", defaultValues, []);
    this.taStatuses = [];
    for (var i = 0; i < taStatuses.length; i++) {
        this.taStatuses.push(taStatuses[i].value);
    }
    this.categoryByKey = window.isNull("Category", defaultValues, {});
    this.subCategoryByKey = window.isNull("SubCategory", defaultValues, {});
    this.categories = ko.observableArray([]);
    if (isOffline) {    //Offline时，获取Categories_Offline
        for (var category in defaultValues.Categories_Offline) {
            var cate = new TrsCategoryModel(category, defaultValues.Categories_Offline[category], this);

            this.categories.push(cate);
        }
    }
    else {
        for (var category in defaultValues.Categories) {
            var cate = new TrsCategoryModel(category, defaultValues.Categories[category], this);

            this.categories.push(cate);
        }
    }

    this.classes = ko.observableArray([]);

    for (var j = 0; j < defaultValues.Classes.length; j++) {
        this.classes.push(new TrsClassModel(defaultValues.Classes[j], this));
    }

    this.submitStatus = ko.observable(Trs_Status.Loaded);
    this.isSubmitting = ko.computed(function () {
        return this.submitStatus() == Trs_Status.Saving || this.submitStatus() == Trs_Status.Finalizing;
    }, that);
    this.isFinalizing = ko.computed(function () {
        return this.submitStatus() == Trs_Status.Finalizing;
    }, that);
    this.type = ko.observable(window.isNull("Type", defaultValues, { value: 0 }).value);

    this.allRequired = function () {
        return true;
    };
    this.allFilled = ko.computed(function () {
        var result = true;

        for (var j = 0; j < that.categories().length; j++) {
            if (!that.categories()[j].allFilled()) {
                result = false;
                break;
            }
        }
        for (var k = 0; k < that.classes().length; k++) {
            if (!that.classes()[k].allFilled()) {
                result = false;
                break;
            }
        }
        return result;
    }, this);
    this.getStatus = function () {
        var categories = this.categories();
        var filled = 0;
        var total = 0;
        var itemsFilled = [];
        for (var k = 0; k < categories.length; k++) {
            category = categories[k];
            for (var n = 0; n < category.subs().length; n++) {
                sub = category.subs()[n];
                total += sub.items().length;
                for (var l = 0; l < sub.items().length; l++) {
                    var item = sub.items()[l];
                    if (!item.filled) {
                    } else {
                        filled++;
                        itemsFilled.push({
                            ItemId: item.itemId,
                            ClassId: 0, Score: 0,
                            AnswerId: item.answer.newAnswer(),
                            Comments: item.comments || "",
                            AgeGroup: item.ageGroup,
                            GroupSize: item.groupSize,
                            CaregiversNo: item.caregiversNo
                        });
                    }
                }
            }

        }
        for (var m = 0; m < this.classes().length; m++) {
            var class1 = this.classes()[m];
            categories = class1.categories();
            for (k = 0; k < categories.length; k++) {
                category = categories[k];
                for (n = 0; n < category.subs().length; n++) {
                    sub = category.subs()[n];
                    total += sub.items().length;
                    for (l = 0; l < sub.items().length; l++) {
                        item = sub.items()[l];
                        if (!item.filled) {
                        } else {
                            filled++;
                            itemsFilled.push({
                                ItemId: item.itemId,
                                ClassId: class1.id,
                                Score: 0,
                                AnswerId: item.answer.newAnswer(),
                                Comments: item.comments || "",
                                AgeGroup: item.ageGroup,
                                GroupSize: item.groupSize,
                                CaregiversNo: item.caregiversNo
                            });
                        }
                    }
                }
            }
        }
        return {
            filled: filled,
            total: total,
            itemsFilled: itemsFilled
        };
    }

    this.getObservations = function () {
        var classObservations = [];
        for (var m = 0; m < this.classes().length; m++) {
            var class1 = this.classes()[m];
            classObservations.push({ AssessmentId: 0, ClassId: class1.id, ObservationLength: class1.observationLength || 0 });
        }
        return JSON.stringify(classObservations);
    }

    this.prepare = function () {
        var status = this.getStatus();
        if (this.allRequired() && this.isFinalizing()) {
            if (this.allFilled()) {
                return JSON.stringify(status.itemsFilled);
            } else {
                return false;
            }
        } else {
            return JSON.stringify(status.itemsFilled);
        }
        return false;
    }

    this.choosed = function (host, item, event) {
        if (host.constructor == TrsClassModel) {// class item clicked, not school
            if (item.syncAnswer !== Trs_SyncAnswer.None) {// Need sync answer 
                for (j = 0; j < that.classes().length; j++) {
                    var class1 = that.classes()[j];
                    if (item.syncAnswer == Trs_SyncAnswer.SamePlayground) {//Sync answer of item under same Playground
                        if (class1.id === host.id || class1.playgroundId == 0 || class1.playgroundId != host.playgroundId) {
                            // sender class does not necessary to set choosed value again
                            // no playgroundId, no share same score
                            // playgroundId not equal, do not share same score
                            continue;
                        }
                    }
                    if (item.syncAnswer === Trs_SyncAnswer.BetweenClass) { //Sync answer of item between all classroom
                        if (class1.id == host.id) {
                            // sender class does not necessary to set choosed value again
                            continue;
                        }
                    }
                    for (var k = 0; k < class1.categories().length; k++) {
                        var category = class1.categories()[k];
                        for (var l = 0; l < category.subs().length; l++) {
                            var sub = category.subs()[l];
                            for (var m = 0; m < sub.items().length; m++) {
                                var tItem = sub.items()[m];
                                if (tItem.itemId == item.itemId) {
                                    tItem.answer.newAnswer(item.answer.newAnswer());
                                }
                            }
                        }
                    }
                }
            }
        }
        // if all items of the category is filled, collapse
        var categories;
        var categoryPrefix = "";
        var checkboxPrefix = "";
        if (host.constructor === TrsSchoolModel) {
            categories = that.categories();
            categoryPrefix = "schoolCategory";
        } else if (host.constructor === TrsClassModel) {
            categories = host.categories();
            categoryPrefix = "class" + host.id + "Category";
        }
        if (categories) {
            for (i = 0; i < categories.length; i++) {
                category = categories[i];
                if (category.allFilled() === true) {
                    var categoryId = categoryPrefix + i;
                    var $category = $("#" + categoryId);
                    if ($category.hasClass("in")) {
                        $category.collapse("hide");
                    }
                }
            }
        }
        return true;
    };

    this.activeClassId = ko.observable(this.classes()[0].id);
    this.switchClass = function (class1, event) {
        that.activeClassId(class1.id);
        var $target = $(event.target).closest("a");
        $target.tab("show");
    }

    this.onItemCommentBlur = function (item, event) {
        setTimeout(function () {
            if ((!item.comments) || (!item.comments.replace(/[ ]/g, ""))) {
                item.isView(true);
            }
        }, 200);
    }


    //TRS Offline Module
    this.visitDate = window.isNull("VisitDate", defaultValues, "");
    this.discussDate = window.isNull("DiscussDate", defaultValues, "");
    this.approveDate = window.isNull("ApproveDate", defaultValues, "");
    this.recertificatedBy = window.isNull("RecertificatedBy", defaultValues, "");
    this.action = window.isNull("Action", defaultValues, "");
}

function TrsSchoolModel(defaultValues) {
    this.assessor = {
        id: window.isNull("AssessorId", defaultValues, 0),
        firstname: window.isNull("AssesssorFirstName", defaultValues, ""),
        lastname: window.isNull("AssesssorLastName", defaultValues, "")
    }
    this.communityId = window.isNull("CommunityId", defaultValues, 0);
    this.communityName = window.isNull("CommunityName", defaultValues, "");
    this.facilityType = window.isNull("FacilityType", defaultValues, {});
    this.id = window.isNull("ID", defaultValues, 0);
    this.name = window.isNull("Name", defaultValues, "");
}

function TrsCategoryModel(category, subCategories, assessmentModel) {
    this.category = category,
    this.text = assessmentModel.categoryByKey[category],
    this.subs = ko.observableArray([]);
    this.allFilled = ko.computed(function () {
        var result = true;
        for (var j = 0; j < this.subs().length; j++) {
            if (!this.subs()[j].allFilled()) {
                result = false;
                break;
            }
        }
        return result;
    }, this);
    this.allFilledAndNA = ko.computed(function () {
        var result = this.allFilled();
        if (result) {
            for (var j = 0; j < this.subs().length; j++) {
                if (!this.subs()[j].allFilledAndNA()) {
                    result = false;
                    break;
                }
            }
        }
        return result;
    }, this);
    for (var sub in subCategories) {
        var subCategory = new TrsSubCategoryModel(sub, subCategories[sub], assessmentModel);
        this.subs.push(subCategory);
    }
}

function TrsSubCategoryModel(subCategory, items, assessmentModel) {
    this.category = subCategory;
    this.text = assessmentModel.subCategoryByKey[subCategory].Name;
    this.type = assessmentModel.subCategoryByKey[subCategory].Type.text;
    this.note = this.type + " Requirements";
    this.items = ko.observableArray([]);
    this.allFilled = ko.computed(function () {
        var result = true;
        for (var j = 0; j < this.items().length; j++) {
            if (!this.items()[j].filled()) {
                result = false;
                break;
            }
        }
        return result;
    }, this);
    this.allFilledAndNA = ko.computed(function () {
        var result = this.allFilled();
        if (result) {
            for (var j = 0; j < this.items().length; j++) {
                if (this.items()[j].answer.newAnswer() != 77) {
                    result = false;
                    break;
                }
            }
        }
        return result;
    }, this);
    for (var i = 0; i < items.length; i++) {
        this.items.push(new TrsItemModel(items[i], assessmentModel));
    }
}

function TrsClassModel(defaultValues, assessmentModel) {
    this.id = window.isNull("Id", defaultValues, 0);
    this.name = window.isNull("Name", defaultValues, "");
    this.star = window.isNull("Star", defaultValues, { value: 0 });
    this.status = window.isNull("Status", defaultValues, { value: 1 });
    this.playgroundId = window.isNull("PlaygroundId", defaultValues, 0);
    this.observationLength = window.isNull("ObservationLength", defaultValues, '') || '';
    this.ifCanAccessObservation = window.isNull("IfCanAccessObservation", defaultValues, false);
    this.categories = ko.observableArray([]);

    this.allFilled = ko.computed(function () {
        var result = true;
        for (var j = 0; j < this.categories().length; j++) {
            if (!this.categories()[j].allFilled()) {
                result = false;
                break;
            }
        }
        return result;
    }, this);

    if (assessmentModel.isOffline) {
        for (var category in defaultValues.Categories_Offline) {
            var cate = new TrsCategoryModel(category, defaultValues.Categories_Offline[category], assessmentModel);
            this.categories.push(cate);
        }
    }
    else {
        for (var category in defaultValues.Categories) {
            var cate = new TrsCategoryModel(category, defaultValues.Categories[category], assessmentModel);
            this.categories.push(cate);
        }
    }

}

function TrsItemModel(defaultValues, assessmentModel) {
    var that = this;
    this.answer = {
        id: window.isNull("AnswerId", defaultValues, 0),
        text: window.isNull("AnswerText", defaultValues, 0),
        newAnswer: ko.observable(window.isNull("AnswerId", defaultValues, 0))
    };

    this.id = window.isNull("AssessmentItemId", defaultValues, 0);
    this.description = window.isNull("Description", defaultValues, "");
    this.keyBehavior = window.isNull("KeyBehavior", defaultValues, "");
    if (this.keyBehavior) {
        this.keyBehavior = "<div style='text-align:left;'>" + this.keyBehavior + '</div>';
    }
    this.filled = ko.observable(window.isNull("Filled", defaultValues, false));
    this.required = window.isNull("IsRequired", defaultValues, false);
    this.itemNo = window.isNull("Item", defaultValues, "");
    this.itemId = window.isNull("ItemId", defaultValues, 0);
    this.linkingDocument = window.isNull("LinkingDocument", defaultValues, "");
    this.score = window.isNull("Score", defaultValues, 0);
    this.subCategory = window.isNull("SubCategoryId", defaultValues, 0);
    this.TAItemInstructions = window.isNull("TAItemInstructions", defaultValues, "");
    this.TAPlanItem = window.isNull("TAPlanItem", defaultValues, "");
    this.TAPlanItemType = window.isNull("TAPlanItemType", defaultValues, "");
    this.text = window.isNull("Text", defaultValues, "");
    this.type = window.isNull("Type", defaultValues, {}).text;
    this.syncAnswer = window.isNull("SyncAnswer", defaultValues, {}).value;
    this.answer.newAnswer.subscribe(function (value) {
        that.filled(true);
    });
    this.comments = window.isNull("Comments", defaultValues, "");
    this.ifCanAccess = window.isNull("IfCanAccess", defaultValues, false);
    this.isView = ko.observable(this.comments == null ? true : ((!this.comments) && (!this.comments.replace(/[ ]/g, ""))));
    this.answers = { Score0: [], Score1: [], Score2: [], Score3: [], Score_1: [] };
    for (var i = 0; i < defaultValues.Answers.length; i++) {
        var answer = {
            id: defaultValues.Answers[i].Id,
            score: defaultValues.Answers[i].Score,
            text: defaultValues.Answers[i].Text
        }
        this.answers["Score" + answer.score.toString().replace("-", "_")].push(answer);
    }
    this.ageGroup = window.isNull("AgeGroup", defaultValues, 0);
    this.groupSize = window.isNull("GroupSize", defaultValues, 0);
    this.caregiversNo = window.isNull("CaregiversNo", defaultValues, 0);
}

function Trs_Select(value) {
    var categories = model.categories();
    for (var k = 0; k < categories.length; k++) {
        var category = categories[k];
        for (var n = 0; n < category.subs().length; n++) {
            sub = category.subs()[n];
            for (var l = 0; l < sub.items().length; l++) {
                var item = sub.items()[l];

                if (item.type == "Structural") {
                    item.answer.newAnswer(75);
                    item.filled(true);
                } else {
                    var i;
                    var maxScoreAnswer;
                    for (i = 0; i < 4; i++) {
                        if (item.answers["Score" + i].length) {
                            maxScoreAnswer = i;
                            if (i == value) {
                                item.answer.newAnswer(item.answers["Score" + i][0].id);
                                item.filled(true);
                            }
                        }
                    }
                    if (!item.filled()) {
                        item.answer.newAnswer(item.answers["Score" + maxScoreAnswer][0].id);
                        item.filled(true);
                    }
                }
            }
        }

    }
    var classes = model.classes();
    for (var m = 0; m < classes.length; m++) {
        var class1 = classes[m];
        categories = class1.categories();
        for (k = 0; k < categories.length; k++) {
            category = categories[k];
            for (n = 0; n < category.subs().length; n++) {
                sub = category.subs()[n];
                for (l = 0; l < sub.items().length; l++) {
                    item = sub.items()[l];
                    if (item.type == "Structural") {
                        item.answer.newAnswer(75);
                        item.filled(true);
                    } else {
                        var i;
                        var maxScoreAnswer;
                        for (i = 0; i < 4; i++) {
                            if (item.answers["Score" + i].length) {
                                maxScoreAnswer = i;
                                if (i == value) {
                                    item.answer.newAnswer(item.answers["Score" + i][0].id);
                                    item.filled(true);
                                }
                            }
                        }
                        if (!item.filled) {
                            item.answer.newAnswer(item.answers["Score" + maxScoreAnswer][0].id);
                            item.filled(true);
                        }
                    }
                }
            }
        }
    }
}
