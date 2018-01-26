
function AssignmentType(datas) {
    var that = this;
    this.data = [];
    for (var i = 0; i < datas.length; i++) {
        this.data.push({
            text: datas[i].text,
            value: datas[i].value,
            selected: ko.observable(false)
        });
    }
    this.selected = ko.computed(function () {
        var allSelected = true;
        for (var i = 0; i < that.data.length; i++) {
            if (!that.data[i].selected()) {
                allSelected = false;
                break;
            }
        }
        return allSelected;
    }, this);
    this.switchSelectAll = function () {
        var target = !that.selected();
        for (var i = 0; i < that.data.length; i++) {
            that.data[i].selected(target);
        }
    };
}

function filterModel(years,coachingModels,eCircles) {
    var that = this;

    this.yearsInProject = {
        data: []
    };
    for (var i = 0; i < years.length; i++) {
        var item = {
            text: years[i].Text,
            value: years[i].Value,
            selected: ko.observable(false)
        };
        this.yearsInProject.data.push(item);
    }
    this.yearsInProject.selected = ko.computed(function () {
        var allSelected = true;
        for (var i = 0; i < that.yearsInProject.data.length; i++) {
            if (!that.yearsInProject.data[i].selected()) {
                allSelected = false;
                break;
            }
        }
        return allSelected;
    }, this);
    this.yearsInProject.switchSelectAll = function () {
        var target = !that.yearsInProject.selected();
        for (var i = 0; i < that.yearsInProject.data.length; i++) {
            that.yearsInProject.data[i].selected(target);
        }
    };

    this.coachingModels = new AssignmentType(coachingModels);
    this.eCircles = new AssignmentType(eCircles);
    
    this.switchItem = function(clickedItem) {
        clickedItem.selected(!clickedItem.selected());
    }
}