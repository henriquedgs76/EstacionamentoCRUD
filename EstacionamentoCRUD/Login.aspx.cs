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
/*
 
 1. Para o Repositório (`GetMensalistasRepositorio.cs`)

  Adicione o método AtualizaUltimaConexao à classe GetMensalistasRepositorio no arquivo AuditoriaCarMob.Infra/GetMensalistasRepositorio.cs. Este método contém o comando SQL que será
  executado no banco de dados.

  Código para o arquivo:

     public void AtualizaUltimaConexao(string serialno)
     {
         DbContext db = new DbContext();
         using (Microsoft.Data.SqlClient.SqlConnection con = new Microsoft.Data.SqlClient.SqlConnection(db.GetConnection()))
         {
             con.Open();
             var data = FuncoesBD.GetInMyTimeZone();
             var sql = $@"UPDATE Aud_MonitoramentoCamera
                        SET aip_ultima_conexao = '{data.ToString("yyyy-MM-dd HH:mm:ss")}'
                       WHERE serialno = '{serialno}'";
   
            // Para depuração, você pode querer logar o SQL:
            // System.Diagnostics.Debug.WriteLine(sql);
   
            con.Execute(sql);
            con.Close();
            con.Dispose();
        }
    }

  Comando SQL gerado (para a equipe de DBA):
  O código acima irá gerar e executar um comando SQL parecido com este. Você pode enviar este exemplo para a equipe de banco de dados para que eles saibam exatamente o que será
  executado.

    UPDATE Aud_MonitoramentoCamera
    SET aip_ultima_conexao = '2025-12-15 10:30:00' -- A data e hora serão dinâmicas
    WHERE serialno = 'NUMERO_DE_SERIE_DA_CAMERA'   -- O serial number será dinâmico

  ---

  2. Para o Serviço (`GetMensalistasServices.cs`)

  Adicione o método abaixo na classe GetMensalistasServices no arquivo AuditoriaCarMob.Servicos/GetMensalistasServices.cs. Ele simplesmente chama o método do repositório.

  Código para o arquivo:

    public void AtualizaUltimaConexao(string serialno)
    {
        _repo.AtualizaUltimaConexao(serialno);
    }

  ---

  3. Para o Controller (`MensalistasController.cs`)

  No arquivo AuditoriaCarMob.API/Controllers/MensalistasController.cs, localize o trecho de código que processa o "Heartbeat" (por volta da linha 50) e adicione a chamada para o novo
  serviço.

  Encontre este bloco:

    if (heartbeat?.HeartBeat?.serialno != null)
    {
        log.InsereLog("Heartbeat", "MensalistasController", "Recebido HeartBeatInteractive", heartbeat.HeartBeat.serialno, null);
        serialNumber = heartbeat.HeartBeat.serialno;
    }

  E modifique para ficar assim:

     if (heartbeat?.HeartBeat?.serialno != null)
     {
         // Instancia o serviço para atualizar a conexão
         GetMensalistasServices serviceHeartbeat = new GetMensalistasServices();
         serviceHeartbeat.AtualizaUltimaConexao(heartbeat.HeartBeat.serialno);
    
         // Mantém o log original
         log.InsereLog("Heartbeat", "MensalistasController", "Recebido HeartBeatInteractive", heartbeat.HeartBeat.serialno, null);
         serialNumber = heartbeat.HeartBeat.serialno;
    }

  Com essas alterações, o sistema estará pronto para atualizar a tabela Aud_MonitoramentoCamera sempre que um heartbeat for recebido.

 
 
 
 */