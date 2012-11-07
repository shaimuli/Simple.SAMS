(function ($) {
    function prepareTournament(host) {
        var container = $("<div/>").addClass("t-container").appendTo(host);
        var rounds = 6;
        var matchesPerRound = [32, 16, 8, 4, 2, 2];
        host.addClass("ms" + matchesPerRound[6 - rounds]);
        function addTeam(toContainer) {
            var t = $("<div/>").addClass("t-team").appendTo(toContainer);
            $("<div/>").addClass("t-icon").appendTo(t);
            var ps = $("<div/>").addClass("t-players").appendTo(t);
            $("<div/>").addClass("t-icon").appendTo(t);
            $("<div/>").addClass("t-icon").appendTo(t);
            var sc = $("<div/>").addClass("t-score").appendTo(t);

            return t;
        }

        var matchPosition = 0;
        for (var r = 0; r <rounds ; r++) {
            var roundContainer = $("<div/>").addClass("t-round r" + r).appendTo(container);
            var sep = r<rounds-1 ? $("<div/>").addClass("t-sep r" + r).appendTo(container) : null;
            var matchesCount = matchesPerRound[r];
            for (var m = 1; m <= matchesCount; m++) {
                var matchContainer = $("<div/>").addClass("t-match m" + m).attr("id","match" + (matchPosition++)).appendTo(roundContainer);

                var team1 = addTeam(matchContainer);
                var team2 = addTeam(matchContainer);
                if (sep) {
                    if (m % 2 == 0) {
                        var msep = $("<div/>").addClass("t-msep").appendTo(sep);
                        $("<div/>").addClass("t-msep-right").appendTo(msep);
                        $("<div/>").addClass("t-msep-left").appendTo(msep);
                    }
                }
            }
        }
        host.overscroll();

    }
    $.fn.tournament = function (options) {
        return this.each(function () {
            var target = $(this);
            var rounds = target.data("rounds");
            var maxMatches = target.data("matches");
            
            if (!target.data("tournament-attached")) {
                target.data("tournament-attached", true);
                target.html("");
                prepareTournament(target);
            } else {
                if ($.isArray(options)) {
                    
                    var matches = options;
                    function getScore(playerNumber, setScores) {
                        var scores = [];
                        for (var setNumber = 0; setNumber < setScores.length; setNumber++) {
                            var setPoints = setScores[setNumber];
                            scores.push(setPoints["Player" + playerNumber + "Points"] || 0);
                        }
                        return scores.join(" ");
                    }
                    function playerName(p) {
                        var text = p.LocalFirstName + (p.LocalLastName ? " " + p.LocalLastName : "");
                        var html = "<a href='#' data-id='" + p.Id + "'>" + text + "</a>";
                        
                        return html;
                    }
                    function players(p1, det, p2, p3) {
                        if (det) {
                            if (p2) {
                                return playerName(p2) + (p3 ? ", " + playerName(p3) : "");
                            }
                        } else {
                            if (p1) {
                                return playerName(p1);
                            }
                        }
                    }

                    
                    
                    var offset = 32;
                    for(var i=0; i<matches.length; i++) {
                        var matchId = "#match" + String(i + offset);
                        
                        var matchContainer = $(matchId, target);
                        var match = matches[i];

                        console.log("MID", matchId, match);
                        match.Player3 = match.Player1;
                        match.Player4 = match.Player2;
                        var player1 = players(match.Player1, match.Player3, match.Player1, match.Player2);
                        var player2 = players(match.Player2, match.Player3, match.Player3, match.Player4);
                        var team1 = $(".t-team:first-child", matchContainer);
                        var team2 = $(".t-team:last-child", matchContainer);
                        $(".t-players", team1).html(player1);
                        $(".t-players", team2).html(player2);
                        if (match.Player1 && match.Player2 && match.Player3) {
                            $(".t-players", team1).addClass("t-group");
                        }

                        if (match.Player3 && match.Player4) {
                            $(".t-players", team2).addClass("t-group");
                        }


                        $(".t-icon:first-child", team1).removeClass("winner");
                        $(".t-icon:first-child", team2).removeClass("winner");
                        
                        if (match.Winner == 1) {
                            $(".t-icon:first-child", team1).addClass("winner");
                        } else if (match.Winner == 2){
                            $(".t-icon:first-child", team2).addClass("winner");
                        }

                        $(".t-score", team1).text(getScore(1, match.SetScores) || "");
                        $(".t-score", team2).text(getScore(2, match.SetScores) || "");
                    }

                  

                }
            }


        });

    };


})(jQuery);