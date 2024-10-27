using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

using UnityEditor;
using UnityEditor.Compilation;
using System.Linq;
using UnityEngine;

public class ExcelTools 
{
    public static string excelInput;
    public static string outputCS;
    public static string outJson;
    public static void Init() {
        //���2��·��
        excelInput = Application.dataPath + @"\..\..\Tools\Excel\"; 
        outputCS = Application.dataPath + @"\Script\Hotfix\ExcelConfig\"; 
        outJson= Application.dataPath + @"\Resources\Res\UnitConfig\"; 
    }

    //_Unity2022
    [MenuItem("Tools/Excel����/������Ϸ���ýű�")]
    public static void Build2() {

        if (Application.isPlaying)
        {
            UnityEngine.Debug.LogError("����ֹͣ����,�ٽ��е���");
            return;
        }
        KillWpsProcesses();
        if (EditorApplication.isCompiling == true)
        {
            UnityEngine.Debug.LogError($"��ȴ�������ɺ�������.");
            return;
        }

        Init();
        if (string.IsNullOrEmpty(excelInput))
        {
            UnityEngine.Debug.LogError("������Excel����Ŀ¼");
            return;
        }

        if (string.IsNullOrEmpty(outputCS))
        {
            UnityEngine.Debug.LogError("������CS���Ŀ¼");
            return;
        }

        var r= Excel2CS.Start(excelInput, outputCS,false);
        if (r)
        {
            CompilationPipeline.RequestScriptCompilation();
            CompilationPipeline.compilationFinished += (obj) => {
                UnityEngine.Debug.Log("������ִ�����!");
                AssetDatabase.Refresh();
            };
        }

    }


    //[MenuItem("����༭/�������ȱʧ����")]
    public static void Build3()
    {

        if (Application.isPlaying)
        {
            UnityEngine.Debug.LogError("����ֹͣ����,�ٽ��е���");
            return;
        }
        KillWpsProcesses();
        if (EditorApplication.isCompiling == true)
        {
            UnityEngine.Debug.LogError($"��ȴ�������ɺ�������.");
            return;
        }

        Init();
        if (string.IsNullOrEmpty(excelInput))
        {
            UnityEngine.Debug.LogError("������Excel����Ŀ¼");
            return;
        }

        if (string.IsNullOrEmpty(outputCS))
        {
            UnityEngine.Debug.LogError("������CS���Ŀ¼");
            return;
        }

        var r =  Excel2CS.Start(excelInput, outputCS, true);
        if (r)
        {
            CompilationPipeline.RequestScriptCompilation();
            CompilationPipeline.compilationFinished += (obj) => {
                UnityEngine.Debug.Log("������ִ�����!");
                AssetDatabase.Refresh();
            };
        }

    }


    //Start


