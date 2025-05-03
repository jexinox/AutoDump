using Guts.DirectoryWatcher;
using Refit;

var dumpFilesMask = Environment.GetEnvironmentVariable("GUTS_DUMPS_MASK") ?? "*";
var directory = Environment.GetEnvironmentVariable("GUTS_DUMPS_DIRECTORY");
if (directory is null)
{
    throw new("GUTS_DUMPS_DIRECTORY environment variable was not found");
}

var dumpsWatcher = new DumpsFileSystemWatcher(directory, dumpFilesMask);



