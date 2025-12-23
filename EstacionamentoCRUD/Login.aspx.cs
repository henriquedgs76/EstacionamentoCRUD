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

            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(senha))
            {
                lblMensagem.Text = " Preencha todos os campos.";
                lblMensagem.CssClass = "text-warning text-center";
                return;
            }

            byte[] hashSalvo = null;
            byte[] salSalvo = null;

            try
            {
                using (SqlConnection conexao = new SqlConnection(connectionString))
                {
                    conexao.Open();
                    // 1. Nova consulta: pega o hash e o sal do usuário informado, com os novos nomes de colunas
                    string sql = "SELECT HashDaSenha, SalDaSenha " +
                        "FROM Usuarios " +
                        "WHERE Usuario = @Usuario";
                    using (SqlCommand comando = new SqlCommand(sql, conexao))
                    {
                        comando.Parameters.AddWithValue("@Usuario", usuario);

                        using (SqlDataReader leitor = comando.ExecuteReader())
                        {
                            if (leitor.Read()) // Se encontrou o usuário
                            {
                                // 2. Pega os valores do banco, verificando se são nulos
                                if (leitor["HashDaSenha"] != DBNull.Value)
                                    hashSalvo = (byte[])leitor["HashDaSenha"];

                                if (leitor["SalDaSenha"] != DBNull.Value)
                                    salSalvo = (byte[])leitor["SalDaSenha"];
                            }
                        }
                    }
                }

                // 3. Se não encontrou o usuário ou se o hash/sal forem nulos, o login falha.
                if (hashSalvo == null || salSalvo == null)
                {
                    lblMensagem.Text = " Usuário ou senha inválidos.";
                    lblMensagem.CssClass = "text-danger text-center";
                    return;
                }

                // 4. Verifica a senha usando nossa classe de segurança
                bool senhaEstaCorreta = DAL.PasswordHasher.VerifyPassword(senha, hashSalvo, salSalvo);

                if (senhaEstaCorreta)
                {
                    // Sucesso!
                    // Aqui você poderia adicionar o usuário a uma sessão, por exemplo:
                    // Session["UsuarioLogado"] = usuario;
                    Response.Redirect("Home.aspx");
                }
                else
                {
                    lblMensagem.Text = " Usuário ou senha inválidos.";
                    lblMensagem.CssClass = "text-danger text-center";
                }
            }
            catch (Exception erro)
            {
                // Em um ambiente de produção, seria bom registrar o erro `erro.Message`
                lblMensagem.Text = " Ocorreu um erro ao tentar fazer o login.";
                lblMensagem.CssClass = "text-danger text-center";
            }
        }
    }
}