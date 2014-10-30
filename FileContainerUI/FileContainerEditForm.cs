using FileContainer;
using FileContainerUI.Waiter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace FileContainerUI
{
    public partial class  FileContainerEditForm : Form
    {
        private bool _loadSucceed = true; //Flag to determine if container loaded successfuly
        private bool _isModifiedMode = false; //Flag to determine if user wants to modify(True) or create(False) new container
        private IContainerManager _containerManager = new ContainerManager();
        private List<ContainerEntry> _files = new List<ContainerEntry>();
        private string _containerName;

        public FileContainerEditForm()
        {
            InitializeComponent();
        }

        public FileContainerEditForm(string containerName)
        {
            InitializeComponent();
            this._isModifiedMode = true;
            this.extractSelected.Visible = true;
            this.extractAllButton.Visible = true;
            _containerName = containerName;

            try
            {
                _files = _containerManager.GetListOfFilesFromContainer(_containerName);


                _files.ForEach(f =>
                {
                    this.fileContainerListView.Items.Add(new ListViewItem(f.Name));
                });
            }
            catch (Exception)
            {
                MessageBox.Show(string.Format("Cannot read {0}", _containerName));
                _loadSucceed = false;
            }
        }
        public void ShowDialog()
        {
            if (_loadSucceed)
                base.ShowDialog();
            else
                this.Dispose();
        }

        private void addFilesButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;


            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] fileNames = openFileDialog.FileNames;

                int startOffset = 0;

                if (_files.Count > 0)
                {
                    startOffset = _files[_files.Count - 1].EndOffset;
                }

                foreach (string fileName in fileNames)
                {
                    ContainerEntry containerEntry = new ContainerEntry()
                    {
                        Name = fileName.Remove(0, fileName.LastIndexOf('\\') + 1),
                        Path = fileName,
                        Offset = startOffset
                    };

                    if (!_files.Contains(containerEntry))
                    {
                        startOffset += (int)new FileInfo(fileName).Length;
                        containerEntry.EndOffset = startOffset;

                        _files.Add(containerEntry);

                        fileContainerListView.Items.Add(new ListViewItem(containerEntry.Name));
                    }
                }

                this.saveButton.Enabled = true;
                if(_isModifiedMode)
                {
                    ExtractButtonsAreEnabled(false);
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!_isModifiedMode)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();

                saveFileDialog.Filter = "container files (*.cntr)|*.cntr";
                saveFileDialog.FilterIndex = 0;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    bool success = true;
                    WaiterForm waiter = new WaiterForm();
                    waiter.Show(() =>
                        {
                            string fileName = saveFileDialog.FileName;

                            if (!_isModifiedMode)
                            {
                                success = _containerManager.CreateContainer(fileName, _files);
                            }
                            
                        });
                    if (success)
                        this.Dispose();
                    else
                        MessageBox.Show("Cannot write file to itself.\nPlease, choose another destination.");
                }
            }
            else
            {
                 WaiterForm waiter = new WaiterForm();
                 waiter.Show(() =>
                     {
                         _containerManager.AddToContainer(_containerName, _files);
                     });

                 ExtractButtonsAreEnabled(true);
            }
        }

        private void extractAllButton_Click(object sender, EventArgs e)
        {
            ExtractAction(_files);
        }

        private void extractSelected_Click(object sender, EventArgs e)
        {

            if(fileContainerListView.SelectedItems.Count > 0)
            {
                List<ContainerEntry> selectedFiles = new List<ContainerEntry>();
                foreach (ListViewItem item in fileContainerListView.SelectedItems)
                {
                    selectedFiles.Add(_files.Find(f => f.Name == item.Text));
                }
                ExtractAction(selectedFiles);
            }
        }

        private void ExtractAction(List<ContainerEntry> files)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                List<ContainerEntry> selectedFiles = new List<ContainerEntry>();

                new WaiterForm().Show(() => _containerManager.ExtractFromContainer(_containerName, folderBrowserDialog.SelectedPath, files));
            }
        }

        private void ExtractButtonsAreEnabled(bool expression)
        {
            this.extractSelected.Enabled = expression;
            this.extractAllButton.Enabled = expression;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
