#define PATCH_OVERLAPPING_NAMES
#define STRANGE_EXCEPTION_ON_PAWN_RECEIVING_ANALYSIS
#define STRANGE_EXCEPTION_ON_MAP_RECEIVING_ANALYSIS
#define DUMP_IMAGE_BUFFER_TO_FILE

using NetUtils;
using ResourceManagement;
using RpgGridUserControls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UtilsData;

namespace RpgGrid
{
    public class RpgGrid : CommunicationModel
    {
#if DEBUG
        public event EventHandler<VerboseDebugArgs> VerboseDebugging;
#endif

        #region CONTROLS
        public Form ViewContainer { get; private set; }

        public ResourceManager ResourceManager{ get; private set; }
        private Grid mainGrid;
        public Grid MainGrid
        {
            get
            {
                return mainGrid;
            }

            private set
            {
                mainGrid = value;
#region PAWN_REMOVED
                mainGrid.PawnRemoved += (s, e) =>
                {
                    LastModifiedPawn = e.Pawn;
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs(String.Format("Removed pawn: {0}", e.Pawn.Name)));
#endif
                    if (e.Propagate)
                    {
                        Connections.Current.Broadcast(  Connections.Commands.PawnRemoved,
                                                        Connections.PAWN_REMOVED);
                    }
                    else
                    {
#if DEBUG
                        OnVerboseDebugging(new VerboseDebugArgs(String.Format("{0} has been removed [ID: {1}]",
                                                                                LastModifiedPawn.Name,
                                                                                LastModifiedPawn.UniqueID)));
#endif
                    }
                };
#endregion
#region PAWN_DRAG_DROPPED
                mainGrid.PawnDragDropped += (s, e) =>
                {
                    LastTouchedPawn = e.Pawn;
                    LastPositionOfGivenPawn = new TaggedObject<Point, string>(e.Location, LastTouchedPawn.UniqueID);

                    if (e.AlreadyContained)
                    {
#if DEBUG
                        OnVerboseDebugging(new VerboseDebugArgs(String.Format("{0} was already in grid [{1}]", LastTouchedPawn.Name, LastTouchedPawn.UniqueID)));
#endif
                    }
                    else
                    {
#if DEBUG
                        OnVerboseDebugging(new VerboseDebugArgs(String.Format(  "{0} added to grid in ({2};{3})[{1}]", 
                                                                                LastTouchedPawn.Name, 
                                                                                LastTouchedPawn.UniqueID,
                                                                                LastPositionOfGivenPawn.Obj.X,
                                                                                LastPositionOfGivenPawn.Obj.Y)));
#endif
                        Connections.Current.Broadcast(  Connections.Commands.AddPawnToGrid, 
                                                        new string[] { Connections.PAWN_ADDED_TO_GRID, Connections.PAWN_CLIENT_LOCATION, });
                    }
                };
                #endregion
#region TEMPLATE_DRAG_DROPPED
                mainGrid.TemplateDragDropped += (s, e) =>
                {
                    LastTouchedPawn = e.Pawn;
                    LastPositionOfGivenPawn = new TaggedObject<Point, string>(e.Location, LastTouchedPawn.UniqueID);
                    if (e.AlreadyContained)
                    {
#if DEBUG
                        OnVerboseDebugging(new VerboseDebugArgs(String.Format("{0} from Template was already in grid [{1}]", LastTouchedPawn.Name, LastTouchedPawn.UniqueID)));
#endif
                    }
                    else
                    {
#if DEBUG
                        OnVerboseDebugging(new VerboseDebugArgs(String.Format("{0} added to grid, from Template [{1}]", LastTouchedPawn.Name, LastTouchedPawn.UniqueID)));
#endif
                        Connections.Current.Broadcast(  Connections.Commands.AddPawnFromTemplateToGrid, 
                                                        new string[] {  Connections.TEMPLATE_ADDED_TO_GRID,
                                                                        Connections.PAWN_CLIENT_LOCATION, });
                    }
                };
                #endregion
#region PAWN_MOVED
                mainGrid.PawnMoved += (s, e) =>
                {
                    LastTouchedPawn = e.Pawn;
                    LastPositionOfGivenPawn = new TaggedObject<Point, string>(e.Location, LastTouchedPawn.UniqueID);

#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs(String.Format(  "{0} moved to ({1};{2}) [ID: {3}]",
                                                                            LastTouchedPawn.Name,
                                                                            LastPositionOfGivenPawn.Obj.X,
                                                                            LastPositionOfGivenPawn.Obj.Y,
                                                                            LastTouchedPawn.UniqueID)));
#endif
                    Connections.Current.Broadcast(Connections.Commands.MovePawnTo,
                                                    Connections.PAWN_CLIENT_LOCATION);
                };
                #endregion
