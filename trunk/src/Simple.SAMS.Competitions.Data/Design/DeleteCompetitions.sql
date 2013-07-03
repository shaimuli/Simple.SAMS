CREATE PROCEDURE ClearCompetitions
AS
delete from matchscore
delete from match
delete from CompetitionPlayer
delete from player
delete from competition

GO
exec ClearCompetitions