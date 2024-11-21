using Npgsql;
namespace OnlineEducationSystem.Helpers;

public class DatabaseHelper
{
    private readonly string _connectionString;

    public DatabaseHelper(string connectionString)
    {
        _connectionString = connectionString;
    }

    public int ExecuteNonQuery(string query, params NpgsqlParameter[] parameters)
    {
        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddRange(parameters);
                return command.ExecuteNonQuery();
            }
        }
    }


    public List<T> ExecuteReader<T>(string query, Func<NpgsqlDataReader, T> readRow, params NpgsqlParameter[] parameters)
    {
        var results = new List<T>();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            connection.Open();
            using (var command = new NpgsqlCommand(query, connection))
            {
                command.Parameters.AddRange(parameters);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        results.Add(readRow(reader));
                    }
                }
            }
        }

        return results;
    }
}