#region PAWN_ROTATED
                mainGrid.PawnRotated90Degrees += (s, e) =>
                {
                    LastModifiedPawn = e.Pawn;
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs(String.Format("{0} rotated 90° [ID: {1}]",
                                                                            LastModifiedPawn.Name,
                                                                            LastModifiedPawn.UniqueID)));
#endif
                    if (e.Propagate)
                    {
                        Connections.Current.Broadcast(Connections.Commands.PawnRotated90Degrees,
                                                        Connections.PAWN_ROTATION_90_DEGREES);
                    }
                    else
                    {
#if DEBUG
                        OnVerboseDebugging(new VerboseDebugArgs(String.Format("{0} rotation 90°  received [ID: {1}]",
                                                                                LastModifiedPawn.Name,
                                                                                LastModifiedPawn.UniqueID)));
#endif
                    }
                };
#endregion
#region PAWN_BORDER_COLOR_CHANGED
                mainGrid.PawnBorderColorChanged += (s, e) =>
                {
                    LastModifiedPawn = e.Pawn;
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs(String.Format("{0} changed border color [ID: {1}]",
                                                                            LastModifiedPawn.Name,
                                                                            LastModifiedPawn.UniqueID)));
#endif
                    if (e.Propagate)
                    {
                        Connections.Current.Broadcast(  Connections.Commands.PawnChangedBorderColor,
                                                        new string[] {
                                                            Connections.PAWN_THAT_CHANGED,
                                                            Connections.PAWN_BORDER_COLOR,
                                                        });
                    }
                    else
                    {
#if DEBUG
                        OnVerboseDebugging(new VerboseDebugArgs(String.Format("{0} border color received  received [ID: {1}]",
                                                                                LastModifiedPawn.Name,
                                                                                LastModifiedPawn.UniqueID)));
#endif
                    }
                };
#endregion
#region BACKGROUND_IMAGE_CHANGED
                mainGrid.BackgroundImageChanged += (s, e) =>
                {
                    for (int i = 0; i < mainGrid.Controls.Count; i++)
                    {
                        var removed = mainGrid.Remove((GridPawn)mainGrid.Controls[i], false);
                        MainPawnManager.LoadPawn(removed);
                    }
                };
                mainGrid.GridImageChangedByDialog += (s, e) =>
                {
                    //for (int i = 0; i < mainGrid.Controls.Count; i++)
                    //{
                    //    var removed = mainGrid.Remove((GridPawn)mainGrid.Controls[i]);
                    //    MainPawnManager.LoadPawn(removed);
                    //}
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs("Grid image has changed"));
#endif
                    Connections.Current.Broadcast(  Connections.Commands.MapReceived,
                                                    new string[] {
                                                        Connections.MAP_NAME_SENDING,
                                                        Connections.MAP_EXTRAINFO_SENDING,
                                                        Connections.MAP_SENDING,
                                                    });
                };
