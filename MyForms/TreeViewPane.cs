using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MyForms
{
    public class TreeViewPane
    {
        private readonly TreeView _myTreeView;
        private DirectoryInfo _currentDirectory;

        public DirectoryInfo CurrentDirectory { get => _currentDirectory; }

        public TreeViewPane(TreeView myTreeView, string startingDirectory)
        {
            _myTreeView = myTreeView;
            _currentDirectory = null;

            Load(
                directoryPath: startingDirectory,
                isHandled: false
            );
        }

        public static IEnumerable<TreeNode>
        GetSubdirectoryTree(
                IEnumerable<DirectoryInfo> subDirs
            )
        {
            TreeNode aNode;
            DirectoryInfo[] subSubDirs;

            foreach (DirectoryInfo subDir in subDirs)
            {
                try
                {
                    aNode = new TreeNode
                    {
                        Text = subDir.Name,
                        ImageIndex = 0,
                        SelectedImageIndex = 0,
                        Tag = subDir,
                        ImageKey = "folder",
                    };

                    subSubDirs = subDir.GetDirectories();

                    if (subSubDirs.Length != 0)
                        AddSubdirectoryToTree(subSubDirs, aNode);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }

                yield return aNode;
            }
        }

        public static void
        AddSubdirectoryToTree(
                IEnumerable<DirectoryInfo> subDirs,
                TreeNode nodeToAddTo
            )
        {
            foreach (var node in GetSubdirectoryTree(subDirs))
                nodeToAddTo.Nodes.Add(node);
        }

        public void Load(string directoryPath, bool isHandled = true)
        {
            Load(new DirectoryInfo(directoryPath), isHandled);
        }

        public void Load(DirectoryInfo directory, bool isHandled = true)
        {
            TreeNode rootNode;

            MyForms.Forms.InvokeIfHandled(
                _myTreeView,
                s => (s as TreeView).Nodes.Clear(),
                isHandled
            );

            if (directory.Exists)
            {
                _currentDirectory = directory;

                rootNode = new TreeNode
                {
                    Text = directory.Name,
                    Tag = directory,
                };
                
                AddSubdirectoryToTree(directory.GetDirectories(), rootNode);

                MyForms.Forms.InvokeIfHandled(
                    _myTreeView,
                    s => (s as TreeView).Nodes.Add(rootNode),
                    isHandled
                );
            }
        }
    }
}
