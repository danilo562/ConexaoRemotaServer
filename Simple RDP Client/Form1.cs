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

namespace Simple_RDP_Client
{
    public partial class Form1 : Form
    {
        SqlConnection conexao = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["programas1"].ConnectionString.ToString());
    
        public Form1()
        {
            InitializeComponent();
            carregaUSUARIO(conexao);
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

            }
            else {
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



    }
}
