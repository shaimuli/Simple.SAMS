if (typeof String.prototype.trim == "undefined") {
    String.prototype.trim = function() {
        return this.replace(/^\s+|\s+$/gm, '');
    };
}

if (typeof String.prototype.getPadding == "undefined") {
    String.prototype.getPadding = function(total, chr) {
        var padding = "";
        if (this.length < total) {
            for (var i = 0; i < (total - this.length); i++) {
                padding += chr;
            }
            return padding;
        } else {
            return "";
        }
    };
}

if (typeof String.prototype.padRight == "undefined") {

    String.prototype.padRight = function(total, chr) {
        return this + this.getPadding(total, chr || " ");
    };
}

if (typeof String.prototype.padLeft == "undefined") {
    String.prototype.padLeft = function(total, chr) {
        return this.getPadding(total, chr || " ") + this;
    };
}