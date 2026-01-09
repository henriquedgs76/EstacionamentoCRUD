using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace EstacionamentoCRUD.DAL
{
    public static class DataAccess
    {
        // pega a string de conexão do banco de dados que tá lá no arquivo Web.config.
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["EstacionamentoDB"].
                ConnectionString;
        }

        // serve para executar comandos que não devolvem dados, tipo INSERT, UPDATE e DELETE.
        // retorna o número de linhas que foram afetadas.
        public static int ExecuteNonQuery(string commandText,
            SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                using (var command = new SqlCommand(commandText, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
        }

        // método que funciona com uma transação, pra garantir que tudo de certo ou nada seja feito no banco.
        public static int ExecuteNonQuery(string commandText,
            SqlParameter[] parameters, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(commandText, transaction.Connection, transaction))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                return command.ExecuteNonQuery();
            }
        }

        // cria um comando SELECT e devolve os resultados organizados em uma tabela datatable pra gente poder usar.
        public static DataTable ExecuteDataTable(string commandText,
            SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                using (var command = new SqlCommand(commandText, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    var dataAdapter = new SqlDataAdapter(command);
                    var dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);
                    return dataTable;
                }
            }
        }

        //método que funciona com uma transação.
        public static DataTable ExecuteDataTable(string commandText,
            SqlParameter[] parameters, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(commandText, transaction.Connection, transaction))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                var dataAdapter = new SqlDataAdapter(command);
                var dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                return dataTable;
            }
        }

        // faz um comando e devolve um único valor pra pegar um ID que acabou de ser criado ou fazer uma contagem (COUNT).
        public static object ExecuteScalar(string commandText, SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(GetConnectionString()))
            {
                using (var command = new SqlCommand(commandText, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    connection.Open();
                    return command.ExecuteScalar();
                }
            }
        }

        // esse metodo funciona com uma transação.
        public static object ExecuteScalar(string commandText, SqlParameter[] parameters, SqlTransaction transaction)
        {
            using (var command = new SqlCommand(commandText, transaction.Connection, transaction))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }
                return command.ExecuteScalar();
            }
        }

        // salva na tabela de auditoria tudo o que o usuário faz no sistema, o que fez , e quem fez
        public static void RegistrarAuditoria(int usuarioId, string acao, string tabela, int registroId)
        {
            string sql = @"INSERT INTO Auditoria (UsuarioId, Acao, Tabela, RegistroId, DataHora)
                           VALUES (@UsuarioId, @Acao, @Tabela, @RegistroId, GETDATE())";

            var parameters = new[]
            {
                new SqlParameter("@UsuarioId", usuarioId),
                new SqlParameter("@Acao", acao),
                new SqlParameter("@Tabela", tabela),
                new SqlParameter("@RegistroId", registroId)
            };

            ExecuteNonQuery(sql, parameters);
        }

       
        public static void RegistrarAuditoria(int usuarioId, string acao, string tabela, int registroId, SqlTransaction transaction)
        {
            string sql = @"INSERT INTO Auditoria (UsuarioId, Acao, Tabela, RegistroId, DataHora)
                           VALUES (@UsuarioId, @Acao, @Tabela, @RegistroId, GETDATE())";

            var parameters = new[]
            {
                new SqlParameter("@UsuarioId", usuarioId),
                new SqlParameter("@Acao", acao),
                new SqlParameter("@Tabela", tabela),
                new SqlParameter("@RegistroId", registroId)
            };

            // Usa o ExecuteNonQuery que já sabe lidar com transações.
            ExecuteNonQuery(sql, parameters, transaction);
        }

        // essa consulta meio complexa que junta as horas de entrada e saída dos veículos pra gente saber os horários de pico no estacionamento.
        public static DataTable GetPrevisaoDeAtividadePorHora()
        {
            string sql = @"
                WITH AllEvents AS (
                    -- Coleta todas as horas de entrada
                    SELECT 
                        DATEPART(hour, DataEntrada) AS Hora
                    FROM 
                        Veiculos
                    WHERE 
                        DataEntrada IS NOT NULL

                    UNION ALL

                    -- Coleta todas as horas de saída
                    SELECT 
                        DATEPART(hour, DataSaida) AS Hora
                    FROM 
                        Veiculos
                    WHERE 
                        DataSaida IS NOT NULL
                )
                -- Agrupa por hora e conta o total de eventos
                SELECT 
                    Hora,
                    COUNT(*) AS TotalAtividade
                FROM 
                    AllEvents
                WHERE
                    Hora IS NOT NULL
                GROUP BY 
                    Hora
                ORDER BY 
                    Hora;
            ";
            return ExecuteDataTable(sql, null);
        }
    }
}
