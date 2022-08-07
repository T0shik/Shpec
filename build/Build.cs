using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.IO.FileSystemTasks;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Pack);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")] readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    AbsolutePath OutputDirectory => RootDirectory / ".output";
    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestDirectory => RootDirectory / "test";

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

    AbsolutePath PlaygroundDirectory => TestDirectory / "Playground";
    Target TestPlayground => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetRun(_ => _
                .SetProjectFile(PlaygroundDirectory)
                .SetConfiguration(Configuration)
            );
        });

    AbsolutePath TestControllersDirectory => TestDirectory / "Playground.Controllers.Test";
    Target TestControllers => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(_ => _
                .SetProjectFile(TestControllersDirectory)
                .SetConfiguration(Configuration)
            );
        });
    AbsolutePath PackagesDirectory => OutputDirectory / "pkg";
    Target Pack => _ => _
        .DependsOn(TestPlayground, TestControllers)
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetProject(RootDirectory)
                .SetConfiguration(Configuration)
                .SetNoBuild(SucceededTargets.Contains(Compile))
                .SetOutputDirectory(PackagesDirectory)
            );

            ReportSummary(_ => _
                .AddPair("Packages", PackagesDirectory.GlobFiles("*.nupkg").Count.ToString()));
        });
}