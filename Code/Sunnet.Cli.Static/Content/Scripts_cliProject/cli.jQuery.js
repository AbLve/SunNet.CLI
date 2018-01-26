CKEDITOR_BASEPATH = window._staticDomain_ + 'Content/lib/ckeditor/';

function TemplateEngine(html, options) {
    var re = /<%([^%>]+)?%>/g, reExp = /(^( )?(if|for|else|switch|case|break|{|}))(.*)?/g, code = 'var r=[];\n', cursor = 0;
    var add = function (line, js) {
        js ? (code += line.match(reExp) ? line + '\n' : 'r.push(' + line + ');\n') :
            (code += line != '' ? 'r.push("' + line.replace(/"/g, '\\"') + '");\n' : '');
        return add;
    }
    var match;
    while (match = re.exec(html)) {
        add(html.slice(cursor, match.index))(match[1], true);
        cursor = match.index + match[0].length;
    }
    add(html.substr(cursor, html.length - cursor));
    code += 'return r.join("");';
    return new Function(code.replace(/[\r\t\n]/g, '')).apply(options);
}

var echartsTheme = echartsTheme || {};
echartsTheme.default = echartsTheme.infograpic;

jQuery(function () {
    // 与jQuery相关的页面加载完成事件

    $.ajaxSetup({
        cache: false,
        statusCode:
        {
            403: function () {
                if ("_TimeoutUrl_" in window) {
                    location.href = window._TimeoutUrl_;
                } else {
                    showMessage("fail", "403");
                }
            }
        }
    });
    $(document).ajaxError(function (event, xhrObj, ajaxOptions, statusText) {
        if (xhrObj.readyState >= 2) {
            if (xhrObj.statusCode != "403") {
                var message = statusText;
                var errorHtml = jQuery(xhrObj.responseText);
                var debugMsg = statusText + "<br>";
                if (errorHtml.length >= 12) {
                    debugMsg += isNull("innerText", errorHtml[1], "") + "<hr>";
                    debugMsg += isNull("innerText", errorHtml[7], "") + "<hr>";
                    debugMsg += isNull("nodeValue", errorHtml[11], "");
                }
                showMessage("debug", message, debugMsg);
            }
        }
    });

    var $body = $("body");

    +(function () {
        var menuStatusKey = "menuShowing";
        var initialShow = JSON.parse(localStorage.getItem(menuStatusKey));

        var $menuToggler = $("#toggle-menu");
        if ($menuToggler.length) {
            var width = $menuToggler.width();
            if (initialShow === null) {
                localStorage.setItem(menuStatusKey, true);
                initialShow = true;
            }

            if (initialShow) {
                $($menuToggler.data("target")).removeClass("hidden");
                //main-con-right-full-screen
                $($menuToggler.data("main")).removeClass("main-con-right-full-screen");
            }
            $menuToggler.on("click", function () {
                var currentStatus = JSON.parse(localStorage.getItem(menuStatusKey));
                if (currentStatus) {
                    $($menuToggler.data("target")).addClass("hidden");
                    $($menuToggler.data("main")).addClass("main-con-right-full-screen");
                    localStorage.setItem(menuStatusKey, false);
                } else {
                    $($menuToggler.data("target")).removeClass("hidden");
                    $($menuToggler.data("main")).removeClass("main-con-right-full-screen");
                    localStorage.setItem(menuStatusKey, true);
                }
                $(window).trigger("resize");
                return false;
            });
        }
    })();

    $body.on("loaded.bs.modal", ".modal", function () {
        setTimeout(function () {
            $(this).find("[data-toggle='popover']").popover({ container: "body" });
            $(this).find("[data-toggle='tooltip']").tooltip({ container: "body" });
            $.validator.unobtrusive.parse(document);
        }, 200);

        window.loading(false);
    });

    $body.on("loading.bs.modal", ".modal", function () {
        window.loading();
    });

    $body.on("hidden.bs.modal", ".modal", function () {
        var $this = $(this);
        var modalObj = $this.data("bs.modal");
        if (modalObj && modalObj.options && modalObj.options.cache == false) {
            var $form = $this.find("form[action][id]");
            if ($form.length) {
                var formId = $form.attr("id");
                unRegisterFormCallbacks(formId);
            }
        }
    });

    $body.on("loadError.bs.modal", ".modal", function (event) {
        var title = event.xhrObj.statusText;
        var errorHtml = $(event.xhrObj.responseText);
        title += ": " + isNull("innerText", errorHtml[1], "") + "<br>";
        var message = title;
        message += isNull("innerText", errorHtml[7], "") + "<hr>";
        message += isNull("nodeValue", errorHtml[11], "");
        showMessage("debug", title, message);

        $(this).modal("hide");
    });

    $body.on("confirm.bs.modal", ".modal", function (e) {
        var $this = $(this);
        var confirmMessage = window.getErrorMessage("confirmToLeave_formChanged");
        $.when(waitingConfirm(confirmMessage, "Close", "Cancel")).done(function () {
            $this.modal("hideAnyway");
        });
        return false;
    });

    /*datetime box****************************************************************************************/
    $body.on("click", "input:text.date", function () {
        var $this = jQuery(this);
        var options = { el: $this.attr("id"), dateFmt: "MM/dd/yyyy", maxDate: "2099-12-31 23:59:59" };
        if ($this.hasClass("time")) {
            options.dateFmt = "MM/dd/yyyy HH:mm:ss";
        }
        if ($this.hasClass("timehm")) {
            options.dateFmt = "MM/dd/yyyy HH:mm";
        }
        if ($this.attr("format")) {
            options.dateFmt = $this.attr("format");
        }
        if ($this.attr("maxDate") || $this.attr("maxdate")) {
            options.maxDate = $this.attr("maxDate") || $this.attr("maxdate")
        }
        if ($this.data("mindate") || $this.data("minDate")) {
            options.minDate = $this.data("mindate") || $this.data("minDate");
        }
        options.onpicked = function (dp) {
            setTimeout(function () { $(dp.el).change(); }, 100);
        };
        options.oncleared = function (dp) {
            setTimeout(function () { $(dp.el).change(); }, 100);
        };

        WdatePicker(options);
    });
    $body.on("change", "input:text.date", function () {
        // alert(1);
    });



    $body.find("form[action]:visible").each(function () {
        var $form = $(this);
        setTimeout(function () { $form.data("_source", $form.serialize()); }, 2000);
    });

    $body.on("click", "input:submit,button:submit,input.submit,button.submit", function () {
        updateCkeditor();
        window._submit = true;
    }).on("submit", function () {
        var $form = $(this).closest("form");
        if ($form.length && (!$form.attr("target") || $form.attr("target") != "_blank")) {
            window.loading();
            window._submit = true;
        }
    });

    $body.find("form[action]").find("input,textarea").placeholder();

    $body.find("[data-toggle='popover']").popover({ container: "body" });

    $body.find("[data-toggle='tooltip']").tooltip({ container: "body" });

    $body.on("keypress", "#_form", function (event) {
        var $inputedControl = $(event.target);
        var $form = $inputedControl.closest("form");
        if (event.which == 13) {
            if ($form.attr("action")) {
                $form.find("button:submit,button.submit-btn,input.submit-btn").filter(":visible:first").focus().click();
            } else {
                $form.find("button.search-btn,input.sreach-btn").filter(":visible:first").focus().click();
            }
        }
    });

    $body.on("submit", "form[action]", function () {
        var $form = $(this);
        if (!$form.attr("target") || $form.attr("target") != "_blank") {
            window.loading();
            window._submit = true;
            $form.find("button:submit,input:submit,button.submit-btn,input.submit-btn").button("loading");
        }
    });

    window.onbeforeunload = function (event) {
        event = event || window.event;
        if (!("_submit" in window) && window.formChanged()) {
            var message = window.getErrorMessage("confirmToRedirect_formChanged");
            return message;
        }
    };

    $body.on("click", ".comingsoon", function () {
        window.showMessage("success", "Coming soon.");
        return false;
    });

});

// ReSharper disable once WrongExpressionStatement
// define common functions for use
+function ($) {
    'use strict';
    if ("_DEBUG" in window && window._DEBUG == true) {
        window._message_["debug"] = {
            title: "Debug",
            message: "Source code error.",
            className: "danger",
            iconClass: "icon-remove"
        }
    }
    var messageHelper = (function () {
        var containerId = "#messageContainer";
        return {
            getContainer: function () {
                var $messageContainer = jQuery(containerId);
                if (!$messageContainer.length) {
                    $messageContainer = jQuery("<div id='messageContainer' class='message-Container'></div>")
                        .appendTo("body");
                }
                $messageContainer.children().each(function () {
                    //$(this).alert("close");
                    $(this).hide();
                });
                return $messageContainer;
            }
        }
    })();

    function decodeUrl(encodedUrl) {
        if (/\/|\&|\?/.test(encodedUrl) || encodedUrl.indexOf("%") < 0) {
            return encodedUrl;
        }
        return decodeUrl(decodeURIComponent(encodedUrl));
    }

    var _urlParams_ = {};
    (window.onpopstate = function () {
        // process url query params to 
        var match,
            pl = /\+/g,
            search = /([^&=]+)=?([^&]*)/g,
            decode = function (s) {
                return decodeURIComponent(s.replace(pl, " "));
            },
            query = window.location.search.substring(1);
        while (match = search.exec(query)) {
            try {
                var key = decode(match[1]).toLowerCase();
                var value = decode(match[2]);
                if (value.toString().toLowerCase() == "true") {
                    _urlParams_[key] = true;
                } else if (value.toString().toLowerCase() == "false") {
                    _urlParams_[key] = false;
                } else {
                    _urlParams_[key] = value;
                }
            } catch (e) {
            }
        }

    })();

    /* ========================================================================
     * showMessage : 显示提示消息
     * name: success | fail | debug , 前两项正常使用, 分别表示成功或者失败的消息
     * debug 仅用于开发时候显示debugMessage
     * 调用debug的方法在发布时调用fail, 显示 messageContent
     * 如果messageContent是一个jsMsgs中的一个KEY, 则直接显示消息内容  showMessageForRoster
     * callback: 
     * ======================================================================== */

    window.showMessage = function (name, messageContent, debugMessage) {
        var deffered = $.Deferred();
        var message = {};
        if (messageContent in window._message_) {
            messageContent = window._message_[messageContent];
        }
        if (name == "debug" && !window._message_["debug"])
            name = "fail";
        if (name == "debug" && window._message_["debug"]) {
            messageContent = debugMessage;
        }

        if (window._message_[name]) {
            jQuery.extend(message, window._message_[name]);
        }
        message.error = "<br/>" + (messageContent || "");
        var htmlTemp = "<div class='alert alert-<%this.className%> alert-dismissable fade in'  role='alert'>" +
            "<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button>" +
            "<i class='<% this.iconClass %> icon-2x'></i>" +
            "<p><% this.message %>&nbsp;<% this.error %></p>" +
            "<p> <button type='button' class='mainbutton _ok btn-<%this.className%>'>OK</button></p>" +
            "</div>";
        var html = TemplateEngine(htmlTemp, message);
        var $messager = jQuery(html).attr("id", "message_" + name).appendTo(messageHelper.getContainer());
        $messager.on("click", "._ok", function () {
            $messager.alert("close");
        });
        if (name == "success") {
            setTimeout(function () {
                $messager.alert("close");
                deffered.resolve();
            }, 2000);
        }
        return deffered.promise();
    };

    window.showMessageForRoster = function (name, messageContent, debugMessage) {
        var deffered = $.Deferred();
        var message = {};
        if (messageContent in window._message_) {
            messageContent = window._message_[messageContent];
        }
        if (name == "debug" && !window._message_["debug"])
            name = "fail";
        if (name == "debug" && window._message_["debug"]) {
            messageContent = debugMessage;
        }

        if (window._message_[name]) {
            jQuery.extend(message, window._message_[name]);
        }
        message.error = "<br/>" + (messageContent || "");
        var htmlTemp = "<div class='alert alert-<%this.className%> alert-dismissable fade in'  role='alert'>" +
            "<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button>" +
            "<i class='<% this.iconClass %> icon-2x'></i>" +
            "<p><% this.error %></p>" +
            "<p> <button type='button' class='mainbutton _ok btn-<%this.className%>'>OK</button></p>" +
            "</div>";
        var html = TemplateEngine(htmlTemp, message);
        var $messager = jQuery(html).attr("id", "message_" + name).appendTo(messageHelper.getContainer());
        $messager.on("click", "._ok", function () {
            $messager.alert("close");
        });
        if (name == "success") {
            setTimeout(function () {
                $messager.alert("close");
                deffered.resolve();
            }, 2000);
        }
        return deffered.promise();
    };
    window.waiting = function (millSeconds) {
        var deffered = $.Deferred();
        setTimeout(function () {
            deffered.resolve();
        }, millSeconds);
        return deffered.promise();
    };

    /* ========================================================================
     * waitingAlert : 显示提示消息,并提供回调方法
     * name: success | fail | debug , 前两项正常使用, 分别表示成功或者失败的消息
     * debug 仅用于开发时候显示debugMessage
     * 调用debug的方法在发布时调用fail, 显示 messageContent
     * 如果messageContent是一个jsMsgs中的一个KEY, 则直接显示消息内容
     * ======================================================================== */
    window.waitingAlert = function (name, messageContent, debugMessage) {
        var deffered = $.Deferred();

        var message = {};
        if (messageContent in window._message_) {
            messageContent = window._message_[messageContent];
        }
        if (name == "debug" && !window._message_["debug"])
            name = "fail";
        if (name == "debug" && window._message_["debug"]) {
            messageContent = debugMessage;
        }

        if (window._message_[name]) {
            jQuery.extend(message, window._message_[name]);
        }
        message.error = "<br/>" + (messageContent || "");
        var htmlTemp = "<div class='alert alert-<%this.className%> alert-dismissable fade in'  role='alert'>" +
            "<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;</button>" +
            "<i class='<% this.iconClass %> icon-2x'></i>" +
            "<p><% this.message %>&nbsp;<% this.error %></p>" +
            '<p> <button type="button" class="mainbutton _ok btn-<%this.className%>">OK</button>      </p>' +
            "</div>";
        var html = TemplateEngine(htmlTemp, message);
        var $messager = jQuery(html).attr("id", "message_" + name);
        $messager.on("click", "._ok", function () {
            $messager.alert("close");
            deffered.resolve();
        }).appendTo(messageHelper.getContainer());
        return deffered.promise();
    }

    /* ========================================================================
     * waitingConfirm : 显示一个确认框, 并返回一个等待用户选择的Promise
     * message: 确认框消息内容
     * yesText: 确认按钮显示文本
     * noText: 取消按钮显示文本
     * resolveObject: 完成时的回调函数参数
     * rejectObject: 操作失败的回调函数参数
     * ======================================================================== */
    window.waitingConfirm = function (message, yesText, noText, resolveObject, rejectObject) {
        if (message in window._message_) {
            message = window._message_[message];
        }
        if (!resolveObject) {
            resolveObject = true;
        }
        if (!rejectObject) {
            rejectObject = false;
        }
        var deffered = $.Deferred();
        var htmlTemp = '<div class="alert alert-warning alert-dismissible fade in" role="alert">' +
            "<i class='icon-warning-sign icon-3x'></i>" +
            '<p><% this.message %></p><p>' +
            '<button type="button" class="mainbutton _warning btn-danger"><% this.yesText %></button>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;' +
            '<button type="button" class="mainbutton _cancel cancel-btn"><% this.noText %></button>' +
            '</p></div>';
        var html = TemplateEngine(htmlTemp, { message: message, yesText: yesText, noText: noText });
        var $confirmer = jQuery(html);
        $confirmer.on("click", "._warning", function () {
            $confirmer.alert("close");
            deffered.resolve(resolveObject);
        }).on("click", "._cancel", function () {
            $confirmer.alert("close");
            deffered.reject(rejectObject);
        }).appendTo(messageHelper.getContainer());
        return deffered.promise();
    };

    /* ========================================================================
     * onFormSubmitFailure : 表单提交失败时的回调函数仅用于 AjaxOption 的 OnFailure属性
     * ======================================================================== */
    window.onFormSubmitFailure = function (xhr, statusText, errorMessage) {
        delete "_submit";
        window.loading(false);

        var form = this;
        var $form = $(this);
        var formId = $form.attr("id");

        $form.find("button:submit,button.submit-btn,input.submit-btn").button("reset");

        var title = errorMessage + "<br>";
        var message = "";
        var errorHtml = jQuery(xhr.responseText);
        if (errorHtml.length >= 12) {
            message += errorHtml[1].innerText + "<hr>";
            message += errorHtml[7].innerText + "<hr>";
            message += errorHtml[11].nodeValue;
        } else {
            message += errorMessage;
        }
        showMessage("debug", title, message);
    }
    window.onFormSubmitFailureForRoster = function (xhr, statusText, errorMessage) {
        delete "_submit";
        window.loading(false);

        var form = this;
        var $form = $(this);
        var formId = $form.attr("id");

        $form.find("button:submit,button.submit-btn,input.submit-btn").button("reset");

        var title = errorMessage + "<br>";
        var message = "";
        var errorHtml = jQuery(xhr.responseText);
        if (errorHtml.length >= 12) {
            message += errorHtml[1].innerText + "<hr>";
            message += errorHtml[7].innerText + "<hr>";
            message += errorHtml[11].nodeValue;
        } else {
            message += errorMessage;
        }
        showMessageForRoster("debug", title, message);
    }
    /* ========================================================================
     * OnFormSubmitSuccess : 表单提交成功时的回调函数仅用于 AjaxOption 的 OnSuccess属性
     * ======================================================================== */
    window.OnFormSubmitSuccess = function (responseText, statusText, xhr) {
        delete "_submit";
        window.loading(false);
        var response = responseText;
        if (typeof response == "string")
            response = jQuery.parseJSON(responseText);

        var form = this;
        var $form = $(this);
        var formId = $form.attr("id");

        $form.find("button:submit,button.submit-btn,input.submit-btn").button("reset");

        if (response.success) {

            var formOnResponsed = window.getFormEvent(formId, "onResponsed");
            if (jQuery.isFunction(formOnResponsed)) {
                formOnResponsed.apply(form, [response]);
                return;
            }

            showMessage("success");
            $form.data("_source", $form.serialize());
            var formOnPosted = window.getFormEvent(formId, "onPosted");
            $.when(window.waiting(1000)).done(function () {
                if (jQuery.isFunction(formOnPosted)) {
                    formOnPosted.apply(form, [response]);
                } else {
                    closeModal($form);
                }
            });

        } else {
            var html = response.msg || "";
            var htmlTemp = "<%for ( var i = 0; i < this.length ; i++) {%> <% this[i].ErrorMessage %><br><%}%>";
            for (var field in response.modelState) {
                var fieldState = response.modelState[field];
                if (fieldState.Errors && fieldState.Errors.length) {
                    html += "<br>" + TemplateEngine(htmlTemp, fieldState.Errors);
                }
            }
            var debugInfo = html;
            if (response.modelState && response.modelState["Exception"]) {
                var exception = response.modelState["Exception"];
                var error = exception.Errors && exception.Errors[0].Exception;
                debugInfo = error.Message + "<hr>"
                    + error.ExceptionMethod + "<hr>"
                    + error.StackTraceString;
            }
            showMessage("fail", html, debugInfo);
        }
    };
    window.OnFormSubmitSuccessForRoster = function (responseText, statusText, xhr) {
        delete "_submit";
        window.loading(false);
        var response = responseText;
        if (typeof response == "string")
            response = jQuery.parseJSON(responseText);

        var form = this;
        var $form = $(this);
        var formId = $form.attr("id");

        $form.find("button:submit,button.submit-btn,input.submit-btn").button("reset");

        if (response.success) {

            var formOnResponsed = window.getFormEvent(formId, "onResponsed");
            if (jQuery.isFunction(formOnResponsed)) {
                formOnResponsed.apply(form, [response]);
                return;
            }

            showMessage("success");
            $form.data("_source", $form.serialize());
            var formOnPosted = window.getFormEvent(formId, "onPosted");
            $.when(window.waiting(1000)).done(function () {
                if (jQuery.isFunction(formOnPosted)) {
                    formOnPosted.apply(form, [response]);
                } else {
                    closeModal($form);
                }
            });

        } else {
            var html = response.msg || "";
            var htmlTemp = "<%for ( var i = 0; i < this.length ; i++) {%> <% this[i].ErrorMessage %><br><%}%>";
            for (var field in response.modelState) {
                var fieldState = response.modelState[field];
                if (fieldState.Errors && fieldState.Errors.length) {
                    html += "<br>" + TemplateEngine(htmlTemp, fieldState.Errors);
                }
            }
            var debugInfo = html;
            if (response.modelState && response.modelState["Exception"]) {
                var exception = response.modelState["Exception"];
                var error = exception.Errors && exception.Errors[0].Exception;
                debugInfo = error.Message + "<hr>"
                    + error.ExceptionMethod + "<hr>"
                    + error.StackTraceString;
            }
            showMessageForRoster("fail", html, debugInfo);
        }
    };
    /* ========================================================================
     * registerFormCallbacks : 注册表单事件供
     * formId : 表单ID
     * events : 事件
     * ======================================================================== */
    window.registerFormCallbacks = function (formId, events) {
        if (!window._form_)
            window._form_ = {};
        if (!window._form_[formId])
            window._form_[formId] = {};
        var formJs = window._form_[formId];
        jQuery.extend(formJs, events);
    };

    /* ========================================================================
     * getFormEvent : 查找由registerFormCallbacks注册的表单事件
     * formId : 表单ID
     * event : 事件名
     * ======================================================================== */
    window.getFormEvent = function (formId, event) {
        if (!window._form_)
            window._form_ = {};
        if (!window._form_[formId])
            window._form_[formId] = {};
        var formJs = window._form_[formId];
        return formJs[event];
    }

    /* ========================================================================
    * unRegisterFormCallbacks : 取消表单事件
    * formId : 表单ID
    * event : 事件名, 如果不传第二个参数, 表示取消表单的所有事件, 即移除表单
    * ======================================================================== */
    window.unRegisterFormCallbacks = function (formId, event) {
        if (!window._form_) return;
        if (!window._form_[formId]) return;
        if (arguments.length == 1)
            delete window._form_[formId];
        else {
            delete window._form_[formId][event];
        }
    };

    /* ========================================================================
    * closeModal : 关闭Modal窗口
    * sender : 离该元素最近的Modal
    * ======================================================================== */
    window.closeModal = function (sender) {
        var $sender = $(sender);
        var $modal = $sender.closest("div.modal");
        if ($modal.length) {
            $modal.modal("hideAnyway");
        }
    };

    /* ========================================================================
    * QueryString : 获取url的查询参数值
    * key : 参数名
    * ======================================================================== */
    window.queryString = function (key, valueIfnoKey) {
        key = key.toLowerCase();
        if (key in _urlParams_)
            return _urlParams_[key];
        if (typeof valueIfnoKey == "undefined") {
            return "";
        }
        return valueIfnoKey;
    }
    window.AllQueryString = _urlParams_;

    /* ========================================================================
    * getErrorMessage : 获取错误消息
    * key : 错误消息的Key
    * ======================================================================== */
    window.getErrorMessage = function (key) {
        if (key in window._message_) {
            return window._message_[key];
        }
        return window._message_["message_notfound"];
    }
    /* ========================================================================
    * getMessage : 获取错误消息
    * key : 错误消息的Key
    * ======================================================================== */
    window.getMessage = function (key) {
        if (key in window._message_) {
            return window._message_[key];
        }
        return window._message_["message_notfound"];
    }

    window.redirectBack = function (urlIfNoReturnurl) {
        ///<summary>
        /// 返回到returnUrl传入的页面或者urlIfNoReturnurl或者首页
        ///</summary>
        ///<param name="urlIfNoReturnurl">如果URL中不存在returnUrl,则返回该页面,若不传该参数,则返回到首页</param>
        var url = window.queryString("returnurl");
        if (url.indexOf("%") >= 0 && /\/|\&|\?/.test(url) == false) {
            url = decodeURIComponent(url);
        }
        if (!url) {
            url = urlIfNoReturnurl;
        }
        if (!url) {
            url = "/";
        }
        location.href = url.replace(/\/\//ig, "/");
    }

    window.formChanged = function () {
        ///<summary>检查该页面表单是否有修改</summary>
        var changed = false;
        $("body").find("form[action]:visible").each(function () {
            var $form = $(this);
            if ($form.data("confirm") !== false) {
                var oldData = $form.data("_source");
                if (oldData && oldData != $form.serialize()) {
                    changed = true;
                    return false;
                }
            }
        });
        return changed;
    };

    window.updateCkeditor = function () {
        ///<summary>把表单里面的富文本框的值填充到元素里面</summary>
        if (("CKEDITOR" in window) && CKEDITOR.instances) {
            $.each(CKEDITOR.instances, function (instanceName, instance) {
                $("#" + instanceName).val(instance.getData()).change();
            });
        }
    }

    window.isNull = function (key, object, valueIfNotExists) {
        ///<summary>检查<paramref name="object"/>是否存在<paramref name="key"/>属性, 否则返回<paramref name="valueIfNotExists"/>
        ///</summary>
        ///<param name="key"></param>
        ///<param name="object"></param>
        ///<param name="valueIfNotExists"></param>
        if (!object) {
            return valueIfNotExists;
        }
        if (object && typeof (object) === "object") {
            if (key in object) {
                return object[key];
            }
        }
        return valueIfNotExists;
    }

    window.isTrueValue = function (key, object, valueIfFalse) {
        ///<summary>检查<paramref name="object"/>是否存在<paramref name="key"/>属性并且可转换为true, 否则返回<paramref name="valueIfFalse"/>
        ///</summary>
        ///<param name="key"></param>
        ///<param name="object"></param>
        ///<param name="valueIfFalse"></param>
        if (!object) {
            return valueIfFalse;
        }
        if (key in object && object[key]) {
            return object[key];
        }
        return valueIfFalse;
    }

    /**
             * 生成唯一的ID
             * @method guid
             * @grammar Base.guid() => String
             * @grammar Base.guid( prefx ) => String
             */
    window.guid = (function () {
        var counter = 0;
        return function (prefix) {
            var guid = (+new Date()).toString(32),
                i = 0;

            for (; i < 5; i++) {
                guid += Math.floor(Math.random() * 65535).toString(32);
            }

            return (prefix || 'cli_') + guid + (counter++).toString(32);
        };
    })();

    var loadingHelper = {
        eventHandler: 0,
        id: "_loading_",
        show: function () {

            clearTimeout(loadingHelper.eventHandler);
            loadingHelper.eventHandler = setTimeout(function () {
                var $loading = $("#" + loadingHelper.id);
                if (!$loading.length) {
                    $loading = $('<div class="loading-container">' +
                        '<div class="loading-main">' +
                        '<em class="icon-spinner icon-spin"></em>' +
                        'Loading...</div></div>')
                        .attr("id", loadingHelper.id)
                        .hide()
                        .appendTo("body");
                }
                $loading.show();
            }, 300);
        },
        hide: function () {
            clearTimeout(loadingHelper.eventHandler);
            var $loading = $("#" + loadingHelper.id);
            if ($loading.length) {
                $loading.hide();
            }
        }
    };

    window.loading = function (isShow) {
        ///<summary>显示/隐藏loading状态提示</summary>
        ///<param name="isShow"></param>
        if (arguments.length == 0 || isShow) {
            loadingHelper.show();
        } else {
            loadingHelper.hide();
        }
    }

    window.getSelectOptions = function (sources, textField, valueField) {
        if (arguments.length == 1) {
            textField = "Text";
            valueField = "Value";
        }
        if (!sources || !sources.length) {
            return [];
        }
        var options = [];
        $.each(sources, function (index, option) {
            var item = new SelectItemModel();
            item.text = isNull(textField, option, "");
            item.value = isNull(valueField, option, "");
            options.push(item);
        });
        return options;
    }

    window.needEncode = /|\!|\"|\#|\$|\%|\&|\'|\(|\)|\*|\+|\,|\-|\.|\/|\:|\;|\<|\=|\>|\%|\@|\[|\\|\]|\^|\_|\`|\{|\||\}|\~|\||\ˉ|\,/;


}(jQuery);

/*
datalist
*/
+(function ($) {
    'use strict';
    var Datalist = function (element, options) {

        var self = this;
        this.options = options;
        this.$element = $(element);
        this.$for = $(options.for);
        this.datalistId = "#" + this.$element.attr("id") + "_opts";
        this.$datalist = $(this.datalistId);
        if (!this.$datalist.length) {
            this.$datalist = $("<ul class='search-list' id='" + this.$element.attr("id") + "_dl'></ul>")
                .hide().insertAfter(this.$element);
        }
        this.inputFromKeyboard = true;
        this.loadEvent = 0;
        this.hideEvent = 0;
        this.triggerEvent = 0;
        this.upDownEvent = 0;
        this.dataset = [];

        if (options.searchfields) {
            var fields = options.searchfields.split(",");
            if (fields && fields.length) {
                this.options.searchfields = fields;
            } else {
                this.options.searchfields = [];
            }
        } else {
            this.options.searchfields = [];
        }
        this.options.searchfields.unshift(this.options.text);
        if (options.cached.toString().toLowerCase() === "true") {
            options.cached = true;
        } else {
            options.cached = false;
        }
        if (options.required.toString().toLowerCase() === "true") {
            options.required = true;
        } else {
            options.required = false;
        }
        this.init();
    }
    Datalist.prototype.recordOldValue = function () {
        var data = { text: this.$element.val(), value: this.$for.val() };
        this.$element.data("_source", data);
    };
    Datalist.prototype.init = function () {
        var upCode = 38;
        var rightCode = 39;
        var bottomCode = 40;
        var leftCode = 37;
        var self = this;
        this.recordOldValue();
        this.$element.on("input", function (event) {
            clearTimeout(self.loadEvent);
            if (self.inputFromKeyboard) {
                self.loadEvent = setTimeout(function () {
                    self.loadItems();
                }, 100);
            }
        });
        this.$element.on("keyup", function (event) {
            if (event.which === rightCode
                || event.which === leftCode
            ) {
                self.inputFromKeyboard = true;
                return false;
            }
            //console.log("%s : %d",event.type, event.which);
            clearTimeout(self.loadEvent);
            if (self.inputFromKeyboard) {
                self.loadEvent = setTimeout(function () {
                    self.loadItems();
                }, 100);
            }
        });

        this.$element.on("keydown", function (event) {
            if (event.which !== upCode
                && event.which !== bottomCode
            ) {
                self.inputFromKeyboard = true;
                return true;
            }
            //console.log("keydown : %d", event.which);
            clearTimeout(self.hideEvent);
            clearTimeout(self.upDownEvent);

            var index = -1;
            var total = self.$datalist.children().length;
            if (self.$datalist.find(".active").length == 1) {
                index = self.$datalist.find(".active").index();
            }
            if (event.which == 40) {
                // down arrow
                index++;
            } else if (event.which == 38) {
                // up arrow
                index--;
            }
            if (index < 0) {
                index = self.$datalist.find(".active:last").index();
            }
            if (index >= total) {
                index = 0;
            }
            self.$datalist.find(".active").removeClass("active");
            self.$datalist.children().eq(index).addClass("active");
            self.upDownEvent = setTimeout(function () {
                self.triggerSelect();
                self.$datalist.hide();
            }, 500);
            return true;
        }).on("blur", function () {
            self.restoreIfNotMatch();
        });

        self.$datalist.on("click", "li", function () {
            self.$datalist.find("li").removeClass("active");
            var $this = $(this);
            $this.addClass("active");
            self.triggerSelect();
            self.$datalist.slideUp("fast");
        }).mouseleave(function () {
            self.hideDatalist();
        }).mouseenter(function () {
            clearTimeout(self.hideEvent);
        });

        $("body").on("click", function () {
            self.$datalist.hide();
        });
    }
    Datalist.prototype.hideDatalist = function () {
        var self = this;
        self.hideEvent = setTimeout(function () { self.$datalist.slideUp("fast"); }, 1000);
    }
    Datalist.prototype.showDatalist = function () {
        clearTimeout(self.hideEvent);
        this.$datalist.show();
    }

    Datalist.prototype.trigger = function (event) {
        clearTimeout(this.triggerEvent);
        var self = this;
        this.triggerEvent = setTimeout(function () {
            var selectEvent = $.Event(event);
            selectEvent.selected = { text: self.$element.val(), value: self.$for.val() };
            var foundItem = {};
            if (self.dataset && self.dataset.length) {
                $.each(self.dataset, function (i, item) {
                    if (item[self.options.value] == self.$for.val()) {
                        $.extend(foundItem, item);
                        return false;
                    }
                });
                $.extend(selectEvent.selected, foundItem);
            }
            self.$element.trigger(selectEvent);
        }, 500);
    };
    Datalist.prototype.restoreIfNotMatch = function () {
        var self = this;
        var inputed = this.$element.val();
        var foundItem = false;
        if (this.dataset && this.dataset.length) {
            $.each(this.dataset, function (i, item) {
                if (item[self.options.text] == inputed) {
                    foundItem = true;
                    return false;
                }
            });
            if (!foundItem) {
                if (self.options.required) {
                    var data = this.$element.data("_source");
                    this.$element.val("").change();
                }
                this.$for.val("").change();
                this.trigger("selected.sunnet.datalist");
            } else {
                self.$for.valid();
            }
        }
    };
    Datalist.prototype.triggerSelect = function () {
        var self = this;
        var $select = self.$datalist.find(".active");
        if (!$select.length) {
            return false;
        }

        var foundItem = {
            text: $select.text(),
            value: $select.attr("value")
        };

        $.each(self.dataset, function (i, item) {
            if (item[self.options.value] == $select.attr("value")) {
                $.extend(foundItem, item);
            }
        });
        self.$element.val(foundItem[self.options.text]).change();
        self.$for.val(foundItem[self.options.value]).change();
        self.recordOldValue();
        self.$for.valid();
        self.trigger("selected.sunnet.datalist");
    };
    Datalist.prototype.getData = function () {
        var self = this;
        var deferred = $.Deferred();
        if (this.dataset && this.dataset.length) {
            setTimeout(function () {
                deferred.resolveWith(self);
            }, 10);
        } else {
            var params = {};
            params[this.options.paramname] = this.$element.val();
            if (self.options.cached) {
                params[this.options.paramname] = -1;
            }
            if (self.options.extraparams) {
                var $container = self.$element.closest("form,body").last();
                for (var key in self.options.extraparams) {
                    params[key] = $container.find(self.options.extraparams[key]).val();
                }
            }
            this.clearItems();
            $.getJSON(this.options.remote, params, function (items) {
                if (items && items.length) {
                    self.dataset = items;
                    deferred.resolveWith(self);
                } else {
                    deferred.reject();
                }
            }).error(function () {
                deferred.reject();
            });
        }
        return deferred.promise();
    }

    function convertKeyword(source, regex) {
        if (!source) {
            return "";
        }
        var matchs = regex.exec(source);
        var newText = "", index = 0, lastIndex = 0;
        for (var i = 0; i < matchs.length; i++) {
            var match = matchs[i];
            if (match != source) {
                index = source.slice(lastIndex).indexOf(match) + lastIndex;
                newText += source.slice(lastIndex, index);
                newText += "<span style='color:red;font-weight:bold;'>" + match + "</span>";
                //newText += "(" + match + ")";
                lastIndex = index + match.length;
            }
        };
        newText += source.slice(lastIndex);
        return newText;
    }

    Datalist.prototype.loadItems = function () {
        var self = this;
        self.inputFromKeyboard = false;
        var optionTemp = "<% for(var i = 0; i< this.length ;i++ ) { %>" +
            "<li value='<% this[i]." + self.options.value + " %>'>" + self.options.template + "</li>" +
            "<% } %>";
        jQuery.when(self.getData()).done(function () {
            self.$datalist.empty();
            var found = false;
            var showlist = [];
            var key = " " + (self.$element.val() || "") + " ";
            var blackListReg = window._message_.search_Blacklist;
            key = key.replace(blackListReg, "");
            // remove space auto added.
            self.$element.val(key.slice(1, key.length - 1)).change();
            var regex1 = new RegExp("^" + key.replace(/(^\s*)|(\s*$)/g, "") + "$", "i");
            var regex2 = new RegExp("^" + key.replace(/(^\s*)|(\s*$)/g, ""), "i");
            var regex3 = new RegExp("^.*(" + key.replace(/\ /g, ").*(").replace(/\+/ig, "\\+") + ").*$", "i");

            var array1 = [];
            var array2 = [];
            var array3 = [];

            $.each(this.dataset, function (index, item) {
                var newItem = $.extend({}, item);
                if (newItem[self.options.text] == self.$element.val()) {
                    self.$for.val(newItem[self.options.value]).change();
                    found = true;
                }
                $.each(self.options.searchfields, function (i2, field) {
                    if (regex1.test(newItem[field]) && array1.length < 10 && newItem[field] != "") {
                        newItem[field] = convertKeyword(newItem[field], regex1);
                        array1.push(newItem);
                        return false;
                    }
                    else if (regex2.test(newItem[field]) && array2.length < 10 && newItem[field] != "") {
                        newItem[field] = convertKeyword(newItem[field], regex2);
                        array2.push(newItem);

                        return false;
                    }
                    else if (regex3.test(newItem[field]) && array3.length < 10 && newItem[field] != "") {
                        newItem[field] = convertKeyword(newItem[field], regex3);
                        array3.push(newItem);
                        return false;
                    }
                });
                if (array1.length == 10) {
                    return false;
                }
            });
            $.each(array1, function (index, item) {
                var item1 = $.extend({}, item);
                if (showlist.length < 10)
                    showlist.push(item1);
                else return;
            });
            $.each(array2, function (index, item) {
                var item2 = $.extend({}, item);
                if (showlist.length < 10)
                    showlist.push(item2);
                else return;
            });
            $.each(array3, function (index, item) {
                var item3 = $.extend({}, item);
                if (showlist.length < 10)
                    showlist.push(item3);
                else return;
            });

            //$.each(this.dataset, function (index, item)
            //{
            //    var newItem = $.extend({}, item);
            //    if (newItem[self.options.text] == self.$element.val())
            //    {
            //        self.$for.val(newItem[self.options.value]).change();
            //        found = true;
            //    }

            //    if (showlist.length < 10)
            //    {
            //        $.each(self.options.searchfields, function (i2, field)
            //        { 
            //            if (regex.test(newItem[field])) {
            //                newItem[field] = convertKeyword(newItem[field], regex);
            //                if (showlist.length == 0)
            //                    showlist.push(newItem);
            //                else {
            //                    var isExsit = false;
            //                    for (var showIndex = 0; showIndex < showlist.length; showIndex++) {
            //                        if (showlist[showIndex].ID == newItem.ID)
            //                           { isExsit = true;break;}
            //                    }
            //                    if (!isExsit)
            //                    showlist.push(newItem);
            //                }
            //                return false;
            //            }
            //        });
            //    }
            //});


            if (jQuery.trim(self.$element.val()) == "") {
                self.$for.val("").change();
                self.$for.valid();
            }

            if (!self.options.cached) {
                self.dataset.length = 0;
            }
            if (found) {
                self.trigger("selected.sunnet.datalist");
                return false;
            }
            if (showlist.length) {
                var html = TemplateEngine(optionTemp, showlist);
                self.$datalist.append(html).show();
            }
        }).fail(function () {
            var event = $.Event("notfound.sunnet.datalist");
            event[self.options.paramname] = self.$element.val();
            self.$element.trigger(event);
        });
    }
    Datalist.prototype.clearItems = function () {
        this.dataset = null;
        this.$datalist.empty().hide();
        this.$for.val("").change();
    }

    Datalist.prototype.clear = function () {
        this.clearItems();
        this.$element.val("").change();
        this.trigger("selected.sunnet.datalist");
    }

    Datalist.DEFAULTS =
        {
            remote: "",
            cached: true,
            text: "text",
            value: "value",
            paramname: "keyword",
            "for": "",
            template: "<% this[i].TextField %>",
            required: true,
            searchfields: false
        }


    function Plugin(option) {
        var params = [];
        for (var i = 1; i < arguments.length; i++) {
            params.push(arguments[i]);
        }
        return this.each(function () {
            var $this = $(this);
            var options = $.extend({}, Datalist.DEFAULTS, $this.data(), option);
            options.template = options.template.replace("TextField", options.text);
            var data = $this.data('sunnet.datalist');
            if (!data) {
                $this.data('sunnet.datalist', (data = new Datalist(this, options)));
            }
            if (typeof option == 'string') {
                data[option].apply(data, params);
            }
            if ($this.data().autoload && $this.data().autoload == 1) {
                $this.data('sunnet.datalist').getData();
            }
        });
    }

    $.fn.datalist = Plugin;

    $.fn.datalist.Constructor = Datalist;
    $("input[data-list][data-remote]").each(function (index, input) {
        $(this).datalist();
    });

})(jQuery);

/*
form
*/
+(function ($) {
    var CliForm = function (element) {
        var self = this;
        this.$element = $(element);
    };
    CliForm.DEFAULTS = {};
    CliForm.prototype.count = function (key, count) {
        var _count = this.$element.data("_" + key + "_") || 0;
        if (arguments.length == 2) {
            _count += count;
            this.$element.data("_waiting_", count);
        } else {
            return _count;
        }
    };
    CliForm.prototype.reset = function (key) {
        this.$element.data("_" + key + "_", 0);
    };

    CliForm.prototype.loaded = function () {
        this.$element.find("button:submit,button.submit-btn,input:submit,input.submit-btn").button("reset");
        window.loading(false);
    };
    CliForm.prototype.loading = function () {
        this.$element.find("button:submit,button.submit-btn,input:submit,input.submit-btn").button("loading");
        window.loading();
    };
    function Plugin(option) {
        var params = [];
        for (var i = 1; i < arguments.length; i++) {
            params.push(arguments[i]);
        }
        return this.each(function () {
            var $this = $(this);
            var options = $.extend({}, CliForm.DEFAULTS, $this.data(), option);
            var data = $this.data('sunnet.form');
            if (!data) {
                $this.data('sunnet.form', (data = new CliForm(this, options)));
            }
            if (typeof option == 'string') {
                data[option].apply(data, params);
            }
        });
    }

    $.fn.cliForm = Plugin;

    $.fn.cliForm.Constructor = CliForm;
})(jQuery);


/*
placeholder
*/
+(function ($) {
    $("input:text._phone,input:text._input_phone,input:text._homephone").attr("placeholder", "(###)###-####");
    var PlaceHolder = function (element, options) {
        var self = this;
        this.$element = $(element);
        this.options = options;
        this.init();
    };
    PlaceHolder.DEFAULTS = {
        placeholder: ""
    }
    PlaceHolder.prototype.update = function () {
        if (this.$element.prop("disabled") || this.$element.prop("readonly")) {
            this.options.placeholder = this.$element.attr("placeholder");
            this.$element.attr("placeholder", "");
        } else {
            if (!this.$element.attr("placeholder")) {
                this.$element.attr("placeholder", this.options.placeholder);
            }
        }
    }
    PlaceHolder.prototype.init = function () {
        var that = this;
        this.update();

    };

    function Plugin(option) {
        var params = [];
        for (var i = 1; i < arguments.length; i++) {
            params.push(arguments[i]);
        }
        return this.each(function () {
            var $this = $(this);
            var options = $.extend({}, PlaceHolder.DEFAULTS, $this.data(), { placeholder: $this.attr("placeholder") }, option);
            var data = $this.data('sunnet.placeholder');
            if (!data) {
                $this.data('sunnet.placeholder', (data = new PlaceHolder(this, options)));
            }
            if (typeof option == 'string') {
                data[option].apply(data, params);
            }
        });
    }

    $.fn.placeholder = Plugin;

    $.fn.placeholder.Constructor = PlaceHolder;

})(jQuery);

