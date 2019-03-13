using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace tcp
{
	class file_server
	{
		/// <summary>
		/// The PORT
		/// </summary>
		const int PORT = 9000;
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		const int BUFSIZE = 1000;

        // Set IP address
        IPAddress localAddr = IPAddress.Parse("10.0.0.1");


        /// <summary>
        /// Initializes a new instance of the <see cref="file_server"/> class.
        /// Opretter en socket.
        /// Venter på en connect fra en klient.
        /// Modtager filnavn
        /// Finder filstørrelsen
        /// Kalder metoden sendFile
        /// Lukker socketen og programmet
        /// </summary>
        private file_server ()
		{

            // TO DO Your own code
            TcpListener serverSocket = new TcpListener(localAddr, PORT);
            serverSocket.Start();
         
   
            while (true)
            {
                try
                {
                    Console.WriteLine("Waiting for a connection...");

                    // Accept requests
                    TcpClient client = serverSocket.AcceptTcpClient();
                    Console.WriteLine("Connected to client!");
                    NetworkStream stream = client.GetStream();


					string fileName = LIB.readTextTCP(stream);
					Console.WriteLine($"Client asks for file: {fileName}");
					Console.WriteLine("Cheking if file exists...");

					long fileSize = LIB.check_File_Exists(fileName);

                    if (fileSize > 0)
					{
						Console.WriteLine($"File exists an has size: {fileSize}");
						LIB.writeTextTCP(stream, fileSize.ToString());
						sendFile(fileName, fileSize, stream);

					}
					else
					{
						Console.WriteLine("File does not exist");
						LIB.writeTextTCP(stream, fileSize.ToString());
					}

					client.Close();               
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                             
                }

                
                                
            }
         
		}
       
                    
		/// <summary>
		/// Sends the file.                                             
		/// </summary>
		/// <param name='fileName'>
		/// The filename.
		/// </param>
		/// <param name='fileSize'>
		/// The filesize.
		/// </param>
		/// <param name='io'>
		/// Network stream for writing to the client.
		/// </param>
		private void sendFile (String fileName, long fileSize, NetworkStream io)
		{
			int tempFileSize = (int)fileSize;
			Console.WriteLine($"Sending file {fileName}");

			Byte[] bytes = File.ReadAllBytes(fileName);

			int offset = 0;
			while(fileSize > BUFSIZE)
			{
				io.Write(bytes, offset, BUFSIZE);
				offset += BUFSIZE;
				fileSize -= BUFSIZE;
				Console.WriteLine($"Transferred {offset} of {tempFileSize} bytes");
			}
            
			io.Write(bytes, offset, (int)fileSize);
			Console.WriteLine($"Transferred {offset + (int)fileSize} of {tempFileSize} bytes");

			Console.WriteLine("File sent!");
			LIB.writeTextTCP(io, $"File {fileName} sent");


        }

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>fileSize
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Server starts...");
			new file_server();
            
		}
	}
}
