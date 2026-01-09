using EstacionamentoCRUD.DAL;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.WebControls;

namespace EstacionamentoCRUD
{
    public partial class RedefinirSenha : System.Web.UI.Page
    {
        // armazena o UsuarioId temporariamente
        // Isso garante que o ID do usuário seja mantido durante o postback
        // e não dependa do token após a validação inicial.
        private int _usuarioIdParaRedefinir;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //bloqueia o painel de redefinição por padrão até o token ser validado
                pnlRedefinicao.Visible = false;
                VerificarTokenNaUrl();
            }
        }

        private void VerificarTokenNaUrl()
        {
            string token = Request.QueryString["token"];

            if (string.IsNullOrEmpty(token))
            {
                ExibirMensagem("Link de redefinição inválido ou ausente.", "text-danger");
                return;
            }

            // cria o hash do token recebido da
            // URL para comparar com o hash no banco.
            string hashDoTokenRecebido = GerarHashSHA256(token);

            //faz a busca no token no banco de dados
            string sqlBuscaToken = @"
                SELECT UsuarioId, DataExpiracao, Usado
                FROM TokensRecuperacaoSenha
                WHERE HashDoToken = @HashDoToken";

            var parametrosBusca = new[] { new SqlParameter("@HashDoToken", hashDoTokenRecebido) };
            DataTable dt = DataAccess.ExecuteDataTable(sqlBuscaToken, parametrosBusca);

            if (dt.Rows.Count == 0)
            {
                ExibirMensagem("Token inválido ou não encontrado.", "text-danger");
                return;
            }

            DataRow tokenData = dt.Rows[0];
            DateTime dataExpiracao = Convert.ToDateTime(tokenData["DataExpiracao"]);
            bool usado = Convert.ToBoolean(tokenData["Usado"]);
            _usuarioIdParaRedefinir = Convert.ToInt32(tokenData["UsuarioId"]);

            // valida o token
            if (usado)
            {
                ExibirMensagem("Este link de redefinição já foi utilizado.", "text-danger");
                return;
            }
            if (dataExpiracao < DateTime.Now)
            {
                ExibirMensagem("Este link de redefinição expirou.", "text-danger");
                return;
            }

            //se o token é válido, exibe o painel de redefinição de senha
            pnlRedefinicao.Visible = true;
            lblMensagem.Text = "Token validado! Por favor, insira sua nova senha.";
            lblMensagem.CssClass = "text-info text-center";

            // salva o UsuarioId para uso no postback do botão "Redefinir"
            ViewState["ResetUsuarioId"] = _usuarioIdParaRedefinir;
        }

        protected void btnRedefinir_Click(object sender, EventArgs e)
        {
            //aqui recupera o usuario
            if (ViewState["ResetUsuarioId"] == null)
            {
                ExibirMensagem("Sessão de redefinição inválida. Por favor, solicite um novo link.", "text-danger");
                return;
            }
            _usuarioIdParaRedefinir = Convert.ToInt32(ViewState["ResetUsuarioId"]);


            string novaSenha = txtNovaSenha.Text;
            string confirmarNovaSenha = txtConfirmarNovaSenha.Text;

            // valida as senhas
            if (string.IsNullOrEmpty(novaSenha) || string.IsNullOrEmpty(confirmarNovaSenha))
            {
                ExibirMensagem("A nova senha e a confirmação são obrigatórias.", "text-warning");
                return;
            }
            if (novaSenha != confirmarNovaSenha)
            {
                ExibirMensagem("As novas senhas não conferem.", "text-danger");
                return;
            }
            if (novaSenha.Length < 6) //aqui é a força da senha,
                                      //menos de 6dig é proibido.
            {
                ExibirMensagem("A senha deve ter no mínimo 6 caracteres.", "text-warning");
                return;
            }

            try
            {
                //criação de novo Hash e Salt para a senha
                (byte[] hashNovaSenha, byte[] salNovaSenha) = PasswordHasher.HashPassword(novaSenha);

                //atualiza a senha do usuário
                string sqlAtualizaSenha = @"
                    UPDATE Usuarios
                    SET HashDaSenha = @HashNovaSenha, SalDaSenha = @SalNovaSenha
                    WHERE Id = @Id";

                var parametrosAtualizaSenha = new[]
                {
                    new SqlParameter("@HashNovaSenha", hashNovaSenha),
                    new SqlParameter("@SalNovaSenha", salNovaSenha),
                    new SqlParameter("@Id", _usuarioIdParaRedefinir)
                };

                DataAccess.ExecuteNonQuery(sqlAtualizaSenha, parametrosAtualizaSenha);

                // aqui invalida o token usado
                string tokenHashDaUrl = GerarHashSHA256(Request.QueryString["token"]); // Pega o hash do token atual
                string sqlInvalidaToken = @"
                    UPDATE TokensRecuperacaoSenha
                    SET Usado = 1
                    WHERE HashDoToken = @HashDoToken";

                var parametrosInvalidaToken = new[]
                {
                    new SqlParameter("@HashDoToken", tokenHashDaUrl)
                };

                DataAccess.ExecuteNonQuery(sqlInvalidaToken, parametrosInvalidaToken);


                ExibirMensagem("Senha redefinida com sucesso! Você já pode fazer o login.", "text-success");
                Response.AddHeader("REFRESH", "1;URL=Login.aspx"); // Redireciona após 1 segundos
            }
            catch (Exception)
            {
                ExibirMensagem("Ocorreu um erro inesperado ao redefinir a senha. Tente novamente.", "text-danger");
            }
        }

        private void ExibirMensagem(string mensagem, string cssClass)
        {
            lblMensagem.Text = mensagem;
            lblMensagem.CssClass = cssClass + " text-center";
            pnlRedefinicao.Visible = false; //painel de redefinição fica invisível em caso de erro
        }

        // cria um hash SHA256 para o token, usado para comparar com o hash no banco.
        private string GerarHashSHA256(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                var builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
