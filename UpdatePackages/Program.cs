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

                        wc.DownloadFile(u, GetFilePath(fn));

                        //line = "FileName: " + u;
                    }
                    package.Add(line);
                }
                AddPackage(package, packages);
            }
            bool separate = false;
            using (var s = File.CreateText(GetFilePath("Packages")))
            {
                s.WriteLine("# Marmalade developer package listing.");
                s.WriteLine("# Generted by mdev v0.2.");
                s.WriteLine("# The format of this file is similar but not identical to that used by debian's apt.");

                foreach (var p in packages)
                {
                    if (separate)
                        s.WriteLine("");
                    else
                        separate = true;
                    foreach (var line in p)
                    {
                        s.WriteLine(line);
                    }
                }
                s.Close();
            }
        }

        private static string GetFilePath(string s)
        {
            return Path.Combine(@"..\..\..\repo",s);
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
                           "https://raw.github.com/marmalade/pugixml/master/Packages",
"https://raw.github.com/marmalade/Freetype/master/Packages",
"https://raw.github.com/marmalade/dpi/master/Packages",
"https://raw.github.com/gamemaster101gr/libcurl/master/Packages",

                       };
        }
    }
}
