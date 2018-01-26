

function fnLimitNumberValidte(id, number) {
    if ($.isNumeric(number)) {
        $(id).rules("add", {
            maxlength: number,
            messages: { max: "Cannot exceed {0} characters." }
        });
    }
}

function fnMouDocument() {
    
    $("#statusSigned").click(function () {
        $("[for='MouDocument']").show();
        $("#divMouDocument").show();
    });

    $("#statusNotSigned").click(function () {
        $("[for='MouDocument']").hide();
        $("#divMouDocument").hide();
    });
}


function InitControlsByRole() {
    $("input.form-control").add("select.form-control").add("textarea.form-control").add(":checkbox")
        .not("#statusNotSigned").not("#statusSigned").not("#DistrictNumber")
        .not("#txtBasicCommunity") 
        .each(function (i, obj) {
            var id = obj.id;
            if (id != "") {
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