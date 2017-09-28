using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexAnalyse
{
    internal class Program
    {
        class FlowRecord
        {
            public string Origin { get; set; }
            public string Destination { get; set; }
            public string Route { get; set; }
            public string Toc { get; set; }
            public int Flowid { get; set; }
        }
        private static void Main(string[] args)
        {
            try
            {
                var filename = @"s:\RJFAF614.FFL";

                var hexNlcSet = new HashSet<string> { "H584", "7090", "7091", "9846" };
                var hexFlowList = new List<FlowRecord>();
                foreach (var line in File.ReadLines(filename))
                {
                    if (line.Length > 0 && line[0] == '/')
                    {
                        continue; 
                    }
                    if (line.Length == 49 && line[0] == 'R' && line[1] == 'F')
                    {
                        var orig = line.Substring(2, 4);
                        var dest = line.Substring(6, 4);
                        var route = line.Substring(10, 5);

                        if ((hexNlcSet.Contains(orig) || hexNlcSet.Contains(dest)) && route != "00128")
                        {
                            var flow = new FlowRecord
                            {
                                Origin = orig,
                                Destination = dest,
                                Route = line.Substring(10, 5),
                                Toc = line.Substring(36, 3),
                                Flowid = Convert.ToInt32(line.Substring(42, 7))
                            };
                            hexFlowList.Add(flow);
                        }
                    }
                }
                var tocdic = hexFlowList.GroupBy(x => x.Toc).ToDictionary(y => y.Key, y=>y.ToList());
                var destdic = hexFlowList.GroupBy(x => hexNlcSet.Contains(x.Origin) ? x.Destination : x.Origin).ToDictionary(y => y.Key, y => y.ToList());
                var routedic = hexFlowList.GroupBy(x=>x.Route).ToDictionary(y => y.Key, y => y.ToList());
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                var codeBase = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                var progname = Path.GetFileNameWithoutExtension(codeBase);
                Console.Error.WriteLine(progname + ": Error: " + ex.Message);
            }

        }
    }
}
