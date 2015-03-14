Console Application to write SVN Revision Number into AssemblyInfo

# Usage #

## Command Prompt ##
```
SVNRevToAssemblyInfo.exe dir "trunk folder path" file "assemblyinfo.cs path"
```
or
```
SVNRevToAssemblyInfo.exe file "assemblyinfo.cs path" dir "trunk folder path" 
```

### Example ###
SVNRevToAssemblyInfo.exe dir "H:\Users\Mihajlo\Documents\Visual Studio 2008\Projects\Google Code\ZScreen\trunk" file "H:\Users\Mihajlo\Documents\Visual Studio 2008\Projects\Google Code\ZScreen\trunk\ZScreen\Properties\AssemblyInfo.cs" > debug.txt

## Visual Studio Pre-build ##
```
cd $(ProjectDir)Tools
SVNRevToAssemblyInfo.exe dir "$(SolutionDir)" file "$(ProjectDir)Properties\AssemblyInfo.cs"
```