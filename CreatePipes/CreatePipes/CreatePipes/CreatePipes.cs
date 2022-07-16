using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.UI.Selection;

using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;

namespace CreatePipes
{
    [Transaction(TransactionMode.Manual)]

    public class CreatePipes : IExternalCommand
    {
        private Autodesk.Revit.DB.Document RevitDoc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIDocument uidoc = commandData.Application.ActiveUIDocument;
                 RevitDoc = uidoc.Document;
            using (Transaction tran = new Transaction(RevitDoc, "Quick Link"))
                {
                    tran.Start();

                    #region filters
                    #region ActiveLEVEL
                    View active = RevitDoc.ActiveView;

                    ElementId levelId = null;

                    Parameter level = active.LookupParameter("Associated Level");

                    FilteredElementCollector lvlCollector = new FilteredElementCollector(RevitDoc);
                    ICollection<Element> lvlCollection = lvlCollector.OfClass(typeof(Level)).ToElements();

                    foreach (Element l in lvlCollection)
                    {
                        Level lvl = l as Level;
                        if (lvl.Name == level.AsString())
                        {
                            levelId = lvl.Id;
                            //TaskDialog.Show("test", lvl.Name + "\n"  + lvl.Id.ToString());
                        }
                    }
                    #endregion


                    // get all curves
                    FilteredElementCollector collector = new FilteredElementCollector(RevitDoc)
                    .WherePasses(new ElementClassFilter(typeof(CurveElement)));

                    #region PipeReqirments
                    MEPSystemType mepSystemType = new FilteredElementCollector(RevitDoc)
                    .OfClass(typeof(MEPSystemType))
                    .Cast<MEPSystemType>()
                    .FirstOrDefault(sysType => sysType.SystemClassification == MEPSystemClassification.DomesticColdWater);

                    PipeType pipeType = new FilteredElementCollector(RevitDoc)
                    .OfClass(typeof(PipeType))
                    .Cast<PipeType>()
                    .FirstOrDefault();
                    #endregion

                    #endregion

                    foreach (CurveElement x in collector)
                    {
                        string o = x.LineStyle.Name;
                        #region Draft

                        //Element lineid = new FilteredElementCollector(RevitDoc).OfCategory(BuiltInCategory.OST_Lines).
                        //WhereElementIsNotElementType().FirstOrDefault(e => e.Name == o);

                        //Category c = RevitDoc.Settings.Categories.get_Item(
                        //  BuiltInCategory.OST_Lines);

                        //CategoryNameMap subcats = c.SubCategories;

                        //foreach (Category lineStyle in subcats)
                        //{
                        //    TaskDialog.Show("Line style", string.Format(
                        //      "Linestyle {0} id {1}", lineStyle.Name,
                        //      lineStyle.Id.ToString()));
                        //}


                        //x.get_Parameter(BuiltInParameter.Line_s); 
                        #endregion
                        // draw pipes from lines in layers that contain the word pipe in them
                        Line xline = x.GeometryCurve as Line;
                        if (o.ToUpper().Contains("PIPE"))
                        {
                            XYZ startPoint = xline.GetEndPoint(0);
                            XYZ endPoint = xline.GetEndPoint(1);
                            Pipe pipe = Pipe.Create(RevitDoc, mepSystemType.Id, pipeType.Id, levelId, startPoint, endPoint);

                        }
                        #region draft3
                        //if (!(o.Contains("<")) && !(o.ToUpper().Contains("PIPE")) )
                        //{
                        //    try
                        //    {
                        //        RevitDoc.Delete(lineid.Id);
                        //    }
                        //    catch (NullReferenceException)
                        //    {
                        //        break;
                        //    }


                        //} 
                        #endregion


                    }


                    #region draft2
                    //FilteredElementCollector detailLineCollection =
                    //new FilteredElementCollector(RevitDoc).OfClass(typeof(CurveElement))
                    //.OfCategory(BuiltInCategory.OST_Lines);

                    //foreach (DetailLine x in detailLineCollection)
                    //{
                    //    Line xline = x.GeometryCurve as Line;
                    //    double xlineDirectionX = xline.Direction.X;
                    //    double xlineDirectionY = xline.Direction.Y;

                    //TaskDialog.Show("answer",$"{xline},+{xlineDirectionX.ToString()}");
                    //}
                    //#region MyRegion
                    //#endregion 
                    #endregion


                    tran.Commit();

                }



                return Result.Succeeded;
        }
            catch
            {
                TaskDialog.Show("Error", "Something went wrong Contact abdul.khaled.Sultan@gmail.com");
                return Result.Failed;
            }


}
    }
}





