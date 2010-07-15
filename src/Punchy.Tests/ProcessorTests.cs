using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Punchy.Configuration;
using System.Configuration;
using System.IO;
using System.Runtime.Remoting.Messaging;

namespace Punchy.Tests
{
    /// <summary>
    /// Summary description for ProcessorTests
    /// </summary>
    [TestClass]
    public class ProcessorTests
    {
        private Processor processor;
        private PunchyConfigurationSection config = (PunchyConfigurationSection)ConfigurationManager.GetSection("punchy");
        private string tempFolder;

        public ProcessorTests()
        {
        }

        [TestInitialize]
        public void Setup()
        {
            this.tempFolder = Path.Combine(config.OutputPhysicalPath, "tmp");
            if(Directory.Exists(this.tempFolder))
                Directory.Delete(this.tempFolder, true);

            this.processor = new Processor();
        }

        [TestMethod]
        public void Revision_between_processor_instances_are_equal()
        {
            File.Delete(Path.Combine(config.OutputPhysicalPath, "app-min.js"));
            File.Delete(Path.Combine(config.OutputPhysicalPath, "css-min.css"));

            var resource1 = processor.GetResourceFor("app-min.js");
            var resource2 = processor.GetResourceFor("app-min.js");

            Processor processor2 = new Processor();
            var resource3 = processor2.GetResourceFor("app-min.js");

            Assert.AreEqual(resource1, resource2);
            Assert.AreEqual(resource1, resource3);

            Console.WriteLine(resource1);
        }

        [TestMethod]
        public void Processor_processes_js()
        {
            string file = this.processor.GetResourceFor("app-min.js");
            Assert.IsNotNull(file);
        }

        [TestMethod]
        [ExpectedException(typeof(BundleException))]
        public void Invalid_css_throw_bundleexception()
        {
            this.processor.GetResourceFor("a-bundle-that-doesnt-exist.js");
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
