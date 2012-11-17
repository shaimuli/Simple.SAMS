(function ($) {
    Simple.Shell = function() {
        var proto = {
            start: function () {
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
            bindPopups: function() {
                $("a[target='popup']").click(function(event) {
                    event.preventDefault();
                    var target = $(this);
                    $("<div/>").load(target.attr("href"), function () {
                        $(this).dialog({title:target.text(), modal: true });
                   
                    });
                    return false;
                });
            },
            bindTables: function () {
                $("table.sortable").each(
                    function () {
                        console.log("SORTABLE", $(this).attr("class"));
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