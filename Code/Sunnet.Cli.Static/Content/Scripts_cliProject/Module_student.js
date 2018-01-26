function EthnicityChanged() {
    if ($("#Ethnicity :selected").text() == "Other")
        $("#EthnicityOther").attr("type", "text");
    else {
        $("#EthnicityOther").val("");
        $("#EthnicityOther").attr("type", "hidden");
    }
}

function PrimaryLanguageChanged() {
    if ($("#PrimaryLanguageId :selected").text() == "Other")
        $("#PrimaryLanguageOther").attr("type", "text");
    else {
        $("#PrimaryLanguageOther").val("");
        $("#PrimaryLanguageOther").attr("type", "hidden");
    }
}

function SecondaryLanguageChanged() {
    if ($("#SecondaryLanguageId :selected").text() == "Other")
        $("#SecondaryLanguageOther").attr("type", "text");
    else {
        $("#SecondaryLanguageOther").val("");
        $("#SecondaryLanguageOther").attr("type", "hidden");
    }
}
function ChkAllClicked() {
    if ($(this).is(":checked"))
        $("[name='chkClassSelect']").prop("checked", true);
    else {
        $("[name='chkClassSelect']").prop("checked", false);
    }
}

function SchoolIdChanged() {
    jQuery.post("/Student/Student/SchoolIdGetStudentClassAll", { schoolId: $("#SchoolId").val() }, function (result) {
        //list.StudentClassList.push(result[0]);
        for (var i = 0; i < result.length; i++) {
            list.StudentClassList.push(result[i]);
        }
    }, 'json');
}
function CheckAll() {
    if ($("#chkSelectAll").is(":checked")) {
        $("[name='chkClasses']").prop("checked", true);
        $("#hiddenChk").prop("checked", false);
    }

    else {
        $("[name='chkClasses']").prop("checked", false);
        $("#hiddenChk").prop("checked", false);
    }
}

function ParentInvitation(id) {

    $.get("GeneratePdf", { id: item.id }, function (data) {
        alert("Data Loaded: " + data);
    });
}

$(function () {
    $("#Ethnicity").change(EthnicityChanged);
    $("#PrimaryLanguageId").change(PrimaryLanguageChanged);
    $("#SecondaryLanguageId").change(SecondaryLanguageChanged);
    $("#chkSelectAll").click(CheckAll);
});

function BeforeSubmit() {
    if ($("#BirthDate").val() == "" || $("#CommunityId").val() == "" || $("#LastName").val() == "" || $("#FirstName").val() == "")
        return true;
    $.get('IsStudentExist',
        { firstName: $("#FirstName").val(), lastName: $("#LastName").val(), birthDate: $("#BirthDate").val(), communityId: $("#CommunityId").val() },
        function (data) {
            if (data) {
                window.showMessage("hint", window.getErrorMessage("studentExists"));
                return false;
            }
            else {
                $("form").submit();
            }
        }, "json");

    return false;
}

function ShowConfirm() {
    jQuery.when(waitingConfirm(window.getErrorMessage("studentExists"), "Yes", "No")).done(function () {
        $("form").submit();
    }).fail(function () {

    });
    return false;
}