using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace tcp
{
	
	class file_client
	{
		/// <summary>
		/// The PORT.
		/// </summary>
		const int PORT = 9000;
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		const int BUFSIZE = 1000;

        // Set IP address
        string server = "10.0.0.1";

		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments. First ip-adress of the server. Second the filename
		/// </param>
		/// 

		long fileSize = 0;
		string FilePathServer = "/root/Documents/OPG6-IKN/Exercise_6_c#/Documents/";
		string FileSavePath = "/root/Documents/Opg6/OPG6-IKN/Exercise_6_c#/file_client/bin/Debug/";
        private file_client (string[] args)
		{
            try
            {
                TcpClient client = new TcpClient(server, PORT);


                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();
                NetworkStream stream = client.GetStream();
				Console.WriteLine("What file do you want from the server?");

				string fileName = Console.ReadLine();
                
				LIB.writeTextTCP(stream, FilePathServer + fileName);

				fileSize = LIB.getFileSizeTCP(stream);
                            

				if(fileSize > 0)
				{
					Console.WriteLine($"File exists and has size {fileSize}");
					Console.WriteLine("Saving file...");
					FileSavePath += fileName;
					receiveFile(FileSavePath, stream);

				}

				else
				{
					Console.WriteLine("File does not exist.");
				}

                
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }


            // TO DO Your own code

        }

		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='io'>
		/// Network stream for reading from the server
		/// </param>
		private void receiveFile (String fileName, NetworkStream stream)
		{
			int tempFileSize = (int)fileSize;
            
			byte[] bytes = new byte[fileSize];

			int offset = 0;
            
			while(fileSize > BUFSIZE)
			{
				stream.Read(bytes, offset, BUFSIZE);
				offset += BUFSIZE;
				fileSize -= BUFSIZE;
				Console.WriteLine($"Read {offset} of {tempFileSize} bytes");
			}
			stream.Read(bytes, offset, (int)fileSize);
			Console.WriteLine($"Read {offset + (int)fileSize} of {tempFileSize} bytes");
            
			File.WriteAllBytes(fileName, bytes);
            stream.Close();
        }



		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Client starts...");
			new file_client(args);
		}
	}
}
