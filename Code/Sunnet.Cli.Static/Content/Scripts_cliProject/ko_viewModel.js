var ViewModelStatus = {
    initialising: 1,
    ready: 20,

    ajaxing: 40,

    showing: 100
}

// 基本分页模型
function ViewModel(options, events) {
    var self = this;
    this.self = this;

    this.status = ko.observable(ViewModelStatus.initialising);

    // 用于在弹开表单标签时标记回调函数的对象名
    this.instanceName = options.instanceName;
    var readSettingsFromUrl = options.instanceName in window;

    this.modalId = options.modalId;
    // 弹出表单容器
    this.$container = "";
    this.getContainer = function () {
        if (!this.$container) {
            if (!this.modalId) {
                this.modalId = "Modal" + Math.round((Math.random() * 1000000));
            }
            this.$container = $("#" + this.modalId);
            if (this.$container.length == 0) {
                this.$container = jQuery('<div class="modal" id="' + this.modalId + '" data-cache="false" data-confirm="true" data-backdrop="static">' +
                    '<div class="modal-dialog">' +
                    '<div class="modal-content">' +
                    '</div></div></div>').appendTo("body");
            }
        }
        return this.$container;
    }

    this.showDialog = function (url) {
        var url = this.appendQueryString(url, { pop: 1 });
        //this.getContainer().removeData("bs.modal").find(".modal-content").empty();
        this.getContainer().modal({
            cache: false,
            remote: url
        });
    }
    this.hideDialog = function () {
        this.getContainer().modal("hideAnyway");
    }

    // 当前是否可显示状态：长时间的加载数据过程会显示Loading...
    this.isDisplaying = ko.observable(false);
    this.waittingEvent = 0;
    this.hide = function () {
        clearTimeout(self.waittingEvent);
        self.waittingEvent = setTimeout(function () { self.isDisplaying(false); }, 1000);
    }
    this.show = function () {
        clearTimeout(self.waittingEvent);
        if (!self.isDisplaying()) {
            self.isDisplaying(true);
        }
    };

    // 当前对象主键的属性名：一般为ID，可以重置
    this.primaryKeyField = options.primaryKeyField || "ID";
    // 统一操作时显示的属性名, 比如删除确认时候 显示哪个属性
    this.showField = options.showField || "Name";
    // 加载数据的基本URL
    this.getDataUrl = options.getDataUrl;
    // 添加对象表单地址
    this.addDataUrl = options.addDataUrl;
    // 添加对象方式: Modal|Redirect
    this.addType = options.addType;
    if (!this.addType) {
        this.addType = "Modal";
    }
    // 查看对象表单地址
    this.viewDataUrl = options.viewDataUrl;
    // 查看对象方式: Modal|Redirect
    this.viewType = options.viewType;
    if (!this.viewType) {
        this.viewType = "Modal";
    }
    // 编辑对象表单地址
    this.editDataUrl = options.editDataUrl;
    // 编辑对象方式: Modal|Redirect
    this.editType = options.editType;
    if (!this.editType) {
        this.editType = "Modal";
    }
    // 删除对象URL
    this.deleteDataUrl = options.deleteDataUrl;

    // 合并查询参数到URL
    this.appendQueryString = function (url, parameters) {
        if (url.indexOf("?") < 0)
            url += "?";
        else if (url.indexOf("&") < 0 || url.indexOf(url.length) != "&")
            url += "&";
        for (var key in parameters) {
            if (parameters[key] === null || parameters[key] === undefined || parameters[key] === "") {
                continue;
            }
            url += key + "=" + encodeURIComponent(parameters[key]) + "&";
        }
        return url.replace(/&$/, "");
    }
    // 合并 MVC | API 参数到URL
    this.appendUrlString = function () {
        if (arguments.length < 2)
            return arguments[0];
        var url = arguments[0];
        if (url[url.length - 1] != "/") url += "/";
        for (var j = 1; j < arguments.length; j++) {
            url += arguments[j];
            url += "/";
        }
        return url.replace(/\/$/, "");
    }

    // 查看对象的操作
    this.viewData = function (data) {
        var param = {};
        param[self.primaryKeyField] = data[self.primaryKeyField];
        if (self.viewType == "Redirect") {
            param["returnUrl"] = self.getLocation();
        }
        var viewDataUrl = self.appendQueryString(self.viewDataUrl, param);
        if (self.viewType == "Modal") {
            self.showDialog(viewDataUrl);
        } else if (self.viewType == "Redirect") {
            location.href = viewDataUrl;
        }
    }

    // 添加对象的操作
    this.addData = function () {
        var param = {};
        if (self.addType == "Redirect") {
            param["returnUrl"] = self.getLocation();
        } else {
            param["listObj"] = self.instanceName;
        }
        var addDataUrl = self.appendQueryString(self.addDataUrl, param);
        if (self.addType == "Modal") {
            self.showDialog(addDataUrl);
        } else if (self.addType == "Redirect") {
            location.href = addDataUrl;
        }
    }
    // 添加对象回调
    this.onDataAdded = function (response) {
        if (typeof response == "string")
            response = JSON.parse(response);
        self.showRecords.unshift(response.data);
        //self.clearCache();
        var pageDatas = self.pageRecords().filter(function (pageData) {
            return pageData.page == self.currentPageIndex();
        });
        if (pageDatas.length) {
            pageDatas[0].data.unshift(response.data);
        } else {
            self.search();
        }
        self.hideDialog();
    }

    // 编辑对象的操作
    this.updateData = function (data, event) {
        var param = {};
        param[self.primaryKeyField] = data[self.primaryKeyField];
        if (self.editType == "Redirect") {
            param["returnUrl"] = self.getLocation();
        } else {
            param["listObj"] = self.instanceName;
        }
        var editDataUrl = self.appendQueryString(self.editDataUrl, param);
        if (self.editType == "Modal") {
            self.showDialog(editDataUrl);
        } else if (self.editType == "Redirect") {
            location.href = editDataUrl;
        }
    }
    this.updateDataTab = function (data, event) {
        var param = {};
        param[self.primaryKeyField] = data[self.primaryKeyField];
        param["listObj"] = self.instanceName;
        param['backurl'] = encodeURI(self.getCurrentLocation());
        var url = self.appendQueryString(self.editDataUrl, param);
        if (jQuery(event.target).attr("target") == "_blank") {
            window.open(url);
        } else {
            location.href = url;
        }
    }

    // 编辑对象的回调
    this.onDataUpdated = function (response) {
        if (typeof response == "string")
            response = JSON.parse(response);
        if (response.success) {
            var datas = self.getPageDataFromLocal(self.currentPageIndex());
            for (var i = 0; i < datas.length; i++) {
                if (datas[i][self.primaryKeyField] == response.data[self.primaryKeyField]) {
                    if (self.dataUpdater) {
                        datas[i] = self.dataUpdater(datas[i], response.data);
                    } else {
                        jQuery.extend(datas[i], response.data);
                    }
                    break;
                }
            }
            self.showPage();
            self.hideDialog();
        }
    }

    // 弹出窗口失败的回调
    this.onFailed = function (xhr, statusText, error) {
        // 由于此方法的调用者是弹出来的form对象，因此方法内部不能使用this获取对应的ViewModel对象，this指向的是相关的Form对象
        var errorMsg = jQuery.parseJSON(xhr.responseText);
        errorMsg.enter = "<br/>";
        var html = errorMsg.Message + errorMsg.enter;
        if (xhr.status == 500) {
            var htmlTemp = "ExceptionType: <%this.ExceptionType%><%this.enter%>" +
                "ExceptionMessage: <%this.ExceptionMessage%><%this.enter%>" +
                "StackTrace: <%this.StackTrace%><%this.enter%>";
            html += TemplateEngine(htmlTemp, errorMsg);
        }
        else {
            var htmlTemp = "<%for(var name in this){%>" +
                                "<%name%>:<%for(var index=0; index < this[name].length; index++){%><%this[name][index]%><%this.enter%>" +
                            "<%}%>" +
                        "<%}%>";
            html += TemplateEngine(htmlTemp, errorMsg.ModelState);
            //alert(html);
        }
        self.showMessage("fail", html);
    }
    // 删除对象的操作
    this.deleteData = function (data, event) {
        var param = {};
        param[self.primaryKeyField] = data[self.primaryKeyField];
        var deleteUrl = self.deleteDataUrl;
        var message = window.getErrorMessage("confirmToDelete");
        message = message.replace("{0}", data[self.showField]);
        $.when(waitingConfirm(message, "Delete", "Cancel")).done(function () {
            $.post(deleteUrl, param, function (response) {
                if (response.success) {
                    showMessage("success");
                    self.onDataDeleted(data);
                } else {
                    showMessage("fail", response.msg);
                }
            }, 'json');
        });
    }
    this.onDataDeleted = function (data) {
        self.clearCache();
        var datas = self.showRecords();
        var total = datas.length;
        for (var i = 0; i < total; i++) {
            if (datas[i][self.primaryKeyField] == data[self.primaryKeyField]) {
                self.showRecords.splice(i, 1);
                var newDatas = self.showRecords().slice(0, i);
                newDatas = newDatas.concat(self.showRecords().slice(i, total));
                self.showRecords.removeAll();
                self.showRecords.push.apply(self.showRecords, newDatas);
                break;
            }
        }
        self.refresh();
    }
    // 分页相关

    // 总记录数
    this.recordCount = ko.observable(0);
    // 已加载记录数
    this.loadedRecords = ko.observable(0);


    // 每页显示几条数据
    this.displayPerPageOptions = ko.observableArray([]);
    var pageSizeIndex = 0;
    var urlPageSize = readSettingsFromUrl ? window.queryString("count", options.pageSize) : options.pageSize;

    if (!options.displayPerPageOptions) {
        options.displayPerPageOptions = [];
        options.displayPerPageOptions.push({ text: 10, value: 10 });
        options.displayPerPageOptions.push({ text: 20, value: 20 });
        options.displayPerPageOptions.push({ text: 50, value: 50 });
        options.displayPerPageOptions.push({ text: 100, value: 100 });
    }
    for (var i = 0; i < options.displayPerPageOptions.length; i++) {
        this.displayPerPageOptions.push({
            text: options.displayPerPageOptions[i].text,
            value: options.displayPerPageOptions[i].value
        });
    }

    var foundPageSize = false;
    var pageSizeCount = this.displayPerPageOptions().length;
    $.each(this.displayPerPageOptions(), function (index, pageSize) {
        if (pageSize.value == urlPageSize) {
            pageSizeIndex = index;
            foundPageSize = true;
            return false;
        }
    });
    if (!foundPageSize) {
        for (var i = 0; i < pageSizeCount; i++) {
            if (urlPageSize < this.displayPerPageOptions()[i].value) {
                this.displayPerPageOptions.splice(i, 0, {
                    text: urlPageSize,
                    value: urlPageSize
                });
                pageSizeIndex = i;
                foundPageSize = true;
                break;
            }
        }
        if (!foundPageSize) {
            this.displayPerPageOptions.push({
                text: urlPageSize,
                value: urlPageSize
            });
            pageSizeIndex = pageSizeCount;
        }
    }
    this.displayPerPage = ko.observable(this.displayPerPageOptions()[pageSizeIndex]);
    this.displayPerPage.subscribe(function (displayPerPage) {
        setTimeout(function () {
            self.clearCache();
            self.currentPageIndex(1);
            jQuery.when(self.getDataByPage(self.currentPageIndex())).then(function () {
                self.showPage();
            });
        }, 1);
    });

    this.pageSize = ko.computed(function () {
        return this.displayPerPage().value;
    }, self);
    this.miniPager = isNull("miniPager", options, false);

    // 当前页索引：从1开始
    this.currentPageIndex = ko.observable(1);
    if (readSettingsFromUrl && window.queryString("page")) {
        this.currentPageIndex(window.queryString("page"));
    }

    // 总页数
    this.pages = ko.observableArray([]);
    this.totalPageCount = ko.computed(function () {
        return Math.ceil(this.recordCount() / this.pageSize());
    }, self);
    this.resetPages = function () {
        var totalPages = self.totalPageCount();
        self.pages().length = 0;
        if (totalPages <= self.showPageNumberCount) {
            for (var pNumber = 1; pNumber <= totalPages; pNumber++) {
                self.pages.push({ text: pNumber, value: pNumber });
            }
        } else {
            var showCount = this.showPageNumberCount % 2 == 1 ?
                (this.showPageNumberCount - 1) : self.showPageNumberCount;

            var startPageIndex = self.currentPageIndex() - Math.ceil(showCount / 2);
            if (startPageIndex < 1) startPageIndex = 1;
            if (startPageIndex > 1) {
                self.pages.push({ text: "...", value: startPageIndex - 1 });
            }
            for (var i = 1; i <= self.showPageNumberCount; i++) {
                if (startPageIndex <= totalPages)
                    self.pages.push({ text: startPageIndex, value: startPageIndex });
                startPageIndex++;
            }
            if (startPageIndex <= totalPages)
                self.pages.push({ text: "...", value: startPageIndex });
        }
    };
    this.showPageNumberCount = options.showPageNumberCount || 5;

    // 排序
    this.orderBy = ko.observable(options.orderBy);
    if (readSettingsFromUrl && window.queryString("sort")) {
        this.orderBy(window.queryString("sort"));
    }
    // 升序 | 降序
    this.orderDirection = ko.observable(options.orderDirection);
    if (readSettingsFromUrl && window.queryString("order")) {
        this.orderDirection(window.queryString("order"));
    }
    // 已下载的数据
    this.cachedRecords = ko.observableArray([]);
    this.pageRecords = ko.observableArray([]);
    // 当前显示的数据
    this.showRecords = ko.observableArray([]);
    // 表头
    this.headers = ko.observableArray(options.headers);
    for (var i = 0; i < this.headers().length; i++) {
        this.headers()[i].order = this.headers()[i].order || false;
        this.headers()[i].field = this.headers()[i].field || this.headers()[i].text;
    }

    this.searchCriteria = options.searchCriteria;
    // 更换排序
    this.changeOrder = function (header, event) {
        if (self.orderBy() != header.field) {
            self.orderDirection("ASC");
        } else {
            self.orderDirection(self.orderDirection().toUpperCase() == "ASC" ? "DESC" : "ASC");
        }
        self.orderBy(header.field);
        self.search();
    }
    // 查询数据时要跳过的记录数
    this.getFirstRecordIndex = function (page) {
        var p = typeof page == "number" ? page : self.currentPageIndex();
        return (p - 1) * self.pageSize();
    };
    // 清空全部缓存
    this.clearCache = function () {
        self.cachedRecords([]);
        self.pageRecords([]);

        self.hide();
    };

    this.showFromRecord = ko.observable(0);
    this.showRecordsCount = ko.observable(0);
    this.showToRecord = ko.computed(function () {
        return this.showFromRecord() + this.showRecordsCount() - 1;
    }, self);

    // 数据地址
    this.getPageUrl = function (page, firstRecord, count) {
        var url = self.getDataUrl;
        if (!firstRecord) firstRecord = self.getFirstRecordIndex(page);
        if (!count) count = self.pageSize();

        if (url.indexOf("{sort}") > 0) {
            url = url.replace(/{sort}/i, self.orderBy());
            url = url.replace(/{order}/i, self.orderDirection());
            url = url.replace(/{first}/i, firstRecord);
            url = url.replace(/{count}/i, count);
        } else {
            url = self.appendQueryString(self.getDataUrl, {
                sort: self.orderBy(),
                order: self.orderDirection(),
                first: firstRecord,
                count: count
            });
        }
        for (var field in this.searchCriteria) {
            var valueAccessor = this.searchCriteria[field]();
            var value = typeof (valueAccessor) == "object" ? isNull("value", valueAccessor, -1) : valueAccessor;
            if (needEncode.test(value))
                value = encodeURIComponent(value);
            url += "&" + field + "=" + value;
        }
        return url;
    }

    // 从第一页开始加载数据
    this.search = function () {
        self.clearCache();
        if (arguments.length > 0)
            self.currentPageIndex(1);
        self.getPageData();
    };

    this.refresh = function () {
        self.getPageData();
    };

    this.isAjaxing = false;
    this.ajaxCount = 0;
    this.ajaxBegin = function () {
        this.status(ViewModelStatus.ajaxing);
        window.loading();
        this.isAjaxing = true;
        this.ajaxCount += 1;
    }
    this.ajaxDone = function () {
        self.isAjaxing = false;
        self.ajaxCount -= 1;
        window.loading(false);
    };

    this.getDataByPage = function (page) {
        if (page < 1 || (page > self.totalPageCount() && self.totalPageCount() > 0)) {
            throw "page index out of data records";
        }
        var deffered = $.Deferred();

        var first = self.getFirstRecordIndex(page);
        var needRemote = false;
        var url = self.getPageUrl(page);
        var pds = self.pageRecords().filter(function (pageData) {
            return pageData.page == page;
        });

        if (!pds || !pds.length) {
            needRemote = true;
            url = self.getPageUrl(page, first);
        } else {
            needRemote = false;
        }
        if (needRemote) {
            self.ajaxBegin();
            jQuery.getJSON(url, function (response) {
                self.pageRecords.push({
                    page: page,
                    data: response.data
                });
                self.recordCount(response.total);
                self.ajaxDone();
                //self.cachedRecords.push.apply(self.cachedRecords, response.data);
                deffered.resolve();
            });
        } else {
            setTimeout(function () {
                deffered.resolve();
            }, 1);
        }
        return deffered.promise();
    };

    // 获取当页数据
    this.getPageData = function () {
        self.hide();
        if (self.currentPageIndex() > self.totalPageCount() && self.recordCount() > 0
            || self.currentPageIndex() < 1)
            self.currentPageIndex(1);
        jQuery.when(this.getDataByPage(self.currentPageIndex())).then(function () {
            self.showPage();
        });
    };
    // 加载更多数据：相当于下一页，但是此方法用于无分页模式
    this.loadMore = function () {
        var nextPage = self.currentPageIndex() + 1;
        jQuery.when(self.getDataByPage(nextPage)).then(function () {
            var datas = self.getPageDataFromLocal(nextPage);
            self.showRecords.push.apply(self.showRecords, self.dataProcessor(datas));
            self.showRecordsCount(self.showRecordsCount() + datas.length);
            self.currentPageIndex(nextPage);
            self.resetPages();
        });
    }
    // 是否还有未显示数据：提供Load More按钮可见性的控制
    this.hasMore = ko.computed(function () {
        var first = this.getFirstRecordIndex();
        return this.recordCount() > this.showToRecord() + 1 - this.showFromRecord();
    }, self);

    // 是否有记录
    this.hasNoRecord = ko.computed(function () {
        return this.status() > ViewModelStatus.ajaxing && this.recordCount() < 1;
    }, this);

    // 数据处理器：如何处理查询到的数据，有可能需要在具体页面覆盖。
    this.dataProcessor = function (records) {
        return records;
    }

    this.getPageDataFromLocal = function (page) {
        var pageDatas = self.pageRecords().filter(function (pageData) {
            return pageData.page == page;
        });
        if (pageDatas && pageDatas.length)
            return pageDatas[0].data;
        return [];
    };
    // 显示当前页数据：处理好的数据
    this.showPage = function (page) {
        self.status(ViewModelStatus.showing);
        self.show();
        if (!page) page = self.currentPageIndex();
        self.showFromRecord(self.getFirstRecordIndex(page) + 1);
        var datas = self.getPageDataFromLocal(page);
        self.showRecords.removeAll();
        self.showRecords.push.apply(self.showRecords, self.dataProcessor(datas));
        self.showRecordsCount(datas.length);
        if (self.onDataBinded) {
            self.onDataBinded(datas);
        }
        self.resetPages();
    }

    // 翻页
    this.trunPage = function (page, event) {
        if (self.isAjaxing) {
            return false;
        }
        if (typeof page == "object") page = page.value;

        if (page === '-1') page = self.currentPageIndex() - 1;
        if (page === '+1') page = self.currentPageIndex() + 1;
        if (page == -1) page = self.totalPageCount();
        if (self.currentPageIndex() == page) {
            return false;
        }
        if (page > 0 && page <= self.totalPageCount() && page != self.currentPageIndex()) {
            self.currentPageIndex(page);
            self.getPageData();
        }
    };
    this.getLocation = function () {
        var notFromUrlParams = ["sort", "order", "page", "count"];
        var params = {};
        params["sort"] = self.orderBy();
        params["order"] = self.orderDirection();
        params["page"] = self.currentPageIndex();
        params["count"] = self.pageSize();
        for (var i in window.AllQueryString) {
            if (notFromUrlParams.indexOf(i) < 0 && !(i.toLowerCase() in params)) {
                url += "&";
                var value = window.queryString(i);
                if (i.toLowerCase() == "returnurl" &&
                    (value.indexOf("%") < 0 || /\/|\&|\?/.test(value))) {
                    value = encodeURIComponent(value);
                }
                params[i] = value;
            }
        }
        for (var i in self.searchCriteria) {
            if (notFromUrlParams.indexOf(i) < 0) {
                var valueAccessor = this.searchCriteria[i]();
                var value = typeof (valueAccessor) == "object" ? isNull("value", valueAccessor, -1) : valueAccessor;
                params[i.toLowerCase()] = value;
            }
        }
        var url = self.appendQueryString(location.pathname, params);
        return encodeURIComponent(url);
    };

    jQuery.extend(self, events);
    jQuery.ajaxSetup({
        beforeSend: function () {
            self.ajaxBegin();
        },
        error: function (xhr, statusText, responseText) {
            if (xhr.status == 401) { //移除 || xhr.status == 0
                location.href = "/?ReturnUrl=" + encodeURI(location.pathname);
            }
        },
        complete: function () {
            self.ajaxDone();
        }
    });

    // 处理消息相关的逻辑
    this.showMessage = function (name, moreContent) {
        showMessage(name, moreContent);
    };

    this.status(ViewModelStatus.ready);

    this.appendLabelToBtn = function () {
        $("a.table-btn").each(function () {
            if ($(this).attr("title") && !$(this).html())
                $(this).html("<span style='display:none'>" + $(this).attr("title") + "0</span>");
        });
    };
    this.appendLabelToBtn();
}
