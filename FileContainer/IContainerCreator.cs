using System.Collections.Generic;

namespace FileContainer
{
    /// <summary>
    /// Represents a class used to convert files to a container
    /// </summary>
    public interface IContainerCreator
    {
        /// <summary>
        /// Creates new container
        /// </summary>
        /// <param name="containerName">full path where container going to be stored</param>
        /// <param name="files">list of files</param>
        bool CreateContainer(string containerName, List<ContainerEntry> files);

        /// <summary>
        /// Return the list of files which represents current container
        /// </summary>
        /// <param name="containerName">full path to the container</param>
        /// <returns>List of ContainerEntry</returns>
        List<ContainerEntry> GetListOfFilesFromContainer(string containerName);

        /// <summary>
        /// Extracts files from the container
        /// </summary>
        /// <param name="containerName">full path to container</param>
        /// <param name="path">path where files have to be saved</param>
        /// <param name="files">list of files to extract</param>
        void ExtractFromContainer(string containerName, string path, List<ContainerEntry> files);

        /// <summary>
        /// Adds new files to the existing container
        /// </summary>
        /// <param name="containerName">full path to the container</param>
        /// <param name="files">list of files</param>
        void AddToContainer(string containerName, List<ContainerEntry> files);
    }
}
