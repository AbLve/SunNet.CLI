/*************************
Input format control
************************/
;
+(function ($) {
    var FormatNumber = function (element, options) {
        this.options = options;
        this.$element = $(element);
    }
    FormatNumber.prototype = {
        constructor: FormatNumber,
        getValue: function () {
            var data = this.$element.val().replace("$", "").replace(":", "").replace(/,/g, "");
            data = parseFloat(data);
            if (isNaN(data))
                return "";
            else {
                return data.toString();
            }
        },
        setValue: function (value) {
            this.$element.val(value).change().removeAttr("ValueBeforeEdit");
        },
        recordValue: function () {
            var value = this.getValue();
            this.$element.attr("ValueBeforeEdit", value).val(value).change();
        },
        getRecordValue: function () {
            return this.$element.attr("ValueBeforeEdit") ? this.$element.attr("ValueBeforeEdit") : "0.00";
        },
        formatValue: function () {
            var $this = this.$element;
            var value = this.getValue();
            if (!value && this.options.nullable) {
                this.setValue("");
            }
            else {
                var _intAndFloat = value.split(".");
                if (isNaN(parseFloat(value))) {
                    var _ValueBeforeEdit = this.getRecordValue();
                    _intAndFloat = _ValueBeforeEdit.split(".");
                }
                if (_intAndFloat.length < 2) {
                    _intAndFloat.push("0");
                }

                var _int = _intAndFloat[0].slice(0, this.options.int);
                if (value.indexOf("-") > -1) {
                    _int = _intAndFloat[0].slice(0, this.options.int + 1);
                }
                var _float = (_intAndFloat[1] + "0000000000000").slice(0, this.options.float);

                if (!_int) {
                    _int = "0";
                }
                var _intArr = _int.split("").reverse();
                var _intArr2 = [];
                var _intArr3 = [];
                for (var i = 0; i < _intArr.length; i++) {
                    if (parseInt(_intArr[i], 10) >= 0) {
                        _intArr2.push(_intArr[i]);
                    }
                }
                for (var i = 0; i < _intArr2.length; i++) {
                    _intArr3.push(_intArr2[i]);
                    if (this.options.separator) {
                        if (i % 3 == 2 && i < _intArr2.length - 1) {
                            _intArr3.push(this.options.separator);
                        }
                    }
                }
                if (value.indexOf("-") >= 0) {
                    _intArr3.push("-");
                }
                if ($this.hasClass("_money")) {
                    //_intArr3.push("$");
                }
                _intArr3.reverse();
                var _newValue = _intArr3.join("");
                if (this.options.float && (_float > 0 || this.options.autocompletion)) {
                    _newValue = _newValue + "." + _float;
                }
                this.setValue(_newValue);
            }
        }
    }
    $.fn.FormatNumber = function (type, execFunc) {
        var $input = $(this);
        if (this && $input && $input.length && $input.get(0).type == "text") {
            var options = {};
            if (type.indexOf("_") != 0) {
                execFunc = type;
                type = $input.data("type", type);
            }
            $.extend(options, { int: 12, float: 9 }, $.fn.FormatNumber.defaults[type]);
            if ($input.hasClass("_nullable")) {
                options.nullable = true;
            }
            if ($input.attr("int")) {
                options.int = parseInt($input.attr("int"), 10);
            }
            if ($input.attr("float")) {
                options.float = parseInt($input.attr("float"), 10);
            }
            var formatNumber = $input.data("FormatNumber");
            if (!formatNumber) {
                $input.data("FormatNumber", formatNumber = new FormatNumber(this, options));
                $input.data("type", type);
                $input.on("focus", function () {
                    formatNumber.recordValue();
                }).on("blur", function () {
                    formatNumber.formatValue();
                });
            }
            if (formatNumber[execFunc]) {
                return formatNumber[execFunc]();
            }
        }
        return $input;
    }
    $.fn.FormatNumber.defaults = {
        _money: {
            int: 9,
            float: 2,
            separator: ",",
            autocompletion: true,
            nullable: false
        },
        _station1: {
            int: 9,
            float: 0,
            autocompletion: true,
            nullable: false
        },
        _station2: {
            int: 5,
            float: 1,
            autocompletion: true,
            nullable: false
        },
        _long: {
            int: 3,
            float: 5,
            autocompletion: true,
            nullable: false
        },
        _number: {
            int: 12,
            float: 8,
            autocompletion: true,
            nullable: false
        },
        _numfloat1: {
            int: 2,
            float: 2,
            autocompletion: true,
            nullable: false
        },
        _numfloat2: {
            int: 3,
            float: 2,
            autocompletion: true,
            nullable: false
        },
        _numfloat3: {
            int: 3,
            float: 3,
            autocompletion: true,
            nullable: false
        }
    }
    $(function () {
        //$.maskedinput.min.js
        var dateRegex = /^((0[13578]|1[02])[\/.]31[\/.](18|19|20)[0-9]{2})|((01|0[3-9]|1[1-2])[\/.](29|30)[\/.](18|19|20)[0-9]{2})|((0[1-9]|1[0-2])[\/.](0[1-9]|1[0-9]|2[0-8])[\/.](18|19|20)[0-9]{2})|((02)[\/.]29[\/.](((18|19|20)(04|08|[2468][048]|[13579][26]))|2000))$/;
        $("body").on("focus", "input:text._date", function () {
            var $this = $(this);
            if (!$this.data("bindMaskInput")) {
                $this.mask("99/99/9999", {
                    completed: function () {
                        var rawInput = this.val();
                        if (!dateRegex.test(rawInput)) {
                            showMessage("hint", rawInput + "is not a valid date.");
                        }
                    }
                });
            }
        }).on("focus", "input:text._num_2", function () {
            var $this = $(this);
            if (!$this.data("bindMaskInput")) {
                $this.attr("maxlength", "2");
            }
        }).on("focus", "input:text._num_4", function () {
            var $this = $(this);
            if (!$this.data("bindMaskInput")) {
                $this.mask("9999");
            }
        }).on("focus", "input:text._num_5", function () {
            var $this = $(this);
            if (!$this.data("bindMaskInput")) {
                $this.mask("99999");
            }
        }).on("focus", "input:text._phone", function () {
            var $this = $(this);
            if (!$this.data("bindMaskInput")) {
                $this.attr("maxlength", "13").mask("(999)999-9999");
            }
        }).on("focus", "input:text._homephone", function () {
            var $this = $(this);
            if (!$this.data("bindMaskInput")) {
                $this.attr("maxlength", "13").mask("(999)999-9999");
            }
        }).on("focus", "input:text._money", function () {
            var $this = $(this).FormatNumber("_money");
        }).on("focus", "input:text._station1", function () {
            var $this = $(this).FormatNumber("_station1");
        }).on("focus", "input:text._station2", function () {
            var $this = $(this).FormatNumber("_station2");
        }).on("focus", "input:text._long", function () {
            var $this = $(this).FormatNumber("_long");
        }).on("focus", "input:text._numfloat1", function () {
            var $this = $(this).FormatNumber("_numfloat1");
        }).on("focus", "input:text._numfloat2", function () {
            var $this = $(this).FormatNumber("_numfloat2");
        }).on("focus", "input:text._numfloat3", function () {
            var $this = $(this).FormatNumber("_numfloat3");
        }).on("focus", "input:text._number", function () {
            var $this = $(this).FormatNumber("_number");
        }).on("blur", "input:text.timeout", function () {
            var $this = $(this);
            var value = $this.val();
            if (+value > 0) {
                if (value < 100) {
                    value = 100;
                } else {
                    value = value.toString().replace(/\d{2}$/i, "00");
                }
                $this.val(value).change();
            } else {
                $this.val('0').change();
            }
        });

        $("input:text._date").mask("99/99/9999").data("bindMaskInput", true);
        $("input:text._phone,input:text._input_phone").attr("maxlength", "13").mask("(999)999-9999").data("bindMaskInput", true)
            .bind("blur", function() {
                $(this).valid();
            });
        $("input:text._homephone").mask("(999)999-9999").attr("maxlength", "13").data("bindMaskInput", true)
            .bind("blur", function() {
                $(this).valid();
            });
        $("input:text._num_5").mask("99999").data("bindMaskInput", true)
            .bind("blur", function() {
                $(this).valid();
            });
        $("input:text._num_4").mask("9999").data("bindMaskInput", true);

        $("input:text._money").each(function (index, element) {
            jQuery(this).FormatNumber("_money", "formatValue");
        });
        $("input:text._station1").each(function (index, element) {
            jQuery(this).FormatNumber("_station1", "formatValue");
        });
        $("input:text._station2").each(function (index, element) {
            jQuery(this).FormatNumber("_station2", "formatValue");
        });
        $("input:text._long").each(function (index, element) {
            jQuery(this).FormatNumber("_long", "formatValue");
        });
        $("input:text._number").each(function (index, element) {
            jQuery(this).FormatNumber("_number", "formatValue");
        });
        $("input:text._numfloat1").each(function (index, element) {
            jQuery(this).FormatNumber("_numfloat1", "formatValue");
        });
        $("input:text._numfloat2").each(function (index, element) {
            jQuery(this).FormatNumber("_numfloat2", "formatValue");
        });
        $("input:text._numfloat3").each(function (index, element) {
            jQuery(this).FormatNumber("_numfloat3", "formatValue");
        });
        $("input:text._num_2").each(function (index, element) {
            jQuery(this).FormatNumber("_num_2", "formatValue");
        });
        $("input:text._num_2").bind("blur", function() {
            $(this).valid();
        });
    });

})(jQuery);