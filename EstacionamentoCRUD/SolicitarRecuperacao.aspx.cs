using EstacionamentoCRUD.DAL;
using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI.WebControls;

namespace EstacionamentoCRUD
{
    public partial class SolicitarRecuperacao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                pnlLink.Visible = false;
                lblMensagem.Text = "";
            }
        }

        protected void btnSolicitar_Click(object sender, EventArgs e)
        {
            string nomeDeUsuario = txtUsuario.Text.Trim();

            if (string.IsNullOrEmpty(nomeDeUsuario))
            {
                ExibirMensagem("Por favor, informe o nome de usuário.", "text-warning");
                return;
            }

            try
            {
                // pegar o ID do usuário.
                string sqlBuscaId = "SELECT Id FROM Usuarios WHERE Usuario = @Usuario";
                var parametrosBusca = new[] { new SqlParameter("@Usuario", nomeDeUsuario) };
                object usuarioIdObj = DataAccess.ExecuteScalar(sqlBuscaId, parametrosBusca);

                if (usuarioIdObj == null)
                {
                    ExibirMensagem("Usuário não encontrado.", "text-danger");
                    return;
                }
                int usuarioId = Convert.ToInt32(usuarioIdObj);

                //  aqui gera um token de recuperação único.
                string token = GerarTokenSeguro();
                string hashDoToken = GerarHashSHA256(token);
                DateTime dataDeExpiracao = DateTime.Now.AddHours(1);

                // salva o Hash do token na tabela
                string sqlInsertToken = @"
                    INSERT INTO TokensRecuperacaoSenha (HashDoToken, UsuarioId, DataExpiracao, Usado)
                    VALUES (@HashDoToken, @UsuarioId, @DataExpiracao, 0)";
                var parametrosInsert = new[]
                {
                    new SqlParameter("@HashDoToken", hashDoToken),
                    new SqlParameter("@UsuarioId", usuarioId),
                    new SqlParameter("@DataExpiracao", dataDeExpiracao)
                };
                DataAccess.ExecuteNonQuery(sqlInsertToken, parametrosInsert);

                // exibe o link de redefinição para o usuário.
                string urlBase = Request.Url.GetLeftPart(UriPartial.Authority);
                string linkDeRecuperacao = $"{urlBase}/RedefinirSenha.aspx?token={token}";

                ExibirLinkDeRecuperacao(linkDeRecuperacao);
            }
            catch (Exception)
            {
                ExibirMensagem("Ocorreu um erro inesperado ao processar sua solicitação. Tente novamente.", "text-danger");
            }
        }

        private void ExibirMensagem(string mensagem, string cssClass)
        {
            lblMensagem.Text = mensagem;
            lblMensagem.CssClass = cssClass + " text-center";
            pnlLink.Visible = false;
        }

        private void ExibirLinkDeRecuperacao(string link)
        {
            lblMensagem.Text = "Para redefinir sua senha, copie e use o link abaixo:";
            lblMensagem.CssClass = "alert alert-success text-center";

            hlRecuperacao.NavigateUrl = link;
            hlRecuperacao.Text = link;

            pnlLink.Visible = true;
        }

        private string GerarTokenSeguro()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] tokenData = new byte[32];
                rng.GetBytes(tokenData);
                return Convert.ToBase64String(tokenData).TrimEnd('=').Replace('+', '-').Replace('/', '_');
            }
        }

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