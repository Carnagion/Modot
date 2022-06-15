using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

using Godot.Serialization;
using Godot.Utility.Extensions;

using JetBrains.Annotations;

namespace Godot.Modding
{
    public sealed record Mod
    {
        public Mod(Metadata metadata)
        {
            this.Meta = metadata;
            this.Assemblies = this.LoadAssemblies();
            this.Data = this.LoadData();
            this.LoadResources();
        }
        
        public Metadata Meta
        {
            get;
        }
        
        public IEnumerable<Assembly> Assemblies
        {
            get;
        }
        
        public XmlNode? Data
        {
            get;
        }
        
        private IEnumerable<Assembly> LoadAssemblies()
        {
            string assembliesPath = $"{this.Meta.Directory}/Assemblies";
            
            using Directory directory = new();
            return directory.DirExists(assembliesPath)
                ? from assemblyPath in directory.GetFiles(assembliesPath, true)
                  where assemblyPath.EndsWith(".dll")
                  select Assembly.LoadFile(assemblyPath)
                : Enumerable.Empty<Assembly>();
        }
        
        private XmlNode? LoadData()
        {
            IEnumerable<XmlDocument> documents = this.LoadDocuments().ToArray();
            if (!documents.Any())
            {
                return null;
            }
            
            XmlDocument data = new();
            data.InsertBefore(data.CreateXmlDeclaration("1.0", "UTF-8", null), data.DocumentElement);
            (from document in documents
             from node in document.Cast<XmlNode>()
             where node.NodeType is not XmlNodeType.XmlDeclaration
             select node).ForEach(node => data.AppendChild(node));
            return data;
        }
        
        private IEnumerable<XmlDocument> LoadDocuments()
        {
            string dataPath = $"{this.Meta.Directory}/Data";
            
            using Directory directory = new();
            if (!directory.DirExists(dataPath))
            {
                yield break;
            }
            
            foreach (string xmlPath in directory.GetFiles(dataPath, true))
            {
                XmlDocument document = new();
                document.Load(xmlPath);
                yield return document;
            }
        }
        
        private void LoadResources()
        {
            string resourcesPath = $"{this.Meta.Directory}/Resources";
            
            using Directory directory = new();
            if (!directory.DirExists(resourcesPath))
            {
                return;
            }
            
            foreach (string resourcePath in from path in directory.GetFiles(resourcesPath, true)
                                            where path.EndsWith(".pck")
                                            select path)
            {
                if (!ProjectSettings.LoadResourcePack(resourcePath))
                {
                    throw new ModLoadException(this.Meta.Directory, $"Error loading resource pack at {resourcePath}");
                }
            }
        }
        
        public sealed record Metadata
        {
            [UsedImplicitly]
            private Metadata()
            {
            }
            
            [Serialize]
            public string Directory
            {
                get;
                [UsedImplicitly]
                private set;
            } = null!;
            
            [Serialize]
            public string Id
            {
                get;
                [UsedImplicitly]
                private set;
            } = null!;
            
            [Serialize]
            public string Name
            {
                get;
                [UsedImplicitly]
                private set;
            } = null!;
            
            [Serialize]
            public string Author
            {
                get;
                [UsedImplicitly]
                private set;
            } = null!;
            
            public IEnumerable<string> Dependencies
            {
                get;
                [UsedImplicitly]
                private set;
            } = Enumerable.Empty<string>();
            
            public IEnumerable<string> Before
            {
                get;
                [UsedImplicitly]
                private set;
            } = Enumerable.Empty<string>();
            
            public IEnumerable<string> After
            {
                get;
                [UsedImplicitly]
                private set;
            } = Enumerable.Empty<string>();
            
            public IEnumerable<string> Incompatible
            {
                get;
                [UsedImplicitly]
                private set;
            } = Enumerable.Empty<string>();
            
            public static Metadata Load(string directoryPath)
            {
                try
                {
                    string metadataFilePath = $"{directoryPath}/Mod.xml";
                    
                    using File file = new();
                    if (!file.FileExists(metadataFilePath))
                    {
                        throw new ModLoadException(directoryPath, new FileNotFoundException($"Mod metadata file {metadataFilePath} does not exist"));
                    }
                    
                    XmlDocument document = new();
                    document.Load(metadataFilePath);
                    if (document.DocumentElement?.Name is not "Mod")
                    {
                        throw new ModLoadException(directoryPath, "Root XML node \"Mod\" for serializing mod metadata does not exist");
                    }
                    
                    XmlNode directoryNode = document.CreateNode(XmlNodeType.Element, "Directory", null);
                    directoryNode.InnerText = directoryPath;
                    document.DocumentElement.AppendChild(directoryNode);
                    
                    Metadata metadata = new Serializer().Deserialize<Metadata>(document.DocumentElement)!;
                    return metadata.IsValid() ? metadata : throw new ModLoadException(directoryPath, "Invalid metadata");
                }
                catch (Exception exception) when (exception is not ModLoadException)
                {
                    throw new ModLoadException(directoryPath, exception);
                }
            }
            
            private bool IsValid()
            {
                // Check that the incompatible, load before, and load after lists don't have anything in common or contain the mod's own ID
                bool invalidLoadOrder = this.Id.Yield()
                    .Concat(this.Incompatible)
                    .Concat(this.Before)
                    .Concat(this.After)
                    .Indistinct()
                    .Any();
                // Check that the dependency and incompatible lists don't have anything in common
                bool invalidDependencies = this.Dependencies
                    .Intersect(this.Incompatible)
                    .Any();
                return !(invalidLoadOrder || invalidDependencies);
            }
        }
    }
}