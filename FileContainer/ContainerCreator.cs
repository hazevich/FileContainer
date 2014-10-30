using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FileContainer
{
    /// <summary>
    /// Converts list of files to one container
    /// </summary>
    public class ContainerCreator : IContainerCreator
    {
        private const int _maxBufferSize = 4096;

        /// <summary>
        /// Creates new container
        /// </summary>
        /// <param name="containerName">full path where container going to be stored</param>
        /// <param name="files">list of files</param>
        /// <returns>Returns true if creation succeed and false if not</returns>
        public bool CreateContainer(string containerName, List<ContainerEntry> files)
        {
            if (files.Find(f => f.Path == containerName) != null)
                return false;

            Container container = new Container()
            {
                ContainerEntries = files
            };

            BinaryFormatter binaryFormatter = new BinaryFormatter();

            foreach (ContainerEntry c in container.ContainerEntries)
            {
                c.Offset += 4; //4 bytes are reserved by container header
                c.EndOffset += 4; //4 bytes are reserved by container header
            }
            int header = container.ContainerEntries[container.ContainerEntries.Count - 1].EndOffset;
            using (FileStream fileStream = new FileStream(containerName, FileMode.Create))
            {
                fileStream.Write(BitConverter.GetBytes(header), 0, 4);

                foreach (ContainerEntry file in files)
                {
                    using (FileStream sourceFileStream = File.OpenRead(file.Path))
                    {
                        WriteByChunks(sourceFileStream, fileStream);
                    }
                }

                binaryFormatter.Serialize(fileStream, container);
            }

            return true;
        }

        /// <summary>
        /// Return the list of files which represents current container
        /// </summary>
        /// <param name="containerName">full path to the container</param>
        /// <returns>List of ContainerEntry</returns>
        public List<ContainerEntry> GetListOfFilesFromContainer(string containerName)
        {
            Container container;

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using(FileStream fileStream = new FileStream(containerName, FileMode.Open))
            {
                byte[] header = new byte[4];
                fileStream.Read(header, 0, 4);

                fileStream.Position = BitConverter.ToInt32(header, 0);

                container = binaryFormatter.Deserialize(fileStream) as Container;
            }

            return container.ContainerEntries;
        }

        /// <summary>
        /// Extracts files from the container
        /// </summary>
        /// <param name="containerName">full path to container</param>
        /// <param name="path">path where files have to be saved</param>
        /// <param name="files">list of files to extract</param>
        public void ExtractFromContainer(string containerName, string path, List<ContainerEntry> files)
        {
            using (FileStream containerStream = new FileStream(containerName, FileMode.Open))
            {
                foreach (ContainerEntry file in files)
                {
                    using(FileStream fileStream = new FileStream(Path.Combine(path, file.Name), FileMode.Create))
                    {
                        SaveFileByChunks(fileStream, containerStream, file.Offset, file.EndOffset);
                    }
                }
            }
        }

        /// <summary>
        /// Adds new files to the existing container
        /// </summary>
        /// <param name="containerName">full path to the container</param>
        /// <param name="files">list of files</param>
        public void AddToContainer(string containerName, List<ContainerEntry> files)
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();

            Container container = new Container()
            {
                ContainerEntries = GetListOfFilesFromContainer(containerName)
            };

            List<ContainerEntry> newFiles = new List<ContainerEntry>();

            files.ForEach(file =>
                {
                    if (!container.ContainerEntries.Contains(file))
                        newFiles.Add(file);
                });

            container.ContainerEntries.AddRange(newFiles);


            int header = container.ContainerEntries[container.ContainerEntries.Count - 1].EndOffset;
            


            using (FileStream fileStream = new FileStream(containerName, FileMode.Open, FileAccess.ReadWrite))
            {
                Console.WriteLine(fileStream.Length);
                int initialFooterLocation = ReadHeader(fileStream);
                fileStream.Position = 0;
                WriteHeader(fileStream, header);
                fileStream.Position = initialFooterLocation;

                foreach (var file in newFiles)
                {
                    using(FileStream sourceStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read))
                    {
                        WriteByChunks(sourceStream, fileStream);
                    }
                }

                Console.WriteLine(fileStream.Length);

                binaryFormatter.Serialize(fileStream, container);

                Console.WriteLine(fileStream.Length);
            }
        }

        private void SaveFileByChunks(Stream sourceStream, Stream containerStream, int startOffset, int endOffset)
        {
            int remainderBuffer = (endOffset - startOffset) % 4096;

            containerStream.Position = startOffset;

            if (remainderBuffer > 0)
            {
                byte[] remainder = new byte[remainderBuffer];
                containerStream.Read(remainder, 0, remainderBuffer);
                sourceStream.Write(remainder, 0, remainderBuffer);
            }

            int chunksLeftToRead = (endOffset - startOffset) / 4096;


            byte[] buffer = new byte[4096];
            for (int i = 0; i < chunksLeftToRead; i++)
            {
                containerStream.Read(buffer, 0, buffer.Length);
                sourceStream.Write(buffer, 0, buffer.Length);
            }
        }

        private void WriteByChunks(Stream sourceStream, Stream containerStream)
        {
            int bufferSize = 4096;
            byte[] fileChunk = new byte[bufferSize];

            int remainderChunkLength = (int)sourceStream.Length % bufferSize;
            if (remainderChunkLength > 0)
            {
                byte[] remainderChunk = new byte[remainderChunkLength];
                sourceStream.Read(remainderChunk, 0, remainderChunk.Length);
                containerStream.Write(remainderChunk, 0, remainderChunk.Length);
            }

            int chunksLeft = (int)sourceStream.Length / bufferSize;

            for (int i = 0; i < chunksLeft; i++)
            {
                sourceStream.Read(fileChunk, 0, fileChunk.Length);
                containerStream.Write(fileChunk, 0, fileChunk.Length);
            }
        }

        private void WriteHeader(Stream stream, int footerLocation)
        {
            stream.Write(BitConverter.GetBytes(footerLocation), 0, 4);
        }

        private int ReadHeader(Stream stream)
        {
            byte[] header = new byte[4];
            stream.Read(header, 0, 4);

            return BitConverter.ToInt32(header, 0);
        }
    }
}
