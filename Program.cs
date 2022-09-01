using System;
using System.IO;
//using System.Diagnostics;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EnginetteClient
{
    public class Theme 
    {
        public string StartFullscreen { get; set; }
        public string SpeedUnits { get; set; }
        public string PressureUnits { get; set; }
        public string TorqueUnits { get; set; }
        public string PowerUnits { get; set; }

        public Theme() 
        {
            StartFullscreen = "false";
            SpeedUnits = "mph";
            PressureUnits = "inHg";
            TorqueUnits = "lb_ft";
            PowerUnits = "hp";
        }

        public override string ToString()
        {
            string result = "";

            result += "import \"engine_sim.mr\"" + "\n";
            result += "\n";
            result += "unit_names units()" + "\n";
            result += "public node use_default_theme {" + "\n";
            result += "    input start_fullscreen: " + StartFullscreen + ";" + "\n";
            result += "    input speed_units: units." + SpeedUnits + ";" + "\n";
            result += "    input pressure_units: units." + PressureUnits + ";" + "\n";
            result += "    input torque_units: units." + TorqueUnits + ";" + "\n";
            result += "    input power_units: units." + PowerUnits + ";" + "\n";
            result += "\n";
            result += "    set_application_settings(" + "\n";
            result += "        start_fullscreen: start_fullscreen," + "\n";
            result += "        speed_units: speed_units," + "\n";
            result += "        pressure_units: pressure_units," + "\n";
            result += "        torque_units: torque_units," + "\n";
            result += "        power_units: power_units," + "\n";
            result += "\n";
            result += "        // Default Color Settings" + "\n";
            result += "        color_background: 0x0E1012," + "\n";
            result += "        color_foreground: 0xFFFFFF," + "\n";
            result += "        color_shadow: 0x0E1012," + "\n";
            result += "        color_highlight1: 0xEF4545," + "\n";
            result += "        color_highlight2: 0xFFFFFF," + "\n";
            result += "        color_pink: 0xF394BE," + "\n";
            result += "        color_red: 0xEE4445," + "\n";
            result += "        color_orange: 0xF4802A," + "\n";
            result += "        color_yellow: 0xFDBD2E," + "\n";
            result += "        color_blue: 0x77CEE0," + "\n";
            result += "        color_green: 0xBDD869" + "\n";
            result += "    )" + "\n";
            result += "}" + "\n";


            return result;
        }
    
        public bool IsDefault()
        {
            bool result = !(StartFullscreen == "false") && !(SpeedUnits == "mph") && !(PressureUnits == "inHg") && !(TorqueUnits == "lb_ft") && !(PowerUnits == "hp");
            return result;
        }
    }

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
            // enginette-client://reset
            // enginette-client://filename=E:\projects\enginette-client\bin\Debug\net48\lawnmower.mr
            // enginette-client://filename=E:\projects\enginette-client\bin\Debug\net48\lawnmower.mr;theme=default
            // enginette-client://filename=E:\projects\enginette-client\bin\Debug\net48\lawnmower.mr;theme.something=value

            string engineFilename = "";
            string themeName = "";

            bool setupTheme = false;

            string theme_start_fullscreen = "false";
            string theme_speed_units = "mph";
            string theme_pressure_units = "inHg";
            string theme_torque_units = "lb_ft";
            string theme_power_units = "hp";

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
                        if(arg.StartsWith("reset"))
                        {
                            Debug.Log("Program", "Resetting settings...");
                            settings = new Settings();
                            settings.SimExeLocation = "";
                            settings.SimLocation = "";
                            SaveSettings();
                            return;
                        }

                        if(arg.StartsWith("filename"))
                        {
                            engineFilename = arg.Split('=')[1];
                            Debug.Log("Program", "Engine filename set to:" + engineFilename);
                            return;
                        }

                        if(arg.StartsWith("theme"))
                        {
                            themeName = arg.Split('=')[1];
                            Debug.Log("Program", "Theme name set to:" + themeName);
                            return;
                        }

                        if(arg.StartsWith("theme.start_fullscreen"))
                        {
                            setupTheme = true;
                            theme_start_fullscreen = arg.Split('=')[1];
                            Debug.Log("Program", "Theme name set to:" + theme_start_fullscreen);
                            return;
                        }

                        if(arg.StartsWith("theme.speed_units"))
                        {
                            setupTheme = true;
                            theme_speed_units = arg.Split('=')[1];
                            Debug.Log("Program", "Theme speed units set to:" + theme_speed_units);
                            return;
                        }

                        if(arg.StartsWith("theme.pressure_units"))
                        {
                            setupTheme = true;
                            theme_pressure_units = arg.Split('=')[1];
                            Debug.Log("Program", "Theme pressure units set to:" + theme_pressure_units);
                            return;
                        }

                        if(arg.StartsWith("theme.torque_units"))
                        {
                            setupTheme = true;
                            theme_torque_units = arg.Split('=')[1];
                            Debug.Log("Program", "Theme torque units set to:" + theme_torque_units);
                            return;
                        }

                        if(arg.StartsWith("theme.power_units"))
                        {
                            setupTheme = true;
                            theme_power_units = arg.Split('=')[1];
                            Debug.Log("Program", "Theme power units set to:" + theme_power_units);
                            return;
                        }
                    }

                    Theme theme = new Theme();
                    theme.StartFullscreen = theme_start_fullscreen;
                    theme.StartFullscreen = theme_start_fullscreen;
                    theme.StartFullscreen = theme_start_fullscreen;
                    theme.StartFullscreen = theme_start_fullscreen;

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
            SaveSettings();
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
                Console.WriteLine("Important note: version 1.3a is NOT supported from now on (i dont wanna code two diffrent algos)");
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

        public static void GetPiranha(string filename, string themeName = "default", Theme theme = null)
        {
            Debug.Log("Sim Launcher", "Getting piranha script...");
            string piranha = File.ReadAllText(filename);
            Debug.Log("Sim Launcher", "Read piranha script. Source: " + filename);

            string nodeName = Path.GetFileNameWithoutExtension(filename);
            Debug.Log("Sim Launcher", "Node name: " + nodeName);

            // write engine to engines.mr
            //if(!File.ReadAllText(Program.settings.SimLocation + "\\..\\assets\\part-library\\engines\\engines.mr").Contains("public import \"" + nodeName + "\""))
            //    File.AppendAllLines(Program.settings.SimLocation + "\\..\\assets\\part-library\\engines\\engines.mr", new string[] { "public import \"" + nodeName + "\"" });
            //Debug.Log("Sim Launcher", "Written engine import to engines.mr");

            // write engine to \\assets\\engines\\custom\\filename.mr
            if(!Directory.Exists(Program.settings.SimLocation + $"\\..\\assets\\engines\\custom"))
                Directory.CreateDirectory(Program.settings.SimLocation + $"\\..\\assets\\engines\\custom");
            File.WriteAllText(Program.settings.SimLocation + $"\\..\\assets\\engines\\custom\\{nodeName}.mr", piranha);
            Debug.Log("Sim Launcher", $"Written engine to custom/{nodeName}.mr");

            if(theme != null) {
                File.WriteAllText(Program.settings.SimLocation + $"\\..\\assets\\themes\\enginette.mr", theme.ToString());
                Debug.Log("Sim Launcher", $"Written theme to themes/enginette.mr");
                themeName = "enginette";
            }

            // write main.mr
            string result = "import \"engine_sim.mr\"\n" +
                            //"import \"part-library/part_library.mr\"\n" +
                            //"import \"engines/kohler/kohler_ch750.mr\"\n" +
                           $"import \"engines/custom/{nodeName}.mr\"\n" +
                           $"import \"themes/{themeName}.mr\"\n" +
                            "\n" +
                           $"use_{themeName}_theme()\n" +
                            "set_engine(\n" +
                           $"    engine: {nodeName}()\n" +
                            ")\n";

            File.WriteAllText(Program.settings.SimLocation + $"\\..\\assets\\main.mr", result);
            Debug.Log("Sim Launcher", $"Written main to main.mr");
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
