/****** Script for SelectTopNRows command from SSMS  ******/
SELECT
Id,
[Round],
Position,
RoundRelativePosition,
Player1,
Player2
  FROM [SimpleITASAMS].[dbo].[Match]
  where competitionid=2305
  and sectionid=3

  --select * from match where CompetitionId =2287 and id>=14752
  --update [SimpleITASAMS].[dbo].[Match] set Winner=NULL, Result = NULL where CompetitionId =2287 and sectionid=3 and position>=16
  --update [SimpleITASAMS].[dbo].[Match] set Player1 = nULL, Player2 = NULL, Winner=NULL, Result = NULL where id=14867 -- position >= 8 and CompetitionId =2287 and sectionid=3