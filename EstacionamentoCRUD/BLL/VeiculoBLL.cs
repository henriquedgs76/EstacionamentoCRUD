using System;
using System.Data;
using System.Data.SqlClient;
using EstacionamentoCRUD.DAL;

namespace EstacionamentoCRUD.BLL
{
    public class VeiculoBLL
    {
        // método principal pra dar entrada em um veículo. Ele faz um monte de coisa de uma vez só.
        // usa uma 'transaction', que é pra garantir que se alguma coisa der errado, ele desfaz tudo.
        public Tuple<string, int> RegistrarEntrada(string placa, int? marcaId, string modelo, string cor, int vagaId, int servicoId, string telefone)
        {
            string ticketId = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
            int novoVeiculoId;

            using (var connection = new SqlConnection(DataAccess.GetConnectionString()))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // verifica se o carro já não tá lá dentro pra não dar duplicidade.
                    var checkParams = new[] { new SqlParameter("@Placa", placa) };
                    int existe = Convert.ToInt32(DataAccess.ExecuteScalar("SELECT COUNT(*) FROM Veiculos WHERE Placa = @Placa AND Status = 'Estacionado'", checkParams, transaction));

                    if (existe > 0)
                    {
                        transaction.Rollback();
                        throw new Exception("Já existe um veículo em serviço com essa placa!");
                    }

                    // insere o veículo novo no banco, junto com a placa, modelo, hora, etc.
                   
                    var insertParams = new[]
                    {
                    new SqlParameter("@Placa", placa),
                    new SqlParameter("@MarcaId", (object)marcaId ?? DBNull.Value),
                    new SqlParameter("@Modelo", modelo),
                    new SqlParameter("@Cor", cor),
                    new SqlParameter("@DataEntrada", DateTime.Now),
                    new SqlParameter("@HoraEntrada", DateTime.Now.TimeOfDay),
                    new SqlParameter("@Status", "Estacionado"),
                    new SqlParameter("@VagaId", vagaId),
                    new SqlParameter("@TicketId", ticketId),
                    new SqlParameter("@ServicoId", servicoId),
                    new SqlParameter("@EmpresaId", 1),
                    new SqlParameter("@Telefone", (object)telefone ?? DBNull.Value) // <-- NOVO
                };

                    // inserindo no sql
                    string sqlInsert = @"
                    INSERT INTO Veiculos (Placa, MarcaId, Modelo, Cor, DataEntrada, HoraEntrada, Status, VagaId, TicketId, ServicoId, Telefone, EmpresaId)
                        VALUES (@Placa, @MarcaId, @Modelo, @Cor, @DataEntrada, @HoraEntrada, @Status, @VagaId, @TicketId, @ServicoId, @Telefone, @EmpresaId);
                    SELECT SCOPE_IDENTITY();";

                    object newIdObj = DataAccess.ExecuteScalar(sqlInsert, insertParams, transaction);
                    novoVeiculoId = Convert.ToInt32(newIdObj);

                    // ocupa a vaga de estacionamento.
                    var updateParams = new[] { new SqlParameter("@VagaId", vagaId) };
                    string sqlUpdate = "UPDATE Vagas SET Status = 'Ocupada' WHERE Id = @VagaId";
                    DataAccess.ExecuteNonQuery(sqlUpdate, updateParams, transaction);

                    transaction.Commit();
                    return new Tuple<string, int>(ticketId, novoVeiculoId);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Erro ao registrar lava-rápido: " + ex.Message, ex);
                }
            }
        }

        // esse método é pra quando o cliente tá indo embora.
        // também usa uma 'transaction' pra garantir que tudo aconteça direitinho.
        public void RegistrarSaida(string placa, decimal valorPago, string formaPagamento)
        {
            using (var connection = new SqlConnection(DataAccess.GetConnectionString()))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // antes de tudo, descobre qual vaga este veículo ocupa pra poder liberar ela depois.
                    object vagaIdObj = DataAccess.ExecuteScalar("SELECT VagaId FROM Veiculos WHERE Placa = @Placa AND Status = 'Estacionado'", new[] { new SqlParameter("@Placa", placa) }, transaction);

                    // Se não achou o veículo, algo tá errado.
                    if (vagaIdObj == null || vagaIdObj == DBNull.Value)
                    {
                        transaction.Rollback();
                        throw new Exception("Veículo não encontrado ou já finalizado.");
                    }

                    // atualiza o status do veículo pra 'Finalizado', registra o pagamento,
                    // e também o status da lavagem pra 'Concluído'.
                    string sqlUpdateVeiculo = @"UPDATE Veiculos 
                           SET DataSaida = GETDATE(), 
                               ValorPago = @ValorPago, 
                               Status = 'Finalizado', 
                               FormaPagamento = @FormaPagamento, -- NOVA COLUNA
                               StatusLavagem = 'Concluído', -- AGORA VAI ATUALIZAR AS DUAS!
                               VagaId = NULL ,
                               Ativo = 0
                           WHERE Placa = @Placa AND Status = 'Estacionado'";




                    var veiculoParams = new[]
                    {
                        new SqlParameter("@Placa", placa),
                        new SqlParameter("@ValorPago", valorPago),
                        new SqlParameter("@FormaPagamento", formaPagamento)
                    };

                    int rowsAffected = DataAccess.ExecuteNonQuery(sqlUpdateVeiculo, veiculoParams, transaction);

                    if (rowsAffected == 0) // Se não afetou nenhuma linha, o veículo não foi encontrado.
                    {
                        transaction.Rollback();
                        throw new Exception("Erro ao atualizar o veículo. Não encontrado ou já finalizado.");
                    }

                    // libera a vaga que o carro estava usando.
                    int vagaId = Convert.ToInt32(vagaIdObj);
                    string sqlUpdateVaga = "UPDATE Vagas SET Status = 'Livre' WHERE Id = @VagaId";
                    DataAccess.ExecuteNonQuery(sqlUpdateVaga, new[] { new SqlParameter("@VagaId", vagaId) }, transaction);

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Erro ao registrar a saída do veículo: " + ex.Message, ex);
                }
            }
        }

        // um método simples pra gente achar a placa do carro usando o número do ticket.
        public string BuscarPlacaPorTicket(string ticketId)
        {
            string sql = "SELECT Placa FROM Veiculos WHERE TicketId = @TicketId AND Status = 'Estacionado'";
            var parameters = new[] { new SqlParameter("@TicketId", ticketId) };

            object result = DataAccess.ExecuteScalar(sql, parameters);

            if (result != null && result != DBNull.Value)
            {
                return result.ToString();
            }

            return null;
        }

        // pega todas as informações do veículo, da marca e do serviço que foi feito, usando o número do ticket.
        // é usado pra mostrar os detalhes na hora de imprimir o ticket
        public DataTable BuscarVeiculoPorTicket(string ticketId)
        {
            
            string sql = @"
            SELECT 
                V.Placa, 
                V.Modelo,
                V.DataEntrada, 
                V.HoraEntrada, 
                V.TicketId,
                V.Telefone, 
                ISNULL(M.Nome, 'N/A') AS Marca,
                S.NomeServico 
            FROM 
                Veiculos V
                LEFT JOIN Marcas M ON V.MarcaId = M.Id
                INNER JOIN Servicos S ON V.ServicoId = S.Id 
            WHERE 
                V.TicketId = @TicketId";

            var parameters = new[] { new System.Data.SqlClient.SqlParameter("@TicketId", ticketId) };
            return DataAccess.ExecuteDataTable(sql, parameters);
        }
        // esse metodo manda os números pro dashboard principal: quantos carros estão no pátio agora, quanto faturou hoje e quantas vagas tão livres.
        public DataTable ObterResumoDashboard()
        {
            // aqui pega os números pro dashboard principal: quantos carros estão no pátio agora, quanto faturou hoje e quantas vagas tão livres.
            string sql = @"
        SELECT 
            (SELECT COUNT(*) FROM Veiculos WHERE Status = 'Estacionado' AND Ativo = 1 AND EmpresaId = 1) as NoPatio,
            (SELECT ISNULL(SUM(ValorPago), 0) FROM Veiculos WHERE Status = 'Finalizado' AND CAST(DataSaida AS DATE) = CAST(GETDATE() AS DATE) AND EmpresaId = 1) as FaturamentoHoje,
            (SELECT COUNT(*) FROM Vagas WHERE Status = 'Livre') as VagasLivres";

            return DataAccess.ExecuteDataTable(sql, null);
        }
        // uma consulta pra marketing: ele encontra clientes que não aparecem há um certo número de dias.
        // util pra mandar uma promoção pra esse pessoal e fazer eles voltarem.
        public DataTable ListarClientesAusentes(int dias)
        {
            // este SQL agrupa por placa e pega a última vez que o cliente veio
            // filtra apenas aqueles, cuja última visita foi há mais de 15 dias
            string sql = @"
        SELECT 
            MAX(V.DataEntrada) as UltimaVisita, 
            V.Placa, 
            V.Modelo, 
            V.Telefone,
            M.Nome as Marca
        FROM Veiculos V
        LEFT JOIN Marcas M ON V.MarcaId = M.Id
        WHERE V.EmpresaId = 1 AND V.Telefone IS NOT NULL AND V.Telefone <> 'N/A'
        GROUP BY V.Placa, V.Modelo, V.Telefone, M.Nome
        HAVING MAX(V.DataEntrada) <= DATEADD(day, -@Dias, GETDATE())
        ORDER BY UltimaVisita DESC";

            var parameters = new[] { new SqlParameter("@Dias", dias) };
            return DataAccess.ExecuteDataTable(sql, parameters);
        }
    }
}
