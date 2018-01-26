function SchoolTypeChanged() {
    var schoolType = $("#SchoolTypeId").val();
    $("#SubTypeId-error").remove();
    InitSubSchoolType(schoolType);
    switch (schoolType) {
        case "1"://Public School
            //$("[typename='childCare']").attr("disabled", "true").hide();
            //$("[typename='FCC']").attr("disabled", "true").hide();
            $("#divClassroomCount").show();
            $("#AtRiskPercent").val("");
            DisplayClassroomCount(1);
            break;
        case "2": //Head Start
            //$("[typename='childCare']").attr("disabled", "true").hide();
            //$("[typename='FCC']").attr("disabled", "true").hide();
            $("#divClassroomCount").show();
            $("#AtRiskPercent").val(100);
            DisplayClassroomCount(2);

            break;
        case "3"://Child Care Center-based
            //$("[typename='childCare']").removeAttr("disabled").show();
            //$("[typename='FCC']").attr("disabled", "true").hide();
            $("#divClassroomCount").show();
            $("#AtRiskPercent").val("");
            DisplayClassroomCount(3);
            break;
        case "4"://Family Child Care (FCC)
            //$("[typename='FCC']").removeAttr("disabled").show();
            //$("[typename='childCare']").attr("disabled", "true").hide();
            $("#divClassroomCount").hide();
            $("#AtRiskPercent").val("");
            DisplayClassroomCount(4);
            break;
            //default:
            //    $("[typename='childCare']").attr("disabled", "true").hide();
            //    $("[typename='FCC']").attr("disabled", "true").hide();

    }
}
function FollowSchoolType(schoolType) {
    switch (schoolType) {
        case "1"://Public School 
            $("#divClassroomCount").show();
            DisplayClassroomCount(1);
            break;
        case "2": //Head Start 
            $("#divClassroomCount").show();
            $("#AtRiskPercent").val(100);
            DisplayClassroomCount(2);

            break;
        case "3"://Child Care Center-based 
            $("#divClassroomCount").show();
            DisplayClassroomCount(3);
            break;
        case "4"://Family Child Care (FCC)
            $("#divClassroomCount").hide();
            DisplayClassroomCount(4);
            break;
    }
}
function DisplayClassroomCount(classroomCount) {

    if (classroomCount == 1 || classroomCount == 2) {
        $("[for='ClassroomCount3Years']").parent().show();
    }
    else {
        $("[for='ClassroomCount3Years']").parent().hide();
        $("[for='ClassroomCount3Years']").parent().find("input").val(0);
    }

    if (classroomCount == 1 || classroomCount == 2) {
        $("[for='ClassroomCount4Years']").parent().show();
    }
    else {
        $("[for='ClassroomCount4Years']").parent().hide();
        $("[for='ClassroomCount4Years']").parent().find("input").val(0);
    }

    if (classroomCount == 1 || classroomCount == 2) {
        $("[for='ClassroomCount34Years']").parent().show();
    } else {
        $("[for='ClassroomCount34Years']").parent().hide();
        $("[for='ClassroomCount34Years']").parent().find("input").val(0);
    }

    if (classroomCount == 1 || classroomCount == 3) {
        $("[for='ClassroomCountKinder']").parent().show();
    } else {
        $("[for='ClassroomCountKinder']").parent().hide();
        $("[for='ClassroomCountKinder']").parent().find("input").val(0);
    }

    if (classroomCount == 1) {
        $("[for='ClassroomCountgrade']").parent().show();
    } else {
        $("[for='ClassroomCountgrade']").parent().hide();
        $("[for='ClassroomCountgrade']").parent().find("input").val(0);
    }

    if (classroomCount == 1 || classroomCount == 3) {
        $("[for='ClassroomCountOther']").parent().show();
    } else {
        $("[for='ClassroomCountOther']").parent().hide();
        $("[for='ClassroomCountOther']").parent().find("input").val(0);
    }

    if (classroomCount == 2) {
        $("[for='ClassroomCountEarly']").parent().show();
    } else {
        $("[for='ClassroomCountEarly']").parent().hide();
    }

    if (classroomCount == 3) {
        $("[for='ClassroomCountInfant']").parent().show();
    } else {
        $("[for='ClassroomCountInfant']").parent().hide();
    }

    if (classroomCount == 3) {
        $("[for='ClassroomCountToddler']").parent().show();
    } else {
        $("[for='ClassroomCountToddler']").parent().hide();
    }
    if (classroomCount == 3) {
        $("[for='ClassroomCountPreSchool']").parent().show();
    } else {
        $("[for='ClassroomCountPreSchool']").parent().hide();
    }
}

function TrsProviderChanged() {
    if ($("#TrsProviderId option:selected").text().toLowerCase() == "Not Participating".toLowerCase()) {

        $("[for='TrsLastStatusChange']").parent().hide();
        $("[for='TrsReviewDate']").parent().hide();

    } else {
        $("[for='TrsLastStatusChange']").parent().show();
        $("[for='TrsReviewDate']").parent().show();
    }
}

function IsSameAddressChanged() {
    if ($("#IsSamePhysicalAddress").prop("checked") == true) {
        $("#MailingAddress1").val($("#PhysicalAddress1").val());
        $("#MailingAddress2").val($("#PhysicalAddress2").val());
        $("#MailingCity").val($("#City").val());
        viewModel.mailSelectedState($("#StateId").val());
        viewModel.defaultMailCounty($("#CountyId").val());

        $("#MailingZip").val($("#Zip").val());
    }
}

