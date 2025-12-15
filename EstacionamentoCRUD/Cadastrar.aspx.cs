using System;
using System.Data.SqlClient;
using EstacionamentoCRUD.DAL;

namespace Estacionamento
{
    public partial class Cadastrar : System.Web.UI.Page
    {
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim();

            //  Verifica se a placa já existe
            var checkParams = new[] { new SqlParameter("@Placa", placa) };
            int existe = Convert.ToInt32(DataAccess.ExecuteScalar("SELECT COUNT(*) FROM Veiculos WHERE Placa = @Placa AND Status = 'Estacionado'", checkParams));

            if (existe > 0)
            {
                lblMensagem.Text = "❌ Já existe um veículo estacionado com essa placa!";
                lblMensagem.CssClass = "text-danger";
                return;
            }

            // Inserção com hora no formato correto (TimeSpan)
            var insertParams = new[]
            {
                new SqlParameter("@Placa", placa),
                new SqlParameter("@Modelo", txtModelo.Text.Trim()),
                new SqlParameter("@Cor", txtCor.Text.Trim()),
                new SqlParameter("@DataEntrada", DateTime.Now.Date),
                new SqlParameter("@HoraEntrada", DateTime.Now),
                new SqlParameter("@Status", "Estacionado")
            };

            string sql = @"
                INSERT INTO Veiculos (Placa, Modelo, Cor, DataEntrada, HoraEntrada, Status)
                VALUES (@Placa, @Modelo, @Cor, @DataEntrada, @HoraEntrada, @Status)";

            DataAccess.ExecuteNonQuery(sql, insertParams);

            lblMensagem.Text = "✅ Veículo cadastrado com sucesso!";
            lblMensagem.CssClass = "text-success";

            // Redireciona pra página inicial após 2 segundos
            Response.AddHeader("REFRESH", "2;URL=Home.aspx");
        }
    }
}
