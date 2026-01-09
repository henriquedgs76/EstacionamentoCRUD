using EstacionamentoCRUD.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Web.Script.Serialization;

namespace EstacionamentoCRUD
{
    public partial class Dashboard : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            // Primeiro, a gente faz uma checagem de segurança pra ver se o usuário tá logado...
            if (Session["UsuarioLogado"] == null || Session["PerfilId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            // ... e se ele tem permissão pra ver essa tela (só Admin e Supervisor podem).
            int perfilId = (int)Session["PerfilId"];
            if (perfilId != 1 && perfilId != 3)
            {
                Response.Redirect("~/Home.aspx");
                return;
            }

            // Se tiver tudo certo e for o primeiro carregamento da página, a gente manda carregar os KPIs e os gráficos.
            if (!IsPostBack)
            {
                CarregarDashboard(7);
                CarregarKPIs();
            }
        }
        #region KPIs
        // Esse é o método que busca todos aqueles números grandes que ficam no topo do dashboard.
        private void CarregarKPIs()
        {
            // Entradas no Dia 
            string sqlEntradasHoje = @"
    SELECT COUNT(*)
    FROM Veiculos
    WHERE DataEntrada >= CAST(GETDATE() AS DATE)
    AND DataEntrada < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
";
            int entradasHoje = Convert.ToInt32(DataAccess.ExecuteScalar(sqlEntradasHoje, null));
            lblEntradasHoje.Text = entradasHoje.ToString();


            //Faturamento do Dia ---
            string sqlFaturamento = @"
        SELECT ISNULL(SUM(ValorPago), 0)
        FROM Veiculos
        WHERE CAST(DataSaida AS DATE) = CAST(GETDATE() AS DATE)
        AND Status = 'Finalizado'";
            decimal faturamentoHoje = Convert.ToDecimal(DataAccess.ExecuteScalar(sqlFaturamento, null));
            lblFaturamentoHoje.Text = faturamentoHoje.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));


            //Veículos Ativos no Pátio
            string sqlAtivos = @"
SELECT COUNT(*) 
FROM Veiculos
WHERE Status = 'Estacionado' AND Ativo = 1";
            lblVeiculosAtivos.Text = DataAccess.ExecuteScalar(sqlAtivos, null).ToString();


            //Taxa de Ocupação ---
            string sqlTotalVagas = "SELECT COUNT(*) FROM Vagas";
            int totalVagas = Convert.ToInt32(DataAccess.ExecuteScalar(sqlTotalVagas, null));
            int veiculosAtivos = Convert.ToInt32(DataAccess.ExecuteScalar(sqlAtivos, null));
            decimal ocupacao = totalVagas == 0 ? 0 : (decimal)veiculosAtivos / totalVagas * 100;
            lblOcupacao.Text = ocupacao.ToString("0.0") + "%";


            // Clientes Ausentes (não vêm há 15 dias) 
            string sqlAusentes = @"
    SELECT COUNT(*) FROM (
        SELECT V.Telefone
        FROM Veiculos V
        WHERE V.Telefone IS NOT NULL AND V.Telefone <> 'N/A'
        GROUP BY V.Placa, V.Telefone
        HAVING MAX(V.DataEntrada) <= DATEADD(day, -15, GETDATE())
    ) AS ClientesSumidos";
            int totalAusentes = Convert.ToInt32(DataAccess.ExecuteScalar(sqlAusentes, null));
            if (lblClientesAusentes != null)
            {
                lblClientesAusentes.Text = totalAusentes.ToString();
            }


