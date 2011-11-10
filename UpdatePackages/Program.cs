using System;
using System.Collections.Generic;
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
                        
                    }
                    package.Add(line);
                }
                AddPackage(package, packages);
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
