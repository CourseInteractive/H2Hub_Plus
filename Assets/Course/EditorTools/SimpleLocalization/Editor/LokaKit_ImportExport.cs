using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AC;
using System.Text;
using System.Text.RegularExpressions;

public class LokaKit_ImportExport : EditorWindow
{
    public LokaKit lokaKit;
    //private SpeechManager speechManager;

    public string[] columnNames = new string[6] { "German_Text", "German_Regie", "German_Comment", "English_Text", "English_Regie", "English_Comment" };
    public bool[] columnsToInclude;

    public string exportPath;

    public static bool importChangedTokens = false;

public static Vector2 version = new Vector2(1,8);

    [MenuItem("Tools/Course/Localization/Simple CSV Manager")]
    static void Open()
    {
        
        LokaKit_ImportExport window = (LokaKit_ImportExport)EditorWindow.GetWindow(typeof(LokaKit_ImportExport));

    }

    private void OnGUI()
    {
        exportPath = Application.dataPath + "/CSV/";
        lokaKit = (LokaKit)EditorGUILayout.ObjectField("Loka Kit", lokaKit, typeof(LokaKit), false);
        if (GUILayout.Button("Assign SpeechLine to Sequence"))
        {
            AssignLineToSequence();
            // SetLastVersion();
        }

        if(lokaKit != null)
        {
            if (GUILayout.Button("Export All"))
            {
                ExportAllCategories();
            }

            if (GUILayout.Button("Export All No Commands"))
            {
                ExportAllCategories(true);
            }

            foreach (LokaKitExportOption option in lokaKit.exportOptions)
            {
                if (GUILayout.Button("Export " + option.name))
                {
                    ExportOption(option);
                }
            }
        }
      
        /*if (GUILayout.Button("Export Names (ConvOptions, Hotspots, InvItems)"))
        {
            ExportNames();
        }
        if (GUILayout.Button("Export Menu Elements"))
        {
            ExportUI();
        }

        if (GUILayout.Button("Export Journal Entries"))
        {
            ExportUI_Content();
        }*/

        if (GUILayout.Button("Remove '\"' Quotes-Problem"))
        {
            RemoveQuotesProblem();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("IMPORT");
            EditorGUILayout.Space();
        if (lokaKit == null)
        {
            EditorGUILayout.LabelField("Wähle zunächst ein LokaKit aus");
            return;
        }
        file = (TextAsset)EditorGUILayout.ObjectField("Datei:", file, typeof(TextAsset), false);


        if (file != null)
        {
            fileToImportContent = CheckContentTypeOfFileToImport();
            columnCount = GetLengthLongestRowAndPrint();
            EditorGUILayout.LabelField("File has up to " + (columnCount + 1) + " columns.");
            switch (fileToImportContent)
            {
                case FileContent.Speech:
                    EditorGUILayout.LabelField("File is Speech");
                    break;
                case FileContent.Simple:

                    EditorGUILayout.LabelField("File is Simple");
                    EditorGUILayout.LabelField(longestRowText);

                    break;
                case FileContent.Broken:
                    EditorGUILayout.LabelField("File is Broken");
                    EditorGUILayout.LabelField(longestRowText);
                    break;
            }
            if (fileToImportContent == FileContent.Broken)
            {
                /* if(GUILayout.Button("Find Longest Row And Print"))
                 {
                     FindLongestRowAndPrint();
                 }
                 */
                return;
            }

          //  bool importNewEntries = false;
            EditorGUILayout.LabelField("File has " + (rowCount) + " entries.");
            if (rowCount > lokaKit.entries.Count)
            {
                EditorGUILayout.LabelField("File has " + (rowCount - lokaKit.entries.Count) + " entries more than loka kit");
                //importNewEntries = EditorGUILayout.Toggle("Import new Entries: ", importNewEntries);
            }
            importChangedTokens = EditorGUILayout.ToggleLeft("Import changed tokens (Higher Risk)", importChangedTokens);
            if (fileToImportContent == FileContent.Simple)
            {
                if (GUILayout.Button("Load Data Simple Text CSV and print Changes"))
                {
                    ImportSimpleTextCSV();
                }
                if (updatedLines != null && updatedLines.Count > 0 && GUILayout.Button("Import Changes"))
                {
                    UpdateLinesInSpeechManagerAndSave(updatedLines.ToArray());
                    updatedLines = null;
                }
                if (newLines != null && newLines.Count > 0 && GUILayout.Button("Import New Entries"))
                {
                    ImportNewLinesInSpeechManagerAndSave(newLines.ToArray());
                    newLines = null;
                }
                return;
            }
            if (columnsToInclude == null)
            {
                columnsToInclude = new bool[columnNames.Length];
            }
            for (int i = 0; i < columnsToInclude.Length; i++)
            {
                columnsToInclude[i] = EditorGUILayout.Toggle(columnNames[i], columnsToInclude[i]);
            }
            

            if (GUILayout.Button("Import CSV and print Changes"))
            {
                ImportSpeechCSV();
            }
        }
        else
        {
            columnCount = 0;
            lastFile = null;
            fileToImportContent = FileContent.None;
        }
        if (updatedLines != null && updatedLines.Count > 0 && GUILayout.Button("Import Changes"))
        {
            UpdateLinesInSpeechManagerAndSave(updatedLines.ToArray());
            updatedLines = null;
        }
        if (newLines != null && newLines.Count > 0 && GUILayout.Button("Import New Entries"))
        {
            ImportNewLinesInSpeechManagerAndSave(updatedLines.ToArray());
            newLines = null;
        }
        if (GUILayout.Button("Set SpeechManager Dirty and Save"))
        {

        }
    }
    string longestRowText;
    int GetLengthLongestRowAndPrint()
    {
        string path = AssetDatabase.GetAssetPath(file);
        Encoding ascii = Encoding.UTF8;

        string csvText = ascii.GetString(file.bytes);
        string[,] csvOutput = CSVReader.SplitCsvGrid(csvText);
        int length = csvOutput.GetLength(1);
        int width = csvOutput.GetLength(0);
        string longestText = "";
        int lengthOfLongest = -1;
        for (int i = 0; i < length; i++)
        {
            int lastFilledColumn = 0;
            string rowData = "";
            for (int j = 0; j < width; j++)
            {
                if (csvOutput[j, i] == null)
                    break;
                if (csvOutput[j, i].Trim() != "")
                    lastFilledColumn = j;
                rowData += " ~ " + csvOutput[j, i];
            }
            if (lastFilledColumn > lengthOfLongest)
            {
                lengthOfLongest = lastFilledColumn;
                longestText = rowData;
            }
            // Debug.Log(rowData);
        }
        longestRowText = "Length of longestRow: " + lengthOfLongest + "  - Text: " + longestText;
        return lengthOfLongest;
    }

    List<LokaKitEntry> updatedLines;
    List<LokaKitEntry> newLines;
    TextAsset lastFile;
    public enum FileContent { None, Broken, Speech, Simple }
    FileContent fileToImportContent;
    int columnCount;
    int rowCount;

    FileContent CheckContentTypeOfFileToImport()
    {
        if (lastFile != null && file == lastFile)
            return fileToImportContent;
        lastFile = file;
        string path = AssetDatabase.GetAssetPath(file);
        Encoding ascii = Encoding.UTF8;

        string csvText = ascii.GetString(file.bytes);
        string[,] csvOutput = CSVReader.SplitCsvGrid(csvText);
        columnCount = csvOutput.GetLength(0);
        rowCount = csvOutput.GetLength(1)-1;
        if (columnCount > 16)
            return FileContent.Speech;
        else if (columnCount > 8)
            return FileContent.Broken;
        else
            return FileContent.Simple;
    }

    void AssignLineToSequence()
    {
    /*    AC_SpeechSequenceCollector.ClearSceneAndSequenceInfosFromSpeechLines();
        List<int> usedSpeechSequenceIndices = AC_SpeechSequenceCollector.GetUsedSpeechSequenceIndices();
        AC_SpeechSequenceCollector.AssignSpeechLinesToUsedSequences(usedSpeechSequenceIndices);
        int counter = 0;
        foreach (SpeechLine line in KickStarter.speechManager.lines)
        {
            if (line.textType == AC_TextType.Speech && line.orderPrefix.Trim() == "")
            {
                counter++;
                string d = line.text + " not in any SpeechSequence";
                if (line.translationText.Count > 0)
                    d += " (" + line.translationText[0] + ")";
                Debug.Log(d);
            }

        }
        Debug.Log("SpeechLines not in SpeechSequence: " + counter);*/
    }

    TextAsset file;

    void ExportAllCategories(bool noCommands = false)
    {
        List<int> types = new List<int>();
        for(int i = 0; i< lokaKit.types.Count; i++)
        {
            types.Add(i);
        }
        ExportFile(types, "LokaKit_All_" + Application.version + ".csv", noCommands);
    }


    void ExportOption(LokaKitExportOption option)
    {
        List<int> types = option.categoryIndices;
        ExportFile(types, "LokaKit_" + option.name + "_" + Application.version + ".csv");
    }

    void ExportSpeech()
    {
        List<int> types = new List<int>();
        types.Add(0);
        ExportFile(types, "Speech_" + Application.version + ".csv");
        /*   AC_SpeechSequenceCollector.ClearSceneAndSequenceInfosFromSpeechLines();
           List<int> usedSpeechSequenceIndices = AC_SpeechSequenceCollector.GetUsedSpeechSequenceIndices();
           AC_SpeechSequenceCollector.AssignSpeechLinesToUsedSequences(usedSpeechSequenceIndices);*/
        /*   List<int> usedSpeechSequenceIndices = new List<int>();
           for(int i = 0; i < lokaKit.entries.Count; i++)
           {
               usedSpeechSequenceIndices.Add(i);
           }
           List<string[]> data = AC_SpeechSequenceCollector.GetSpeechSequenceLokaKit(usedSpeechSequenceIndices);
           CreateCSVSpeechFile(data);*/
    }

    void CheckAndCreateDirectory()
    {

    }

    void ExportNames()
    {
        List<int> types = new List<int>();
        // types.Add(AC_TextType.DialogueOption);
        // types.Add(AC_TextType.Hotspot);
        // types.Add(AC_TextType.InventoryItem);
        types.Add(0);
        types.Add(1); 
        types.Add(2);
        ExportFile(types, "Names_" + Application.version + ".csv");
    }

    void ExportUI()
    {
        List<int> types = new List<int>();
        types.Add(3);
        ExportFile(types, "UI-Labels_" + Application.version + ".csv");
    }

    void ExportUI_Content()
    {
        List<int> types = new List<int>();
        types.Add(4);
        ExportFile(types, "Content_" + Application.version + ".csv");

    }

    private string RemoveParen(string text)
    {
        text = text.Replace('"', '\'');
        return text;
    }

    private string ReplaceLineBreaks(string text)
    {
        if (text.Length == 0) return " ";
        text = text.Replace("\r\n", "[break]");
        text = text.Replace("\n", "[break]");
        text = text.Replace("\r", "[break]");
        return text;
    }

    private string RemoveLineBreakCodes(string text)
    {
        if (text.Length == 0) return " ";
        if(text.Contains("§") || text.Contains("\n"))
        {
            text = "\"" + text + "\"";
        }
       
        text = text.Replace("§", "\n");
        text = text.Replace("\n\n", "\n");
        return text;
    }

    public string StripRichTextTags(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        Regex RichTextTagRegex =
        new Regex(@"<\/?[a-zA-Z][a-zA-Z0-9\-]*(=[^>]*)?\s*\/?>", RegexOptions.Compiled);
        return RichTextTagRegex.Replace(input, string.Empty);
    }

    void LoadSpeechManager()
    {
        //speechManager = AC.KickStarter.speechManager;
    }

    void ExportFile(List<int> types, string fileName, bool noCommands = false)
    {
        if (!System.IO.Directory.Exists(exportPath))
            System.IO.Directory.CreateDirectory(exportPath);
        string path = exportPath + fileName;
        if (noCommands)
            path = exportPath + "NoComm_" + fileName;
        LoadSpeechManager();
        List<LokaKitEntry> exportLines = new List<LokaKitEntry>();
        foreach (LokaKitEntry line in lokaKit.entries)
        {
            if (line == null || line.lineID < 0 || line.token.Trim() == "")
                continue;
            if (types.Contains(line.textType))
                exportLines.Add(new LokaKitEntry(line));
        }
        List<string[]> output = new List<string[]>();
        foreach (LokaKitEntry line in exportLines)
        {
            List<string> row = new List<string>();
            row.Add(line.lineID.ToString());
            row.Add(line.textType.ToString());
            row.Add("__");
            row.Add(line.token);
            for (int i = 0; i < lokaKit.languageCount; i++)
            {
                if(noCommands)
                {
                    string textToAdd = line.translationText[i];
                    textToAdd = RemoveLineBreakCodes(RemoveParen(textToAdd));
                    textToAdd = StripRichTextTags(textToAdd);
                    row.Add(textToAdd);
                }
                else
                {
                    row.Add(ReplaceLineBreaks(RemoveParen(line.translationText[i])));
                }
              
            }
            // Tags
            //SpeechLineExtension extension = KickStarter.speechManager.extension.GetSpeechLineExtension(line.lineID);
            row.Add(line.tagMask.ToString());
            //row.Add(extension.GetTagLine(KickStarter.speechManager.extension.tagList));
            // Tags End
            output.Add(row.ToArray());
        }
        int length = output.Count;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        for (int j = 0; j < length; j++)
        {
            sb.AppendLine(string.Join(CSVReader.csvDelimiter, output[j]));
        }

        if (Serializer.SaveFile(path, sb.ToString()))
        {
            int numLines = exportLines.Count;
            string message = numLines.ToString() + " line" + ((numLines != 1) ? "s" : string.Empty) + " exported.";
            message += " (";
            string ts = "";
            foreach (int t in types)
            {
                ts += "," + t.ToString();
            }
            message += ts.Substring(1) + ")";
            Debug.Log(message + " Path: " + path);

            AssetDatabase.Refresh();
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(ToAssetPath(path));
            Selection.activeObject = asset;
            EditorUtility.FocusProjectWindow();
            EditorGUIUtility.PingObject(asset);
        }


    }

    public static string ToAssetPath(string fullPath)
    {
        fullPath = fullPath.Replace("\\", "/");
        int i = fullPath.IndexOf("/Assets/");
        return i >= 0 ? fullPath.Substring(i + 1) : null;
    }

    void CreateCSVSpeechFile(List<string[]> output)
    {
        string path = Application.dataPath + "/CSV/" + "SpeechSequences_" + Application.version + ".csv";
        int length = output.Count;
        int counter = 0;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        for (int j = 0; j < length; j++)
        {
            // Debug.Log(output[j].Length + "|" + string.Join("-", output[j]));
            sb.AppendLine(string.Join(CSVReader.csvDelimiter, output[j]));
            counter++;
        }

        if (Serializer.SaveFile(path, sb.ToString()))
        {
           // ACDebug.Log(counter + " line" + ((counter != 1) ? "s" : string.Empty) + " exported.");

        }
    }

    void ImportSpeechCSV()
    {
       // VersionnumberEditor.IncreaseLastDigit();
        LoadSpeechManager();
        string path = AssetDatabase.GetAssetPath(file);
        Encoding ascii = Encoding.UTF8;

        string csvText = ascii.GetString(file.bytes);
        string[,] csvOutput = CSVReader.SplitCsvGrid(csvText);
        List<SpeechLineImport> imports = new List<SpeechLineImport>();
        int length = csvOutput.GetLength(1);
        for (int i = 1; i < length; i++)
        {

            if (csvOutput[0, i] == null || csvOutput[0, i].Trim() == "")
                continue;
            int x = -1;
            if (csvOutput[0, i].Trim().ToLower() == "x" || csvOutput[0, i].Trim().ToLower() == "new")
                x = -2;
            else if (!int.TryParse(csvOutput[0, i], out x))
                continue;
            if (x == -2)
                imports.Add(new SpeechLineImport(GetRowFromTable(i, csvOutput), fileToImportContent, true));
            else
                imports.Add(new SpeechLineImport(GetRowFromTable(i, csvOutput), fileToImportContent));

        }
        updatedLines = new List<LokaKitEntry>();
        newLines = new List<LokaKitEntry>();
        Debug.Log("Start Update lines: " + imports.Count);
        foreach (SpeechLineImport import in imports)
        {
            if (import == null)
                continue;
            if(import.newEntry)
            {
                LokaKitEntry newLine = import.GetAsNewEntry();
                newLines.Add(newLine);
                continue;
            }
            LokaKitEntry currentLine = lokaKit.GetLine(import.lineID);
            if (currentLine == null)
                continue;
            currentLine = new LokaKitEntry(currentLine);
            bool changed = false;
            LokaKitEntry updatedLine = import.UpdateSpeechLine(currentLine, out changed, columnsToInclude);
            if (updatedLine == null)
            {
                Debug.Log("Skip line " + currentLine.lineID);
                continue;
            }
            updatedLine = new LokaKitEntry(updatedLine);
            //SpeechManagerExtension ext = speechManager.extension;
            //SpeechLineExtension currentExtension = ext.GetSpeechLineExtension(updatedLine.lineID);
            //currentExtension = import.UpdateSpeechLineExt(currentExtension, columnsToInclude);
            //speechManager.extension.SetSpeechLineExtension(currentExtension);
            if (updatedLine == null)
                continue;
            if (changed)
            {
                Debug.Log("Geändert: " + updatedLine.token + " (" + updatedLine.translationText[0] + ")");
                updatedLines.Add(updatedLine);
            }

        }
        Debug.Log("Before Finish");

    }

    void ImportSimpleTextCSV(bool importNewEntries = false)
    {
        //VersionnumberEditor.IncreaseLastDigit();
        LoadSpeechManager();
        string path = AssetDatabase.GetAssetPath(file);
        Encoding ascii = Encoding.UTF8;

        string csvText = ascii.GetString(file.bytes);
        string[,] csvOutput = CSVReader.SplitCsvGrid(csvText);
        List<SpeechLineImport> imports = new List<SpeechLineImport>();
        int length = csvOutput.GetLength(1);
        for (int i = 0; i < length; i++)
        {

            if (csvOutput[0, i] == null || csvOutput[0, i].Trim() == "")
            {
                continue;
            }
              
            int x = -1;
            if (csvOutput[0, i].Trim().ToLower() == "x" || csvOutput[0, i].Trim().ToLower() == "new")
                x = -2;
            else if (!int.TryParse(csvOutput[0, i], out x))
                continue;
            if(x == -2)
                imports.Add(new SpeechLineImport(GetRowFromTable(i, csvOutput), fileToImportContent, true));
            else
                imports.Add(new SpeechLineImport(GetRowFromTable(i, csvOutput), fileToImportContent));
        }
        updatedLines = new List<LokaKitEntry>();
        newLines = new List<LokaKitEntry>();
        Debug.Log("Start Update lines: " + imports.Count);
        foreach (SpeechLineImport import in imports)
        {
            if (import == null)
                continue;
            if(import.newEntry)
            {
                LokaKitEntry newLine = import.GetAsNewEntry();
                newLines.Add(newLine);
                Debug.Log("New Entry " + newLine.token);
                continue;
            }

            LokaKitEntry currentLine = lokaKit.GetLine(import.lineID);
            if (currentLine == null)
                continue;
            currentLine = new LokaKitEntry(currentLine);
            bool changed = false;
            LokaKitEntry updatedLine = import.UpdateSpeechLine_Simple(currentLine, out changed);
            if (updatedLine == null)
            {
                Debug.Log("Skip line " + currentLine.lineID);
                continue;
            }
            updatedLine = new LokaKitEntry(updatedLine);
            //SpeechManagerExtension ext = speechManager.extension;
            //SpeechLineExtension currentExtension = ext.GetSpeechLineExtension(updatedLine.lineID);
            //currentExtension = import.UpdateSpeechLineExt(currentExtension, columnsToInclude);
            //speechManager.extension.SetSpeechLineExtension(currentExtension);
            if (updatedLine == null)
                continue;
            if (changed)
            {
                Debug.Log("Geändert: " + updatedLine.token + " (" + updatedLine.translationText[0] + ")");
                updatedLines.Add(updatedLine);
            }

        }
        Debug.Log("Before Finish");

    }

    void UpdateLinesInSpeechManagerAndSave(LokaKitEntry[] lines)
    {
        for (int i = 0; i < lokaKit.entries.Count; i++)
        {
            for (int a = 0; a < lines.Length; a++)
            {
                if (lokaKit.entries[i].lineID == lines[a].lineID)
                {
                    Debug.Log(i + " found");
                    lokaKit.entries[i] = lines[a];
                }

            }
        }
        Debug.Log("Save");
        //KickStarter.speechManager = speechManager;
        UnityEditor.EditorUtility.SetDirty(lokaKit);
       // UnityEditor.EditorUtility.SetDirty(KickStarter.speechManager.extension);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void ImportNewLinesInSpeechManagerAndSave(LokaKitEntry[] lines)
    {

        

        for (int i = 0; i < lines.Length; i++)
        {
            lokaKit.AddNewEntry(lines[i]);
           /* for (int a = 0; a < lines.Length; a++)
            {
                if (lokaKit.entries[i].lineID == lines[a].lineID)
                {
                    Debug.Log(i + " found");
                    lokaKit.entries[i] = lines[a];
                }

            }*/
        }
        Debug.Log("Save");
        //KickStarter.speechManager = speechManager;
        UnityEditor.EditorUtility.SetDirty(lokaKit);
        // UnityEditor.EditorUtility.SetDirty(KickStarter.speechManager.extension);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    void SetLastVersion()
    {
        for (int i = 0; i < lokaKit.entries.Count; i++)
        {
            // Debug.Log(i + " found");
            // speechManager.lines[i] = lines[a];
            //  speechManager.lines[i].lastChangedInVersion = "0.9.5.63";

        }
        Debug.Log("Save");
        //KickStarter.speechManager = speechManager;
        UnityEditor.EditorUtility.SetDirty(lokaKit);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    string[] GetRowFromTable(int rowIndex, string[,] csvOutput)
    {
        List<string> row = new List<string>();
        int width = csvOutput.GetLength(0);
        //Debug.Log(width);
        for (int i = 0; i < width; i++)
        {
            // Debug.Log(csvOutput[i, rowIndex]);
            row.Add(csvOutput[i, rowIndex]);
        }
        return row.ToArray();
    }

    public string RemoveDoubleQuotesFromText(string text)
    {
        if (text.Length == 0)
            return text;
        string con = text.Replace("[break]", "\n");
        if (con.Contains("\"\"") && con[0] == '"')
        {
            con = con.Replace("\"\"", "\"");
            con = con.Substring(1);
            con = con.Substring(0, con.Length - 1);
        }
        return con;
    }


    public class SpeechLineImport
    {
        public int lineID;
        public string token;
        public string[] translations;
        public string[] directions;
        public string[] comments;
        public bool newEntry;
        public int textType;

        public SpeechLineImport(string[] csvRow, FileContent contentType, bool _newEntry = false)
        {
            newEntry = _newEntry;
            if (contentType == FileContent.Speech)
            {
                SpeechLineImport_Speech(csvRow);
            }
            else if (contentType == FileContent.Simple)
            {
                SpeechLineImport_Simple(csvRow);
            }
        }


        public void SpeechLineImport_Speech(string[] csvRow)
        {
            Debug.Log(csvRow[0]);
            lineID = int.Parse(csvRow[0].Trim());
            token = csvRow[7];
            List<string> translationList = new List<string>();
            List<string> directionsList = new List<string>();
            List<string> commentsList = new List<string>();
            translationList.Add(csvRow[8]);
            translationList.Add(csvRow[11]);
            directionsList.Add(csvRow[9]);
            directionsList.Add(csvRow[12]);
            commentsList.Add(csvRow[10]);
            commentsList.Add(csvRow[13]);
            if (!int.TryParse(csvRow[1].Trim(), out textType))
            {
                textType = -1;
            }

            translations = translationList.ToArray();
            directions = directionsList.ToArray();
            comments = commentsList.ToArray();
        }

        public void SpeechLineImport_Simple(string[] csvRow)
        {
            Debug.Log(csvRow[0]);
            if(int.TryParse(csvRow[0].Trim(), out lineID))
            {

            }
            token = csvRow[3];
            List<string> translationList = new List<string>();
            //List<string> directionsList = new List<string>();
            //List<string> commentsList = new List<string>();
            if (!int.TryParse(csvRow[1].Trim(), out textType))
            {
                textType = -1;
            }
            translationList.Add(csvRow[4]);
            translationList.Add(csvRow[5]);
            /*  directionsList.Add(csvRow[9]);
              directionsList.Add(csvRow[12]);
              commentsList.Add(csvRow[10]);
              commentsList.Add(csvRow[13]);*/

            translations = translationList.ToArray();
            // directions = directionsList.ToArray();
            // comments = commentsList.ToArray();
        }

        public LokaKitEntry UpdateSpeechLine(LokaKitEntry line, out bool changed, bool[] columnsToChange)
        {
            changed = false;
            if (line.lineID != lineID)
                return null;
            if (token.Trim() != line.token.Trim())
            {
                if (importChangedTokens)
                {
                    Debug.Log("(" + line.lineID + ")" + " Changed TOKEN -> " + token.Trim());
                    changed = true;
                    line.token = token.Trim();
                }
                else
                    return null;

            }
            //Debug.Log(line.translationText.Count + " " + translations.Length);
            string german = AdjustText(translations[0]);
            string english = AdjustText(translations[1]);

            if (line.translationText[0].Trim() != german.Trim() && columnsToChange[0])
            {
                Debug.Log("(" + line.lineID + ")" + " Changed DEUTSCH - TEXT");
                changed = true;
                line.lastChangedInVersion = Application.version;
                line.translationText[0] = german;
            }
            if (line.translationText[1].Trim() != english.Trim() && columnsToChange[3])
            {
                Debug.Log("(" + line.lineID + ")" + " Changed ENGLISH - TEXT");
                changed = true;
                line.lastChangedInVersion = Application.version;
                line.translationText[1] = english;
            }
            Debug.Log("Type: " + textType);
            if (textType > -1)
            {
                if (textType != line.textType)
                {
                    changed = true;
                    line.textType = textType;
                }
            }

            /*
                        for (int i = 0; i < translations.Length; i++)
                        {
                            string con = translations[i].Replace("[break]", "\n");
                            if (con[con.Length - 1] == '\n')
                                con = con.Substring(0, con.Length - 1);
                            if (line.translationText[i].Trim() != con.Trim())
                            {
                                changed = true;
                                line.lastChangedInVersion = Application.version;
                            }

                            line.translationText[i] = con;

                        }*/

            return line;
        }

        public LokaKitEntry UpdateSpeechLine_Simple(LokaKitEntry line, out bool changed)
        {
            Debug.Log("Check for changes " + lineID + " - " + line.lineID);
            Debug.Log("Check for changes " + token.Trim() + " - " + line.token.Trim());
            Debug.Log("Check for changes " + textType + " - " + line.textType);
            changed = false;
            if (line.lineID != lineID)
                return null;
            if (token.Trim() != line.token.Trim())
            {
                if (importChangedTokens)
                {
                    Debug.Log("(" + line.lineID + ")" + " Changed TOKEN -> " + token.Trim());
                    changed = true;
                    line.token = token.Trim();
                }
                else
                    return null;

            }
            //Debug.Log(line.translationText.Count + " " + translations.Length);
            string german = AdjustText(translations[0]);
            Debug.Log(german);
            string english = AdjustText(translations[1]);
            Debug.Log(english);
            if (line.translationText[0].Trim() != german.Trim())
            {
                Debug.Log("(" + line.lineID + ")" + " Changed DEUTSCH - TEXT");
                changed = true;
                line.lastChangedInVersion = Application.version;
                line.translationText[0] = german;
            }
            if (line.translationText[1].Trim() != english.Trim())
            {
                Debug.Log("(" + line.lineID + ")" + " Changed ENGLISH - TEXT");
                changed = true;
                line.lastChangedInVersion = Application.version;
                line.translationText[1] = english;
            }
            Debug.Log("Type: " + textType);
            if (textType > -1)
            {
                if(textType != line.textType)
                {
                    changed = true;
                    line.textType = textType;
                }    
                
            }
            return line;

        }

        public LokaKitEntry GetAsNewEntry()
        {
            LokaKitEntry entry = new LokaKitEntry();
            string german = AdjustText(translations[0]);
            Debug.Log(german);
            string english = AdjustText(translations[1]);
            Debug.Log(english);
            entry.translationText = new List<string>();
            entry.translationText.Add(german);
            entry.translationText.Add(english);
            entry.token = token;
            if (textType > -1)
            {
                entry.textType = textType;
            }
             return entry;
        }

        public string AdjustText(string text)
        {
            if (text == null)
            {
                Debug.LogError("Translation empty");
                return "";
            }
            // Debug.Log(text);
            if (text.Length == 0)
                return text;
            string con = text.Replace("[break]", "\n");
            if (con[con.Length - 1] == '\n')
                con = con.Substring(0, con.Length - 1);
            if (con.Contains("\"\"") && con[0] == '"')
            {
                con = con.Replace("\"\"", "\"");
                con = con.Substring(1);
                con = con.Substring(0, con.Length - 1);
            }
            return con;
        }

        /*public SpeechLineExtension UpdateSpeechLineExt(SpeechLineExtension line, bool[] columnsToChange)
        {
            if (line == null || line.lineID != lineID)
                return null;
            if (columnsToChange[1])
            {
                Debug.Log("(" + line.lineID + ")" + " Changed DEUTSCH - REGIE");
                line.directions[0] = AdjustText(directions[0]);
            }

            if (columnsToChange[2])
            {
                Debug.Log("(" + line.lineID + ")" + " Changed DEUTSCH - KOMMENTAR");
                line.comments[0] = AdjustText(comments[0]);
            }

            if (columnsToChange[4])
            {
                if (line.directions.Length >= 2)
                {
                    Debug.Log("(" + line.lineID + ")" + " Changed ENGLISH - DIRECTION");
                    line.directions[1] = AdjustText(directions[1]);
                }

            }

            if (columnsToChange[5])
            {
                if (line.comments.Length >= 2)
                {
                    Debug.Log("(" + line.lineID + ")" + " Changed ENGLISH - COMMENT");
                    line.comments[1] = AdjustText(comments[1]);
                }
            }


            return line;
        }*/
    }

    void RemoveQuotesProblem()
    {
        LoadSpeechManager();
        for (int i = 0; i < lokaKit.entries.Count; i++)
        {
            bool changed = false;

            LokaKitEntry line = lokaKit.entries[i];
            string falseText = "";
            for (int j = 0; j < line.translationText.Count; j++)
            {
                string newText = RemoveDoubleQuotesFromText(line.translationText[j]);
                if (line.translationText[j] != newText)
                {
                    changed = true;
                    falseText = line.translationText[j];
                    line.translationText[j] = newText;

                }
            }

            if (changed)
            {
                lokaKit.entries[i] = line;
                Debug.Log("Quotes Problem found and solved in " + line.token + " (" + falseText + ")");
            }
        }

        //KickStarter.speechManager = speechManager;
        UnityEditor.EditorUtility.SetDirty(lokaKit);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
}
