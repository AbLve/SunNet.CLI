

function InterventionChanged()
{
    var txt = $("#InterventionStatus option:selected").text().trim().toUpperCase();
    if (txt == undefined || txt == "") {
        return false;
    } else {
        if (txt == "Materials Eligible".toUpperCase()) {
            $("#FundingId").val("");
            $("#FundingId").show();
            $("#FundingId").rules("add", { required: true, messages: { required: "Classroom Funding can not be null." } });
            $("[for='FundingId']").show();
            $("#KitOptions").show();
            $("#FundingId-error").show();
        }
        else {
            $("#FundingId").rules("remove", "required");
            $("#FundingId").val(0);

            $("[for='FundingId']").hide();
            $("#FundingId").hide();
            $("#FundingId-error").hide();
            $("#KitOptions select").val(0);
            $("#KitOptions").hide();
        }

        if (txt == "Other".toLowerCase()) {
            $("[for='InterventionOther']").show();
            $("#InterventionOther").show();
        }
        else {
            $("[for='InterventionOther']").hide();
            $("#InterventionOther").hide();
        }
    }
}

function InternetTypeChanged2()
{
    if ($("#InternetType option:selected").text().toLowerCase() == "Wireless".toLowerCase())
    {
        $("[for='WirelessType']").show();
        $("#WirelessType").show();
        $("#WirelessType-error").show();
        $("#WirelessType").rules("add", { required: true, messages: { required: "Type of Wireless is required." } });
        $("#WirelessType").val("");
    } else
    {
        $("#WirelessType").rules("remove", "required");
        $("#WirelessType").val(0);
        $("[for='WirelessType']").hide();
        $("#WirelessType-error").hide();
        $("#WirelessType").hide();
    }
}
function InitControlsByRole()
{
    registerFormCallbacks("@(formId)", {
        onPosted: function (response)
        {
            redirectBack("index");
        }
    });

    $("input.form-control ").not("#txtCommunity").not("#LCchildrenNumber").not("#TypeOfClassroom").not("#LCchildrenTypeId").not("#txtSchool").not("#CommunityName").not("#SchoolName").add("select.form-control")
        .add("textarea.form-control").add("#IsInteractiveWhiteboard").add("#IsUsingInClassroom")
        .each(function (i, obj)
        {
            var id = obj.id;
            if (id != "") {
                if (id != "TypeOfClassroom" && id != "LCchildrenNumber" && id != "LCchildrenTypeId")
                {
                    $(obj).parent().parent().attr("data-bind", "visible: " + id + " != 'X'");
                    $(obj).attr("data-bind", "visible: " + id + " != 'X'");
                    $(obj).attr("data-bind", "disable: " + id + " == 'R'");
                }
               
            }
        });
    ko.applyBindings(roleJson);
}

function BeforeSubmit()
{
    $.get('IsClassroomExist', { name: $("#Name").val(), id: $("#ID").val(), schoolId: $("#SchoolId").val() }, function (data)
    {
        if (data)
            return ShowConfirm();
        else
        {
            $("form").submit();
        }
    }, "json");
    return false;
}

function ShowConfirm()
{
    jQuery.when(waitingConfirm(window.getErrorMessage("ClassroomExists"), "Yes", "No")).done(function ()
    {
        $("form").submit();
    }).fail(function ()
    {

    });
    return false;
}

function GenerateNumber(obj)
{
    var i = 0;
    for (i = 0; i <= 99; i++)
    {
        $(obj).append("<option value='"+i+"'>"+i+"</option>");
    }
    
}