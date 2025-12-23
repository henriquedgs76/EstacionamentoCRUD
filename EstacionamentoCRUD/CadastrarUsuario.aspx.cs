using System;
using System.Data.SqlClient;
using EstacionamentoCRUD.DAL;

namespace EstacionamentoCRUD
{
    public partial class CadastrarUsuario : System.Web.UI.Page
    {
        // É uma boa prática mover a string de conexão para o arquivo Web.config,
        // mas, por enquanto, a manteremos aqui para ser consistente com sua página de Login.
        string stringDeConexao = "Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=EstacionamentoDB;Data Source=DESKTOP-GLQ18K5";

        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string senha = txtSenha.Text;
            string confirmarSenha = txtConfirmarSenha.Text;

            // 1. Validações Iniciais
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(senha))
            {
                lblMensagem.Text = " Usuário e senha são obrigatórios.";
                lblMensagem.CssClass = "text-warning text-center";
                return;
            }

            if (senha != confirmarSenha)
            {
                lblMensagem.Text = " As senhas não conferem.";
                lblMensagem.CssClass = "text-danger text-center";
                return;
            }

            try
            {
                using (SqlConnection conexao = new SqlConnection(stringDeConexao))
                {
                    conexao.Open();

                    // 2. Verifica se o nome de usuário já existe
                    string sqlVerificaUsuario = "SELECT COUNT(*) FROM Usuarios " +
                        "WHERE Usuario = @Usuario";
                    using (SqlCommand comandoVerifica = new SqlCommand(sqlVerificaUsuario, conexao))
                    {
                        comandoVerifica.Parameters.AddWithValue("@Usuario", usuario);
                        int usuarioExiste = (int)comandoVerifica.ExecuteScalar();
                        if (usuarioExiste > 0)
                        {
                            lblMensagem.Text = "❌ Este nome de usuário já está em uso.";
                            lblMensagem.CssClass = "text-danger text-center";
                            return;
                        }
                    }

                    // 3. Cria o Hash e o Salt da senha (usando a classe que definimos)
                    (byte[] hash, byte[] salt) = PasswordHasher.HashPassword(senha);

                    // 4. Insere o novo usuário no banco de dados
                    string sqlInserir = "INSERT INTO Usuarios (Usuario, HashDaSenha, SalDaSenha) " +
                        "VALUES (@Usuario, @HashDaSenha, @SalDaSenha)";
                    using (SqlCommand comandoInserir = new SqlCommand(sqlInserir, conexao))
                    {
                        comandoInserir.Parameters.AddWithValue("@Usuario", usuario);
                        comandoInserir.Parameters.AddWithValue("@HashDaSenha", hash);
                        comandoInserir.Parameters.AddWithValue("@SalDaSenha", salt);

                        comandoInserir.ExecuteNonQuery();
                    }
                }

                lblMensagem.Text = " Usuário cadastrado com sucesso!";
                lblMensagem.CssClass = "text-success text-center";

                // Redireciona para a página de login após 2 segundos
                Response.AddHeader("REFRESH", "2;URL=Login.aspx");
            }
            catch (Exception erro)
            {
                // Em um caso real, seria bom registrar o erro em um arquivo de log (erro.Message)
                lblMensagem.Text = " Ocorreu um erro inesperado ao cadastrar.";
                lblMensagem.CssClass = "text-danger text-center";
            }
        }
    }
}