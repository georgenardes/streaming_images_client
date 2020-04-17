using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace streaming_images_client
{
    public partial class Form1 : Form
    {
        const int dataArraySize = 50000;
        const int streamBufferSize = 50000;        

        static PictureBox pbObject;

        public Form1()
        {
            InitializeComponent();
            pbObject = pictureBox1;
        }

        private void btnCarregar_Click(object sender, EventArgs e)
        {
            /* https://docs.microsoft.com/pt-br/dotnet/api/system.io.bufferedstream?view=netframework-4.8#moniker-applies-to */
            /* CLIENT SIDE */

            // Create the underlying socket and connect to the server.
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(new IPEndPoint(IPAddress.Parse("192.168.25.69"), 1800));
            MessageBox.Show("Client is connected.\n");


            // Create a NetworkStream that owns clientSocket and
            // then create a BufferedStream on top of the NetworkStream.
            // Both streams are disposed when execution exits the
            // using statement.
            using (Stream netStream = new NetworkStream(clientSocket, true),
                    bufStream = new BufferedStream(netStream, streamBufferSize))
            {
                if (bufStream.CanRead)
                {
                    ReceiveData(netStream, bufStream);
                }

                MessageBox.Show("\nShutting down the connection.");
                bufStream.Close();
            }
        }

        static void ReceiveData(Stream netStream, Stream bufStream)
        {
            /* cria a imagem */
            Bitmap bitmap = new Bitmap(bufStream);


            /* escreve os bytes recebidos do stream
            * apenas para verificar o conteúdo.  
            */
            SaveFileDialog salvarArquivo = new SaveFileDialog();
            salvarArquivo.FileName = "new_image_bufstream";
            salvarArquivo.DefaultExt = "jpg";
            if (salvarArquivo.ShowDialog() == DialogResult.OK && salvarArquivo.FileName.Length > 0)
            {
                bitmap.Save(salvarArquivo.FileName);
                MessageBox.Show("Bitmap salvo!");
            }
            
            /* cria a imagem */
            pbObject.Image = bitmap;
        }

    }
}
