using System;
using System.Data;
using System.Data.SqlClient;
using EstacionamentoCRUD.DAL;

namespace Estacionamento
{
    public partial class Cadastrar : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarVagasLivres();
            }
        }

        private void CarregarVagasLivres()
        {
            try
            {
                string sql = "SELECT Id, NumeroDaVaga " +
                    "FROM Vagas " +
                    "WHERE Status = 'Livre' " +
                    "ORDER BY NumeroDaVaga";
                DataTable dt = DataAccess.ExecuteDataTable(sql, null);

                ddlVagas.DataSource = dt;
                ddlVagas.DataValueField = "Id";       // O valor interno será o ID da vaga
                ddlVagas.DataTextField = "NumeroDaVaga"; // O texto que o usuário vê é o número da vaga
                ddlVagas.DataBind();

                // Adiciona um item inicial não selecionável
                ddlVagas.Items.Insert(0, new System.Web.UI.WebControls.ListItem("Selecione uma vaga...", ""));
            }
            catch (Exception ex)
            {
                lblMensagem.Text = " Erro ao carregar as vagas disponíveis.";
                lblMensagem.CssClass = "text-danger";
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim();
            string vagaId = ddlVagas.SelectedValue;

            // Validações
            if (string.IsNullOrEmpty(placa))
            {
                lblMensagem.Text = " A placa do veículo é obrigatória.";
                lblMensagem.CssClass = "text-warning";
                return;
            }
            if (string.IsNullOrEmpty(vagaId))
            {
                lblMensagem.Text = " Você precisa selecionar uma vaga.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            // Verifica se a placa já existe em um veículo ativo
            var checkParams = new[] { new SqlParameter("@Placa", placa) };
            int existe = Convert.ToInt32(DataAccess.ExecuteScalar("SELECT COUNT(*) " +
                "FROM Veiculos " +
                "WHERE Placa = @Placa " +
                "AND Status = 'Estacionado'", checkParams));

            if (existe > 0)
            {
                lblMensagem.Text = " Já existe um veículo estacionado com essa placa!";
                lblMensagem.CssClass = "text-danger";
                return;
            }

            // --- Início da Transação (Conceitual) ---
            // Em um sistema real, os dois comandos (INSERT e UPDATE) estariam dentro de uma transação
            // para garantir que ambos sejam executados com sucesso, ou nenhum deles.
            try
            {
                // 1. Insere o veículo
                var insertParams = new[]
                {
                    new SqlParameter("@Placa", placa),
                    new SqlParameter("@Modelo", txtModelo.Text.Trim()),
                    new SqlParameter("@Cor", txtCor.Text.Trim()),
                    new SqlParameter("@DataEntrada", DateTime.Now),
                    new SqlParameter("@HoraEntrada", DateTime.Now.TimeOfDay), // Salva apenas a hora
                    new SqlParameter("@Status", "Estacionado"),
                    new SqlParameter("@VagaId", Convert.ToInt32(vagaId))
                };

                string sqlInsert = @"
                    INSERT INTO Veiculos (Placa, Modelo, Cor, DataEntrada, 
                            HoraEntrada, Status, VagaId)
                    VALUES (@Placa, @Modelo, @Cor, @DataEntrada, 
                            @HoraEntrada, @Status, @VagaId)";

                DataAccess.ExecuteNonQuery(sqlInsert, insertParams);

                // 2. Ocupa a vaga
                var updateParams = new[] { new SqlParameter("@VagaId", Convert.ToInt32(vagaId)) };
                string sqlUpdate = "UPDATE Vagas SET Status = 'Ocupada' " +
                    "               WHERE Id = @VagaId";
                DataAccess.ExecuteNonQuery(sqlUpdate, updateParams);

                // --- Fim da Transação ---

                lblMensagem.Text = " Veículo cadastrado com sucesso na vaga!";
                lblMensagem.CssClass = "text-success";

                Response.AddHeader("REFRESH", "2;URL=Home.aspx");
            }
            catch (Exception ex)
            {
                lblMensagem.Text = " Erro ao salvar o veículo no banco de dados.";
                lblMensagem.CssClass = "text-danger";
                // Aqui, a lógica de rollback da transação seria chamada
            }
        }
    }
}