#Template v1.0.0
#--------------------------------------
# Dependencies
#--------------------------------------
require 'albacore'
#--------------------------------------
# Debug
#--------------------------------------
#ENV.each {|key, value| puts "#{key} = #{value}" }
#--------------------------------------
# Environment vars
#--------------------------------------
@env_solutionname = 'SisoDb-Provider'
@env_solutionfolderpath = "../Source"

@env_projectnameCore = 'SisoDb'
@env_projectnameAzure = 'SisoDb.Azure'
@env_projectnameSql2005 = 'SisoDb.Sql2005'
@env_projectnameSql2008 = 'SisoDb.Sql2008'
@env_projectnameSql2012 = 'SisoDb.Sql2012'
@env_projectnameSqlCe4 = 'SisoDb.SqlCe4'
@env_projectnameMsMemoryCache = 'SisoDb.MsMemoryCache'
@env_projectnameDynamic = 'SisoDb.Dynamic'
@env_projectnameGlimpse = 'SisoDb.Glimpse'
@env_projectnameMiniProfiler = 'SisoDb.MiniProfiler'
@env_projectnameJsonNet = 'SisoDb.JsonNet'
@env_projectnameServiceStack = 'SisoDb.ServiceStack'
@env_projectnameSpatial = 'SisoDb.Spatial'

@env_buildfolderpath = 'build'
@env_assversion = "17.0.1"
@env_version = "#{@env_assversion}"
@env_buildversion = @env_version + (ENV['env_buildnumber'].to_s.empty? ? "" : ".#{ENV['env_buildnumber'].to_s}")
@env_buildconfigname = ENV['env_buildconfigname'].to_s.empty? ? "Release" : ENV['env_buildconfigname'].to_s

buildNameSuffix = "-v#{@env_buildversion}-#{@env_buildconfigname}"
@env_buildnameCore = "#{@env_projectnameCore}#{buildNameSuffix}"
@env_buildnameAzure = "#{@env_projectnameAzure}#{buildNameSuffix}"
@env_buildnameSql2005 = "#{@env_projectnameSql2005}#{buildNameSuffix}"
@env_buildnameSql2008 = "#{@env_projectnameSql2008}#{buildNameSuffix}"
@env_buildnameSql2012 = "#{@env_projectnameSql2012}#{buildNameSuffix}"
@env_buildnameSqlCe4 = "#{@env_projectnameSqlCe4}#{buildNameSuffix}"
@env_buildnameMsMemoryCache = "#{@env_projectnameMsMemoryCache}#{buildNameSuffix}"
@env_buildnameDynamic = "#{@env_projectnameDynamic}#{buildNameSuffix}"
@env_buildnameGlimpse = "#{@env_projectnameGlimpse}#{buildNameSuffix}"
@env_buildnameMiniProfiler = "#{@env_projectnameMiniProfiler}#{buildNameSuffix}"
@env_buildnameJsonNet = "#{@env_projectnameJsonNet}#{buildNameSuffix}"
@env_buildnameServiceStack = "#{@env_projectnameServiceStack}#{buildNameSuffix}"
@env_buildnameSpatial = "#{@env_projectnameSpatial}#{buildNameSuffix}"
#--------------------------------------
# Reusable vars
#--------------------------------------
coreOutputPath = "#{@env_buildfolderpath}/#{@env_projectnameCore}"
azureOutputPath = "#{@env_buildfolderpath}/#{@env_projectnameAzure}"
sql2005OutputPath = "#{@env_buildfolderpath}/#{@env_projectnameSql2005}"
sql2008OutputPath = "#{@env_buildfolderpath}/#{@env_projectnameSql2008}"
sql2012OutputPath = "#{@env_buildfolderpath}/#{@env_projectnameSql2012}"
sqlCe4OutputPath = "#{@env_buildfolderpath}/#{@env_projectnameSqlCe4}"
msMemoryCacheOutputPath = "#{@env_buildfolderpath}/#{@env_projectnameMsMemoryCache}"
dynamicOutputPath = "#{@env_buildfolderpath}/#{@env_projectnameDynamic}"
glimpseOutputPath = "#{@env_buildfolderpath}/#{@env_projectnameGlimpse}"
miniProfilerOutputPath = "#{@env_buildfolderpath}/#{@env_projectnameMiniProfiler}"
jsonNetOutputPath = "#{@env_buildfolderpath}/#{@env_projectnameJsonNet}"
serviceStackOutputPath = "#{@env_buildfolderpath}/#{@env_projectnameServiceStack}"
spatialOutputPath = "#{@env_buildfolderpath}/#{@env_projectnameSpatial}"
sharedAssemblyInfoPath = "#{@env_solutionfolderpath}/SharedAssemblyInfo.cs"
#--------------------------------------
# Albacore flow controlling tasks
#--------------------------------------
task :ci => [:installNuGets, :buildIt, :copyIt, :testIt, :zipIt, :packIt]

