using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace HTTPServer
{
    class WebServer
    {
        public WebServer(string address, int port)
        {
            this.address = address;
            this.port = port;
            listener = new TcpListener(IPAddress.Parse(address), port);
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine(string.Format("Local server started at: {0}:{1}", address, port));
        }

        public void Listen()
        {
            try
            {
                Byte[] result = new Byte[1000];
                String data;

                TcpClient client = listener.AcceptTcpClient();

                NetworkStream stream = client.GetStream(); //Obtenemos el stream para lectura y escritura
                int i = stream.Read(result, 0, result.Length); //Leemos en el array "result" y almacenamos en i el numero de result leidos.
                data = System.Text.Encoding.ASCII.GetString(result, 0, i); //Convertimos la cadena
                Console.WriteLine("Received: {0}", data); //Mostramos por pantalla el resultado.

                //Dividimos el mensaje en un array de strings
                var lista = data.Split(' ');
                //Por si acaso la petición viene vacía.
                if (lista.Length < 3) return;
                //El primer elemento de la lista será la instrucción
                var instruccion = lista[0];
                //El segundo elemento de la lista será la ruta
                var ruta = lista[1];
                //El tercer elemento antes del salto de carro, será el protocolo
                string protocolo = lista[2].Split('\n')[0];
                //Finalmente mostramos los datos por pantalla
                Console.WriteLine("Instruction: {0}\nPath: {1}\nProtocol: {2}", instruccion, ruta, protocolo);
                byte[] msg;

                //Comprobamos que estemos recibiendo la peticion de la home
                if (ruta.Equals("/"))
                {
                    //Leemos todo el contenido del fichero especificado
                    var fichero = File.ReadAllText("home.html");
                    //Redactamos la cabecera de respuesta.
                    string response = "HTTP/1.1 200 OK\r\n\r\n\r\n";
                    //Agregamos a la cabecera la informacion del fichero.
                    response = response + fichero;
                    //Mostramos por pantalla el resultado
                    Console.WriteLine("Sent: {0}", response);
                    //Codificamos el texto que hemos cargado en un array de result
                    msg = System.Text.Encoding.ASCII.GetBytes(response);
                    //Escribimos en el stream el mensaje codiificado
                    stream.Write(msg, 0, msg.Length);
                }
                else
                {
                    //Redactamos una cabecera de fichero no encontrado
                    string response = "HTTP/1.1 404 Not Found";
                    //Mostramos por pantalla el resultado
                    Console.WriteLine("Sent: {0}", response);
                    //Codificamos, exactamente igual que en la parte superior
                    msg = System.Text.Encoding.ASCII.GetBytes(response);
                    //Escribimos en el stream el mensaje codificado
                    stream.Write(msg, 0, msg.Length);
                }

                // Send back a response.
                // Shutdown and end connection
                client.Close();

            }
            finally
            {
                listener.Stop();
            }

        }

        private TcpListener listener;
        private string address;
        private int port;
    }
}