#endregion
            }
        }

        private CharacterPawn LastCreatedPawn;
        private CharacterPawnTemplate LastCreatedPawnTemplate;

        private PawnManager mainPawnManager;
        public PawnManager MainPawnManager
        {
            get
            {
                return mainPawnManager;
            }

            private set
            {
                mainPawnManager = value;

#region NEW AND SAVING PAWN
                mainPawnManager.SavePawn += (s, e) =>
                {
                    using (var sDlg = new SaveFileDialog())
                    {
                        sDlg.InitialDirectory = Path.Combine(Application.StartupPath, ResourceManager.Current.PawnsFolder);
                        sDlg.Filter = "Pawn|" + ResourceManager.pawnFilePattern;

                        var result = sDlg.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            ResourceManager.Current.SavePawn(e.Pawn, sDlg.FileName);
                        }
                    }
                };

                mainPawnManager.CreateNewPawn += (s, e) =>
                {
                    mainPawnManager.LoadPawn(e.Pawn);
                    LastCreatedPawn = e.Pawn;
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs("Broadcasted new pawn to clients"));
#endif
                    Connections.Current.Broadcast(  Connections.Commands.CreateNewPawn,
                                                    Connections.CREATED_NEW_PAWN);
                };
#endregion
#region NEW AND SAVING TEMPLATE
                mainPawnManager.SaveTemplate += (s, e) =>
                {
                    using (var sDlg = new SaveFileDialog())
                    {
                        sDlg.InitialDirectory = Path.Combine(Application.StartupPath, ResourceManager.Current.TemplatesFolder);
                        sDlg.Filter = "Template|" + ResourceManager.templateFilePattern;

                        var result = sDlg.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            ResourceManager.Current.SaveTemplate(e.Template, sDlg.FileName);
                        }
                    }
                };

                mainPawnManager.CreateNewTemplate += (s, e) =>
                {
                    MessageBox.Show("TODO - Create a Template");
                };
