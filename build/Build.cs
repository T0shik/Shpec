using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.IO.FileSystemTasks;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Run);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")] readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    AbsolutePath OutputDirectory => RootDirectory / "output";
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestDirectory => RootDirectory / "test";
    AbsolutePath PlaygroundDirectory => TestDirectory / "Playground";

    Target Clean => _ => _
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("*/bin", "*/obj").ForEach(DeleteDirectory);
            TestDirectory.GlobDirectories("*/bin", "*/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
        {
            DotNetRestore(_ => _.SetProjectFile(RootDirectory));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(RootDirectory)
                .SetConfiguration(Configuration)
            );
        });

    Target TestPlayground => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetRun(_ => _
                .SetProjectFile(PlaygroundDirectory)
                .SetConfiguration(Configuration)
            );
        });

    Target Run => _ => _
        .DependsOn(TestPlayground)
        .Executes(() =>
        {
        });
}