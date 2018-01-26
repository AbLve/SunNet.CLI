if (!Date.prototype.Format) {
    Date.prototype.Format = function (fmt) {
        var o = {
            "M+": this.getMonth() + 1,
            "d+": this.getDate(),
            "H+": this.getHours(),
            "h+": this.getHours() > 12 ? this.getHours() - 12 : this.getHours(),
            "m+": this.getMinutes(),
            "s+": this.getSeconds(),
            "q+": Math.floor((this.getMonth() + 3) / 3),
            "S+": this.getMilliseconds()
        };
        if (/(y+)/.test(fmt)) fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
        for (var k in o) {
            var padLeft = "00";
            if (k === "S+") padLeft = "000";
            if (new RegExp("(" + k + ")").test(fmt)) fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : ((padLeft + o[k]).substr(("" + o[k]).length)));
        }
        return fmt;
    }
}

if (!Array.prototype.filter) {
    Array.prototype.filter = function (fun /*, thisp*/) {
        var len = this.length >>> 0;
        if (typeof fun != "function") {
            throw new TypeError();
        }

        var res = [];
        var thisp = arguments[1];
        for (var i = 0; i < len; i++) {
            if (i in this) {
                var val = this[i]; // in case fun mutates this
                if (fun.call(thisp, val, i, this)) {
                    res.push(val);
                }
            }
        }
        return res;
    };
}

Function.prototype.method = function(name, func) {
    if (!this.prototype[name]) {
        this.prototype[name] = func;
    }
}
//Object.method('superior', function (name) {
//    var that = this,
//        method = that[name];
//    return function () {
//        return method.apply(that, arguments);
//    };
//});

function isNumber(value) {
    return typeof (value) === "number" || (typeof (value) === "string" && !isNaN(+value));
}

function parseDate(date, format) {
    /// format: MM/dd/yyyy as default
    if (!format) {
        format = "MM/dd/yyyy";
    }
    if (!date) {
        return "";
    }
    var yearIndex = format.indexOf("yyyy"),
        monthIndex = format.indexOf("MM"),
        dayIndex = format.indexOf("dd"),
        hourIndex = format.indexOf("HH"),
        minuteIndex = format.indexOf("mm"),
        secondIndex = format.indexOf("ss");
    var year = 0, month = 0, day = 0, hour = 0, minute = 0, second = 0;
    year = date.substr(yearIndex, 4);
    month = +date.substr(monthIndex, 2);
    if (isNumber(month))
        month--;
    day = date.substr(dayIndex, 2);
    hour = hourIndex >= 0 && date.substr(hourIndex, 2);
    minute = minuteIndex >= 0 && date.substr(minuteIndex, 2);
    second = secondIndex >= 0 && date.substr(secondIndex, 2);
    if (!hour) {
        hour = 12;
    }
    var newDate = new Date(year, month, day,hour,minute,second);
    return newDate;
}

(function ($) {
    var ua = navigator.userAgent;
    var isiOS = /iPad/i.test(ua) || /iPhone OS 3_1_2/i.test(ua) || /iPhone OS 3_2_2/i.test(ua);
    var isAndroid = /Android/i.test(ua);
    $.support.mobile = isiOS||isAndroid;
    $.support.isiOS = isiOS;
    $.support.isAndroid = isAndroid;

    var isIE = /msie/i.test(ua);
    $.support.isIE = isIE;
})(jQuery || {});

///0: 表示日期相等，1: 表示d1 > d2 , 2: 表示d1 < d2
function DateCompare(date1, date2) {
    var tmpD1 = new Date(date1);
    var tmpD2 = new Date(date2);
    if (tmpD1 === tmpD2) return 0;
    if (tmpD1 > tmpD2) return 1;
    if (tmpD1 < tmpD2) return 2;
}