using System;
using System.Data.SqlClient;

namespace EstacionamentoCRUD
{
    public partial class Editar : System.Web.UI.Page
    {
        string connectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=EstacionamentoDB;Data Source=DESKTOP-GLQ18K5";

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim();

            if (string.IsNullOrEmpty(placa))
            {
                lblMensagem.Text = "⚠️ Digite a placa do veículo.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = "SELECT Modelo, Cor, DataEntrada, HoraEntrada FROM Veiculos WHERE Placa = @Placa";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Placa", placa);

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    txtModelo.Text = dr["Modelo"].ToString();
                    txtCor.Text = dr["Cor"].ToString();
                    txtDataEntrada.Text = Convert.ToDateTime(dr["DataEntrada"]).ToString("yyyy-MM-dd");
                    txtHoraEntrada.Text = dr["HoraEntrada"].ToString();
                    lblMensagem.Text = "✅ Veículo encontrado! Você pode editar os dados.";
                    lblMensagem.CssClass = "text-success";
                }
                else
                {
                    lblMensagem.Text = "❌ Veículo não encontrado.";
                    lblMensagem.CssClass = "text-danger";
                }
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim();
            string modelo = txtModelo.Text.Trim();
            string cor = txtCor.Text.Trim();
            string dataEntrada = txtDataEntrada.Text;
            string horaEntrada = txtHoraEntrada.Text;

            if (string.IsNullOrEmpty(placa) || string.IsNullOrEmpty(modelo))
            {
                lblMensagem.Text = "⚠️ Preencha todos os campos obrigatórios.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = @"UPDATE Veiculos 
                               SET Modelo=@Modelo, Cor=@Cor, DataEntrada=@DataEntrada, HoraEntrada=@HoraEntrada
                               WHERE Placa=@Placa";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Modelo", modelo);
                cmd.Parameters.AddWithValue("@Cor", cor);
                cmd.Parameters.AddWithValue("@DataEntrada", dataEntrada);
                cmd.Parameters.AddWithValue("@HoraEntrada", horaEntrada);
                cmd.Parameters.AddWithValue("@Placa", placa);

                int rows = cmd.ExecuteNonQuery();
                if (rows > 0)
                {
                    lblMensagem.Text = "✅ Alterações salvas com sucesso!";
                    lblMensagem.CssClass = "text-success";
                    Response.Redirect("Home.aspx");
                }
                else
                {
                    lblMensagem.Text = "❌ Erro ao salvar alterações. Verifique a placa.";
                    lblMensagem.CssClass = "text-danger";
                }
            }
        }
    }
}
