using UnityEngine;
using UnityEditor.PackageManager.Requests;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.VersionControl;
using static System.IO.Path;
using static UnityEditor.AssetDatabase;
using Task = System.Threading.Tasks.Task;

public static class ProjectSetup
{
        [MenuItem("Tools/Setup/Import Essential Assets", priority = 1)]
        static void ImportEssentials()
        {
               Assets.ImportAsset("Odin Inspector and Serializer.package","Plugins/Editor ExtentionsSystem");
                Assets.ImportAsset("DOTween HOTween v2","Demigiant/Editor ExtentionsAnimation");
                Assets.ImportAsset("Feel","More Mountains/Editor ExtentionsEffects");
                Assets.ImportAsset("Audio Preview Tool","Warped Imagination/Editor ExtentionsAudio");
                Assets.ImportAsset("Better Hierarchy","Toaster Head/Editor ExtentionsUtility");
        }

        [MenuItem("Tools/Setup/Import Essential Packages", priority = 2)]
        static void InstallPackages()
        {
                
        }
        
        [MenuItem("Tools/Setup/Create Folders")]
        public static void CreateFolders() {
                Folders.Create("_Project", "Animation", "Art", "Materials", "Prefabs", "Scripts/Tests", "Scripts/Tests/Editor", "Scripts/Tests/Runtime");
                Refresh();
                Folders.Move("_Project", "Scenes");
                Folders.Move("_Project", "Settings");
                Folders.Delete("TutorialInfo");
                Refresh();
                
        }
        
        
        static class Assets
        {
                public static void ImportAsset(string asset, string folder)
                {
                        string basePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
                        string assetFolder = Combine(basePath, "Unity/Asset Store-5.x");   
                        
                        ImportPackage(Combine(assetFolder,folder,asset), true);
                }
        }

        static class Packages
        {
                private static AddRequest _request;
                static Queue<string> _packages = new Queue<string>();

                public static void InstallPackages(string[] packages)
                {
                        foreach (var package in packages)
                        {
                                _packages.Enqueue(package);
                        }

                        if (_packages.Count > 0)
                        {
                                StartNextPackageInstallationAsync();
                        }
                }

                private static async void StartNextPackageInstallationAsync()
                {
                        _request = Client.Add(_packages.Dequeue());
                        
                        while(!_request.IsCompleted) await Task.Delay(10);
                        
                        if(_request.Status == StatusCode.Success) Debug.Log("Installed: " + _request.Result.packageId);
                        else if (_request.Status >= StatusCode.Failure) Debug.LogError(_request.Error.message);

                        if (_packages.Count > 0)
                        {
                                await Task.Delay(1000);
                                StartNextPackageInstallationAsync();
                        }
                }
        }

        static class Folders
        {
                public static void Create(string root, params string[] folders) {
                        var fullpath = Combine(Application.dataPath, root);
                        if (!Directory.Exists(fullpath)) {
                                Directory.CreateDirectory(fullpath);
                        }

                        foreach (var folder in folders) {
                                CreateSubFolders(fullpath, folder);
                        }
                }
        
                static void CreateSubFolders(string rootPath, string folderHierarchy) {
                        var folders = folderHierarchy.Split('/');
                        var currentPath = rootPath;

                        foreach (var folder in folders) {
                                currentPath = Combine(currentPath, folder);
                                if (!Directory.Exists(currentPath)) {
                                        Directory.CreateDirectory(currentPath);
                                }
                        }
                }
        
                public static void Move(string newParent, string folderName) {
                        var sourcePath = $"Assets/{folderName}";
                        if (IsValidFolder(sourcePath)) {
                                var destinationPath = $"Assets/{newParent}/{folderName}";
                                var error = MoveAsset(sourcePath, destinationPath);

                                if (!string.IsNullOrEmpty(error)) {
                                        Debug.LogError($"Failed to move {folderName}: {error}");
                                }
                        }
                }
        
                public static void Delete(string folderName) {
                        var pathToDelete = $"Assets/{folderName}";

                        if (IsValidFolder(pathToDelete)) {
                                DeleteAsset(pathToDelete);
                        }
                }
        }
}
