using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InserirImagemBanco
{
    public partial class Form1 : Form
    {
        Bitmap bmp;//bitmap em classe global, ele é uma classe que trabalha com os pixels de uma imagem.

        const string StringConexao = @"Data Source=DESKTOP-MCQJPQB;Initial Catalog=master;Integrated Security=True";//a minha tabela foi feita na pasta master.

        SqlConnection conexao = new SqlConnection(StringConexao);

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlCommand comando = new SqlCommand("select * from tbimagem where numero = @numero", conexao);
            
            SqlParameter numero = new SqlParameter("@numero", SqlDbType.Int);
            
            numero.Value = textBox2.Text;

            comando.Parameters.Add(numero);

            try
            {
                conexao.Open();

                SqlDataReader reader = comando.ExecuteReader();//aqui foi criado um objeto o datareader para ler as linhas da tabela e buscar os dados que ele acabou de ler.

                reader.Read();

                if (reader.HasRows)//quando o reader fizer a leitura se tiver alguma linha ou seja tiver algum conteudo dentro dele, pegamos e jogamos no formulario.
                {
                    textBox1.Text = reader[1].ToString();//esta parte vai pegar o que tem na tabela e mostrar na tela(formulario) o [1] representa a linha da tabela onde esta o nome como na tabela tem 3 linha e a primeira é a de codigo que corresponde ao [0] que nao vou usar aqui entao colocamos o [1] que é onde esta a linha nome.
                    byte[] imagem = (byte[])(reader[2]);//aqui vamos no campo [2] da tabela do banco onde esta a imagem.

                    if (imagem == null)//esta condição é para se o array onde esta a imagem for igual a nulo ele nao vai mostrar nada da picturebox nada.
                        pictureBox1.Image = null;
                    else//caso contrario vamos fazer mais um objeto memorystream que contem a imagem, ele é responsavel por guardar a imagem na memoria.
                    {
                        MemoryStream memory = new MemoryStream(imagem);//aqui no objeto colocamos (imagem) que é a array do tipo byte que contem os dados da imagem que estamos buscando.
                        pictureBox1.Image = Image.FromStream(memory);//aqui vamos mandar a imagem reculperada para a picturebox
                    }
                }

                //conexao.Close(); no video o cara apagou o finally onde fecha a conexao mas eu resolvi manter no finally se der errado vou apagar o finally e deixo essa parte para ver se o erro é aqui.

            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
            finally
            {
                conexao.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Todos os codigos neste butao é para pegar buscar a imagem e coloca-la no picturebox para exibir para o usuario.

            //openFileDialog serve para abrir a pasta de arquivos do pc.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)//Quando o usuario selecionar o seu arquivo e clicar em OK vem para ca.
            {
                string nome = openFileDialog1.FileName;//para pegar o nome do arquivo escolhido.

                bmp = new Bitmap(nome);

                pictureBox1.Image = bmp;//aqui vamos pegar o nome da imagem e colocalo no picturebox para aparecer a imagem no picturebox sem necessidade de colocar a imagem antes.
            }
        
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Agora vamos salvar o arquivo selecionado.

            MemoryStream memory = new MemoryStream();//O memorystream é um objeto que serve para guardar dados na memoria.

            bmp.Save(memory, ImageFormat.Bmp);

            byte[] foto = memory.ToArray();

            SqlCommand comando = new SqlCommand("INSERT INTO tbimagem (nome, imagem, numero) VALUES(@nome, @imagem, @numero)", conexao); //O que esta dentro dos parenteses é a sintaxe para salvar as imagens no banco.

            SqlParameter nome = new SqlParameter("@nome", SqlDbType.VarChar);
            SqlParameter imagem = new SqlParameter("imagem", SqlDbType.Binary);
            SqlParameter numero = new SqlParameter("numero", SqlDbType.Int);
            nome.Value = textBox1.Text;
            imagem.Value = foto;
            numero.Value = textBox2.Text;

            comando.Parameters.Add(nome);
            comando.Parameters.Add(imagem);
            comando.Parameters.Add(numero);

            try
            {
                conexao.Open();

                comando.ExecuteNonQuery();

                MessageBox.Show("Imagem SALVA no banco de dados com SUCESSO!");

                textBox1.Text = "";
                pictureBox1.Image = null;

            }
            catch (Exception E)
            {

                MessageBox.Show(E.Message);
            }
            finally
            {
                conexao.Close();
            }

        }
    }
}
