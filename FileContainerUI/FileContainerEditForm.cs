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
        private bool _loadSucceed = true;
        private bool _inEditMode = false;
        private IContainerCreator _containerCreator = new ContainerCreator();
        private List<ContainerEntry> _files = new List<ContainerEntry>();
        private string _containerName;

        public FileContainerEditForm()
        {
            InitializeComponent();
        }

        public FileContainerEditForm(string containerName)
        {
            InitializeComponent();
            this._inEditMode = true;
            this.extractSelected.Visible = true;
            this.extractAllButton.Visible = true;
            _containerName = containerName;

            try
            {
                _files = _containerCreator.GetListOfFilesFromContainer(_containerName);


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
                if(_inEditMode)
                {
                    ExtractButtonsAreEnabled(false);
                }
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (!_inEditMode)
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

                            if (!_inEditMode)
                            {
                                success = _containerCreator.CreateContainer(fileName, _files);
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
                         _containerCreator.AddToContainer(_containerName, _files);
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

                new WaiterForm().Show(() => _containerCreator.ExtractFromContainer(_containerName, folderBrowserDialog.SelectedPath, files));
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
