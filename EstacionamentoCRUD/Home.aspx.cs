using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using EstacionamentoCRUD.DAL;

namespace Estacionamento
{
    public partial class Home : Page
    {
        public string HorizonteJsonData;
        public string AlertasPreditivos;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarHorizonteAtividade();
            }
           
            CarregarVeiculos();
        }

        private void CarregarHorizonteAtividade()
        {
            DataTable dt = DataAccess.GetPrevisaoDeAtividadePorHora();
            var atividadePorHora = new Dictionary<int, int>();

            //preenche todas as horas com 0 atividade
            for (int i = 0; i < 24; i++)
            {
                atividadePorHora[i] = 0;
            }

            //completa com os dados do banco
            foreach (DataRow row in dt.Rows)
            {
                atividadePorHora[Convert.ToInt32(row["Hora"])] =
                    Convert.ToInt32(row["TotalAtividade"]);
            }

            int maxAtividade = atividadePorHora.Values.Count > 0 ? atividadePorHora.Values.Max() : 1;
            if (maxAtividade == 0) maxAtividade = 1; //nada de divisão por zero

            var timelineData = atividadePorHora.Select(kvp => new
            {
                Hora = kvp.Key,
                TotalAtividade = kvp.Value,
                Calor = Math.Round((double)kvp.Value / maxAtividade, 2) //nivel de calor de 0.0 a 1.0
            }).ToList();

            //aqui é pra serializar pra Json
            var serializer = new JavaScriptSerializer();
            HorizonteJsonData = serializer.Serialize(timelineData);

            //gera alertas
            var picos = timelineData.Where(d => d.Calor > 0.6).OrderByDescending(d => d.TotalAtividade).Take(3);
            if (picos.Any())
            {
                var horariosPico = string.Join(", ", picos.Select(p => $"{p.Hora}h"));
                AlertasPreditivos = $"Atenção aos horários de pico de atividade previstos para: " +
                    $"<strong>{horariosPico}</strong>.";
            }
            else
            {
                AlertasPreditivos = "Nenhum pico de atividade significativo foi detectado com base no histórico.";
            }
        }

        //aqui é pra carregar
        //os veiculos estacionados
        private void CarregarVeiculos()
        {
            string sql = @"
        SELECT 
            V.Id,
            V.Placa,
            M.Nome AS Marca,
            V.Modelo,
            V.Status,
            V.StatusLavagem,
            S.NomeServico,
            VG.NumeroDaVaga,
            V.DataEntrada,
            V.HoraEntrada
        FROM 
            Veiculos V
            LEFT JOIN Vagas VG ON V.VagaId = VG.Id
            LEFT JOIN Marcas M ON V.MarcaId = M.Id
            LEFT JOIN Servicos S ON V.ServicoId = S.Id
        WHERE
            V.Ativo = 1
        ORDER BY 
            V.DataEntrada DESC, 
            V.HoraEntrada DESC";

            DataTable dt = DataAccess.ExecuteDataTable(sql, null);

           
            if (dt != null)
            {
                // Conta quantos carros estão ativos no pátio
                litPatio.Text = dt.Rows.Count.ToString();

                // Conta quantos desses ativos possuem StatusLavagem como 'Pendente' ou 'Aguardando'
                
                int pendentes = dt.AsEnumerable().Count(row =>
     row["NomeServico"].ToString() != "Estacionamento" &&
     (row["StatusLavagem"].ToString() == "Pendente" ||
      row["StatusLavagem"].ToString() == "Aguardando"));

                litPendentes.Text = pendentes.ToString();

                // CLIENTES AUSENTES NA HOME
                string sqlAusentes = @"
    SELECT COUNT(*)
    FROM (
        -- Primeiro, encontra a última DataEntrada para cada cliente (Placa, Telefone)
        SELECT Placa, Telefone, MAX(DataEntrada) AS UltimaEntrada
        FROM Veiculos
        WHERE Telefone IS NOT NULL AND Telefone <> 'N/A'
        GROUP BY Placa, Telefone
    ) AS AllCustomers
    WHERE AllCustomers.UltimaEntrada <= DATEADD(day, -15, GETDATE()) -- Clientes cuja última entrada foi há mais de 15 dias
      AND NOT EXISTS ( -- E que NÃO possuem nenhum veículo ativo no pátio AGORA
          SELECT 1
          FROM Veiculos AS V_ACTIVE
          WHERE V_ACTIVE.Placa = AllCustomers.Placa
            AND V_ACTIVE.Telefone = AllCustomers.Telefone
            AND V_ACTIVE.Ativo = 1
            AND V_ACTIVE.Status = 'Estacionado'
      );";

                try
                {
                    // Busca o total de clientes sumidos para alertar na Home
                    object total = DataAccess.ExecuteScalar(sqlAusentes, null);
                    litAusentes.Text = total != null ? total.ToString() : "0";
                }
                catch
                {
                    litAusentes.Text = "0";
                }

                gvVeiculos.DataSource = dt;
                gvVeiculos.DataBind();
            }
        }
    }
}