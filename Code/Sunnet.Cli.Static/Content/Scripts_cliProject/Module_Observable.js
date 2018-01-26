function getDateString(strDate, format)
{
    if (!format)
    {
        format = "MM/dd/yyyy";
    }
    if ("01/01/0001" == strDate)
        return "";
    var date = parseDate(strDate, format);
    var minDate = parseDate("01/01/2014");
    if (date > minDate)
    {
        return date.Format(format);
    }
    return "";
}

function ObservableAssessment(defaultValues)
{
    var that = this;
    this.id = window.isNull("ID", defaultValues, 0);
    this.assessmentId = window.isNull("AssessmentId", defaultValues, 0);
    this.measures = [];
    var measures = window.isNull("Measures", defaultValues, []);
    for (var i = 0; i < measures.length; i++)
    {
        this.measures.push(new ObservableMeasure(measures[i]));
    }

    this.showFullTargetText = ko.observable(false);
    this.toggleFullTargetText = function ()
    {
        that.showFullTargetText(!that.showFullTargetText());
    }

    this.showActiveGoalsOnly = ko.observable(false);
    this.toggleActiveGoals = function ()
    {
        that.showActiveGoalsOnly(!that.showActiveGoalsOnly());
    };

    this.prapare = function (isCreateStgReport)
    {
        var items = [];
        var savedItems = [];
        var i, j, k, item, itemForUpdate;
        for (i = 0; i < this.measures.length; i++)
        {
            var measure = this.measures[i];
            if (measure.items && measure.items.length)
            {
                for (j = 0; j < measure.items.length; j++)
                {
                    item = measure.items[j];
                    item.saved(false);
                    itemForUpdate = item.itemForUpdate(isCreateStgReport);
                    if (itemForUpdate)
                    {
                        items.push(itemForUpdate);
                        savedItems.push(item);
                    }
                }
            }
            if (measure.children && measure.children.length)
            {
                for (k = 0; k < measure.children.length; k++)
                {
                    var child = measure.children[k];
                    if (child.items && child.items.length)
                    {
                        for (j = 0; j < child.items.length; j++)
                        {
                            item = child.items[j];
                            item.saved(false);
                            itemForUpdate = item.itemForUpdate(isCreateStgReport);
                            if (itemForUpdate)
                            {
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
    this.m_resourceClick = function () {
        window.open(this.url);
    };
};

function ObservableMeasure(defaultValues)
{
    this.id = window.isNull("ID", defaultValues, 0);
    this.visible = window.isNull("Visible", defaultValues, false);
    this.measureId = window.isNull("MeasureId", defaultValues, 0);
    this.name = window.isNull("Name", defaultValues, "");
    this.children = false;
    this.parent = false;
    this.items = false;

    var items = window.isNull("Items", defaultValues, false);
    if (items && items.length)
    {
        this.items = [];
        for (var i = 0; i < items.length; i++)
        {
            this.items.push(new ObservableItem(items[i]));
        }
    }

    var children = window.isNull("Children", defaultValues, false);
    if (children && children.length)
    {
        this.children = [];
        for (var i = 0; i < children.length; i++)
        {
            var child = new ObservableMeasure(children[i]);
            child.parent = this;
            this.children.push(child);
        }
    }
    this.links = [];
    if (defaultValues.Links && defaultValues.Links.length)
    {
        for (var i = 0; i < defaultValues.Links.length; i++)
        {
            this.links.push({
                text: defaultValues.Links[i].DisplayText,
                url: defaultValues.Links[i].Link
            });
        }
    };

    this.isActive = ko.computed(function ()
    {
        if (this.visible)
        {
            var isShow = false;
            if (this.children)
            {
                $.each(this.children, function (index, mea)
                {
                    if (mea.isActive())
                    {
                        isShow = true;
                        return true;
                    }
                });

                if (isShow == false)
                {
                    if (this.items)
                    {
                        $.each(this.items, function (index, opinion)
                        {
                            if (opinion.isActive())
                            {
                                isShow = true;
                                return true;
                            }
                        });
                    }
                }
                return isShow;
            } else
            {
                $.each(this.items, function (index, opinion)
                {
                    if (opinion.isActive())
                    {
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

function ObservableAnswer(defaultValues)
{
    this.id = window.isNull("ID", defaultValues, 0);
    this.itemId = window.isNull("ItemId", defaultValues, 0);
    this.text = window.isNull("Text", defaultValues, 0);
}

function ObservableItem(defaultValues)
{
    var that = this;
    this.id = window.isNull("ID", defaultValues, 0);
    this.observableAssessmentId = window.isNull("ObservableAssessmentId", defaultValues, 0);
    this.itemId = window.isNull("ItemId", defaultValues, 0);
    this.observableItem = "<span class='note'>&nbsp;" + window.isNull("ObservableItemId", defaultValues, "Not set") + "</span>";
    this.level = window.isNull("Level", defaultValues, {});
    this.shortTargetText = window.isNull("ShortTargetText", defaultValues, "");
    this.fullTargetText = window.isNull("FullTargetText", defaultValues, "");
    this.goalSetDate = getDateString(window.isNull("GoalSetDate", defaultValues, ""));
    this.goalMetDate = ko.observable(getDateString(window.isNull("GoalMetDate", defaultValues, "")));
    this.Name = window.isNull("Name", defaultValues, 0);

    this.cotUpdatedOn = ko.observable(getDateString(window.isNull("CotUpdatedOn", defaultValues, "")));
    this.goalMetDone = window.isNull("GoalMetDone", defaultValues, false);


    this.circleManual = window.isNull("CircleManual", defaultValues, "");
    this.mentoringGuide = window.isNull("MentoringGuide", defaultValues, "");
    this.prekindergartenGuidelines = window.isNull("PrekindergartenGuidelines", defaultValues, "");

    this.isshown = window.isNull("IsShown", defaultValues, "");
    this.res = window.isNull("Res", defaultValues, "");
    this.updated = window.isNull("Date", defaultValues, "");
    this.highlight = ko.computed(function ()
    {
        var threeDate = (this.boyObsDate + this.moyObsDate + this.cotUpdatedOn()).length > 0;
        return threeDate || this.goalMetDone;
    }, that);
    this.answertype = window.isNull("AnswerType", defaultValues, 0);
    this.type = window.isNull("Type", defaultValues, 0);
    this.isRequired = window.isNull("IsRequired", defaultValues, false);
    this.ismultichoice = window.isNull("IsMultiChoice", defaultValues, 0);

    var answers = window.isNull("Answers", defaultValues, false);
    this.answers = [];
    if (answers && answers.length)
    {

        for (var i = 0; i < answers.length; i++)
        {
            this.answers.push(new ObservableAnswer(answers[i]));
        }
    }



    // Observable assessment
    this.observed = ko.observable(false);
    this.observed.subscribe(function (observed)
    {
        if (!observed)
            that.needSupport(false);
    });
    if (this.boyObsDate || this.moyObsDate)
    {
        this.observed(true);
    }

    // Observable report
    this.changed = ko.observable(false);
    this.saved = ko.observable(false);
    this.highlightAfterSave = ko.computed(function ()
    {
        return this.saved();
    }, this);
    this.waitingGoalMet = ko.observable(window.isNull("WaitingGoalMet", defaultValues, false));
    this.needSupport = ko.observable(window.isNull("NeedSupport", defaultValues, false));
    this.needSupportEnabled = ko.computed(function ()
    {
        return (this.boyObsDate + this.moyObsDate + this.cotUpdatedOn()).length > 0;
    }, that);

    this.cotUpdatedOn.subscribe(function (newDate)
    {
        if (newDate)
        {
            that.needSupport(false);
        } else
        {
            that.needSupport(true);
        }
        that.changed(true);
    });

    this.needSupport.subscribe(function ()
    {
        that.changed(true);
    });
    this.goalMetDate.subscribe(function (newDate)
    {
        if (newDate)
        {
            that.waitingGoalMet(false);
        } else
        {
            that.waitingGoalMet(true);
        }
        that.changed(true);
    });
    this.showMetDateBox = ko.computed(function ()
    {
        //var onlyHaveSetDate = this.goalSetDate.length && !this.goalMetDate().length;
        var onlyHaveSetDate = this.goalSetDate.length && (!this.goalMetDate().length || (this.goalMetDate().length && that.changed()));
        if (onlyHaveSetDate)
        {
            return true;
        }
        return false;
    }, this);
    this.isActive = ko.computed(function ()
    {
        var onlyHaveSetDate = this.goalSetDate.length && !this.goalMetDate().length;
        var needSupport = this.needSupport();
        return onlyHaveSetDate || needSupport || that.changed();
    }, this);

    this.itemForUpdate = function (isCreateStgReport)
    {
        var newItem;
        if (that.changed() || (isCreateStgReport === true && that.waitingGoalMet()))
        {
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
    if (this.goalMetDate)
    {
        {
            this.goalMetAbled = true;
            this.goalMetAble = false;
        }
    }

    this.visible = true;
    this.links = [];
    if (defaultValues.Links && defaultValues.Links.length)
    {
        for (var i = 0; i < defaultValues.Links.length; i++)
        {
            this.links.push({
                text: defaultValues.Links[i].DisplayText,
                url: defaultValues.Links[i].Link
            });
        }
    }
}

function ContainsItem(sourceItems, selectedItemIds)
{
    var contain = false;
    if (sourceItems && sourceItems.length)
    {
        for (var i = 0; i < sourceItems.length; i++)
        {
            if (selectedItemIds.indexOf(sourceItems[i].ItemId) >= 0 || selectedItemIds.indexOf(sourceItems[i].itemId) >= 0)
            {
                contain = true;
                break;
            }
        }
    }
    return contain;
}

function GetItemByItemId(sourceItems, itemId)
{
    var item = null;
    if (sourceItems && sourceItems.length)
    {
        for (var i = 0; i < sourceItems.length; i++)
        {
            if (itemId == sourceItems[i].ItemId || itemId == sourceItems[i].itemId)
            {
                item = sourceItems[i];
                break;
            }
        }
    }
    return item;
}

function ItemAnswer(itemId, res)
{
    this.ItemId = itemId;
    this.Res = res;
}

function GetItemAnswers(assessmentModel)
{
    var listAnswers = [];
    for (var a = 0; a < assessmentModel.measures.length; a++)
    {
        var measure = assessmentModel.measures[a];
        for (var m = 0; m < measure.items.length; m++)
        {
            var item = measure.items[m];
            var newAnswer;
            if (item.ismultichoice) // "Multiple Choice"
            {
                var chkValue = "";
                $('input[name="' + item.itemId + '"]:checked').each(function ()
                {
                    chkValue += "," + $(this).val();
                });
                newAnswer = new ItemAnswer(item.itemId, chkValue);
                listAnswers.push(newAnswer);
            }
            else if (item.type.value == 14)// "Single Choice"
            {
                newAnswer = new ItemAnswer(item.itemId, $("[name='" + item.itemId + "']:checked").val());
                listAnswers.push(newAnswer);
            }
            else
            {
                newAnswer = new ItemAnswer(item.itemId, $("[name='" + item.itemId + "']").val());
                listAnswers.push(newAnswer);
            }
        }
        if (measure.children.length >0)
        {
            for (var aa = 0; aa< measure.children.length; aa++) {
                var childMeasure = measure.children[aa];
              
                for (var mm = 0; mm < childMeasure.items.length; mm++)
                {
                    var childitem = childMeasure.items[mm];
                    var newChildAnswer;
                    if (childitem.ismultichoice) // "Multiple Choice"
                    {
                        var chkChildValue = "";
                        $('input[name="' + childitem.itemId + '"]:checked').each(function ()
                        {
                            chkChildValue += "," + $(this).val();
                        });
                        newChildAnswer = new ItemAnswer(childitem.itemId, chkValue);
                        listAnswers.push(newChildAnswer);
                    }
                    else if (childitem.type.value == 14)// "Single Choice"
                    {
                        newChildAnswer = new ItemAnswer(childitem.itemId, $("[name='" + childitem.itemId + "']:checked").val());
                        listAnswers.push(newChildAnswer);
                    }
                    else
                    {
                        newChildAnswer = new ItemAnswer(childitem.itemId, $("[name='" + childitem.itemId + "']").val());
                        listAnswers.push(newChildAnswer);
                    }
                }
            }
        }
    }
    return listAnswers;
}

