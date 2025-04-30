using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RDPCOMAPILib;
using AxRDPCOMAPILib;
using System.Data.SqlClient;
using System.Data.Odbc;
using System.Runtime.InteropServices;

namespace Simple_RDP_Client
{
    public partial class Form1 : Form
    {
        SqlConnection conexao = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["programas1"].ConnectionString.ToString());


        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        private const byte VK_LWIN = 0x5B; // Tecla Windows esquerda
        private const byte VK_R = 0x52;    // Tecla R
        private const uint KEYEVENTF_KEYUP = 0x0002;


        public Form1()
        {
            InitializeComponent();
            carregaUSUARIO(conexao);
            resolucao();
            this.KeyPreview = true;
        }

        public static void Connect(string invitation, AxRDPViewer display, string userName, string password)
        {
            display.Connect(invitation, userName, password);
        }

        public static void disconnect(AxRDPViewer display)
        {
            display.Disconnect();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Connect(textConnectionString.Text, this.axRDPViewer, "", "");
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to connect to the Server");
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            textConnectionString.Text = (dataGridView1.CurrentRow.Cells[0].Value).ToString();
            lbNome.Text = (dataGridView1.CurrentRow.Cells[1].Value).ToString();
            lbId.Text = (dataGridView1.CurrentRow.Cells[6].Value).ToString();


            tabControl1.SelectedIndex = 0;
        }

        public void carregaUSUARIO(SqlConnection conexao)
        {
            if (conexao.State == ConnectionState.Closed) //Validar a conexão
            {
                conexao.Open();
            }

            //este metodo esta carregando os nome dos modulos
            try
            {

                SqlDataAdapter ADAP = new SqlDataAdapter("SELECT string,maquina,dtcad,processador,memoria,windows,idM FROM conexaoRemota where delet =''", conexao);
                DataSet DS = new DataSet();

                
                ADAP.Fill(DS, "fornecedor");

                
                dataGridView1.DataSource = DS.Tables["fornecedor"];

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            conexao.Close();

        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            carregaUSUARIO(conexao);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            disconnect(this.axRDPViewer);
        }

        private void btnFinalizar_Click(object sender, EventArgs e)
        {
            if (lbId.Text == "id")
            {
                // Não faz nada se for o valor padrão
                return;
            }

            var result = MessageBox.Show(
                $"Deseja realmente finalizar a conexão com '{lbNome.Text}'? \n Caso sim para acessar novamente esta maquina o usuário tera que abrir novamente o programa ou clicar em Atualizar.",
                "Confirmação",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                finalizarConexao(conexao, lbId.Text);
                lbNome.Text = "";
            }
        }

        public void finalizarConexao(SqlConnection conexao, string id)
        {
            if (conexao.State == ConnectionState.Closed) //Validar a conexão
            {
                conexao.Open();
            }

            try
            {
                string sql = "update conexaoRemota set delet='*' where idM='" + id + "'";

                SqlCommand cmd1 = new SqlCommand(sql, conexao);

                cmd1.ExecuteNonQuery();
                carregaUSUARIO(conexao);
            }
            catch { conexao.Close(); }

            conexao.Close();
        }

        private void btnAplicarResolucao_Click(object sender, EventArgs e)
        {
            try
            {
                AjustarResolucaoViewer();
                Connect(textConnectionString.Text, this.axRDPViewer, "", "");
            }
            catch (Exception)
            {
                MessageBox.Show("Unable to connect to the Server");
            }
        }

        public void resolucao()
        {
            comboBoxResolucao.Items.Add("800x600");
            comboBoxResolucao.Items.Add("1024x768");
            comboBoxResolucao.Items.Add("1280x720");
            comboBoxResolucao.Items.Add("1366x768");
            comboBoxResolucao.Items.Add("1920x1080");
            comboBoxResolucao.SelectedIndex = 1; // Padrão
        }

        private void AjustarResolucaoViewer()
        {
            string resolucao = comboBoxResolucao.SelectedItem.ToString();
            var partes = resolucao.Split('x');
            int largura = int.Parse(partes[0]);
            int altura = int.Parse(partes[1]);

            axRDPViewer.Width = largura;
            axRDPViewer.Height = altura;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.R)
            {
                EnviarWinR(); // Isso simula Win + R
            }
        }

        private void EnviarWinR()
        {
            // Pressionar Win
            keybd_event(VK_LWIN, 0, 0, UIntPtr.Zero);
            // Pressionar R
            keybd_event(VK_R, 0, 0, UIntPtr.Zero);
            // Soltar R
            keybd_event(VK_R, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
            // Soltar Win
            keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            EnviarWinR(); // Isso simula Win + R
        }
    }
}
