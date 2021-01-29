using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Net;
using System.Web;
using System.IO;
using System.Text;
using DBUtility;

namespace PartialViewExportFacePic.ViewModels
{
    public class BllProcess
    {
        public List<PersonInfo> GetFileFullPath()
        {
            List<PersonInfo> list = new List<PersonInfo>();
            string sql = "select * from HR.Person where DeptID<>3 and DeleteFlag =0 and Len(Photo) >0 ";
            DataTable dt = SQLHelper.ExecuteDataTableEx(sql, null);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    PersonInfo info = new PersonInfo();
                    info.Photo = dr["Photo"].ToString();
                    info.PersonNO = dr["NO"].ToString();
                    info.PersonName = dr["Name"].ToString();
                    list.Add(info);
                }
            }
            return list;
        }

        public string GetPersonImageSavePath()
        {
            string sql = "select top 1 PersonImageSavePath from mc.SystemInfo";
            DataTable dt = SQLHelper.ExecuteDataTableEx(sql, null);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["PersonImageSavePath"].ToString();
            }
            return "";
        }
        public string GetDownServerUrl()
        {
            string url = "";
            string sql = string.Format("select DownloadServerUrl from control_http_param where type=1 ");
            MySqlDataReader reader = DBUtility.MySqlHelper.ExecuteReader(sql, null);
            while (reader.Read())
            {
                url = reader[0].ToString();
                break;
            }
            return url;
        }

        public List<PersonInfo> GetJielinkPersonImage()
        {
            List<PersonInfo> list = new List<PersonInfo>();
            string sql = string.Format("select PhotoPath,PersonNo,PersonName from control_person where status = 0 and LENGTH(photopath) > 0");
            MySqlDataReader reader = DBUtility.MySqlHelper.ExecuteReader(sql, null);
            while (reader.Read())
            {
                PersonInfo info = new PersonInfo();
                info.Photo = reader[0].ToString();
                info.PersonNO = reader[1].ToString();
                info.PersonName = reader[2].ToString();
                list.Add(info);
            }
            return list;
        }

        public bool TestConnect(string connection, int type)
        {
            string sql = "select * from sys_user limit 1";
            if (type == 1)
            {
                return DBUtility.MySqlHelper.TestConnection(connection, sql);
            }
            else
            {
                return SQLHelper.TestConnection(connection, sql);
            }
        }



        /// <summary> 
        /// 下载图片 
        /// </summary> 
        /// <param name="picUrl">图片Http地址</param> 
        /// <param name="savePath">保存路径</param> 
        /// <param name="timeOut">Request最大请求时间，如果为-1则无限制</param> 
        /// <returns></returns> 

        public bool DownloadPicture(string picUrl, string savePath)
        {
            bool value = false;
            WebResponse response = null;
            Stream stream = null;
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(picUrl);
                request.Timeout = 10000;
                response = request.GetResponse();
                stream = response.GetResponseStream();
                if (!response.ContentType.ToLower().StartsWith("text/"))
                    value = SaveBinaryFile(response, savePath);
            }
            finally
            {
                if (stream != null) stream.Close();
                if (response != null) response.Close();
            }
            return value;
        }

        private bool SaveBinaryFile(WebResponse response, string savePath)
        {
            bool value = false;
            byte[] buffer = new byte[1024];
            Stream outStream = null;
            Stream inStream = null;
            try
            {
                if (File.Exists(savePath)) File.Delete(savePath);
                outStream = System.IO.File.Create(savePath);
                inStream = response.GetResponseStream();
                int l;
                do
                {

                    l = inStream.Read(buffer, 0, buffer.Length);
                    if (l > 0) outStream.Write(buffer, 0, l);
                } while (l > 0);
                value = true;
            }
            finally
            {
                if (outStream != null) outStream.Close();
                if (inStream != null) inStream.Close();
            }
            return value;
        }

    }
}
