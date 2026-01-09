using EstacionamentoCRUD.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace EstacionamentoCRUD
{
    public partial class CadastrarUsuario : System.Web.UI.Page
    {
        protected void btnCadastrar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string senha = txtSenha.Text;
            string confirmarSenha = txtConfirmarSenha.Text;
            int perfilId = int.Parse(ddlNivelAcesso.SelectedValue);
            string nivelAcesso = ddlNivelAcesso.SelectedItem.Text;


            // Validações Iniciais
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(senha))
            {
                lblMensagem.Text = "Usuário e senha são obrigatórios.";
                lblMensagem.CssClass = "text-warning text-center";
                return;
            }

            if (senha != confirmarSenha)
            {
                lblMensagem.Text = "As senhas não conferem.";
                lblMensagem.CssClass = "text-danger text-center";
                return;
            }

            try
            {
                // verifica se o nome de usuário já existe
                string sqlVerificaUsuario = "SELECT COUNT(*) FROM Usuarios WHERE Usuario = @Usuario";
                var parametersVerificaUsuario = new[] { new SqlParameter("@Usuario", usuario) };
                if ((int)DataAccess.ExecuteScalar(sqlVerificaUsuario, parametersVerificaUsuario) > 0)
                {
                    lblMensagem.Text = "Este nome de usuário já está em uso.";
                    lblMensagem.CssClass = "text-danger text-center";
                    return;
                }

                // Cria o Hash e o Salt da senha
                (byte[] hash, byte[] salt) = PasswordHasher.HashPassword(senha);

                // cria o novo usuário no banco de dados com o nivel de acesso
                string sqlInserir = @"
                       INSERT INTO Usuarios
                        (Usuario, HashDaSenha, SalDaSenha, PerfilId, NivelAcesso)
                        VALUES
                        (@Usuario, @HashDaSenha, @SalDaSenha, @PerfilId, @NivelAcesso)";
                var parametersInserir = new[]
                {
                    new SqlParameter("@Usuario", usuario),
                    new SqlParameter("@HashDaSenha", hash),
                    new SqlParameter("@SalDaSenha", salt),
                    new SqlParameter("@PerfilId", perfilId),
                    new SqlParameter("@NivelAcesso", nivelAcesso)
                };


                DataAccess.ExecuteNonQuery(sqlInserir, parametersInserir);

                lblMensagem.Text = "Usuário cadastrado com sucesso!";
                lblMensagem.CssClass = "text-success text-center";

                // volta para a página de login após 2 segundos
                Response.AddHeader("REFRESH", "2;URL=Login.aspx");
            }
            catch (Exception)
            {
                lblMensagem.Text = "Ocorreu um erro inesperado ao cadastrar.";
                lblMensagem.CssClass = "text-danger text-center";
            }
        }
    }
}