#endregion
            }
        }

        private GridPawn lastTouchedPawn;
        public GridPawn LastTouchedPawn
        {
            get
            {
                if(lastTouchedPawn == null)
                {
                    throw new Exception("No pawn touched");
                }

                return lastTouchedPawn;
            }

            set
            {
                lastTouchedPawn = value;
            }
        }

        public GridPawn LastModifiedPawn { get; private set; }
        public GridPawnValueChangedEventArgs.ChangeableItems LastChangedValueType { get; private set; }
        public byte[] LastChangedValue { get; private set; }

        private TaggedObject<Point, string> lastPositionOfGivenPawn;
        public TaggedObject<Point, string> LastPositionOfGivenPawn
        {
            get
            {
                if (lastPositionOfGivenPawn == null)
                {
                    throw new Exception("No pawn moved");
                }

                return lastPositionOfGivenPawn;
            }

            set
            {
                lastPositionOfGivenPawn = value;
            }
        }

        private GridPawnValueController gridPawnController;
        public GridPawnValueController GridPawnController
        {
            get
            {
                return gridPawnController;
            }

            private set
            {
                gridPawnController = value;
                gridPawnController.ValueChanged += (s, e) =>
                {
                    LastModifiedPawn = e.Pawn;
                    LastChangedValueType = e.ValueChanged;
                    LastChangedValue = e.GetValueBuffer(GetBytesFromString);
                    Connections.Current.Broadcast(  Connections.Commands.PawnValueChanged, 
                                                    new string[] {
                                                        Connections.PAWN_THAT_CHANGED,
                                                        Connections.PAWN_VALUE_TYPE_CHANGED,
                                                        Connections.PAWN_VALUE_CHANGED,
                                                    });
                };
            }
        }
        #endregion

        public RpgGrid( Form viewContainer,
                        Grid mainGrid,
                        PawnManager mainPawnManager,
                        GridPawnValueController gridPawnController)
            : base()
        {
            ResourceManager = ResourceManager.Current;

            ViewContainer = viewContainer;
            MainGrid = mainGrid;
            MainPawnManager = mainPawnManager;
            GridPawnController = gridPawnController;
        }

        public override void ShowProcessing()
        {
            ViewContainer.Cursor = Cursors.WaitCursor;
        }

        public override void EndShowProcessing()
        {
            ViewContainer.Cursor = Cursors.Default;
        }

        #region RESPONSE METHODS

        protected override void InitReceiveProcessingMethods()
        {
            var processingDict = this.Processing;
            var behaviours = this.GetType()
                        .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                        .Where(m => m.GetCustomAttributes(typeof(ResponseMethods)).Count() == 1).ToArray();

            for (int i = 0; i < behaviours.Length; i++)
            {
                var b = behaviours[i];
                var cmd = ((ResponseMethods)b.GetCustomAttribute(typeof(ResponseMethods))).Semantic;
                var del = Delegate.CreateDelegate(typeof(CommunicationDataProcessing), this, b);
                processingDict[cmd] = (CommunicationDataProcessing)del;
            }
        }

        [ResponseMethods(Connections.MAP_NAME_SENDING)]
        private DataRes SendMapName(byte[] buffer)
        {
            var name = Path.GetFileName(MainGrid.ImagePath);            
#if DEBUG
            OnVerboseDebugging(new VerboseDebugArgs(String.Format("Sent map name: {0}", name)));
#endif
            return new DataRes(GetBytesFromString(name));
        }

        [ResponseMethods(Connections.MAP_EXTRAINFO_SENDING)]
        private DataRes SendMapExtraInfo(byte[] buffer)
        {
            var content = File.ReadAllText(Path.ChangeExtension(MainGrid.ImagePath, Grid.metricInfoExt));
#if DEBUG
            OnVerboseDebugging(new VerboseDebugArgs("Sending map extra info: " + Environment. NewLine + content));
#endif
            return new DataRes(GetBytesFromString(content));
        }

        [ResponseMethods(Connections.MAP_SENDING)]
        private DataRes SendMapData(byte[] buffer)
        {
            using (var ms = new MemoryStream())
            {
                MainGrid.Image.Save(ms, ImageFormat.Png);
                ms.Position = 0;

                var imageBuffer = ms.ToArray();
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Sent map: {0}x{1}", MainGrid.Image.Width, MainGrid.Image.Height)));
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Buffer map: {0}", imageBuffer.Length)));
#endif
#if DEBUG && DUMP_IMAGE_BUFFER_TO_FILE
                var strB = new StringBuilder();
                for (int i = 0; i < imageBuffer.Length; i++)
                {
                    strB.AppendLine(imageBuffer[i].ToString());
                }
                File.WriteAllText("send_img_dump.txt", strB.ToString());
#endif
                
                return new DataRes(imageBuffer);
            }
        }

        string tmpMapName;
        [ResponseMethods(Connections.MAP_NAME_RECEIVING)]
        private DataRes ReceiveMapName(byte[] buffer)
        {
            tmpMapName = GetStringFromByteArray(buffer);
#if DEBUG
            OnVerboseDebugging(new VerboseDebugArgs(String.Format("Received map name: {0}", tmpMapName)));
#endif
            return DataRes.Empty;
        }

        [ResponseMethods(Connections.MAP_EXTRAINFO_RECEIVING)]
        private DataRes ReceiveMapExtraInfo(byte[] buffer)
        {
            if (tmpMapName == null)
            {
                throw new Exception("Not received MapName");
            }
            else
            {
                var name = Path.ChangeExtension(tmpMapName, Grid.metricInfoExt);
                var outpath = Path.Combine(ResourceManager.Current.MapsFolder, name);
#if DEBUG && PATCH_OVERLAPPING_NAMES
                if(File.Exists(outpath))
                {
                    var noExtName = Path.GetFileNameWithoutExtension(name);
                    var ext = Path.GetExtension(outpath);
                    var count = Directory.GetFiles(ResourceManager.Current.MapsFolder, noExtName + "*" + ext).Length;
                    outpath = Path.Combine(ResourceManager.Current.MapsFolder, String.Format("{0}_{1}{2}", noExtName, count, ext));
                }
#endif
                var content = GetStringFromByteArray(buffer);
                File.WriteAllText(outpath, content);
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Received extra info: {0}", outpath)));
                OnVerboseDebugging(new VerboseDebugArgs("Sending map extra info: " + Environment.NewLine + content));
#endif
            }
            return DataRes.Empty;
        }

        [ResponseMethods(Connections.MAP_RECEIVING)]
        private DataRes ReceiveMapData(byte[] buffer)
        {
            if (tmpMapName == null)
            {
                throw new Exception("Not received MapName");
            }
            else
            {
                var name = Path.ChangeExtension(tmpMapName, "png");

#if DEBUG && STRANGE_EXCEPTION_ON_MAP_RECEIVING_ANALYSIS
                try
                {
#endif
#if DEBUG && DUMP_IMAGE_BUFFER_TO_FILE
                    var strB = new StringBuilder();
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        strB.AppendLine(buffer[i].ToString());
                    }
                    File.WriteAllText("save_img_dump.txt", strB.ToString());
#endif
                    MainGrid.ImagePath = ResourceManager.Current.SaveMap(buffer, name);
                    tmpMapName = null;
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs(String.Format("Received map: {0}x{1}", MainGrid.Image.Width, MainGrid.Image.Height)));
                    OnVerboseDebugging(new VerboseDebugArgs(String.Format("Buffer map: {0}", buffer.Length)));
#endif
#if DEBUG && STRANGE_EXCEPTION_ON_MAP_RECEIVING_ANALYSIS
                }
                catch (Exception ex)
                {
                    OnVerboseDebugging(new VerboseDebugArgs("ReceiveMapData: " + ex.Message));
                }
