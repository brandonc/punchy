using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Punchy.Configuration;
using System.Configuration;
using System.IO;

namespace Punchy.Tests
{
    /// <summary>
    /// Summary description for ProcessorTests
    /// </summary>
    [TestClass]
    public class ProcessorTests
    {
        private readonly Processor processor;
        private PunchyConfigurationSection config = (PunchyConfigurationSection)ConfigurationManager.GetSection("punchy");
        private readonly string tempFolder;

        public ProcessorTests()
        {
            this.processor = new Processor();
            this.tempFolder = Path.Combine(config.OutputPhysicalPath, "tmp");
        }

        [TestMethod]
        public void Processor_processes_js()
        {
            string file = this.processor.GetResourceFor("app-min.js");
            Assert.IsNotNull(file);
        }

        [TestMethod]
        public void Processor_processes_css()
        {
            string file = this.processor.GetResourceFor("css-min.css");
            Assert.IsNotNull(file);
        }

        [TestCleanup]
        public void Cleanup()
        {
            processor.Dispose();
        }
    }
}