task :local => [:cleanIt, :versionIt, :buildIt, :copyIt, :testIt, :zipIt, :packIt]

task :buildandpackage => [:cleanIt, :versionIt, :buildIt, :copyIt, :zipIt, :packIt]
#--------------------------------------
task :copyIt => [:copyCore, :copyAzure, :copySql2005, :copySql2008, :copySql2012, :copySqlCe4, :copyMsMemoryCache, :copyDynamic, :copyGlimpse, :copyMiniProfiler, :copyJsonNet, :copyServiceStack, :copySpatial]

task :testIt => [:unittests]

task :zipIt => [:zipCore, :zipAzure, :zipSql2005, :zipSql2008, :zipSql2012, :zipSqlCe4, :zipMsMemoryCache, :zipDynamic, :zipGlimpse, :zipMiniProfiler, :zipJsonNet, :zipServiceStack, :zipSpatial]

task :packIt => [:packCore, :packAzure, :packSql2005, :packSql2008, :packSql2012, :packSqlCe4, :packMsMemoryCache, :packDynamic, :packGlimpse, :packMiniProfiler, :packJsonNet, :packServiceStack, :packSpatial]
#--------------------------------------
# Albacore tasks
#--------------------------------------
task :installNuGets do
    FileList["#{@env_solutionfolderpath}/**/packages.config"].each { |filepath|
        sh "NuGet.exe i #{filepath} -o #{@env_solutionfolderpath}/packages"
    }
end

task :cleanIt do
	FileUtils.rm_rf(@env_buildfolderpath)
	FileUtils.mkdir_p(@env_buildfolderpath)
end

assemblyinfo :versionIt do |asm|
    asm.input_file = sharedAssemblyInfoPath
    asm.output_file = sharedAssemblyInfoPath
    asm.version = "#{@env_assversion}.*"
    asm.file_version = @env_buildversion
end

msbuild :buildIt do |msb|
    msb.properties :configuration => @env_buildconfigname
    msb.targets :Clean, :Build
    msb.solution = "#{@env_solutionfolderpath}/#{@env_solutionname}.sln"
end

def copyProject(projectName, outputPath)
    FileUtils.mkdir_p(outputPath)
    FileUtils.cp_r(FileList["#{@env_solutionfolderpath}/Projects/#{projectName}/bin/#{@env_buildconfigname}/**"], outputPath)
end

task :copyCore do
    copyProject(@env_projectnameCore, coreOutputPath)
end

task :copyAzure do
    copyProject(@env_projectnameAzure, azureOutputPath)
end

task :copySql2005 do
    copyProject(@env_projectnameSql2005, sql2005OutputPath)
end

task :copySql2008 do
    copyProject(@env_projectnameSql2008, sql2008OutputPath)
end

task :copySql2012 do
    copyProject(@env_projectnameSql2012, sql2012OutputPath)
end

task :copySqlCe4 do
    copyProject(@env_projectnameSqlCe4, sqlCe4OutputPath)
end

task :copyMsMemoryCache do
    copyProject(@env_projectnameMsMemoryCache, msMemoryCacheOutputPath)
end

task :copyDynamic do
    copyProject(@env_projectnameDynamic, dynamicOutputPath)
end

task :copyGlimpse do
    copyProject(@env_projectnameGlimpse, glimpseOutputPath)
end

task :copyMiniProfiler do
    copyProject(@env_projectnameMiniProfiler, miniProfilerOutputPath)
end

task :copyJsonNet do
    copyProject(@env_projectnameJsonNet, jsonNetOutputPath)
end

task :copyServiceStack do
    copyProject(@env_projectnameServiceStack, serviceStackOutputPath)
end

task :copySpatial do
    copyProject(@env_projectnameSpatial, spatialOutputPath)
