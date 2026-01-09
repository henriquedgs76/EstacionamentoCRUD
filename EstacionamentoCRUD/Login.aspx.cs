using System;
using System.Data;
using System.Data.SqlClient;
using EstacionamentoCRUD.DAL;

namespace EstacionamentoCRUD
{
    public partial class Login : System.Web.UI.Page
    {
        // Quando a página carrega pela primeira vez, limpa qualquer mensagem que possa ter ficado da última vez.
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

            // Primeiro, vê se o usuário digitou alguma coisa nos campos de usuário e senha.
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(senha))
            {
                lblMensagem.Text = "Preencha todos os campos.";
                lblMensagem.CssClass = "text-warning text-center";
                return;
            }

            try
            {
                // Busca no banco de dados as informações do usuário, principalmente a senha criptografada (hash e sal).
                string sql = @"
            SELECT 
                U.Id,
                U.Usuario,
                U.HashDaSenha,
                U.SalDaSenha,
                U.PerfilId,
                P.Nome AS NivelAcesso
            FROM Usuarios U
            INNER JOIN Perfis P ON U.PerfilId = P.Id
            WHERE U.Usuario = @Usuario";

                var parameters = new[] { new SqlParameter("@Usuario", usuario) };
                DataTable dt = DataAccess.ExecuteDataTable(sql, parameters);

                // Se não encontrar ninguém com aquele nome de usuário, já avisa que tá errado.
                if (dt.Rows.Count == 0)
                {
                    lblMensagem.Text = "Usuário ou senha inválidos.";
                    lblMensagem.CssClass = "text-danger text-center";
                    return;
                }

                DataRow row = dt.Rows[0];

                byte[] hashSalvo = (byte[])row["HashDaSenha"];
                byte[] salSalvo = (byte[])row["SalDaSenha"];

                // Pega a senha que o usuário digitou e o hash/sal que vieram do banco e usa o nosso PasswordHasher pra ver se batem.
                if (!PasswordHasher.VerifyPassword(senha, hashSalvo, salSalvo))
                {
                    // Se o PasswordHasher falar que a senha tá errada, avisa o usuário.
                    lblMensagem.Text = "Usuário ou senha inválidos.";
                    lblMensagem.CssClass = "text-danger text-center";
                    return;
                }

                // A Sessão serve pra que o sistema "lembre" que o usuário tá logado enquanto ele navega pelas páginas.
                Session["UsuarioId"] = Convert.ToInt32(row["Id"]);
                Session["UsuarioLogado"] = row["Usuario"].ToString();
                Session["PerfilId"] = Convert.ToInt32(row["PerfilId"]);
                Session["NivelAcesso"] = row["NivelAcesso"].ToString();

                // Manda o usuário pra página principal do sistema.
                Response.Redirect("Home.aspx");
            }
            catch (Exception)
            {
                // Se der qualquer outro erro no meio do caminho, mostra uma mensagem genérica.
                lblMensagem.Text = "Erro ao tentar fazer login.";
                lblMensagem.CssClass = "text-danger text-center";
            }
        }
    }
}