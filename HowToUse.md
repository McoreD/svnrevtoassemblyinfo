# Getting Started #

  * Copy the SVNRevToAssemblyInfo.exe to your desired location. It is assumed you have copied it to:
```
$(ProjectDir)Tools\SVNRevToAssemblyInfo.exe
```

# Editing the Project file #
  * Locate the PreBuildEvent in the project file
  * The following modification will update an AssemblyInfo.cs on the Release event
```
  <PropertyGroup Condition=" '$(Configuration)' == 'Release' ">
    <PreBuildEvent>"$(ProjectDir)Tools\SVNRevToAssemblyInfo.exe" dir "$(SolutionDir)" file "$(ProjectDir)Properties\AssemblyInfo.cs"</PreBuildEvent>
    <PostBuildEvent>mkdir "$(TargetDir)ILMerge"
"$(ProjectDir)Tools\ILMerge.exe" /wildcards /t:winexe /out:"$(TargetDir)ILMerge\ZScreen.exe" "$(TargetPath)" "$(TargetDir)*.dll"</PostBuildEvent>
  </PropertyGroup>
```

# Ensure correct SVN Revision is used in your application #
  * Code as usual. If using AnkhSVN the changed code will be displayed:
![http://i41.tinypic.com/30v2h51.png](http://i41.tinypic.com/30v2h51.png)
  * Once you are done with coding, click **Update**. This is to make sure the local .svn data will have the latest SVN Revision number. If you are using a different SVN Client then use the Update function in that program.
  * Perform a Release. For C# Project, it is possible to create a Batch build that will execute the Release configuration.
![http://i43.tinypic.com/fx841t.png](http://i43.tinypic.com/fx841t.png)
  * A successful Release configuration will display SVNRevToAssemblyInfo debug info in the Output
![http://i43.tinypic.com/eld3x1.png](http://i43.tinypic.com/eld3x1.png)
  * Upload the new build
  * Commit the code