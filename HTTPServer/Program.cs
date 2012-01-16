using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener server = null;
            try
            {
                Int32 port = 5000; //O cualquier otro siempre que no interfiera con los ya existentes
                IPAddress localAddr = IPAddress.Parse("127.0.0.1"); //Nos mantenemos a la escucha en "localhost"
                
                server = new TcpListener(localAddr, port);

                server.Start(); //Nos mantenemos a la espera de nuevas peticiones
                Byte[] bytes = new Byte[1000]; //Array donde guardaremos el resultado
                String data = null; //Cadena de caracteres que contendrá los datos una vez procesados

                // Enter the listening loop.
                while (true)
                {
                    Console.Write("Waiting for a connection... ");

                    TcpClient client = server.AcceptTcpClient(); //Aceptamos la conexión entrante
                    Console.WriteLine("Connected!");

                    data = null;

                    NetworkStream stream = client.GetStream(); //Obtenemos el stream para lectura y escritura
                    int i = stream.Read(bytes, 0, bytes.Length); //Leemos en el array "bytes" y almacenamos en i el numero de bytes leidos.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i); //Convertimos la cadena
                    Console.WriteLine("Received: {0}", data); //Mostramos por pantalla el resultado.

                    //Dividimos el mensaje en un array de strings
                    var lista = data.Split(' ');
                    //Por si acaso la petición viene vacía.
                    if (lista.Length < 3) continue;
                    //El primer elemento de la lista será la instrucción
                    var instruccion = lista[0];
                    //El segundo elemento de la lista será la ruta
                    var ruta = lista[1];
                    //El tercer elemento antes del salto de carro, será el protocolo
                    string protocolo = lista[2].Split('\n')[0];
                    //Finalmente mostramos los datos por pantalla
                    Console.WriteLine("Instruccion: {0}\nRuta: {1}\nProtocolo: {2}", instruccion, ruta, protocolo);
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
                        //Codificamos el texto que hemos cargado en un array de bytes
                        msg = System.Text.Encoding.ASCII.GetBytes(response);
                        //Escribimos en el stream el mensaje codiificado
                        stream.Write(msg, 0, msg.Length);
                    }
                    else {
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
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            catch(Exception e){
                Console.WriteLine("Exception: {0}", e);
            }
            finally
            {
                // Stop listening for new clients.
                server.Stop();
            }


            Console.WriteLine("\nHit enter to continue...");
            Console.Read();
        }
    }
}

