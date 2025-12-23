using System;
using System.Data;
using System.Data.SqlClient;
using EstacionamentoCRUD.DAL;

namespace EstacionamentoCRUD
{
    public partial class RedefinirSenha : System.Web.UI.Page
    {
        string stringDeConexao = "Integrated Security=SSPI;" +
            "Persist Security Info=False;" +
            "Initial Catalog=EstacionamentoDB;" +
            "Data Source=DESKTOP-GLQ18K5";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Verifica se um token foi passado pela URL
                if (!string.IsNullOrEmpty(Request.QueryString["token"]))
                {
                    string token = Request.QueryString["token"];
                    txtToken.Text = token;
                    txtToken.ReadOnly = true; // Trava o campo para evitar que o usuário o altere
                }
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            string token = txtToken.Text.Trim();
            string novaSenha = txtNovaSenha.Text;
            string confirmarNovaSenha = txtConfirmarNovaSenha.Text;

            // 1. Validações iniciais
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(novaSenha))
            {
                lblMensagem.Text = " Todos os campos são obrigatórios.";
                lblMensagem.CssClass = "text-warning text-center";
                return;
            }

            if (novaSenha != confirmarNovaSenha)
            {
                lblMensagem.Text = " As novas senhas não conferem.";
                lblMensagem.CssClass = "text-danger text-center";
                return;
            }

            try
            {
                using (SqlConnection conexao = new SqlConnection(stringDeConexao))
                {
                    conexao.Open();
                    int usuarioId = 0;

                    // 2. Valida o token: verifica se ele existe e não está expirado
                    string sqlValidaToken = "SELECT Id FROM Usuarios " +
                        "WHERE TokenDeRecuperacao = @Token " +
                        "AND ValidadeDoToken > GETDATE()";
                    using (SqlCommand comandoValida = new SqlCommand(sqlValidaToken, conexao))
                    {
                        comandoValida.Parameters.AddWithValue("@Token", token);
                        var resultado = comandoValida.ExecuteScalar(); // Retorna o Id ou null

                        if (resultado == null || resultado == DBNull.Value)
                        {
                            lblMensagem.Text = " Token inválido ou expirado.";
                            lblMensagem.CssClass = "text-danger text-center";
                            return;
                        }
                        usuarioId = Convert.ToInt32(resultado);
                    }

                    // 3. Se o token é válido, cria o hash da nova senha
                    (byte[] hashNovaSenha, byte[] salNovaSenha) = PasswordHasher.HashPassword(novaSenha);

                    // 4. Atualiza a senha e invalida o token para não ser usado novamente
                    string sqlAtualizaSenha = @"UPDATE Usuarios 
                                                SET HashDaSenha = @HashNovaSenha, 
                                                    SalDaSenha = @SalNovaSenha, 
                                                    TokenDeRecuperacao = NULL, 
                                                    ValidadeDoToken = NULL
                                                WHERE Id = @Id";

                    using (SqlCommand comandoAtualiza = new SqlCommand(sqlAtualizaSenha, conexao))
                    {
                        comandoAtualiza.Parameters.AddWithValue("@HashNovaSenha", hashNovaSenha);
                        comandoAtualiza.Parameters.AddWithValue("@SalNovaSenha", salNovaSenha);
                        comandoAtualiza.Parameters.AddWithValue("@Id", usuarioId);
                        comandoAtualiza.ExecuteNonQuery();
                    }
                }

                lblMensagem.Text = " Senha redefinida com sucesso! Você já pode fazer o login.";
                lblMensagem.CssClass = "text-success text-center";
                Response.AddHeader("REFRESH", "3;URL=Login.aspx");
            }
            catch (Exception erro)
            {
                lblMensagem.Text = " Ocorreu um erro inesperado ao redefinir a senha.";
                lblMensagem.CssClass = "text-danger text-center";
            }
        }
    }
}
