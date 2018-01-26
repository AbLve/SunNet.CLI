var OfflineStatus = {
    None: 0,
    Cached: 10,
    Changed: 20,
    Syncing: 30,
    Synced: 40,
    Error: 50
}
function getOfflineApp() {
    var app = {};
    app.values =
    {
        Key: {
            suffix: "",
            assessment: "__Assessment",
            targets: "__Targets",
            target: "__Target",
            cachedOn: "__CachedOn",
            syncedOn: "__SyncedOn",
            cachedStatus: "__CachedStatus",
            pin: "__Pin",
            unlockedOn: "__UnlockedOn",
            onlineUrl: "__OnLineUrl",
            workingTarget: "__Working",
            working: "__running",
            syncing: "__syncing"
        },
        Status: OfflineStatus,
        Url: {
            prepareDataUrl: "",
            checkOnline: "",
            sync: "",
            exec: "",
            view: ""
        },
        locker: "#locker",
        unlocker: "#unlock",
        reset: "#resetPin",
        // 4 letters or numbers
        pinChecker: /^.*(?=.{6,10})(?=.*\d)(?=.*[A-Z]).*$/g,
        CheckOnLineSeconds: 5 * 1000, // 5 seconds
        CheckPinTime: 10 * 60 * 1000, // 10 mins
        data_ExpiredAfterDays: 90 * 24 * 60 * 60 * 1000, // 90Days
        checkOnlineTryTimes: 1,
        checkOnlineLeftTimes: 1
    };
    app.appendKey = function (key, value) {
        if (arguments.length === 1) {
            value = key;
            // append for all key
            for (var prop in this.values.Key) {
                if (this.values.Key.hasOwnProperty(prop)) {
                    this.values.Key[prop] += "__" + value;
                }
            }
        } else {
            // set for Specified key
            if (key in this.values.Key) {
                this.values.Key[key] += "__" + (value || "");
            } else {
                // add Key
                this.values.Key[key] = "__" + value + this.values.Key.suffix;
            }
        }
        return this;
    };

    app.getItem = function (key, valueIfNull) {
        var result = localStorage.getItem(key);
        if (result) {
            try {
                return JSON.parse(result);
            } catch (e) {
                return result;
            }
        }
        return valueIfNull;
    };

    app.setItem = function (key, value) {
        var strResult = "";
        if (typeof (value) == "object" && value.constructor === Date) {
            strResult = value.Format("MM/dd/yyyy HH:mm:ss");
        } else {
            strResult = JSON.stringify(value);
        }
        localStorage.setItem(key, strResult);
        return this;
    };

    app.removeItem = function (key) {
        localStorage.removeItem(key);
    };

    app.getBoolItem = function (key) {
        var boolValue = this.getItem(key);
        return boolValue === true;
    };

    app.getDateItem = function (key) {
        var value = this.getItem(key);
        var d = new Date(value);
        if (!/Invalid|NaN/.test(d.toString())) {
            return d;
        }
        return null;
    };
    app.clearCachedData = function () {
        //localStorage.clear();
        var targets = app.getItem(app.values.Key.targets);
        if (targets && targets.length) {
            for (var i = 0; i < targets.length; i++) {
                localStorage.removeItem(app.values.Key.target + targets[i]);
            }
        }
        // will be reset values, no need to remove
        // pin has been set online , can not remove;
        //for (var key in app.values.Key) {
        //    localStorage.removeItem(app.values.Key[key]);
        //}

        //trs cache
        var targetAssessment = app.getItem(app.values.Key.WorkingTargetAssessment);
        if (targetAssessment && targetAssessment.length > 0) {
            localStorage.removeItem(app.values.Key.WorkingTargetAssessment);
        }
    };

    app.writeLocalData = function () {
        this.setItem(this.values.Key.cachedOn, new Date());
        this.setItem(this.values.Key.cachedStatus, this.values.Status.Cached);
        this.setItem(this.values.Key.unlockedOn, new Date());
    };

    app.cachedOn = null;
    app.lastSyncedOn = null;
    app.unlockedOn = null;
    app.status = app.values.Status.None;
    app.pin = "";
    app.onlineUrl = "";
    app.targetIds = [];
    app.targets = [];
    app.targetByKey = {};
    app.assessment = null;
    app.viewFromOnline = false;
    app.working = false;
    var $reset, $locker, $unlocker;

    app.changed = ko.observable(false);

    app.init = function () {
        $.ajaxSetup({ cache: true });
        $reset = $(this.values.reset);
        $locker = $(this.values.locker);
        $unlocker = $(this.values.unlocker);

        if (location.pathname.toLowerCase().indexOf("offline") < 0
            || location.pathname.toLowerCase().indexOf("preparing") >= 0) {
            app.viewFromOnline = true;
        }
        // current user has cached data before
        if (localStorage.length && localStorage[this.values.Key.assessment]) {
            this.cachedOn = this.getDateItem(this.values.Key.cachedOn);
            this.pin = this.getItem(this.values.Key.pin);
            this.onlineUrl = this.getItem(this.values.Key.onlineUrl);
            this.unlockedOn = this.getDateItem(this.values.Key.unlockedOn);

            var running = Cookies.get(this.values.Key.working);
            if (running) {
                app.working = true;
            }

            if (!this.viewFromOnline && this.cachedOn) {
                var expired = new Date() - this.cachedOn > this.values.data_ExpiredAfterDays;
                if (expired) {
                    // data expired after 3 months
                    $.when(window.waitingAlert("warning", "Offline_Expired")).done(function () {
                        //location.href = app.onlineUrl;
                        history.back();
                    });
                    return false;
                }
            }

            this.status = this.getItem(this.values.Key.cachedStatus);
            this.targetIds = this.getItem(this.values.Key.targets);
            if (this.targetIds && this.targetIds.length) {
                for (var i = 0; i < this.targetIds.length; i++) {
                    var target = this.getItem(this.values.Key.target + this.targetIds[i]);
                    this.targetByKey[this.targetIds[i]] = target;
                    this.targets.push(target);
                }
            }

            this.assessment = this.getItem(this.values.Key.assessment);
        } else {
            this.status = this.values.Status.None;
            this.working = true;

            if (this.viewFromOnline === false) {
                location.href = "/";
            }
        }
        if (this.viewFromOnline) {
            this.bindOnlineEvents();
        } else {
            this.bindOfflineEvents();
            this.checkOnline();
        }
    };

    app.showLocker = function (prepareUrl) {
        this.values.Url.prepareDataUrl = prepareUrl;
        $locker.modal("show");
    };

    app.bindOnlineEvents = function () {
        $("#btnLock").click(function () {
            var $txtLock = $("#txtLock");
            var pin = $txtLock.val();
            if (!pin) {
                window.showMessage("hint", "pin_Required");
                return false;
            }
            if (!app.values.pinChecker.test(pin)) {
                window.showMessage("hint", "pin_Complexity");
                return false;
            }
            app.updatePin(pin);
            $locker.modal("hide");
            $txtLock.val("");
            if (app.values.Url.prepareDataUrl) {
                location.href = app.values.Url.prepareDataUrl;
                return false;
            } else if (app.customOfflinePreparing) {
                app.customOfflinePreparing();
                return false;
            }
            return true;
        });

        $("#btnResetPin").click(function () {
            var $txtLock = $("#txtNewPin");
            var pin = $txtLock.val();
            if (!pin) {
                window.showMessage("hint", "pin_Required");
                return false;
            }
            if (!app.values.pinChecker.test(pin)) {
                window.showMessage("hint", "pin_Complexity");
                return false;
            }

            app.updatePin(pin);
            $reset.modal("hide");
            $txtLock.val("");
        });
    };
    app.bindOfflineEvents = function () {
        this.bindApplicationCacheListener();

        $("#btnUnlock").click(function () {
            var $txtUnlock = $("#txtUnlock");
            if (app.unlock($txtUnlock.val())) {
                $unlocker.modal("hide");
                $txtUnlock.val("");
            } else {
                window.showMessage("hint", "pin_Error");
            }
        });

        $("#hrefOnline").click(function () {
            app.goOnline();
            return false;
        });

        if (this.needUnlock()) {
            $unlocker.modal("show");
        }

        $("body").on("click", "input:text.pin,input:password.pin", function () {
            $(this)[0].select();
        });
        window.onunload = function () {
            //console.log(arguments);
            //var now = new Date();
            //var lastTime = now - app.values.CheckPinTime;
            //var lastDate = new Date(lastTime);
            //app.setItem(app.values.Key.unlockedOn, lastDate);
        };
    };
    app.bindApplicationCacheListener = function () {
        var that = this;
        // Fired after the first cache of the manifest.
        applicationCache.addEventListener('cached', function () {
            var msg = "Resource download is complete. You may now disconnect the internet connection.";
            that.log("success", msg);
            alert(msg);
        }, false);

        // Fired after the first download of the manifest.
        applicationCache.addEventListener('noupdate', function () {
            that.log("success", "No resource needs to be updated.");
        }, false);

        // Checking for an update. Always the first event fired in the sequence.
        applicationCache.addEventListener('checking', function () {
            that.log("info", "Checking for update...");
        }, false);

        // An update was found. The browser is fetching resources.
        applicationCache.addEventListener('downloading', function () {
            that.log("info", "Downloading resources need to be updated...");
        }, false);

        // The manifest returns 404 or 410, the download failed,
        // or the manifest changed while the download was in progress.
        applicationCache.addEventListener('error', function () {
            //that.log("danger",  "manifest file");
            console.log(arguments);
            console.log("manifest file error, or download error, please refresh this page later.");
        }, false);

        // Fired if the manifest file returns a 404 or 410.
        // This results in the application cache being deleted.
        applicationCache.addEventListener('obsolete', function () {
            that.log("danger", "Local cache was deleted by random, please download resources again online.");
        }, false);

        var helper = (function () {
            var index = 1;
            return {
                show: function (event) {
                    var total = event.total ? ("/" + (event.total + 1)) : "";
                    that.log("info", "Downloading file(s) " + index++ + total);
                }
            }
        })();
        // Fired for each resource listed in the manifest as it is being fetched.
        applicationCache.addEventListener('progress', function (event) {
            helper.show(event);
        }, false);

        // Fired when the manifest resources have been newly redownloaded.
        applicationCache.addEventListener('updateready', function () {
            if (window.applicationCache.status == window.applicationCache.UPDATEREADY) {
                // Browser downloaded a new app cache.
                // Swap it in and reload the page to get the new hotness.
                try {
                    window.applicationCache.swapCache();
                    alert('Resource download is complete. You may now disconnect the internet connection. A new version of assessment is available and downloaded. The page will reload!');
                    window.location.reload();
                } catch (e) {
                    execHelper.log(e.name, e.message);
                }
            } else {
                that.log("success", "Nothing needs to be updated.");
            }
        }, false);
    };

    app.log = function (type, format) {
        if (arguments.length > 2) {
            for (var i = 2; i < arguments.length; i++) {
                format = format.replace(new RegExp("\\{" + (i - 2) + "\\}", "g"), arguments[i]);
            }
        }
        if (typeof (this.writeLog) === "function") {
            this.writeLog(type, format);
        } else {
            console.group(type);
            console.log(format);
        }
    };

    app.registeChange = function () {
        this.setItem(this.values.Key.cachedStatus, this.values.Status.Changed);
    };

    app.checkChangeStatus = function (changed) {
        var hasChanged = false;
        if (arguments.length === 0 && this.targetIds && this.targetIds.length && this.targets && this.targets.length) {
            for (var i = 0; i < this.targets.length; i++) {
                if (this.targets[i].changed) {
                    hasChanged = true;
                    break;
                }
            }
        }
        if (hasChanged || changed) {
            this.changed(true);
            this.setItem(this.values.Key.cachedStatus, this.values.Status.Changed);
        } else {
            this.changed(false);
            this.setItem(this.values.Key.cachedStatus, this.values.Status.Synced);
        }
    };

    app.needSetPin = function () {
        if (this.viewFromOnline) {
            return false;
        }
        if (this.pin) {
            return false;
        }
        return true;
    };

    app.updatePin = function (pin) {
        var encryptPin = md5(pin);
        this.setItem(this.values.Key.pin, encryptPin);
        this.setItem(this.values.Key.unlockedOn, new Date());
        Cookies.set(this.values.Key.working, true, { path: '/' });
        this.working = true;
    };

    app.needUnlock = function (pin) {
        $("#coverLayer").hide();
        if (this.viewFromOnline) {
            return false;
        }
        if (this.pin && this.working === false) {
            // first open the page
            return true;
        }
        if (this.pin) {
            var lastUnlockDate = this.unlockedOn;
            var date = new Date();
            var timespan = date - lastUnlockDate;
            return timespan >= this.values.CheckPinTime;
        }
        return false;
    };

    app.unlock = function (pinInput) {
        var encryptPin = md5(pinInput);
        if (this.pin) {
            if (this.pin == encryptPin) {
                this.updatePin(pinInput);
                return true;
            }
        }
        return false;
    };
    app.resetPin = function () {
        if (this.viewFromOnline) {
            $reset.modal("show");
        }
    };
    app.resumePin = function () {
        this.setItem(this.values.Key.unlockedOn, new Date());
    };

    app.network = {
        online: ko.observable(false),
        logged: ko.observable(false),
        date: ko.observable("")
    };

    app.checkNavigatorOnline = function () {
        if (navigator.onLine === true) {
            app.values.checkOnlineLeftTimes = app.values.checkOnlineTryTimes;
            app.checkOnline();
        } else {
            app.network.online(false);
            setTimeout(function () {
                //console.log("navigator Offline");
                app.checkNavigatorOnline();
            }, app.values.CheckOnLineSeconds);
        }
    };

    app.checkOnline = function () {
        var errorOccuredDuringNerwork = false;
        if (!navigator.onLine) {
            app.checkNavigatorOnline();

            app.network.online(false);
            //app.network.logged(false);
            app.network.date("");
        }
        else {
            var newDate = new Date();
            app.network.online(true);
            //app.network.logged(true);//没有调用服务器的Online，无法判断用户是否已登录 David 10/02/2016
            app.network.date("[" + newDate + "]");

            setTimeout(function () { app.checkOnline(); }, app.values.CheckOnLineSeconds);
        }

    };
    //--- David 10/02/2016 Begin
    app.checkLogin = function () {
        jQuery.ajax(
           {
               url: this.values.Url.checkOnline,
               cache: false,
               dataType: "text"
           }
       ).success(
           function (response) {
               var online = null;
               try {
                   online = JSON.parse(response);
               } catch (e) {

               }
               if (online && typeof (online) === "object") {
                   var message = "Online now, " + " logged: <strong>" + (online.logged ? "Yes" : "No") + "</strong>, server: " + online.date;
                   //app.log(online.logged ? "info" : "warning", message);
                   if (app.network.online() === false) {
                       app.log("success", message);
                   }
                   app.network.online(true);
                   app.network.logged(online.logged);
                   app.network.date("[" + online.date + "]");
                   return online.logged;
               } else {
                   if (app.network.online()) {
                       //console.log(arguments);
                       app.log("danger", response);
                   }

                   //app.log("info", "Offline now.");
                   app.network.online(false);
                   app.network.logged(false);
                   app.network.date("");
               }
           }
       ).error(function (jqXhr, errorMsg, errorObj) {
           if (app.network.online()) {
               //console.log(arguments);
               app.log("danger", jqXhr.responseText);
           }
           errorOccuredDuringNerwork = true;
           app.network.online(false);
           app.network.logged(false);
           app.network.date("");
       }).complete(function () {

       }
     
       );

        return false;
       
    }//For app.checkLogin  --David 10/02/2016

    app.goOnline = function () {
        if (this.changed()) {
            window.showMessage("warning", "need_Sync_Before_goOnline");
        } else {
            location.href = this.onlineUrl;
        }
    };

    app.markSyncing = function (isSyncing) {
        isSyncing = arguments.length === 1 ? isSyncing : true;
        app.setItem(app.values.Key.syncing, isSyncing);
    };

    app.shouldStartSyncing = function () {
        var should = app.getItem(app.values.Key.syncing);
        if (should === true || should === false) {
            app.removeItem(app.values.Key.syncing);
            return should;
        }
        return false;
    };
    return app;
}

function getOfflineAppForModule(type, userId, urls) {
    var app = getOfflineApp();
    app.appendKey(type).appendKey(userId).appendKey("CLI").appendKey("target", "");
    $.extend(app.values.Url, urls);

    app.getAssessmentName = function (name) {
        return name;
    }

    return app;
};
