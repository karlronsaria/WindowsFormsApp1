using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class File
    {
        public static bool
        GetContainerAndLeaf(string path, out string container, out string leaf)
        {
            var nodes = System.Linq.Enumerable.ToList(
                path.Split(System.IO.Path.DirectorySeparatorChar)
            );

            if (!nodes.Any())
            {
                container = "";
                leaf = path;
                return false;
            }

            leaf = nodes[nodes.Count - 1];
            nodes.RemoveAt(nodes.Count - 1);
            container = string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), nodes);
            return true;
        }

        public static bool
        GetNameAndExtension(string filename, out string name, out string extension)
        {
            var split = System.Linq.Enumerable.ToList(
                filename.Split('.')
            );

            if (!split.Any())
            {
                name = filename;
                extension = "";
                return false;
            }

            extension = split[split.Count - 1];
            split.RemoveAt(split.Count - 1);
            name = string.Join(".", split);
            return true;
        }

        public static void
        NewDatedFilePath(string filePath, out string newName, out string newPath)
        {
            bool hasContainer = GetContainerAndLeaf(
                path: filePath,
                out string container,
                out string leaf
            );

            bool hasExtension = GetNameAndExtension(
                filename: leaf,
                out string name,
                out string extension
            );

            bool hasDateTime = DateText.TryReplaceDateTimeString(
                input: name,
                out newName
            );

            if (!hasDateTime)
                newName += DateText.DELIMITER + DateTime.Now.ToString(DateText.DATETIME_FORMAT);

            if (hasExtension)
                newName = $"{newName}.{extension}";

            newPath = hasContainer
                ? System.IO.Path.Combine(container, newName)
                : newName;
        }
    }
}







