
$(function () {
    //Context控制单选
    $(":input[name='Context'][type=checkbox]").each(function () {
        $(this).click(function () {
            var value = $(this).attr("value");
            if (this.checked) {
                var isOther = $(this).attr("isOther") == "true";
                if (isOther) {
                    $("#ContextOther").prop("disabled", "");
                } else {
                    $("#ContextOther").prop("disabled", "disabled");
                    $("#ContextOther").val("");
                }
                $(":input[name='Context'][type=checkbox][value!=" + value + "]").each(function () {
                    this.checked = false;
                });
            }
            else {
                if (isOther) {
                    $("#ContextOther").prop("disabled", "disabled");
                    $("#ContextOther").val("");
                }
            }
        });
    });

    //Screening控制单选
    $(":input[name='screening'][type=checkbox]").each(function () {
        $(this).click(function () {
            var value = $(this).attr("value");
            if (this.checked) {
                $(":input[name='screening'][type=checkbox][value!=" + value + "]").each(function () {
                    this.checked = false;
                });
            }
        });
    });

    //Language控制单选
    $("input[name='language'][type=checkbox]").each(function () {
        $(this).click(function () {
            var value = $(this).attr("value");
            if (this.checked) {
                $("input[name='language'][type=checkbox][value!=" + value + "]").each(function () {
                    this.checked = false;
                });
            }
        });
    });
})


//控制Content
function ContentClick(sender) {
    if ($(sender)[0].checked) {
        $("#ContentOther").removeAttr("disabled");
    } else {
        $("#ContentOther").attr("disabled", "disabled");
        $("#ContentOther").val('');
    }
}


//控制Strategy
function StrategyClick(sender) {
    if ($(sender)[0].checked) {
        $("#StrategyOther").removeAttr("disabled");
    } else {
        $("#StrategyOther").attr("disabled", "disabled");
        $("#StrategyOther").val('');
    }
}
