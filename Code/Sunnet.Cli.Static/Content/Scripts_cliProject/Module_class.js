


function addMinMaxValidate(id) {
    $(id).rules("add", {
        min: 0,
        required: true,
        max: 100
    });
}

function removeMinManValidate(id) {
    $(id).rules("remove", "min max required");
}

function addRequiredValidate(id) {
    $(id).rules("add", {
        required: true
    });
}
function removeRequiredValidate(id) {
    $(id).rules("remove", "required");
}


function addValidateById(id) {
    $(id).rules("add", {
        number: true,
        required: true
    });
}

function removeValidateById(id) {
    $(id).rules("remove", "min required");
}

function fnLimitNumberValidte(id, number) {
    if ($.isNumeric(number)) {
        $(id).rules("add", {
            maxlength: number
        });
    }
}


function InitControlsByRole() {
    $("input.form-control").not("#txtCommunity").not("#txtSchool").not("#txtClassroom").not("#CurriculumOther")
        .not("#SupplementalCurriculumOther").not("#MonitoringToolOther").not("#EquipmentNumber").not("#txtAgeGroup1").not("#txtSchoolType")
        .not("#txtAgeGroup2").not("#txtAgeGroup3").not("#txtAgeGroup4").not("#LCchildrenNumber").not("#TypeOfClassroom").not("#LCchildrenTypeId")
        .add("select.form-control").not("#UsedEquipment").not("#languageSelectList").not("#TrsAssessorId").not("#TrsMentorId")
        .add("textarea.form-control")
        .each(function (i, obj) {
            var id = obj.id;
            if (id != "") {
                var noAuthorityLimits = ["TypeOfClass", "LCchildrenNumber", "LCchildrenTypeId", "LeadTeacherId"];
                if (noAuthorityLimits.indexOf(id) < 0) {
                    $(obj).parent().parent().attr("data-bind", "visible: " + id + " != 'X'");
                    $(obj).attr("data-bind", "visible: " + id + " != 'X'");
                    $(obj).attr("data-bind", "disable: " + id + " == 'R'");
                }
            }
        });
}


function GetSchoolType() {
    var schoolId = $("#SchoolId").val();
    if (schoolId != null && schoolId != "") {
        $.post("/School/School/GetSchoolType", { schoolId: schoolId }, function (result) {
            $("#txtSchoolType").html(result);
        });
    }
}

function showOther() {
    if ($("#CurriculumId :selected").text() == "Other") {
        $("#CurriculumOther").show();
    } else {
        $("#CurriculumOther").val("").hide();
    }
    if ($("#SupplementalCurriculumId :selected").text() == "Other") {
        $("#SupplementalCurriculumOther").show();
    } else {
        $("#SupplementalCurriculumOther").val("").hide();
    }

    $("#CurriculumId").change(function () {
        if ($("#CurriculumId :selected").text() == "Other") {
            $("#CurriculumOther").show();
        } else {
            $("#CurriculumOther").val("").hide();
        }
    });
    $("#SupplementalCurriculumId").change(function () {
        if ($("#SupplementalCurriculumId :selected").text() == "Other") {
            $("#SupplementalCurriculumOther").show();
        } else {
            $("#SupplementalCurriculumOther").val("").hide();
        }
    });
}

function changeStatusByMonitoringTool() {

    var txt = $("#MonitoringToolId option:selected").text().trim().toUpperCase();
    if (txt == undefined || txt == "") {
        return false;
    } else {
        switch (txt) {
            case "CPALLS+":
                $("#divEquipment").show();
                $("#MonitoringToolOther").val("");
                $("#MonitoringToolOther").hide();
                $("#UsedEquipment option:first").val(null);
                break;
            case "OTHER":
                clearUsedEquipment();
                $("#divEquipment").hide();
                $("#MonitoringToolOther").show();
                $("#UsedEquipment option:first").val("0");
                break;
            case "PLEASE SELECT...":
            case "PLEASE SELECT…":
            default:
                clearUsedEquipment();
                $("#divEquipment").hide();
                $("#MonitoringToolOther").val("");
                $("#MonitoringToolOther").hide();
                $("#UsedEquipment option:first").val("0");
                break;
        }
        return true;
    }
}

function fnUsedEquipment() {
    if ($("#UsedEquipment").val() != undefined || $("#UsedEquipment").val() != "") {
        $("#EquipmentNumber").removeAttr("readonly");
    }

    $("#UsedEquipment").change(function () {
        var valTmp = $(this).val().trim();
        if (valTmp == undefined || valTmp == "") {
            $("#EquipmentNumber").prop("readonly", "true");
            removeRequiredValidate("#EquipmentNumber");
        } else {
            $("#EquipmentNumber").removeAttr("readonly");
            addRequiredValidate("#EquipmentNumber");
        }
    });
}



function clearUsedEquipment() {
    $("#UsedEquipment").get(0).selectedIndex = 0;
    $("#EquipmentNumber").val("0").prop("readonly", "true");
}


function GenerateNumber(obj) {
    var i = 0;
    for (i = 0; i <= 31; i++) {
        $(obj).append("<option value='" + i + "'>" + i + "</option>");
    }
}

function classTrsObject(trsObject) {
    $.each(trsObject.childrenTypes, function (index, type) {
        type.checked = ko.observable(false);
    });
    trsObject.visible = ko.observable(trsObject.visible);
    trsObject.multiple = ko.observable(trsObject.multiple);
    trsObject.typeOfClass = ko.observable(trsObject.typeOfClass);
    trsObject.show = ko.observable(trsObject.show);
    return trsObject;
}
function classTrsObjectEdit(trsObject) {
    var object = classTrsObject(trsObject);
    $.each(object.childrenTypes, function (index, type) {
        type.checked = ko.observable(type.Selected);
    });
    return object;
}

function IsClassExist() {
    $.get('IsClassExist', { name: $("#Name").val(), id: $("#ID").val(), schoolId: $("#SchoolId").val() }, function (data) {
        if (data) {
            jQuery.when(waitingConfirm(window.getErrorMessage("ClassExists"), "Yes", "No")).done(function () {
                $("form").submit();
            }).fail(function () {

            });
        }
        else {
            $("form").submit();
        }
    }, "json");
}