(function($) {
    window.Tournament = function() {
        var api = {
            render: renderTournament  
        };

        return api;

        function renderElement(name, cssClass, dataAttrs, attrs, callback) {
            var html = [];
            html.push("<");
            html.push(name);
            if (cssClass) {
                html.push(" class='");
                html.push(cssClass);
                html.push("' ");
            }
            
            if (dataAttrs) {
                
                _.each(_.pairs(dataAttrs), function (item) {
                    html.push(" data-");
                    html.push(item[0].toLowerCase());
                    html.push("='");
                    html.push(item[1]);
                    html.push("' ");
                });
            }
            if (attrs) {
                _.each(_.pairs(attrs), function (item) {
                    html.push(" ");
                    html.push(item[0].toLowerCase());
                    html.push("='");
                    html.push(item[1] || "");
                    html.push("' ");
                });
            }

            html.push(">");
            if (callback) {
                callback(html);
            }
            html.push("</");
            html.push(name);
            html.push(">");
            return html.join("");
        }

        function renderDiv(cssClass, dataAttrs, attrs, callback) {
            return renderElement("div", cssClass, dataAttrs, attrs, callback)
        }
        function renderTeam(match, team, position) {
            return renderDiv("t-team", { id: (team ? team.Id : "") }, null, function (html) {
                var cssClass = "t-icon" + (((match.Winner) === (position+1)) ? " winner" : "");
                console.log(match.Winner, position, cssClass);
                html.push(renderDiv(cssClass));
                html.push(renderDiv("t-players", null, null, function (playersHtml) {
                    
                    if (team) {
                        playersHtml.push(renderElement("a","", { id: team.Id }, { href: "#" }, function(linkHtml) {
                            
                            linkHtml.push(team.FullName);
                            if (team.Rank) {
                                linkHtml.push(" ");
                                linkHtml.push("(");
                                linkHtml.push(team.Rank);
                                linkHtml.push(")");
                            }
                        }));
                    }
                }));
                html.push(renderDiv("t-score"));
                if (position == 0) {
                    html.push(renderDiv("t-icon t-stats"));
                } else {
                    html.push(renderDiv("t-icon t-scorecard"));
                }
            });
        }

        function renderMatch(match, part) {
            return renderDiv("t-match", { id: match.Id, part: part }, null, function (html) {
                html.push(renderTeam(match, match.Player1,0));
                html.push(renderTeam(match, match.Player2,1));
            });
        }

        function renderTournament(container, data) {
            var rounds = _.groupBy(data, "Round");
            var roundContainers = [];
            _.each(rounds, function (round, roundIndex) {
                var matches = _.sortBy(round, "RoundRelativePosition");
                var template = [];
                
                template = renderDiv("t-round", {round:roundIndex}, null, function(templateHtml) {
                    var topPart = [], bottomPart = [];
                    _.each(matches, function (match, index) {
                        if ((index < round.length / 2)) {
                            topPart.push(renderMatch(match, 0));
                        } else {
                            bottomPart.push(renderMatch(match, 1));
                        }
                    });
                    templateHtml.push(renderDiv("t-round-top", null, null, function(roundTopHtml) {
                        roundTopHtml.push(topPart.join(""));
                    }));
                    templateHtml.push(renderDiv("t-round-bottom", null, null, function(roundBottomHtml) {
                        roundBottomHtml.push(bottomPart.join(""));
                    }));
                });
                roundContainers.push(renderDiv("t-sep"));
                roundContainers.push(template);
            });
            
            $(roundContainers.join("")).appendTo(container);
        }
    };
})(jQuery);