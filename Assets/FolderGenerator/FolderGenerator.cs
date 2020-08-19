using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

[CreateAssetMenu(fileName = "FolderGenerator", menuName = "CUSTOM/FolderGenerator")]
public class FolderGenerator : ScriptableObject
{
    #region Variables
    public List<string> exceptions;
    public List<string> folders;

    private List<string> list = new List<string>();
    private List<string> reference = new List<string>();

    #endregion

#if UNITY_EDITOR
    public void GenerateFolders()
    {
        for (int i = 0; i < folders.Count; i++)
            if (folders[i] != null)
            {
                if (!Directory.Exists("Assets/" + folders[i]))
                {
                    var folder = Directory.CreateDirectory("Assets/" + folders[i]);
                    var emptyTXT = File.CreateText("Assets/" + folders[i] + "/Empty");
                }
            }

        AssetDatabase.Refresh();
    }

    public void RenameFolders()
    {
        for (int i = 0; i < reference.Count; i++)
            if (reference[i] != folders[i] && folders[i] != string.Empty)
            {
                //Rename Asset
                AssetDatabase.RenameAsset("Assets/" + reference[i], folders[i] == null ? reference[i] : folders[i]);


                //Replace new name in hierarchie
                for (int j = i; j < folders.Count; j++)
                {
                    string tempo = string.Empty;
                    string[] divide = folders[j].Split('/');

                    for (int k = 0; k < divide.Length; k++)
                    {
                        if (divide[k] == reference[i])
                            divide[k] = folders[i];

                        tempo += divide[k];

                        if (k < divide.Length - 1)
                            tempo += "/";
                    }
                    folders[j] = tempo;
                }
            }

        AssetDatabase.Refresh();
    }


    public void GetFolders()
    {
        list.Clear();

        DirectoryInfo dir = new DirectoryInfo("Assets");
        DirectoryInfo[] info = dir.GetDirectories();

        foreach (DirectoryInfo f in info)
        {
            bool found = false;

            foreach (string s in exceptions)
                if (f.Name == s)
                    found = true;

            if(!found)
                GetChildren(f.Name);
        }

        reference = new List<string>(folders);
    }

    private void GetChildren(string _path)
    {
        list.Add(_path);
        folders = list;

        DirectoryInfo dir = new DirectoryInfo("Assets/" + _path);
        DirectoryInfo[] info = dir.GetDirectories();

        foreach (DirectoryInfo f in info)
                GetChildren(_path + "/" + f.Name);
    }
#endif
}
