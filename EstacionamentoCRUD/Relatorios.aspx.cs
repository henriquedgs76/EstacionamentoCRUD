using EstacionamentoCRUD.DAL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Data;
using System.Data.SqlClient;
//using System.Drawing;
using System.Globalization;
using System.IO;
using System.Web;

namespace EstacionamentoCRUD
{
    public partial class Relatorios : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (Session["UsuarioLogado"] == null || Session["PerfilId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            int perfilId = (int)Session["PerfilId"];

            // aqui é o adm 1, e supervisor 3.
            if (perfilId != 1 && perfilId != 3)
            {
                Response.Redirect("~/Home.aspx");
                return;
            }


            if (!IsPostBack)
            {
                // os últimos 5 anos até o ano atual
                int anoAtual = DateTime.Now.Year;
                for (int i = anoAtual; i >= anoAtual - 5; i--)
                {
                    ddlAno.Items.Add(i.ToString());
                }

                // seleciona o mês e ano atuais
                ddlMes.SelectedValue = DateTime.Now.Month.ToString();
                ddlAno.SelectedValue = anoAtual.ToString();

                // mensagem de teste para depuração
                lblMensagem.Text = "Versão 2 do código carregada.";
                lblMensagem.CssClass = "text-success";
            }
        }

        protected void btnGerarDiario_Click(object sender, EventArgs e)
        {
            DateTime dataRelatorio;
            if (!DateTime.TryParse(txtDataDiario.Text, out dataRelatorio))
            {
                divResultados.Visible = true;
                btnExportarPdf.Visible = true;
                lblTotalFaturamento.Text = "R$ 0,00";
                gvRelatorio.DataSource = null;
                gvRelatorio.DataBind();
                lblMensagem.Text = " Por favor, selecione uma data válida.";
                lblMensagem.CssClass = "text-danger";
                return;
            }

            //relatório diário 
            string sqlDetalhes = @"
    SELECT V.Placa, V.Modelo, S.NomeServico, V.DataSaida, V.ValorPago 
    FROM Veiculos V
    INNER JOIN Servicos S ON V.ServicoId = S.Id
    WHERE CAST(V.DataSaida AS DATE) = @Data 
    AND V.Status = 'Finalizado'";
            var parametros = new[] { 
                new SqlParameter("@Data", dataRelatorio.Date) 
            };
            string sqlTotal = "SELECT SUM(ValorPago) " +
                "FROM Veiculos " +
                "WHERE CAST(DataSaida AS DATE) = @Data " +
                "AND Status = 'Finalizado'";

            
            Session["RelatorioTitulo"] = "Relatório de Faturamento Diário - " 
                + dataRelatorio.ToString("dd/MM/yyyy");

            //salvando relatorio
            GerarRelatorio(sqlDetalhes, sqlTotal, parametros);
        }

        protected void btnGerarMensal_Click(object sender, EventArgs e)
        {
            int mes = Convert.ToInt32(ddlMes.SelectedValue);
            int ano = Convert.ToInt32(ddlAno.SelectedValue);

            //relatório mensal
            string sqlDetalhes = @"
        SELECT 
            V.Placa, 
            V.Modelo, 
            S.NomeServico, 
            V.DataSaida, 
            V.ValorPago 
        FROM Veiculos V
        INNER JOIN Servicos S ON V.ServicoId = S.Id
        WHERE MONTH(V.DataSaida) = @Mes 
        AND YEAR(V.DataSaida) = @Ano 
        AND V.Status = 'Finalizado'
        ORDER BY V.DataSaida ASC";

            var parametros = new[]
            {
        new SqlParameter("@Mes", mes),
        new SqlParameter("@Ano", ano)
    };

            string sqlTotal = @"
        SELECT SUM(ValorPago) 
        FROM Veiculos 
        WHERE MONTH(DataSaida) = @Mes 
        AND YEAR(DataSaida) = @Ano 
        AND Status = 'Finalizado'";

            // Salva o título na sessão
            Session["RelatorioTitulo"] = $"Relatório de Faturamento Mensal - {ddlMes.SelectedItem.Text} de {ano}";

            GerarRelatorio(sqlDetalhes, sqlTotal, parametros);
        }

        /* private void GerarRelatorio(string sqlDetalhes, string sqlTotal, SqlParameter[] parametros)
         {
             try
             {
                 object totalObj = DataAccess.ExecuteScalar(sqlTotal, parametros);
                 decimal totalFaturamento = (totalObj == DBNull.Value || totalObj == null) ? 0 : Convert.ToDecimal(totalObj);
                 DataTable dt = DataAccess.ExecuteDataTable(sqlDetalhes, parametros);

                 // Salva os dados na Sessão para serem usados pelo exportador de PDF
                 Session["RelatorioDados"] = dt;
                 Session["RelatorioTotal"] = totalFaturamento;

                 divResultados.Visible = true;
                 lblTotalFaturamento.Text = totalFaturamento.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));
                 gvRelatorio.DataSource = dt;
                 gvRelatorio.DataBind();

                 if (dt.Rows.Count > 0)
                 {
                     btnExportarPdf.Visible = true; // Mostra o botão de exportar
                     lblMensagem.Text = $"{dt.Rows.Count} registro(s) encontrado(s).";
                     lblMensagem.CssClass = "text-info";
                 }
                 else
                 {
                     btnExportarPdf.Visible = false; // Esconde se não há dados
                     lblMensagem.Text = "Nenhum registro encontrado para o período selecionado.";
                     lblMensagem.CssClass = "text-info";
                 }
             }
             catch (Exception erro)
             {
                 divResultados.Visible = true;
                 btnExportarPdf.Visible = true;
                 lblTotalFaturamento.Text = "Erro";
                 gvRelatorio.DataSource = null;
                 gvRelatorio.DataBind();
                 lblMensagem.Text = " Ocorreu um erro: " + erro.Message;
                 lblMensagem.CssClass = "text-danger";
             }
         } */

        private void GerarRelatorio(string sqlDetalhes, string sqlTotal,
            SqlParameter[] parametrosOriginais)
        {
            try
            {
                //faz  os parâmetros para cada comando
                SqlParameter[] parametrosTotal = CloneParametros(parametrosOriginais);
                SqlParameter[] parametrosDetalhes = CloneParametros(parametrosOriginais);

                object totalObj = DataAccess.ExecuteScalar(sqlTotal, parametrosTotal);
                decimal totalFaturamento = (totalObj == DBNull.Value || totalObj == null)
                    ? 0
                    : Convert.ToDecimal(totalObj);

                DataTable dt = DataAccess.ExecuteDataTable(sqlDetalhes,
                    parametrosDetalhes);

                Session["RelatorioDados"] = dt;
                Session["RelatorioTotal"] = totalFaturamento;

                divResultados.Visible = true;
                lblTotalFaturamento.Text = totalFaturamento.ToString("C",
                    CultureInfo.GetCultureInfo("pt-BR"));

                gvRelatorio.DataSource = dt;
                gvRelatorio.DataBind();

                btnExportarPdf.Visible = dt.Rows.Count > 0;
                lblMensagem.Text = dt.Rows.Count > 0
                    ? $"{dt.Rows.Count} registro(s) encontrado(s)."
                    : "Nenhum registro encontrado para o período selecionado.";

                lblMensagem.CssClass = "text-info";
            }
            catch (Exception erro)
            {
                divResultados.Visible = true;
                btnExportarPdf.Visible = true;
                lblTotalFaturamento.Text = "Erro";
                gvRelatorio.DataSource = null;
                gvRelatorio.DataBind();

                lblMensagem.Text = "Ocorreu um erro: " + erro.Message;
                lblMensagem.CssClass = "text-danger";
            }

        }
        private SqlParameter[] CloneParametros(SqlParameter[] parametros)
        {
            SqlParameter[] clone = new SqlParameter[parametros.Length];

            for (int i = 0; i < parametros.Length; i++)
            {
                clone[i] = new SqlParameter(
                    parametros[i].ParameterName,
                    parametros[i].Value
                );
            }

            return clone;
        }



        protected void btnExportarPdf_Click(object sender, EventArgs e)
        {
            if (Session["RelatorioDados"] == null)
            {
                lblMensagem.Text = "Não há dados de relatório para exportar. " +
                    "Por favor, escolha um dia e gere um relatório primeiro.";
                lblMensagem.CssClass = "text-danger";
                return;
            }

            DataTable dt = (DataTable)Session["RelatorioDados"];
            string titulo = Session["RelatorioTitulo"].ToString();
            decimal total = (decimal)Session["RelatorioTotal"];


            Document pdfDoc = new Document(PageSize.A4, 40f, 40f, 60f, 60f);
            MemoryStream ms = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);

            pdfDoc.Open();


            var fonteTitulo = FontFactory.GetFont("Arial", 18, Font.BOLD);
            var pTitulo = new Paragraph(new Chunk(titulo, fonteTitulo));
            pTitulo.Alignment = Element.ALIGN_CENTER;
            pdfDoc.Add(pTitulo);
            pdfDoc.Add(Chunk.NEWLINE);

            // criando a tabela com os dados no pdf
            // btnExportarPdf_Click e adicione as larguras:
            var tabela = new PdfPTable(dt.Columns.Count);
            tabela.WidthPercentage = 100;

            //definir larguras (Placa, Modelo, Serviço, Data, Valor)
            float[] larguraColunas = { 15f, 20f, 30f, 20f, 15f };
            tabela.SetWidths(larguraColunas);

            // fonte do cabeçalho
            var fonteCabecalho = FontFactory.GetFont("Arial", 10, Font.BOLD);
            foreach (DataColumn coluna in dt.Columns)
            {
                var celula = new PdfPCell(new Phrase(coluna.ColumnName, fonteCabecalho));
                celula.BackgroundColor = BaseColor.LIGHT_GRAY;
                celula.HorizontalAlignment = Element.ALIGN_CENTER;
                tabela.AddCell(celula);
            }

            // aqui é as linhas da tabela do pdf
            foreach (DataRow linha in dt.Rows)
            {
                foreach (object item in linha.ItemArray)
                {
                    string valorFormatado;
                    if (item is DateTime)
                        valorFormatado = ((DateTime)item).ToString("dd/MM/yyyy HH:mm");
                    else if (item is decimal)
                        valorFormatado = ((decimal)item).ToString("C",
                            CultureInfo.GetCultureInfo("pt-BR"));
                    else
                        valorFormatado = item.ToString();

                    tabela.AddCell(new Phrase(valorFormatado,
                        FontFactory.GetFont("Arial", 9)));
                }
            }
            pdfDoc.Add(tabela);

            
            //total 
            var fonteTotal = FontFactory.GetFont("Arial", 14, Font.BOLD);
            var pTotal = new Paragraph(new Chunk($"\nFaturamento Total: " +
                $"{total:C}", fonteTotal));
            pTotal.Alignment = Element.ALIGN_RIGHT;
            pdfDoc.Add(pTotal);

            pdfDoc.Close(); //aqui estou encerrando o pdf
            

            // aqui envia pro usuario
            pdfDoc.Close();

            byte[] pdfBytes = ms.ToArray();
            ms.Close();

            Response.Clear();
            Response.ContentType = "application/pdf";
            Response.AddHeader("Content-Disposition",
                "attachment; filename=RelatorioFaturamento.pdf");
            Response.OutputStream.Write(pdfBytes, 0, pdfBytes.Length);
            Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
    }
}