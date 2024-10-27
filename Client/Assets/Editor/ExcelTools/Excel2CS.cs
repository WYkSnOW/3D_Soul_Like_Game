using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExcelDataReader;
using UnityEngine;
//using OfficeOpenXml;

public class Excel2CS
{
    static string _writePath;
    static List<string> excelList = new List<string>();
    public static Dictionary<string, string> fileDct = new Dictionary<string, string>();

    public static bool Start(string path, string writePath, bool check_font)
    {
        ms1 = 0;
        ms2 = 0;
        ms3 = 0;
        ds_dct.Clear();
        TypesDctInit();

        _writePath = writePath;
        fileDct.Clear();
        List<string> fileLst = new List<string>();
        var _files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        fileLst.AddRange(_files);
        //��ȡҪת�������ñ�
        string[] files = fileLst.ToArray(); //Directory.GetFiles(path);
        for (int i = 0; i < files.Length; i++)
        {
            //Debug.Log(files[i]);
            if (!files[i].Contains("~$") && files[i].EndsWith(".xlsx") && files[i].Contains("_"))//xlsx
            {
                excelList.Add(files[i]);
            }
        }

        readInfoLst.Clear();

        if (check_font)
        {

            table_text.Clear();
            sstext = File.ReadAllText(Application.dataPath + @"\Resources\Res\Sprite\Font\3500��.txt");
            var p2 = Application.dataPath + @"\Resources\Res\Sprite\Font\3500��1.txt";
            if (File.Exists(p2))
            {
                var sstext2 = File.ReadAllText(p2);
                sstext += sstext2;
            }

            new_text = "";
        }

        Stopwatch stopwatch1 = new System.Diagnostics.Stopwatch();
        stopwatch1.Start();

        List<Task> t1 = new List<Task>();
        for (int i = 0; i < excelList.Count; i++)
        {
            var item = excelList[i];
           
            DataSet dataSet = new DataSet();
            ds_dct[item] = dataSet;
            var tt= Task.Run(() => {
                ReadDataForExcelTable1(item, dataSet);
            });
            t1.Add(tt);
        }
        Task.WaitAll(t1.ToArray());

        stopwatch1.Stop();
       UnityEngine.Debug.LogError($"�����ʱ:{stopwatch1.Elapsed.TotalMilliseconds}");

        Stopwatch stopwatch2 = new System.Diagnostics.Stopwatch();
        stopwatch2.Start();
        //����ÿ�����ñ� ����ת������
        for (int i = 0; i < excelList.Count; i++)
        {
            var item = excelList[i];
            ReadDataForExcelTable(item, check_font);
        }
        stopwatch2.Stop();
        UnityEngine.Debug.LogError($"������ȡ��ʱ:{stopwatch2.Elapsed.TotalMilliseconds}");

        if (check_font)
        {
            StringBuilder ss = new StringBuilder();
            ss.AppendLine(new_text);
            ss.AppendLine();
            foreach (var item in table_text)
            {
                ss.AppendLine($"��:{item.Key}");
                ss.AppendLine($"{item.Value}");
                ss.AppendLine();
            }
            var target_path = Application.dataPath + "/../�ֿ�ȱʧ���ñ�����.txt";
            File.WriteAllText(target_path, ss.ToString(), Encoding.UTF8);
            Application.OpenURL(target_path);
        }


        foreach (var item in readInfoLst)
        {
            if (string.IsNullOrEmpty(item.error) == false)
            {
                UnityEngine.Debug.Log($"����ʧ��,�����ж�:{item.error}");
                return false;
            }
            fileDct[item.path] = item.content;
        }


        UnityEngine.Debug.Log($"��ʼ����{fileDct.Count}�������ļ�");
        fileDct[writePath + "GlobalConfig.cs"] = "namespace Game.Config\r\n{\r\n    public class GlobalConfig\r\n    {\r\n        public static int LanguageType=0;\r\n    }\r\n}";

        if (fileDct.Count > 0)
        {
            //��������ת�� cs�ű���Ŀ¼
            if (Directory.Exists(writePath))
            {
                Directory.Delete(writePath, true);
            }
            Directory.CreateDirectory(writePath);
            List<Task> tasks = new List<Task>();
            foreach (var item in fileDct)
            {
                string k = item.Key;
                string v = item.Value;  

               var t = File.WriteAllTextAsync(k, v, UTF8Encoding.UTF8);
                tasks.Add(t);   
                //WriteFile(item.Key, item.Value);
            }
            Task.WaitAll(tasks.ToArray());
        }

        UnityEngine.Debug.Log($"���ɳɹ�,��{fileDct.Count}�����ļ� ��ȡ��ʱ:{ms1}  �����ʱ:{ms2}  ���ɺ�ʱ:{ms3}");
        return true;
    }
    static string sstext = "";
    static string new_text = "";
    static Dictionary<string, string> table_text = new Dictionary<string, string>();
    public class ReadInfo
    {
        public bool result;
        public string path;
        public string content;
        public string error;
    }
    //public static object _readObj=new object();
    public static List<ReadInfo> readInfoLst = new List<ReadInfo>();
    static double ms1, ms2, ms3;
    public static Dictionary<string, DataSet> ds_dct = new Dictionary<string, DataSet>();

