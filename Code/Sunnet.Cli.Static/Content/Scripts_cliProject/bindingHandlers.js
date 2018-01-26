ko.bindingHandlers.fadeVisible = {
    init: function (element, valueAccessor) {
        // Start visible/invisible according to initial value
        //var shouldDisplay = valueAccessor();
        //$(element).toggle(shouldDisplay);
    },
    update: function (element, valueAccessor) {
        // On update, fade in/out
        var shouldDisplay = valueAccessor();
        shouldDisplay ? $(element).fadeIn(2000) : $(element).hide();
    }
};
ko.bindingHandlers.waitting = {
    init: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var isDisplaying = valueAccessor()();
        isDisplaying === true && $(element).hide();
        isDisplaying === false && $(element).show();
    },
    update: function (element, valueAccessor, viewModel) {
        var isDisplaying = valueAccessor()();
        isDisplaying === true && $(element).hide();
        isDisplaying === false && $(element).show();
    }
};
ko.bindingHandlers.popover = {
    init: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        $ele.popover();
    }
}

ko.bindingHandlers.boolText = {
    init: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var isTrue = valueAccessor();
        var text = "";
        if (isTrue === true || isTrue == "true" || isTrue == "True") {
            text = "Yes";
        } else if (isTrue === false || isTrue == "false" || isTrue == "False") {
            text = "No";
        }
        else if (isTrue) {
            text = "Yes";
        }
        $ele.text(text);
    }
}

ko.bindingHandlers.mark = {
    init: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var isTrue = valueAccessor();
        var text = isTrue ? '<i class="icon-ok"></i>' : "";
        $ele.html(text);
    }
}

ko.bindingHandlers.upload = {
    init: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var isTrue = valueAccessor();
        if (isTrue) {
            var options = $ele.data();
            if (options["filesinglesizelimit"])
                options["fileSingleSizeLimit"] = options["filesinglesizelimit"];
            var propertyName = $ele.attr("name");
            var uploader = SunnetWebUploader.CreateWebUploader(options);
            if (viewModel.uploader) {
                viewModel.uploader(propertyName + "Uploader", uploader);
                if (options["filedequeued"]) {
                    uploader.on('fileDequeued', function () {
                        uploader.enable();
                        var func = eval(options["filedequeued"]);
                        new func(viewModel);
                    });
                }
                if (options["uploadsuccess"]) {
                    uploader.on('uploadSuccess', function (file, result) {
                        var func = eval(options["uploadsuccess"]);
                        if (result) {
                            if (result.success) {
                                func(viewModel, getToFolder() + "/" + getUploaderPrefix() + result.file, $ele.attr("id"));
                                uploader.disable();
                            }
                        } else if (file) {
                            func(viewModel, file.dbName, $ele.attr("id"));
                            uploader.disable();
                        }
                    });
                }
            }
        }
    }
}

ko.bindingHandlers.number = {
    init: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var isTrue = valueAccessor();
        if (isTrue) {
            setTimeout(function () { $ele.rules("add", { number: true }); }, 0);
        }
    }
}
ko.bindingHandlers.required = {
    init: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var $form = $ele.closest("form");
        var isTrue = valueAccessor();
        if (isTrue) {
            $ele.rules("add", { required: true });
        } else {
            $ele.rules("remove", "required");
        }
    },
    update: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var $form = $ele.closest("form");
        var isTrue = valueAccessor();
        if (isTrue) {
            $ele.rules("add", { required: true });
        } else {
            $ele.rules("remove", "required");
        }
    }
}

ko.bindingHandlers.tooltip = {
    init: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var title = valueAccessor();
        var options = $.extend({}, $ele.data(), { title: title });
        if (allBinding.trigger) {
            options.trigger = allBinding.trigger;
        }
        $ele.tooltip(options);
    }, update: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var title = valueAccessor();
        var options = $.extend({}, $ele.data(), { title: title });
        $ele.tooltip(options);
    }
}


