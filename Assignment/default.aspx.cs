using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Configuration;
using System.Globalization;



namespace Assignment
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            panelStatistic.Visible = false;
            labelStatus.Visible = false;

            DataSet DtSet = new DataSet();
            if ((fileSubmit.PostedFile != null) && (fileSubmit.PostedFile.ContentLength > 0))
            {
                string fn = System.IO.Path.GetFileName(fileSubmit.PostedFile.FileName);
                string SaveLocation = Server.MapPath("Excel upload") + "\\" + fn;

                try
                {
                    fileSubmit.PostedFile.SaveAs(SaveLocation);
                    labelStatus.Visible = true;
                    labelStatus.Text = "File has been uploaded.";
                    panelStatistic.Visible = true;
                }

                catch (Exception ex)
                {
                    panelStatistic.Visible = false;
                    Response.Write("Error: " + ex.Message);
                }

                DataTable stringData = new DataTable();


                stringData = ucitajXls(SaveLocation);

                parseColon(stringData);

                parseData(stringData);

                prikaziXls(stringData);

                transferData(stringData);

            }

        }

        //unos xls-a na server
        private DataTable ucitajXls(string path)
        {
            try
            {

                OleDbConnection MyConnection = new OleDbConnection(string.Format(ConfigurationManager.ConnectionStrings["Assignment.Properties.Settings.xlsCon"].ConnectionString, path));
                OleDbDataAdapter MyCommand = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", MyConnection);
                MyCommand.TableMappings.Add("Table", "Farming data");

                DataTable loadXls = new DataTable();
                MyCommand.Fill(loadXls);

                MyConnection.Close();

                return loadXls;

            }
            catch (Exception e)
            {
                Response.Write("Error: " + e.Message);
                return null;
            }

        }

        //prikaz u Grid view
        void prikaziXls(DataTable xlsTable)
        {


            dataGridView1.DataSource = xlsTable;
            dataGridView1.DataBind();



        }

        // procesljaj kolone
        private DataTable parseColon(DataTable rawData)
        {
            int number_of_columns = 0;

            try
            {
                number_of_columns = rawData.Columns.Count;
            }
            
            catch (Exception e)
            {
                Response.Write("Error: " + e.Message);
            }

            try
            {

                for (int i = 0; i < number_of_columns; i++)
                {
                    string find = rawData.Columns[i].ToString();
                    if (Regex.IsMatch(find, "animal life number", RegexOptions.IgnoreCase) || Regex.IsMatch(find, "milking date", RegexOptions.IgnoreCase) || Regex.IsMatch(find, "milking", RegexOptions.IgnoreCase))
                    {
                    }
                    else rawData.Columns.Remove(find);
                }
            }

            catch (Exception e2)
            {
                Response.Write("Error: " + e2.Message);
            }
            return rawData;

        }

        //castaj tipove podataka
        private DataTable parseData(DataTable myData)
        {

            int number_of_columns;
            int number_of_rows;

            try
            {
                number_of_columns = myData.Columns.Count;
                number_of_rows = myData.Rows.Count;
            }
            
            catch (Exception e)
            {
                Response.Write("Error: " + e.Message);
            }

            try
            {

                foreach (DataRow row in myData.Rows)
                {
                    foreach (DataColumn column in myData.Columns)
                    {


                        if (Regex.IsMatch(column.ColumnName, "Animal life number"))
                        {
                            try
                            {
                                row[column] = Convert.ToString(row[column]);
                            }
                            catch (Exception e1)
                            {
                                Response.Write("Error: " + e1.Message);
                            }

                        }
                        if (Regex.IsMatch(column.ColumnName, "Milking date"))
                        {
                            try
                            {
                                row[column] = Convert.ToDateTime(row[column]);

                            }

                            catch (Exception e2)
                            {
                                Response.Write("Error: " + e2.Message);
                            }

                        }
                        if (Regex.IsMatch(column.ColumnName, "milking"))
                        {
                            try
                            {
                                row[column] = Convert.ToDouble(row[column]);
                            }

                            catch (Exception e3)
                            {
                                Response.Write("Error: " + e3.Message);
                            }

                        }
                    }

                }
            }
            
            catch (Exception e4)
            {
                Response.Write("Error: " + e4.Message);
            }



            return myData;
        }

        //prijenos podataka na bazu
        void transferData(DataTable myData)
        {
            string connString = ConfigurationManager.ConnectionStrings["Assignment.Properties.Settings.myDbFarm"].ConnectionString;
            SqlConnection sqlConn = null;

            try
            {

                sqlConn = new SqlConnection(connString);
                sqlConn.Open();
            }

            catch (Exception e)
            {
                Response.Write("Error: " + e.Message);
            }


            string sqlQuery = "Select * From myDataFarm";
            SqlCommand command = new SqlCommand(sqlQuery, sqlConn);
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(command);

            DataTable processedData = new DataTable();
            processedData = procesData(myData);


            using (SqlBulkCopy bulkcopy = new SqlBulkCopy(connString))
            {
                bulkcopy.DestinationTableName = "dbo.myDataFarm";
                bulkcopy.ColumnMappings.Add(0, "AnimalLifeNumber");
                bulkcopy.ColumnMappings.Add(1, "LastMilkingDate");
                bulkcopy.ColumnMappings.Add(2, "NumberOfMilking");
                bulkcopy.ColumnMappings.Add(3, "TotalMilk");

                try
                {
                    bulkcopy.WriteToServer(processedData);
                }
                catch (Exception e)
                {
                    Response.Write("Error: " + e.Message);
                }

            }

            animalstat(sqlConn);
            avgMilkStat(sqlConn);
            totalMilkStat(sqlConn);
            prodCowStat(sqlConn);

            sqlConn.Close();
        }

        //strukturiraj podatke
        private DataTable procesData(DataTable myData)
        {
            
            int number_of_columns = myData.Columns.Count;
            int number_of_milking = number_of_columns - 2;

            DataTable procData = new DataTable();
            procData = myData.Copy();

            for (int i = procData.Columns.Count - 1; procData.Columns.Count - 1 >= 0; i--)
            {
                if (Regex.IsMatch(procData.Columns[i].ToString(), "3# milking"))
                {
                    procData.Columns.RemoveAt(4);
                    i--;
                }
                if (Regex.IsMatch(procData.Columns[i].ToString(), "2# milking"))
                {
                    procData.Columns.RemoveAt(3);
                    i--;
                }
                if (Regex.IsMatch(procData.Columns[i].ToString(), "1# milking"))
                {
                    procData.Columns.RemoveAt(2);
                    i--;
                }
                if (i == 0) break;
            }

            procData.Columns.Add("Number of Milking");
            procData.Columns.Add("Total milk");
            double milk1, milk2, milk3;
            milk1 = milk2 = milk3 = 0;

            if (myData.Columns.Count == 4)
            {
                for (int i = 0; i <= myData.Rows.Count - 1; i++)
                {
                    procData.Rows[i][2] = number_of_milking;
                    for (int j = 2; j <= myData.Columns.Count - 1; j++)
                    {
                        if (j == 2) milk1 = Convert.ToDouble(myData.Rows[i][j]);
                        if (j == 3)
                        {
                            milk2 = Convert.ToDouble(myData.Rows[i][j]);
                            procData.Rows[i][3] = milk1 + milk2;
                        }


                    }
                }

            }

            if (myData.Columns.Count == 5)
            {
                for (int i = 0; i <= myData.Rows.Count - 1; i++)
                {
                    procData.Rows[i][2] = number_of_milking;
                    for (int j = 2; j <= myData.Columns.Count - 1; j++)
                    {
                        if (j == 2) milk1 = Convert.ToDouble(myData.Rows[i][j]);
                        if (j == 3) milk2 = Convert.ToDouble(myData.Rows[i][j]);
                        if (j == 4)
                        {
                            milk3 = Convert.ToDouble(myData.Rows[i][j]);
                            procData.Rows[i][3] = milk1 + milk2 + milk3;

                        }

                    }
                }
            }

            return procData;

        }

        //SQL querry funkcije

        private void animalstat(SqlConnection myConn)
        {
            string querry = @"
                SELECT COUNT(DISTINCT m.AnimalLifeNumber) AS NumberOfAnimals
                FROM myDataFarm m";

            try
            {
                SqlCommand cmd = new SqlCommand(querry, myConn);
                int number = (int)cmd.ExecuteScalar();
                noAnimals.Text = string.Format("There are {0} animals in database.", number);
            }
            catch (Exception e)
            {
                Response.Write("Error: " + e.Message);
            }
        }


        private void avgMilkStat(SqlConnection myConn)
        {
            string querry = @"SELECT SUM(m.TotalMilk)/COUNT(DISTINCT m.AnimalLifeNumber) AS AverageMilkProductionPerCow
                            FROM myDataFarm m";

            try
            {

                SqlCommand cmd = new SqlCommand(querry, myConn);
                decimal avgMilkNumber = (decimal)cmd.ExecuteScalar();
                avgMilkLabel.Text = string.Format("Average milk per animal: {0} ", avgMilkNumber);
            }
            catch (Exception e)
            {
                Response.Write("Error: " + e.Message);
            }

        }


        private void totalMilkStat(SqlConnection myConn)
        {
            string querry = @"SELECT SUM(m.TotalMilk) AS TotalMilkProduction
                            FROM myDataFarm m";
            try
            {

                SqlCommand cmd = new SqlCommand(querry, myConn);
                decimal totalMilkNumber = (decimal)cmd.ExecuteScalar();
                totalMilkLabel.Text = string.Format("Total milk number: {0} ", totalMilkNumber);
            }
            catch (Exception e)
            {
                Response.Write("Error: " + e.Message);
            }

        }


        private void prodCowStat(SqlConnection myConn)
        {
            SqlDataAdapter sqlAdapter = null;
            string querry = @"SELECT TOP 5 m.AnimalLifeNumber
	                        , SUM(m.TotalMilk) AS MilkPerCow
                            FROM myDataFarm m
                            GROUP BY m.AnimalLifeNumber
                            ORDER BY SUM(m.TotalMilk) DESC";

            try
            {
                SqlCommand cmd = new SqlCommand(querry, myConn);
                sqlAdapter = new SqlDataAdapter(cmd);
            }
            
            catch (Exception e)
            {
                Response.Write("Error: " + e.Message);
            }

            DataTable prodCowTable = new DataTable();

            sqlAdapter.Fill(prodCowTable);
            prodCow.DataSource = prodCowTable;
            prodCow.DataBind();

        }

        //prikaz datuma u datagridu
        protected void dataGridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.Cells[1].Text.Equals("Milking date"))
            {
                //check for header
            }
            else if (e.Row.Cells[1].Text.Equals("&nbsp;"))
            {
                //check for endline
            }
            else
            {
                e.Row.Cells[1].Text = Convert.ToDateTime(e.Row.Cells[1].Text).ToShortDateString();
            }
        }


    }
}