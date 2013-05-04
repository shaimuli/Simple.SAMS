(function ($, _, S) {
    S.Utils = {
        isNumberKey: function (event) {
            var charCode = (event.which) ? event.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }

            return true;

        }
    };
    S.ContinuesEdit = Class.extend({
        init: function (config) {
            this.config = config;
            this.storageRootKey = config.storageRootKey;
            this.sendUrl = config.sendUrl;
            this.container = $(config.containerSelector);
            this.logicalItemContainerSelector = config.logicalItemContainerSelector;
            this.inputs = $(":input", this.container);
            this.inputs.blur(_.bind(this.onFieldBlur, this));
            this.inputs.focus(_.bind(this.onFieldFocus, this));
            this.inputs.keyup(_.bind(this.onFieldKeyUp, this));
            this.inputs.keypress(_.bind(this.onFieldKeyPress, this));

            $("input[type=radio]", this.inputs).change(_.bind(this.onRadioFieldChanged, this));
            this.rxQueued = /.*?\:Q/gi;

            this.version = this.get("Version") || 1;
            this.updateInterval = config.interval;

            $(".sendUpdatesNow", this.container).click(_.bind(function () {
                this.saveNow();
            }, this));

            this.scheduleUpdates();

        },
        scheduleUpdates: function () {
            if (this.config.autoCommit) {
                this.timeout = setTimeout(_.bind(this.onSave, this), this.interval || 15 * 1000);
            }
        },
        cancelSchedule: function () {
            if (this.timeout) {
                clearTimeout(this.timeout);
            }
        },
        saveNow: function () {
            this.cancelSchedule();
            this.onSave();
        },
        getByKey: function (key) {
            var item = localStorage.getItem(key);
            if (item) {
                item = JSON.parse(item);
            }
            return item;
        },
        setByKey: function (key, item) {
            if (item) {
                localStorage.setItem(key, JSON.stringify(item));
            } else {
                localStorage.removeItem(key);
            }
        },
        get: function (key) {
            var storageKey = this.storageRootKey + "[" + key + "]";
            return this.getByKey(storageKey);
        },
        set: function (key, item) {
            var storageKey = this.storageRootKey + "[" + key + "]";
            this.setByKey(storageKey, item);
        },
        exists: function (key) {
            var storageKey = this.storageRootKey + "[" + key + "]";
            var value = localStorage.getItem(storageKey);
            return typeof value !== "undefined" && value != null;
        },
        loadItems: function () {
            var items = $(this.logicalItemContainerSelector, this.container);
            _.each(items, function (item) {
                var key = $(item).removeClass("sending").attr("data-key");
                var itemValue = this.get(key);
                if (itemValue) {
                    _.each(itemValue, function (value, name) {
                        $("input[name='" + name + "']", item).val(value);
                    });
                }

            }, this);
        },
        translate: function (item) {
            return item;
        },
        send: function (updates) {
            var items = [];
            if (updates && updates.length) {
                for (var j = 0; j < updates.length; j++) {
                    var row = $(this.logicalItemContainerSelector + "[data-key='" + updates[j].Id + "']", this.container);
                    row.removeClass("waiting").addClass("sending");
                    items.push({ item: this.translate(updates[j]), row: row });
                }
            } else {

                for (var i = 0; i < localStorage.length; i++) {
                    var key = localStorage.key(i).toString();
                    if (this.rxQueued.test(key)) {
                        var item = this.getByKey(key);

                        if (item) {
                            var row = $(this.logicalItemContainerSelector + "[data-key='" + item.Id + "']", this.container);
                            row.removeClass("waiting").addClass("sending");
                            items.push({ item: item, row: row });
                        }
                    }
                }
            }

            if (items.length > 0) {
                $.ajax({
                    url: this.sendUrl,
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify($.map(items, function (singleItem) {
                        return singleItem.item;
                    })),
                    success: _.bind(function () {
                        if (this.config.autoCommit) {
                            _.each(items, _.bind(function (it) {
                                this.setByKey(it.Id + ":Q", null);
                            }, this));
                        }
                        $.each(items, function (index, singleItem) {
                            singleItem.row.removeClass("sending").addClass("success");
                        });
                        var now = new Date();
                        $(".lastSaveTime", this.container).text(String(now.getHours()).padLeft(2, '0') + ":" + String(now.getMinutes()).padLeft(2, '0') + ":" + String(now.getSeconds()).padLeft(2, '0'));
                        if (this.config.onSuccessfullUpdate) {
                            this.config.onSuccessfullUpdate();
                        }
                    }, this),
                    failure: _.bind(function () {
                        console.log("Send failure: ");
                    }, this),
                    complete: _.bind(function () {
                        this.scheduleUpdates();
                    }, this)
                });

            }


        },
        queueSave: function (row) {
            var key = row.attr("data-key");
            var item = this.get(key);
            var rowItem = { key: key };
            if (row.attr("data-changed")) {
                $(":input", row).each(function (index, input) {
                    input = $(input);
                    rowItem[input.attr("name")] = input.val();
                });
                row.attr("data-changed", false);
                this.set(key + ":Q", this.translate(rowItem));
            }

            /*            if (item && !this.exists(key + ":Q")) {
                            item = this.translate(item);
                            
                            this.set(key + ":Q", item);
                            this.set(key, null);
                        }*/
        },
        onSave: function () {
            var updates = [];
            var items = $(this.logicalItemContainerSelector, this.container);
            _.each(items, function (item) {
                if (this.config.autoCommit) {
                    this.queueSave($(item));
                } else {
                    var row = $(item);
                    var key = row.attr("data-key");
                    var rowItem = { key: key };
                    if (row.attr("data-changed")) {
                        $(":input", row).each(function (index, input) {
                            input = $(input);
                            rowItem[input.attr("name")] = input.val();
                        });
                        row.attr("data-changed", false);
                        updates.push(rowItem);
                    }
                }
            }, this);


            this.send(updates);

        },
        storeValue: function (input) {
            input = $(input);
            var parentItem = input.closest(this.logicalItemContainerSelector);
            parentItem.attr("data-changed", true);
            var key = parentItem.attr("data-key");
            var item = this.get(key) || { key: key, version: this.version++ };
            var name = input.attr("data-name") || input.attr("name");
            item[name] = input.is("input[type=radio]") ? $(":checked", input).val() : input.val();

            this.set(key, item);
            if (!parentItem.is(".sending")) {
                parentItem.addClass("waiting");
            }
        },
        onRadioFieldChanged: function (event) {
            this.storeValue(event.target);
        },
        onFieldBlur: function (event) {
            $(event.target).closest(this.logicalItemContainerSelector).removeClass("edit-mode");
            this.storeValue(event.target);
        },
        onFieldFocus: function (event) {
            var target = $(event.target);
            if (target.is("[type=text]")) {
                target.select();
            }
            target.data("original-value", target.val());
            target.closest(this.logicalItemContainerSelector).addClass("edit-mode");
        },
        onFieldKeyPress: function (event) {
            var result = true;
            if ($(event.target).is("[data-numeric=true]")) {
                // check if number only...
                result = S.Utils.isNumberKey(event);
            }
            return result;
        },
        onFieldKeyUp: function (event) {
            if (event.keyCode == 13) {
                var currentIndex = _.indexOf(this.inputs, event.target);
                if (currentIndex < this.inputs.length) {
                    currentIndex++;
                } else {
                    currentIndex = 0;
                }
                var textInput = $(event.target);
                textInput.blur();
                if (textInput.is(".hasDatepicker")) {
                    textInput.datepicker("hide");
                    textInput.data("visible", false);
                }
                $(this.inputs[currentIndex]).focus();
            } else if (event.keyCode == 27) {
                var originalValue = $(event.target).data("original-value");
                $(event.target).val(originalValue);
            }
        }
    });

    S.MatchResultsContinuesEdit = S.ContinuesEdit.extend({
        translate: function (item) {
            console.log("TRANS", item);
            var updateInfo = {
                Id: parseInt(item.key, 10),
                Version: item.version
            };
            var rxSetScore = /\bp(\d)s(\d)\b/;
            var rxBreakPoint = /\bbp(\d)\b/;
            var setScores = [];

            function getSetScore(setNumber) {
                var score = _.find(setScores, function (scoreItem) {
                    return scoreItem.Number == setNumber;
                });
                if (!score) {
                    score = {
                        Number: setNumber
                    };

                    setScores.push(score);
                }

                return score;
            }
            _.each(item,
                function (value, name) {
                    var setMatch = rxSetScore.exec(name);
                    var bpMatch = rxBreakPoint.exec(name);

                    if (setMatch) {

                        var playerNumber = parseInt(setMatch[1], 10);
                        var setNumber = parseInt(setMatch[2], 10);
                        var score = getSetScore(setNumber);
                        score["Player" + playerNumber + "Points"] = value;

                    } else if (bpMatch) {
                        var setNumber = parseInt(bpMatch[1], 10);
                        var score = getSetScore(setNumber);
                        score["BreakPoints"] = value;
                    } else if (name == "startTimeType") {
                        updateInfo.StartTimeType = value;
                    } else if (name == "startTimeHours") {
                        updateInfo.StartTimeHours = value;
                    } else if (name == "startTimeMinutes") {
                        updateInfo.StartTimeMinutes = value;
                    } else if (name == "Date") {
                        var minDate = new Date(1900, 1, 1);
                        var date = Date.parseExact(value, "dd/MM/yyyy");
                        if (date && date > minDate) {
                            updateInfo.Date = value;
                        }
                    } else if (name == "matchWinner") {
                        updateInfo.Winner = value;
                    } else if (name == "matchResult") {
                        updateInfo.Result = value;
                    }
                });
            if (_.some(setScores, function (s) {
                return s.Player1Points != "" ||
                    s.Player2Points != "" ||
                    s.BreakPoints != "";
            })) {

                updateInfo.SetScores = setScores;
            } else {
                updateInfo.SetScores = null;
            }


            return updateInfo;
        }
    });

})(jQuery, _, Simple);