    static void ReadDataForExcelTable1(string path, DataSet dataSet) {

        ////�����ñ������ ��ȡ�� ���������
        //DataSet dataSet;
        using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
        {
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                do { while (reader.Read()) { } } while (reader.NextResult());

                var configuration = new ExcelDataSetConfiguration { ConfigureDataTable = tableReader => new ExcelDataTableConfiguration { UseHeaderRow = true } };
                dataSet = reader.AsDataSet(configuration);
                ds_dct[path] = dataSet;
            }
        }


        //using (var package = new ExcelPackage(new FileInfo(path)))
        //{
        //    // ��ȡ������
        //    var worksheet = package.Workbook.Worksheets["##"];

        //    UnityEngine.Debug.LogError($"{path}  ����:{worksheet.Dimension.End.Row}  ����:{worksheet.Dimension.End.Column}");
        //    // ��ȡ����
        //    var value1 = worksheet.Cells["A7"].Value.ToString();
        //    var value2 = worksheet.Cells["B7"].Value.ToString();

        //    UnityEngine.Debug.LogError(value1);
        //    UnityEngine.Debug.LogError(value2);

        //    UnityEngine.Debug.LogError(worksheet.GetValue(7, 1) );
        //    UnityEngine.Debug.LogError(worksheet.GetValue(7, 2));
        //}

    }

    /// <summary>
    /// ��ȡ���ñ�
    /// </summary>
    /// <param name="path"></param>
    static void ReadDataForExcelTable(string path, bool check_font)
    {


        ReadInfo _ReadInfo = new ReadInfo();
        _ReadInfo.result = true;


        //�����ñ������ ��ȡ�� ���������
        DataSet dataSet=ds_dct[path];
        //using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
        //{
        //    using (var reader = ExcelReaderFactory.CreateReader(stream))
        //    {
        //        do { while (reader.Read()) { } } while (reader.NextResult());

        //        var configuration = new ExcelDataSetConfiguration { ConfigureDataTable = tableReader => new ExcelDataTableConfiguration { UseHeaderRow = true } };
        //        dataSet = reader.AsDataSet(configuration);
        //    }
        //}

        
        //UDebug.LogError($"��ȡʱ:{stopwatch1.Elapsed.TotalMilliseconds} +{path}");


        //Stopwatch stopwatch2 = new System.Diagnostics.Stopwatch();
        //stopwatch2.Start();

        List<TableEntity> tabList = new List<TableEntity>();

        #region �Ȼ���ÿ�ű������
        for (int i = 0; i < dataSet.Tables.Count; i++)
        {
            //Console.WriteLine(dataSet.Tables[i].TableName);
            var title = dataSet.Tables[i].TableName.Trim();
            if (title != "##")// if (!dataSet.Tables[i].TableName.Contains("##"))
            {
                continue;
            }

            DataTable tab = dataSet.Tables[i];//��
                                              //ת��Ϊʵ����� ��¼�����߼���Ҫ����Ϣ
            TableEntity tableEntity = new TableEntity();
            string filePath = Path.GetFileName(path);
            string tableName = filePath.Split('_')[1].Replace(".xlsx", "");
            //����
            tableEntity.excel_name = tableName;
            tableEntity.TableName = tableName + "Data";
            //Debug.Log("�ű�����:" + tableName);

            //��ȡ��ͷ:ע�͡��ֶ����ơ�����
            for (int j = 0; j < tab.Columns.Count; j++)
            {
                var columnName = tab.Columns[j].ColumnName;
                var x = tab.Rows[3][columnName].ToString().Trim();
                string type = tab.Rows[4][j].ToString().Trim();

                if (!string.IsNullOrEmpty(x) && type != "ע��")
                {
                    //�ֶ�ע��
                    tableEntity.FieldName.Add(tab.Rows[2][columnName].ToString().Trim());
                    //�ֶ�����
                    tableEntity.FieldList.Add(tab.Rows[3][columnName].ToString().Trim());

                    if (!HasType(type))
                    {
                        //Debug.LogError($"���Ͳ�����-------��:{path} ��:{columnName} {type}");
                        _ReadInfo.error = $"���Ͳ�����-------��:{path} ��:{columnName} {type}";
                        _ReadInfo.result = false;
                        readInfoLst.Add(_ReadInfo);
                        return;
                    }
                    //��¼�ֶ�����
                    tableEntity.FieldType.Add(type);
                    tableEntity.isLog.Add(false);
                }
                else
                {
                    if (type == "ע��")
                    {
                        tableEntity.FieldName.Add("ע��");
                        tableEntity.FieldType.Add("ע��");
                        tableEntity.FieldList.Add("ע��");
                        tableEntity.isLog.Add(true);
                    }
                }
            }


            //��ȡʵ�ʵ���������
            for (int x = 5; x < tab.Rows.Count; x++)
            {
                //һ�е����� 
                List<string> row = new List<string>();
                //һ���е�ÿ���ֶ�
                for (int y = 0; y < tableEntity.FieldName.Count; y++)
                {
                    if (y == 0 && string.IsNullOrEmpty(tab.Rows[x][y].ToString()))
                    {
                        _ReadInfo.result = false;
                        _ReadInfo.error = $"ȱ������:-------��:{path} ��{x + 2}��";

                        readInfoLst.Add(_ReadInfo);
                    }
                    string str = tab.Rows[x][y].ToString();
                    str = str.Replace("\n", "").Replace("\r", "").Replace("\r\n", "").Trim();
                    //�ɵ�json���� �Ƴ��ո�
                    //if (str.StartsWith("["))
                    //{
                    //    str = str.Replace(" ", "");
                    //}
                    row.Add(str);
                }
                tableEntity.Rows.Add(row);
            }
            tabList.Add(tableEntity);
        }

        //������ �ֿ�ȱ©���
        if (check_font)
        {

            for (int i = 0; i < tabList.Count; i++)
            {
                var tab = tabList[i];
                if (tab.Rows == null)
                {
                    continue;
                }
                for (int j = 0; j < tab.Rows.Count; j++)
                {
                    var row = tab.Rows[j];
                    if (row == null)
                    {
                        continue;
                    }

                    for (int k = 0; k < row.Count; k++)
                    {
                        if (string.IsNullOrEmpty(row[k]))
                        {
                            continue;
                        }

                        var rowText = row[k];
                        for (int x = 0; x < rowText.Length; x++)
                        {
                            if (rowText[x] == ' ')
                            {
                                continue;
                            }

                            if (sstext.Contains(rowText[x]) == false)
                            {
                                if (new_text.Contains(rowText[x]) == false)
                                {
                                    new_text += rowText[x];
                                    if (table_text.ContainsKey(tab.excel_name) == false)
                                    {
                                        table_text[tab.excel_name] = "";
                                    }
                                    table_text[tab.excel_name] += rowText[x];
                                }
                            }

                        }

                    }

                }
            }
        }
        #endregion
        
        //stopwatch2.Stop();
        //ms2 += stopwatch2.Elapsed.TotalMilliseconds;

        //UDebug.LogError($"����ʱ:{stopwatch2.Elapsed.TotalMilliseconds}");


        //--------------�ָ���:����ʵ��ģ��--------------//

        #region �ڶ������Ǽ�������ʲô����
        Stopwatch stopwatch3 = new System.Diagnostics.Stopwatch();
        stopwatch3.Start();
        string temp = @"
using System.Collections.Generic;
using UnityEngine;

namespace Game.Config
{
    public class ClassName
    {

        static ClassName()
        {
            //��ʼ��
        }

       
        //Get����
    }

    //ʵ����
}
";
           

        //��ÿ�ű���н���
        for (int i = 0; i < tabList.Count; i++)
        {
            //����ʵ���� �ӵ�ÿ�����õ�ĩβ
            string templateEntity = CreateEntity(tabList[i]);
            string init = CreateInit(tabList[i]);
            string get = CreateGet(tabList[i]);
            //ȥ������ʵ���Ľӿ�
            //string instance = CreateGetInstance(tabList[i]);
            string configTemp = temp;
            //.Replace("//����ʵ��",instance)
            configTemp = configTemp.Replace("ClassName", tabList[i].TableName)
                .Replace("//��ʼ��", init).Replace("//Get����", get).Replace("//ʵ����", templateEntity);


            #region ���������ǽ��ű��ļ��������
            _ReadInfo.path = _writePath + tabList[i].TableName + ".cs";
            _ReadInfo.content = configTemp;
            _ReadInfo.result = true;
            readInfoLst.Add(_ReadInfo);
            #endregion
        }
        stopwatch3.Stop();
        ms3 += stopwatch3.Elapsed.TotalMilliseconds;
        //UDebug.LogError($"����ʱ:{stopwatch3.Elapsed.TotalMilliseconds}");

        #endregion
    }


    /// <summary>
    /// д���ļ�
    /// </summary>
    /// <param name="path"></param>
    /// <param name="content"></param>
    public static void WriteFile(string path, string content)
    {
        //if (File.Exists(path))
        //{
        //    File.Delete(path);
        //}
        //using (StreamWriter sw = new StreamWriter(path, false, UTF8Encoding.UTF8))
        //{
        //    sw.Write(content);
        //}

        File.WriteAllTextAsync(path, content, UTF8Encoding.UTF8);
    }



    private static string CreateGet(TableEntity tableEntity)
    {
        string temp = @"
        public static Dictionary<keyType, TempEntity> all {
            get {
                return entityDic;
            }
        }
		static Dictionary<keyType, TempEntity> entityDic;
		public static TempEntity Get(keyType keyID)
		{
            if (entityDic!=null&&entityDic.TryGetValue(keyID,out var entity))
			{
				return entity;
			}
            return null;
		}";
        temp = temp.Replace("TempEntity", tableEntity.TableName.Replace("Data", "Entity"));
        temp = temp.Replace("keyType", tableEntity.FieldType[0]);
        temp = temp.Replace("keyID", tableEntity.FieldList[0]);
        //Console.WriteLine(temp);
        return temp;
    }

    public static string CreateInit(TableEntity tableEntity)
    {
        StringBuilder init = new StringBuilder();

        //string temp = "             TempEntity tempEntity = new TempEntity(//canshu);";
        //������������
        for (int j = 0; j < tableEntity.Rows.Count; j++)
        {
            StringBuilder sb = new StringBuilder();
            if (j == 0)
            {
                sb.AppendLine($"entityDic = new Dictionary<{tableEntity.FieldType[0]}, {tableEntity.TableName.Replace("Data", "Entity")}>({tableEntity.Rows.Count});");
            }
            string entityName = tableEntity.TableName.Replace("Data", "Entity");
            string className = "e" + j;

            //string memberInit = "           tempEntity.m_id = m_value;";

        
           //sb.AppendLine(temp.Replace("TempEntity", entityName).Replace("tempEntity", className));

            //�β�
            StringBuilder formalParameterText = new StringBuilder();
            //ÿһ�е�ÿ���ֶ�
            for (int k = 0; k < tableEntity.Rows[j].Count; k++)
            {
                if (tableEntity.isLog[k])
                {
                    continue;
                }

                var is_en = tableEntity.FieldList[k].Contains("_en");

                //�ֶ����ƶ����������
                List<string> Row = tableEntity.Rows[j];
                //�����Ԫ�������ǿ� ���Ӹ�Ĭ��ֵ
                Row[k] = GetDefault(Row[k], tableEntity.FieldType[k]);

                //ת���ɴ����ķ��Ŵ��� "" f []
                var fx_value = GetFieldGenerCode(Row[k], tableEntity.FieldType[k]);
                if (is_en)
                {
                    fx_value = fx_value.Replace("��", ",");
                }

                formalParameterText.Append(fx_value);// GetFieldGenerCode(Row[k], tableEntity.FieldType[k]));

                if (k != tableEntity.Rows[j].Count - 1)
                {
                    formalParameterText.Append(",");
                }
            }

            sb.AppendLine($"             {entityName} {className} = new {entityName}({formalParameterText});");

            //sb = sb.Replace("//canshu", formalParameterText.ToString());
            /*sb.Append($"{formalParameterText});");//*/  sb.Replace("//canshu", formalParameterText.ToString());

            //string add = @"            entityDic.Add(tempEntity.ID, tempEntity);";
            //add = add.Replace("tempEntity", className);
            //add = add.Replace("ID", tableEntity.FieldList[0]);
            //sb.AppendLine(add);

            sb.AppendLine(@$"            entityDic.Add({className}.{tableEntity.FieldList[0]}, {className});");//

            init.Append(sb);
            //Console.WriteLine(sb);
        }

        return init.ToString();
    }

    public static string GetDefault(string input, string fieldType)
    {

        //Ĭ��ֵ
        if (string.IsNullOrEmpty(input))
        {
            //Row[k] = SetDefalutValue(Row[k], tableEntity.FieldType[k]);
            switch (fieldType)
            {
                case "int":
                case "long":
                case "float":
                    input = "0";
                    break;
                case "bool":
                    input = "false";
                    break;
                case "string":
                case "String":
                case "json":
                    input = "null";
                    break;
                default:
                    input = "null";
                    break;
            }
        }
        return input;
    }

    public static string GetFieldGenerCode(string input, string fieldType, bool isJson = false)
    {
        StringBuilder _sb = new StringBuilder();
        if (fieldType == "string" || fieldType == "json")
        {
            if (input == "null")
            {
                _sb.Append("null");
            }
            else
            {
                if (isJson == false)
                {
                    if (input.Contains(@"\n"))
                    {
                        _sb.Append($"\"{input.Replace("\"", "\"\"")}\"");
                    }
                    else
                    {
                        _sb.Append($"@\"{input.Replace("\"", "\"\"")}\"");
                    }
                }
                else
                {
                    _sb.Append($"\"{input}\"");
                }

            }
        }
        else if (fieldType == "float")
        {
            if (isJson == false)
            {
                _sb.Append(input + "f");
            }
            else
            {
                _sb.Append(input);
            }
        }
        else if (fieldType.Contains("[]"))
        {
            if (input == "null")
            {
                _sb.Append("null");
            }
            else
            {
                if (fieldType.Contains("string[]"))
                {
                    if (isJson == false)
                    {
                        _sb.Append($"new {fieldType}{{ \"{input.Replace(",", "\",\"")}\" }}");
                    }
                    else
                    {
                        _sb.Append($"[\"{input.Replace(",", "\",\"")}\"]");
                    }

                }
                else if (fieldType.Contains("float[]"))
                {
                    if (isJson == false)
                    {
                        _sb.Append($"new {fieldType}{{{input.Replace(",", "f,")}f}}");
                    }
                    else
                    {
                        _sb.Append($"[{input}]");
                    }
                }
                else
                {
                    if (isJson == false)
                    {
                        _sb.Append($"new {fieldType}{{{input}}}");
                    }
                    else
                    {
                        _sb.Append($"[{input}]");
                    }
                }
            }
        }
        else
        {
            //�е����ñ� ����õ���ֵ�� �� or ��  ��true ����false ��ʵ��else������
            if (input == "��" || input.ToLower() == "true")
            {
                _sb.Append("true");
            }
            else if (input == "��" || input.ToLower() == "false")
            {
                _sb.Append("false");
            }
            else
            {
                // int long ...
                _sb.Append(input);
            }
        }
        return _sb.ToString();
    }

    public static string CreateEntity(TableEntity tableEntity)
    {
        string templateEntity = @"
    public class TemplateEntity
    {
        //TemplateMember
        public TemplateEntity(){}
        public TemplateEntity(//_canshu){
            //fuzhi
        }
    }";
        string memberInit = "           this.field = field;";
        StringBuilder entity = new StringBuilder();
        StringBuilder canshu = new StringBuilder();
        StringBuilder fuzhi = new StringBuilder();

        //�����ֶ��б�
        for (int i = 0; i < tableEntity.FieldList.Count; i++)
        {
            if (tableEntity.isLog[i])
            {
                continue;
            }
            string field = tableEntity.FieldList[i];
            bool addCN = false;
            if (!tableEntity.FieldList.Contains(field + "_en") && !tableEntity.FieldList.Contains(field + "_tw"))
            {
                entity.AppendLine("		public " + tableEntity.FieldType[i] + " " + tableEntity.FieldList[i] + ";//" + tableEntity.FieldName[i]);
            }
            else
            {
                addCN = true;
                //�ȴ���һ���ֶ� ��׺Ϊ_CN
                entity.AppendLine("		public " + tableEntity.FieldType[i] + " " + field + "_cn" + ";//" + tableEntity.FieldName[i]);

                //����һ�����Է����� private
                //entity.AppendLine("		" + tableEntity.FieldType[i] + " " + "_"+field  + ";//" + tableEntity.FieldName[i]);

                //�������Է���������
                string _get = "get{if(GlobalConfig.LanguageType==0){return " + field + "_cn;}" + "else if(GlobalConfig.LanguageType==1){return " + field + "_tw;}" + "else if(GlobalConfig.LanguageType==2){return " + field + "_en;}" + "else{return null;}" + "}";
                //string _set = "set{_"+ field+"=value;}";
                //string _set = "set{}";

                entity.AppendLine("		public " + tableEntity.FieldType[i] + " " + field + "{" + _get + "}");

            }

            canshu.Append(tableEntity.FieldType[i] + " " + tableEntity.FieldList[i] + ((i == tableEntity.FieldList.Count - 1) ? "" : ","));
            if (addCN == false)
            {
                fuzhi.AppendLine(memberInit.Replace("field", tableEntity.FieldList[i]));
            }
            else
            {
                string memberInit2 = "           this.field_cn = field;";
                fuzhi.AppendLine(memberInit2.Replace("field", tableEntity.FieldList[i]));
            }
        }

        templateEntity = templateEntity.Replace("TemplateEntity", tableEntity.TableName.Replace("Data", "Entity"));
        templateEntity = templateEntity.Replace("//TemplateMember", "//TemplateMember" + "\n" + entity.ToString());
        templateEntity = templateEntity.Replace("//_canshu", canshu.ToString());
        templateEntity = templateEntity.Replace(" //fuzhi", "\n" + fuzhi.ToString());

        //Console.Write(templateEntity);
        return templateEntity;
    }
    public static Dictionary<string, int> typesdct = new Dictionary<string, int>();
    public static Dictionary<string, string> str_dct= new Dictionary<string, string>();

    public static void TypesDctInit() {

        replace_cache.Clear();

        typesdct.Clear();
        typesdct.Add("int", 0);
        typesdct.Add("long", 0);
        typesdct.Add("float", 0);
        typesdct.Add("string", 0);
        typesdct.Add("bool", 0);
        typesdct.Add("", 0);

        str_dct.Clear();
        str_dct.Add("json", "string");
        str_dct.Add("String", "string");
    }

    //public static string[] types = new string[] { "int", "long", "float", "string", "bool", "json", "" };
   
    public static Dictionary<string, string> replace_cache = new Dictionary<string, string>();
    public static string CustomReplace(string str) {
        if (replace_cache.TryGetValue(str,out var x))
        {
            return x;
        }
        else
        {
            replace_cache[str] = $"{str.Replace("[]", "")}";
            return replace_cache[str];
        }
      
    }


    public static bool HasType(string str)
    {
        str = str.ToLower();
        if (typesdct.ContainsKey(str))
        {
            return true;
        }
        else if (typesdct.ContainsKey(CustomReplace(str)))//         $"{str.Replace("[]", "")}"))
        {
            return true;
        }
        return false;
        //return types.Contains(str) || types.Contains($"{str.Replace("[]", "")}");
    }

    public class TableEntity
    {
        public string excel_name;//����
        public string TableName;//����
        public List<string> FieldName = new List<string>();//�ֶ�ע�� ����0
        public List<string> FieldType = new List<string>();//�ֶ��������� ����1
        public List<string> FieldList = new List<string>();//�ֶ����� ����2
        public List<bool> isLog = new List<bool>();
        public List<List<string>> Rows = new List<List<string>>();//ÿһ�еļ���
    }

}
