


function InitControlsByRole() {
    $("input.form-control").add("select.form-control").add("textarea.form-control").not("#txtCommunity").not("#txtSchool")
        .not("#Ethnicity").not("#EthnicityOther").not("#UserInfo_Status")
        .not("#SecondaryLanguageOther").not("#PrimaryLanguageOther").not("#CLIFundingID").not("#EmployedBy")
        .not("#AgeGroupOther").not("#ageGroups").not("#TeacherTypeOther").not("#PDOther").not("#CoachAssignmentWayOther")
        .not("#ECIRCLEAssignmentWayOther").not("#UserInfo_InternalID").not("#UserInfo_InvitationEmail")
        .each(function (i, obj) {
            var id = obj.id;
            if (id != "") {
                if (id == "CoachingHours") {
                    //console.log(roleJson)
                }
                if (id == "StateId") {
                    $(obj).parent().parent().attr("data-bind", "visible: " + id + " != 'X'");
                    $(obj).attr("data-bind", "disable: " + id + " == 'R',value:selectedState ,visible: " + id + " != 'X'");
                } else if (id == "CountyId") {
                    $(obj).parent().parent().attr("data-bind", "visible: " + id + " != 'X'");
                    $(obj).attr("data-bind", "disable: " + id + " == 'R',options: countiesOptions, optionsText: 'Text', optionsValue: 'Value',value:selectedCounty, visible: " + id + " != 'X'");
                }
                else {
                    $(obj).parent().parent().attr("data-bind", "visible: " + id + " != 'X'");
                    $(obj).attr("data-bind", "disable: " + id + " == 'R',visible: " + id + " != 'X'");
                }
            }
        });
    ko.applyBindings(roleJson);
}