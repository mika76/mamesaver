using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Mamesaver.Configuration.Models;
using Mamesaver.Test.Unit;
using NUnit.Framework;

namespace Mamesaver.Test.Integration
{
    [TestFixture]
    [TestOf(typeof(MamePathManager))]
    public class MamePathManagerTests : MamesaverTests
    {
        private MamePathManager _manager;
        private string _mamePath;

        [SetUp]
        public void SetUp()
        {
            
            _manager = GetInstance<MamePathManager>();

            var settings = GetInstance<Settings>();
            _mamePath = Directory.GetParent(settings.ExecutablePath).ToString();
        }

        [Test]
        public void NotMatchingConfigLine()
        {
            _manager.ExtractConfigPaths("rompath", "bananas foo bar").Should().BeNull();
        }

        [Test]
        public void MatchingConfigLine()
        {
            var directories = _manager.ExtractConfigPaths("rompath", "rompath roms");
            directories.Should().BeEquivalentTo(new List<string> { Path.Combine(_mamePath, "roms") });
        }

        [TestCase("rompath vector;shmups")]
        [TestCase("rompath vector;  shmups")]
        [TestCase("rompath          vector;shmups")]
        [Description("Verifies extraction of multiple paths")]
        public void MatchingConfigLineMultiplePaths(string line)
        {
            var directories = _manager.ExtractConfigPaths("rompath", line);
            directories.Should().BeEquivalentTo(new List<string>
            {
                Path.Combine(_mamePath, "vector"),
                Path.Combine(_mamePath, "shmups")
            });
        }

        [Test]
        [Description("Verifies that absolute paths are unchanged")]
        public void MatchingConfigLineMixedAbsoluteRelative()
        {
            var directories = _manager.ExtractConfigPaths("rompath", @"rompath vector;c:\roms\shmups");
            directories.Should().BeEquivalentTo(new List<string>
            {
                Path.Combine(_mamePath, "vector"),
                @"c:\roms\shmups"
            });
        }

        [Test]
        [Description("Verifies that absolute paths are unchanged")]
        public void MatchingConfigLineAbsolutePaths()
        {
           var directories = _manager.ExtractConfigPaths("rompath", @"rompath c:\roms\vector;c:\roms\shmups");
            directories.Should().BeEquivalentTo(new List<string>
            {
                @"c:\roms\vector",
                @"c:\roms\shmups"
            });
        } 

        [Test]
        [Description("Verifies that enclosing speech marks are removed from directory paths")]
        public void MatchingConfigLineWithQuotes()
        {
            var directories = _manager.ExtractConfigPaths("rompath", @"rompath ""shmups and things""");
            directories.Should().BeEquivalentTo(new List<string>
            {
                Path.Combine(_mamePath, "shmups and things")
            });
        }

        [Test]
        public void MatchingSinglePathWithQuotes()
        {
            var directories = _manager.ExtractConfigPaths("rompath", @"rompath vector;""shmups and things""");
            directories.Should().BeEquivalentTo(new List<string>
            {
                Path.Combine(_mamePath, "vector"),
                Path.Combine(_mamePath, "shmups and things")
            });
        }

        [Test]
        public void MatchingConfigLineMultiplePathsWithQuotes()
        {
            var directories = _manager.ExtractConfigPaths("rompath", @"rompath ""vector;shmups and things""");
            directories.Should().BeEquivalentTo(new List<string>
            {
                Path.Combine(_mamePath, "vector"),
                Path.Combine(_mamePath, "shmups and things")
            });
 
        }
    }
}
