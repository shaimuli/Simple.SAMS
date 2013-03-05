(function ($) {
    var matchesPerRound = [32, 16, 8, 4, 2, 2];
    function prepareTournament(host, resources, maxRound) {
        var container = $("<div/>").addClass("t-container").appendTo(host);
        var rounds = 6;
        
        
        host.addClass("ms" + matchesPerRound[6 - rounds]);
        function addTeam(toContainer, index) {
            var t = $("<div/>").addClass("t-team").appendTo(toContainer);
            $("<div/>").addClass("t-icon").appendTo(t);

            var ps = $("<div/>").addClass("t-players").appendTo(t);
            var sc = $("<div/>").addClass("t-score").appendTo(t);

            if (index == 0) {
                $("<div/>").addClass("t-icon t-stats").appendTo(t);
            } else {
                $("<div/>").addClass("t-icon t-scorecard").appendTo(t);
            }

            return t;
        }
        function roundName(round) {
            var result = resources["Round"] + " " + round;
            if (round == 3) {
                result = resources["QuarterFinal"];
            } else if (round == 4) {
                result = resources["SemiFinal"];
            } else if (round == 5) {
                result = resources["Final"];

            } else if (round == 6) {
                result = resources["ThirdPlace"];
            }
            return result;
        }
        var matchPosition = 0;
        for (var r = 0; r < maxRound ; r++) {
            var roundContainer = $("<div/>").addClass("t-round r" + r).appendTo(container);

            $("<h3/>").text(roundName(r)).appendTo(roundContainer);
            var sep = r < rounds - 1 ? $("<div/>").addClass("t-sep r" + r).appendTo(container) : null;
            var matchesCount = matchesPerRound[r];
            for (var m = 1; m <= matchesCount; m++) {
                if (m == matchesCount && r == (rounds - 1)) {
                    $("<h3/>").text(roundName(6)).appendTo(roundContainer);
                }
                var matchContainer = $("<div/>").addClass("t-match m" + m).attr("id", "match" + (matchPosition++)).appendTo(roundContainer);

                var team1 = addTeam(matchContainer,0);
                var team2 = addTeam(matchContainer,1);
                if (sep && r < (maxRound-1)) {
                    if (m % 2 == 0) {
                        var msep = $("<div/>").addClass("t-msep").appendTo(sep);
                        $("<div/>").addClass("t-msep-right").appendTo(msep);
                        $("<div/>").addClass("t-msep-left").appendTo(msep);
                    }
                }
            }
        }
        

    }
    $.fn.tournament = function (options) {
        return this.each(function () {
            var target = $(this);
            var rounds = target.data("rounds");
            var playersCount = target.data("players-count");
            var maxMatches = target.data("matches");
            var startRound = matchesPerRound.indexOf(playersCount / 2);
            var maxRound = startRound + rounds;
            /*for (var i = startRound + rounds; i <= 5; i++) {
                $("div.r" + (i - 1) + ".t-sep", target).hide();
                $("div.r" + i, target).hide();
            }*/

            if (!target.data("tournament-attached")) {
                target.data("tournament-attached", true);
                target.html("");
                prepareTournament(target, options.resources, maxRound);
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
                        var text = (p.Rank ? "(" + p.Rank + ") " : "") + p.LocalFirstName + (p.LocalLastName ? " " + p.LocalLastName : "") ;
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

                    var offset = matchesPerRound.sum(function (index, value) {
                        return (index < startRound) ? value : 0;
                    });
                    

                    for (var i = 0; i < matches.length; i++) {
                        var matchId = "#match" + String(i + offset);

                        var matchContainer = $(matchId, target);
                        var match = matches[i];

                        /*match.Player3 = match.Player2;
                        match.Player4 = match.Player2;*/
                        var player1 = players(match.Player1, match.Player3, match.Player1, match.Player2);
                        var player2 = players(match.Player2, match.Player3, match.Player3, match.Player4);
                        matchContainer.attr("title", match.RoundRelativePosition + ", " + match.Position);
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
                        } else if (match.Winner == 2) {
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