    //[MenuItem("Tools/Excel����/������Ϸ���ýű�")]
    public static void Build() {
        if (Application.isPlaying)
        {
            UnityEngine.Debug.LogError("����ֹͣ����,�ٽ��е���");
            return;
        }

        KillWpsProcesses();
        if (EditorApplication.isCompiling == true)
        {
            UnityEngine.Debug.LogError($"��ȴ�������ɺ�������.");
            return;
        }
     
        
        Init();
        if (string.IsNullOrEmpty(excelInput))
        {
           UnityEngine.Debug.LogError("������Excel����Ŀ¼");
            return;
        }

        if (string.IsNullOrEmpty(outputCS))
        {
           UnityEngine.Debug.LogError("������CS���Ŀ¼");
            return;
        }
        //ɾ�����ɵ�����
        //if (Directory.Exists(outputCS))
        //{
        //    Directory.Delete(outputCS, true);
        //}

         var WorkingDirectory = Application.dataPath + @"/../../Tools/Excel2CS/bin/Debug/netcoreapp3.1/";
         var FileName = "Excel2CS.exe";

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = WorkingDirectory + FileName;
        startInfo.Arguments = excelInput + " " + outputCS+" "+ outJson;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = false;
        startInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;
    
        Process processTemp = new Process();
        List<string> log = new List<string>();

        processTemp.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                log.Add(e.Data);
                UnityEngine.Debug.Log(e.Data);//e.Data
            }
        });
        processTemp.StartInfo = startInfo;
        processTemp.EnableRaisingEvents = true;
        processTemp.Start();
        processTemp.BeginOutputReadLine();
        //UnityEngine.Debug.Log(processTemp.StandardOutput.ReadToEnd());
        processTemp.WaitForExit();
        CompilationPipeline.RequestScriptCompilation();
        CompilationPipeline.compilationFinished += (obj) => {
            for (int i = 0; i < log.Count; i++)
            {
                UnityEngine.Debug.Log(log[i]);//e.Data
            }
            UnityEngine.Debug.Log("������ִ�����!"); 
            AssetDatabase.Refresh();
        };
    }



    //�������ñ����
    //[MenuItem("Tools/Excel����/�����������ñ�")]
    public static void Json2Excel()
    {
        KillWpsProcesses();
        if (EditorApplication.isCompiling == true)
        {
            UnityEngine.Debug.LogError($"��ȴ�������ɺ�������.");
            return;
        }

        //xlsxPath
        //jsonpath
        //string xlsxPath = $@"E:\xlsx\" + "test.xlsx";
        //string jsonPath = $@"D:\src\client\zyz\Assets\Resources\Res\UnitConfig\ZYStateData.txt";

        var excelFolder = Application.dataPath + "/../../Tools/Excel/";
       var excels = Directory.GetFiles(excelFolder, "*.xlsx", SearchOption.AllDirectories);
        //for (int i = 0; i < excels.Length; i++)
        //{
        //   UnityEngine.Debug.Log(excels[i]);
        //}
        //return;
        string jsonRoot = $@"{Application.dataPath}\Resources\Res\UnitConfig\";
        string[] jsonLst = new string[] { "ZYAnimationData", "ZYAnimationSpeedConfigData", "ZYAudioConfigData", "ZYEffectConfigData", "ZYHitConfigData", "ZYPhysicsConfigData", "ZYStateData" };

        for (int i = 0; i < jsonLst.Length; i++)
        {
            var excelFile=jsonLst[i].Replace("Data", ".xlsx");
            if (!excelFile.EndsWith(".xlsx"))
            {
                UnityEngine.Debug.LogError($"������ƴ���:{excelFile}");
                return;
            }
            string xlsxPath;
            string jsonPath = jsonRoot + jsonLst[i]+".txt";
            UnityEngine.Debug.LogError($"json�ļ�:{jsonPath}");
            for (int j = 0; j < excels.Length; j++)
            {
                //UnityEngine.Debug.LogError($"ƥ��:{excelFile}  {excels[j]}");
                if (!excels[j].Contains("$")&&excels[j].EndsWith(excelFile))
                {
                    xlsxPath = excels[j];
                    UnityEngine.Debug.LogError($"��ʼ����:{xlsxPath}");
                    UpdateExcel(xlsxPath, jsonPath);
                }
            }
        }


        //ZYAnimationData
        //ZYAnimationSpeedConfigData
        //ZYAudioConfigData
        //ZYEffectConfigData
        //ZYHitConfigData
        //ZYPhysicsConfigData
        //ZYStateData
    }

    private static void KillWpsProcesses()
    {
        Process[] procs = Process.GetProcessesByName("wps");
        foreach (Process pro in procs)
        {
            //UnityEngine.Debug.LogError("wps��������");
            //pro.Close();
            pro.Kill();//û�и��õķ���,ֻ��ɱ������
        }

        Process[] excelProcs = Process.GetProcessesByName("excel");
        foreach (Process pro in excelProcs)
        {
            //UnityEngine.Debug.LogError("wps��������");
            //pro.Close();
            pro.Kill();//û�и��õķ���,ֻ��ɱ������
        }

        //return procs;
    }

    public static void UpdateExcel(string xlsxPath,string jsonPath) {

        var WorkingDirectory = Application.dataPath + @" /../../Tools/Json2Excel/";
        var FileName = "Excel2JsonProject.exe";

        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = WorkingDirectory + FileName;
        startInfo.Arguments = xlsxPath + " " + jsonPath;  //excelInput + " " + outputCS + " " + outJson;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = false;
        startInfo.StandardOutputEncoding = System.Text.Encoding.UTF8;

        Process processTemp = new Process();
        List<string> log = new List<string>();

        processTemp.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                log.Add(e.Data);
                UnityEngine.Debug.Log(e.Data);//e.Data
            }
        });
        processTemp.StartInfo = startInfo;
        processTemp.EnableRaisingEvents = true;
        processTemp.Start();
        processTemp.BeginOutputReadLine();
        //UnityEngine.Debug.Log(processTemp.StandardOutput.ReadToEnd());
        processTemp.WaitForExit();
        CompilationPipeline.RequestScriptCompilation();
        CompilationPipeline.compilationFinished += (obj) => {
            for (int i = 0; i < log.Count; i++)
            {
                UnityEngine.Debug.Log(log[i]);//e.Data
            }
            UnityEngine.Debug.Log("������ִ�����!");
            AssetDatabase.Refresh();
        };

    }

    //[MenuItem("Tools/Excel����/Excel2ScriptableObjectData")]
    //public static void ExcelData2ScriptableObjectData() {

    //    var anm = Resources.Load<AnmScriptableObject>("Res/Config/AnmScriptableObject");
    //    var anmSpeed = Resources.Load<AnmSpeedScriptableObject>("Res/Config/AnmSpeedScriptableObject");
    //    var audio = Resources.Load<AudioScriptableObject>("Res/Config/AudioScriptableObject");
    //    var effect = Resources.Load<EffectScriptableObject>("Res/Config/EffectScriptableObject");
    //    var phy = Resources.Load<PhysicsScriptableObject>("Res/Config/PhysicsScriptableObject");
    //    var state = Resources.Load<StateScriptableObject>("Res/Config/StateScriptableObject");

    //    //var anmJson = JsonHelper.ToJson(ZYAnimationData.all.Values.ToList());
    //    //anm.data = JsonHelper.ToObject<List<AnimationEntity>>(anmJson);

    //    //var anmSpeedJson = JsonHelper.ToJson(ZYAnimationSpeedConfigData.all.Values.ToList());
    //    //anmSpeed.data = JsonHelper.ToObject<List<AnmSpeedConfig>>(anmSpeedJson);

    //    //var audioJson = JsonHelper.ToJson(ZYAudioConfigData.all.Values.ToList());
    //    //audio.data = JsonHelper.ToObject<List<AudioConfig>>(audioJson);

    //    //var effectJson = JsonHelper.ToJson(ZYEffectConfigData.all.Values.ToList());
    //    //effect.data = JsonHelper.ToObject<List<EffectConfig>>(effectJson);

    //    //var phyJson = JsonHelper.ToJson(ZYPhysicsConfigData.all.Values.ToList());
    //    //phy.data = JsonHelper.ToObject<List<PhysicsConfig>>(phyJson);

    //    var stateJson = JsonHelper.ToJson(ZYStateData.all.Values.ToList());
    //    state.data = JsonHelper.ToObject<List<StateEntity>>(stateJson);

    //    UnityEditor.EditorUtility.SetDirty(anm);
    //    UnityEditor.EditorUtility.SetDirty(anmSpeed);
    //    UnityEditor.EditorUtility.SetDirty(audio);
    //    UnityEditor.EditorUtility.SetDirty(effect);
    //    UnityEditor.EditorUtility.SetDirty(phy);
    //    UnityEditor.EditorUtility.SetDirty(state);
    //    UnityEditor.AssetDatabase.SaveAssets();
    //}
}