$(function () {
    $("[for='IspOther']").hide();
    $("#IspOther").hide();
    InternetTypeChanged();
    if ($("#Notes").attr("disabled") == "disabled")
        $("#Notes").attr("placeholder", "");
});


function IspChanged() {

    if ($("#IspId option:selected").text().toLowerCase() != "Other".toLowerCase()) {
        $("[for='IspOther']").hide();
        $("#IspOther").hide();
    } else {
        $("[for='IspOther']").show();
        $("#IspOther").show();
    }
}
function InternetTypeChanged() {

    if ($("#InternetType option:selected").text().toLowerCase() != "Wireless".toLowerCase()) {
        $("[for='WirelessType']").hide();
        $("#WirelessType").hide();
    } else {
        $("[for='WirelessType']").show();
        $("#WirelessType").show();
    }
}


function InitSubSchoolType(schoolTypeId) {
    if (schoolTypeId == 0) {
        $("#SubTypeId").empty();
        $("#SubTypeId").hide();
        return;
    }
    $.get('/School/School/GetSchoolTypesJson', { parentId: schoolTypeId }, function (data) {
        var list = JSON.parse(data);
        $("#SubTypeId").empty();

        if (list.length > 1) {
            list.forEach(function (obj) {
                jQuery("#SubTypeId").append("<option  value='" + obj.Value + "'>" + obj.Text + "</option>");
            });
            $("#SubTypeId").show();
            if (subTypeVal != "" && subTypeVal != "0")
                $("#SubTypeId").val(subTypeVal);
        } else {
            $("#SubTypeId").hide();
        }
    });
}

function SubInitTrs(controlId) {
    $(controlId).rules("remove", "required");
    $(controlId).removeClass("input-validation-error");
    $(controlId + "-error").hide();
}

var playGroundCount = 0;

function DeletePlayGround(id) {
    $("#IsPlaygroundChanged").val("1");
    $("#divPlayGroundContainer" + id).remove();
    playGroundCount--;

}

var oldVSDesignationValue = 0;//edit page set default value
var SCHOOL_FourStart = 4; //  add/edit page set default value
function changeVerifiedStar(oldVlu) {
    var isFourStart = false;
    $(".VerifiedStar").each(function () {
        if ($(this).prop("checked")) {
            isFourStart = true;
        }
    });
    if (isFourStart)
        $("#VSDesignation").val(SCHOOL_FourStart);
    else
        $("#VSDesignation").val(oldVSDesignationValue);
}

function AutoAssign4Star(verifiedStar, recertificationBy, enabled, choosenRegulation, naeyc, canasa, necpa, nacecce, nafcc, acsi, usmilitary, qels) {
    var that = this;
    var oldVerifiedStar = verifiedStar || -1;

    var date = new Date();
    var minDate = new Date(2018, 8, 1);
    if (recertificationBy) {
        date = new Date(recertificationBy);
        if (date < minDate) {
            date = new Date(minDate);
        }
    }
    var recertification = date.Format("MM/dd/yyyy");
    recertificationBy = recertification;

    this.recertificationBy = ko.observable(recertificationBy);

    // U.S. Military:2, auto assign
    this.choosenRegulation = ko.observable(choosenRegulation);

    // checked: auto assign
    this.NAEYC = ko.observable(naeyc);
    this.CANASA = ko.observable(canasa);
    this.NECPA = ko.observable(necpa);
    this.NACECCE = ko.observable(nacecce);
    this.NAFCC = ko.observable(nafcc);
    this.ACSI = ko.observable(acsi);
    this.USMilitary = ko.observable(usmilitary);
    this.QELS = ko.observable(qels);

    this.enableAutoAssign4Star = ko.observable(enabled);

    this.verifiedStar = ko.observable(verifiedStar);

    function autoAssign4Star() {
        if (that.enableAutoAssign4Star()) {
            var regulatingUsMilitary = that.choosenRegulation() === "2";
            var typeOfNationalAccrChecked = that.NAEYC() || that.CANASA() || that.NECPA() || that.NACECCE() || that.NAFCC() || that.ACSI() || that.USMilitary() || that.QELS();
            if (regulatingUsMilitary || typeOfNationalAccrChecked) {
                that.verifiedStar(4);
            }
        }
    }

    this.enableAutoAssign4Star.subscribe(autoAssign4Star);

    this.choosenRegulation.subscribe(autoAssign4Star);

    this.NAEYC.subscribe(autoAssign4Star);
    this.CANASA.subscribe(autoAssign4Star);
    this.NECPA.subscribe(autoAssign4Star);
    this.NACECCE.subscribe(autoAssign4Star);
    this.NAFCC.subscribe(autoAssign4Star);
    this.ACSI.subscribe(autoAssign4Star);
    this.USMilitary.subscribe(autoAssign4Star);
    this.QELS.subscribe(autoAssign4Star);

    this.verifiedStar.subscribe(function (value) {
        if (oldVerifiedStar == -1 && value == 0) {
            that.recertificationBy("");
        }
        else if (oldVerifiedStar != value) {
            var today = new Date();
            if (today < minDate) {
                today = new Date(minDate);
            }
            that.recertificationBy(today.Format("MM/dd/yyyy"));
        } else {
            that.recertificationBy(recertification);
        }
    });

    this.recertificationBy.subscribe(function (value) {
        if (oldVerifiedStar == -1 && value == 0) {
        } else {
            var d = new Date(value);
            if (d < minDate) {
                d = new Date(minDate);
            }
            that.recertificationBy(d.Format("MM/dd/yyyy"));
        }
    });
}



