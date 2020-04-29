using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamplesNet
{
   public class BulkLoadCSV
    {
        private string filePath;
        private bool firstRowContainsFieldNames = false;
        ////Read CsvFile
        private DataTable ReadCSV()
        {
            DataTable result = new DataTable();

            if (filePath == "")
            {
                return result;
            }

            string delimiters = ",";
            string extension = Path.GetExtension(filePath);

            if (extension.ToLower() == "txt")
                delimiters = "\t";
            else if (extension.ToLower() == "csv")
                delimiters = ",";

            using (TextFieldParser tfp = new TextFieldParser(filePath))
            {
                tfp.SetDelimiters(delimiters);


                if (!tfp.EndOfData)
                {
                    string[] fields = tfp.ReadFields();

                    for (int i = 0; i < fields.Count(); i++)
                    {
                        if (firstRowContainsFieldNames)
                            result.Columns.Add(fields[i]);
                        else
                            result.Columns.Add("Col" + i);
                    }


                    if (!firstRowContainsFieldNames)
                        result.Rows.Add(fields);
                }


                while (!tfp.EndOfData)
                    result.Rows.Add(tfp.ReadFields());
            }

            return result;
        }

        public void InsertDataToReconTable()
        {

            var dt = ReadCSV();
            var _conn = new SqlConnection();
            try
            {
                SqlCommand cmd = new SqlCommand("Delete  From <Your Table> ", _conn);


                _conn.Open();
                cmd.ExecuteNonQuery();
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(_conn))
                {
                    bulkCopy.DestinationTableName = "<Your Table>";
                    bulkCopy.WriteToServer(dt);
                }
                cmd.CommandText = "UPDATE  <Your Table>t  SET Field1= NULL WHERE Field2 = 'NULL'";
                cmd.ExecuteNonQuery();
            }
            finally
            {
                _conn.Close();
            }

        }

    }
}
