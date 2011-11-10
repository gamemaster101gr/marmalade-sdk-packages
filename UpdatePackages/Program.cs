using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace UpdatePackages
{
    class Program
    {
        static void Main(string[] args)
        {
            var packages = new List<List<string>>();
            List<string> package = null;

            var urls = GetUrls();
            WebClient wc = new WebClient();
            foreach (var url in urls)
            {
                var uri = new Uri(url);
                package = new List<string>();
                
                foreach (var str in wc.DownloadString(url).Split(new char[]{'\n'}))
                {
                    var line = str.Trim(new char[] {'\r'});
                    if (line.StartsWith("#"))
                        continue;
                 
                    if (string.IsNullOrEmpty(line))
                    {
                        AddPackage(package, packages);
                        package = new List<string>();
                        continue;
                    }
                    if (line.StartsWith("FileName:", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var fn = line.Split(new char[]{':'},2)[1];
                        fn = fn.Trim();
                        var u = new Uri(uri, fn);
                        line = "FileName: " + u;
                    }
                    package.Add(line);
                }
                AddPackage(package, packages);
            }
            using (var s = File.CreateText(@"..\..\..\repo\Packages"))
            {
                foreach (var p in packages)
                {
                    foreach (var line in p)
                    {
                        s.WriteLine(line);
                    }
                    s.WriteLine("");
                }
                s.Close();
            }
        }

        private static void AddPackage(List<string> package, List<List<string>> packages)
        {
            if (package != null)
            {
                if (package.Count != 0)
                {
                    packages.Add(package);
                }
            }
        }

        private static IEnumerable<string> GetUrls()
        {
            return new[]
                       {
                           "https://raw.github.com/marmalade/simplemenu/master/Packages",
                           "https://raw.github.com/marmalade/Freetype/master/Packages",
                       };
        }
    }
}
