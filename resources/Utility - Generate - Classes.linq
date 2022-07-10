<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Data.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.DirectoryServices.AccountManagement.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.DirectoryServices.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.IO.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.IO.FileSystem.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Linq.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Linq.Expressions.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Linq.Queryable.dll</Reference>
  <Namespace>System.DirectoryServices.AccountManagement</Namespace>
  <Namespace>System.IO</Namespace>
</Query>

void Main()
{
	try{
		string createdBy = "TS MOGANO";
		string dateCreated = DateTime.Now.ToString("yyyy-MM-dd");
		string directoryCurrent = Directory.GetCurrentDirectory();
		string directoryRoot = Directory.GetParent(directoryCurrent).ToString();
		string directoryResources = Path.Combine(directoryRoot, "resources");
		string directoryResourcesDocumentation = Path.Combine(directoryResources, "documentation");
		string directoryResourcesDocumentationDesignPatternsCsvFile = Path.Combine(directoryResourcesDocumentation, "software design patterns.csv");
		string[] dataDesignPatterns = File.ReadAllLines(directoryResourcesDocumentationDesignPatternsCsvFile);
		IEnumerable<DesignPattern> designPatterns = dataDesignPatterns.Skip(1).Select(line => {
			var items = line.Split(';');
			var name = String.Join(" ", GetElementAt(items, 0).Replace("-", " ").Split(' ').Select(nm => CapitalizeFirstLetter(nm).Trim())).Trim();
			var type = GetElementAt(items, 1);
			var description = GetElementAt(items, 2);
			return new DesignPattern(type, name, description);
		});
		string directorySrc = Path.Combine(directoryRoot, "src");
		string directoryLib = Path.Combine(directorySrc, "lib");
		string directoryTests = Path.Combine(directoryRoot, "tests");
		string directoryTestsMS = Path.Combine(directoryTests, "ms-test");
		string directoryTestsNUnit = Path.Combine(directoryTests, "nunit");
		string directoryTestsNUnitWithMoq = Path.Combine(directoryTests, "nunit-moq");
		
		directoryRoot.Dump();
		directoryResources.Dump();
		directoryResourcesDocumentation.Dump();
		directoryResourcesDocumentationDesignPatternsCsvFile.Dump();
		directorySrc.Dump();
		directoryLib.Dump();
		directoryTests.Dump();
		directoryTestsMS.Dump();
		directoryTestsNUnit.Dump();
		directoryTestsNUnitWithMoq.Dump();
		
		CreateDirectoryIfNotExists(directorySrc);
		CreateDirectoryIfNotExists(directoryLib);
		CreateDirectoryIfNotExists(directoryTests);
		CreateDirectoryIfNotExists(directoryTestsMS);
		CreateDirectoryIfNotExists(directoryTestsNUnit);
		CreateDirectoryIfNotExists(directoryTestsNUnitWithMoq);
		
		string solutionName = new DirectoryInfo(directoryRoot).Name;
		string solutionNameWithExtension = String.Format("{0}.sln", solutionName);
		string solutionPath = Path.Combine(directoryRoot, solutionNameWithExtension);
		
		CreateMSSolution(directoryRoot);
		
		foreach(var designPattern in designPatterns){
			string path = Path.Combine(directorySrc, designPattern.ProjectName);
			if(!System.IO.Directory.Exists(path)){
				CreateMSConsoleProject(path, "5.0");
			}
			path = Path.Combine(directoryTestsMS, designPattern.MsTestProjectName);
			if(!System.IO.Directory.Exists(path)){
				CreateMSTestProject(designPattern.MsTestClassName, path);
			}
		}
		
		CreateMSAddMultipleProjectsToSolution(solutionPath, Directory.GetFiles(directorySrc, "*.csproj", SearchOption.AllDirectories));
		CreateMSAddMultipleProjectsToSolution(solutionPath, Directory.GetFiles(directoryTests, "*.csproj", SearchOption.AllDirectories));
		
	}catch(Exception exception){
		exception.Dump();
	}finally{}
}

