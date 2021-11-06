using PartialViewDataArchiving.ViewModels;
using PartialViewInterface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewDataArchiving.DB
{
    public class ArchiveTableManager
    {
        public List<ArchiveTable> ArchiveTables()
        {
            string error = "";
            DataTable dataTable = EnvironmentInfo.SqliteHelper.GetDataTable(out error, "select * from sys_archive_table;");
            List<ArchiveTable> archiveTables = new List<ArchiveTable>();
            foreach (DataRow dr in dataTable.Rows)
            {
                ArchiveTable table = new ArchiveTable();
                table.Id = int.Parse(dr["Id"].ToString());
                table.TableName = dr["TableName"].ToString();
                table.DateField = dr["DateField"].ToString();
                table.Where = dr["Where"].ToString();
                archiveTables.Add(table);
            }

            return archiveTables;
        }

        public void RemoveArchiveTable(ArchiveTable archiveTable)
        {
            EnvironmentInfo.SqliteHelper.ExecuteSql($"delete from sys_archive_table where tablename='{archiveTable.TableName}'");
        }

        public void AddArchiveTable(ArchiveTable archiveTable)
        {
            EnvironmentInfo.SqliteHelper.ExecuteSql($"insert into sys_archive_table (TableName,DateField,Where) values ('{archiveTable.TableName}','{archiveTable.DateField}','{archiveTable.Where}')");

        }
    }
}
