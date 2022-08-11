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

            string engineFilename = "";

            if(args.Length != 0)
            {
                if(args[0].StartsWith("enginette-client://"))
                {
                    Debug.Log("Program", "Program opened using website!");
                    Debug.Log("Program", "Resolving arguments...");

                    string arguments = args[0].Remove(0, 19);
                    string[] argumentList = arguments.Split(';');
                    foreach(string arg in argumentList)
                    {
                        if(arg.StartsWith("file-name"))
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
            Program.Exit(0);
        }

        public static void CheckSim()
        {
            Debug.Log("Sim Launcher", "Checking Sim...");
            if(Program.settings.SimLocation == "" || Program.settings.SimExeLocation == "")
            {
                Debug.Log("Sim Launcher", "Sim not set asking user...");

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
            }
        }

        public static void GetPiranha(string filename)
        {
            Debug.Log("Sim Launcher", "Getting piranha scripts...");
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
            File.WriteAllLines("logs.txt", logs);
        }
    }
}
