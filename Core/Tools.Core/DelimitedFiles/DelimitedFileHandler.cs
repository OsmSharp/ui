// OsmSharp - OpenStreetMap tools & library.
// Copyright (C) 2012 Abelshausen Ben
// 
// This file is part of OsmSharp.
// 
// OsmSharp is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 2 of the License, or
// (at your option) any later version.
// 
// OsmSharp is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OsmSharp. If not, see <http://www.gnu.org/licenses/>.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using Tools.Core.Progress;

namespace Tools.Core.DelimitedFiles
{
    /// <summary>
    /// Handles common delimited file functions.
    /// </summary>
    public static class DelimitedFileHandler
    {
        public static DataSet ReadDelimitedFile(
            IProgressReporter reporter, 
            FileInfo file, 
            DelimiterType delimiter, 
            bool firstRowHasHeaders,
            bool ignoreHeader)
        {
            if (reporter == null)
            {
                reporter = EmptyProgressReporter.Instance;
            }

            char delimiterChar = DelimitedFileHandler.GetDelimiterChar(delimiter);

            DataSet delimited_data_set = null;
            DataTable DelimitedDataTable = null;
            DataRow DelimitedDataRow = null;
            int iCounter = 0;
            ProgressStatus status;
            if (!file.Exists) 
            {
	            throw new FileNotFoundException(string.Format("Input file {0} not found!", file.FullName));
            } 
            else 
            {
	            // Build dataset
	            delimited_data_set = new DataSet();
	            delimited_data_set.DataSetName = file.Name;
	            //Set the DataSet name
	            delimited_data_set.Namespace = file.Name;
	            //Set the XML Document NameSpace
	            delimited_data_set.Prefix = "";
	            //Set the prefix (Optional)
	            delimited_data_set.Tables.Add(file.Name);
	            //Add the table               'Read the delimited file

	            System.Text.Encoding enc = null;
	            enc = System.Text.Encoding.GetEncoding(1252);
	            FileStream fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
	            StringBuilder strBuild = new StringBuilder(Convert.ToInt32(fileStream.Length));
                
                // report the status.
                status = new ProgressStatus();
                status.Status = ProgressStatus.ProgressStatusEnum.Busy;
                status.CurrentNumber = 0;
                status.Message = "Opening file...";
     
                reporter.Report(status);

	            for (int i = 0; i <= Convert.ToInt32(fileStream.Length) - 1; i++) {
		            strBuild.Append(enc.GetString(new byte[] { Convert.ToByte(fileStream.ReadByte()) }));
	            }

	            fileStream.Close();
	            string str = strBuild.ToString();
	            StringReader strReader = new StringReader(str);
	            List<string> lines = new List<string>();
	            while ((strReader.Peek() > -1)) {
		            lines.Add(strReader.ReadLine());
	            }

	            // Add in the Header Columns if they are present.
	            if (firstRowHasHeaders & !(ignoreHeader)) 
                {
		            //Loop through header list
		            foreach (string sFields in lines[0].Split(delimiterChar)) 
                    {
			            delimited_data_set.Tables[0].Columns.Add(sFields);
		            }
	            } 
                else 
                {
                    // add a number of columns depending on the data in the first row.
                    // TODO: improve on this by adding columns on the fly.
                    for (int i = 1; i <= (lines[0].Split(delimiterChar)).Length; i++)
                    {
			            delimited_data_set.Tables[0].Columns.Add("Column" + i);
		            }
	            }

	            // Now add in the Rows
                // report the status.
                status = new ProgressStatus();
                status.Status = ProgressStatus.ProgressStatusEnum.Busy;
                status.CurrentNumber = 0;
                status.Message = "Reading file...";
                reporter.Report(status);

	            DelimitedDataTable = delimited_data_set.Tables[0];
	            int startLine = 0;
	            if (firstRowHasHeaders)
		            startLine = 1;
	            //Loop while there are rows in the delimited file
	            for (int l = startLine; l <= lines.Count - 1; l++) {
		            string line = lines[l];
		            DelimitedDataRow = DelimitedDataTable.NewRow();

		            //Add the items to the DataSet
		            foreach (string sFields in line.Split(delimiterChar)) 
                    {
			            if (DelimitedDataRow.Table.Columns.Count > iCounter) 
                        {
				            // Put data in dataset.
				            DelimitedDataRow[iCounter] = sFields;
				            iCounter += 1;
			            } 
                        else 
                        {
				            break; 
			            }
		            }
		            iCounter = 0;
		            //Add the new row to the DataSet
		            DelimitedDataTable.Rows.Add(DelimitedDataRow);

                    // report the status.
                    status = new ProgressStatus();
                    status.Status = ProgressStatus.ProgressStatusEnum.Busy;
                    status.CurrentNumber = 0;
                    status.Message = "Reading file...";
                    reporter.Report(status);
	            }
            }

            // report the status.
            status = new ProgressStatus();
            status.Status = ProgressStatus.ProgressStatusEnum.Succeeded;
            status.CurrentNumber = 0;
            status.Message = "Reading file...";
            reporter.Report(status);

            return delimited_data_set;
        }

        #region Read Delimited Files

