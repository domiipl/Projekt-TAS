using MySql.Data.MySqlClient;
using System.Data;

namespace ProjektTAS.Classes
{
    public class MySQLObject
    {
        public MySqlConnection _Connection { get; private set; }
        public DataTable Data { get; private set; }

        public MySQLObject(string ConnectionString = Config.ConnectionString)
        {
            _Connection = new MySqlConnection(ConnectionString);
        }


        public DataTable Select(string sql)
        {
            _Connection.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter(sql, _Connection);
            MySqlCommand command = new MySqlCommand(sql, _Connection);
            adapter.Fill(Data);
            _Connection.Close();
            return Data;
        }

        private void ReturnlessQuery(string sql)
        {
            _Connection.Open();
            MySqlCommand command = new MySqlCommand(sql, _Connection);
            command.ExecuteNonQuery();
            _Connection.Close();
        }

        public void Insert(string sql) => ReturnlessQuery(sql);
        public void Update(string sql) => ReturnlessQuery(sql);
    }
}
