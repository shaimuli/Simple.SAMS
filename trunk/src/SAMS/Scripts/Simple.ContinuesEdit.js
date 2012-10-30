(function ($, _, S) {

    S.ContinuesEdit = Class.extend({
        isNumberKey: function (event) {
            var charCode = (event.which) ? event.which : event.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }

            return true;

        },
        init: function (config) {
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
            this.loadItems();
            setInterval(_.bind(this.onSave, this), config.interval || 15 * 1000);
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
        exists: function(key) {
            var storageKey = this.storageRootKey + "[" + key + "]";
            var value = localStorage.getItem(storageKey);
            return typeof value !== "undefined" && value != null;
        },
        loadItems: function () {
            var items = $(this.logicalItemContainerSelector, this.container);
            _.each(items, function (item) {
                var key = $(item).attr("data-key");
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
        send: function () {
            for(var i=0; i<localStorage.length; i++) {
                var key = localStorage.key(i).toString();
                if (this.rxQueued.test(key)) {
                    var item = this.getByKey(key);
                    if (item) {
                        $.post(this.sendUrl, item, _.bind(function () {
                            this.setByKey(key, null);
                        }, this));
                    }
                }
            }
        },
        queueSave: function (key) {
            var item = this.get(key);
            
            if (item && !this.exists(key + ":Q")) {
                item = this.translate(item);
                this.set(key + ":Q", item);
                this.set(key, null);
            }
            this.send();
        },
        onSave: function () {
            var items = $(this.logicalItemContainerSelector, this.container);
            _.each(items, function (item) {
                this.queueSave($(item).attr("data-key"));
            }, this);
        },
        storeValue: function (input) {
            input = $(input);
            var parentItem = input.closest(this.logicalItemContainerSelector);
            var key = parentItem.attr("data-key");
            var item = this.get(key) || { };
            var name = input.attr("data-name") || input.attr("name");
            item[name] = input.is("input[type=radio]") ? $(":checked", input).val() : input.val();

            this.set(key, item);
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
                result = this.isNumberKey(event);
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
                $(event.target).blur();
                $(this.inputs[currentIndex]).focus();
            } else if (event.keyCode == 27) {
                var originalValue = $(event.target).data("original-value");
                $(event.target).val(originalValue);
            }
        }
    });
})(jQuery, _, Simple);
