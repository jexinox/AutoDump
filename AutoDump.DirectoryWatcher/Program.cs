using AutoDump.DirectoryWatcher;
using Microsoft.Extensions.Logging;
using Refit;

var logger = LoggerFactory.Create(configure => configure.AddConsole());
var log = logger.CreateLogger<Program>();
var serverAddress = Environment.GetEnvironmentVariable("AUTODUMP_SERVER_ADDRESS") ?? 
                    throw new("AUTODUMP_SERVER_ADDRESS environment variable was not found");
log.LogInformation("Configured server to {serverAddress}", serverAddress);
var client = RestService.For<IAutoDumpServerClient>(serverAddress);

var dumpFilesMask = Environment.GetEnvironmentVariable("AUTODUMP_DUMPS_MASK") ?? "*";
var directory = Environment.GetEnvironmentVariable("AUTODUMP_DUMPS_DIRECTORY") ??
                throw new("AUTODUMP_DUMPS_DIRECTORY environment variable was not found");
var dumpsWatcher = new DumpsFileSystemWatcher(directory, dumpFilesMask, logger.CreateLogger<DumpsFileSystemWatcher>());
var changes = dumpsWatcher.CreateChangesChannel();

var dumpsEventsWatcher = new DumpsEventsWatcher(changes, client, logger.CreateLogger<DumpsEventsWatcher>());
await dumpsEventsWatcher.Start();
