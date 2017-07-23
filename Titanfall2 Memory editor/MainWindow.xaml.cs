using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Titanfall2ModdingLibrary;
using Newtonsoft.Json;

namespace Titanfall2_Memory_editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<CheckBox> CheckBoxes = new List<CheckBox>();
        Dictionary<CheckBox, Mod> Mods = new Dictionary<CheckBox, Mod>();
        Modder ModLoader;
        public MainWindow()
        {
            InitializeComponent();

            foreach (var item in Directory.GetDirectories("Mods"))
            {
                //Gets the name of the end folder
                string OnlyName = System.IO.Path.GetFileName(item);
                if (File.Exists(item + @"\" + OnlyName + ".json"))
                {
                    string Json = File.ReadAllText(item + @"\" + OnlyName + ".json");
                    Mod M = JsonConvert.DeserializeObject<Mod>(Json);
                    AddCheckbox(M);
                }
            }
        }


        private async void Load_Click(object sender, RoutedEventArgs e)
        {
            ModLoader = new Modder();
            Load.IsEnabled = false;
            foreach (var item in CheckBoxes)
            {
                if(item.IsChecked.Value)
                    await Task.Factory.StartNew(() => WriteMod(Mods[item]));
            }
            System.Windows.Forms.MessageBox.Show("Finished loading");
            Load.IsEnabled = true;
        }

        private void AddCheckbox(Mod M)
        {
            CheckBox CB = new CheckBox();
            CB.Content = M.Name;
            CB.ToolTip = M.Description;

            CheckBoxes.Add(CB);
            Mods.Add(CB, M);
            Mods_to_load.Children.Add(CB);
        }


        /// <summary>
        /// Writes a mod into memory
        /// </summary>
        /// <param name="M"></param>
        /// <returns>The list of files that couldnt be written into memory</returns>
        private async Task<List<SingleFile>> WriteMod(Mod M)
        {
            List<SingleFile> Fail = new List<SingleFile>();
            foreach (var item in M.Files)
            {
                if (!WriteModFileIntoMemory(item,@"Mods\" + M.Directory + @"\"))
                    Fail.Add(item);
            }

            return Fail;
        }

        /// <summary>
        /// Writes a single modfile into memory
        /// </summary>
        /// <param name="SingleMod">A single mod file</param>
        /// <returns>returns true if successful</returns>
        private bool WriteModFileIntoMemory(SingleFile SingleModFile, string Path)
        {
            try
            {
                long Address = -1;
                if (SingleModFile.SQLiteFiles != null && SingleModFile.SQLiteFiles.Length > 0)
                {
                    foreach (var FileName in SingleModFile.SQLiteFiles)
                    {
                        List<Pointer> Pointers = SQLiteLoader.GetPointerListFromSQLiteFile(Path + SingleModFile.Directory + @"\" + FileName);
                        Address = ModLoader.TestPointers(Pointers.ToArray(), SingleModFile.ComparisonString);
                        if (Address != -1)
                            break;
                    }
                }

                if(Address == -1)
                {
                    Address = ModLoader.findAddress(Encoding.ASCII.GetBytes(SingleModFile.ComparisonString));
                }

                //Add the offset
                if (SingleModFile.AddressOffset != 0)
                    Address += SingleModFile.AddressOffset;

                if (Address != -1)
                {
                    ModLoader.WriteMemory(Address, File.ReadAllBytes(Path + SingleModFile.Directory + @"\" + SingleModFile.ReplacedCodeFile));
                    return true;
                }

                return false;

            }
            catch (Exception)
            {

                return false;
            }


        }


    }
}