ko.bindingHandlers.cpalls_item = {
    init: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        //console.log("init item,", viewModel.label);
        var $ele = jQuery(element);
        var isShowing = valueAccessor();
        if (!isShowing)
            $ele.hide();
    },
    update: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var isShowing = valueAccessor();
        var item = viewModel;
        if (isShowing) {
            if (!bindingContext.$index || bindingContext.$index() == bindingContext.$root.currentItemIndex()) {
                $ele.show();

                if (item.type == Ade_ItemType.TxkeaReceptive.value) {
                    //顺序 ：1.Instruction Audio 2.Image1 3.Audio1 4.Image2 5.Audio2 
                    if (item.instructionAudio && item.instructionAudio.file) {
                        if (item.instructionAudio.player) {
                            //播放完成后开始显示和播放answer的图片和音频
                            item.instructionAudio.player.onended = function () {
                                if (item.isShowing()) {
                                    item.unEndAudioCount(item.unEndAudioCount() - 1);
                                    item.showItem && item.showItem();
                                }
                            }
                            execHelper && execHelper.overrideAudioSrc(item.instructionAudio);
                            item.instructionAudio.player.play();
                        };
                    }
                    else {
                        item.showItem && item.showItem();
                    }
                }
                else if (item.type == Ade_ItemType.TxkeaExpressive.value) {
                    //顺序 ：1.Image1 2.Audio1 3.Image2 4.Audio2 5.Instruction Audio
                    item.showItem && item.showItem();
                }
                else {
                    if (item.target) {
                        if (item.target.text) {
                            $ele.find(".target").html(item.target.text).hide();

                            if (+(item.target.textTimeout) > 0) {
                                item.target.textTimer = new TimeoutTimer(+(item.target.textTimeout));
                                $.when(item.target.textTimer.timeUp()).done(function () {
                                    $ele.find(".target").show();
                                });
                                item.target.textTimer.start();
                            } else {
                                $ele.find(".target").show();
                            }
                        } else {
                            $ele.find(".target").closest("div").hide();
                        }

                        if (item.target.audio) {
                            if (+(item.target.audioTimeout) > 0) {
                                item.target.audioTimer = new TimeoutTimer(+(item.target.audioTimeout));
                                $.when(item.target.audioTimer.timeUp()).done(function () {
                                    execHelper && execHelper.overrideAudioSrc(item.target);
                                    item.target.player && item.target.player.play();
                                });
                                item.target.audioTimer.start();
                            } else {
                                execHelper && execHelper.overrideAudioSrc(item.target);
                                item.target.player && item.target.player.play();
                            }
                        }

                    }

                    if (item.prompt && item.prompt.picture && item.prompt.picture.file) {
                        var $promptImg = $ele.find(".prompt.img").empty().append(item.prompt.picture.html);
                        $promptImg.find("img.img").hide();
                        $promptImg.append(execHelper.waitingHtml).find("img.img").hide();
                        if (+(item.prompt.picture.timeout) > 0) {
                            item.prompt.picture.timer = new TimeoutTimer(+(item.prompt.picture.timeout));
                            $.when(item.prompt.picture.timer.timeUp()).done(function () {
                                $ele.find(".prompt.img").find("img.img").show().css("style", "block");
                                $ele.find(".prompt.img").find("img.time").hide();
                            });
                            item.prompt.picture.timer.start();
                        } else {
                            $ele.find(".prompt.img").find("img.time").hide();
                            $ele.find(".prompt.img").find("img.img").show().css("style", "block");
                        }
                    }

                    if (item.prompt && item.prompt.text && item.prompt.text.text) {
                        $ele.find(".prompt.text").html(item.prompt.text.text).hide();
                        if (+(item.prompt.text.timeout) > 0) {
                            item.prompt.text.timer = new TimeoutTimer(+(item.prompt.text.timeout));
                            $.when(item.prompt.text.timer.timeUp()).done(function () {
                                $ele.find(".prompt.text").show();
                            });
                            item.prompt.text.timer.start();
                        } else {
                            $ele.find(".prompt.text").show();
                        }
                    }

                    if (item.prompt && item.prompt.audio) {
                        if (+(item.prompt.audio.timeout) > 0) {
                            item.prompt.audio.timer = new TimeoutTimer(+(item.prompt.audio.timeout));
                            $.when(item.prompt.audio.timer.timeUp()).done(function () {
                                execHelper && execHelper.overrideAudioSrc(item.prompt);
                                item.prompt.audio.player && item.prompt.audio.player.play();
                            });
                            item.prompt.audio.timer.start();
                        } else {
                            execHelper && execHelper.overrideAudioSrc(item.prompt);
                            item.prompt.audio && item.prompt.audio.player && item.prompt.audio.player.play();
                        }
                    }

                    if (item.answers && item.answers.length) {
                        if (item.type == Ade_ItemType.Pa.value) {
                            $.each(item.answers, function (i, answer) {
                                $ele.find(".answer").find("li:not(.blank)").eq(i).hide();
                                if (answer.text.timeout) {
                                    answer.text.timer = new TimeoutTimer(+(answer.text.timeout));
                                    $.when(answer.text.timer.timeUp()).done(function () {
                                        $ele.find(".answer").find("li:not(.blank)").eq(i).show();
                                    });
                                    answer.text.timer.start();
                                } else {
                                    $ele.find(".answer").find("li:not(.blank)").eq(i).show();
                                }
                            });
                        } else {
                            $.each(item.answers, function (i, answer) {
                                +(function (_index, _answer) {
                                    if (_answer.audio.file && _answer.audio.player) {
                                        if (+(_answer.audio.timeout) > 0) {
                                            _answer.audio.timer = new TimeoutTimer(+(_answer.audio.timeout));
                                            $.when(_answer.audio.timer.timeUp()).done(function () {
                                                execHelper && execHelper.overrideAudioSrc(_answer.audio);
                                                _answer.audio.player.play();
                                            });
                                            _answer.audio.timer.start();
                                        } else {
                                            execHelper && execHelper.overrideAudioSrc(_answer.audio);
                                            _answer.audio.player.play();
                                        }
                                    }
                                })(i, answer);
                            });
                        }
                    } //end if (item.answers && item.answers.length)
                }
            }
            //console.log("update", element, valueAccessor(), allBinding(), viewModel, bindingContext);
        } else {
            $ele.hide();

            item.target && item.target.textTimer && item.target.textTimer.cancel();
            bindingContext.$root.nextTimer && bindingContext.$root.nextTimer.cancel();

            if (item.target && item.target.player) {
                item.target.player.pause();
                item.target.player.currentTime = 0;
                //item.target.player.fastSeek(0);
            }
            item.target && item.target.audioTimer && item.target.audioTimer.cancel();

            item.prompt && item.prompt.picture && item.prompt.picture.timer && item.prompt.picture.timer.cancel();
            item.prompt && item.prompt.audio && item.prompt.audio.timer && item.prompt.audio.timer.cancel();
            item.prompt && item.prompt.audio && item.prompt.audio.player && item.prompt.audio.player.pause();
            item.prompt && item.prompt.text && item.prompt.text.timer && item.prompt.text.timer.cancel();

            if (item.answers && item.answers.length) {
                for (var i = 0; i < item.answers.length; i++) {
                    var answer = item.answers[i];
                    answer.audio.timer && answer.audio.timer.cancel();
                    if (answer.audio.player) {
                        answer.audio.player.pause();
                        answer.audio.player.currentTime = 0;
                    }

                    answer.picture.timer && answer.picture.timer.cancel();
                    answer.text.timer && answer.text.timer.cancel();
                }
            }
        }
    }
}
ko.bindingHandlers.showIn = {
    init: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = $(element);
        var time = valueAccessor();
        if (time > 0) {
            setTimeout(function () {
                $ele.show();
            }, time);
        }
    }
};
ko.bindingHandlers.hideIn = {
    init: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = $(element);
        var time = valueAccessor();
        if (time > 0) {
            setTimeout(function () {
                $ele.hide();
            }, time);
        }
    }
};

