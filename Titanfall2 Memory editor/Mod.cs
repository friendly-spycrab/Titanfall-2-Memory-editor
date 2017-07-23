using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Titanfall2_Memory_editor
{
    /// <summary>
    /// mod data struct. is decoded from json using newtonsoft json.
    /// </summary>
    public struct Mod
    {
        /// <summary>
        /// Contains list of all files to replace
        /// </summary>
        public SingleFile[] Files;


        /// <summary>
        /// Describes what the mod does
        /// </summary>
        public string Description;

        /// <summary>
        /// The name of the mod
        /// </summary>
        public string Name;


        /// <summary>
        /// Contains the directory name
        /// </summary>
        public string Directory;
    }



    /// <summary>
    /// Contains data for replacing a single file in memory
    /// </summary>
    public struct SingleFile
    {
        /// <summary>
        /// paths relative to directory string. Contains pointer addresses found using the cheat engine pointerscan format
        /// </summary>
        public string[] SQLiteFiles;

        /// <summary>
        /// This is to check that we have found the correct memory location, So we do not override random memory
        /// </summary>
        public string ComparisonString;

        /// <summary>
        /// This is the filename of the code we want to replace. this is a relative path to the directory string. the file must not be bigger than the one it is replaceing.
        /// </summary>
        public string ReplacedCodeFile;

        /// <summary>
        /// The base directory where the sqlite and replacecode files are
        /// </summary>
        public string Directory;
    }
}
