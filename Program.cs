using System;
using System.IO;
//using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using Ookii.Dialogs.WinForms;
using Newtonsoft.Json;

namespace EnginetteClient
{
    public class Program
    {
        public static Settings settings;
        public static string appdata = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\enginette-client";
        public static string settingsLocation = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\enginette-client\\settings.json";

        public static void Main(string[] args)
        {
            Environment.CurrentDirectory = Microsoft.Win32.Registry.GetValue("HKEY_CLASSES_ROOT\\enginette-client", "Directory", null).ToString();
            Debug.Log("Program", "Program opened with " + args.Length + " argument(s).");
            Debug.Log("Program", "Loading settings...");

            if(File.Exists(settingsLocation))
            {
                settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(settingsLocation));
                Debug.Log("Program", "Settings Loaded.");
            }
            else
            {
                Debug.Log("Program", "Settings not found.");
                settings = new Settings();
                settings.SimExeLocation = "";
                settings.SimLocation = "";
                SaveSettings();
            }

            // resolve args
            // assuming we usin proto
            // enginette-client://url-engine=blah blah;url-camshaft=blah blah;
            // enginette-client://filename=E:\projects\enginette-client\bin\Debug\net48\lawnmower.mr

            string engineFilename = "";

            if(args.Length != 0)
            {
                if(args[0].StartsWith("enginette-client://"))
                {
                    Debug.Log("Program", "Program opened using website!");
                    Debug.Log("Program", "Resolving arguments...");
                    Debug.Log("Program", "Main arg:" + args[0]);

                    string arguments = args[0].Remove(0, 19);
                    arguments = arguments.Remove(arguments.Length - 1, 1);
                    arguments = arguments.Replace("%20", " ");
                    arguments = arguments.Replace("%5C", "\\");
                    string[] argumentList = arguments.Split(';');
                    foreach(string arg in argumentList)
                    {
                        if(arg.StartsWith("filename"))
                        {
                            engineFilename = arg.Split('=')[1];
                        }
                    }

                    SimLauncher.LaunchSim(engineFilename);
                }
                else
                {
                    Debug.Log("Program", "Nothing to do, exiting...");
                    Exit(0);
                }
            }
            else
            {
                Debug.Log("Program", "Nothing to do, exiting...");
                Exit(0);
            }
        }

        public static void Exit(int code)
        {
            Debug.Save();
            Environment.Exit(code);
        }

        public static void SaveSettings()
        {
            if(!Directory.Exists(appdata))
                Directory.CreateDirectory(appdata);
            
            string result = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(settingsLocation, result);
        }
    }

    public class Settings 
    {
        [JsonProperty("simLocation")]
        public string SimLocation { get; set; }
        [JsonProperty("simExeLocation")]
        public string SimExeLocation { get; set; }
    }

    public class SimLauncher
    {
        public static void LaunchSim(string filename)
        {
            CheckSim();
            GetPiranha(filename);
            Launch();
            Program.Exit(0);
        }

        public static void Launch()
        {
            using (System.Diagnostics.Process pro = new System.Diagnostics.Process())
            {
                pro.StartInfo.FileName = Program.settings.SimExeLocation;
                pro.StartInfo.WorkingDirectory = Program.settings.SimLocation;
                pro.StartInfo.UseShellExecute = true;

                pro.Start();
            }
        }

        public static void CheckSim()
        {
            Debug.Log("Sim Launcher", "Checking Sim...");
            if(Program.settings.SimLocation == "" || Program.settings.SimExeLocation == "")
            {
                Debug.Log("Sim Launcher", "Sim not set, asking user...");

                /*
                VistaOpenFileDialog fileDialog = new VistaOpenFileDialog();
                fileDialog.Multiselect = false;
                if(fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Program.settings.SimExeLocation = fileDialog.FileName;
                    Program.settings.SimLocation = Path.GetDirectoryName(fileDialog.FileName);
                    Debug.Log("Sim Launcher", "User confirmed :>");
                    Debug.Log("Sim Launcher", "Saving settings...");
                    Program.SaveSettings();
                }
                else
                {
                    Debug.Log("Sim Launcher", "User clicked cancel :<");
                    Program.Exit(1);
                }
                */

                Console.Write("Please insert sim directory: ");
                string simPath = Console.ReadLine();

                if(File.Exists(simPath))
                {
                    Program.settings.SimExeLocation = simPath;
                    Program.settings.SimLocation = Path.GetDirectoryName(simPath);
                    Debug.Log("Sim Launcher", "User inserted right settings :>");
                    Debug.Log("Sim Launcher", "Saving settings...");
                    Program.SaveSettings();
                }
                else
                {
                    Debug.Log("Sim Launcher", "User didn't insert correct settings :<");
                    Program.Exit(1);
                }
            }
            else
            {
                Debug.Log("Sim Launcher", "Found Sim. Directory: " + Program.settings.SimExeLocation);
            }
        }

        public static void GetPiranha(string filename)
        {
            Debug.Log("Sim Launcher", "Getting piranha script...");
            string piranha = File.ReadAllText(filename);
            Debug.Log("Sim Launcher", "Read piranha script. Source: " + filename);

            string nodeName = Path.GetFileNameWithoutExtension(filename);
            Debug.Log("Sim Launcher", "Node name: " + nodeName);

            // write engine to engines.mr
            if(!File.ReadAllText(Program.settings.SimLocation + "\\..\\assets\\part-library\\engines\\engines.mr").Contains("public import \"" + nodeName + "\""))
                File.AppendAllLines(Program.settings.SimLocation + "\\..\\assets\\part-library\\engines\\engines.mr", new string[] { "public import \"" + nodeName + "\"" });
            Debug.Log("Sim Launcher", "Written engine import to engines.mr");

            // write engine to \\assets\\part-library\\engines\\filename.mr
            File.WriteAllText(Program.settings.SimLocation + $"\\..\\assets\\part-library\\engines\\{nodeName}.mr", piranha);
            Debug.Log("Sim Launcher", $"Written engine to {nodeName}.mr");

            // write test.mr
            string result = "import \"engine_sim.mr\"\n" +
                            "import \"part-library/part_library.mr\"\n" +
                            "import \"video-scripts/454-tuning/engine_04.mr\"\n" +
                            "\n" +
                            "set_engine(\n" +
                           $"    engine: {nodeName}()\n" +
                            ")\n";

            File.WriteAllText(Program.settings.SimLocation + $"\\..\\assets\\test.mr", result);
            Debug.Log("Sim Launcher", $"Written test to test.mr");
        }
    }
    
    public class Debug
    {
        public static List<string> logs = new List<string>();

        public static void Log(string source, string msg)
        {
            logs.Add($"[{source}]" + " [ LOG ] " + msg);
        }

        public static void LogWarning(string source, string msg)
        {
            logs.Add($"[{source}]" + " [ WARNING ] " + msg);
        }

        public static void LogError(string source, string msg)
        {
            logs.Add($"[{source}]" + " [ ERROR ] " + msg);
        }

        public static void Save()
        {
            logs.Add("SAVING LOGS...");
            File.WriteAllLines(Program.appdata + "\\logs.txt", logs);
        }
    }
}
