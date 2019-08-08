using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Data;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading;
using System.ComponentModel;

namespace ProjectTemplate
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]

    public class ProjectServices : System.Web.Services.WebService
    {
        ////////////////////////////////////////////////////////////////////////
        ///replace the values of these variables with your database credentials
        ////////////////////////////////////////////////////////////////////////
        private string dbID = "bradd";
        private string dbPass = "!!Cis440";
        private string dbName = "bradd";
        ////////////////////////////////////////////////////////////////////////

        ////////////////////////////////////////////////////////////////////////
        ///call this method anywhere that you need the connection string!
        ////////////////////////////////////////////////////////////////////////
        private string getConString()
        {
            return "SERVER=107.180.1.16; PORT=3306; DATABASE=" + dbName + "; UID=" + dbID + "; PASSWORD=" + dbPass;
        }
        ////////////////////////////////////////////////////////////////////////



        /////////////////////////////////////////////////////////////////////////
        //don't forget to include this decoration above each method that you want
        //to be exposed as a web service!
        [WebMethod(EnableSession = true)]
        /////////////////////////////////////////////////////////////////////////
        public string TestConnection()
        {
            try
            {
                string testQuery = "select * from test";

                ////////////////////////////////////////////////////////////////////////
                ///here's an example of using the getConString method!
                ////////////////////////////////////////////////////////////////////////
                MySqlConnection con = new MySqlConnection(getConString());
                ////////////////////////////////////////////////////////////////////////

                MySqlCommand cmd = new MySqlCommand(testQuery, con);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                return "Connection Tested: Success!";
            }
            catch (Exception e)
            {
                return "Something went wrong, please check your credentials and db name and try again.  Error: " + e.Message;
            }
        }

        [WebMethod(EnableSession = true)]
        public bool LogOn(string email, string pass)
        {

            bool success = false;
            MySqlConnection con = new MySqlConnection(getConString());

            string sqlSelect = $"SELECT * FROM bradd.accounts WHERE email='{HttpUtility.UrlDecode(email)}' and password='{HttpUtility.UrlDecode(pass)}'";

            MySqlCommand cmd = new MySqlCommand(sqlSelect, con);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable table = new DataTable();
          

            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                Session["id"] = email;
                success = true;
            }

            return success;
        }

        [WebMethod(EnableSession = true)]
        public string LoadComments()
        {
            try
            {
                string loadCommentsQuery = "select PostID, PostBody, timeOfPost from post";
                MySqlConnection con = new MySqlConnection(getConString());
                MySqlCommand cmd = new MySqlCommand(loadCommentsQuery, con);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                StringBuilder html = new StringBuilder();
                /*DateTime postTime = DateTime.Now;
                string userIP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.GetValue(0).ToString();

                return "LoadComments() tried: Success!" + postTime.ToString("MM/dd/yyyy hh:mm tt") + userIP;*/
                
                foreach (DataRow row in table.Rows)
                {
                    html.Append("<div class='media'>");
                    html.Append("<img src='https://twistedsifter.files.wordpress.com/2012/09/trippy-profile-pic-portrait-head-on-and-from-side-angle.jpg?w=800&h=700'");
                    html.Append("class='mr-3' alt='Profile Picture'>");
                    html.Append("<div class='media-body'>");
                    html.Append("<h5 class='mt-0'>" + row[0] + "</h5>");
                    html.Append(row[1]);
                    html.Append("</div>");
                    html.Append("<button type='button' class='btn btn-light'>Reply</button>");
                    html.Append("</div>");
                }

                return html.ToString();
            }
            catch (Exception e)
            {
                return "Something went wrong, please check your credentials and db name and try again.  Error: " + e.Message;
            }
        }

        [WebMethod(EnableSession = true)]
        public string CommentPost(string CommentBody)
        {
            try
            {
                string CommentPostQuery = $"INSERT INTO post (PostBody) VALUES (\"{CommentBody}\");";

                ////////////////////////////////////////////////////////////////////////
                ///here's an example of using the getConString method!
                ////////////////////////////////////////////////////////////////////////
                MySqlConnection con = new MySqlConnection(getConString());
                ////////////////////////////////////////////////////////////////////////

                MySqlCommand cmd = new MySqlCommand(CommentPostQuery, con);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                StringBuilder html = new StringBuilder();

                return "CommentPost() Tried: Success!";
            }
            catch (Exception e)
            {
                return "CommentPost() Failed  Error: " + e.Message;
            }
        }

        [WebMethod(EnableSession = true)]
        public string CommentReply()
        {
            try
            {
                string CommentReplyQuery = "#";
                MySqlConnection con = new MySqlConnection(getConString());
                MySqlCommand cmd = new MySqlCommand(CommentReplyQuery, con);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                Notification("accountStatus"); // dummy code for an unimplementable method
                return "CommentReply() Tried: Success!";
            }
            catch (Exception e)
            {
                return "CommentReply() Failed  Error: " + e.Message;
            }

        }

        // This is supposed to email the user when they get replied to by a user or a manager, 
        // but we cannot implement this within our project due to server and email limitations
        [WebMethod(EnableSession = true)]
        public void Notification(string accountStatus)
        {
            if (accountStatus == "Manager")
            {
                // code for sending email notice if a manager replies to your post
            }
            else { }// code for sending email notice if a non-manager replies to your post
        }

        [WebMethod(EnableSession = true)]
        public string GetPostsByUser(string userEmail)
        {
            try
            {
                // string query = $"INSERT INTO post (UserEmail,PostBody) VALUES (\"{userEmail}\",\"This is my first post and I'd appreciate any feedback!\")";
                string query = $"SELECT * FROM post WHERE UserEmail=\"{userEmail}\"";
                MySqlConnection conn = new MySqlConnection(getConString());
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();

                adapter.Fill(table);

                //Table start.
                StringBuilder html = new StringBuilder();
                html.Append("<table border = '1'>");
                // maybe add the table class to the table tag?

                //Building the Header row.
                html.Append("<tr>");
                foreach (DataColumn column in table.Columns)
                {
                    html.Append("<th>");
                    html.Append(column.ColumnName);
                    html.Append("</th>");
                }
                html.Append("</tr>");

                //Building the Data rows.
                foreach (DataRow row in table.Rows)
                {
                    html.Append("<tr>");
                    foreach (DataColumn column in table.Columns)
                    {
                        html.Append("<td>");
                        html.Append(row[column.ColumnName]);
                        html.Append("</td>");
                    }
                    html.Append("</tr>");
                }

                //Table end.
                html.Append("</table>");

                return html.ToString();
            }
            catch (Exception e)
            {
                return $"Something went wrong getting posts for user {userEmail}. Error: " + e.Message;
            }
        }

        [WebMethod(EnableSession = true)]
        public string SubmitUserRequest(string userEmail, string userFirst, string userLast)
        {


         //   try
            {

                string query = "INSERT INTO bradd.accounts(email,firstname,lastname, stat) VALUES(@email,@firstname,@lastname, 'Pending')";
       
                MySqlConnection conn = new MySqlConnection(getConString());
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.Add("@email", MySqlDbType.VarChar).Value = userEmail;
                cmd.Parameters.Add("@firstname", MySqlDbType.VarChar).Value = userFirst;
                cmd.Parameters.Add("@lastname", MySqlDbType.VarChar).Value = userLast;
                cmd.ExecuteNonQuery();
                conn.Close();
                return "added";
            }


         /*   catch (Exception e)
            {
                return $"Something went wrong getting posts for user Error: " + e.Message;
            }*/

            return "failed";
        }

        [WebMethod(EnableSession = true)]
        public string DenyRequest(string email)
        {


            //   try
            {

                

                    string query = $"UPDATE bradd.accounts SET stat = 'Denied' WHERE email=\"{email}\"";
                    MySqlConnection conn = new MySqlConnection(getConString());
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.ExecuteNonQuery();
                    conn.Close();

                var from = new MailAddress("braddasu@gmail.com", "BRADD Admin");
                var to = new MailAddress($"{email}", "New User");
                string fromPassword = "!!Cis440";
                string subject = "BRADD Account Approved";
                string body = "You have been denied!";

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(from.Address, fromPassword),
                    Timeout = 20000
                };
                using (var message = new MailMessage(from, to)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }


            }

            return "added";


            /*   catch (Exception e)
               {
                   return $"Something went wrong getting posts for user Error: " + e.Message;
               }*/

            //     return "failed";
        }

        [WebMethod(EnableSession = true)]
        public string ApproveRequest(string email, string password)
        {


            //   try
            {

                string query = $"UPDATE bradd.accounts SET stat = 'Accepted', password =\"{password}\" WHERE email=\"{email}\"";
                MySqlConnection conn = new MySqlConnection(getConString());
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
                conn.Close();

                var from = new MailAddress("braddasu@gmail.com", "BRADD Admin");
                var to = new MailAddress($"{email}", "New User");
                string fromPassword = "!!Cis440";
                string subject = "BRADD Account Approved";
                string body = $"You have been approved! Your Password is:" + password;

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential(from.Address, fromPassword),
                    Timeout = 20000
                };
                using (var message = new MailMessage(from, to)
                {
                    Subject = subject,
                    Body = body
                })
                {
                    smtp.Send(message);
                }



                return "added";
            }


            /*   catch (Exception e)
               {
                   return $"Something went wrong getting posts for user Error: " + e.Message;
               }*/

        //    return "failed";
        }




        [WebMethod(EnableSession = true)]
        public string CheckUserRequest()
        {
            try
            {
                // string query = $"INSERT INTO post (UserEmail,PostBody) VALUES (\"{userEmail}\",\"This is my first post and I'd appreciate any feedback!\")";
                string query = "SELECT * FROM bradd.accounts WHERE stat ='Pending'";
                MySqlConnection conn = new MySqlConnection(getConString());
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();

                adapter.Fill(table);

                //Table start.
                StringBuilder html = new StringBuilder();
                html.Append("<table border = '1'>");

                //Building the Header row.
                html.Append("<tr>");
                foreach (DataColumn column in table.Columns)
                {
                    html.Append("<th>");
                    html.Append(column.ColumnName);
                    html.Append("</th>");
                }
                html.Append("</tr>");

                //Building the Data rows.
                foreach (DataRow row in table.Rows)
                {
                    html.Append("<tr>");
                    foreach (DataColumn column in table.Columns)
                    {
                        html.Append("<td>");
                        html.Append(row[column.ColumnName]);
                        html.Append("</td>");
                    }
                    html.Append("</tr>");
                }

                //Table end.
                html.Append("</table>");
               var requestTable = html.ToString();
                return requestTable;
            }
            catch (Exception e)
            {
                return "Something went wrong getting posts for user. Error: " + e.Message;
            }
        }

        [WebMethod(EnableSession = true)]
        public string LogOut()
        {
            Session.Abandon();
            return "Logged out successfully";
        }


    }
}