using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Project_2022_
{
    internal class DB_Connection
    {
        MySqlConnection connection;
        public DB_Connection(string ip, string db_user, string database, string db_password)
        {
            string db = "server=" + ip + ";user=" + db_user + ";database=" + database + ";password=" + db_password + ";";
            connection = new MySqlConnection(db);    // Соединение с БД
            connection.Open();
        }
        public int request_auth(string login, string password)
        {
            int f = 0;
            string request = "SELECT `id`, `log`, `status` FROM `name` WHERE `log` = '" + login + "' and `password` = '" + password + "'";
            MySqlCommand command = new MySqlCommand(request, connection);   // Отправляем запрос
            MySqlDataReader reader = command.ExecuteReader();               // Читаем ответ

            while (reader.Read())
            {
                if (reader[1].ToString() == login)
                {
                    f = 1;
                }
                if (reader[2].ToString() == "1" && reader[1].ToString() == login)
                {
                    f = 2;
                }
            }
            reader.Close();

            if (f == 1)
            {
                request = "UPDATE `name` SET `status` = '1' WHERE `log` = '" + login + "'";
                command = new MySqlCommand(request, connection);
                command.ExecuteNonQuery();
            }

            return f;
        }
        public bool request_registr(string login, string password)
        {
            bool f = false;
            string request = "SELECT `log` FROM `name` WHERE `log` = '" + login + "'";
            MySqlCommand command = new MySqlCommand(request, connection);
            MySqlDataReader reader = command.ExecuteReader();               // Отправляем запрос

            while (reader.Read())
            {
                if (reader[0].ToString() == login)
                    f = true;
                else f = false;
            }
            reader.Close();

            if (f)
                return false;
            request = "INSERT INTO `name` (`log`, `password`, `status`) VALUES ('" + login + "', '" + password + "', '1')";
            command = new MySqlCommand(request, connection);
            command.ExecuteNonQuery();                                      // Отправляем запрос
            return true;
        }

        public bool request_logout(string login)
        {
            try
            {
                string request = "UPDATE `name` SET `status` = '0' WHERE `log` = '" + login + "'";
                MySqlCommand command = new MySqlCommand(request, connection);
                command.ExecuteNonQuery();                                      // Отправляем запрос
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex);
            }
            return true;
        }

        public List<(int, int, string)> request_UsersList()
        {
            List<(int, int, string)> users = new List<(int, int, string)>();
            string request = "SELECT * FROM `name`";
            MySqlCommand command = new MySqlCommand(request, connection);
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                users.Add((Convert.ToInt32(reader[0]), Convert.ToInt32(reader[3]), reader[1].ToString()));
            }
            reader.Close();
            return users;
        }

        public List<(string, string, string, int)> request_MessageList(string user1, string user2)
        {
            List<(string, string, string, int)> messages = new List<(string, string, string, int)>();
            string request;
            if (user2 == "Global")
            {
                request = "SELECT `ot`, `mess`, `id` FROM `message` WHERE `kou` = 'Global'";
                MySqlCommand command = new MySqlCommand(request, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    messages.Add(("Null", Convert.ToString(reader[1]), Convert.ToString(reader[2]), Convert.ToInt32(reader[3])));
                }
                reader.Close();
            }
            else
            {
                request = "SELECT  `ot`, `mess`, `id` FROM `message` WHERE `ot` = '" + user1 + "' AND `komu` = '" + user2 + "' or `ot` = '" + user2 + "' AND `komu` = '" + user1 + "'";
                MySqlCommand command = new MySqlCommand(request, connection);
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    messages.Add(("Null", Convert.ToString(reader[1]), Convert.ToString(reader[2]), Convert.ToInt32(reader[3])));
                }
                reader.Close();
            }

            return messages;
        }

        public void request_MessageAdd(string sender, string recipient, string message)
        {
            string request = "INSERT INTO `message` (`ot`, `komu`, `mess`) VALUES ('" + sender + "', '" + recipient + "', '" + message + "')";
            MySqlCommand command = new MySqlCommand(request, connection);
            command.ExecuteNonQuery();
        }

        public void request_MessageDelete(string id, string user)
        {
            string request = "DELETE FROM `message` WHERE `id` = '" + id + "' AND `ot` = '" + user + "'";
            MySqlCommand command = new MySqlCommand(request, connection);
            command.ExecuteNonQuery();
        }

        public void request_UserDelete(string user)
        {
            Console.WriteLine("User deleted: " + user);
            string request = "DELETE FROM `name` WHERE `log` = '" + user + "'";
            MySqlCommand command = new MySqlCommand(request, connection);
            command.ExecuteNonQuery();
        }

        public void close()
        {
            connection.Close();
        }
    }
}