end

nunit :unittests do |nunit|
    nunit.command = "nunit-console.exe"
    nunit.options "/framework=v4.0.30319","/xml=#{@env_buildfolderpath}/NUnit-Report-#{@env_solutionname}-UnitTests.xml"
    nunit.assemblies FileList["#{@env_solutionfolderpath}/Tests/#{@env_projectnameCore}.**UnitTests/bin/#{@env_buildconfigname}/#{@env_projectnameCore}.**UnitTests.dll"]
end

def zipProject(zip, outputPath, buildName)
    zip.directories_to_zip outputPath
    zip.output_file = "#{buildName}.zip"
    zip.output_path = @env_buildfolderpath
end

zip :zipCore do |zip|
    zipProject(zip, coreOutputPath, @env_buildnameCore)
end

zip :zipAzure do |zip|
    zipProject(zip, azureOutputPath, @env_buildnameAzure)
end

zip :zipSql2005 do |zip|
    zipProject(zip, sql2005OutputPath, @env_buildnameSql2005)
end

zip :zipSql2008 do |zip|
    zipProject(zip, sql2008OutputPath, @env_buildnameSql2008)
end

zip :zipSql2012 do |zip|
    zipProject(zip, sql2012OutputPath, @env_buildnameSql2012)
end

zip :zipSqlCe4 do |zip|
    zipProject(zip, sqlCe4OutputPath, @env_buildnameSqlCe4)
end

zip :zipMsMemoryCache do |zip|
    zipProject(zip, msMemoryCacheOutputPath, @env_buildnameMsMemoryCache)
end

zip :zipDynamic do |zip|
    zipProject(zip, dynamicOutputPath, @env_buildnameDynamic)
end

zip :zipGlimpse do |zip|
    zipProject(zip, glimpseOutputPath, @env_buildnameGlimpse)
end

zip :zipMiniProfiler do |zip|
    zipProject(zip, miniProfilerOutputPath, @env_buildnameMiniProfiler)
end

zip :zipJsonNet do |zip|
    zipProject(zip, jsonNetOutputPath, @env_buildnameJsonNet)
end

zip :zipServiceStack do |zip|
    zipProject(zip, serviceStackOutputPath, @env_buildnameServiceStack)
end

zip :zipSpatial do |zip|
    zipProject(zip, spatialOutputPath, @env_buildnameSpatial)
end

def packProject(cmd, projectname, basepath)
    cmd.command = "#{@env_solutionfolderpath}/.nuget/NuGet.exe"
    cmd.parameters = "pack #{projectname}.nuspec -version #{@env_version} -basepath #{basepath} -outputdirectory #{@env_buildfolderpath}"
end

exec :packCore do |cmd|
    packProject(cmd, @env_projectnameCore, coreOutputPath)
end

exec :packAzure do |cmd|
    packProject(cmd, @env_projectnameAzure, azureOutputPath)
end

exec :packSql2005 do |cmd|
    packProject(cmd, @env_projectnameSql2005, sql2005OutputPath)
end

exec :packSql2008 do |cmd|
    packProject(cmd, @env_projectnameSql2008, sql2008OutputPath)
end

exec :packSql2012 do |cmd|
    packProject(cmd, @env_projectnameSql2012, sql2012OutputPath)
end

exec :packSqlCe4 do |cmd|
    packProject(cmd, @env_projectnameSqlCe4, sqlCe4OutputPath)
end

exec :packMsMemoryCache do |cmd|
    packProject(cmd, @env_projectnameMsMemoryCache, msMemoryCacheOutputPath)
end

exec :packDynamic do |cmd|
    packProject(cmd, @env_projectnameDynamic, dynamicOutputPath)
end

exec :packGlimpse do |cmd|
    packProject(cmd, @env_projectnameGlimpse, glimpseOutputPath)
end

exec :packMiniProfiler do |cmd|
    packProject(cmd, @env_projectnameMiniProfiler, miniProfilerOutputPath)
end

exec :packJsonNet do |cmd|
    packProject(cmd, @env_projectnameJsonNet, jsonNetOutputPath)
end

exec :packServiceStack do |cmd|
    packProject(cmd, @env_projectnameServiceStack, serviceStackOutputPath)
end

exec :packSpatial do |cmd|
    packProject(cmd, @env_projectnameSpatial, spatialOutputPath)
end