(function ($) {
    Simple.Shell = function() {
        var proto = {
            start: function () {
                this.bindDatePickers();
                this.bindPopups();
                this.bindTables();
            },
            registerDataTemplate: function(name, template) {
                this.dataTemplates = this.dataTemplates || { };
                this.dataTemplates[name] = template;
            },
            getDataTemplate: function(name) {
                if (this.dataTemplates) {
                    return this.dataTemplates[name];
                }
            },
            bindDatePickers: function () {
                $(".date").each(function () {
                    var textInput = $("input[type=text]", this);

                    textInput.datepicker({
                        dateFormat: "dd/mm/yy",
                        isRTL: true,
                        onClose: function () {
                            textInput.data("visible", false);
                        },
                        onSelect: function () {
                            textInput.change().focus();
                        }
                    });
                    var isNullable = textInput.data("isnullable");
                    var minDate = new Date(1900, 1, 1);
                    textInput.change(function (event) {
                        var date = Date.parseExact(textInput.val(), "dd/MM/yyyy");

                        if ((!isNullable && !date) || date < minDate) {
                            textInput.addClass("error").data("is-valid", false);
                        } else {
                            textInput.removeClass("error").data("is-valid", true);
                        }
                    });

                    function hide() {
                        textInput.datepicker("hide");
                        textInput.data("visible", false);
                    }
                    function show() {
                        textInput.datepicker("show");
                        textInput.data("visible", true);
                    }
                    $(".trigger", this).click(function () {
                        if (textInput.data("visible")) {
                            hide();
                        } else {
                            show();
                        }
                    });
                });

            },
            bindPopups: function() {
                $("a[target='popup']").click(function(event) {
                    event.preventDefault();
                    var target = $(this);
                    $("<div/>").load(target.attr("href"), function () {
                        $(this).dialog({title:target.text(), modal: true, width: 600, resizable: false});
                   
                    });
                    return false;
                });
            },
            bindTables: function () {
                $("table.sortable").each(
                    function () {
                        var sortableTable = new Simple.SortableTable({ containerSelector: this });
                        
                    });
            }
        };
        return proto;
    };


    window.Shell = new Simple.Shell();

    $(function () {

        Shell.registerDataTemplate("CompetitionTypeDetailsPopup", function (container, item) {
            container.html("");
            $("<div/>").text(item.PlayersCount).appendTo(container);
            $("<div/>").text(item.WildcardPlayersCount).appendTo(container);
        });

        Shell.start();
    });
})(jQuery);