#endif
            }
            return DataRes.Empty;
        }

        [ResponseMethods(Connections.CREATED_NEW_PAWN)]
        private DataRes CreatedNewPawn(byte[] buffer)
        {
            if (buffer == null)
            {
                using (var ms = new MemoryStream())
                {
                    Utils.BinaryFormatter.Serialize(ms, LastCreatedPawn);
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs(String.Format("Send new created pawn: ID {0}", LastCreatedPawn.UniqueID)));
#endif
                    return new DataRes(ms.ToArray());
                }
            }
            else
            {
                using (var ms = new MemoryStream(buffer))
                {
                    var pawn = (CharacterPawn)Utils.BinaryFormatter.Deserialize(ms);
                    MainPawnManager.LoadPawn(pawn);
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs(String.Format("Received new created pawn: ID {0}", pawn.UniqueID)));
#endif
                    return DataRes.Empty;
                }
            }
        }

        [ResponseMethods(Connections.PAWNS_RECEIVING)]
        private DataRes ReceivePawns(byte[] buffer)
        {
            if (buffer.Length > 0)
            {
                using (var ms = new MemoryStream(buffer))
                {
#if DEBUG && STRANGE_EXCEPTION_ON_PAWN_RECEIVING_ANALYSIS
                    try
                    {
#endif
                        var pawns = (GridPawn[])Utils.BinaryFormatter.Deserialize(ms);
                        MainPawnManager.LoadPawns(pawns);
#if DEBUG
                        OnVerboseDebugging(new VerboseDebugArgs(String.Format("Received pawns: {0} bytes, {1} items", buffer.Length, pawns.Length)));
#endif
#if DEBUG && STRANGE_EXCEPTION_ON_PAWN_RECEIVING_ANALYSIS
                    }
                    catch (Exception ex)
                    {
                        Message(GetBytesFromString("ReceivePawns: " + ex.Message));
                    }
#endif
                }
            }

            return DataRes.Empty;
        }
        
        [ResponseMethods(Connections.PAWNS_SENDING)]
        private DataRes SendPawns(byte[] buffer)
        {
            var pawns = this.MainPawnManager.GetPawns();

            if (pawns != null && pawns.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    Utils.BinaryFormatter.Serialize(ms, pawns);
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs(String.Format("Sent pawns: {0} items, {1} bytes", pawns.Length, ms.Length)));
#endif
                    return new DataRes(ms.ToArray());
                }
            }
            else
            {
                return DataRes.Empty;
            }
        }

        [ResponseMethods(Connections.TEMPLATES_RECEIVING)]
        private DataRes ReceiveTemplates(byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer))
            {
#if DEBUG && STRANGE_EXCEPTION_ON_PAWN_RECEIVING_ANALYSIS
                try
                {
#endif
                    var templates = (CharacterPawnTemplate[])Utils.BinaryFormatter.Deserialize(ms);
                    MainPawnManager.LoadPawnTemplates(templates);
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs(String.Format("Received templates: {0} bytes, {1} items", buffer.Length, templates.Length)));
#endif
#if DEBUG && STRANGE_EXCEPTION_ON_PAWN_RECEIVING_ANALYSIS
                }
                catch (Exception ex)
                {
                    Message(GetBytesFromString("ReceiveTemplates: " + ex.Message));
                }