ko.bindingHandlers.title = {
    init: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var length = $ele.data("title-length");
        var html = valueAccessor();
        if (html.length > length) {
            $ele.html(html.slice(0, length) + "...").attr("title", html);
        } else {
            $ele.html(html);
        }
    }, update: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var binddings = allBinding();
        var $ele = jQuery(element);
        var length = $ele.data("title-length") || binddings.attr['title-length'];
        var html = valueAccessor();
        if (html.length > length) {
            $ele.html(html.slice(0, length) + "...").attr("title", html);
        } else {
            $ele.html(html);
        }
    }
}

ko.bindingHandlers.datetime = {
    init: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var value = $.isFunction(valueAccessor) ? valueAccessor() : valueAccessor;
        value = $.isFunction(value) ? value() : value;
        var invalidDate = ['01/01/0001', '1/1/0001', '01/01/1753', '1/1/1753'];
        if (invalidDate.indexOf(value) >= 0) value = "";
        if ($ele.is(":input")) {
            $ele.val(value);
        } else {
            $ele.text(value);
        }
    },
    update: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var value = $.isFunction(valueAccessor) ? valueAccessor() : valueAccessor;
        value = $.isFunction(value) ? value() : value;
        var invalidDate = ['01/01/0001', '1/1/0001', '01/01/1753', '1/1/1753'];
        if (invalidDate.indexOf(value) >= 0) value = "";
        if ($ele.is(":input")) {
            $ele.val(value);
        } else {
            $ele.text(value);
        }
    }
};

