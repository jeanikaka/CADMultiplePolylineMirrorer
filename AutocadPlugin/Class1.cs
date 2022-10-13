using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;

namespace MyAutoCADDll
{
    public class Commands : IExtensionApplication
    {
        // функция инициализации (выполняется при загрузке плагина)
        public void Initialize()
        {
        }

        // функция, выполняемая при выгрузке плагина
        public void Terminate()
        {
        }

        // эта функция будет вызываться при выполнении в AutoCAD команды «TestCommand»
        [CommandMethod("PolylineMirror", CommandFlags.UsePickSet)]
        public void MyCommand()
        {

            // Get the current document and database
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            // Start a transaction 
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                PromptSelectionResult acSSPrompt;
                acSSPrompt = acDoc.Editor.SelectImplied();
                SelectionSet acSSet = null;
                if(acSSPrompt.Status == PromptStatus.OK)
                {
                    acSSet = acSSPrompt.Value;
                }
                else
                {
                    // Request for objects to be selected in the drawing area
                    acSSPrompt = acDoc.Editor.GetSelection();
                    // If the prompt status is OK, objects were selected
                    if (acSSPrompt.Status == PromptStatus.OK)
                    {
                        acSSet = acSSPrompt.Value;
                    }
                }
                Matrix3d curUCSMatrix = acDoc.Editor.CurrentUserCoordinateSystem;
                CoordinateSystem3d curUCS = curUCSMatrix.CoordinateSystem3d;
                if (acSSet != null)
                {
                    // Step through the objects in the selection set
                    foreach (SelectedObject acSSObj in acSSet)
                    {
                        // Check to make sure a valid SelectedObject object was returned
                        if (acSSObj != null)
                        {
                            // Open the selected object for write
                            Entity acEnt = acTrans.GetObject(acSSObj.ObjectId,
                                                             OpenMode.ForWrite) as Entity;
                            if (acEnt != null)
                            {
                                if (acEnt.ColorIndex == 1)
                                {
                                    var random = new Random(Guid.NewGuid().GetHashCode());
                                    int randomValue = random.Next(100);
                                    Polyline polyline = acEnt as Polyline;
                                    Point3d point1 = polyline.GetPoint3dAt(1);
                                    Point3d point2 = polyline.GetPoint3dAt(3);
                                    Point3d centralPoint = new Point3d((point1.X + point2.X) / 2, (point1.Y + point2.Y) / 2, 0);
                                    if (randomValue <= 50)
                                    {
                                        polyline.TransformBy(Matrix3d.Rotation(Math.PI, curUCS.Zaxis, centralPoint));
                                        // Change the object's color to Green
                                        acEnt.ColorIndex = 3;
                                    }

                                }
                            }
                        }
                    }
                }
                
                // Save the new object to the database
                acTrans.Commit();
                // Dispose of the transactio
            }
        }
    }
}
