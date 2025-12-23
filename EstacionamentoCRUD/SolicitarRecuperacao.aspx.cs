using System;
using System.Data.SqlClient;

namespace EstacionamentoCRUD
{
    public partial class SolicitarRecuperacao : System.Web.UI.Page
    {
        string stringDeConexao = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=EstacionamentoDB;Data Source=DESKTOP-GLQ18K5";

        protected void btnSolicitar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();

            if (string.IsNullOrEmpty(usuario))
            {
                lblMensagem.Text = " Por favor, informe o nome de usuário.";
                lblMensagem.CssClass = "text-warning text-center";
                return;
            }

            try
            {
                using (SqlConnection conexao = new SqlConnection(stringDeConexao))
                {
                    conexao.Open();

                    // 1. Verifica se o usuário realmente existe
                    string sqlVerifica = "SELECT COUNT(*) FROM Usuarios " +
                        "WHERE Usuario = @Usuario";
                    int usuarioExiste;
                    using (SqlCommand comandoVerifica = new SqlCommand(sqlVerifica, conexao))
                    {
                        comandoVerifica.Parameters.AddWithValue("@Usuario", usuario);
                        usuarioExiste = (int)comandoVerifica.ExecuteScalar();
                    }

                    if (usuarioExiste == 0)
                    {
                        lblMensagem.Text = " Usuário não encontrado.";
                        lblMensagem.CssClass = "text-danger text-center";
                        return;
                    }

                    // 2. Gera um token único e define sua validade (1 hora)
                    string token = Guid.NewGuid().ToString();
                    DateTime dataDeExpiracao = DateTime.Now.AddHours(1);

                    // 3. Salva o token e a data de expiração no banco de dados para este usuário
                    string sqlAtualiza = "UPDATE Usuarios " +
                        "SET TokenDeRecuperacao = @Token, ValidadeDoToken = @DataDeExpiracao " +
                        "WHERE Usuario = @Usuario";
                    using (SqlCommand comandoAtualiza = new SqlCommand(sqlAtualiza, conexao))
                    {
                        comandoAtualiza.Parameters.AddWithValue("@Token", token);
                        comandoAtualiza.Parameters.AddWithValue("@DataDeExpiracao", dataDeExpiracao);
                        comandoAtualiza.Parameters.AddWithValue("@Usuario", usuario);
                        comandoAtualiza.ExecuteNonQuery();
                    }

                    // 4. Redireciona para a página de redefinição com o token na URL
                    Response.Redirect("RedefinirSenha.aspx?token=" + token);
                }
            }
            catch (Exception erro)
            {
                lblMensagem.Text = " Ocorreu um erro inesperado.";
                lblMensagem.CssClass = "text-danger text-center";
            }
        }
    }
}
