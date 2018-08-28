# NetStandardUnitTestProblem
This project demonstrates a problem consuming .NET Standard 2.0 libraries from a C# Unit Test project.

If the library targeting .NET Standard attempts to access ConfigurationManager.AppSettings, it fails with a PlatformNotSupportedException:

    at System.Configuration.ClientConfigPaths..ctor(String exePath, Boolean includeUserConfig)
       at System.Configuration.ClientConfigPaths.GetPaths(String exePath, Boolean includeUserConfig)
       at System.Configuration.ClientConfigurationHost.get_ConfigPaths()
       at System.Configuration.ClientConfigurationHost.GetStreamName(String configPath)
       at System.Configuration.ClientConfigurationHost.get_IsAppConfigHttp()
       at System.Configuration.Internal.DelegatingConfigHost.get_IsAppConfigHttp()
       at System.Configuration.ClientConfigurationSystem..ctor()
       at System.Configuration.ConfigurationManager.EnsureConfigurationSystem()
    --- End of inner exception stack trace ---
        at System.Configuration.ConfigurationManager.EnsureConfigurationSystem()
       at System.Configuration.ConfigurationManager.PrepareConfigSystem()
       at System.Configuration.ConfigurationManager.GetSection(String sectionName)
       at System.Configuration.ConfigurationManager.get_AppSettings()
       at NetStandardTest.Test.DoSomething()
       at TestNetStandardUnitTest.UnitTest1.TestMethod1()
    Result Message:       
    Test method TestNetStandardUnitTest.UnitTest1.TestMethod1 threw exception: 
    System.Configuration.ConfigurationErrorsException: Configuration system failed to initialize ---> System.PlatformNotSupportedException: Operation is not supported on this platform.

To reproduce the issue, simply build the solution in Visual Studio and then execute the tests (Test | Run | All Tests).

Or from the command line:

    msbuild .\TestNetStandardUnitTest.sln
    vstest.console .\TestNetStandardUnitTest\bin\Debug\TestNetStandardUnitTest.dll