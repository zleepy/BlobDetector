using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Trainer
{
    public class ReportProgressArgs : EventArgs
    {
        public string Message { get; set; }
        public string Directory { get; set; }
        public int TotalFileCount { get; set; }
        public int CompletedFileCount { get; set; }
        public int NextFile { get; set; }
    }

    /// <summary>
    /// Letar reda på alla xml-filer i katalogen och underkataloger. I en xml-fil kan det finnas 
    /// länkar till en eller flera bilder med original att initiera databasen med. 
    /// Man bör skapa en underkatalog för varje font.
    /// </summary>
    public class Trainer
    {
        public event EventHandler<ReportProgressArgs> ReportProgress;

        private void OnReportProgress(ReportProgressArgs args)
        {
            if (ReportProgress != null)
                ReportProgress(this, args);
        }

        public string Root { get; private set; }

        internal void Run(Arguments args)
        {
            Root = args.Root;

            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Hittar alla filer med sanningar som finns i en katalog eller dess underkataloger.
    /// </summary>
    public class TrainingSetLocator
    {
        private bool verbose;
        private string root;
        
        public TrainingSetLocator(string rootPath, bool verboseOutput)
        {
            this.verbose = verboseOutput;
        }

        //private string FindXmlFiles();
    }

    // 
    // <TrainingSet>
    //  <FontName>Generic</FontName>
    //  <FontFamily>Generic</FontFamily>
    //  <Subsets>
    //      <Subset MinimumCharacterDistance="10">
    //          <Attributes>
    //              <Attribute>bold</Attribute>
    //          </Attributes>
    //          <ImagePath>generic</ImagePath>
    //          <Truth>
    //              ,.;:-~_'´`*/\*-+?!()[]{}<>=&%$£@
    //              abcdefghijklmnopqrstuvwxyzåäö
    //              ABCDEFGHIJKLMNOPQRSTUVWXYZÅÄÖ
    //          </Truth>
    //      </Subset>
    //  <Subsets>
    // <TrainingSet>
    //
    [Serializable]
    public class TrainingSet
    {
        /// <summary>
        /// Fontens fulla namn, är det en generell sanning så namnge den "generic".
        /// </summary>
        public string FontName { get; set; }

        /// <summary>
        /// Fontens familje namn. Om den inte har något så sätt det till samma som fontens namn. 
        /// Är det en generell sanning så namnge den "generic".
        /// </summary>
        public string FontFamily { get; set; }
        
        [XmlElement("Subset")]
        public List<TrainingSubSet> Subsets { get; set; }
    }

    [Serializable]
    public class TrainingSubSet
    {
        /// <summary>
        /// Minsta avstånd som det måste vara mellan tecken i bilden för att dom 
        /// inte ska slås samman och räknas som ett enda tecken.
        /// </summary>
        [XmlAttribute]
        public int MinimumCharacterDistance { get; set; }

        /// <summary>
        /// Namn på de attribut som den här sanningen gäller för. 
        /// Sanningar med samma attribut slås samman.
        /// </summary>
        [XmlArray(IsNullable=true), XmlElement("Attribute")]
        public string[] Attributes { get; set; }

        /// <summary>
        /// Absolut eller relativ sökväg till bilden som innehåller sanningarna i bildformat.
        /// </summary>
        public string ImagePath { get; set; }

        /// <summary>
        /// Facit till sanningarna i bilden. Tecknen tolkas rad för rad. Vita tecken ignoreras.
        /// </summary>
        public string Truth { get; set; }
    }
}
