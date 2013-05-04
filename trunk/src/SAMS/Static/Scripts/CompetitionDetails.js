(function ($, S) {
    S.Dialog = Class.extend({
        init: function (config) {
            this.config = config;

            this.dialogContainer = $("div#" + config.id);
            if (this.dialogContainer.length == 0) {
                this.dialogContainer = $("<div id='" + config.id + "'/>");
            }
            var buttons = [
                {
                    text: this.config.resources.Cancel,
                    click: _.bind(function () {
                        this.hide();
                        if (this.config.onCancel) {
                            this.config.onCancel(this.dialogContainer);
                        }
                    }, this),
                    attr: { "class": "btn " + (this.config.defaultIsCancel ? "btn-primary":"") }
                },
                {
                    text: this.config.resources.Confirm,
                    click: _.bind(this.approve, this),
                    attr: { "class": "btn " + (this.config.defaultIsCancel ? "" : "btn-primary") }
                }
            ];


            this.dialogContainer.dialog({
                title: this.config.resources.Title,
                buttons: buttons,
                autoOpen: false,
                modal: true,
                resizable: false,
                width: this.config.width || 500,
                height: this.config.height,
                onClose: _.bind(function () {
                    delete this.parameters;
                }, this)
            });
        },
        approve: function () {
            
            if (this.config.onConfirming) {
                if (!this.config.onConfirming(this.parameters, this.dialogContainer)) {
                    return;
                }
            }
            
            this.hide();
            
            if (this.config.onConfirm) {
                this.config.onConfirm(this.parameters, this.dialogContainer);
            }
        },
        show: function (parameters) {
            this.applyTemplate();
            this.parameters = parameters;
            this.dialogContainer.dialog("open");
        },
        applyTemplate: function () {
            this.dialogContainer.html("");
            if (this.config.applyTemplate) {
                this.config.applyTemplate(this.dialogContainer);
            }
            else {
                this.dialogContainer.text(this.config.text);
            }
        },
        hide: function () {
            this.dialogContainer.dialog("close");
        }

    });
    
    S.CompetitionDetails = Class.extend({
        updateMatchesResults: function() {
            var competitionId = this.getCompetitionId();
            var url = this.config.getMatchesUrl + "/" + competitionId;
            var self = this;
            $(".t-host").each(function () {
                var target = $(this);
                var section = target.data("section");
                $.getJSON(
                    url, { section: section }, function (items) {
                        target.tournament(items);
                        self.updateMatchesList(items);
                    });
            });
            
        },
        initTournament: function () {
            $(".t-host").tournament({ resources: this.config.resources, links:this.config.links });
            $(".t-host").overscroll();
            this.updateMatchesResults();
            /*
            var items = [];
            var itemsJson = $("#matchesInfo").val();
            if (itemsJson && itemsJson.length) {
                items = JSON.parse(itemsJson);
            }
            $(".t-host").tournament(items);*/
        },
        initPrint: function () {
            var model = this.config.model;
            var tournamentPrinntCss = this.config.tournamentPrinntCss;
            function addPrintPageHeaders(html) {
                html.push("<h1>");
                html.push(model.Name);
                html.push("</h1>");
                html.push("<h3>");
                html.push(model.Type.Name);
                html.push("</h3>");

                html.push("<i>");
                html.push(model.StartTime);
                html.push(model.EndTime);
                html.push("</i>");


            }

            function printPage(content) {
                var html = ["<html><head>"];
                html.push("<title>");
                html.push(model.Name);
                html.push("</title>");
                html.push("<link type='text/css' rel='stylesheet' href='");
                html.push(tournamentPrinntCss);
                html.push("'/></head><body class='t-print rtl'>");
                addPrintPageHeaders(html);
                html.push(content);
                html.push("</body></html>");

                var win = window.open();

                win.document.write(html.join(""));

            }

            $(".PrintMatchesList").click(function () {
                var html = [];
                html.push("<style text='text/css'>table{ width: 100%; border:solid 1px black; border-collapse: collapse; } table td{ border: solid 1px black; } table tr{ vertical-align: top; } .span2{ display:inline-block; width: 24px;}</style>");
                html.push($(".competitionMatchesList").html());
                printPage(html.join(""));
            });

            $(".PrintPlayersList").click(function () {
                var html = [];
                html.push("<style text='text/css'>table{ width: 100%; border:solid 1px black; border-collapse: collapse; } table td{ border: solid 1px black; } table tr{ vertical-align: top; }</style>");
                html.push($(".competitionPlayersContainer").html());
                printPage(html.join(""));
            });

            $(".PrintMatchesDraw").click(function () {
                var html = [];
                html.push("<div class='t-host t-@(Model.Type.PlayersCount)p'>");
                html.push($("#section-Final .t-host").html());
                html.push("</div>");
                printPage(html.join(""));
            });

        },
        initMatchesEdit: function () {
            var matchesContinuesEditConfig = {
                containerSelector: ".competitionUpdateResultsContainer",
                logicalItemContainerSelector: "tbody tr",
                storageRootKey: "Matches:Results",
                sendUrl: this.config.sendUpdatesUrl,
                resources: this.config.resources,
                autoCommit: false,
                onSuccessfullUpdate: _.bind(function() {
                    $(".competition_status").hide();
                    this.updateMatchesResults();
                }, this)
            };

            this.matchesEditor = new Simple.MatchResultsContinuesEdit(matchesContinuesEditConfig);
            this.initFilters($(".competitionUpdateResultsContainer"), function (container,value) {
                return $(".matchRow:has(" + "input[name='Date'][value='" + value.trim() + "']" + ")", container);
            });
            this.initFilters($(".competitionMatchesList"), function (container,value) {
                return $("tr", container).filter(function (index) {
                    var text = $(".matchDate", this).text();
                     return  text.trim() == value.trim();
                 });
            });
        },
        initFilters:function (container, filter) {
            $("select[name=Round]", container).change(function (event) {
                var round = $(this).val();

                if (round == "0") {
                    $(".roundContainer", container).show();
                } else {
                    $(".roundContainer", container).hide();
                    $(".roundContainer[data-key='" + round + "']", container).show();
                }
            });
            $("select[name=Section]", container).change(function (event) {
                var section = $(this).val();

                if (section == "0") {
                    $(".sectionContainer").show();
                } else {
                    $(".sectionContainer").hide();
                    $(".sectionContainer[data-key='" + section + "']", container).show();
                }
            });

            $("input[name='DateFilter']", container)
                .keyup(function (event) {
                    if ($(this).val() == "") {
                        $(this).change();
                    }
                })
                .change(function (event) {
                    var value = $(this).val();
                    console.log(value);
                    if (value.length > 0) {
                        $(".matchRow", container).hide();
                        var items = filter(container, value);
                        
                        items.show();
                        
                    } else {
                        $(".matchRow", container).show();
                    }
                });
        },
        initCompetitionPlayers: function () {

            $("input[name='NameFilter']", ".competitionPlayersContainer")
                .keyup(function (event) {
                    if ($(this).val() == "" || event.keyCode == 13) {
                        $(this).change();
                    }
                })
                .change(function (event) {
                    var value = $(this).val();

                    var players = $(".item.player", ".competitionPlayersContainer");
                    $.each(players,
                        function (index, player) {
                            player = $(player);
                            var texts = [$("td:first-child", player).text(), $("td:nth-child(2)", player).text()];
                            if (texts[0].indexOf(value) >= 0 ||
                                texts[1].indexOf(value) >= 0) {
                                player.show();
                            } else {
                                player.hide();
                            }
                        });
                });
        },
        updateMatchesList: function (items) {
            var config = this.config;
            var matchesMap = {};
            for (var i = 0; i < items.length; i++) {
                matchesMap[items[i].Id] = items[i];
            }
            $(".matchesListContainer tbody tr.matchRow").each(function() {
                var tds = $(this).children("td");
                var id = $(this).attr("data-key");
                var match = matchesMap[id];

                function playerName(p) {
                    var text = (p.Rank ? "(" + p.Rank + ") " : "") + p.LocalFirstName + (p.LocalLastName ? " " + p.LocalLastName : "");
                    var html = "<a href='#' data-id='" + p.Id + "'>" + text + "</a>";

                    return html;
                }
                function getScore(playerNumber, setScores) {
                    var scores = [];
                    for (var setNumber = 0; setNumber < setScores.length; setNumber++) {
                        var setPoints = setScores[setNumber];
                        if (playerNumber == "BreakPoints") {
                            scores.push(setPoints["BreakPoints"] || 0);
                        } else {
                            scores.push(setPoints["Player" + playerNumber + "Points"] || 0);
                        }
                    }
                    return scores.join(" ");
                }
                
                if (match) {
                    if (match.StartTime) {
                        var startTime = match.StartTime.toDate();
                        $(tds[1]).text(startTime.toString("dd/MM/yyyy"));
                        $(tds[2]).text(startTime.toString("HH:mm") + " " + config.startTimeType[match.StartTimeType]);
                    }
                    //for (var i = 0; i < tds.length; i++) {
                    //    $(tds[i]).text(i);
                    //}
                    if (match.Id == 3515) {
                        console.log("M", match, config.startTimeType[match.StartTimeType]);
                    }
                    var scoresHtml = [];
                    scoresHtml.push(getScore(1, match.SetScores));
                    scoresHtml.push(getScore(2, match.SetScores));
                    scoresHtml.push(getScore("BreakPoints", match.SetScores));
                    $(tds[4]).html(scoresHtml.join("<br/>"));
                    if (typeof match.Result !== "undefined" && match.Result != null) {
                        var winner = match.Winner == 0 ? match.Player1 : match.Player2;
                        if (winner) {
                            winner = playerName(winner);
                        } else {
                            winner = "";
                        }
                        $(tds[5]).html(winner);
                        $(tds[6]).text(config.matchResult[match.Result]);
                        
                    }
                    $(tds[7]).text(config.matchStatus[match.Status]);
                    
                }
            });
        },
        init: function(config) {
            this.config = config;
            
            this.initPrint();
            this.initMatchesEdit();
            this.initCompetitionPlayers();
            this.initTournament();
            this.initRemovePlayerDialog();
            this.initReplacePlayerDialog();
            this.initAddPlayerDialog();
            
        },
        initPlayerDialog: function (config) {
            
            var dialogConfig = {
                id: config.id,
                onConfirming: _.bind(function (parameters, dialogContainer) {
                    var idNumberInput = $("input[name='" + config.idNumberName + "']", dialogContainer);
                    var idNumber = idNumberInput.val();
                    var result = true;
                    if (idNumber.length < 9) {
                        idNumberInput.addClass("error");
                        result = false;
                    } else {
                        idNumberInput.removeClass("error");
                    }

                    return result;
                }, this),
                onConfirm: _.bind(function (parameters, dialogContainer) {
                    var idNumber = $("input[name='" + config.idNumberName + "']", dialogContainer).val();
                    var source = $("select[name='source']", dialogContainer).val();
                    var section = $("select[name='section']", dialogContainer).val();
                    var status = $("select[name='status']", dialogContainer).val();
                    var reason = $("input[name='reason']", dialogContainer).val();
                    this[config.actionName](idNumber, parameters, source, section, status, reason);
                }, this),
                applyTemplate: function (dialogContainer) {
                    $("<span/>").text(config.text).appendTo(dialogContainer);
                    $("<hr class='space'/>").appendTo(dialogContainer);
                    var form = $("<form/>").appendTo(dialogContainer);
                    //var row = $("<div class='row-fluid'/>").appendTo(form);
                    //$("<div class='span3'/>").appendTo(row);
                    //$("<div class='span6'/>").appendTo(row);

                    var host = $("<div class='control-group'/>").appendTo(form);
                    $("<label/>").text(config.resources.IdNumber).appendTo(host);
                    host = $("<div class='controls'/>").appendTo(host);
                    var idNumberInput = $("<input type='text' name='" + config.idNumberName + "' maxlength='9'/>").appendTo($("<div/>").appendTo(host));
                    //$("<hr class='space'/>").appendTo(dialogContainer);
                    idNumberInput.keypress(function (event) {
                        var result = S.Utils.isNumberKey(event);
                        return result;
                    });
                    
                    host = $("<div class='control-group'/>").appendTo(form);
                    $("<label/>").text(config.resources.CompetitionPlayerSource).appendTo(host);
                    host = $("<div class='controls'/>").appendTo(host);
                    var sourceSelect = $("<select/>").attr("name", "source").appendTo(host);
                    $.each(config.competitionPlayerSources, function (index, item) {
                        $("<option/>").val(index).text(item).appendTo(sourceSelect);
                    });
                    host = $("<div class='control-group'/>").appendTo(form);
                    var hasSections = false;
                    var sectionSelect = $("<select/>").attr("name", "section");
                    if (config.canAddToFinal) {
                        $("<option/>").val(1).text(config.competitionSections[1]).appendTo(sectionSelect);
                        hasSections = true;
                    }

                    if (config.canAddToQualifying) {
                        $("<option/>").val(2).text(config.competitionSections[2]).appendTo(sectionSelect);
                        hasSections = true;
                    }
                    if (hasSections) {
                        $("<label/>").text(config.resources.CompetitionSection).appendTo(host);
                        host = $("<div class='controls'/>").appendTo(host);
                        sectionSelect.appendTo(host);
                    }
                    
                    if (config.removeStatus) {
                        host = $("<div class='control-group'/>").appendTo(form);
                        $("<label/>").text(config.resources.CompetitionPlayerStatus).appendTo(host);
                        var removeStatusSelect = $("<select/>").attr("name", "status").appendTo(host);
                        $.each(config.removeStatus, function(index, item) {
                            $("<option/>").val(index).text(item).appendTo(removeStatusSelect);
                        });
                        host = $("<div class='control-group'/>").appendTo(form);
                        $("<label/>").text(config.resources.CompetitionPlayerStatusRemove).appendTo(host);
                        $("<input/>").attr("name", "reason").appendTo(host);
                    }
                    
                },
                resources: {
                    Cancel: this.config.resources.Cancel,
                    Confirm: config.confirmText,
                    Title: config.titleText
                }
            };

            this[config.dialogName] = new S.Dialog(dialogConfig);
        },
        initAddPlayerDialog: function () {
            var config = {
                id: "confirmAddCompetitionPlayer",
                text: this.config.resources.AddCompetitionPlayerConfirmText,
                idNumberName: "AddPlayerIdNumber",
                confirmText: this.config.resources.AddCompetitionPlayerConfirm,
                titleText: this.config.resources.AddCompetitionPlayerConfirmTitle,
                dialogName: "addPlayerDialog",
                actionName: "addPlayer",
                competitionPlayerSources: this.config.competitionPlayerSources,
                competitionSections: this.config.competitionSections,
                canAddToFinal: this.config.canAddToFinal,
                canAddToQualifying: this.config.canAddToQualifying,
                resources: this.config.resources
            };
            this.initPlayerDialog(config);
            $(".AddCompetitionPlayer").click(_.bind(this.onAddCompetitionPlayer, this));
        },
        initReplacePlayerDialog: function () {
            var config = {
                id: "confirmReplaceCompetitionPlayer",
                text: this.config.resources.ReplaceCompetitionPlayerConfirmText,
                idNumberName: "ReplacePlayerIdNumber",
                confirmText: this.config.resources.ReplaceCompetitionPlayerConfirm,
                titleText: this.config.resources.ReplaceCompetitionPlayerConfirmTitle,
                dialogName: "replacePlayerDialog",
                actionName: "replacePlayer",
                competitionPlayerSources: this.config.competitionPlayerSources,
                competitionSections: this.config.competitionSections,
                removeStatus: this.config.removeStatus,
                canAddToFinal: false,
                canAddToQualifying: false,
                resources: this.config.resources
            };
            this.initPlayerDialog(config);
            $("a.ReplaceCompetitionPlayer").click(_.bind(this.onReplaceCompetitionPlayer, this));
        },
        addPlayer: function (idNumber,parameters, source, section) {
            var competitionId = this.getCompetitionId();
            var playerFound = _.bind(function (result) {
                if (result) {
                    $.ajax({
                        url: this.config.addPlayerUrl,
                        type: "POST",
                        data: { competitionId: competitionId, playerId: result, source: source, section: section },
                        success: function () {
                            location.reload();
                        },
                        failure: _.bind(function () {
                            console.log("failure: ");
                        }, this)
                    });
                } else {
                    location.href = this.config.createPlayerUrl + "?competitionId=" + String(competitionId) + "&idNumber=" + String(idNumber) + "&source=" + String(source) + "&section=" + String(section);
                }
            }, this);
            $.getJSON(this.config.getPlayerIdByIdNumberUrl, { idNumber: idNumber }, playerFound);
        },
        replacePlayer: function(idNumber, replacedPlayerId, source, section, status, reason) {
            var competitionId = this.getCompetitionId();
            var playerFound = _.bind(function (result) {
                if (result) {
                    $.ajax({
                        url: this.config.replacePlayerUrl,
                        type: "POST",
                        data: { competitionId: competitionId, replacedPlayerId: replacedPlayerId, replacementPlayerId: result, source: source, status: status, reason: reason },
                        success: function () {
                            location.reload();
                        },
                        failure: _.bind(function () {
                            console.log("failure: ");
                        }, this)
                    });
                } else {
                    location.href = this.config.createPlayerUrl + "?competitionId=" + String(competitionId) + "&replacePlayerId=" + String(replacedPlayerId) + "&idNumber=" + String(idNumber) + "&source=" + String(source) + "&status=" + String(status) + "&reason=" + String(reason);
                }
            }, this);
            $.getJSON(this.config.getPlayerIdByIdNumberUrl, { idNumber: idNumber }, playerFound);
        },
        initRemovePlayerDialog: function () {
            var config = this.config;
            var dialogConfig = {
                id: "confirmRemoveCompetitionPlayer",
                onConfirm: _.bind(function (parameters,dialogContainer) {
                    this.removeCompetitionPlayer(parameters, dialogContainer);
                }, this),
                defaultIsCancel: true,
                text: this.config.resources.RemoveCompetitionPlayerConfirmText,
                applyTemplate: function (dialogContainer) {
                    $("<div/>").text(config.resources.RemoveCompetitionPlayerConfirmText).appendTo(dialogContainer);
                    $("<hr class='space'/>").appendTo(dialogContainer);
                    var form = $("<form/>").appendTo(dialogContainer);
                    var host = $("<div class='control-group'/>").appendTo(form);
                    $("<label/>").text(config.resources.CompetitionPlayerStatus).appendTo(host);
                    var removeStatusSelect = $("<select/>").attr("name", "status").appendTo(host);
                    $.each(config.removeStatus, function (index, item) {
                        $("<option/>").val(index).text(item).appendTo(removeStatusSelect);
                    });
                    host = $("<div class='control-group'/>").appendTo(form);
                    $("<label/>").text(config.resources.CompetitionPlayerStatusRemove).appendTo(host);
                    $("<input/>").attr("name", "reason").appendTo(host);
                    
                },
                resources: {
                    Cancel: this.config.resources.Cancel,
                    Confirm: this.config.resources.RemoveCompetitionPlayerConfirm,
                    Title: this.config.resources.RemoveCompetitionPlayerConfirmTitle
                }
            };

            this.removePlayerDialog = new S.Dialog(dialogConfig);
            
            $("a.RemoveCompetitionPlayer").click(_.bind(this.onRemoveCompetitionPlayer, this));
        },
        getCompetitionId: function() {
            return parseInt($("input[type=hidden][name=CompetitionId]").val(), 10);
        },
        removeCompetitionPlayer: function (target, dialogContainer) {
            var playerId = parseInt(target.closest("tr").data("key"));
            var status = $("select[name=status]", dialogContainer).val();
            var reason = $("input[name=reason]", dialogContainer).val();
            $.ajax({
                url: this.config.removeCompetitionPlayerUrl,
                type: "POST",
                data: { competitionId: this.getCompetitionId(), playerId: playerId, status: status, reason: reason },
                success: function() {
                    location.reload();
                },
                failure: _.bind(function() {
                    console.log("failure: ");
                }, this)
            });
        },
        onReplaceCompetitionPlayer: function(event) {
            event.preventDefault();
            var target = $(event.target);
            var playerId = parseInt(target.closest("tr").data("key"));
            this.replacePlayerDialog.show(playerId);
        },
        onAddCompetitionPlayer: function (event) {
            event.preventDefault();
            this.addPlayerDialog.show();
        },
        onRemoveCompetitionPlayer: function(event) {
            event.preventDefault();
            var target = $(event.target);

            this.removePlayerDialog.show(target);

        }
    });
    
 
})(jQuery, Simple);