//Sam begin 2015-06-18
// Popup warning if date entered is less than 2 years before the current date or if 12 years or more before the current date
ko.bindingHandlers.minDate = {
    init: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var $form = $ele.closest("form");
        var minDate = valueAccessor();
        if (minDate) {
            $ele.rules("add", { minDate: minDate });
        } else {
            $ele.rules("remove", "required");
        }
    },
    update: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var $form = $ele.closest("form");
        var minDate = valueAccessor();
        if (minDate) {
            $ele.rules("add", { minDate: minDate });
        } else {
            $ele.rules("remove", "required");
        }
    }
};
ko.bindingHandlers.maxDate = {
    init: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var $form = $ele.closest("form");
        var maxDate = valueAccessor();
        if (maxDate) {
            $ele.rules("add", { maxDate: maxDate });
        } else {
            $ele.rules("remove", "required");
        }
    },
    update: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var $form = $ele.closest("form");
        var maxDate = valueAccessor();
        if (maxDate) {
            $ele.rules("add", { maxDate: maxDate });
        } else {
            $ele.rules("remove", "required");
        }
    }
};
//Sam end 

//joe begin 2015-06-27
// format js date like 2015-06-26T11:04:44.446Z
ko.bindingHandlers.jsdatetime = {
    init: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var value = $.isFunction(valueAccessor) ? valueAccessor() : valueAccessor;
        value = $.isFunction(value) ? value() : value
        value = new Date(value);
        var year = value.getFullYear();
        var month = value.getMonth() + 1;
        var date = value.getDate();
        if (month < 10) {
            month = "0" + month;
        }
        if (date < 10) {
            date = "0" + date;
        }
        value = month + "/" + date + "/" + year;

        var invalidDate = ['01/01/0001', '1/1/0001', '01/01/1753', '1/1/1753'];
        if (invalidDate.indexOf(value) >= 0) value = "";
        if ($ele.is(":input")) {
            $ele.val(value);
        } else {
            $ele.text(value);
        }
    },
    update: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var value = $.isFunction(valueAccessor) ? valueAccessor() : valueAccessor;
        value = $.isFunction(value) ? value() : value;
        value = new Date(value);
        var year = value.getFullYear();
        var month = value.getMonth() + 1;
        var date = value.getDate();
        if (month < 10) {
            month = "0" + month;
        }
        if (date < 10) {
            date = "0" + date;
        }
        value = month + "/" + date + "/" + year;

        var invalidDate = ['01/01/0001', '1/1/0001', '01/01/1753', '1/1/1753'];
        if (invalidDate.indexOf(value) >= 0) value = "";
        if ($ele.is(":input")) {
            $ele.val(value);
        } else {
            $ele.text(value);
        }
    }
};
//joe end

!(function () {
    function setValue(element, valueAccessor, allBinding, viewModel, bindingContext) {
        var $ele = jQuery(element);
        var value = $.isFunction(valueAccessor) ? valueAccessor() : valueAccessor;
        value = $.isFunction(value) ? value() : value;
        var options = $.extend({
            prop: 'Name',
            separator: ', '
        }, allBinding());
        var result = "";
        if (typeof value === "object" && value.length) {
            for (var i = 0; i < value.length; i++) {
                var isLast = i === value.length - 1;
                var obj = value[i];
                var propValue = typeof obj === "string" ? obj : obj[options.prop];
                result += propValue;
                if (!isLast) {
                    result += options.separator;
                }
            }
        }
        $ele.html(result);
    }

    ko.bindingHandlers.each = {
        init: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
            setValue(element, valueAccessor, allBinding, viewModel, bindingContext);
        },
        update: function (element, valueAccessor, allBinding, viewModel, bindingContext) {
            setValue(element, valueAccessor, allBinding, viewModel, bindingContext);
        }
    }
})();