            // Ticket Médio do Dia 
            string sqlTicket = @"
        SELECT ISNULL(AVG(ValorPago), 0)
FROM Veiculos
WHERE Status = 'Finalizado'
AND ValorPago > 0
AND DataSaida >= CAST(GETDATE() AS DATE)
AND DataSaida < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))
";
            decimal ticketMedio = Convert.ToDecimal(DataAccess.ExecuteScalar(sqlTicket, null));
            lblTicketMedio.Text = ticketMedio.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));

        }
        #endregion

        #region Graficos

        // Prepara os dados pro gráfico de faturamento.
        private void CarregarFaturamentoPorDia(int dias)
        {
            // Busca no banco quanto foi faturado em cada um dos últimos X dias.
            string sql = @"
        SELECT CAST(DataSaida AS DATE) AS Dia, SUM(ValorPago) AS Total
        FROM Veiculos
        WHERE Status = 'Finalizado'
        AND DataSaida >= DATEADD(DAY, -@dias, CAST(GETDATE() AS DATE))
        GROUP BY CAST(DataSaida AS DATE)
        ORDER BY Dia";

            var parameters = new[] { new System.Data.SqlClient.SqlParameter("@dias", dias) };
            DataTable dt = DataAccess.ExecuteDataTable(sql, parameters);

            var labels = new List<string>();
            var values = new List<decimal>();

            foreach (DataRow row in dt.Rows)
            {
                labels.Add(Convert.ToDateTime(row["Dia"]).ToString("dd/MM"));
                values.Add(Convert.ToDecimal(row["Total"]));
            }
            // Transforma esses dados num formato JSON que o plugin do gráfico entende.
            hfFaturamento.Value = ConverterParaJson(labels, values);
        }

        // Prepara os dados pro gráfico de fluxo de veículos.
        private void CarregarVeiculosPorDia(int dias)
        {
            // Ele faz uma consulta mais complexa pra contar quantas entradas e saídas aconteceram em cada um dos últimos X dias.
            string sql = @"
        SELECT Dia, SUM(Entradas) AS TotalEntradas, SUM(Saidas) AS TotalSaidas
        FROM (
            SELECT CAST(DataEntrada AS DATE) AS Dia, 1 AS Entradas, 0 AS Saidas
            FROM Veiculos
            WHERE DataEntrada >= DATEADD(DAY, -@dias, CAST(GETDATE() AS DATE))
            UNION ALL
            SELECT CAST(DataSaida AS DATE) AS Dia, 0 AS Entradas, 1 AS Saidas
            FROM Veiculos
            WHERE DataSaida >= DATEADD(DAY, -@dias, CAST(GETDATE() AS DATE))
        ) AS SubQuery
        WHERE Dia IS NOT NULL
        GROUP BY Dia
        ORDER BY Dia;";

            var parameters = new[] { new System.Data.SqlClient.SqlParameter("@dias", dias) };
            DataTable dt = DataAccess.ExecuteDataTable(sql, parameters);

            var labels = new List<string>();
            var entradas = new List<int>();
            var saidas = new List<int>();

            foreach (DataRow row in dt.Rows)
            {
                labels.Add(Convert.ToDateTime(row["Dia"]).ToString("dd/MM"));
                entradas.Add(Convert.ToInt32(row["TotalEntradas"]));
                saidas.Add(Convert.ToInt32(row["TotalSaidas"]));
            }

            // Também converte tudo pra JSON pro gráfico poder desenhar as linhas.
            var chartData = new { labels = labels, entradas = entradas, saidas = saidas };
            hfVeiculos.Value = new JavaScriptSerializer().Serialize(chartData);
        }
        #endregion


        // Quando o usuário muda o período do filtro "Últimos 7 dias" para "Últimos 30 dias"
        protected void ddlPeriodo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // ...a gente chama os métodos de carregar os dados de novo, mas agora passando o novo número de dias.
            int dias = Convert.ToInt32(ddlPeriodo.SelectedValue);
            CarregarDashboard(dias);
            CarregarKPIs();
        }
        // Um método "organizador". Ele só serve pra chamar os outros métodos que carregam os dados dos gráficos.
        private void CarregarDashboard(int dias)
        {
            CarregarFaturamentoPorDia(dias);
            CarregarVeiculosPorDia(dias);
            CarregarStatusVeiculos();
        }

       
        private void CarregarStatusVeiculos()
        {
            // Ele conta quantos veículos estão com status 'Estacionado' e quantos estão 'Finalizado' pra gente ter uma ideia da proporção.
            string sql = @"
                SELECT Status, COUNT(*) AS Total
                FROM Veiculos
                GROUP BY Status";

            DataTable dt = DataAccess.ExecuteDataTable(sql, null);

            int ativos = 0;
            int finalizados = 0;

            foreach (DataRow row in dt.Rows)
            {
                string status = row["Status"].ToString().Trim();

                if (status.Equals("Estacionado", StringComparison.OrdinalIgnoreCase))
                {
                    ativos = Convert.ToInt32(row["Total"]);
                }
                else if (status.Equals("Finalizado", StringComparison.OrdinalIgnoreCase))
                {
                    finalizados = Convert.ToInt32(row["Total"]);
                }
            }

            // Manda os dados pro gráfico no formato que ele entende.
            if (hfStatus != null)
            {
                var serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                hfStatus.Value = serializer.Serialize(new int[] { ativos, finalizados });
            }
        }

        protected void btnVoltarHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Home.aspx");
        }

        // Uma funçãozinha útil pra converter nossas listas de dados em um texto no formato JSON.
        // O JavaScript que desenha os gráficos lá na página precisa dos dados nesse formato.
        private string ConverterParaJson<T>(List<string> labels, List<T> values)
        {
            var serializer = new JavaScriptSerializer();

            var obj = new
            {
                labels = labels,
                values = values
            };

            return serializer.Serialize(obj);
        }
    }
}