// Define other methods and classes here
public string SplitCamelCase(string str)
{
	return Regex.Replace( Regex.Replace( str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2" ), @"(\p{Ll})(\P{Ll})", "$1 $2" );
}
private string CapitalizeFirstLetter(string str) {
	str = (str ?? String.Empty);
	return str.Substring(0, 1).ToUpper() + str.Substring(1);
}
private string LowerFirstLetter(string str) {
	str = (str ?? String.Empty);
	return str.Substring(0, 1).ToLower() + str.Substring(1);
}
private string GetElementAt(string[] items, int index){
	return (items == null || (items != null && items.Length < 0 || !Enumerable.Range(0, items.Length).Contains(index))) ? String.Empty : items[index];
}
private void CleanAndDeleteDirectory(string path){
	if(System.IO.Directory.Exists(path)){
		Console.WriteLine("Clean and Delete Directory : {0}", path);
		System.IO.DirectoryInfo rootDir = new DirectoryInfo(path);
		foreach (FileInfo file in rootDir.EnumerateFiles())
		{
		    file.Delete(); 
		}
		foreach (DirectoryInfo dir in rootDir.EnumerateDirectories())
		{
		    CleanAndDeleteDirectory(dir.ToString());
		}
		rootDir.Delete(true);
	}
}
private void CleanDirectoryOnly(string path, string fileExtension = "*.*"){
	if(System.IO.Directory.Exists(path)){
		Console.WriteLine("Cleaning Directory : {0}", path);
		System.IO.DirectoryInfo rootDir = new DirectoryInfo(path);
		IEnumerable<FileInfo> files = rootDir.EnumerateFiles().Where(file => fileExtension.Equals("*.*") || file.Name.EndsWith(fileExtension));
		foreach (FileInfo file in files)
		{
		    file.Delete(); 
		}
		foreach (DirectoryInfo dir in rootDir.EnumerateDirectories())
		{
		    CleanAndDeleteDirectory(dir.ToString());
		}
	}
}
private static IEnumerable<FileInfo> GetDirectoryFilesByExtensions(DirectoryInfo directory, params string[] extensions)
{
	if((directory == null || !directory.Exists) || (extensions == null || extensions.Length == 0)) return Enumerable.Empty<FileInfo>();
	return directory.EnumerateFiles().Where(f => extensions.Contains(f.Extension));
}

private void CreateDirectoryIfNotExists(string path){
	if(!System.IO.Directory.Exists(path)){
		Console.WriteLine("Create Directory : {0}", path);
		System.IO.Directory.CreateDirectory(path);
	}
}
private void deleteFileIfExists(string path){
	if(System.IO.File.Exists(path)){
		Console.WriteLine("Delete File : {0}", path);
		System.IO.File.Delete(path);
	}
}
private void createFileIfNotExists(string path, string content){
	if(!System.IO.File.Exists(path)){
		Console.WriteLine("Create File : {0}", path);
		System.IO.File.WriteAllText(path, content);
	}
}
private void replaceFile(string path, string content){
	deleteFileIfExists(path);
	System.IO.File.WriteAllText(path, content);
}
private String templateLibaryClass(string name, string createdBy, string dateCreated){
	StringBuilder sb = new StringBuilder();
	name = SplitCamelCase(name);
	string className = name.Replace(" ", String.Empty);
	string variableName = LowerFirstLetter(className);
	sb.AppendLine("using System;");
	sb.AppendLine("using System.Collections.Generic;");
	sb.AppendLine("using System.Linq;");
	sb.AppendLine("using System.Text;");
	sb.AppendLine("using System.Threading.Tasks;");
	sb.AppendLine("");
	sb.AppendLine("namespace lib");
	sb.AppendLine("{");
	sb.AppendLine("    /// <summary>");
	sb.AppendLine(String.Format("    /// Defines the structure (properties, methods, etc.) and syntax for the {0} Design Pattern", name));
	sb.AppendLine("    /// </summary>");
	sb.AppendLine(String.Format("    public interface I{0}", className));
	sb.AppendLine("    {");
	sb.AppendLine("    }");
	sb.AppendLine("    /// <summary>");
	sb.AppendLine(String.Format("    /// Implements the structure (properties, methods, etc.) and syntax for the {0} Design Pattern", name));
	sb.AppendLine("    /// </summary>");
	sb.AppendLine(String.Format("    public class {0}: I{0}", className));
	sb.AppendLine("    {");
	sb.AppendLine("    }");
	sb.AppendLine("}");
	sb.AppendLine("");
	return sb.ToString();
}
private String templateMSTestClass(string name, string createdBy, string dateCreated){
	StringBuilder sb = new StringBuilder();
	name = SplitCamelCase(name);
	string className = name.Replace(" ", String.Empty);
	string variableName = LowerFirstLetter(className);
	sb.AppendLine("using lib;");
	sb.AppendLine("using Microsoft.VisualStudio.TestTools.UnitTesting;");
	sb.AppendLine("");
	sb.AppendLine("namespace ms_test_working_with_design_patterns");
	sb.AppendLine("{");
	sb.AppendLine("    /// <summary>");
	sb.AppendLine(String.Format("    /// Defines, sets up and implements the MS (Microsoft) test(s) for the {0} Design Pattern", name));
	sb.AppendLine("    /// </summary>");
	sb.AppendLine("    [TestClass]");
	sb.AppendLine(String.Format("    public class MSTests{0}DesignPattern", className));
	sb.AppendLine("    {");
	sb.AppendLine(String.Format("        private I{0} {1};", className, variableName));
	sb.AppendLine("");
	sb.AppendLine("        [TestMethod]");
	sb.AppendLine("        public void TestMethod1()");
	sb.AppendLine("        {");
	sb.AppendLine("        }");
	sb.AppendLine("    }");
	sb.AppendLine("}");
	sb.AppendLine("");
	return sb.ToString();
}
private String templateNUnitTestClass(string name, string createdBy, string dateCreated){
	StringBuilder sb = new StringBuilder();
	name = SplitCamelCase(name);
	string className = name.Replace(" ", String.Empty);
	string variableName = LowerFirstLetter(className);
	sb.AppendLine("using lib;");
	sb.AppendLine("using NUnit.Framework;");
	sb.AppendLine("");
	sb.AppendLine("namespace nunit_working_with_design_patterns");
	sb.AppendLine("{");
	sb.AppendLine("    /// <summary>");
	sb.AppendLine(String.Format("    /// Defines, sets up and implements the NUnit test(s) for the {0} Design Pattern", name));
	sb.AppendLine("    /// </summary>");
	sb.AppendLine(String.Format("    public class NUnitTests{0}DesignPattern", className));
	sb.AppendLine("    {");
	sb.AppendLine(String.Format("        private I{0} {1};", className, variableName));
	sb.AppendLine("");
	sb.AppendLine("        [SetUp]");
	sb.AppendLine("        public void Setup()");
	sb.AppendLine("        {");
	sb.AppendLine("        }");
	sb.AppendLine("");
	sb.AppendLine("        [Test]");
	sb.AppendLine("        public void Test1()");
	sb.AppendLine("        {");
	sb.AppendLine("            Assert.Pass();");
	sb.AppendLine("        }");
	sb.AppendLine("    }");
	sb.AppendLine("}");
	sb.AppendLine("");
	return sb.ToString();
}
private String templateNUnitMoqTestClass(string name, string createdBy, string dateCreated){
	StringBuilder sb = new StringBuilder();
	name = SplitCamelCase(name);
	string className = name.Replace(" ", String.Empty);
	string variableName = LowerFirstLetter(className);
	sb.AppendLine("using lib;");
	sb.AppendLine("using Moq;");
	sb.AppendLine("using NUnit.Framework;");
	sb.AppendLine("");
	sb.AppendLine("namespace nunitmoq_working_with_design_patterns");
	sb.AppendLine("{");
	sb.AppendLine("    /// <summary>");
	sb.AppendLine(String.Format("    /// Defines, sets up and implements the NUnit test(s) for the {0} Design Pattern", name));
	sb.AppendLine("    /// </summary>");
	sb.AppendLine(String.Format("    public class NUnitTests{0}DesignPattern", className));
	sb.AppendLine("    {");
	sb.AppendLine(String.Format("        private Mock<I{0}> {1};", className, variableName));
	sb.AppendLine("");
	sb.AppendLine("        [SetUp]");
	sb.AppendLine("        public void Setup()");
	sb.AppendLine("        {");
	sb.AppendLine("        }");
	sb.AppendLine("");
	sb.AppendLine("        [Test]");
	sb.AppendLine("        public void Test1()");
	sb.AppendLine("        {");
	sb.AppendLine("            Assert.Pass();");
	sb.AppendLine("        }");
	sb.AppendLine("    }");
	sb.AppendLine("}");
	sb.AppendLine("");
	return sb.ToString();
}
private void CreateMSSolution(string directory){
	try{
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
		startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
		startInfo.FileName = "dotnet.exe";
		startInfo.Arguments = String.Format("dotnet new sln --output {0}", directory);
		process.StartInfo = startInfo;
		process.Start();
	}catch(Exception exception){
	}
}
private void CreateMSConsoleProject(string directory, string framework = "5.0"){
	try{
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
		startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
		startInfo.FileName = "dotnet.exe";
		startInfo.Arguments = String.Format("dotnet new console --framework net{0} --output {1}", framework, directory);
		process.StartInfo = startInfo;
		process.Start();
	}catch(Exception exception){
	}
}
private void CreateMSTestProject(string name, string directory, string framework = "5.0"){
	try{
	String.Format("dotnet new mstest -–name {0} -–no-restore --output {2}", name, framework, directory).Dump();
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
		startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
		startInfo.FileName = "dotnet.exe";
		startInfo.Arguments = String.Format("dotnet new mstest -–name {0} -–no-restore --output {2}", name, framework, directory);
		process.StartInfo = startInfo;
		process.Start();
	}catch(Exception exception){
	}
}
private void CreateNUnitTestProject(string directory, string framework = "5.0"){
	try{
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
		startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
		startInfo.FileName = "dotnet.exe";
		startInfo.Arguments = String.Format("dotnet new console --framework net{0} --output {1}", framework, directory);
		process.StartInfo = startInfo;
		process.Start();
	}catch(Exception exception){
	}
}
private void CreateMSAddProjectToSolution(string solution, string project){
	try{
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
		startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
		startInfo.FileName = "dotnet.exe";
		startInfo.Arguments = String.Format("dotnet sln {0} add {1}", solution, project);
		process.StartInfo = startInfo;
		process.Start();
	}catch(Exception exception){
	}
}
private void CreateMSAddMultipleProjectsToSolution(string solution, string[] projects){
	try{
		System.Diagnostics.Process process = new System.Diagnostics.Process();
		System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
		startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
		startInfo.FileName = "dotnet.exe";
		startInfo.Arguments = String.Format("dotnet sln {0} add {1}", solution, String.Join(" ", projects.Select(project => String.Format("\"{0}\"", project))));
		process.StartInfo = startInfo;
		process.Start();
	}catch(Exception exception){
	}
}
public class DesignPattern {
	public string Type;
	public string Name;
	public string ProjectName;
	public string ClassName;
	public string Description;
	public string MsTestProjectName;
	public string NUnitTestProjectName;
	public string NUnitMoqTestProjectName;
	public string MsTestClassName;
	public string NUnitTestClassName;
	public DesignPattern(string type, string name, string description){
		this.Type = type;
		this.Name = String.Format("{0} Design Pattern", name);
		this.ProjectName = String.Format("{0}-Design-Pattern", name.Replace(" ", "-").Trim()).ToLower().Trim();
		this.ClassName = String.Format("{0}DesignPattern", name.Replace(" ", "").Trim());
		this.Description = description;
		this.MsTestProjectName = String.Format("mstest-{0}-Design-Pattern", name.Replace(" ", "-").Trim()).ToLower().Trim();
		this.NUnitTestProjectName = String.Format("nunitest-{0}-Design-Pattern", name.Replace(" ", "-").Trim()).ToLower().Trim();
		this.NUnitMoqTestProjectName = String.Format("nunimoqtest-{0}-Design-Pattern", name.Replace(" ", "-").Trim()).ToLower().Trim();
		this.MsTestClassName = String.Format("MSTest{0}DesignPattern", name.Replace(" ", "").Trim());
		this.NUnitTestClassName = String.Format("NUnitTest{0}DesignPattern", name.Replace(" ", "").Trim());
	}
}