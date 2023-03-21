using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class DialogDB_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Excel/DialogDB.xlsx";
	private static readonly string exportPath = "Assets/Excel/DialogDB.asset";
	private static readonly string[] sheetNames = { "Dialoge", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			Entity_Dialoge data = (Entity_Dialoge)AssetDatabase.LoadAssetAtPath (exportPath, typeof(Entity_Dialoge));
			if (data == null) {
				data = ScriptableObject.CreateInstance<Entity_Dialoge> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

					Entity_Dialoge.Sheet s = new Entity_Dialoge.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						Entity_Dialoge.Param p = new Entity_Dialoge.Param ();
						
					cell = row.GetCell(0); p.index = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.speakerUIindex = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.dialogue = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(4); p.characterPath = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(5); p.tweenType = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.nextindex = (int)(cell == null ? 0 : cell.NumericCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
