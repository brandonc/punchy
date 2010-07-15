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
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ConfigurationTests
    {
        private PunchyConfigurationSection config = (PunchyConfigurationSection)ConfigurationManager.GetSection("punchy");

        [TestMethod]
        public void Configuration_has_three_toolchains()
        {
            Assert.AreEqual(3, config.Toolchains.Count);
        }

        [TestMethod]
        public void Configuration_output_directory_is_physical()
        {
            Assert.AreEqual(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"static\cache"), config.OutputPhysicalPath);
        }

        [TestMethod]
        public void Configuration_has_two_bundles()
        {
            Assert.AreEqual(2, config.Bundles.Count);
        }

        [TestMethod]
        public void Configuration_bundle_1_is_javascript()
        {
            Assert.AreEqual("text/javascript", config.Bundles[0].MimeType);
        }

        [TestMethod]
        public void Configuration_bundle_2_is_css()
        {
            Assert.AreEqual("text/css", config.Bundles[1].MimeType);
        }

        [TestMethod]
        public void Configuration_bundle_2_has_3_files()
        {
            Assert.AreEqual(3, config.Bundles[1].Files.Count);
        }
    }
}