        /// <summary>
        /// Reads a delimited file into an array of an array of strings.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="delimiter"></param>
        /// <param name="firstRowHasHeaders"></param>
        /// <param name="ignoreHeader"></param>
        /// <returns></returns>
        public static string[][] ReadDelimitedFileFromStream(
            Stream stream,
            DelimiterType delimiter,
            bool ignoreHeader)
        {
            // converts the stream into a text reader.
            TextReader tr = new StreamReader(stream);

            // get the lines.
            StringReader strReader = new StringReader(tr.ReadToEnd());
            List<string> lines = new List<string>();
            bool isheader = ignoreHeader;
            while ((strReader.Peek() > -1))
            {
                if (isheader)
                {
                    isheader = false;
                    strReader.ReadLine();
                }
                else
                {
                    lines.Add(strReader.ReadLine());
                }
            }
            
            // get the columns.
            string[][] values = new string[lines.Count][];
            char split = DelimitedFileHandler.GetDelimiterChar(delimiter);
            for (int idx = 0; idx < lines.Count; idx++)
            {
                values[idx] = lines[idx].Split(split);
            }
            return values;
        }

        /// <summary>
        /// Reads a delimited file into an array of an array of strings.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string[][] ReadDelimitedFileFromStream(
            Stream stream,
            DelimiterType delimiter)
        {
            return DelimitedFileHandler.ReadDelimitedFileFromStream(stream, delimiter, true);
        }

        #endregion

        /// <summary>
        /// Writes a delimited file using the given format.
        /// </summary>
        /// <param name="reporter"></param>
        /// <param name="data"></param>
        /// <param name="writer"></param>
        /// <param name="delimiter_type"></param>
        /// <param name="first_row_as_header"></param>
        /// <param name="format"></param>
        public static void WriteDelimitedFile(
            IProgressReporter reporter,
            DataTable data,
            TextWriter writer,
            DelimiterType delimiter_type,
            bool first_row_as_header,
            IDelimitedFormat format)
        {
            // get the delimiter character
            char delimiter = GetDelimiterChar(delimiter_type);

            // initialize the progress status.
            ProgressStatus status = new ProgressStatus();
            if (reporter != null)
            {
                status.TotalNumber = data.Rows.Count;
                status.Status = ProgressStatus.ProgressStatusEnum.Busy;
                status.CurrentNumber = 0;
                status.Message = "Creating File...";

                // report the status.
                reporter.Report(status);
            }

            // export header if needed
            if (first_row_as_header)
            {
                // loop over all columns
                for (int idx = 0; idx < data.Columns.Count; idx++)
                {
                    string name = data.Columns[idx].ColumnName;
                    if (format.DoExport(idx, name))
                    {
                        writer.Write(name);
                        // no delimiter at the end
                        if (idx < data.Columns.Count - 1)
                        {
                            writer.Write(delimiter);
                        }
                    }
                }
                writer.WriteLine();
            }

            // export data
            if (reporter != null)
            {
                status.Message = "Exporting... {progress}!";
            }
            for (int idx = 0; idx < data.Rows.Count; idx++)
            {
                for (int col_idx = 0; col_idx < data.Columns.Count; col_idx++)
                {
                    string name = data.Columns[col_idx].ColumnName;
                    if (format.DoExport(idx, name))
                    {
                        object field_data = data.Rows[idx][col_idx];
                        string field_data_string = format.ConvertValue(name, field_data);
                        writer.Write(field_data_string);
                        // no delimiter at the end
                        if (col_idx < data.Columns.Count - 1)
                        {
                            writer.Write(delimiter);
                        }
                    }
                }
                if (reporter != null)
                {
                    status.CurrentNumber = idx + 1;
                    reporter.Report(status);
                }
                writer.WriteLine();
            }

            // report done
            if (reporter != null)
            {
                status.Message = "Exporting Done!";
                status.Status = ProgressStatus.ProgressStatusEnum.Succeeded;
                reporter.Report(status);
            }
        }

        /// <summary>
        /// Returns the delimiter char for a delimiter type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static char GetDelimiterChar(DelimiterType type)
        {
            char delimiter;
            switch (type)
            {
                case DelimiterType.CommaSeperated:
                    delimiter = ',';
                    break;
                case DelimiterType.DotCommaSeperated:
                    delimiter = ';';
                    break;
                case DelimiterType.TabSeperated:
                    delimiter = (char)9;
                    break;
                default:
                    throw new NotImplementedException();
            }
            return delimiter;
        }

        /// <summary>
        /// Writes a delimited file using a default format.
        /// </summary>
        /// <param name="reporter"></param>
        /// <param name="data"></param>
        /// <param name="writer"></param>
        /// <param name="delimiter_type"></param>
        /// <param name="first_row_as_header"></param>
        public static void WriteDelimitedFile(
            IProgressReporter reporter,
            DataTable data,
            TextWriter writer,
            DelimiterType delimiter_type,
            bool first_row_as_header)
        {
            WriteDelimitedFile(reporter, data, writer, delimiter_type, first_row_as_header, new DefaultDelimitedFormat());
        }

        /// <summary>
        /// Writes a delimited file using a default format.
        /// </summary>
        /// <param name="reporter"></param>
        /// <param name="data"></param>
        /// <param name="writer"></param>
        /// <param name="delimiter_type"></param>
        /// <param name="first_row_as_header"></param>
        public static void WriteDelimitedFile(
            IProgressReporter reporter,
            DataTable data,
            FileInfo file,
            DelimiterType delimiter_type,
            bool first_row_as_header)
        {
            StreamWriter writer = new StreamWriter(file.OpenWrite());
            WriteDelimitedFile(reporter, data, writer, delimiter_type, first_row_as_header, new DefaultDelimitedFormat());
            writer.Flush();
            writer.Close();
        }
    }
}
