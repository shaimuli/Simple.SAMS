using SAMS.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using Simple;
namespace SAMS
{
    public class PrintManager
    {
        public string Print(CompetitionDetailsModel competition, string template)
        {

            var dataSetmanager = new DataSetManager();

            var dataTable = dataSetmanager.CreateDataSet(competition);


            var file = Updatexls(dataTable, template);


            return file;
        }



        private string Updatexls(DataTable data, string template)
        {

            var rdlcPath = template;//Server.MapPath("~/Static/Reports/TournamentTemplate.xls");

            var tempPath = System.IO.Path.GetTempFileName();

            tempPath = Path.ChangeExtension(tempPath, "xls");

            System.IO.File.Copy(rdlcPath, tempPath, true);


            string connString = "Provider=Microsoft.Jet.OleDb.4.0; data source=" + tempPath + "; Extended Properties=\"Excel 8.0;HDR=Yes\";";




            OleDbConnection conn = new OleDbConnection(connString);

            conn.Open();

            OleDbCommand cmd = new OleDbCommand();
            cmd.Connection = conn;
            OleDbDataAdapter clientsAdapter = new OleDbDataAdapter();
            var index = 0;
            foreach (DataRow item in data.Rows)
            {

                var i = item;

                cmd.Parameters.Add(new OleDbParameter("@B", item.ItemArray[0].ToString()));
                cmd.Parameters.Add(new OleDbParameter("@C", item.ItemArray[1].ToString()));
                cmd.Parameters.Add(new OleDbParameter("@D", item.ItemArray[2].ToString()));
                cmd.Parameters.Add(new OleDbParameter("@E", item.ItemArray[3].ToString()));
                cmd.Parameters.Add(new OleDbParameter("@F", item.ItemArray[4].ToString()));
                cmd.Parameters.Add(new OleDbParameter("@G", item.ItemArray[5].ToString()));
                cmd.Parameters.Add(new OleDbParameter("@H", item.ItemArray[6].ToString()));
                cmd.Parameters.Add(new OleDbParameter("@K", item.ItemArray[7].ToString()));
                cmd.Parameters.Add(new OleDbParameter("@L", item.ItemArray[8].ToString()));

                cmd.Parameters.Add(new OleDbParameter("@id", index.ToString()));
                index++;
                var sql = @"UPDATE [Sheet1$] SET B=@B, C=@C,D=@D,E=@E,F=@F,G=@G,H=@H,K=@K,L=@L where id =@id";



                //  var sql = @"Select * from [Sheet1$] ";


                cmd.CommandText = sql;

                var result = cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
            }

            //  clientsAdapter.UpdateCommand = cmd;

            //   clientsAdapter.Update(data);

            conn.Close();





            return tempPath;
        }
    }


    public class DataSetManager
    {


        public DataTable CreateDataSet(CompetitionDetailsModel competitionDetailsModel)
        {
            return CreatePrintDataSet(competitionDetailsModel);
        }




        private DataRow CreateRow(MatchPlayerViewModel player, DataTable dataTable, string index, string ClumeName)
        {
            var row = dataTable.NewRow();
            if (player != null)
            {
                row[ClumeName] = index + " " + player.FullName;
            }
            else
            {
                row[ClumeName] = index;
            }
            m_TabaleDictionary.Add(DictionaryIndex, row);
            DictionaryIndex++;
            return row;
        }


        private void UpdateRowByIndex(int index, string columName, string value)
        {
            var row = m_TabaleDictionary[index];
            row[columName] = value;
        }

        private Dictionary<int, DataRow> m_TabaleDictionary;
        private int DictionaryIndex = 1;
        private int AddMatchRoundOneToTable(CompetitionMatchViewModel match, DataTable dataTable, int index)
        {

            if (match != null && match.Round == 1)
            {
                dataTable.Rows.Add(CreateRow(null, dataTable, string.Empty, "A"));
                dataTable.Rows.Add(CreateRow(match.Player1, dataTable, index.ToString(), "A"));
                index++;
                dataTable.Rows.Add(CreateRow(null, dataTable, match.StartTime.HasValue ? match.StartTime.Value.ToString() : string.Empty, "A"));
                dataTable.Rows.Add(CreateRow(match.Player2, dataTable, index.ToString(), "A"));
                index++;
            }
            return index;

        }


        private int AddMatch(CompetitionMatchViewModel match, int index, string cloumName, int IndexJump)
        {
            if (match.Player1 != null)
                UpdateRowByIndex(index, cloumName, match.Player1.FullName);
            index = index + IndexJump;
            UpdateRowByIndex(index, cloumName, match.StartTime.HasValue ? match.StartTime.Value.ToString() : string.Empty);
            index = index + IndexJump;
            if (match.Player2 != null)
                UpdateRowByIndex(index, cloumName, match.Player2.FullName);
            index = index + IndexJump + IndexJump;
            return index;
        }


        private int AddMathRound(CompetitionMatchViewModel match, string cloumName, int startIndex, int indexJump)
        {
            startIndex = AddMatch(match, startIndex, cloumName, indexJump);
            return startIndex;
        }

        private DataTable CreatePrintDataSet(CompetitionDetailsModel competitionDetailsModel)
        {
            m_TabaleDictionary = new Dictionary<int, DataRow>();
            DataTable dataTable = new DataTable("Comp");
            dataTable.Columns.Add("A");
            dataTable.Columns.Add("B");
            dataTable.Columns.Add("C");
            dataTable.Columns.Add("D");
            dataTable.Columns.Add("E");
            dataTable.Columns.Add("F");
            dataTable.Columns.Add("G");
            dataTable.Columns.Add("H");
            dataTable.Columns.Add("I");


            var model = competitionDetailsModel;

            var index = 1;

            model.Matches.ForEach(match =>
            {
                index = AddMatchRoundOneToTable(match, dataTable, index);
            });

            int jumpIndexRound1 = 2;
            int jumpIndexRound2 = 4;
            int jumpIndexRound3 = 9;
            int jumpIndexRound4 = 15;


            int startIndexRoond1 = 3;
            int startIndexRoond2 = 5;
            int startIndexRoond3 = 9;
            int startIndexRoond4 = 20;




            model.Matches.ForEach(match =>
            {
                if (match != null)
                {
                    if (match.Round == 2)
                    {
                        startIndexRoond1 = AddMatch(match, startIndexRoond1, "C", jumpIndexRound1);
                    }

                    if (match.Round == 3)
                    {
                        startIndexRoond2 = AddMatch(match, startIndexRoond2, "E", jumpIndexRound2);
                    }

                    if (match.Round == 4)
                    {
                        startIndexRoond3 = AddMatch(match, startIndexRoond3, "G", jumpIndexRound3);
                    }
                    if (match.Round == 5)
                    {
                        startIndexRoond4 = AddMatch(match, startIndexRoond4, "H", jumpIndexRound4);
                    }
                }


            });
            return dataTable;
        }

    }
}