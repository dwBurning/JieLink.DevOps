using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PartialViewInterface.Utils
{
    public class SendEmailHelper
    {
        // MailMessage

        public static void SendEmail(string receiver, string title, string content, bool isBodyHtml = false)
        {
            try
            {
                var emailAcount = "864108071@qq.com";
                var emailPassword = "vkxatwjwzctebaii";

                MailMessage message = new MailMessage();
                //设置发件人,发件人需要与设置的邮件发送服务器的邮箱一致
                MailAddress fromAddr = new MailAddress(emailAcount);
                message.From = fromAddr;
                //设置收件人,可添加多个,添加方法与下面的一样
                message.To.Add(receiver);
                //设置抄送人
                message.CC.Add("deadlinewei@outlook.com");
                //设置邮件标题
                message.Subject = title;
                //设置邮件内容
                message.Body = content;
                //设置邮件内容是否为html
                message.IsBodyHtml = isBodyHtml;
                //设置邮件发送服务器,服务器根据你使用的邮箱而不同,可以到相应的 邮箱管理后台查看,下面是QQ的
                SmtpClient client = new SmtpClient("smtp.qq.com", 25);
                //设置发送人的邮箱账号和密码
                client.Credentials = new NetworkCredential(emailAcount, emailPassword);
                //启用ssl,也就是安全发送
                client.EnableSsl = true;
                //发送邮件
                client.Send(message);
            }
            catch (Exception)
            {

            }
        }

        public static async void SendEmailAsync(string reciver, string title, string content, bool isBodyHtml = false)
        {
            await TaskHelper.Start(new Action(() =>
            {
                SendEmail(reciver, title, content, isBodyHtml);
            }));
        }

        public static string HtmlBody(DataTable dataTable)
        {
            StringBuilder htmlBody = new StringBuilder();
            htmlBody.AppendLine("<!DOCTYPE html>");
            htmlBody.AppendLine("<html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">");
            htmlBody.AppendLine("<head>");
            htmlBody.AppendLine("<meta charset=\"utf - 8\" />");
            htmlBody.AppendLine("<title></title>");

            htmlBody.AppendLine("<style type=\"text/css\">");
            htmlBody.AppendLine("html {font-family: sans-serif;-ms-text-size-adjust: 100%;-webkit-text-size-adjust: 100%;}body {margin: 10px;}table {border-collapse: collapse;border-spacing: 0;}td,th {padding: 0;}.pure-table {border-collapse: collapse;border-spacing: 0;empty-cells: show;border: 1px solid #cbcbcb;}.pure-table caption {color: #000;font: italic 85%/1 arial,sans-serif;padding: 1em 0;text-align: center;}.pure-table td,.pure-table th {border-left: 1px solid #cbcbcb;border-width: 0 0 0 1px;font-size: inherit;margin: 0;overflow: visible;padding: .5em 1em;}.pure-table thead {background-color: #e0e0e0;color: #000;text-align: left;vertical-align: bottom;}.pure-table td {background-color: transparent;}.pure-table-bordered td {border-bottom: 1px solid #cbcbcb;}.pure-table-bordered tbody>tr:last-child>td {border-bottom-width: 0;}");
            htmlBody.AppendLine("</style>");

            htmlBody.AppendLine("</head>");
            htmlBody.AppendLine("<body>");
            htmlBody.AppendLine("<table class=\"pure-table pure-table-bordered\">");
            htmlBody.AppendLine("<thead>");
            htmlBody.AppendLine("<tr>");
            htmlBody.AppendLine("<th>#</th>");
            foreach (DataColumn dataColumn in dataTable.Columns)
            {
                htmlBody.Append("<th>").Append(dataColumn.ColumnName).AppendLine("</th>");
            }
            htmlBody.AppendLine("</tr>");
            htmlBody.AppendLine("</thead>");
            int rowIndex = 0;
            foreach (DataRow dataRow in dataTable.Rows)
            {
                rowIndex++;
                htmlBody.Append("<tr>");
                htmlBody.AppendLine($"<td>{rowIndex}</td>");
                foreach (DataColumn dataColumn in dataTable.Columns)
                {
                    htmlBody.Append("<td>").Append(dataRow[dataColumn.ColumnName]).AppendLine("</td>");
                }
                htmlBody.AppendLine("</tr>");
            }
            htmlBody.AppendLine("</table>");
            htmlBody.AppendLine("</body>");
            htmlBody.AppendLine("</html>");

            return htmlBody.ToString();
        }
    }
}
