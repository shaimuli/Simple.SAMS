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
            
            $(".t-host").each(function () {
                var target = $(this);
                var section = target.data("section");
                $.getJSON(
                    url , { section: section }, function (items) {
                        target.tournament(items);
                    });
            });
        },
        initTournament: function () {
            $(".t-host").tournament({ resources: this.config.resources });
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
            var model = this.config.mdel;
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
                html.push(this.config.tournamentPrinntCss);
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
                html.push($(".t-host").html());
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
                onSuccessfullUpdate: _.bind(this.updateMatchesResults, this)
            };

            this.matchesEditor = new Simple.MatchResultsContinuesEdit(matchesContinuesEditConfig);
            this.initFilters($(".competitionUpdateResultsContainer"), function (container,value) {
                return $(".matchRow:has(" + "input[name='Date'][value='" + value + "']" + ")", container);
            });
            this.initFilters($(".competitionMatchesList"), function (container,value) {
                return $("tr", container).filter(function (index) {
                    var text = $(".matchDate", this).text();
                     return  text == value;
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
        init: function(config) {
            this.config = config;

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
                    this[config.actionName](idNumber, parameters);
                }, this),
                applyTemplate: function (dialogContainer) {
                    $("<span/>").text(config.text).appendTo(dialogContainer);
                    $("<hr class='space'/>").appendTo(dialogContainer);
                    var row = $("<div class='row-fluid'/>").appendTo(dialogContainer);
                    $("<div class='span3'/>").appendTo(row);
                    var host = $("<div class='span6'/>").appendTo(row);
                    var idNumberInput = $("<input type='text' name='" + config.idNumberName + "' maxlength='9'/>").appendTo($("<div/>").appendTo(host));
                    $("<hr class='space'/>").appendTo(dialogContainer);
                    idNumberInput.keypress(function (event) {
                        var result = S.Utils.isNumberKey(event);
                        return result;
                    });
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
                dialogName: "AddPlayerToSectionDialog",
                actionName: "AddPlayerToSection"
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
                actionName: "replacePlayer"
            };
            this.initPlayerDialog(config);
            $("a.ReplaceCompetitionPlayer").click(_.bind(this.onReplaceCompetitionPlayer, this));
        },
        addPlayer: function (idNumber) {
            var competitionId = this.getCompetitionId();
            var playerFound = _.bind(function (result) {
                if (result) {
                    $.ajax({
                        url: this.config.addPlayerUrl,
                        type: "POST",
                        data: { competitionId: competitionId, playerId: result },
                        success: function () {
                            location.reload();
                        },
                        failure: _.bind(function () {
                            console.log("failure: ");
                        }, this)
                    });
                } else {
                    location.href = this.config.createPlayerUrl + "?competitionId=" + String(competitionId) + "&idNumber=" + String(idNumber);
                }
            }, this);
            $.getJSON(this.config.getPlayerIdByIdNumberUrl, playerFound);
        },
        replacePlayer: function(idNumber, replacedPlayerId) {
            var competitionId = this.getCompetitionId();
            var playerFound = _.bind(function (result) {
                if (result) {
                    $.ajax({
                        url: this.config.replacePlayerUrl,
                        type: "POST",
                        data: { competitionId: competitionId, replacedPlayerId: replacedPlayerId, replacementPlayerId: result },
                        success: function () {
                            location.reload();
                        },
                        failure: _.bind(function () {
                            console.log("failure: ");
                        }, this)
                    });
                } else {
                    location.href = this.config.createPlayerUrl + "?competitionId=" + String(competitionId) + "&replacePlayerId=" + String(replacedPlayerId) +"&idNumber=" + String(idNumber);
                }
            }, this);
            $.getJSON(this.config.getPlayerIdByIdNumberUrl, playerFound);
        },
        initRemovePlayerDialog: function () {
            var dialogConfig = {
                id: "confirmRemoveCompetitionPlayer",
                onConfirm: _.bind(function (parameters,dialogContainer) {
                    this.removeCompetitionPlayer(parameters, dialogContainer);
                }, this),
                defaultIsCancel: true,
                text: this.config.resources.RemoveCompetitionPlayerConfirmText,
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
            $.ajax({
                url: this.config.removeCompetitionPlayerUrl,
                type: "POST",
                data: { competitionId: this.getCompetitionId(), playerId: playerId },
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