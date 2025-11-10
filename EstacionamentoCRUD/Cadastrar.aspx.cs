using System;
using System.Data.SqlClient;

namespace Estacionamento
{
    public partial class Cadastrar : System.Web.UI.Page
    {
        string conexao = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=EstacionamentoDB;Data Source=DESKTOP-GLQ18K5";

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(conexao))
            {
                con.Open();

                // 🔎 Verifica se a placa já existe
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Veiculos WHERE Placa = @Placa", con);
                checkCmd.Parameters.AddWithValue("@Placa", txtPlaca.Text.Trim());
                int existe = (int)checkCmd.ExecuteScalar();

                if (existe > 0)
                {
                    lblMensagem.Text = "❌ Já existe um veículo com essa placa!";
                    lblMensagem.CssClass = "text-danger";
                    return;
                }

                // ✅ Inserção com hora no formato correto (TimeSpan)
                SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Veiculos (Placa, Modelo, Cor, DataEntrada, HoraEntrada, Status)
                    VALUES (@Placa, @Modelo, @Cor, @DataEntrada, @HoraEntrada, @Status)", con);

                cmd.Parameters.AddWithValue("@Placa", txtPlaca.Text.Trim());
                cmd.Parameters.AddWithValue("@Modelo", txtModelo.Text.Trim());
                cmd.Parameters.AddWithValue("@Cor", txtCor.Text.Trim());

                // Data e hora separadas corretamente
                cmd.Parameters.AddWithValue("@DataEntrada", DateTime.Now.Date);
                cmd.Parameters.AddWithValue("@HoraEntrada", DateTime.Now.ToString("HH:mm:ss")); // <- formato certo pro tipo TIME
                cmd.Parameters.AddWithValue("@Status", "Estacionado");

                cmd.ExecuteNonQuery();

                lblMensagem.Text = "✅ Veículo cadastrado com sucesso!";
                lblMensagem.CssClass = "text-success";

                // Redireciona pra página inicial
                Response.Redirect("Home.aspx");
            }
        }
    }
}
