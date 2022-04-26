namespace Surabhi
{
    using System;
    using MySql.Data;
    using MySql.Data.MySqlClient;
    public class MySqlDataConnector : IDisposable
    {
        private readonly MySqlConnection connection;

        public MySqlDataConnector()
        {
            this.connection = new MySqlConnection("server=localhost;user=root;database=remy_data;port=3306;password=incon");
            this.connection.Open();
        }

        public void Dispose()
        {
            ((IDisposable)connection).Dispose();
        }

        public void PutLevel(Level level)
        {
            string sql = $"INSERT IGNORE INTO level (id, timestamp, name, open, high, low, close, volume) VALUES ('{level.Id}','{level.Timestamp}', '{level.Name}', {level.High}, {level.Open}, {level.Low}, {level.Close}, {level.Volume})";
            var cmd = new MySqlCommand(sql, connection);
            cmd.ExecuteNonQuery();
        }
    }
}