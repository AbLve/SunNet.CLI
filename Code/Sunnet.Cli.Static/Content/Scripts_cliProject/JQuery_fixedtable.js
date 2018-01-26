(function ($) {
    jQuery.fn.fixedTable = function (options) {
        var opts = $.extend({}, $.fn.fixedTable.defaults, options, true);
        // iterate and reformat each matched element    
        return this.each(function () {
            $this = $(this);
            // build element specific options    
            var o = $.meta ? $.extend({}, opts, $this.data()) : opts;
            var tableID = $(this).attr("id");
            // update element styles   
            var fixColumnColor = o.fixColumnColor;
            var fixColumnNumber = o.fixColumnNumber;
            var fixRowNumber = o.fixRowNumber;
            var maxwidth = $("" + o.maxwidth + "")[0].offsetWidth;
            var maxheight = o.maxheight;
            /// <summary>
            ///     锁定表头和列
            ///     <para> sorex.cnblogs.com </para>
            /// </summary>
            /// <param name="tableID" type="String">
            ///     要锁定的Table的ID
            /// </param>
            /// <param name="fixColumnNumber" type="Number"> 
            ///     要锁定列的个数
            /// </param>
            /// <param name="fixColumnColor" type="String">
            ///     要锁定列的颜色
            /// </param>
            /// <param name="maxwidth" type="Number">
            ///     显示的最大宽度
            /// </param>
            /// <param name="maxheight" type="Number">
            ///     显示的最大高度
            /// </param>
            if ($("#" + tableID + "_tableLayout").length != 0) {
                $("#" + tableID + "_tableLayout").before($("#" + tableID));
                $("#" + tableID + "_tableLayout").empty();
            }
            else {
                $("#" + tableID).after("<div id='" + tableID + "_tableLayout' style='overflow:hidden;max-height:" + maxheight + "px; max-width:" + maxwidth + "px;'></div>");
            }
            $('<div id="' + tableID + '_tableFix"></div>'
            + '<div id="' + tableID + '_tableHead"></div>'
            + '<div id="' + tableID + '_tableColumn"></div>'
            + '<div id="' + tableID + '_tableData"></div>')
            .appendTo("#" + tableID + "_tableLayout");
            var oldtable = $("#" + tableID);


            //固定的行和列交叉的部分(tableFix)
            var tableFixClone = oldtable.clone(true);
            tableFixClone.find("tr:gt(" + parseInt(fixRowNumber - 1) + ")").each(function () { //去除需要固定的行之外的多余行
                $(this).remove();
            });
            tableFixClone.find("tr").each(function () {   //将要固定的列之外的列清空，用于保持原来样式
                $(this).children("*:gt(" + parseInt(fixColumnNumber - 1) + ")").each(function () {
                    $(this).empty();
                });
            });
            tableFixClone.attr("id", tableID + "_tableFixClone");
            $("#" + tableID + "_tableFix").append(tableFixClone);


            //计算固定行和列交叉的部分的宽度
            var ColumnsWidth = 0;
            var ColumnsNumber = 0;
            $("#" + tableID + " tbody:first tr:last td:lt(" + fixColumnNumber + ")").each(function () {
                ColumnsWidth += $(this).outerWidth(true);
                ColumnsNumber++;
            });

            //固定表头（tableHead）
            var tableHeadClone = oldtable.clone(true);
            tableHeadClone.find("tr:gt(" + parseInt(fixRowNumber - 1) + ")").each(function () { //去除需要固定的行之外的多余行
                $(this).remove();
            });
            tableHeadClone.find("tr").each(function () {
                $(this).children("*:lt(" + parseInt(fixColumnNumber) + ")").each(function () { //将要固定的列之外的列清空，用于保持原来样式
                    $(this).empty();
                });
            });
            //在清空之后的列中添加一个空的div，宽度为固定列的宽度-边框的宽度
            var newdiv_head = $("<div style='min-width:" + parseInt(ColumnsWidth - 1) + "px'>&nbsp;</div>");
            newdiv_head.appendTo(tableHeadClone.find("tr:eq(0)").children("*:eq(" + parseInt(fixColumnNumber - 1) + ")"));
            tableHeadClone.attr("id", tableID + "_tableHeadClone");
            $("#" + tableID + "_tableHead").append(tableHeadClone);

            //计算固定表头的高度
            var HeadHeight = 0;
            $("#" + tableID + "_tableHead tr:lt(" + fixRowNumber + ")").each(
                function () {
                    HeadHeight += $(this).height();
                });

            //固定列
            var tableColumnClone = oldtable.clone(true);
            tableColumnClone.find("tr").each(function () {  //去除多余列
                $(this).children("*:gt(" + parseInt(fixColumnNumber - 1) + ")").remove();
            });
            tableColumnClone.find("tr:lt(" + parseInt(fixRowNumber) + ")").each(function () { //将多余行清空，
                $(this).empty();
            });
            //在清空之后的行中添加一个空的div，高度为固定表头的高度-边框的高度
            var columnfixedtd = tableColumnClone.find("tr:eq(" + parseInt(fixRowNumber - 1) + ")");
            var newdiv = $("<div style='min-height:" + parseInt(HeadHeight - 1) + "px'>&nbsp;</div>");
            newdiv.appendTo($(columnfixedtd));
            tableColumnClone.attr("id", tableID + "_tableColumnClone");
            $("#" + tableID + "_tableColumn").append(tableColumnClone);

            //数据表格
            var tableData = oldtable.clone(true);
            tableData.find("tr:lt(" + parseInt(fixRowNumber) + ")").each(function () { //其余元素设置为不可用
                $(this).children().each(function () {
                    var oldwidth = $(this).width();
                    $(this).empty();
                    $(this).css("min-width", oldwidth + "px");
                });
            });
            tableData.find("tr:gt(" + parseInt(fixRowNumber - 1) + ")").each(function () { //其余元素设置为不可用
                $(this).children("*:lt(" + parseInt(fixColumnNumber) + ")").each(function () {
                    var oldwidth = $(this).width();
                    $(this).empty();
                    $(this).css("min-width", oldwidth + "px");
                });
            });

            $("#" + tableID + "_tableData").append(tableData);

            $("#" + tableID + "_tableLayout table").each(function () {
                $(this).css("margin", "0");
            });

            $("#" + tableID + "_tableHead").css("height", HeadHeight);
            $("#" + tableID + "_tableFix").css("height", HeadHeight);
            //if ($.browser.msie) {
            //    switch ($.browser.version) {
            //        case "7.0":
            //            if (ColumnsNumber >= 3) ColumnsWidth--;
            //            break;
            //        case "8.0":
            //            if (ColumnsNumber >= 2) ColumnsWidth--;
            //            break;
            //    }
            //}

            $("#" + tableID + "_tableColumn").css("width", ColumnsWidth);
            $("#" + tableID + "_tableFix").css("width", ColumnsWidth);
            $("#" + tableID + "_tableData").scroll(function () {
                $("#" + tableID + "_tableHead").scrollLeft($("#" + tableID + "_tableData").scrollLeft());
                $("#" + tableID + "_tableColumn").scrollTop($("#" + tableID + "_tableData").scrollTop());
            });
            $("#" + tableID + "_tableFix").css({ "overflow": "hidden", "position": "relative", "z-index": "50", "background-color": fixColumnColor });
            $("#" + tableID + "_tableHead").css({ "overflow": "hidden", "max-width": maxwidth, "position": "relative", "z-index": "45", "background-color": fixColumnColor });
            $("#" + tableID + "_tableColumn").css({ "overflow": "hidden", "max-height": parseInt(maxheight - 34), "position": "relative", "z-index": "40", "background-color": fixColumnColor });
            $("#" + tableID + "_tableData").css({ "overflow": "scroll", "max-width": maxwidth, "max-height": parseInt(maxheight - 17), "position": "relative", "z-index": "35" });


            if (oldtable.width() <= maxwidth) {   //如果table本身宽度小于最大宽度值，则将宽度设置为原始数据宽度
                $("#" + tableID + "_tableHead").css("width", oldtable.width() - 34);//去除滚动条的宽度
                $("#" + tableID + "_tableData").css("width", oldtable.width() - 17);
            }
            else {
                $("#" + tableID + "_tableHead").css("width", $("#" + tableID + "_tableHead").width() - 17);//去除滚动条的宽度
            }

            if (oldtable.height() <= maxheight) {   //如果最大高度大于数据高度，则将高度设置为自定义高度+滚动条高度（去除多余部分）
                var customHeight = 0;
                if ($("#" + tableID + "_tableData").height() > maxheight) {
                    customHeight = maxheight;
                }
                else {
                    customHeight = $("#" + tableID + "_tableData").height();
                }
                $("#" + tableID + "_tableLayout").css("height", customHeight + 17);
            }
            else {
                if ($("#" + tableID + "_tableColumn").height() > $("#" + tableID + "_tableColumn table").height()) {
                    $("#" + tableID + "_tableColumn").css("height", $("#" + tableID + "_tableColumn table").height());
                    $("#" + tableID + "_tableData").css("height", $("#" + tableID + "_tableColumn table").height());
                }
            }

            $("#" + tableID + "_tableFix").offset($("#" + tableID + "_tableLayout").offset());
            $("#" + tableID + "_tableHead").offset($("#" + tableID + "_tableLayout").offset());
            $("#" + tableID + "_tableColumn").offset($("#" + tableID + "_tableLayout").offset());
            $("#" + tableID + "_tableData").offset($("#" + tableID + "_tableLayout").offset());
            $("#" + tableID + "_tableLayout").css("max-width", maxwidth);

            var isChrome = navigator.userAgent.toLowerCase().match(/chrome/) != null;//判断是否是谷歌浏览器
            if (isChrome) {
                var tablecolumntop = $("#" + tableID + "_tableColumn").css("top");
                $("#" + tableID + "_tableColumn").css("top", parseInt(tablecolumntop) + 1);
            }

            $(oldtable).remove();
        });
    };
    // 插件的defaults    
    $.fn.fixedTable.defaults = {
        fixColumnColor: '#fff',
        fixColumnNumber: 1,
        fixRowNumber: 1,
        maxwidth: "body",//最大宽度元素选择器
        maxheight: 350
    };
})(jQuery);