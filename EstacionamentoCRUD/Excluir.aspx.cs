using System;
using System.Data.SqlClient;

namespace EstacionamentoCRUD
{
    public partial class Excluir : System.Web.UI.Page
    {
        string connectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=EstacionamentoDB;Data Source=DESKTOP-GLQ18K5";

        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            string placa = txtPlaca.Text.Trim();

            if (string.IsNullOrEmpty(placa))
            {
                lblMensagem.Text = "⚠️ Digite a placa.";
                lblMensagem.CssClass = "text-warning";
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = "SELECT * FROM Veiculos WHERE Placa = @Placa";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Placa", placa);

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    lblMensagem.Text = "⚠️ Veículo encontrado. Clique em 'Excluir' para confirmar.";
                    lblMensagem.CssClass = "text-danger";
                }
                else
                {
                    lblMensagem.Text = "❌ Veículo não encontrado.";
                    lblMensagem.CssClass = "text-danger";
                }
            }
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = "DELETE FROM Veiculos WHERE Placa = @Placa";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Placa", txtPlaca.Text);

                int rows = cmd.ExecuteNonQuery();

                if (rows > 0)
                {
                    lblMensagem.Text = "✅ Veículo excluído com sucesso!";
                    lblMensagem.CssClass = "text-success";
                    Response.Redirect("Home.aspx");
                }
                else
                {
                    lblMensagem.Text = "❌ Erro: veículo não encontrado.";
                    lblMensagem.CssClass = "text-danger";
                }
            }
        }
    }
}
