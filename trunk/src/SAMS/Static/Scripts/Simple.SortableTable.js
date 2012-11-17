(function ($, S) {
    $.tablesorter.addParser({
        // set a unique id 
        id: 'date',
        is: function (s) {
            // return false so this parser is not auto detected 
            return false;
        },
        format: function (s) {
            // format your data for normalization 

            var date = Date.parse(s);
            var result = -1;
            if (date) {
                result = ((date.getTime() * 10000) + 621355968000000000);
            }
            return result;
        },
        // set type, either numeric or text 
        type: 'numeric'
    });
    
    S.SortableTable = Class.extend({
        init: function (config) {
            this.container = $(config.containerSelector);
            var headers = { };
            $("th", this.container).each(function(index, item) {
                item = $(item);
                var sortType = item.data("sort-type");
                var sorter = { sorter: false };
                if (sortType) {
                    sorter.sorter = sortType;
                }

                headers[index] = sorter;
            });
            var options = {
                headers: headers
            };
            var tableSorter = this.container.tablesorter(options);
            console.log($.tablesorter.parsers);
        }
    });
})(jQuery, Simple);