#endif
                return DataRes.Empty;
            }
        }

        [ResponseMethods(Connections.TEMPLATES_SENDING)]
        private DataRes SendTemplates(byte[] buffer)
        {
            var templates = this.MainPawnManager.GetPawnTemplates();

            if (templates != null && templates.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    Utils.BinaryFormatter.Serialize(ms, templates);
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs(String.Format("Sent templates: {0} items, {1} bytes", templates.Length, ms.Length)));
#endif
                    return new DataRes(ms.ToArray());
                }
            }
            else
            {
                return DataRes.Empty;
            }
        }

        [ResponseMethods(Connections.PAWN_ADDED_TO_GRID)]
        private DataRes PawnAddedToGrid(byte[] buffer)
        {
            if (buffer == null)
            {
                // Telling that pawn is added
                var pawnUniqueID = LastTouchedPawn.UniqueID;
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn is added to grid: {0} [OUT]", pawnUniqueID)));
#endif
                return new DataRes(GetBytesFromString(pawnUniqueID));
            }
            else
            {
                var pawnUniqueID = GetStringFromByteArray(buffer);
                MainGrid.DragDropAdding(MainPawnManager.GetByUniqueID(pawnUniqueID), new Point(), receivedFromOutside: true);
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn is added to grid: {0} [IN]", pawnUniqueID)));
#endif
                return DataRes.Empty;
            }
        }

        [ResponseMethods(Connections.TEMPLATE_ADDED_TO_GRID)]
        private DataRes TemplateAddedToGrid(byte[] buffer)
        {
            if (buffer == null)
            {
                // Telling that pawn is added
                var pawnUniqueID = LastTouchedPawn.UniqueID;
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn is added to grid: {0} [OUT]", pawnUniqueID)));
#endif

                using (var ms = new MemoryStream())
                {
                    LastTouchedPawn.NormalizeUniqueID();
                    Utils.BinaryFormatter.Serialize(ms, LastTouchedPawn);
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs("Pawn was generated: sent integrally"));
#endif
                    return new DataRes(ms.ToArray());
                }
            }
            else
            {
                var ms = new MemoryStream(buffer);
                var pawn = (GridPawn)Utils.BinaryFormatter.Deserialize(ms);
                LastTouchedPawn = pawn;
                MainGrid.AddIfNotPresent(pawn);
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn is added to grid from template: {0} [IN]", "Name??")));
#endif
                return DataRes.Empty;
            }
        }

        [ResponseMethods(Connections.PAWN_CLIENT_LOCATION)]
        private DataRes PawnClientLocation(byte[] buffer)
        {
            if (buffer == null)
            {
                // Telling that pawn is moved
                using (var ms = new MemoryStream())
                {
                    Utils.BinaryFormatter.Serialize(ms, LastPositionOfGivenPawn);
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs(String.Format("Sent movement: {0} in ({1};{2})", LastPositionOfGivenPawn.Obj, LastPositionOfGivenPawn.Obj.X, LastPositionOfGivenPawn.Obj.Y)));
#endif
                    return new DataRes(ms.ToArray());
                }
            }
            else
            {
                using (var ms = new MemoryStream(buffer))
                {
                    var locationOfMoved = (TaggedObject<Point, string>)Utils.BinaryFormatter.Deserialize(ms);
                    var movedPawn = MainGrid.GetByUniqueID(locationOfMoved.Tag);
                    if (movedPawn != null)
                    {
                        movedPawn.SetPositionAtNoZoom(locationOfMoved.Obj);
                        MainGrid.DragDropAdding(movedPawn, null, receivedFromOutside: true);
                    }
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs(String.Format("Moved pawn [{0}] to ({1};{2})", locationOfMoved.Tag, locationOfMoved.Obj.X, locationOfMoved.Obj.Y)));
#endif
                    return DataRes.Empty;
                }
            }
        }

        [ResponseMethods(Connections.PAWN_THAT_CHANGED)]
        private DataRes PawnThatChanged(byte[] buffer)
        {
            if (buffer == null)
            {
                // Telling which pawn has been modified
                var pawnUniqueID = LastModifiedPawn.UniqueID;
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn modified - Which one: {0}", pawnUniqueID)));
#endif
                return new DataRes(GetBytesFromString(pawnUniqueID));
            }
            else
            {
                // Which pawn will be modified
                var pawnUniqueID = GetStringFromByteArray(buffer);
                LastModifiedPawn = MainGrid.GetByUniqueID(pawnUniqueID);
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn modified: {0} [IN]", LastModifiedPawn.UniqueID)));
#endif
                return DataRes.Empty;
            }
        }

        [ResponseMethods(Connections.PAWN_VALUE_TYPE_CHANGED)]
        private DataRes PawnValueTypeChanged(byte[] buffer)
        {
            if (buffer == null)
            {
                // Telling which value has been modified
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn modified - Which value: {0}", LastChangedValueType)));
#endif
                return new DataRes(new byte[] { (byte)LastChangedValueType });
            }
            else
            {
                // Which value type will be modified
                LastChangedValueType = (GridPawnValueChangedEventArgs.ChangeableItems)buffer[0];
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn has been modified in {0}", LastChangedValueType)));
#endif
                return DataRes.Empty;
            }
        }

        [ResponseMethods(Connections.PAWN_VALUE_CHANGED)]
        private DataRes PawnValueChanged(byte[] buffer)
        {
            if (buffer == null)
            {
                // Telling the content of the modification
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn modified - Value [{0} Bytes]", LastChangedValue.Length)));
#endif
                return new DataRes(LastChangedValue);
            }
            else
            {
                // Receiveing the content of the modification
                LastChangedValue = buffer;
                var deserializedResult = GridPawnValueChangedEventArgs.Deserialize(LastChangedValueType, LastChangedValue, GetStringFromByteArray);
                GridPawnValueChanger.GetCurrent((CharacterPawn)LastModifiedPawn).SetValueByType(LastChangedValueType, deserializedResult);

#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn modified - Value [{0} Bytes]", LastChangedValue.Length)));
#endif
                return DataRes.Empty;
            }
        }

        [ResponseMethods(Connections.PAWN_ROTATION_90_DEGREES)]
        private DataRes PawnRotated90Degrees(byte[] buffer)
        {
            if (buffer == null)
            {
                // Telling which pawn has been rotated
                var pawnUniqueID = LastModifiedPawn.UniqueID;
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn rotates - Value [{0} ID]", LastModifiedPawn)));
#endif
                return new DataRes(GetBytesFromString(pawnUniqueID));
            }
            else
            {
                // Which pawn will be rotated
                var pawnUniqueID = GetStringFromByteArray(buffer);
                LastModifiedPawn = MainGrid.GetByUniqueID(pawnUniqueID);
                LastModifiedPawn.PerformRotate90Degrees(hasToBePropagated: false);
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn {0} rotated [ID {0}]", LastModifiedPawn.Name, LastModifiedPawn.UniqueID)));
#endif
                return DataRes.Empty;
            }
        }

        [ResponseMethods(Connections.PAWN_BORDER_COLOR)]
        private DataRes PawnBorderColor(byte[] buffer)
        {
            if (buffer == null)
            {
                // Telling which pawn has been modified
                var color = ((CharacterPawn)LastModifiedPawn).CirclePenColor;
                var strColor = ColorTranslator.ToHtml(color);
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn border color modified: {0}", color)));
#endif
                return new DataRes(GetBytesFromString(strColor));
            }
            else
            {
                // Which pawn will be modified
                var strColor = GetStringFromByteArray(buffer);
                ((CharacterPawn)LastModifiedPawn).CirclePenColor = ColorTranslator.FromHtml(strColor);
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn {0} color modified", LastModifiedPawn.UniqueID)));
#endif
                return DataRes.Empty;
            }
        }

        [ResponseMethods(Connections.PAWN_REMOVED)]
        private DataRes PawnRemoved(byte[] buffer)
        {
            if (buffer == null)
            {
                // Telling which pawn has been modified
                var pawnUniqueID = LastModifiedPawn.UniqueID;
#if DEBUG
                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn modified - Which one: {0}", pawnUniqueID)));
#endif
                return new DataRes(GetBytesFromString(pawnUniqueID));
            }
            else
            {
                // Which pawn will be modified
                var pawnUniqueID = GetStringFromByteArray(buffer);
                LastModifiedPawn = MainGrid.GetByUniqueID(pawnUniqueID);
                if (LastModifiedPawn != null)
                {
                    var removed = mainGrid.Remove(LastModifiedPawn, propagate: false);
                    MainPawnManager.LoadPawn(removed);
#if DEBUG
                    OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn modified: {0} [IN]", LastModifiedPawn.UniqueID)));
#endif
                }
                return DataRes.Empty;
            }
        }

        [ResponseMethods(Connections.PAWN_DELETED)]
        private DataRes PawnDeleted(byte[] buffer)
        {
            throw new NotImplementedException();
//            if (buffer == null)
//            {
//                // Telling which pawn has been modified
//                var pawnUniqueID = LastModifiedPawn.UniqueID;
//#if DEBUG
//                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn modified - Which one: {0}", pawnUniqueID)));
//#endif
//                return new DataRes(GetBytesFromString(pawnUniqueID));
//            }
//            else
//            {
//                // Which pawn will be modified
//                var pawnUniqueID = GetStringFromByteArray(buffer);
//                LastModifiedPawn = MainGrid.GetByUniqueID(pawnUniqueID);
//#if DEBUG
//                OnVerboseDebugging(new VerboseDebugArgs(String.Format("Pawn modified: {0} [IN]", LastModifiedPawn.UniqueID)));
//#endif
//                return DataRes.Empty;
//            }
        }

        [ResponseMethods(Connections.MESSAGE)]
        private DataRes Message(byte[] buffer)
        {
            var msg = GetStringFromByteArray(buffer);
#if DEBUG
            OnVerboseDebugging(new VerboseDebugArgs(String.Format("[MESSAGE] {0}", msg)));
#endif
            return DataRes.Empty;
        }

        [ResponseMethods(Connections.WARNING)]
        private DataRes Warning(byte[] buffer)
        {
            var msg = GetStringFromByteArray(buffer);
#if DEBUG
            OnVerboseDebugging(new VerboseDebugArgs(String.Format("[WARNING] {0}", msg)));
#endif
            return DataRes.Empty;
        }

        [ResponseMethods(Connections.ERROR)]
        private DataRes Error(byte[] buffer)
        {
            var msg = GetStringFromByteArray(buffer);
#if DEBUG
            OnVerboseDebugging(new VerboseDebugArgs(String.Format("[ERROR] {0}", msg)));
#else
            throw new Exception(msg);
#endif
            return DataRes.Empty;
        }

        #endregion

        #region VERBOSE_DEBUGGING
#if DEBUG
        public class VerboseDebugArgs : EventArgs
        {
            public string Message { get; private set; }
            public VerboseDebugArgs(string message)
            {
                Message = message;
            }

            public override string ToString()
            {
                return String.Format("[DEBUG] - {0}", Message);
            }
        }

        protected void OnVerboseDebugging(VerboseDebugArgs verboseArgs)
        {
            var tmp = VerboseDebugging;
            if(tmp != null)
            {
                tmp(this, verboseArgs);
            }
        }
#endif
        #endregion
    }
}
