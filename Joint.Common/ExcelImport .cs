using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Joint.Common
{
    public class ExcelImport
    {
        public List<DataTable> GetData(string filePath, int? type = null)
        {
            HSSFWorkbook workbook;
            try
            {
                using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    workbook = new HSSFWorkbook(stream);
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }

            List<DataTable> dataTableList = new List<DataTable>();

            using (Sheet sheet = workbook.GetSheetAt(0))
            {
                try
                {
                    DataTable table = new DataTable();
                    table.TableName = sheet.SheetName;
                    Row row = sheet.GetRow(0);
                    int lastCellNum = row.LastCellNum;
                    int lastRowNum = sheet.LastRowNum;
                    for (int i = row.FirstCellNum; i < lastCellNum; i++)
                    {
                        DataColumn column = new DataColumn(row.GetCell(i).StringCellValue);
                        table.Columns.Add(column);
                    }
                    for (int j = sheet.FirstRowNum + 1; j <= lastRowNum; j++)
                    {
                        Row row2 = sheet.GetRow(j);
                        DataRow row3 = table.NewRow();
                        if (row2 != null)
                        {
                            for (int k = row2.FirstCellNum; k < lastCellNum; k++)
                            {
                                if (row2.GetCell(k) != null)
                                {
                                    row3[k] = SetCell(row2.GetCell(k), type);
                                }
                            }
                        }
                        table.Rows.Add(row3);
                    }
                    dataTableList.Add(table);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("未将对象引用设置到对象的实例"))
                    {
                        throw new JeasuException("请删除excel中的空行和空列，或使用程序中提供的模版，然后将excel中的数据，一列一列拷贝到对应模版中");
                    }
                    else
                    {
                        throw ex;
                    }
                }

            }

            RemoveEmpty(dataTableList[0]);
            if (dataTableList[0].Rows.Count > 2100)
            {
                throw new JeasuException("一次性导入文件数量大于2000行，请分成多个文件导入");
            }
            return dataTableList;
        }

        public void RemoveEmpty(DataTable dt)
        {
            List<DataRow> removelist = new List<DataRow>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                bool IsNull = true;
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (!string.IsNullOrWhiteSpace(dt.Rows[i][j].ToString().Trim()))
                    {
                        IsNull = false;
                    }
                }
                if (IsNull)
                {
                    removelist.Add(dt.Rows[i]);
                }
            }
            for (int i = 0; i < removelist.Count; i++)
            {
                dt.Rows.Remove(removelist[i]);
            }
        }

        private static string SetCell(Cell cell1, int? type = null)
        {
            if (cell1 == null)
            {
                return string.Empty;
            }

            //if (type == 1)
            //{
            //    if (cell1.ColumnIndex == 18 || cell1.ColumnIndex == 11)
            //    {
            //        return cell1.DateCellValue.ToString();
            //    }

            //}


            switch (cell1.CellType)
            {
                case CellType.STRING:
                    return cell1.StringCellValue;

                case CellType.FORMULA:
                    {
                        try
                        {
                            new HSSFFormulaEvaluator(cell1.Sheet.Workbook).EvaluateInCell(cell1);
                            return cell1.ToString();
                        }
                        catch
                        {
                            return cell1.NumericCellValue.ToString();
                        }
                    }
                case CellType.BLANK:
                    return string.Empty;

                case CellType.BOOLEAN:
                    return cell1.BooleanCellValue.ToString();

                case CellType.ERROR:
                    return cell1.ErrorCellValue.ToString();

            }
            return cell1.ToString();
        }

        public MemoryStream RenderToExcel(DataTable table)
        {
            MemoryStream ms = new MemoryStream();

            using (table)
            {
                using (Workbook workbook = new HSSFWorkbook())
                {
                    using (Sheet sheet = workbook.CreateSheet())
                    {
                        Row headerRow = sheet.CreateRow(0);

                        // handling header.
                        foreach (DataColumn column in table.Columns)
                            headerRow.CreateCell(column.Ordinal).SetCellValue(column.Caption);//If Caption not set, returns the ColumnName value

                        // handling value.
                        int rowIndex = 1;

                        foreach (DataRow row in table.Rows)
                        {
                            Row dataRow = sheet.CreateRow(rowIndex);

                            foreach (DataColumn column in table.Columns)
                            {
                                dataRow.CreateCell(column.Ordinal).SetCellValue(row[column].ToString());
                            }

                            rowIndex++;
                        }

                        workbook.Write(ms);
                        ms.Flush();
                        ms.Position = 0;
                    }
                }
            }
            return ms;
        }

        public byte[] GetExcelAsByte(DataTable table, Dictionary<string, string> renameColumns = null)
        {
            MemoryStream msExcel = new MemoryStream();
            byte[] buffer = null;
            try
            {
                if (renameColumns != null)
                {
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        //如果有重命名列，则重命名
                        if (renameColumns.Keys.Contains(table.Columns[i].ColumnName))
                        {
                            table.Columns[i].ColumnName = renameColumns[table.Columns[i].ColumnName];
                        }
                    }
                }
                msExcel = RenderToExcel(table);
                //MemoryStream ms = new MemoryStream();
                buffer = new byte[msExcel.Length];
                //buffer = new byte[ms.Length];
                msExcel.Position = 0;
                msExcel.Read(buffer, 0, buffer.Length);
            }
            catch
            {

            }
            finally
            {
                msExcel.Close();
            }

            return buffer;
        }

        public void SaveToFile(MemoryStream ms, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                byte[] data = ms.ToArray();

                fs.Write(data, 0, data.Length);
                fs.Flush();

                data = null;
            }
        }

        public string SaveErrorToFile(DataTable dt, Dictionary<int, string> errIndex, string fileName = "")
        {
            //没有名字则生成guid当名字
            if (string.IsNullOrWhiteSpace(fileName))
            {
                fileName = Guid.NewGuid().ToString("N");
            }
            //创建新table
            DataTable dtNew = new DataTable();
            dtNew = dt.Clone();//拷贝框架   
            foreach (var item in errIndex)
            {
                dtNew.ImportRow(dt.Rows[item.Key - 1]);
            }
            //不包含错误信息列 则添加一列
            if (!dtNew.Columns.Contains("错误信息"))
            {
                dtNew.Columns.Add("错误信息", typeof(string));
            }

            for (int i = 0; i < dtNew.Rows.Count; i++)
            {
                dtNew.Rows[i]["错误信息"] = errIndex.ElementAt(i).Value;
            }

            //保存excel让用户下载
            var msStream = RenderToExcel(dtNew);
            string endExcelPath = System.Web.HttpContext.Current.Server.MapPath("~/Upload/temp") + "\\" + fileName + ".xls";
            SaveToFile(msStream, endExcelPath);
            return "/Upload/temp/" + fileName + ".xls";
        }

        /// <summary>
        /// 判断表格是否包含这些字段
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="Titles"></param>
        /// <returns></returns>
        public bool IsRightExcelTitle(DataTable dt, List<string> Titles)
        {
            bool flage = true;
            try
            {
                for (int i = 0; i < Titles.Count; i++)
                {
                    if (dt.Columns[i].ColumnName.Trim() != Titles[i].Trim())
                    {
                        flage = false;
                        break;
                    }
                }
            }
            catch
            {
                flage = false;
            }
            return flage;

            //foreach (var item in Titles)
            //{
            //    if (!dt.Columns.Contains(item))
            //    {
            //        flage = false;
            //        break;
            //    }
            //}

        }

        static void RenderToBrowser(MemoryStream ms, HttpContext context, string fileName)
        {
            if (context.Request.Browser.Browser == "IE")
                fileName = HttpUtility.UrlEncode(fileName);
            context.Response.AddHeader("Content-Disposition", "attachment;fileName=" + fileName);
            context.Response.BinaryWrite(ms.ToArray());
        }
    }

}
