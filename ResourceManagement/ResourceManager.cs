#define PATCH_OVERLAPPING_NAMES

using RpgGridUserControls;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilsData;

namespace ResourceManagement
{
    public sealed partial class ResourceManager
    {
        private static ResourceManager current;
        public static ResourceManager Current
        {
            get
            {
                if(current == null)
                {
                    current = new ResourceManager();
                }
                return current;
            }
        }

        public bool ParallelExecution { get; set; }

        #region PATHS

        private string pawnsFolder;
        public string PawnsFolder
        {
            get
            {
                if (pawnsFolder == null || pawnsFolder == "")
                {
                    pawnsFolder = Path.Combine(Application.StartupPath, "Data", "Pawns");
                }
                return pawnsFolder;
            }
        }

        private string templatesFolder;
        public string TemplatesFolder
        {
            get
            {
                if (templatesFolder == null || templatesFolder == "")
                {
                    templatesFolder = Path.Combine("Data", "Templates");
                }

                return templatesFolder;
            }
        }

        private string mapsFolder;
        public string MapsFolder
        {
            get
            {
                if (mapsFolder == null || mapsFolder == "")
                {
                    mapsFolder = Path.Combine("Data", "Maps");
                }

                return mapsFolder;
            }
        }

        #endregion

        private const string pawnExt = "pwn";
        private const string templateExt = "tpl";

        public const string pawnFilePattern = "*." + pawnExt;
        public const string templateFilePattern = "*." + templateExt;

        private string[] folderTree;
        private string[] FolderTree
        {
            get
            {
                if (folderTree == null)
                {
                    folderTree = new string[]
                    {
                        MapsFolder,
                        PawnsFolder,
                        TemplatesFolder,
                    };
                }

                return folderTree;
            }
        }

        private ResourceManager()
        {
            ParallelExecution = true;

            CreateFolderTree();   
        }

        private void CreateFolderTree()
        {
            for (int i = 0; i < FolderTree.Length; i++)
            {
                if (!Directory.Exists(FolderTree[i]))
                {
                    Directory.CreateDirectory (FolderTree[i]);
                }
            }
        }

        public delegate void GridPawnResultCallback(GridPawn[] pawns);
        public delegate void GridPawnTemplateResultCallback(CharacterPawnTemplate[] pawns);
        public delegate void ErrorCallback(Exception ex);

        public GridPawn[] RetrievePawns()
        {
            var filePawns = Directory.GetFiles(PawnsFolder, pawnFilePattern);

            var pawns = new GridPawn[filePawns.Length];

            if (pawns.Length > 0)
            {
                var partition = Partitioner.Create(0, pawns.Length);
                var parOption = new ParallelOptions()
                {
                    MaxDegreeOfParallelism = ParallelExecution ? -1 : 1
                };

                Parallel.ForEach(partition, parOption, p =>
                {
                    for (int i = p.Item1; i < p.Item2; i++)
                    {
                        //pawns[i] = new CharacterPawn()
                        //{
                        //    CurrentPf = 59,
                        //    MaxPf = 60,
                        //    Image = Image.FromFile(@"character1.png"),
                        //    ModSize = i > 10 ? RpgGridUserControls.GridPawn.RpgSize.Large : RpgGridUserControls.GridPawn.RpgSize.Medium,
                        //};

                        pawns[i] = RetrievePawnFromFile(filePawns[i]);
                    };
                });
            }

            return pawns;
        }

        public GridPawn RetrievePawnFromFile(string pawnFile)
        {
            using (var fs = new FileStream(pawnFile, FileMode.OpenOrCreate))
            {
                return (CharacterPawn)Utils.BinaryFormatter.Deserialize(fs);
            }
        }

        public CharacterPawnTemplate[] RetrievePawnTemplates()
        {
            var fileTemplates = Directory.GetFiles(TemplatesFolder, templateFilePattern);

            var templates = new CharacterPawnTemplate[fileTemplates.Length];

            if (templates.Length > 0)
            {
                var partition = Partitioner.Create(0, templates.Length);
                var parOption = new ParallelOptions()
                {
                    MaxDegreeOfParallelism = ParallelExecution ? -1 : 1
                };

                Parallel.ForEach(partition, parOption, p =>
                {
                    for (int i = p.Item1; i < p.Item2; i++)
                    {
                        //templates[i] = CharacterPawnTemplate.Builder.Create();
                        templates[i] = RetrieveTemplateFromFile(fileTemplates[i]);
                    };
                });
            }

            return templates;
        }

        private CharacterPawnTemplate RetrieveTemplateFromFile(string templateFile)
        {
            using (var fs = new FileStream(templateFile, FileMode.OpenOrCreate))
            {
                return (CharacterPawnTemplate)Utils.BinaryFormatter.Deserialize(fs);
            }
        }

        public void SavePawn(CharacterPawn pawn, string path)
        {
            //var path = Path.ChangeExtension(Path.Combine(PawnsFolder, pawn.UniqueID), pawnExt);
            using (var fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                Utils.BinaryFormatter.Serialize(fs, pawn);
            }
        }

        public void SaveTemplate(CharacterPawnTemplate template, string path)
        {
            using (var fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                Utils.BinaryFormatter.Serialize(fs, template);
            }
        }

        public void AsyncRetrievePawns(GridPawnResultCallback callback, ErrorCallback onError = null)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (s, e) =>
            {
                e.Result = RetrievePawns();
            };
            bw.RunWorkerCompleted += (s, e) =>
            {
                if (e.Error == null)
                {
                    callback((GridPawn[])e.Result);
                }
                else
                {
                    if (onError != null)
                    {
                        onError(e.Error);
                    }
                }
            };

            bw.RunWorkerAsync();
        }

        public void AsyncRetrievePawnTemplates(GridPawnTemplateResultCallback callback, ErrorCallback onError = null)
        {
            var bw = new BackgroundWorker();
            bw.DoWork += (s, e) =>
            {
                e.Result = RetrievePawnTemplates();
            };
            bw.RunWorkerCompleted += (s, e) =>
            {
                if (e.Error == null)
                {
                    callback((CharacterPawnTemplate[])e.Result);
                }
                else
                {
                    if (onError != null)
                    {
                        onError(e.Error);
                    }
                }
            };

            bw.RunWorkerAsync();
        }

        public string SaveMap(byte[] buffer, string name)
        {
            var outpath = Path.Combine(ResourceManager.Current.MapsFolder, name);

#if DEBUG && PATCH_OVERLAPPING_NAMES
                if (File.Exists(outpath))
                {
                    var noExtName = Path.GetFileNameWithoutExtension(name);
                    var ext = Path.GetExtension(outpath);
                    var count = Directory.GetFiles(ResourceManager.Current.MapsFolder, noExtName + "*" + ext).Length;
                    outpath = Path.Combine(ResourceManager.Current.MapsFolder, String.Format("{0}_{1}{2}", noExtName, count, ext));
                }
#endif

            using (var ms = new MemoryStream(buffer))
            {
                ms.Position = 0;
                var img = Image.FromStream(ms);
                //Utils.ShowImage(img);
                img.Save(outpath);

                //using (var fs = new FileStream(outpath, FileMode.OpenOrCreate))
                //{
                //    ms.WriteTo(fs);
                //    fs.Flush();
                //    fs.Close();
                //}

                ms.Close();
                //ms.Flush();
            }

            return outpath;
        }
    }
}
