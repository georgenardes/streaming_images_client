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
        // V1 
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
            int qtdBytesLidos = 0;
            byte[] receivedData = new byte[dataArraySize];

            // Receive data using the BufferedStream.
            Console.WriteLine("Receiving data using BufferedStream.");

            int numBytesToRead = receivedData.Length;
            while (numBytesToRead > 0)
            {
                // Read may return anything from 0 to numBytesToRead.
                int n = bufStream.Read(receivedData, 0, receivedData.Length);

                // The end of the file is reached.
                if (n == 0)
                    break;

                qtdBytesLidos += n;
                numBytesToRead -= n;

            }

            /* escreve os bytes recebidos do stream
            * apenas para verificar o conteúdo.  
            */
            SaveFileDialog salvarArquivo = new SaveFileDialog();
            salvarArquivo.FileName = "new_image_bufstream";
            salvarArquivo.DefaultExt = "jpg";
            if (salvarArquivo.ShowDialog() == DialogResult.OK && salvarArquivo.FileName.Length > 0)
            {
                File.WriteAllBytes(salvarArquivo.FileName, receivedData);
                MessageBox.Show("Arquivo 2 criado!");
            }

            /* mostra a imagem */
            Bitmap bitmap = new Bitmap(new MemoryStream(receivedData));
            pbObject.Image = bitmap;
        }

    }
}
