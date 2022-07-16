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
using System.Windows.Forms;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;

namespace GetCad
{
    [Transaction(TransactionMode.Manual)]

    public class Browse  : IExternalCommand
    {
        private Autodesk.Revit.DB.Document RevitDoc;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            RevitDoc = uidoc.Document;

            try
            {
                using (Transaction tran = new Transaction(RevitDoc, "Quick Link"))
                {
                    tran.Start();

                    // Autocad Import Options
                    DWGImportOptions opt = new DWGImportOptions();
                    opt.Placement = ImportPlacement.Origin;
                    opt.AutoCorrectAlmostVHLines = false;
                    opt.ThisViewOnly = false;
                    opt.Unit = ImportUnit.Default;

                    ElementId linkId = ElementId.InvalidElementId;


                    #region GetFilePath
                    OpenFileDialog choofdlog = new OpenFileDialog();
                    choofdlog.Filter = "All Files (*.*)|*.*";
                    choofdlog.FilterIndex = 1;
                    choofdlog.Multiselect = false;

                    if (choofdlog.ShowDialog() == DialogResult.OK)
                    {
                        string sFileName = choofdlog.FileName;

                        RevitDoc.Import(sFileName, opt, RevitDoc.ActiveView, out linkId);
                        // make an instance for the imported 
                        ImportInstance cadInst = RevitDoc.GetElement(linkId) as ImportInstance;
                        CADLinkType cadLinkType = RevitDoc.GetElement(cadInst.GetTypeId()) as CADLinkType;
                        //RevitDoc.GetElement(cadLinkType.)

                    }

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


//tran.Start();
//#region MyRegion
//OpenFileDialog choofdlog = new OpenFileDialog();
//choofdlog.Filter = "All Files (*.*)|*.*";
//choofdlog.FilterIndex = 1;
//choofdlog.Multiselect = true;

//if (choofdlog.ShowDialog() == DialogResult.OK)
//{
//    string sFileName = choofdlog.FileName;
//    string[] arrAllFiles = choofdlog.FileNames; //used when Multiselect = true

//}



//#endregion
//ImportInstance cadInst = RevitDoc.GetElement(linkId) as ImportInstance;
//CADLinkType cadLinkType = RevitDoc.GetElement(cadInst.GetTypeId()) as CADLinkType;
//string path = @"E:\ITI\Drawing1.dwg";
//tran.Commit();