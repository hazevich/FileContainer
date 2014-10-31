using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FileContainer
{
    /// <summary>
    /// Converts list of files to one container
    /// </summary>
    public class ContainerManager : IContainerManager
    {
        private const int MaxBufferSize = 4096;
        private const int HeaderByteSize = 4;

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

            var binaryFormatter = new BinaryFormatter();

            foreach (ContainerEntry file in files)
            {
                file.Offset += HeaderByteSize; //4 bytes are reserved by container header
                file.EndOffset += HeaderByteSize; //4 bytes are reserved by container header
            }
            int header = files[files.Count - 1].EndOffset;
            using (var fileStream = new FileStream(containerName, FileMode.Create))
            {
                WriteHeader(fileStream, header);

                foreach (ContainerEntry file in files)
                {
                    using (FileStream sourceFileStream = File.OpenRead(file.Path))
                    {
                        WriteByChunks(sourceFileStream, fileStream);
                    }
                }

                binaryFormatter.Serialize(fileStream, files);
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
            var binaryFormatter = new BinaryFormatter();
            using(var fileStream = new FileStream(containerName, FileMode.Open))
            {
                fileStream.Position = ReadHeader(fileStream);

                return binaryFormatter.Deserialize(fileStream) as List<ContainerEntry>;
            }
        }

        /// <summary>
        /// Extracts files from the container
        /// </summary>
        /// <param name="containerName">full path to container</param>
        /// <param name="path">path where files have to be saved</param>
        /// <param name="files">list of files to extract</param>
        public void ExtractFromContainer(string containerName, string path, List<ContainerEntry> files)
        {
            using (var containerStream = new FileStream(containerName, FileMode.Open))
            {
                foreach (ContainerEntry file in files)
                    using(var fileStream = new FileStream(Path.Combine(path, file.Name), FileMode.Create))
                    {
                        SaveFileByChunks(fileStream, containerStream, file.Offset, file.EndOffset);
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
            var binaryFormatter = new BinaryFormatter();

            var existingEntries = GetListOfFilesFromContainer(containerName);

            var newFiles = new List<ContainerEntry>();

            files.ForEach(file =>
                {
                    if (!existingEntries.Contains(file))
                        newFiles.Add(file);
                });

            existingEntries.AddRange(newFiles);

            int header = existingEntries[existingEntries.Count - 1].EndOffset;
            
            using (var fileStream = new FileStream(containerName, FileMode.Open, FileAccess.ReadWrite))
            {
                int initialFooterLocation = ReadHeader(fileStream);
                fileStream.Position = 0;
                WriteHeader(fileStream, header);
                fileStream.Position = initialFooterLocation;

                foreach (var file in newFiles)
                {
                    using(var sourceStream = new FileStream(file.Path, FileMode.Open, FileAccess.Read))
                    {
                        WriteByChunks(sourceStream, fileStream);
                    }
                }

                binaryFormatter.Serialize(fileStream, existingEntries);
            }
        }

        /// <summary>
        /// Extracts file from container by chunks
        /// </summary>
        /// <param name="sourceStream">stream of the file</param>
        /// <param name="containerStream">stream of the container</param>
        /// <param name="startOffset">start offset of the file in container</param>
        /// <param name="endOffset">end offset of the file in container</param>
        private void SaveFileByChunks(Stream sourceStream, Stream containerStream, int startOffset, int endOffset)
        {
            int remainderBuffer = (endOffset - startOffset) % MaxBufferSize;

            containerStream.Position = startOffset;

            if (remainderBuffer > 0)
            {
                var remainder = new byte[remainderBuffer];
                containerStream.Read(remainder, 0, remainderBuffer);
                sourceStream.Write(remainder, 0, remainderBuffer);
            }

            int chunksLeftToRead = (endOffset - startOffset) / MaxBufferSize;


            var buffer = new byte[MaxBufferSize];
            for (int i = 0; i < chunksLeftToRead; i++)
            {
                containerStream.Read(buffer, 0, buffer.Length);
                sourceStream.Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// Writes file to the container by chunks
        /// </summary>
        /// <param name="sourceStream">stream of the file</param>
        /// <param name="containerStream">stream of the container</param>
        private void WriteByChunks(Stream sourceStream, Stream containerStream)
        {
            var fileChunk = new byte[MaxBufferSize];

            var remainderChunkLength = (int)sourceStream.Length % MaxBufferSize;
            if (remainderChunkLength > 0)
            {
                var remainderChunk = new byte[remainderChunkLength];
                sourceStream.Read(remainderChunk, 0, remainderChunk.Length);
                containerStream.Write(remainderChunk, 0, remainderChunk.Length);
            }

            var chunksLeft = (int)sourceStream.Length / MaxBufferSize;

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
            var header = new byte[4];
            stream.Read(header, 0, 4);

            return BitConverter.ToInt32(header, 0);
        }
    }
}
