(function ($) {

    $.fn.grid = function (options) {
        return this.each(function() {
            var target = $(this);
            
       
            var rounds = target.data("rounds");
            var maxMatches = target.data("matches");
            if (!target.data("grid-attached")) {
                target.data("grid-attached", true);
                target.html("");
                var container = $("<div/>").addClass("matchesGrid m" + maxMatches + " r" + rounds).appendTo(target);
            for (var r = 0; r < rounds; r++) {
                var roundContainer = $("<div/>").addClass("round m" + maxMatches).appendTo(container);
                var matchesContainer = $("<div/>").addClass("matches m" + maxMatches).appendTo(roundContainer);
                for (var i = 0; i < maxMatches; i++) {
                    var matchSpot = $("<div/>").addClass("matchSpot").attr("id","s"+ target.data("section") + "r" + r + "m" + i).appendTo(matchesContainer);
                }
            }
        } else {
            if ($.isArray(options)) {
                
                var mc = options.length;
                var lastRound = 1;
                var p = 0;
                var xy = [];
                for (var m = 0; m < mc; m++) {
                    var match = options[m];
                    var rnd = match.Round;
                    
                    if (lastRound != rnd) {
                        p = 0;
                    }
                    lastRound = rnd;
                    xy[rnd - 1] = xy[rnd - 1] || [];
                    xy[rnd - 1][p] = match;
                    p++;
                }
                for (var x = 0; x < xy.length; x++) {
                    var xl = xy[x].length;
                    var offset = (x > 0) ? ((mc/2) - xl) / 2 : 0;
                    
                    for (var y = 0; y < xl; y++) {
                        var match = xy[x][y];
                        
                        var realOffset = offset;
                        if (y < xl / 2) {
                            realOffset -= Math.floor(offset / 2);
                        } else {
                            realOffset += Math.floor(offset / 2);
                        }
                        var p = realOffset + y;
                        var spotId = "#s" + target.data("section") + "r" + x + "m" + p;
                        var spot = $(spotId).addClass("filled");
                        var player1 = $("<div/>").addClass("players").appendTo(spot);
                        var player2 = $("<div/>").addClass("players").appendTo(spot);
                        if (match.Player1Won) {
                            player1.addClass("winner");
                        } else {
                            player2.addClass("winner");
                        }
                        function player(host, players, score) {
                            var pHost = $("<div/>").addClass("player").appendTo(host);
                            if ($.isArray(players)) {
                            } else {
                                $("<a/>").attr("href", "#").text((players.LocalLastName || "") + " " + players.LocalFirstName).appendTo(pHost);
                            }
                            $("<div/>").addClass("score").text(score).appendTo(host);
                        }
                        
                        function getScore(playerNumber, setScores) {
                            var scores = [];
                            for (var setNumber = 0; setNumber < setScores.length; setNumber++) {
                                var setPoints = setScores[setNumber];
                                scores.push(setPoints["Player" + playerNumber + "Points"] || 0);
                            }
                            return scores.join(" ");
                        }

                        if (match.Player3 || match.Player4) {
                            
                        } else {
                            if (match.Player1) {
                                
                                player(player1, match.Player1, getScore(1, match.SetScores));
                            }
                            
                            if (match.Player2) {
                                player(player2, match.Player2, getScore(2, match.SetScores));
                            }
                        }
                        
                        
                    }
                }

            }
        }


        });

    };


})(jQuery);