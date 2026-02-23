
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Threading.Tasks;

namespace CoreORM;

public class ORMConfigFindReplace
{
    public string FindRegEx { get; set; }
    public string Replace { get; set; }
}
public class ORMProcess
{
    public string ProcessExec { get; set; }
    public string ProcessArgs { get; set; }
    public string ProcessWorkingDir { get; set; }
}
public class ORMConfig
{
    public bool IsDefault { get; set; }
    public string ConfigName { get; set; }
    public string ConnectionSource { get; set; }
    public string SourceName { get; set; }

    public string SourceType { get; set; }

    public string DirOutDir { get; set; }
    public string NameSpace { get; set; }
    public string ViewsDirectory { get; set; }
    public List<ORMConfigFindReplace> RegExReplace { get; set; }
    public List<ORMProcess> PostProcess { get; set; }
    public List<ORMProcess> PreProcess { get; set; }
    public List<ORMView> Views { get; set; }

}

public class ORMView
{
    public string ViewFileName { get; set; }
    public string ViewOutputFilePath { get; set; }
    public string ViewParams { get; set; }
    public string ViewNameSpace { get; set; }
    public List<ORMProcess> ViewPostProcess { get; set; }
}
