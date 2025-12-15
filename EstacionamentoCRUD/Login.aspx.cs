using System;
using System.Data.SqlClient;

namespace EstacionamentoCRUD
{
    public partial class Login : System.Web.UI.Page
    {
        string connectionString = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=EstacionamentoDB;Data Source=DESKTOP-GLQ18K5";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblMensagem.Text = "";
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string senha = txtSenha.Text.Trim();

            if (usuario == "" || senha == "")
            {
                lblMensagem.Text = "⚠️ Preencha todos os campos.";
                lblMensagem.CssClass = "text-warning text-center";
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string sql = "SELECT COUNT(*) FROM Usuarios WHERE Usuario = @Usuario AND Senha = @Senha";
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@Usuario", usuario);
                cmd.Parameters.AddWithValue("@Senha", senha);

                int count = (int)cmd.ExecuteScalar();

                if (count > 0)
                {
                    lblMensagem.Text = "✅ Login realizado com sucesso!";
                    lblMensagem.CssClass = "text-success text-center";
                    Response.Redirect("Home.aspx");
                }
                else
                {
                    lblMensagem.Text = "❌ Usuário ou senha inválidos.";
                    lblMensagem.CssClass = "text-danger text-center";
                }
            }
        }
    }
}