using Guts.DirectoryWatcher;
using Refit;

var serverAddress = Environment.GetEnvironmentVariable("GUTS_SERVER_ADDRESS") ?? 
                    throw new("GUTS_SERVER_ADDRESS environment variable was not found");
var client = RestService.For<IGutsServerClient>(serverAddress);

var dumpFilesMask = Environment.GetEnvironmentVariable("GUTS_DUMPS_MASK") ?? "*";
var directory = Environment.GetEnvironmentVariable("GUTS_DUMPS_DIRECTORY") ??
                throw new("GUTS_DUMPS_DIRECTORY environment variable was not found");
var dumpsWatcher = new DumpsFileSystemWatcher(directory, dumpFilesMask);
var changes = dumpsWatcher.CreateChangesChannel();

var dumpsEventsWatcher = new DumpsEventsWatcher(changes, client);
await dumpsEventsWatcher.Start();
