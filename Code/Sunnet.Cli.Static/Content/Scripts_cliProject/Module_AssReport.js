var echartsTheme = echartsTheme || {};
echartsTheme["default"] = echartsTheme.infograpic;

String.prototype.padLeft = String.prototype.padLeft || function (totalWidth, paddingChar) {
    if (paddingChar != null) {
        return this.pad(totalWidth, paddingChar, false);
    } else {
        return this.pad(totalWidth, ' ', false);
    }
}
String.prototype.padRight = String.prototype.padRight || function (totalWidth, paddingChar) {
    if (paddingChar != null) {
        return this.pad(totalWidth, paddingChar, true);
    } else {
        return this.pad(totalWidth, ' ', true);
    }
}
String.prototype.pad = String.prototype.pad || function (totalWidth, paddingChar, isRightPadded) {
    if (this.length < totalWidth) {
        var paddingString = new String();
        for (i = 1; i <= (totalWidth - this.length) ; i++) {
            paddingString += paddingChar;
        }

        if (isRightPadded) {
            return (this + paddingString);
        } else {
            return (paddingString + this);
        }
    } else {
        return this;
    }
}
if (!String.prototype.trim) {
    String.prototype.trim = function () {
        return this.replace(/(^\s*)|(\s*$)/g, "");
    }
}
Array.prototype.uniquePush = function (item) {
    function getCount(arr, item) {
        var index = 0;
        for (var i = 0; i < arr.length ; i++) {
            var matches = /([A-Za-z0-9\ \-^\(]*)(\ *\(\d{1,2}\))?$/g.exec(arr[i]);
            if (matches && matches.length >= 2 && matches[1].trim() == item.trim()) {
                index++;
            }
        }
        return index;
    }
    if (typeof item == "string" || typeof item == "number") {
        if (this.indexOf(item) >= 0) {
            var exists = getCount(this, item);
            item = item + " (" + exists + ")";
        }
        this.push(item);
    }
}

function ParentMeasure(text) {
    this.text = text;
    this.colspan = 1;
    this.times = 0;
    this.getText = function () {
        var result = this.text.slice(0, this.getTotalLength());
        if (this.colspan % 2 == 0)
            result = "".padRight(8, " ") + result;
        return result;
    };
    this.getPivot = function () {
        if (this.colspan % 2 == 0)
            return Math.floor(this.colspan / 2) - 1;
        return Math.floor(this.colspan / 2);
    };
    this.getTotalLength = function () {
        return this.colspan * 8;
    }
}

function ParentMeasuresModel() {
    this.parentMeasures = {};
    this.add = function (key, colspan) {
        var _k = key.replace(/\ /ig, "_");
        var _pm = this.parentMeasures[_k];
        if (_pm) {
            _pm.colspan++;
        } else {
            _pm = new ParentMeasure(key);
            _pm.colspan = colspan || 1;
            this.parentMeasures[_k] = _pm;
        }
    };
    this._total = -1;

    this.getText = function (key) {
        var _k = key.replace(/\ /ig, "_");
        var _pm = this.parentMeasures[_k];
        var result = "";
        if (_pm) {
            var p = _pm.getPivot();
            var isTick = _pm.times % 2 == 0;
            if (!isTick) {
                var times = Math.floor(_pm.times / 2) % _pm.colspan;
                if (_pm.colspan == 1) {
                    result = "|<--" + _pm.text.slice(0, 4) + "-->|";
                }
                else if (_pm.colspan == 2) {
                    var totalLength = 10;
                    var spaceNeedToPad = totalLength > _pm.text.length ? Math.floor((totalLength - _pm.text.length) / 2) : 4;
                    spaceNeedToPad = spaceNeedToPad.length < 4 ? 4 : spaceNeedToPad;
                    if (times == 0) {
                        result = "|<--".padRight(spaceNeedToPad, " ") + _pm.text.slice(0, 7);
                    }
                    if (times == 1) {
                        result = _pm.text.slice(7, 16).padRight(14, " ") + "-->|".padLeft(spaceNeedToPad, " ");
                    }
                } else {
                    if (times == 0) {
                        result = "|<--".padRight(20, " ");
                    }
                    if (1 <= times && times < p) {
                        result = " ";
                    }
                    if (times == p) {
                        result = _pm.getText();
                    }
                    if (p < times && times < _pm.colspan - 1) {
                        result = " ";
                    }
                    if (times == _pm.colspan - 1) {
                        result = "-->|".padLeft(22, " ");
                    }
                }
            }
            _pm.times++;
        }
        return result + " ";
    }

    this.getTextForGrouth = function (key) {
        var _k = key.replace(/\ /ig, "_");
        var _pm = this.parentMeasures[_k];
        var result = "";
        if (_pm) {
            var p = _pm.getPivot();
            var isTick = _pm.times % 2 == 0;
            if (!isTick) {
                var times = Math.floor(_pm.times / 2) % _pm.colspan;
                if (_pm.colspan == 1) {
                    result = "|<--" + _pm.text + "-->|";
                }
                else if (_pm.colspan == 2) {
                    var totalLength = 10;
                    var spaceNeedToPad = totalLength > _pm.text.length ? Math.floor((totalLength - _pm.text.length) / 2) : 4;
                    spaceNeedToPad = spaceNeedToPad.length < 4 ? 4 : spaceNeedToPad;
                    if (times == 0) {
                        result = "";
                    }
                    if (times == 1) {
                        result = "|<--".padRight(spaceNeedToPad, " ") + _pm.text;
                    }
                } else {
                    if (times == 0) {
                        result = "|<--".padRight(20, " ");
                    }
                    if (1 <= times && times < p) {
                        result = " ";
                    }
                    if (times == p) {
                        result = _pm.getText();
                    }
                    if (p < times && times < _pm.colspan - 1) {
                        result = " ";
                    }
                    if (times == _pm.colspan - 1) {
                        result = "-->|".padLeft(22, " ");
                    }
                }
            }
            _pm.times++;
        }
        return result + " ";
    }
}

function GetNumber(input) {
    input = input + "";
    input = input.replace(/%$/ig, "");
    if (isNaN(+input)) {
        return 0;
    }
    return +input;
}
function ReportOption() {
    this.title = {
        text: '',
        subtext: ''
    };
    this.tooltip = {
        trigger: 'axis'
    };
    this.legend = {
        show:true,
        data: [],
        x: 150,
        y: 0
    };
    this.toolbox = {
        show: true,
        orient:"vertical",
        x: 5,
        y:50,
        feature: {
            dataZoom: {
                show: true,
                title: {
                    dataZoom: 'Zoom',
                    dataZoomReset: 'Zoom'
                }
            },
            magicType: {
                show: true,
                title: {
                    line: 'Line Chart',
                    bar: 'Bar Chart',
                    stack: 'Stack Chart',
                    tiled: 'Tiled Chart'
                },
                type: ['line', 'bar'] // 'line', 'bar', 'stack', 'tiled'
            },
            restore: {
                show: true,
                title: 'Restore'
            }
        }
    };
    this.grid = {
        y: 70,
        width: 80,
        height: 330
    };
    this.calculable = false;
    this.xAxis = [
    {
        type: 'category',
        data: [],
        boundaryGap: true,
        axisLabel: {
            interval: 0,
            rotate:90
            },
        splitArea: { show: true,areaStyle: {
            color:["#dddddd","#eeeeee"]
        } }
        },
        {
            type: 'category',
            data: [],
            boundaryGap: true,
            axisLabel: {
                interval: 0
            },
            axisTick: { show: true }
        }
    ];
    this.yAxis = [
        {
            type: 'value',
            axisLabel: {
            },
            axisTick: {}
        }
    ];
    this.series = [];
}

